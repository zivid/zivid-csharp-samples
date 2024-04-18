/*
Cover the same dynamic range in a scene with different acquisition settings to optimize for quality, speed, or to find a compromise.

The camera captures multi-acquisition HDR point clouds in a loop, with settings from YML files.
*/

using System;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            int captures = 3;
            for (int i = 1; i <= captures; i++)
            {
                var settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                                   + "/Zivid/Settings/" + SettingsFolder(camera) + "/Settings0" + i + ".yml";
                Console.WriteLine("Loading settings from file: " + settingsFile);
                var settings = new Zivid.NET.Settings(settingsFile);
                Console.WriteLine(settings.Acquisitions);

                Console.WriteLine("Capturing frame (HDR)");
                var frame = camera.Capture(settings);

                var dataFile = "Frame0" + i + ".zdf";
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

    static string SettingsFolder(Zivid.NET.Camera camera)
    {
        var model = camera.Info.Model;
        switch (model)
        {
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusSmall: return "zividOne";
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusMedium: return "zividOne";
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusLarge: return "zividOne";
            case Zivid.NET.CameraInfo.ModelOption.ZividTwo: return "zivid2";
            case Zivid.NET.CameraInfo.ModelOption.ZividTwoL100: return "zivid2";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM130: return "zivid2Plus";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM60: return "zivid2Plus";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusL110: return "zivid2Plus";
            default: throw new System.InvalidOperationException("Unhandled enum value " + model.ToString());
        }
    }
}
