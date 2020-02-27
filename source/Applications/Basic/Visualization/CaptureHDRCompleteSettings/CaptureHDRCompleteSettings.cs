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

            Console.WriteLine("Setting up visualization");
            var visualizer = new Zivid.NET.CloudVisualizer();
            zivid.DefaultComputeDevice = visualizer.ComputeDevice;

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
            frames.Add(camera.Capture());
            Console.WriteLine("Frame 1 " + camera.Settings.ToString());

            camera.UpdateSettings(s =>
            {
                s.Iris = 20;
                s.ExposureTime = Duration.FromMicroseconds(20000);
                s.Brightness = 0.5;
                s.Gain = 2;
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
            frames.Add(camera.Capture());
            Console.WriteLine("Frame 2 " + camera.Settings.ToString());

            camera.UpdateSettings(s =>
            {
                s.Iris = 30;
                s.ExposureTime = Duration.FromMicroseconds(33000);
                s.Brightness = 1;
                s.Gain = 1;
                s.Bidirectional = true;
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
            frames.Add(camera.Capture());
            Console.WriteLine("Frame 3 " + camera.Settings.ToString());

            Console.WriteLine("Creating the HDR frame");
            var hdrFrame = Zivid.NET.HDR.CombineFrames(frames);

            Console.WriteLine("Saving the frames");
            frames[0].Save("10.zdf");
            frames[1].Save("20.zdf");
            frames[2].Save("30.zdf");
            hdrFrame.Save("HDR.zdf");

            Console.WriteLine("Displaying the HDR frame");
            visualizer.ShowMaximized();
            visualizer.Show(hdrFrame);
            visualizer.ResetToFit();

            Console.WriteLine("Running the visualizer. Blocking until the window closes");
            visualizer.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
