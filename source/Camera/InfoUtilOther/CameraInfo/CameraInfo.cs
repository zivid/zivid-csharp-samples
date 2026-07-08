/*
List connected cameras and print camera version and state information for each connected camera.
*/

using System;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();
            Console.WriteLine("Zivid SDK: {0}", Zivid.NET.Version.CoreVersion.Full);
            var cameras = zivid.Cameras;
            Console.WriteLine("Number of cameras found: {0}", cameras.Count);
            foreach (var camera in cameras)
            {
                Console.WriteLine(camera.Info);
                Console.WriteLine(camera.State);
            }

            foreach (var camera in cameras)
            {
                var temperature = camera.State.Temperature;
                Console.WriteLine("Temperatures:");
                Console.WriteLine("  DMD:     {0} °C", temperature.DMD);
                Console.WriteLine("  LED:     {0} °C", temperature.LED);
                Console.WriteLine("  Lens:    {0} °C", temperature.Lens);
                Console.WriteLine("  PCB:     {0} °C", temperature.PCB);
                Console.WriteLine("  General: {0} °C", temperature.General);
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
