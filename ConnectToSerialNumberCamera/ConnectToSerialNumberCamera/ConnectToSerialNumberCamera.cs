/*
This example shows how to connect to a specific Zivid camera based on its
serial number.
*/

using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to the camera");
            var camera = zivid.ConnectCamera(new Zivid.NET.SerialNumber("122021016180"));

            Console.WriteLine("Connected to the camera with the following serial number: " + camera.SerialNumber.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
