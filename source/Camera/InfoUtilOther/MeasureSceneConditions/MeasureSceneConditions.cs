/*
Measure ambient light conditions in the scene and output the measured flickering frequency of the ambient light if flickering is detected.
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
            Console.WriteLine($"Connected to {camera.Info.SerialNumber} {camera.Info.ModelName}");

            Console.WriteLine("Measuring scene conditions");
            var sceneConditions = camera.MeasureSceneConditions();
            var FlickerClassification = sceneConditions.AmbientLight.FlickerClassification.ToString();

            if (FlickerClassification == "NoFlicker")
            {
                Console.WriteLine("No flickering lights were detected in the scene.");
                var SettingsPath = new Zivid.NET.Settings(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                               + "/Settings/" + SanitizedModelName(camera) + "_ConsumerGoodsFast.yml");
                Console.WriteLine(SettingsPath);
                return 0;
            }

            var cameraSpecificPresets = Zivid.NET.Presets.Presets.Categories(camera.Info.Model);

            if (FlickerClassification == "UknownFlicker")
            {
                var unknownFlickerFrequency = sceneConditions.AmbientLight.FlickerFrequency;
                Console.WriteLine("Flickering not found to match any known grid frequency.");
                Console.WriteLine($"Measured flickering frequency in the scene is: {unknownFlickerFrequency} Hz.");
                Console.WriteLine("This is a non-standard flickering frequency. Consider adjusting the exposure time to be a multiple of this frequency to avoid artifacts.");
                return 0;
            }
            else if (FlickerClassification == "Grid50hz")
            {
                Console.WriteLine("Found flickering corresponding to 50 Hz frequency in the scene, applying compensated preset:");
                var Settings50HzPath = new Zivid.NET.Settings(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                               + "/Settings/" + SanitizedModelName(camera) + "_ConsumerGoodsFast.yml");
                Console.WriteLine(Settings50HzPath);
            }
            else if (FlickerClassification == "Grid60hz")
            {
                Console.WriteLine("Found flickering corresponding to 60 Hz frequency in the scene, applying compensated preset:");
                var Settings60HzPath = new Zivid.NET.Settings(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                               + "/Settings/" + SanitizedModelName(camera) + "_ConsumerGoodsFast.yml");
                Console.WriteLine(Settings60HzPath);
            }

            var flickerFrequency = sceneConditions.AmbientLight.FlickerFrequency;
            Console.WriteLine($"Measured flickering frequency in the scene: {flickerFrequency} Hz.");

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static string SanitizedModelName(Zivid.NET.Camera camera)
    {
        var model = camera.Info.Model;
        switch (model)
        {
            case Zivid.NET.CameraInfo.ModelOption.ZividTwo: return "Zivid_Two_M70";
            case Zivid.NET.CameraInfo.ModelOption.ZividTwoL100: return "Zivid_Two_L100";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM130: return "Zivid_Two_Plus_M130";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM60: return "Zivid_Two_Plus_M60";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusL110: return "Zivid_Two_Plus_L110";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR130: return "Zivid_Two_Plus_MR130";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR60: return "Zivid_Two_Plus_MR60";
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusLR110: return "Zivid_Two_Plus_LR110";
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusSmall: return "Zivid_One_Plus_Small";
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusMedium: return "Zivid_One_Plus_Medium";
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusLarge: return "Zivid_One_Plus_Large";
            default: throw new System.InvalidOperationException("Unhandled camera model: " + model.ToString());
        }
    }
}
