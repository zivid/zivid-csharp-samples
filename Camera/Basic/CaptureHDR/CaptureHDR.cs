using System;
using System.Collections.Generic;

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

            Console.WriteLine("Recording HDR source images");
            var frames = new List<Zivid.NET.Frame>();
            foreach (var iris in new ulong[] { 10, 25, 35 })
            {
                Console.WriteLine("Measure with iris = " + iris);
                camera.UpdateSettings(s =>
                {
                    s.Iris = iris;
                });
                frames.Add(camera.Capture());
            }

            Console.WriteLine("Creating HDR frame");
            var hdrFrame = Zivid.NET.HDR.CombineFrames(frames);

            Console.WriteLine("Display the frame");
            visualizer.ShowMaximized();
            visualizer.Show(hdrFrame);
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
