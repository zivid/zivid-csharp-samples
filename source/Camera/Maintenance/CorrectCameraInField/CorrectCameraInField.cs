/*
Correct the dimension trueness of a Zivid camera.

This example shows how to perform In-field correction. This involves gathering data from
a compatible calibration board at several distances, calculating an updated camera
correction, and optionally saving that new correction to the camera.

The correction will persist on the camera even though the camera is power-cycled or
connected to a different PC. After saving a correction, it will automatically be used any
time that camera captures a new point cloud.

Note: This example uses experimental SDK features, which may be modified, moved, or deleted in the future without notice.
*/

using System;
using System.Collections.Generic;

class Program
{
    static bool YesNoPrompt(string question)
    {
        while (true)
        {
            Console.WriteLine(question + ": y/n");
            var response = Console.ReadLine();
            if (response == null || response.ToLower() == "n")
            {
                return false;
            }
            if (response.ToLower() == "y")
            {
                return true;
            }
        }
    }

    static List<Zivid.NET.Experimental.Calibration.InfieldCorrectionInput> CollectDataset(Zivid.NET.Camera camera)
    {
        var dataset = new List<Zivid.NET.Experimental.Calibration.InfieldCorrectionInput>();
        Console.WriteLine("Please point the camera at a Zivid infield calibration board.");

        const string printLine = "------------------------------------------------------------------------";
        while (true)
        {
            Console.WriteLine(printLine);
            if (YesNoPrompt("Capture (y) or finish (n)?"))
            {
                Console.WriteLine("Capturing calibration board");
                var detectionResult = Zivid.NET.Calibration.Detector.DetectCalibrationBoard(camera);
                if (detectionResult.Valid())
                {
                    var input = new Zivid.NET.Experimental.Calibration.InfieldCorrectionInput(detectionResult);

                    if (input.Valid)
                    {
                        dataset.Add(input);
                        Console.WriteLine("Valid measurement at: " + input.DetectionResult.Centroid().ToString());
                    }
                    else
                    {
                        Console.WriteLine("****Invalid Input****");
                        Console.WriteLine("Feedback: " + input.StatusDescription());
                    }
                }
                else
                {
                    Console.WriteLine("****Failed Detection****");
                    Console.WriteLine("Feedback: " + detectionResult.StatusDescription());
                }
                Console.WriteLine(printLine);
            }
            else
            {
                Console.WriteLine("End of capturing stage.");
                Console.WriteLine(printLine);
                break;
            }
            Console.WriteLine("You have collected " + dataset.Count + " valid measurements so far.");
        }
        return dataset;
    }

    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            // Gather data
            var dataset = CollectDataset(camera);

            // Calculate infield correction
            Console.WriteLine("Collected " + dataset.Count + " valid measurements.");
            Console.WriteLine("Computing new camera correction...");
            var correction = Zivid.NET.Experimental.Calibration.Calibrator.ComputeCameraCorrection(dataset);
            var accuracyEstimate = correction.AccuracyEstimate;
            Console.WriteLine("If written to the camera, this correction can be expected to yield a dimension accuracy error of "
                    + (accuracyEstimate.DimensionAccuracy * 100).ToString("0.00") + "% or better in the range of z=["
                    + accuracyEstimate.ZMin.ToString("0.00") + "," + accuracyEstimate.ZMax.ToString("0.00")
                    + "] across the full FOV. Accuracy close to where the correction data was collected is likely better.");

            // Optionally save to camera
            if (YesNoPrompt("Save to camera?"))
            {
                Console.WriteLine("Writing camera correction...");
                Zivid.NET.Experimental.Calibration.Calibrator.WriteCameraCorrection(camera, correction);
                Console.WriteLine("Success");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
