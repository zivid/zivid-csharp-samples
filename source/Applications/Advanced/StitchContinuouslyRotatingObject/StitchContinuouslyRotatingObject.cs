/*
Stitch point clouds from a continuously rotating object without pre-alignment using Local Point Cloud Registration and apply Voxel Downsample.

It is assumed that the object is rotating around its own axis and the camera is stationary.
The camera settings should have defined a region of interest box that removes unnecessary points, keeping only the object to be stitched.

Note: This example uses experimental SDK features, which may be modified, moved, or deleted in the future without notice.

*/

using System;
using System.IO;
using System.Threading;
using Zivid.NET.Calibration;
using Zivid.NET.Experimental;
using Zivid.NET.Experimental.Toolbox;
using Zivid.NET.Visualization;

class Program
{
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

    private static void ShowHelp()
    {
        Console.WriteLine("Usage: StitchContinuouslyRotatingObject.exe [options]");
        Console.WriteLine("Options:");
        Console.WriteLine("  --settings-path=<path>   Path to camera settings YAML file containing ROI");
        Console.WriteLine("  -h, --help               Show this help message");
    }

    private static string ParseOptions(string[] args)
    {
        string settingsPath = "";
        foreach (var arg in args)
        {
            if (arg.StartsWith("--settings-path="))
            {
                settingsPath = arg.Substring("--settings-path=".Length);
            }
        }
        return settingsPath;
    }

    static int Main(string[] args)
    {
        try
        {
            if (Array.Exists(args, arg => arg == "-h" || arg == "--help"))
            {
                ShowHelp();
                return 0;
            }

            var settingsPath = ParseOptions(args);

            if (string.IsNullOrEmpty(settingsPath) || !File.Exists(settingsPath))
            {
                Console.WriteLine("Missing or invalid --settings-path argument.");
                ShowHelp();
                return 1;
            }

            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            using (var camera = zivid.ConnectCamera())
            {
                Console.WriteLine("Loading settings from file: " + settingsPath);
                var settings = new Zivid.NET.Settings(settingsPath);

                var unorganizedStitchedPointCloud = new Zivid.NET.UnorganizedPointCloud();
                var registrationParams = new Zivid.NET.Experimental.LocalPointCloudRegistrationParameters();
                var previousToCurrentPointCloudTransform = new Pose(Zivid.NET.Matrix4x4.Identity());

                var captureCount = 20;
                for (int numberOfCaptures = 0; numberOfCaptures < captureCount; ++numberOfCaptures)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));

                    using (var frame = camera.Capture2D3D(settings))
                    {
                        var unorganizedPointCloud = frame.PointCloud.ToUnorganizedPointCloud().VoxelDownsampled(1.0f, 2);

                        if (numberOfCaptures != 0)
                        {
                            if (unorganizedStitchedPointCloud.Size < 4 || unorganizedPointCloud.Size < 4)
                            {
                                Console.WriteLine("Not enough points for registration, skipping stitching...");
                                continue;
                            }

                            var registrationResult = PointCloudRegistration.LocalPointCloudRegistration(
                                unorganizedStitchedPointCloud,
                                unorganizedPointCloud,
                                registrationParams,
                                previousToCurrentPointCloudTransform);

                            if (!registrationResult.Converged)
                            {
                                Console.WriteLine("Registration did not converge, skipping this frame...");
                                continue;
                            }

                            previousToCurrentPointCloudTransform = registrationResult.Transform;
                            var previousToCurrentMatrix = new Zivid.NET.Matrix4x4(previousToCurrentPointCloudTransform.ToMatrix());
                            unorganizedStitchedPointCloud.Transform(previousToCurrentMatrix.Inverse());
                        }

                        unorganizedStitchedPointCloud.Extend(unorganizedPointCloud);
                        Console.WriteLine("Captures done: " + (numberOfCaptures + 1));
                    }
                }

                Console.WriteLine("Voxel-downsampling the stitched point cloud");
                unorganizedStitchedPointCloud = unorganizedStitchedPointCloud.VoxelDownsampled(0.75f, 2);

                VisualizePointCloud(unorganizedStitchedPointCloud);

                var filename = "StitchedPointCloudOfRotatingObject.ply";
                var plyFile = new Zivid.NET.Experimental.PointCloudExport.FileFormat.PLY(
                    filename,
                    Zivid.NET.Experimental.PointCloudExport.FileFormat.PLY.Layout.Unordered,
                    Zivid.NET.Experimental.PointCloudExport.ColorSpace.SRGB
                );

                Console.WriteLine("Exporting point cloud to file: " + filename);
                PointCloudExport.ExportUnorganizedPointCloud(unorganizedStitchedPointCloud, plyFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
            return 1;
        }
        return 0;
    }
}
