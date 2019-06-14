/*
This example shows how to capture a Zivid point cloud and save it to a .PLY
file format.
*/

using System;
using Duration = Zivid.NET.Duration;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var FilenamePLY = "Zivid3D.ply";

            Console.WriteLine("Connecting to the camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring the camera settings");
            camera.UpdateSettings(s =>
            {
                s.Iris = 22;
                s.ExposureTime = Duration.FromMicroseconds(8333);
                s.Filters.Outlier.Enabled = true;
                s.Filters.Outlier.Threshold = 5;
            });

            Console.WriteLine("Capturing a frame");
            var frame = camera.Capture();

            Console.WriteLine("Saving the frame to " + FilenamePLY);
            frame.Save(FilenamePLY);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}