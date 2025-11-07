/*
Measure ambient light conditions in the scene and output the measured flickering frequency of the ambient light if flickering is detected.
*/

using System;
using System.IO;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();
            Console.WriteLine($"Connected to {camera.Info.SerialNumber} {camera.Info.ModelName}");

            Console.WriteLine("Measuring scene conditions");
            var sceneConditions = camera.MeasureSceneConditions();
            var flickerClassification = sceneConditions.AmbientLight.FlickerClassification.ToString();
            Console.WriteLine("Flicker classification: " + flickerClassification);

            if (flickerClassification != "NoFlicker")
            {
                var flickerFrequency = sceneConditions.AmbientLight.FlickerFrequency;
                Console.WriteLine($"Measured flickering frequency in the scene: {flickerFrequency} Hz.");
            }

            var settingsPath = "";
            if (flickerClassification != "UnknownFlicker")
            {
                settingsPath = FindSettings2D3D(camera);
            }

            if (flickerClassification == "NoFlicker")
            {
                Console.WriteLine("No flickering lights were detected in the scene.");
            }
            else if (flickerClassification == "UnknownFlicker")
            {
                Console.WriteLine("Flickering not found to match any known grid frequency.");
                Console.WriteLine("This is a non-standard flickering frequency. Consider adjusting the exposure time to be a multiple of this frequency to avoid artifacts.");
                return 0;
            }
            else if (flickerClassification == "Grid50hz")
            {
                Console.WriteLine("Found flickering corresponding to 50 Hz frequency in the scene, applying compensated preset:");
                settingsPath = AddSuffixBeforeExtension(settingsPath, "_50Hz");
            }
            else if (flickerClassification == "Grid60hz")
            {
                Console.WriteLine("Found flickering corresponding to 60 Hz frequency in the scene, applying compensated preset:");
                settingsPath = AddSuffixBeforeExtension(settingsPath, "_60Hz");
            }
            else
            {
                Console.WriteLine("Invalid flicker classification");
                return 1;
            }
            Console.WriteLine(settingsPath);

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static string FindSettings2D3D(Zivid.NET.Camera camera)
    {
        var presetsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            + "/Settings/";
        var settings2D3DPath = "";

        var model = camera.Info.Model;
        switch (model)
        {
            case Zivid.NET.CameraInfo.ModelOption.ZividTwo:
                {
                    settings2D3DPath = presetsPath + "/Zivid_Two_M70_ManufacturingSpecular.yml";
                    break;
                }
            case Zivid.NET.CameraInfo.ModelOption.ZividTwoL100:
                {
                    settings2D3DPath = presetsPath + "/Zivid_Two_L100_ManufacturingSpecular.yml";
                    break;
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM130:
                {
                    settings2D3DPath = presetsPath + "/Zivid_Two_Plus_M130_ConsumerGoodsQuality.yml";
                    break;
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM60:
                {
                    settings2D3DPath = presetsPath + "/Zivid_Two_Plus_M60_ConsumerGoodsQuality.yml";
                    break;
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusL110:
                {
                    settings2D3DPath = presetsPath + "/Zivid_Two_Plus_L110_ConsumerGoodsQuality.yml";
                    break;
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR130:
                {
                    settings2D3DPath = presetsPath + "/Zivid_Two_Plus_MR130_ConsumerGoodsQuality.yml";
                    break;
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR60:
                {
                    settings2D3DPath = presetsPath + "/Zivid_Two_Plus_MR60_ConsumerGoodsQuality.yml";
                    break;
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusLR110:
                {
                    settings2D3DPath = presetsPath + "/Zivid_Two_Plus_LR110_ConsumerGoodsQuality.yml";
                    break;
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid3XL250:
                {
                    settings2D3DPath = presetsPath + "/Zivid_Three_XL250_DepalletizationQuality.yml";
                    break;
                }
            default: throw new System.InvalidOperationException("Unhandled camera model: " + model.ToString());
        }
        return settings2D3DPath;
    }
    static string AddSuffixBeforeExtension(string path, string suffix)
    {
        int posDot = path.LastIndexOf('.');
        return path.Substring(0, posDot) + suffix + path.Substring(posDot);
    }
}
