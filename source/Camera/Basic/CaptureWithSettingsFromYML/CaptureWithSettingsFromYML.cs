﻿/*
Capture images and point clouds, with and without color, from the Zivid camera with settings from YML file.

Choose whether to get the image in the linear RGB or the sRGB color space.

The YML files for this sample can be found under the main Zivid sample instructions.
*/

using System;
using Zivid.NET;
using System.Collections.Generic;

class Program
{
    static int Main(string[] args)
    {
        try
        {
            if (Array.Exists(args, arg => arg == "-h" || arg == "--help"))
            {
                ShowHelp();
                return 0;
            }

            var userOptions = ParseOptions(args);

            var zivid = new Zivid.NET.Application();
            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Loading settings from file");
            if (string.IsNullOrEmpty(userOptions.SettingsPath))
            {
                userOptions.SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                    + "/Zivid/Settings/" + SettingsFolder(camera) + "/Settings01.yml";
            }

            string settingsFile = userOptions.SettingsPath;
            var settings = new Zivid.NET.Settings(settingsFile);

            Console.WriteLine("Capturing 2D frame");
            using (var frame2D = camera.Capture2D(settings))
            {
                var pixelRow = 100;
                var pixelCol = 50;

                if (userOptions.LinearRgb)
                {
                    var imageRGBA = frame2D.ImageRGBA();
                    var imageFile = "ImageRGBA_linear.png";
                    Console.WriteLine($"Saving 2D color image (Linear RGB) to file: {imageFile}");
                    imageRGBA.Save(imageFile);

                    Console.WriteLine("Extracting 2D pixel array");
                    var pixelArrayRGBA = imageRGBA.ToArray();
                    Console.WriteLine("Height: {0}, Width: {1}", pixelArrayRGBA.GetLength(0), pixelArrayRGBA.GetLength(1));
                    Console.WriteLine("Color at pixel ({0},{1}):  R:{2}  G:{3}  B:{4}  A:{5}",
                                      pixelRow,
                                      pixelCol,
                                      pixelArrayRGBA[pixelRow, pixelCol].r,
                                      pixelArrayRGBA[pixelRow, pixelCol].g,
                                      pixelArrayRGBA[pixelRow, pixelCol].b,
                                      pixelArrayRGBA[pixelRow, pixelCol].a);
                }
                else
                {
                    var imageSRGB = frame2D.ImageRGBA_SRGB();
                    var imageFile = "ImageRGBA_sRGB.png";
                    Console.WriteLine($"Saving 2D color image (sRGB color space) to file: {imageFile}");
                    imageSRGB.Save(imageFile);

                    Console.WriteLine("Extracting 2D pixel array");
                    var pixelArraySRGB = imageSRGB.ToArray();
                    Console.WriteLine("Height: {0}, Width: {1}", pixelArraySRGB.GetLength(0), pixelArraySRGB.GetLength(1));
                    Console.WriteLine("Color at pixel ({0},{1}):  R:{2}  G:{3}  B:{4}  A:{5}",
                                      pixelRow,
                                      pixelCol,
                                      pixelArraySRGB[pixelRow, pixelCol].r,
                                      pixelArraySRGB[pixelRow, pixelCol].g,
                                      pixelArraySRGB[pixelRow, pixelCol].b,
                                      pixelArraySRGB[pixelRow, pixelCol].a);
                }
            }

            Console.WriteLine("Capturing 3D frame");
            using (var frame3D = camera.Capture3D(settings))
            {
                var dataFile = "Frame3D.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
                frame3D.Save(dataFile);

                var dataFilePly = "PointCloudWithoutColor.ply";
                Console.WriteLine("Exporting point cloud (default pink colored points) to file: " + dataFilePly);
                frame3D.Save(dataFilePly);
            }

            Console.WriteLine("Capturing 2D3D frame");
            using (var frame = camera.Capture2D3D(settings))
            {
                var dataFile = "Frame.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
                frame.Save(dataFile);

                var dataFilePly = "PointCloudWithColor.ply";
                Console.WriteLine("Exporting point cloud to file: " + dataFilePly);
                frame.Save(dataFilePly);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static (string SettingsPath, bool LinearRgb) ParseOptions(string[] args)
    {
        string settingsPath = "";
        bool linearRgb = false;

        foreach (var arg in args)
        {
            if (arg.StartsWith("--settings-path="))
            {
                settingsPath = arg.Substring("--settings-path=".Length);
            }
            else if (arg.Equals("--linear-rgb"))
            {
                linearRgb = true;
            }
        }

        return (settingsPath, linearRgb);
    }

    static string SettingsFolder(Zivid.NET.Camera camera)
    {
        var model = camera.Info.Model;
        switch (model)
        {
            case Zivid.NET.CameraInfo.ModelOption.ZividTwo: return "zivid2";
            case Zivid.NET.CameraInfo.ModelOption.ZividTwoL100: return "zivid2";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM130: return "zivid2Plus";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM60: return "zivid2Plus";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusL110: return "zivid2Plus";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR130: return "zivid2Plus/R";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR60: return "zivid2Plus/R";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusLR110: return "zivid2Plus/R";
            default: throw new InvalidOperationException("Unhandled enum value " + model.ToString());
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("Usage: CaptureWithSettingsFromYML.exe [options]");
        Console.WriteLine("Options:");
        Console.WriteLine("  --settings-path=<path>   Path to the camera settings YML file (default: based on camera model)");
        Console.WriteLine("  --linear-rgb             Use linear RGB instead of sRGB");
        Console.WriteLine("  -h, --help               Show this help message");
    }
}
