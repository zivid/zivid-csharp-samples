using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var resultFile = "HDR.zdf";

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Creating settings");
            var settings = new Zivid.NET.Settings();
            foreach (var aperture in new double[] { 11.31, 5.66, 2.83 })
            {
                Console.WriteLine("Adding acquisition with aperture = " + aperture);
                var acquisitionSettings = new Zivid.NET.Settings.Acquisition { Aperture = aperture };
                settings.Acquisitions.Add(acquisitionSettings);
            }

            Console.WriteLine("Capturing HDR frame");
            using (var hdrFrame = camera.Capture(settings))
            {
                Console.WriteLine("Saving frame to file: " + hdrFrame);
                hdrFrame.Save(resultFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
