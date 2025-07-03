/*
Read a 2D image from file and project it using the camera projector.

The image for this sample can be found under the main instructions for Zivid samples.
*/

using System;
using System.Reflection;
using System.Runtime;
using Zivid.NET;
using Duration = Zivid.NET.Duration;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            using (var camera = zivid.ConnectCamera())
            {
                string projectorImageFileForGivenCamera = GetProjectorImageFileForCamera(camera);

                Console.WriteLine("Reading 2D image (of resolution matching the Zivid camera projector resolution) from file: " + projectorImageFileForGivenCamera);
                var projectorImageForGivenCamera = new Zivid.NET.ImageBGRA(projectorImageFileForGivenCamera);

                using (var projectedImageHandle = Zivid.NET.Projection.Projection.ShowImage(camera, projectorImageForGivenCamera))
                { // A Local Scope to handle the projected image lifetime

                    var settings2D = MakeSettings2D();
                    if (!cameraSupportsProjectionBrightnessBoost(camera))
                    {
                        settings2D.Acquisitions[0].Brightness = 0.0;
                        settings2D.Sampling.Color = Zivid.NET.Settings2D.SamplingGroup.ColorOption.Rgb;
                    }

                    Console.WriteLine("Capturing a 2D image with the projected image");
                    using (var frame2D = projectedImageHandle.Capture2D(settings2D))
                    {
                        var capturedImageFile = "CapturedImage.png";
                        Console.WriteLine("Saving the captured image: {0}", capturedImageFile);
                        frame2D.ImageRGBA_SRGB().Save(capturedImageFile);
                    }

                    Console.WriteLine("Press enter to stop projecting...");
                    Console.ReadLine();

                } // projectedImageHandle now goes out of scope, thereby stopping the projection
            }

            Console.WriteLine("Done");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static string GetProjectorImageFileForCamera(Zivid.NET.Camera camera)
    {
        var model = camera.Info.Model;
        switch (model)
        {
            case Zivid.NET.CameraInfo.ModelOption.ZividTwo:
            case Zivid.NET.CameraInfo.ModelOption.ZividTwoL100:
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/ZividLogoZivid2ProjectorResolution.png";
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM130:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM60:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusL110:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR130:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR60:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusLR110:
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/ZividLogoZivid2PlusProjectorResolution.png";
                }
            default:
                {
                    throw new System.InvalidOperationException("Unhandled enum value " + model.ToString());
                }
        }
    }
    static Zivid.NET.Settings2D MakeSettings2D()
    {
        return new Zivid.NET.Settings2D
        {
            Acquisitions = { new Zivid.NET.Settings2D.Acquisition
            {
                Brightness = 2.5,
                ExposureTime = Duration.FromMicroseconds(20000),
                Aperture = 2.83
            }},
            Sampling = new Zivid.NET.Settings2D.SamplingGroup
            {
                Color = Zivid.NET.Settings2D.SamplingGroup.ColorOption.Grayscale
            }
        };
    }

    static bool cameraSupportsProjectionBrightnessBoost(Zivid.NET.Camera camera)
    {
        var model = camera.Info.Model;
        return model == Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR130 ||
               model == Zivid.NET.CameraInfo.ModelOption.Zivid2PlusLR110 ||
               model == Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR60;
    }
}
