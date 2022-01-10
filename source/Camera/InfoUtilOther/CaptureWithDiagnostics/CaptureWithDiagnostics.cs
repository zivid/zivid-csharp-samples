/*
Capture point clouds, with color, from the Zivid camera, with settings from YML file and diagnostics enabled.

Enabling diagnostics allows collecting additional data to be saved in the zdf file.
Send zdf files with diagnostics enabled to the Zivid support team to allow more thorough troubleshooting.
Have in mind that enabling diagnostics increases the capture time and the RAM usage.

The YML file for this sample can be found under the main instructions for Zivid samples.
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
            var cameraModel = camera.Info.Model.ToString().Substring(0, 8);
            var settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                               + "/Zivid/Settings/" + cameraModel + "/Settings01.yml";
            var settings = new Zivid.NET.Settings(settingsFile);

            Console.WriteLine("Enabling diagnostics");
            settings.Diagnostics.Enabled = true;

            Console.WriteLine("Capturing frame");
            using(var frame = camera.Capture(settings))
            {
                var dataFile = "FrameWithDiagnostics.zdf";
                Console.WriteLine("Saving frame with diagnostic data to file: " + dataFile);
                frame.Save(dataFile);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
