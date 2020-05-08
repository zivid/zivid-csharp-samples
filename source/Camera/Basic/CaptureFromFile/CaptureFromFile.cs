// Latest version of Zivid software (including samples) can be found at http://zivid.com/software/.

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

            Console.WriteLine("Initializing camera emulation using file: " + cameraFile);
            var camera = zivid.CreateFileCamera(cameraFile);

            Console.WriteLine("Configuring settings");
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
                Processing = { Filters = { Smoothing = { Gaussian = { Enabled = true, Sigma = 1.5 } },
                                           Reflection = { Removal = { Enabled = true } } },
                               Color = { Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 } } }
            };

            Console.WriteLine("Capture a frame");
            using (var frame = camera.Capture(settings))
            {
                Console.WriteLine("Saving frame to file: " + resultFile);
                frame.Save(resultFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
