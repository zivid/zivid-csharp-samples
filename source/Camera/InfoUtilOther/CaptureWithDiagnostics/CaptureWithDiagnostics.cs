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

            Console.WriteLine("Configuring settings from file");
            var settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                               + "/Zivid/Settings/" + SettingsFolder(camera) + "/Settings01.yml";
            var settings = new Zivid.NET.Settings(settingsFile);

            Console.WriteLine("Enabling diagnostics");
            settings.Diagnostics.Enabled = true;

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture(settings))
            {
                var dataFile = "FrameWithDiagnostics.zdf";
                Console.WriteLine("Saving frame with diagnostic data to file: " + dataFile);
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

    static string SettingsFolder(Zivid.NET.Camera camera)
    {
        var model = camera.Info.Model;
        switch (model)
        {
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusSmall: return "zividOne";
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusMedium: return "zividOne";
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusLarge: return "zividOne";
            case Zivid.NET.CameraInfo.ModelOption.ZividTwo: return "zividTwo";
            case Zivid.NET.CameraInfo.ModelOption.ZividTwoL100: return "zivid2";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM130: return "zivid2Plus";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM60: return "zivid2Plus";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusL110: return "zivid2Plus";
            default: throw new System.InvalidOperationException("Unhandled enum value " + model.ToString());
        }
    }
}
