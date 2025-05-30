﻿/*
Capture point clouds, with color, with the Zivid file camera and visualize them.
This example can be used without access to a physical camera.

The file camera files are found in Zivid Sample Data with ZFC as the file extension.
See the instructions in README.md to download the Zivid Sample Data.
There are five available file cameras to choose from, one for each camera model.
The default file camera used in this sample is the Zivid 2 M70 file camera.
*/

using System;

class Program
{
    static int Main(string[] args)
    {
        try
        {
            var userInput = ParseOptions(args);

            string fileCamera;
            if (userInput != null)
            {
                fileCamera = userInput;
            }
            else
            {
                fileCamera = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/FileCameraZivid2M70.zfc";
            }

            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Creating virtual camera using file: " + fileCamera);
            var camera = zivid.CreateFileCamera(fileCamera);

            Console.WriteLine("Configuring settings");
            var settings2D = new Zivid.NET.Settings2D
            {
                Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } },
                Processing = { Color = { Balance = { Blue = 1.0, Green = 1.0, Red = 1.0 } } }
            };
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
                Processing = { Filters = { Smoothing = { Gaussian = { Enabled = true, Sigma = 1.5 } },
                                           Reflection = { Removal = { Enabled = true } } } }
            };
            settings.Color = settings2D;

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture2D3D(settings))
            {
                Console.WriteLine("Setting up visualization");
                var visualizer = new Zivid.NET.Visualization.Visualizer();

                Console.WriteLine("Visualizing point cloud");
                visualizer.Show(frame);
                visualizer.ShowMaximized();
                visualizer.ResetToFit();

                Console.WriteLine("Running visualizer. Blocking until window closes.");
                visualizer.Run();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static ArgumentException UsageException()
    {
        return new ArgumentException("Usage: --file-camera <Path to the file camera .zfc file>");
    }

    static string ParseOptions(string[] args)
    {
        if (args.Length == 0)
        {
            return null;
        }
        if (args.Length == 2)
        {
            if (args[0].Equals("--file-camera"))
            {
                return args[1];
            }
        }
        throw UsageException();
    }
}
