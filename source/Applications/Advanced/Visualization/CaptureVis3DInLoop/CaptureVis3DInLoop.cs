/*
Capture point clouds, with color, from the Zivid camera, and visualize them in a loop.
*/

using System;
using System.Threading;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Creating default settings");
            var settings2D = new Zivid.NET.Settings2D
            {
                Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } }
            };
            var settings = new Zivid.NET.Settings
            {
                Engine = Zivid.NET.Settings.EngineOption.Phase,
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } }
            };
            settings.Color = settings2D;

            Console.WriteLine("Capturing frame");
            var frame = camera.Capture2D3D(settings);
            Console.WriteLine($"Settings: {frame.Settings}");

            Console.WriteLine("Setting up visualization");
            var visualizerRunning = new ManualResetEventSlim(false);
            Zivid.NET.Visualization.Visualizer visualizerHandle = null;
            var visualizerReady = new ManualResetEventSlim(false);

            var visualizationThread = new Thread(() =>
            {
                using (var visualizer = new Zivid.NET.Visualization.Visualizer())
                {
                    // Pass the visualizer to the main thread
                    visualizerHandle = visualizer;
                    visualizerReady.Set();

                    Console.WriteLine("Visualizing point cloud");
                    visualizer.ShowMaximized();
                    visualizer.Show(frame);
                    visualizer.ResetToFit();

                    Console.WriteLine("Running visualizer. Blocking until window closes.");
                    visualizerRunning.Set();
                    visualizer.Run();
                    visualizerRunning.Reset();
                }
            });
            visualizationThread.Start();

            // Get the visualizer handle in the main thread
            visualizerReady.Wait();

            visualizerRunning.Wait();
            while (visualizerRunning.IsSet)
            {
                frame = camera.Capture2D3D(settings);
                if (!visualizerRunning.IsSet)
                    break;
                visualizerHandle.Show(frame);
                Thread.Sleep(10);
            }
            visualizationThread.Join();
            Console.WriteLine("Visualizer closed");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
