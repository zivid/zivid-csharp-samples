/*
Filter the point cloud based on a ROI box given relative to the ArUco marker on a Zivid Calibration Board.

The ZFC file for this sample can be downloaded from https://support.zivid.com/en/latest/api-reference/samples/sample-data.html.
*/

using System;
using System.Collections.Generic;
using Zivid.NET;
using Zivid.NET.Calibration;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            string fileCamera = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/BinWithCalibrationBoard.zfc";

            Console.WriteLine("Creating virtual camera using file: " + fileCamera);
            var camera = zivid.CreateFileCamera(fileCamera);

            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } }
            };

            var originalFrame = camera.Capture(settings);
            var pointCloud = originalFrame.PointCloud;

            Console.WriteLine("Displaying the original point cloud");
            VisualizeZividPointCloud(originalFrame);

            Console.WriteLine("Configuring ROI box based on bin size and checkerboard placement");
            float roiBoxLength = 545F;
            float roiBoxWidth = 345F;
            float roiBoxHeight = 150F;

            // Coordinates are relative to the checkerboard origin which lies in the intersection between the four checkers
            // in the top-left corner of the checkerboard: Positive x-axis is "East", y-axis is "South" and z-axis is "Down"
            var roiBoxLowerRightCornerInCheckerboardFrame = new Zivid.NET.PointXYZ
            {
                x = 240F,
                y = 30F,
                z = 5F
            };
            var roiBoxUpperRightCornerInCheckerboardFrame = new Zivid.NET.PointXYZ
            {
                x = roiBoxLowerRightCornerInCheckerboardFrame.x,
                y = roiBoxLowerRightCornerInCheckerboardFrame.y - roiBoxWidth,
                z = roiBoxLowerRightCornerInCheckerboardFrame.z
            };
            var roiBoxLowerLeftCornerInCheckerboardFrame = new Zivid.NET.PointXYZ
            {
                x = roiBoxLowerRightCornerInCheckerboardFrame.x - roiBoxLength,
                y = roiBoxLowerRightCornerInCheckerboardFrame.y,
                z = roiBoxLowerRightCornerInCheckerboardFrame.z

            };

            var pointOInCheckerboardFrame = roiBoxLowerRightCornerInCheckerboardFrame;
            var pointAInCheckerboardFrame = roiBoxUpperRightCornerInCheckerboardFrame;
            var pointBInCheckerboardFrame = roiBoxLowerLeftCornerInCheckerboardFrame;

            Console.WriteLine("Configuring ArUco marker");
            var markerDictionary = Zivid.NET.MarkerDictionary.Aruco4x4_50;
            var markerId = new List<int> { 1 };

            Console.WriteLine("Detecting ArUco marker");
            var detectionResult = Detector.DetectMarkers(originalFrame, markerId, markerDictionary);

            if (!detectionResult.Valid())
            {
                Console.WriteLine("No ArUco markers detected");
                return 1;
            }

            Console.WriteLine("Estimating pose of detected ArUco marker");
            var cameraToMarkerTransform = new Zivid.NET.Matrix4x4(detectionResult.DetectedMarkers()[0].Pose().ToMatrix());

            Console.WriteLine("Transforming the ROI base frame points to the camera frame");
            var roiPointsInCameraFrame = TransformPoints(
                new List<Zivid.NET.PointXYZ> { pointOInCheckerboardFrame, pointAInCheckerboardFrame, pointBInCheckerboardFrame },
                cameraToMarkerTransform);

            Console.WriteLine("Setting the ROI");
            settings.RegionOfInterest.Box.Enabled = true;
            settings.RegionOfInterest.Box.PointO = roiPointsInCameraFrame[0];
            settings.RegionOfInterest.Box.PointA = roiPointsInCameraFrame[1];
            settings.RegionOfInterest.Box.PointB = roiPointsInCameraFrame[2];
            settings.RegionOfInterest.Box.Extents = new Zivid.NET.Range<double>(-10, roiBoxHeight);

            var roiFrame = camera.Capture(settings);

            Console.WriteLine("Displaying the ROI-filtered point cloud");
            VisualizeZividPointCloud(roiFrame);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
    static void VisualizeZividPointCloud(Zivid.NET.Frame frame)
    {
        using (var visualizer = new Zivid.NET.Visualization.Visualizer())
        {
            visualizer.Show(frame);
            visualizer.ShowMaximized();
            visualizer.ResetToFit();
            Console.WriteLine("Running visualizer. Blocking until window closes.");
            visualizer.Run();
        }
    }

    static List<Matrix<double>> ZividToMathDotNet(List<PointXYZ> zividPoints)
    {
        var mathDotNetPoints = new List<Matrix<double>>(zividPoints.Count);
        foreach (var point in zividPoints)
        {
            mathDotNetPoints.Add(CreateMatrix.DenseOfArray(new double[,] { { point.x }, { point.y }, { point.z } }));
        }
        return mathDotNetPoints;
    }

    static Matrix<double> ZividToMathDotNet(Matrix4x4 zividMatrix)
    {
        return CreateMatrix.DenseOfArray(zividMatrix.ToArray()).ToDouble();
    }

    public static List<PointXYZ> MathDotNetToZivid(List<Matrix<double>> mathDotNetPoints)
    {
        var zividPoints = new List<PointXYZ>(mathDotNetPoints.Count);
        foreach (var point in mathDotNetPoints)
        {
            var zividPoint = new PointXYZ
            {
                x = (float)point[0, 0],
                y = (float)point[1, 0],
                z = (float)point[2, 0]
            };
            zividPoints.Add(zividPoint);
        }
        return zividPoints;
    }
    static List<PointXYZ> TransformPoints(List<PointXYZ> zividPoints, Matrix4x4 zividTransform)
    {
        var mathDotNetPoints = ZividToMathDotNet(zividPoints);
        var mathDotNetTransform = ZividToMathDotNet(zividTransform);

        var transformedMathDotNetPoints = new List<Matrix<double>>(mathDotNetPoints.Count);
        for (int i = 0; i < mathDotNetPoints.Count; i++)
        {
            var transformedPoint = mathDotNetTransform.SubMatrix(0, 3, 0, 3) * mathDotNetPoints[i] + mathDotNetTransform.SubMatrix(0, 3, 3, 1);
            transformedMathDotNetPoints.Add(transformedPoint);
        }

        return MathDotNetToZivid(transformedMathDotNetPoints);
    }
}
