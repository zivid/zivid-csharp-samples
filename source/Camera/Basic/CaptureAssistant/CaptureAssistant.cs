/*
Use Capture Assistant to capture point clouds, with color, from the Zivid camera.
*/

using System;
using Duration = Zivid.NET.Duration;
using ReflectionFilterModeOption =
    Zivid.NET.Settings.ProcessingGroup.FiltersGroup.ReflectionGroup.RemovalGroup.ModeOption;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            var suggestSettingsParameters = new Zivid.NET.CaptureAssistant.SuggestSettingsParameters
            {
                AmbientLightFrequency =
                    Zivid.NET.CaptureAssistant.SuggestSettingsParameters.AmbientLightFrequencyOption.none,
                MaxCaptureTime = Duration.FromMilliseconds(1200)
            };

            Console.WriteLine("Running Capture Assistant with parameters:\n{0}", suggestSettingsParameters);
            var settings = Zivid.NET.CaptureAssistant.Assistant.SuggestSettings(camera, suggestSettingsParameters);

            Console.WriteLine("Settings suggested by Capture Assistant:");
            Console.WriteLine(settings.Acquisitions);

            Console.WriteLine(
                "Manually configuring processing settings (Capture Assistant only suggests acquisition settings)");
            settings.Processing.Filters.Reflection.Removal.Enabled = true;
            settings.Processing.Filters.Reflection.Removal.Mode = ReflectionFilterModeOption.Global;
            settings.Processing.Filters.Smoothing.Gaussian.Enabled = true;
            settings.Processing.Filters.Smoothing.Gaussian.Sigma = 1.5;

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture(settings))
            {
                string dataFile = "Frame.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
                frame.Save(dataFile);
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
