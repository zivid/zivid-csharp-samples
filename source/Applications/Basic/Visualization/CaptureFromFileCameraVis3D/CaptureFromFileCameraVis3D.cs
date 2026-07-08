/*
Capture point clouds, with color, with the Zivid file camera and visualize them.
This example can be used without access to a physical camera.

The file camera is created from a ZDF with diagnostics.
ZDF files with diagnostics are found in Zivid Sample Data.
See the instructions in README.md to download the Zivid Sample Data.
There are nine available file cameras to choose from, one for each camera model.
The default ZDF used in this sample is from Zivid 2 M70.

For more information about file cameras, check out this tutorial:
https://support.zivid.com/en/latest/camera/academy/camera/file-camera.html
*/

using System;

class Program
{
    static int Main(string[] args)
    {
        try
        {
            var userInput = ParseOptions(args);

            string fileCamera;
            if (userInput != null)
            {
                fileCamera = userInput;
            }
            else
            {
                fileCamera = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/FileCameraZivid2M70.zdf";
            }

            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Creating virtual camera using file: " + fileCamera);
            var loadedFrameWithDiagnostics = new Zivid.NET.Frame(fileCamera);
            var camera = zivid.CreateFileCamera(loadedFrameWithDiagnostics);

            Console.WriteLine("Capturing frame");
            var settings = loadedFrameWithDiagnostics.Settings;
            settings.Processing.Filters.Smoothing.Gaussian.Enabled = true;
            settings.Processing.Filters.Smoothing.Gaussian.Sigma = 1.5;
            settings.Processing.Filters.Reflection.Removal.Enabled = true;
            settings.Processing.Filters.Reflection.Removal.Mode = Zivid.NET.Settings.ProcessingGroup.FiltersGroup.ReflectionGroup.RemovalGroup.ModeOption.Global;
            var roiBox = new Zivid.NET.Settings.RegionOfInterestGroup.BoxGroup
            {
                Enabled = true,
                PointO = new Zivid.NET.PointXYZ { x = -331, y = 201, z = 661 },
                PointA = new Zivid.NET.PointXYZ { x = 299, y = 203, z = 667 },
                PointB = new Zivid.NET.PointXYZ { x = -331, y = -203, z = 844 }
            };
            roiBox.Extents = new Zivid.NET.Range<double>(0, 178);
            settings.RegionOfInterest.Box = roiBox;
            using (var frame = camera.Capture2D3D(settings))
            {
                Console.WriteLine("Setting up visualization");
                var visualizer = new Zivid.NET.Visualization.Visualizer();

                Console.WriteLine("Visualizing point cloud");
                visualizer.Show(frame);
                visualizer.ShowMaximized();
                visualizer.ResetToFit();

                Console.WriteLine("Running visualizer. Blocking until window closes.");
                visualizer.Run();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static ArgumentException UsageException()
    {
        return new ArgumentException("Usage: --file-camera <Path to a ZDF with diagnostics enabled>");
    }

    static string ParseOptions(string[] args)
    {
        if (args.Length == 0)
        {
            return null;
        }
        if (args.Length == 2)
        {
            if (args[0].Equals("--file-camera"))
            {
                return args[1];
            }
        }
        throw UsageException();
    }
}
