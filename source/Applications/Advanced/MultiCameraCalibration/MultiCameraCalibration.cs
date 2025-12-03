/*
Use captures of a calibration object to generate transformation matrices to a single coordinate frame.
*/

using System;
using System.Collections.Generic;
using System.Net;
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
                using (var frame = Detector.CaptureCalibrationBoard(camera))
                {
                    Console.WriteLine("Detecting checkerboard in point cloud");
                    var detectionResult = Detector.DetectCalibrationBoard(frame);
                    if (detectionResult.Valid())
                    {
                        detectionResults.Add(detectionResult);
                        serialNumbers.Add(serialNumber);
                    }
                    else
                    {
                        throw new System.InvalidOperationException(
                            "Could not detect checkerboard. Please ensure it is visible from all cameras. " + detectionResult.StatusDescription());
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

    public struct Detection
    {
        public string serialNumber { get; }
        public DetectionResult detectionResult { get; }

        public Detection(string serial, DetectionResult result)
        {
            serialNumber = serial;
            detectionResult = result;
        }
    }

    static List<Detection> GetDetections(IList<Zivid.NET.Camera> connectedCameras)
    {
        var detectionList = new List<Detection>();
        foreach (var camera in connectedCameras)
        {
            var serial = camera.Info.SerialNumber.ToString();
            Console.WriteLine("Capturing frame with camera: {0}", serial);

            using (var frame = Detector.CaptureCalibrationBoard(camera))
            {
                Console.WriteLine("Detecting checkerboard in point cloud");
                var detectionResult = Detector.DetectCalibrationBoard(frame);
                if (detectionResult.Valid())
                {
                    var currenDetection = new Detection(serial, detectionResult);
                    detectionList.Add(currenDetection);
                }
                else
                {
                    throw new System.InvalidOperationException(
                        "Could not detect checkerboard. Please ensure it is visible from all cameras. " + detectionResult.StatusDescription());
                }
            }
        }

        return detectionList;
    }

    static void RunMultiCameraCalibration(List<Detection> detectionsList, string transformationMatricesSavePath)
    {
        var detectionResults = new List<DetectionResult>();
        foreach (var detection in detectionsList)
        {
            detectionResults.Add(detection.detectionResult);
        }

        var results = Calibrator.CalibrateMultiCamera(detectionResults);

        if (results)
        {
            Console.WriteLine("Multi-camera calibration OK.");
            var transforms = results.Transforms();
            var residuals = results.Residuals();
            for (int i = 0; i < transforms.Length; i++)
            {
                new Zivid.NET.Matrix4x4(transforms[i]).Save(transformationMatricesSavePath + "/" + detectionsList[i].serialNumber + ".yaml");

                Console.WriteLine("Pose of camera {0} in first camera {1} frame", detectionsList[i].serialNumber, detectionsList[0].serialNumber);
                PrintMatrix(transforms[i]);

                Console.WriteLine(residuals[i].ToString());
            }
        }
        else
        {
            Console.WriteLine("Multi-camera calibration FAILED.");
        }
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
