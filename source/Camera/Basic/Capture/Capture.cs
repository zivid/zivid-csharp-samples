/*
Capture point clouds, with color, from the Zivid camera.
*/

using System;
using Duration = Zivid.NET.Duration;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Creating default capture settings");
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } }
            };

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture(settings))
            {
                var dataFile = "Frame.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
                frame.Save(dataFile);

                var dataFilePLY = "PointCloud.ply";
                Console.WriteLine("Exporting point cloud to file: " + dataFilePLY);
                frame.Save(dataFilePLY);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
