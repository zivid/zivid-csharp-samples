/*
This example shows how to acquire an HDR image from the Zivid camera with fully
configured settings for each frame. In general, taking an HDR image is a lot
simpler than this as the default settings work for most scenes. The purpose of
this example is to demonstrate how to configure all the settings.
*/

using System;
using System.Collections.Generic;
using Duration = Zivid.NET.Duration;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to the camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring settings for all HDR frames");
            ulong[] iris = { 17, 27, 27 };
            long[] exposureTime = { 10000, 10000, 40000 };
            double[] gain = { 1.0, 1.0, 2.0 };
            var settingsHDR = new List<Zivid.NET.Settings>();
            for (int i = 0; i < 3; ++i)
            {
                var settings = new Zivid.NET.Settings
                {
                    Brightness = 1,
                    Iris = iris[i],
                    ExposureTime = Duration.FromMicroseconds(exposureTime[i]),
                    Gain = gain[i],
                    Bidirectional = false,
                    BlueBalance = 1.081,
                    RedBalance = 1.709,
                    Filters = {
                        Contrast = {
                            Enabled = true,
                            Threshold = 5,
                        },
                        Gaussian = {
                            Enabled = true,
                            Sigma = 2.245,
                        },
                        Outlier = {
                            Enabled = true,
                            Threshold = 5,
                        },
                        Reflection = {
                            Enabled = true
                        },
                        Saturated = {
                            Enabled = true
                        }
                    },
                };
                settingsHDR.Add(settings);
                Console.WriteLine("Frame " + i + " " + settingsHDR[i].ToString());
            }

            Console.WriteLine("Capturing the HDR frame");
            using (var hdrFrame = Zivid.NET.HDR.Capture(camera, settingsHDR))
            {
                Console.WriteLine("Saving the frame");
                hdrFrame.Save("HDR.zdf");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
