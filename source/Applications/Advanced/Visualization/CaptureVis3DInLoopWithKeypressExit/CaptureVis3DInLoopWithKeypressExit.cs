/*
Capture point clouds, with color, from the Zivid camera, and visualize them in a loop. Press 'q' to exit.
*/

using System;
using System.Threading;
using System.Windows.Forms;

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

            Console.WriteLine("Setting up visualization");
            var visualizerRunning = new ManualResetEventSlim(false);
            var acceptEnd = new ManualResetEventSlim(true);
            var quitRequested = new ManualResetEventSlim(false);
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
                    do
                    {
                        visualizerRunning.Set();
                        visualizer.Run();
                        visualizerRunning.Reset();

                        if (quitRequested.IsSet)
                            break;
                        Console.WriteLine(
                            "Visualizer window closed by user. It will be reopened if we're currently capturing."
                        );
                    } while (!acceptEnd.IsSet);
                }
            });
            visualizationThread.Start();

            // Get the visualizer handle in the main thread
            visualizerReady.Wait();

            visualizerRunning.Wait();
            Console.WriteLine("Press 'q' in the terminal to quit");
            while (visualizerRunning.IsSet)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.KeyChar == 'q')
                    {
                        Console.WriteLine("Closing application because user pressed 'q'");
                        quitRequested.Set();
                        Application.DoEvents();
                        foreach (Form form in Application.OpenForms)
                        {
                            if (form.Text.Contains("Zivid"))
                            {
                                form.Invoke(new Action(() => form.Close()));
                                break;
                            }
                        }
                    }
                }
                else
                {
                    acceptEnd.Reset();
                    frame = camera.Capture2D3D(settings);
                    visualizerHandle.Show(frame);
                    acceptEnd.Set();
                }
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
