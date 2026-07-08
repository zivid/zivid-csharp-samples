/*
Capture a 2D+3D frame and a 2D frame from the Zivid camera with diagnostics enabled.

Enabling diagnostics allows collecting additional data to be saved in the ZDF file.
Send ZDF files with diagnostics enabled to the Zivid support team to allow more thorough troubleshooting.
The 2D frame ZDF (Frame2DWithDiagnostics.zdf) must be loaded using the Frame2D API.
Have in mind that enabling diagnostics increases the capture time and the RAM usage.

For more information on diagnostics, check out this article:
https://support.zivid.com/en/latest/reference-articles/settings/diagnostics.html
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

            Console.WriteLine("Configuring settings for 2D+3D capture");
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
                Color = new Zivid.NET.Settings2D { Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } } }
            };

            Console.WriteLine("Enabling diagnostics");
            settings.Diagnostics.Enabled = true;

            Console.WriteLine("Capturing 2D+3D frame");
            using (var frame = camera.Capture2D3D(settings))
            {
                var dataFile = "FrameWithDiagnostics.zdf";
                Console.WriteLine("Saving frame with diagnostic data to file: " + dataFile);
                frame.Save(dataFile);
            }

            Console.WriteLine("Configuring settings for 2D capture");
            var settings2D = new Zivid.NET.Settings2D
            {
                Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } }
            };

            Console.WriteLine("Enabling 2D diagnostics");
            settings2D.Diagnostics.Enabled = true;

            Console.WriteLine("Capturing 2D frame");
            using (var frame2D = camera.Capture2D(settings2D))
            {
                var dataFile2D = "Frame2DWithDiagnostics.zdf";
                Console.WriteLine("Saving 2D frame with diagnostic data to file: " + dataFile2D);
                frame2D.Save(dataFile2D);
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
