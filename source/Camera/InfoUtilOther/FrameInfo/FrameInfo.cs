/*
Read frame info from the Zivid camera.

The frame info consists of the version information for installed software at the time of capture,
information about the system that captured the frame, and the time stamp of the capture.
*/

using System;
using Zivid.NET;
using Zivid.NET.Calibration;
using Zivid.NET.CaptureAssistant;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            var frame = AssistedCapture(camera);

            var frameInfo = frame.Info;

            Console.WriteLine("The version information for installed software at the time of image capture:");
            Console.WriteLine(frameInfo.SoftwareVersion);

            Console.WriteLine("Information about the system that captured this frame:");
            Console.WriteLine(frameInfo.SystemInfo);

            Console.WriteLine("The time of frame capture:");
            Console.WriteLine(frameInfo.TimeStamp);

            Console.WriteLine("Acquisition time:");
            Console.WriteLine(frameInfo.Metrics.AcquisitionTime.Milliseconds + " ms");

            Console.WriteLine("Capture time:");
            Console.WriteLine(frameInfo.Metrics.CaptureTime.Milliseconds + " ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static Zivid.NET.Frame AssistedCapture(Zivid.NET.Camera camera)
    {
        var suggestSettingsParameters = new Zivid.NET.CaptureAssistant.SuggestSettingsParameters
        {
            AmbientLightFrequency =
                Zivid.NET.CaptureAssistant.SuggestSettingsParameters.AmbientLightFrequencyOption.none,
            MaxCaptureTime = Duration.FromMilliseconds(800)
        };
        var settings = Zivid.NET.CaptureAssistant.Assistant.SuggestSettings(camera, suggestSettingsParameters);
        return camera.Capture(settings);
    }
}
