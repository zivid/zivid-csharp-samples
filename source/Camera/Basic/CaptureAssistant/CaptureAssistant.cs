using System;
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

            var suggestSettingsParameters = new Zivid.NET.CaptureAssistant.SuggestSettingsParameters(Duration.FromMilliseconds(1200), Zivid.NET.CaptureAssistant.AmbientLightFrequency.none);
            Console.WriteLine("Running Capture Assistant with parameters: {0}", suggestSettingsParameters);
            var settingsList = Zivid.NET.CaptureAssistant.SuggestSettings(camera, suggestSettingsParameters);

            Console.WriteLine("Suggested settings are:");
            foreach (var settings in settingsList)
            {
                Console.WriteLine(settings);
            }

            Console.WriteLine("Capture (and merge) frames using automatically suggested settings");
            var hdrFrame = Zivid.NET.HDR.Capture(camera, settingsList);

            string resultFile = "Result.zdf";
            Console.WriteLine("Saving frame to file: " + resultFile);
            hdrFrame.Save(resultFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
