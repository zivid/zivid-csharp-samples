/*
List connected cameras and print version information.
*/

using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();
            Console.WriteLine("Zivid SDK: {0}", Zivid.NET.Version.CoreVersion.Full);
            var cameras = zivid.Cameras;
            Console.WriteLine("Number of cameras found: {0}", cameras.Count);
            foreach(var camera in cameras)
            {
                Console.WriteLine("Camera Info: {0}", camera.Info);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}