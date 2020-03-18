/*
This example shows how to acquire HDR images from the Zivid camera in a loop,
with settings from .yml files
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

            Console.WriteLine("Connecting to the camera");
            var camera = zivid.ConnectCamera();

            const int numberOfCaptures = 3;
            const int numberOfFramesPerCapture = 3;
            for (var set = 1; set <= numberOfCaptures; set++)
            {
                Console.WriteLine("Configure HDR settings");

                var settingsList = new List<Zivid.NET.Settings>();
                for (var frame = 1; frame <= numberOfFramesPerCapture; frame++)
                {
                    string file_path = "Settings/set" + set + "/frame_0" + frame + ".yml";
                    Console.WriteLine("Add settings from " + file_path + ":");
                    var settings = new Zivid.NET.Settings(file_path);
                    Console.WriteLine(settings);
                    settingsList.Add(settings);
                }

                Console.WriteLine("Capture the HDR frame");
                using (var hdrFrame = Zivid.NET.HDR.Capture(camera, settingsList))
                {
                    string hdr_path = "HDR_" + set + ".zdf";
                    Console.WriteLine("Saving the HDR to " + hdr_path);
                    hdrFrame.Save(hdr_path);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
