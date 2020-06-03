/*
This example shows how to capture point clouds, with color, from the Zivid camera, and visualize it.
*/

using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring settings");
            var settings =
                new Zivid.NET.Settings { Acquisitions = { new Zivid.NET.Settings.Acquisition { Aperture = 5.66 } } };

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture(settings))
            {
                Console.WriteLine("Setting up visualization");
                var visualizer = new Zivid.NET.Visualization.Visualizer();

                Console.WriteLine("Visualizing point cloud");
                visualizer.Show(frame);
                visualizer.ShowMaximized();
                visualizer.ResetToFit();

                Console.WriteLine("Running visualizer. Blocking until window closes");
                visualizer.Run();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
