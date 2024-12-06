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
                Console.WriteLine("Camera Info: {0}", camera.Info);
                Console.WriteLine("Camera State: {0}", camera.State);
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
