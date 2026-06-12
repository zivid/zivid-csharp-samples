/*
Stitch two point clouds using a transformation estimated by Local Point Cloud Registration and apply Voxel Downsample.

Dataset: https://support.zivid.com/en/latest/api-reference/samples/sample-data.html

Extract the content into:
    - Windows:   %ProgramData%/Zivid/StitchingPointClouds/
    - Linux:     /usr/share/Zivid/data/StitchingPointClouds/

StitchingPointClouds/
    └── BlueObject/

The folder must contain two ZDF files used for this sample.

Note: This example uses experimental SDK features, which may be modified, moved, or deleted in the future without notice.
*/

using System;
using System.IO;
using Zivid.NET.Calibration;
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

    static int Main()
    {
        try
        {
            var app = new Zivid.NET.Application();

            // Ensure the dataset is extracted to the correct location depending on the operating system:
            //   - Windows:   %ProgramData%/Zivid/StitchingPointClouds/
            //   - Linux:     /usr/share/Zivid/data/StitchingPointClouds/
            //  StitchingPointClouds/
            //      └── BlueObject/

            var sampleDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid";
            var directory = Path.Combine(sampleDataDir, "StitchingPointClouds", "BlueObject");

            Console.WriteLine("Reading point clouds from ZDF files");
            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Missing dataset folders.");
                Console.WriteLine("Make sure 'StitchingPointClouds/BlueObject/' exists at " + sampleDataDir);
                Console.WriteLine("You can download the dataset (StitchingPointClouds.zip) from:");
                Console.WriteLine("https://support.zivid.com/en/latest/api-reference/samples/sample-data.html");
                return 1;
            }

            var frame1 = new Zivid.NET.Frame(Path.Combine(directory, "BlueObject.zdf"));
            var frame2 = new Zivid.NET.Frame(Path.Combine(directory, "BlueObjectSlightlyMoved.zdf"));

            Console.WriteLine("Converting organized point clouds to unorganized point clouds and voxel downsampling");
            var unorganizedPointCloud1 = frame1.PointCloud.ToUnorganizedPointCloud();
            var unorganizedPointCloud2 = frame2.PointCloud.ToUnorganizedPointCloud();

            Console.WriteLine("Displaying point clouds before stitching");
            var unorganizedNotStitchedPointCloud = new Zivid.NET.UnorganizedPointCloud();
            unorganizedNotStitchedPointCloud.Extend(unorganizedPointCloud1);
            unorganizedNotStitchedPointCloud.Extend(unorganizedPointCloud2);
            VisualizePointCloud(unorganizedNotStitchedPointCloud);

            Console.WriteLine("Estimating transformation between point clouds");
            var unorganizedPointCloud1LPCR = unorganizedPointCloud1.VoxelDownsampled(1.0f, 3);
            var unorganizedPointCloud2LPCR = unorganizedPointCloud2.VoxelDownsampled(1.0f, 3);
            var registrationParams = new Zivid.NET.Experimental.LocalPointCloudRegistrationParameters();

            var identityPose = new Pose(Zivid.NET.Matrix4x4.Identity());
            var localPointCloudRegistrationResult = PointCloudRegistration.LocalPointCloudRegistration(
                unorganizedPointCloud1LPCR,
                unorganizedPointCloud2LPCR,
                registrationParams,
                identityPose);

            if (!localPointCloudRegistrationResult.Converged)
            {
                throw new Exception("Registration did not converge...");
            }

            var pointCloud1ToPointCloud2Transform = localPointCloudRegistrationResult.Transform;
            var unorganizedPointCloud2Transformed =
                unorganizedPointCloud2.Transform(pointCloud1ToPointCloud2Transform.ToMatrix());

            Console.WriteLine("Stitching and displaying painted point clouds to evaluate stitching quality");
            var finalPointCloud = new Zivid.NET.UnorganizedPointCloud();
            finalPointCloud.Extend(unorganizedPointCloud1);
            finalPointCloud.Extend(unorganizedPointCloud2Transformed);

            var paintedFinalPointCloud = new Zivid.NET.UnorganizedPointCloud();
            paintedFinalPointCloud.Extend(unorganizedPointCloud1.PaintedUniformColor(new Zivid.NET.ColorRGBA { r = 255, g = 0, b = 0, a = 255 }));
            paintedFinalPointCloud.Extend(unorganizedPointCloud2Transformed.PaintedUniformColor(new Zivid.NET.ColorRGBA { r = 0, g = 255, b = 0, a = 255 }));
            VisualizePointCloud(paintedFinalPointCloud);

            Console.WriteLine("Voxel-downsampling the stitched point cloud");
            finalPointCloud = finalPointCloud.VoxelDownsampled(2.0f, 1);
            Console.WriteLine("Visualize the overlapped point clouds");
            VisualizePointCloud(finalPointCloud);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
            return 1;
        }
        return 0;
    }
}
