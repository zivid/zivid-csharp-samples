/*
This example shows how to import and display a Zivid point cloud from a.ZDF
file.
*/

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

            var Filename = Zivid.NET.Environment.DataPath + "/Zivid3D.zdf";
            Console.WriteLine("Reading " + Filename + " point cloud");
            var frame = new Zivid.NET.Frame(Filename);

            Console.WriteLine("Displaying the frame");
            visualizer.Show(frame);
            visualizer.ShowMaximized();
            visualizer.ResetToFit();

            Console.WriteLine("Running the visualizer. Blocking until the window closes");
            visualizer.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}