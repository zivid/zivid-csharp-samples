/*
Capture point clouds, with color, from the Zivid camera, with settings from YML file.

The YML files for this sample can be found under the main Zivid sample instructions.
*/

using System;
using System.Collections.Generic;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Loading settings from file");
            var cameraModel = camera.Info.Model.ToString().Substring(0, 8);
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
            return 1;
        }
        return 0;
    }
}
