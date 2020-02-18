// Please make sure that Zivid sample data has been selected during installation of Zivid software.
// Latest version of Zivid software (including samples) can be found at http://zivid.com/software/.

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

            var zdfFile = Zivid.NET.Environment.DataPath + "/MiscObjects.zdf";

            Console.WriteLine("Initializing camera emulation using file: " + zdfFile);
            var camera = zivid.CreateFileCamera(zdfFile);

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
