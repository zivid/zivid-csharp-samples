/*
Poll the camera health check from a separate thread while capturing in the main thread, printing the statuses and values every second.
*/

using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void PrintHealthcheck(Zivid.NET.CameraHealth health)
    {
        var temperature = health.Temperature;
        Console.WriteLine("Overall:                {0}", health.Overall);
        Console.WriteLine("  Max transfer speed:   {0} ({1} Mbps)", health.MaxTransferSpeed.Status, health.MaxTransferSpeed.Value);
        Console.WriteLine("  Temperature (DMD):    {0} ({1} C)", temperature.DMD.Status, temperature.DMD.Value);
        Console.WriteLine("  Temperature (LED):    {0} ({1} C)", temperature.LED.Status, temperature.LED.Value);
        Console.WriteLine("  Temperature (Lens):   {0} ({1} C)", temperature.Lens.Status, temperature.Lens.Value);
        Console.WriteLine("  Fan:                  {0} ({1})", health.Fan.Status, health.Fan.Value);
        Console.WriteLine("  Memory:               {0} ({1} errors)", health.Memory.Status, health.Memory.Value);
        Console.WriteLine("  Infield verification: {0} ({1})", health.InfieldVerification.Status, health.InfieldVerification.Value);
    }

    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            var pollInterval = TimeSpan.FromSeconds(1);
            var stop = new ManualResetEventSlim(false);

            var pollingTask = Task.Run(() =>
            {
                while (!stop.IsSet)
                {
                    PrintHealthcheck(camera.CheckHealth());
                    Console.WriteLine();
                    stop.Wait(pollInterval);
                }
            });

            var settings = new Zivid.NET.Settings { Acquisitions = { new Zivid.NET.Settings.Acquisition { } } };
            var captureCycle = TimeSpan.FromSeconds(5);
            const int numberOfCaptures = 5;

            for (int i = 1; i <= numberOfCaptures; ++i)
            {
                using (camera.Capture3D(settings)) { }
                Console.WriteLine("Captured frame {0} of {1}", i, numberOfCaptures);
                if (i < numberOfCaptures)
                {
                    Thread.Sleep(captureCycle);
                }
            }

            stop.Set();
            pollingTask.Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
