/*
Read a 2D image from file and project it using the camera projector.

The image for this sample can be found under the main instructions for Zivid samples.
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
            using (var camera = zivid.ConnectCamera())
            {
                string projectorImageFileForGivenCamera = GetProjectorImageFileForCamera(camera);

                Console.WriteLine("Reading 2D image (of resolution matching the Zivid camera projector resolution) from file: " + projectorImageFileForGivenCamera);
                var projectorImageForGivenCamera = new Zivid.NET.ImageBGRA(projectorImageFileForGivenCamera);

                using (var projectedImageHandle = Zivid.NET.Experimental.Projection.Projection.ShowImage(camera, projectorImageForGivenCamera))
                { // A Local Scope to handle the projected image lifetime

                    var settings2D = new Zivid.NET.Settings2D
                    {
                        Acquisitions = { new Zivid.NET.Settings2D.Acquisition {
                            Aperture = 2.83, ExposureTime = Duration.FromMicroseconds(20000), Brightness = 0.0} }
                    };

                    Console.WriteLine("Capturing a 2D image with the projected image");
                    using (var frame2D = projectedImageHandle.Capture(settings2D))
                    {
                        var capturedImageFile = "CapturedImage.png";
                        Console.WriteLine("Saving the captured image: {0}", capturedImageFile);
                        frame2D.ImageRGBA().Save(capturedImageFile);
                    }

                    Console.WriteLine("Press enter to stop projecting...");
                    Console.ReadLine();

                } // projectedImageHandle now goes out of scope, thereby stopping the projection

            }

            Console.WriteLine("Done");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
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
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/ZividLogoZivid2PlusProjectorResolution.png";
                }
            default:
                {
                    throw new System.InvalidOperationException("Unhandled enum value " + model.ToString());
                }
        }
    }
}
