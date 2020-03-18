/*
This example shows how to acquire HDR images from the Zivid camera in a loop
(while actively changing some HDR settings).
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

            Console.WriteLine("Recording HDR source images");
            var frames = new List<Zivid.NET.Frame>();

            camera.UpdateSettings(s =>
            {
                s.Iris = 10;
                s.ExposureTime = Duration.FromMicroseconds(10000);
                s.Brightness = 1;
                s.Gain = 1;
                s.Bidirectional = false;
                s.Filters.Contrast.Enabled = true;
                s.Filters.Contrast.Threshold = 5;
                s.Filters.Gaussian.Enabled = true;
                s.Filters.Gaussian.Sigma = 1.5;
                s.Filters.Outlier.Enabled = true;
                s.Filters.Outlier.Threshold = 5;
                s.Filters.Reflection.Enabled = true;
                s.Filters.Saturated.Enabled = true;
                s.BlueBalance = 1.081;
                s.RedBalance = 1.709;
            });

            ulong[] iris = { 10, 20, 30 };
            long[] exposure = { 10000, 20000, 30000 };

            for (int i = 0; i < 3; ++i)
            {
                camera.UpdateSettings(s =>
                {
                    s.Iris = iris[i];
                    s.ExposureTime = Duration.FromMicroseconds(exposure[i]);
                });
                frames.Add(camera.Capture());
                Console.WriteLine("Frame " + (i + 1) + " " + camera.Settings.ToString());
            }

            Console.WriteLine("Creating the HDR frame");
            using (var hdrFrame = Zivid.NET.HDR.CombineFrames(frames))
            {
                Console.WriteLine("Saving the frames");
                frames[0].Save("10.zdf");
                frames[1].Save("20.zdf");
                frames[2].Save("30.zdf");
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
