/*
Capture images and point clouds, with or without color, from the Zivid camera with settings from YML file.

The YML files for this sample can be found under the main Zivid sample instructions.
*/

using System;
using System.Collections.Generic;
using Zivid.NET;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Loading settings from file");
            var settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                               + "/Zivid/Settings/" + SettingsFolder(camera) + "/Settings01.yml";
            var settings = new Zivid.NET.Settings(settingsFile);

            Console.WriteLine("Capturing 2D frame");
            using (var frame2D = camera.Capture2D(settings))
            {
                var imageSRGB = frame2D.ImageSRGB();
                var imageFile = "ImageSRGB.png";
                Console.WriteLine("Saving 2D color image (sRGB color space) to file: " + imageFile);
                imageSRGB.Save(imageFile);

                // More information about linear RGB and sRGB color spaces is available at:
                // https://support.zivid.com/en/latest/reference-articles/color-spaces-and-output-formats.html#color-spaces
                // To get linear RGB image, use the following line instead:
                // var imageLinearRGB = frame2D.ImageRGBA();

                var pixelRow = 100;
                var pixelCol = 50;

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

                Console.WriteLine("Extracting 3D array of bytes");
                var nativeArray = imageSRGB.ToByteArray();
                Console.WriteLine("Height: {0}, Width: {1}, Channels: {2}",
                                  nativeArray.GetLength(0),
                                  nativeArray.GetLength(1),
                                  nativeArray.GetLength(2));
                Console.WriteLine("Color at pixel ({0},{1}):  R:{2}  G:{3}  B:{4}  A:{5}",
                                  pixelRow,
                                  pixelCol,
                                  nativeArray[pixelRow, pixelCol, 0],
                                  nativeArray[pixelRow, pixelCol, 1],
                                  nativeArray[pixelRow, pixelCol, 2],
                                  nativeArray[pixelRow, pixelCol, 3]);

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
            default: throw new System.InvalidOperationException("Unhandled enum value " + model.ToString());
        }
    }
}
