/*
Update firmware on the Zivid camera.
*/

using System;
using Zivid.NET.Firmware;


class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var cameras = zivid.Cameras;
            if (cameras.Count == 0)
            {
                Console.WriteLine("No camera found.");
                return 1;
            }
            Console.WriteLine("Found {0} camera(s).", cameras.Count);
            foreach (var camera in cameras)
            {
                if(!Updater.IsUpToDate(camera))
                {
                    Console.WriteLine("Firmware update required");
                    Console.WriteLine("Updating firmware on camera {0}, model name: {1}, firmware version: {2}",
                        camera.Info.SerialNumber, camera.Info.ModelName, camera.Info.FirmwareVersion);
                    Updater.Update(camera, callback: (progressPercentage, stageDescription) => {
                        Console.WriteLine("{0}% : {1}" + ((progressPercentage < 100) ? "..." : ""), progressPercentage, stageDescription);
                    });
                }
                else
                {
                    Console.WriteLine("Skipping update of camera {0}, model name: {1}, firmware version: {2}",
                        camera.Info.SerialNumber, camera.Info.ModelName, camera.Info.FirmwareVersion);
                }
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
