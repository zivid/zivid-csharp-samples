/*
Use captures of a calibration object to generate transformation matrices to a single coordinate frame.
*/

using System;
using System.Collections.Generic;

using Zivid.NET.Calibration;
using Duration = Zivid.NET.Duration;

class Program
{
    static int Main()
    {
        try
        {
            Console.WriteLine("Finding cameras");
            var zivid = new Zivid.NET.Application();
            var cameras = zivid.Cameras;
            Console.WriteLine("Number of cameras found: {0}", cameras.Count);
            var connectedCameras = connectToAllAvailableCameras(cameras);

            var detectionResults = new List<DetectionResult>();
            var serialNumbers = new List<string>();
            foreach (var camera in connectedCameras)
            {
                var serialNumber = camera.Info.SerialNumber.ToString();
                Console.WriteLine("Capturing frame with camera: {0}", serialNumber);
                using (var frame = AssistedCapture(camera))
                {
                    Console.WriteLine("Detecting checkerboard in point cloud");
                    var detectionResult = Detector.DetectCalibrationBoard(frame);
                    if (detectionResult)
                    {
                        detectionResults.Add(detectionResult);
                        serialNumbers.Add(serialNumber);
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
                    Console.WriteLine(residuals[i].ToString());
                    new Zivid.NET.Matrix4x4(transforms[i]).Save(serialNumbers[i] + ".yaml");
                }
            }
            else
            {
                Console.WriteLine("Multi-camera calibration FAILED.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.ToString());
            return 1;
        }
        return 0;
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

    static List<Zivid.NET.Camera> connectToAllAvailableCameras(IList<Zivid.NET.Camera> cameras)
    {
        var connectedCameras = new List<Zivid.NET.Camera>();
        foreach (var camera in cameras)
        {
            if (camera.State.Status == Zivid.NET.CameraState.StatusOption.Available)
            {
                Console.WriteLine("Connecting to camera: {0}", camera.Info.SerialNumber);
                camera.Connect();
                connectedCameras.Add(camera);
            }
            else
            {
                Console.WriteLine("Unable to connect to camera: {0}. ", camera.Info.SerialNumber);
                Console.WriteLine("Camera status: {0}.", camera.State.Status);
            }
        }
        return connectedCameras;
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
