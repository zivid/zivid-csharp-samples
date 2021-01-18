/*
This example shows how to capture point clouds, with color, from the Zivid camera,
with settings from YML file.
*/

using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring settings from file");
            var cameraModel = camera.Info.ModelName.Substring(0, 9);
            var settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                               + "/Zivid/Settings/" + cameraModel + "/Settings01.yml";
            var settings = new Zivid.NET.Settings(settingsFile);

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture(settings))
            {
                var dataFile = "Frame.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
                frame.Save(dataFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
