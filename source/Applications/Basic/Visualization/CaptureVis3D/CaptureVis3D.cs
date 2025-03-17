/*
Capture point clouds, with color, from the Zivid camera, and visualize them.
*/

using System;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring settings");
            var settings2D = new Zivid.NET.Settings2D
            {
                Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } }
            };
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { Aperture = 5.66 } }
            };
            settings.Color = settings2D;

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture2D3D(settings))
            {
                Console.WriteLine("Setting up visualization");
                using (var visualizer = new Zivid.NET.Visualization.Visualizer())
                {
                    Console.WriteLine("Visualizing point cloud");
                    visualizer.Show(frame);
                    visualizer.ShowMaximized();
                    visualizer.ResetToFit();

                    Console.WriteLine("Running visualizer. Blocking until window closes.");
                    visualizer.Run();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
