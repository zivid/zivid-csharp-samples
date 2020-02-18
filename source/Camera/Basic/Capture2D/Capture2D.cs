using System;
using System.Drawing;

using Duration = Zivid.NET.Duration;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Setting the capture settings");
            var settings = new Zivid.NET.Settings2D()
            {
                Iris = 22,
                ExposureTime = Duration.FromMicroseconds(10000),
                Gain = 1.0
            };

            Console.WriteLine("Capture a 2D frame");
            var frame2D = camera.Capture2D(settings);

            Console.WriteLine("Get RGBA8 image from Frame2D");
            var image = frame2D.Image<Zivid.NET.RGBA8>();

            Console.WriteLine("Image Width: {0}, Height: {1}", image.Width, image.Height);
            Console.WriteLine("Pixel (0, 0): R={0}, G={1}, B={2}, A={3}",
                              image[0, 0].r,
                              image[0, 0].g,
                              image[0, 0].b,
                              image[0, 0].a);

            byte[] byteArray = image.ToArray();
            Console.WriteLine("First four bytes:  [0]: {0}, [1]: {1}, [2]: {2}, [3]: {3}",
                              byteArray[0],
                              byteArray[1],
                              byteArray[2],
                              byteArray[3]);

            var resultFile = "image.png";
            Console.WriteLine("Saving the image to {0}", resultFile);
            image.Save(resultFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
