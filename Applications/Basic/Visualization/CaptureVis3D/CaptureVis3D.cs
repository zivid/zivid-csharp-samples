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

            Console.WriteLine("Adjusting the iris");
            camera.UpdateSettings(s =>
            {
                s.Iris = 22;
            });

            Console.WriteLine("Capture a frame");
            var frame = camera.Capture();

            Console.WriteLine("Display the frame");
            visualizer.Show(frame);
            visualizer.ShowMaximized();
            visualizer.ResetToFit();

            Console.WriteLine("Run the visualizer. Block until window closes");
            visualizer.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
