/*
Transform a point cloud from camera to checkerboard (Zivid Calibration Board) coordinate frame by getting checkerboard pose from the API.

The ZDF file for this sample can be found under the main instructions for Zivid samples.
*/

using System;

using Zivid.NET.Calibration;

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

            Console.WriteLine("Detecting and estimating pose of the Zivid checkerboard in the camera frame");
            var detectionResult = Detector.DetectCalibrationBoard(frame);
            var cameraToCheckerboardTransform = new Zivid.NET.Matrix4x4(detectionResult.Pose().ToMatrix());
            Console.WriteLine(cameraToCheckerboardTransform);
            Console.WriteLine("Camera pose in checkerboard frame:");
            var checkerboardToCameraTransform = cameraToCheckerboardTransform.Inverse();
            Console.WriteLine(checkerboardToCameraTransform);

            var transformFile = "CheckerboardToCameraTransform.yaml";
            Console.WriteLine("Saving a YAML file with Inverted checkerboard pose to file: " + transformFile);
            checkerboardToCameraTransform.Save(transformFile);

            Console.WriteLine("Transforming point cloud from camera frame to Checkerboard frame");
            pointCloud.Transform(checkerboardToCameraTransform);

            var checkerboardTransformedFile = "CalibrationBoardInCheckerboardOrigin.zdf";
            Console.WriteLine("Saving transformed point cloud to file: " + checkerboardTransformedFile);
            frame.Save(checkerboardTransformedFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
