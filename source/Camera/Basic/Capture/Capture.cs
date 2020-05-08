using System;
using Duration = Zivid.NET.Duration;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var resultFile = "Result.zdf";

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Creating settings");
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition{ Aperture = 5.66,
                                                                     ExposureTime = Duration.FromMicroseconds(8333) } },
                Processing = { Filters = { Outlier = { Removal = { Enabled = true, Threshold = 5.0 } } } }
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
