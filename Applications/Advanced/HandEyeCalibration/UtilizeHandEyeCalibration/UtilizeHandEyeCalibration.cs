/*
Transform single data point or entire point cloud from camera frame to robot base frame using Hand-Eye calibration
matrix.

This example shows how to utilize the result of Hand-Eye calibration to transform either (picking) point coordinates
or the entire point cloud from the camera frame to the robot base frame.

For both Eye-To-Hand and Eye-In-Hand, there is a Zivid gem placed approx. 500 mm away from the robot base (see below).
The (picking) point is the Zivid gem centroid, defined as image coordinates in the camera frame and hard-coded
in this code example. Open the ZDF files in Zivid Studio to inspect the gem's 2D and corresponding 3D coordinates.

Eye-To-Hand
- ZDF file: ZividGemEyeToHand.zdf
- 2D image coordinates: (1035,255)
- Corresponding 3D coordinates: (37.77 -145.92 1227.1)
- Corresponding 3D coordinates (robot base frame): (-12.4  514.37 -21.79)

Eye-In-Hand:
- ZDF file: ZividGemEyeInHand.zdf
- 2D image coordinates: (1460,755)
- Corresponding 3D coordinates (camera frame): (83.95  28.84 305.7)
- Corresponding 3D coordinates (robot base frame): (531.03  -5.44 164.6)

For verification, check that the Zivid gem centroid 3D coordinates are the same as above after the transformation.

The YAML files for this sample can be found under the main instructions for Zivid samples.
*/

using System;

