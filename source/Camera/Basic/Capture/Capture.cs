/*
Capture colored point cloud, save 2D image, save 3D ZDF, and export PLY, using the Zivid camera.
*/

using System;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Creating default capture settings");
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
                Color = new Zivid.NET.Settings2D { Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } } }
            };

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture2D3D(settings))
            {
                var imageRGBA = frame.Frame2D.ImageRGBA_SRGB();
                var imageFile = "ImageRGB.png";
                Console.WriteLine("Saving 2D color image (sRGB color space) to file: " + imageFile);
                imageRGBA.Save(imageFile);

                var dataFile = "Frame.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
                frame.Save(dataFile);

                var dataFilePLY = "PointCloud.ply";
                Console.WriteLine("Exporting point cloud to file: " + dataFilePLY);
                frame.Save(dataFilePLY);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
