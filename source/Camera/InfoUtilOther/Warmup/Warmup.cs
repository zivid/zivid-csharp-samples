// DOCTAG-START-ALL-1
/*
Short example of a basic way to warm up the camera with specified time and capture cycle.
*/

using CommandLine;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Duration = Zivid.NET.Duration;

class Program
{
    public class Options
    {
        [Option('s', "settings-path", Required = false, HelpText = "Path to the YML file that contains camera settings.")]
        public string SettingsPath { get; set; }

        [Option('c', "capture-cycle", Required = false, HelpText = "Capture cycle in seconds.")]
        public int? CaptureCycle { get; set; }
    }

    static Zivid.NET.Settings LoadOrDefaultSettings(string settingsPath)
    {
        if (settingsPath != null)
        {
            Console.WriteLine("Loading settings from file");
            return new Zivid.NET.Settings(settingsPath);
        }

        Console.WriteLine("Using default 3D settings");
        var settings = new Zivid.NET.Settings
        {
            Acquisitions = { new Zivid.NET.Settings.Acquisition { } }
        };

        return settings;
    }

    static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<Options>(args)
        .MapResult(
            (Options opts) => RunWarmupWithOptionsAndReturnExitCode(opts),
            errs => 1);
    }

    static int RunWarmupWithOptionsAndReturnExitCode(Options opts)
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            var warmupTime = TimeSpan.FromMinutes(10);
            var captureCycle = opts.CaptureCycle.HasValue ? TimeSpan.FromSeconds(opts.CaptureCycle.Value) : TimeSpan.FromSeconds(5);
            var settings = LoadOrDefaultSettings(opts.SettingsPath);

            DateTime beforeWarmup = DateTime.Now;

            Console.WriteLine("Starting warm up for: {0} minutes", warmupTime.Minutes);

            while (DateTime.Now.Subtract(beforeWarmup) < warmupTime)
            {
                var beforeCapture = DateTime.Now;

                // Use the same capture method as you would use in production
                // to get the most accurate results from warmup
                using (camera.Capture3D(settings)) { }

                var afterCapture = DateTime.Now;

                var captureTime = afterCapture.Subtract(beforeCapture);

                if (captureTime < captureCycle)
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
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
