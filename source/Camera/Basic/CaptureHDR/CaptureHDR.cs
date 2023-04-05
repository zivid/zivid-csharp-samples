/*
Capture point clouds, with color, from the Zivid camera.

For scenes with high dynamic range we combine multiple acquisitions to get an HDR point cloud.
*/

using System;
using System.Collections.Generic;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring settings");
            var settings = new Zivid.NET.Settings();
            foreach (var aperture in new double[] { 9.57, 4.76, 2.59 })
            {
                Console.WriteLine("Adding acquisition with aperture = " + aperture);
                var acquisitionSettings = new Zivid.NET.Settings.Acquisition { Aperture = aperture };
                settings.Acquisitions.Add(acquisitionSettings);
            }

            Console.WriteLine("Capturing frame (HDR)");
            using (var frame = camera.Capture(settings))
            {
                var dataFile = "Frame.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
                frame.Save(dataFile);
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
