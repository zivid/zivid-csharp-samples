// DOCTAG-START-ALL-1
/*
A basic warm-up method for a Zivid camera with specified time and capture cycle.
*/

using System;
using System.Threading;
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

            var warmupTime = TimeSpan.FromMinutes(10);
            var captureCycle = TimeSpan.FromSeconds(5);
            var maxCaptureTime = Duration.FromMilliseconds(1000);

            Console.WriteLine("Getting camera settings");
            var suggestSettingsParameters = new Zivid.NET.CaptureAssistant.SuggestSettingsParameters {
                MaxCaptureTime = maxCaptureTime,
                AmbientLightFrequency =
                    Zivid.NET.CaptureAssistant.SuggestSettingsParameters.AmbientLightFrequencyOption.none
            };
            var settings = Zivid.NET.CaptureAssistant.Assistant.SuggestSettings(camera, suggestSettingsParameters);

            DateTime beforeWarmup = DateTime.Now;

            Console.WriteLine("Starting warm up for: {0} minutes", warmupTime.Minutes);

            while(DateTime.Now.Subtract(beforeWarmup) < warmupTime)
            {
                var beforeCapture = DateTime.Now;
                camera.Capture(settings);
                var afterCapture = DateTime.Now;

                var captureTime = afterCapture.Subtract(beforeCapture);

                if(captureTime < captureCycle)
                {
                    Thread.Sleep(captureCycle.Subtract(captureTime));
                }
                else
                {
                    Console.WriteLine(
                        "Your capture time is longer than your desired capture cycle. Please increase the desired capture cycle.");
                }
                var remainingTime = warmupTime.Subtract(DateTime.Now.Subtract(beforeWarmup));
                var remainingMinutes = Math.Floor(remainingTime.TotalMinutes);
                var remainingSeconds = Math.Floor(remainingTime.TotalSeconds) % 60;

                Console.WriteLine("Remaining time: {0} minutes, {1} seconds.", remainingMinutes, remainingSeconds);
            }

            Console.WriteLine("Warm up completed");
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