using MathNet.Numerics.LinearAlgebra;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var fileName = "";
            var imageCoordinateX = 0;
            var imageCoordinateY = 0;
            var transformBaseToCameraMath = CreateMatrix.Diagonal<float>(4, 4);

            var loopContinue = true;
            while(loopContinue)
            {
                switch(Interaction.EnterRobotCameraConfiguration())
                {
                    case RobotCameraConfiguration.EyeToHand:

                        fileName = "ZividGemEyeToHand.zdf";

                        // The (picking) point is defined as image coordinates in camera frame. It is hard-coded for the
                        // ZividGemEyeToHand.zdf (1035,255) X: 37.8 Y: -145.9 Z: 1227.1
                        imageCoordinateX = 1035;
                        imageCoordinateY = 255;

                        var eyeToHandTransformFile = "EyeToHandTransform.yaml";

                        Console.WriteLine("Reading camera pose in robot base frame (result of eye-to-hand calibration");
                        var eyeToHandTransform = new Zivid.NET.Matrix4x4(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/"
                            + eyeToHandTransformFile);

                        // Converting to Math.NET matrices for easier computation
                        transformBaseToCameraMath = zividToMathDotNet(eyeToHandTransform);

                        loopContinue = false;
                        break;

                    case RobotCameraConfiguration.EyeInHand:

                        fileName = "ZividGemEyeInHand.zdf";

                        // The (picking) point is defined as image coordinates in camera frame. It is hard-coded for the
                        // ZividGemEyeInHand.zdf (1460,755) X: 83.95 Y: 28.84 Z: 305.7
                        imageCoordinateX = 1357;
                        imageCoordinateY = 666;

                        var eyeInHandTransformFile = "EyeInHandTransform.yaml";
                        var robotTransformFile = "RobotTransform.yaml";

                        Console.WriteLine(
                            "Reading camera pose in end-effector frame (result of eye-in-hand calibration)");
                        var eyeInHandTransform = new Zivid.NET.Matrix4x4(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/"
                            + eyeInHandTransformFile);

                        Console.WriteLine("Reading end-effector pose in robot base frame");
                        var robotTransform = new Zivid.NET.Matrix4x4(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/"
                            + robotTransformFile);

                        // Converting to Math.NET matrices for easier computation
                        Matrix<float> transformEndEffectorToCamera = zividToMathDotNet(eyeInHandTransform);
                        Matrix<float> transformBaseToEndEffector = zividToMathDotNet(robotTransform);

                        Console.WriteLine("Computing camera pose in robot base frame");
                        transformBaseToCameraMath = transformBaseToEndEffector * transformEndEffectorToCamera;

                        loopContinue = false;
                        break;

                    case RobotCameraConfiguration.Unknown:
                        Console.WriteLine("Entered unknown Hand-Eye calibration type");
                        break;
                }
            }

            var dataFile =
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/" + fileName;
            Console.WriteLine("Reading ZDF frame from file: " + dataFile);
            var frame = new Zivid.NET.Frame(dataFile);
            var pointCloud = frame.PointCloud;

            loopContinue = true;
            while(loopContinue)
            {
                switch(Interaction.EnterCommand())
                {
                    case Command.TransformSinglePoint:

                        Console.WriteLine("Transforming single point");

                        var xyz = pointCloud.CopyPointsXYZW();

                        var pickingPoint = new float[] { xyz[imageCoordinateY, imageCoordinateX, 0],
                                                         xyz[imageCoordinateY, imageCoordinateX, 1],
                                                         xyz[imageCoordinateY, imageCoordinateX, 2],
                                                         xyz[imageCoordinateY, imageCoordinateX, 3] };

                        Vector<float> pointInCameraFrame = CreateVector.DenseOfArray(pickingPoint);
                        Console.WriteLine("Point coordinates in camera frame: " + pointInCameraFrame);

                        Console.WriteLine("Transforming (picking) point from camera to robot base frame");
                        Vector<float> pointInBaseFrame = transformBaseToCameraMath * pointInCameraFrame;

                        Console.WriteLine("Point coordinates in robot base frame: " + pointInBaseFrame);

                        loopContinue = false;
                        break;

                    case Command.TransformPointCloud:

                        Console.WriteLine("Transforming point cloud");

                        var transformBaseToCamera = mathDotNetToZivid(transformBaseToCameraMath);
                        pointCloud.Transform(transformBaseToCamera);

                        var saveFile = "ZividGemTransformed.zdf";
                        Console.WriteLine("Saving frame to file: " + saveFile);
                        frame.Save(saveFile);

                        loopContinue = false;
                        break;

                    case Command.Unknown: Console.WriteLine("Entered unknown command"); break;
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
            return 1;
        }
        return 0;
    }

    static Matrix<float> zividToMathDotNet(Zivid.NET.Matrix4x4 zividMatrix)
    {
        var mathNetMatrix = CreateMatrix.DenseOfArray(zividMatrix.ToArray());
        return mathNetMatrix;
    }

    static Zivid.NET.Matrix4x4 mathDotNetToZivid(Matrix<float> mathNetMatrix)
    {
        var zividMatrix = new Zivid.NET.Matrix4x4(mathNetMatrix.ToArray());
        return zividMatrix;
    }
}

enum Command
{
    TransformSinglePoint,
    TransformPointCloud,
    Unknown
}

enum RobotCameraConfiguration
{
    EyeToHand,
    EyeInHand,
    Unknown
}

class Interaction
{
    public static Command EnterCommand()
    {
        Console.Write("Enter command, s (to transform single point) or p (to transform point cloud): ");
        var command = Console.ReadLine().ToLower();

        switch(command)
        {
            case "s": return Command.TransformSinglePoint;
            case "p": return Command.TransformPointCloud;
            default: return Command.Unknown;
        }
    }

    public static RobotCameraConfiguration EnterRobotCameraConfiguration()
    {
        Console.Write("Enter type of calibration, eth (for eye-to-hand) or eih (for eye-in-hand): ");
        var command = Console.ReadLine().ToLower();

        switch(command)
        {
            case "eth": return RobotCameraConfiguration.EyeToHand;
            case "eih": return RobotCameraConfiguration.EyeInHand;
            default: return RobotCameraConfiguration.Unknown;
        }
    }
}
