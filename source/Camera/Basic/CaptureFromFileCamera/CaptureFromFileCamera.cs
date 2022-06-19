/*
Capture point clouds, with color, from the Zivid file camera.

This example can be used without access to a physical camera.
*/

using System;
using ReflectionFilterModeOption = 
    Zivid.NET.Settings.ProcessingGroup.FiltersGroup.ReflectionGroup.RemovalGroup.ExperimentalGroup.ModeOption;
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
            var settings = new Zivid.NET.Settings {
                Acquisitions = { new Zivid.NET.Settings.Acquisition {} },
                Processing = { Filters = { Smoothing = { Gaussian = { Enabled = true, Sigma = 1.5 } },
                                           Reflection = { Removal = { Enabled = true, Experimental = { Mode = ReflectionFilterModeOption.Global} } } },
                               Color = { Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 } } }
            };

            Console.WriteLine("Capturing frame");
            using(var frame = camera.Capture(settings))
            {
                var dataFile = "Frame.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
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
