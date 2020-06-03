/*
This example shows how to capture point clouds, with color, from the Zivid camera.
For scenes with high dynamic range we combine multiple acquisitions to get an HDR
point cloud. This example shows how to fully configure settings for each acquisition.
In general, capturing an HDR point cloud is a lot simpler than this. The purpose of
this example is to demonstrate how to configure all the settings.
*/

using System;
using Duration = Zivid.NET.Duration;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring global processing settings:");
            var settings = new Zivid.NET.Settings()
            {
                Processing = { Filters = { Smoothing = { Gaussian = { Enabled = true, Sigma = 1.5 } },
                                           Noise = { Removal = { Enabled = true, Threshold = 7.0 } },
                                           Outlier = { Removal = { Enabled = true, Threshold = 5.0 } },
                                           Reflection = { Removal = { Enabled = true } },
                                           Experimental = { ContrastDistortion = { Correction = { Enabled = true,
                                                                                                  Strength = 0.4 },
                                                                                   Removal = { Enabled = true,
                                                                                               Threshold = 0.5 } } } },
                               Color = { Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 } } }
            };
            Console.WriteLine(settings.Processing);

            Console.WriteLine("Configuring base acquisition with settings same for all HDR acquisitions:");
            var baseAcquisition =
                new Zivid.NET.Settings.Acquisition { Brightness = 1.8, Patterns = { Sine = { Bidirectional = false } } };
            Console.WriteLine(baseAcquisition);

            Console.WriteLine("Configuring acquisition settings different for all HDR acquisitions:");
            double[] aperture = { 8.0, 4.0, 4.0 };
            int[] exposureTime = { 10000, 10000, 40000 };
            double[] gain = { 1.0, 1.0, 2.0 };
            for (int i = 0; i < aperture.Length; i++)
            {
                Console.WriteLine("Acquisition {0}:", i + 1);
                Console.WriteLine("  Exposure Time: {0}", exposureTime[i]);
                Console.WriteLine("  Aperture: {0}", aperture[i]);
                Console.WriteLine("  Gain: {0}", gain[i]);
                var acquisitionSettings = baseAcquisition.CopyWith(s =>
                {
                    s.Aperture = aperture[i];
                    s.ExposureTime = Duration.FromMicroseconds(exposureTime[i]);
                    s.Gain = gain[i];
                });
                settings.Acquisitions.Add(acquisitionSettings);
            }

            Console.WriteLine("Capturing frame (HDR)");
            using (var frame = camera.Capture(settings))
            {
                Console.WriteLine("Complete settings used:");
                Console.WriteLine(frame.Settings);

                var dataFile = "Frame.zdf";
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
