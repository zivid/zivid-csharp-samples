﻿/*
Capture point clouds, with color, from the Zivid file camera, and visualize them.

This example can be used without access to a physical camera.
*/

using System;

class Program
{
    static int Main()
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
                                           Reflection = { Removal = { Enabled = true } } },
                               Color = { Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 } } }
            };

            Console.WriteLine("Capturing frame");
            using(var frame = camera.Capture(settings))
            {
                Console.WriteLine("Setting up visualization");
                var visualizer = new Zivid.NET.Visualization.Visualizer();

                Console.WriteLine("Visualizing point cloud");
                visualizer.Show(frame);
                visualizer.ShowMaximized();
                visualizer.ResetToFit();

                Console.WriteLine("Running visualizer. Blocking until window closes");
                visualizer.Run();
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
