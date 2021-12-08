/*
Perform downsampling on a zivid point cloud.
*/

using System;

class Program
{
    static void Main()
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
            pointCloud.Downsample(Zivid.NET.PointCloud.Downsampling.By2x2);

            Console.WriteLine("Size of point cloud after downsampling: " + pointCloud.Size + " data point");

            Console.WriteLine("Setting up visualization");
            var visualizer = new Zivid.NET.Visualization.Visualizer();

            Console.WriteLine("Visualizing downsampled point cloud");
            visualizer.Show(pointCloud);
            visualizer.ShowMaximized();
            visualizer.ResetToFit();

            Console.WriteLine("Running visualizer. Blocking until window closes");
            visualizer.Run();
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}