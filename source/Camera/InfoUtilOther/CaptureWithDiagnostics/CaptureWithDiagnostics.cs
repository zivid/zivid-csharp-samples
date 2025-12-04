/*
Capture point clouds, with color, from the Zivid camera, with settings from YML file and diagnostics enabled.

Enabling diagnostics allows collecting additional data to be saved in the ZDF file.
Send ZDF files with diagnostics enabled to the Zivid support team to allow more thorough troubleshooting.
Have in mind that enabling diagnostics increases the capture time and the RAM usage.

The YML file for this sample can be found under the main instructions for Zivid samples.
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

            Console.WriteLine("Configuring settings");
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
                Color = new Zivid.NET.Settings2D { Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } } }
            };

            Console.WriteLine("Enabling diagnostics");
            settings.Diagnostics.Enabled = true;

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture2D3D(settings))
            {
                var dataFile = "FrameWithDiagnostics.zdf";
                Console.WriteLine("Saving frame with diagnostic data to file: " + dataFile);
                frame.Save(dataFile);
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
