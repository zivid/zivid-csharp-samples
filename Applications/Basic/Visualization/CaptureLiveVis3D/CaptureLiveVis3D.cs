using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Setting up visualization");
            var visualizer = new Zivid.NET.CloudVisualizer();
            zivid.DefaultComputeDevice = visualizer.ComputeDevice;

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            visualizer.ShowMaximized();

            Console.WriteLine("Starting live capturing of frames");
            var resetToFit = true;
            camera.SetFrameCallback(frame => {
                visualizer.Show(frame);
                if (resetToFit)
                {
                    visualizer.ResetToFit();
                    resetToFit = false;
                }
                frame.Dispose();
            });

            camera.StartLive();

            Console.WriteLine("Run the visualizer. Block until window closes");
            visualizer.Run();

            Console.WriteLine("Stopping live capturing");
            camera.StopLive();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
