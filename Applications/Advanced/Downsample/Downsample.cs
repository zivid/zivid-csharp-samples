/*
Downsample point cloud from a ZDF file.

The ZDF files for this sample can be found under the main instructions for Zivid samples.
*/

using System;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var dataFile =
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/Zivid3D.zdf";
            Console.WriteLine("Reading ZDF frame from file: " + dataFile);
            var frame = new Zivid.NET.Frame(dataFile);

            Console.WriteLine("Getting point cloud from frame");
            var pointCloud = frame.PointCloud;

            Console.WriteLine("Size of point cloud before downsampling: " + pointCloud.Size + " data point");

            Console.WriteLine("Downsampling point cloud");
            Console.WriteLine("This does not modify the current point cloud but returns");
            Console.WriteLine("the downsampled point cloud as a new point cloud instance.");
            var downsampledPointCloud = pointCloud.Downsampled(Zivid.NET.PointCloud.Downsampling.By2x2);

            Console.WriteLine("Size of point cloud after downsampling: " + downsampledPointCloud.Size + " data points");

            Console.WriteLine("Downsampling point cloud (in-place)");
            Console.WriteLine("This modifies the current point cloud.");
            pointCloud.Downsample(Zivid.NET.PointCloud.Downsampling.By2x2);

            Console.WriteLine("Size of point cloud after downsampling: " + pointCloud.Size + " data points");

            Console.WriteLine("Setting up visualization");
            var visualizer = new Zivid.NET.Visualization.Visualizer();

            Console.WriteLine("Visualizing point cloud");
            visualizer.Show(pointCloud);
            visualizer.ShowMaximized();
            visualizer.ResetToFit();

            Console.WriteLine("Running visualizer. Blocking until window closes");
            visualizer.Run();
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
