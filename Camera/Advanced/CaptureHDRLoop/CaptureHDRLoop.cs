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

            var cameraModel = camera.Info.Model.ToString().Substring(0, 8);
            int captures = 3;
            for(int i = 1; i <= captures; i++)
            {
                var settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                                   + "/Zivid/Settings/" + cameraModel + "/Settings0" + i + ".yml";
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
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
