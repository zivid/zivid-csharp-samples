/*
Capture 2D images from the Zivid camera.

The color information is provided in linear RGB and sRGB color spaces.

Color represented in linear RGB space is suitable as input to traditional computer vision algorithms
for specialized tasks that require precise color measurements or high dynamic range.

Color represented in sRGB color space is suitable for showing an image on a display and for machine
learning based tasks like image classification, object detection, and segmentation as most image
datasets used for training neural networks are in sRGB color space.

More information about linear RGB and sRGB color spaces is available at:
https://support.zivid.com/en/latest/reference-articles/color-spaces-and-output-formats.html#color-spaces

Note: While the data of the saved images is provided in linear RGB and sRGB color space, the meta data
information that indicates the color space is not saved in the .PNG. Hence, both images are likely
to be interpreted as if they were saved in sRGB color space and displayed as such.
*/

using System;
using Duration = Zivid.NET.Duration;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring 2D settings");
            // Note: The Zivid SDK supports 2D captures with a single acquisition only
            var settings2D = new Zivid.NET.Settings2D
            {
                Acquisitions = { new Zivid.NET.Settings2D.Acquisition {
                    Aperture = 9.51, ExposureTime = Duration.FromMicroseconds(20000), Gain = 2.0, Brightness = 1.80
                } },
                Processing = { Color = { Balance = { Red = 1.0, Blue = 1.0, Green = 1.0 } } }
            };

            Console.WriteLine("Capturing 2D frame");
            using (var frame2D = camera.Capture(settings2D))
            {
                Console.WriteLine("Getting color image (linear RGB color space)");
                var image = frame2D.ImageRGBA();

                var pixelRow = 100;
                var pixelCol = 50;
                Console.WriteLine("Extracting 2D pixel array");
                var pixelArrayRGB = image.ToArray();
                Console.WriteLine("Height: {0}, Width: {1}", pixelArrayRGB.GetLength(0), pixelArrayRGB.GetLength(1));
                Console.WriteLine("Color at pixel ({0},{1}):  R:{2}  G:{3}  B:{4}  A:{5}",
                                  pixelRow,
                                  pixelCol,
                                  pixelArrayRGB[pixelRow, pixelCol].r,
                                  pixelArrayRGB[pixelRow, pixelCol].g,
                                  pixelArrayRGB[pixelRow, pixelCol].b,
                                  pixelArrayRGB[pixelRow, pixelCol].a);

                Console.WriteLine("Extracting 3D array of bytes");
                var nativeArray = image.ToByteArray();
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

                var imageFile = "ImageRGB.png";
                Console.WriteLine("Saving 2D color image (linear RGB color space) to file: {0}", imageFile);
                image.Save(imageFile);

                Console.WriteLine("Getting color image (sRGB color space)");
                var imageSRGB = frame2D.ImageSRGB();

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
                nativeArray = imageSRGB.ToByteArray();
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

                var imageSRGBFile = "ImageSRGB.png";
                Console.WriteLine("Saving 2D color image  (sRGB color space) to file: {0}", imageSRGBFile);
                imageSRGB.Save(imageSRGBFile);
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
