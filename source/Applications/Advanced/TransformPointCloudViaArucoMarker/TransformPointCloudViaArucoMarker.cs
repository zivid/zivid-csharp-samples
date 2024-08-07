/*
Transform a point cloud from camera to ArUco marker coordinate frame by estimating the marker's pose from the point cloud.

The ZDF file for this sample can be found under the main instructions for Zivid samples.
*/

using System;
using Zivid.NET.Calibration;
using System.Collections.Generic;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var calibrationBoardFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                           + "/Zivid/CalibrationBoardInCameraOrigin.zdf";
            Console.WriteLine("Reading ZDF frame from file: " + calibrationBoardFile);
            var frame = new Zivid.NET.Frame(calibrationBoardFile);
            var pointCloud = frame.PointCloud;

            Console.WriteLine("Configuring ArUco marker");
            var markerDictionary = Zivid.NET.MarkerDictionary.Aruco4x4_50;
            var markerId = new List<int> { 1 };

            Console.WriteLine("Detecting ArUco marker");
            var detectionResult = Detector.DetectMarkers(frame, markerId, markerDictionary);

            Console.WriteLine("Estimating pose of detected ArUco marker");
            var cameraToMarkerTransform = new Zivid.NET.Matrix4x4(detectionResult.DetectedMarkers()[0].Pose().ToMatrix());
            Console.WriteLine("ArUco marker pose in camera frame:");
            Console.WriteLine(cameraToMarkerTransform);
            Console.WriteLine("Camera pose in ArUco marker frame:");
            var markerToCameraTransform = cameraToMarkerTransform.Inverse();
            Console.WriteLine(markerToCameraTransform);

            var transformFile = "ArUcoMarkerToCameraTransform.yaml";
            Console.WriteLine("Saving a YAML file with Inverted ArUco marker pose to file: " + transformFile);
            markerToCameraTransform.Save(transformFile);

            Console.WriteLine("Transforming point cloud from camera frame to ArUco marker frame");
            pointCloud.Transform(markerToCameraTransform);

            var arucoMarkerTransformedFile = "CalibrationBoardInArucoMarkerOrigin.zdf";
            Console.WriteLine("Saving transformed point cloud to file: " + arucoMarkerTransformedFile);
            frame.Save(arucoMarkerTransformedFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
