using System;
using Duration = Zivid.NET.Duration;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var resultFile = "result.zdf";

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Adjusting the iris");
            camera.UpdateSettings(s =>
            {
                s.Iris = 22;
                s.ExposureTime = Duration.FromMicroseconds(8333);
                s.Filters.Outlier.Enabled = true;
                s.Filters.Outlier.Threshold = 5;
            });

            Console.WriteLine("Capture a frame");
            var frame = camera.Capture();

            Console.WriteLine("Saving frame to file: " + resultFile);
            frame.Save(resultFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
