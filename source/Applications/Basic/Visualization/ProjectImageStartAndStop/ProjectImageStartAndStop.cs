/*
Start the Image Projection and Stop it.

How to stop the image projection is demonstrated in three different ways:
- calling stop() function on the projected image handle
- projected image handle going out of scope
- triggering a 3D capture

*/

using System;

class Program
{
    static Zivid.NET.ImageBGRA CreateProjectorImage(Zivid.NET.Resolution resolution, Zivid.NET.ColorBGRA color)
    {
        var pixelArray = new Zivid.NET.ColorBGRA[resolution.Height, resolution.Width];
        for (ulong y = 0; y < resolution.Height; y++)
        {
            for (ulong x = 0; x < resolution.Width; x++)
            {
                pixelArray[y, x] = color;
            }
        }
        var projectorImage = new Zivid.NET.ImageBGRA(pixelArray);
        return projectorImage;
    }

    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            using (var camera = zivid.ConnectCamera())
            {
                Console.WriteLine("Retrieving the projector resolution that the camera supports");
                var projectorResolution = Zivid.NET.Projection.Projection.ProjectorResolution(camera);

                var redColor = new Zivid.NET.ColorBGRA { b = 0, g = 0, r = 255, a = 255 };

                var projectorImage = CreateProjectorImage(projectorResolution, redColor);

                var projectedImageHandle = Zivid.NET.Projection.Projection.ShowImage(camera, projectorImage);

                Console.WriteLine("Press enter to stop projecting using the \".Stop()\" function");
                Console.ReadLine();
                projectedImageHandle.Stop();

                var greenColor = new Zivid.NET.ColorBGRA { b = 0, g = 255, r = 0, a = 255 };
                projectorImage = CreateProjectorImage(projectorResolution, greenColor);
                using (projectedImageHandle = Zivid.NET.Projection.Projection.ShowImage(camera, projectorImage))
                {
                    Console.WriteLine("Press enter to stop projecting by leaving a local scope");
                    Console.ReadLine();
                }

                var zividPinkColor = new Zivid.NET.ColorBGRA { b = 114, g = 52, r = 237, a = 255 };
                projectorImage = CreateProjectorImage(projectorResolution, zividPinkColor);
                projectedImageHandle = Zivid.NET.Projection.Projection.ShowImage(camera, projectorImage);

                Console.WriteLine("Press enter to stop projecting by performing a 3D capture");
                Console.ReadLine();
                var settings = new Zivid.NET.Settings
                {
                    Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
                };
                using (var frame3D = camera.Capture(settings)) { }
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
