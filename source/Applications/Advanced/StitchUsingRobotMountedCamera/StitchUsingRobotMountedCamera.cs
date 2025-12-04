/*
Stitch multiple point clouds captured with a robot mounted camera.

The sample simulates a camera capturing the point clouds at different robot poses.
The point clouds are pre-aligned using the robot's pose and a hand-eye calibration transform.
The resulting stitched point cloud is displayed and saved to a PLY file.

The sample demonstrates stitching of a small and a big object.
The small object fits within the camera's field of view and the result of stitching is a full
point cloud of the object, seen from different angles, i.e, from the front, back, left, and right sides.
The big object does not fit within the camera's field of view, so the stitching is done to extend the
field of view of the camera, and see the object in full.

The resulting stitched point cloud is voxel downsampled if the `--full-resolution` flag is not set.

Dataset: https://support.zivid.com/en/latest/api-reference/samples/sample-data.html

Extract the content into:
    - Windows:   %ProgramData%/Zivid/StitchingPointClouds/
    - Linux:     /usr/share/Zivid/data/StitchingPointClouds/

    StitchingPointClouds/
        ├── SmallObject/
        └── BigObject/

Each of these folders must contain ZDF captures, robot poses, and a hand-eye transform file.

Note: This example uses experimental SDK features, which may be modified, moved, or deleted in the future without notice.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Zivid.NET.Calibration;
using Zivid.NET.Experimental;
using Zivid.NET.Experimental.Toolbox;
using Zivid.NET.Visualization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

class Program
{
    private struct RegistrationResults
    {
        public Zivid.NET.Matrix4x4 baseToCameraTransform;
        public Zivid.NET.Matrix4x4 previousToCurrentTransform;
    }

    private static void VisualizePointCloud(Zivid.NET.UnorganizedPointCloud unorganizedPointCloud)
    {
        using (var visualizer = new Visualizer())
        {
            visualizer.ShowMaximized();
            visualizer.Show(unorganizedPointCloud);
            visualizer.ResetToFit();

            Console.WriteLine("Running visualizer. Blocking until window closes.");
            visualizer.Run();
        }
    }

    private static Matrix<double> ZividToMathDotNet(Zivid.NET.Matrix4x4 zividMatrix)
    {
        return CreateMatrix.DenseOfArray(zividMatrix.ToArray()).ToDouble();
    }

    private static Zivid.NET.Matrix4x4 MathDotNetToZivid(Matrix<double> mathMatrix)
    {
        var result = new float[4, 4];
        for (int r = 0; r < 4; ++r)
        {
            for (int c = 0; c < 4; ++c)
            {
                result[r, c] = (float)mathMatrix[r, c];
            }
        }
        return new Zivid.NET.Matrix4x4(result);
    }

    private static void GetZDFAndPoses(
        DirectoryInfo directory,
        List<FileInfo> zdfFilePaths,
        List<FileInfo> poseFilePaths)
    {
        var zdfPattern = new Regex("capture_.*\\.zdf", RegexOptions.IgnoreCase);
        var posePattern = new Regex("robot_pose_.*\\.yaml", RegexOptions.IgnoreCase);

        foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
        {
            if (!file.Exists)
            {
                continue;
            }
            var filename = file.Name;
            if (zdfPattern.IsMatch(filename))
            {
                zdfFilePaths.Add(file);
            }
            else if (posePattern.IsMatch(filename))
            {
                poseFilePaths.Add(file);
            }
        }

        if (zdfFilePaths.Count == 0)
        {
            throw new Exception("No ZDF files found.");
        }
        if (poseFilePaths.Count == 0)
        {
            throw new Exception("No robot pose files found.");
        }
        if (!File.Exists(Path.Combine(directory.FullName, "hand_eye_transform.yaml")))
        {
            throw new Exception("Missing hand_eye_transform.yaml.");
        }
        if (zdfFilePaths.Count != poseFilePaths.Count)
        {
            throw new Exception("Number of ZDF files and robot pose files do not match.");
        }

        zdfFilePaths.Sort((a, b) => String.Compare(a.FullName, b.FullName, StringComparison.Ordinal));
        poseFilePaths.Sort((a, b) => String.Compare(a.FullName, b.FullName, StringComparison.Ordinal));
    }

    private static Zivid.NET.UnorganizedPointCloud StitchPointClouds(DirectoryInfo directory, bool fullResolution)
    {
        var zdfFilePaths = new List<FileInfo>();
        var poseFilePaths = new List<FileInfo>();

        GetZDFAndPoses(directory, zdfFilePaths, poseFilePaths);

        var handEyeTransform = new Zivid.NET.Matrix4x4(Path.Combine(directory.FullName, "hand_eye_transform.yaml"));

        var previousToCurrentTransform = Zivid.NET.Matrix4x4.Identity();
        var accumulatedPointCloud = new Zivid.NET.UnorganizedPointCloud();

        var registrationParameters = new Zivid.NET.Experimental.LocalPointCloudRegistrationParameters();
        registrationParameters.MaxCorrespondenceDistance = 2.0f;

        var poseTransforms = new List<RegistrationResults>();

        for (int index = 0; index < zdfFilePaths.Count; ++index)
        {
            var robotPose = new Zivid.NET.Matrix4x4(poseFilePaths[index].FullName);
            using (var frame = new Zivid.NET.Frame(zdfFilePaths[index].FullName))
            {
                var baseToCameraTransform = MathDotNetToZivid(ZividToMathDotNet(robotPose) * ZividToMathDotNet(handEyeTransform));
                var organizedPointCloudInBaseFrame =
                    frame.PointCloud.ToUnorganizedPointCloud().VoxelDownsampled(1.0f, 2).Transform(baseToCameraTransform);

                if (index != 0)
                {
                    var identityPose = new Pose(Zivid.NET.Matrix4x4.Identity());
                    var registrationResult = PointCloudRegistration.LocalPointCloudRegistration(
                        accumulatedPointCloud,
                        organizedPointCloudInBaseFrame,
                        registrationParameters,
                        identityPose);

                    if (!registrationResult.Converged)
                    {
                        throw new Exception("Registration did not converge for the point cloud -> " + zdfFilePaths[index].FullName);
                    }

                    previousToCurrentTransform = new Zivid.NET.Matrix4x4(registrationResult.Transform.ToMatrix());
                    accumulatedPointCloud.Transform(previousToCurrentTransform.Inverse());

                    Console.WriteLine((index + 1) + " out of " + zdfFilePaths.Count +
                                      (fullResolution ? " point clouds aligned." : " point clouds stitched."));
                }

                poseTransforms.Add(new RegistrationResults
                {
                    baseToCameraTransform = baseToCameraTransform,
                    previousToCurrentTransform = previousToCurrentTransform
                });
                accumulatedPointCloud.Extend(organizedPointCloudInBaseFrame);
            }
        }

        var finalPointCloud = new Zivid.NET.UnorganizedPointCloud();
        if (!fullResolution)
        {
            //Downsampling the final result for efficiency
            finalPointCloud = accumulatedPointCloud.VoxelDownsampled(1.0f, 2);
        }
        else
        {
            for (int index = 0; index < poseTransforms.Count; ++index)
            {
                using (var frame = new Zivid.NET.Frame(zdfFilePaths[index].FullName))
                {
                    var registrationResult = poseTransforms[index];
                    frame.PointCloud.Transform(registrationResult.baseToCameraTransform);
                    finalPointCloud.Transform(registrationResult.previousToCurrentTransform.Inverse());
                    finalPointCloud.Extend(frame.PointCloud.ToUnorganizedPointCloud());
                    if (index > 0)
                    {
                        Console.WriteLine((index + 1) + " out of " + zdfFilePaths.Count + " point clouds stitched.");
                    }
                }
            }
        }
        return finalPointCloud;
    }

    private static void ShowHelp()
    {
        Console.WriteLine("SYNOPSIS:");
        Console.WriteLine("  StitchUsingRobotMountedCamera [options]");
        Console.WriteLine("OPTIONS:");
        Console.WriteLine("  -h, --help            Show help message");
        Console.WriteLine("  --full-resolution     Use full resolution for stitching. If not set, downsampling is applied.");
    }

    private static bool ParseOptions(string[] args, out bool fullResolution, out bool showHelp)
    {
        fullResolution = false;
        showHelp = false;
        foreach (var arg in args)
        {
            if (arg == "-h" || arg == "--help")
            {
                showHelp = true;
            }
            else if (arg == "--full-resolution")
            {
                fullResolution = true;
            }
            else if (arg.StartsWith("-"))
            {
                return false;
            }
        }
        return true;
    }

    static int Main(string[] args)
    {
        try
        {
            bool fullResolution;
            bool showHelp;

            var parsedOk = ParseOptions(args, out fullResolution, out showHelp);
            if (!parsedOk || showHelp)
            {
                ShowHelp();
                if (showHelp)
                {
                    return 0;
                }
                throw new Exception("Command-line parsing failed.");
            }

            var app = new Zivid.NET.Application();

            // Ensure the dataset is extracted to the correct location depending on the operating system:
            //   - Windows:   %ProgramData%/Zivid/StitchingPointClouds/
            //   - Linux:     /usr/share/Zivid/data/StitchingPointClouds/
            //   StitchingPointClouds/
            //     ├── SmallObject/
            //     └── BigObject/
            // Each folder must include:
            //   - capture_*.zdf
            //   - robot_pose_*.yaml
            //   - hand_eye_transform.yaml
            var sampleDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid";
            var smallObjectDirPath = Path.Combine(sampleDataDir, "StitchingPointClouds", "SmallObject");
            var bigObjectDirPath = Path.Combine(sampleDataDir, "StitchingPointClouds", "BigObject");

            if (!Directory.Exists(smallObjectDirPath) || !Directory.Exists(bigObjectDirPath))
            {
                Console.WriteLine("Missing dataset folders.");
                Console.WriteLine("Make sure 'StitchingPointClouds/SmallObject' and 'StitchingPointClouds/BigObject' exist at " + sampleDataDir + ".");
                Console.WriteLine("You can download the dataset (StitchingPointClouds.zip) from:");
                Console.WriteLine("https://support.zivid.com/en/latest/api-reference/samples/sample-data.html");
                return 1;
            }

            // Small object
            Console.WriteLine("Stitching small object...");
            var finalPointCloudSmallObject = StitchPointClouds(new DirectoryInfo(smallObjectDirPath), fullResolution);
            VisualizePointCloud(finalPointCloudSmallObject);
            var fileNameSmall = "StitchedPointCloudSmallObject.ply";
            var plyFileSmall = new Zivid.NET.Experimental.PointCloudExport.FileFormat.PLY(
                fileNameSmall,
                Zivid.NET.Experimental.PointCloudExport.FileFormat.PLY.Layout.Unordered,
                Zivid.NET.Experimental.PointCloudExport.ColorSpace.SRGB
            );
            Console.WriteLine("Exporting point cloud to file: " + fileNameSmall);
            PointCloudExport.ExportUnorganizedPointCloud(finalPointCloudSmallObject, plyFileSmall);

            // Big object
            Console.WriteLine("Stitching big object...");
            var finalPointCloudBigObject = StitchPointClouds(new DirectoryInfo(bigObjectDirPath), fullResolution);
            VisualizePointCloud(finalPointCloudBigObject);
            var fileNameBig = "StitchedPointCloudBigObject.ply";
            var plyFileBig = new Zivid.NET.Experimental.PointCloudExport.FileFormat.PLY(
                fileNameBig,
                Zivid.NET.Experimental.PointCloudExport.FileFormat.PLY.Layout.Unordered,
                Zivid.NET.Experimental.PointCloudExport.ColorSpace.SRGB
            );
            Console.WriteLine("Exporting point cloud to file: " + fileNameBig);
            PointCloudExport.ExportUnorganizedPointCloud(finalPointCloudBigObject, plyFileBig);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
