/*
Mask point cloud from a ZDF file using the Mask API and visualize it with Zivid.NET Visualizer.

This example shows how to:
1. Create a mask using rectangle drawing
2. Apply the mask to a point cloud using the Mask API
3. Visualize the results using Zivid.NET Visualizer

The ZDF file for this sample can be found under the main instructions for Zivid samples.
*/

using System;
using System.IO;
using Zivid.NET;
using Duration = Zivid.NET.Duration;

class Program
{
    static void VisualizePointCloudWithTitle(Zivid.NET.PointCloud pointCloud, string title)
    {
        using (var visualizer = new Zivid.NET.Visualization.Visualizer())
        {
            visualizer.WindowTitle = title;
            visualizer.Show(pointCloud);
            visualizer.Show();
            visualizer.ResetToFit();
            visualizer.Run();
        }
    }

    static Zivid.NET.Mask CreateRectangularMask(Zivid.NET.Resolution resolution, int pixelsToDisplay)
    {
        // Create a ones-filled mask
        var mask = new Zivid.NET.Mask(resolution);

        // Calculate rectangle bounds
        int height = (int)resolution.Height;
        int width = (int)resolution.Width;
        int heightMin = (height - pixelsToDisplay) / 2;
        int heightMax = (height + pixelsToDisplay) / 2;
        int widthMin = (width - pixelsToDisplay) / 2;
        int widthMax = (width + pixelsToDisplay) / 2;

        // Set pixels inside the rectangle to zero
        for (int y = heightMin; y < heightMax; ++y)
        {
            for (int x = widthMin; x < widthMax; ++x)
            {
                mask[x, y] = 0;
            }
        }

        return mask;
    }

    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var fileName = Environment.GetEnvironmentVariable("ZIVID_SAMPLE_DATA_DIR") + "/Zivid3D.zdf";
            Console.WriteLine($"Reading ZDF frame from file: {fileName}");
            var frame = new Zivid.NET.Frame(fileName);

            Console.WriteLine("Getting point cloud from frame");
            var pointCloud = frame.PointCloud;

            const int pixelsToDisplay = 300;
            Console.WriteLine($"Creating rectangular mask of central {pixelsToDisplay} x {pixelsToDisplay} pixels using new Mask API.");

            var resolution = new Zivid.NET.Resolution(pointCloud.Width, pointCloud.Height);
            Console.WriteLine($"Point cloud resolution: {resolution.Width} x {resolution.Height}");

            // Create mask using the new Mask API
            var mask = CreateRectangularMask(resolution, pixelsToDisplay);

            Console.WriteLine("Displaying original point cloud");
            VisualizePointCloudWithTitle(pointCloud, "Original Point Cloud");

            Console.WriteLine("Applying mask to point cloud using new Mask API");
            var maskedPointCloud = pointCloud.Masked(mask);

            Console.WriteLine("Displaying masked point cloud");
            VisualizePointCloudWithTitle(maskedPointCloud, "Masked Point Cloud");

            Console.WriteLine("Mask API demonstration completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
            return 1;
        }

        return 0;
    }
}
