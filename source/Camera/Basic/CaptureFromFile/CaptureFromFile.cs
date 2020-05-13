/*
This example shows how to capture point clouds, with color, from the Zivid file camera.
This example can be used without access to a physical camera.
*/

using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            // This FileCamera file is in Zivid Sample Data. See instructions in README.md.
            var fileCamera = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                             + "/Zivid/FileCameraZividOne.zfc";

            Console.WriteLine("Creating virtual camera using file: " + fileCamera);
            var camera = zivid.CreateFileCamera(fileCamera);

            Console.WriteLine("Configuring settings");
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
                Processing = { Filters = { Smoothing = { Gaussian = { Enabled = true, Sigma = 1.5 } },
                                           Reflection = { Removal = { Enabled = true } } },
                               Color = { Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 } } }
            };

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture(settings))
            {
                var dataFile = "Result.zdf";
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
