/*
This sample shows how to perform Multi-Camera calibration.
*/

using System;
using System.Collections.Generic;

using Zivid.NET.Calibration;
using Duration = Zivid.NET.Duration;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Finding cameras");
            var zivid = new Zivid.NET.Application();
            var cameras = zivid.Cameras;
            Console.WriteLine("Number of cameras found: {0}", cameras.Count);
            foreach (var camera in cameras)
            {
                Console.WriteLine("Connecting to camera: {0}", camera.Info.SerialNumber);
                camera.Connect();
            }

            var detectionResults = new List<DetectionResult>();
            foreach (var camera in cameras)
            {
                Console.WriteLine("Capturing frame with camera: {0}", camera.Info.SerialNumber);
                using (var frame = AssistedCapture(camera))
                {
                    Console.WriteLine("Detecting checkerboard in point cloud");
                    var detectionResult = Detector.DetectFeaturePoints(frame.PointCloud);
                    if (detectionResult)
                    {
                        detectionResults.Add(detectionResult);
                    }
                    else
                    {
                        throw new System.InvalidOperationException(
                            "Could not detect checkerboard. Please ensure it is visible from all cameras.");
                    }
                }
            }

            Console.WriteLine("Performing Multi-camera calibration");
            var result = Calibrator.CalibrateMultiCamera(detectionResults);
            if (result)
            {
                Console.WriteLine("Multi-camera calibration OK.");
                var transforms = result.Transforms();
                var residuals = result.Residuals();
                for (int i = 0; i < transforms.Length; i++)
                {
                    PrintMatrix(transforms[i]);
                    Console.WriteLine(residuals[i]
                                          .ToString());
                }
            }
            else
            {
                Console.WriteLine("Multi-camera calibration FAILED.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
            Environment.ExitCode = 1;
        }
    }

    static Zivid.NET.Frame AssistedCapture(Zivid.NET.Camera camera)
    {
        var suggestSettingsParameters = new Zivid.NET.CaptureAssistant.SuggestSettingsParameters
        {
            AmbientLightFrequency =
                Zivid.NET.CaptureAssistant.SuggestSettingsParameters.AmbientLightFrequencyOption.none,
            MaxCaptureTime = Duration.FromMilliseconds(800)
        };
        var settings = Zivid.NET.CaptureAssistant.Assistant.SuggestSettings(camera, suggestSettingsParameters);
        return camera.Capture(settings);
    }

    static void PrintMatrix(float[,] matrix)
    {
        var lineSep = new String('-', 50);
        Console.WriteLine(lineSep);
        for (int j = 0; j < matrix.GetLength(0); j++)
        {
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                Console.Write("{0,10:0.0000} ", matrix[j, i]);
            }
            Console.WriteLine();
        }
        Console.WriteLine(lineSep);
    }
}
