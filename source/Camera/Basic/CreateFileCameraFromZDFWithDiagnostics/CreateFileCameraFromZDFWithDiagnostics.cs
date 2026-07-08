/*
Capture a frame with diagnostics enabled and create a file camera from it.

A file camera is a virtual camera that replays captures offline using the raw sensor data
stored in the original frame. This allows you to develop and test without a physical camera.

The workflow:
1. Capture a frame with diagnostics enabled
2. Save the diagnostics frame as a .zdf file
3. Disconnect from the camera (no longer needed)
4. Load the .zdf frame and create a file camera from it
5. Adjust processing settings and capture from the file camera
6. Save the resulting frame

For more information about file cameras, check out this tutorial:
https://support.zivid.com/en/latest/camera/academy/camera/file-camera.html
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

            Console.WriteLine("Creating default settings");
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
                Color = new Zivid.NET.Settings2D
                {
                    Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } }
                }
            };
            Console.WriteLine("Enabling diagnostics");
            settings.Diagnostics.Enabled = true;

            Console.WriteLine("Capturing frame with diagnostics");
            var frameWithDiagnostics = camera.Capture2D3D(settings);

            var frameWithDiagnosticsFile = "FrameWithDiagnostics.zdf";
            Console.WriteLine("Saving diagnostics frame to: " + frameWithDiagnosticsFile);
            frameWithDiagnostics.Save(frameWithDiagnosticsFile);

            Console.WriteLine("Disconnecting from camera");
            camera.Disconnect();

            Console.WriteLine("Loading ZDF with diagnostics enabled from: " + frameWithDiagnosticsFile);
            var loadedFrameWithDiagnostics = new Zivid.NET.Frame(frameWithDiagnosticsFile);

            Console.WriteLine("Creating file camera from frame");
            var fileCamera = zivid.CreateFileCamera(loadedFrameWithDiagnostics);

            Console.WriteLine("File camera info: " + fileCamera.Info);

            Console.WriteLine("Configuring settings");
            var settingsFromFrame = loadedFrameWithDiagnostics.Settings;
            settingsFromFrame.Diagnostics.Enabled = false;
            settingsFromFrame.Processing.Filters.Smoothing.Gaussian.Enabled = true;
            settingsFromFrame.Processing.Filters.Smoothing.Gaussian.Sigma = 1.5;
            settingsFromFrame.Processing.Filters.Reflection.Removal.Enabled = true;
            settingsFromFrame.Processing.Filters.Reflection.Removal.Mode = Zivid.NET.Settings.ProcessingGroup.FiltersGroup.ReflectionGroup.RemovalGroup.ModeOption.Global;

            Console.WriteLine("Capturing from file camera");
            var fileCameraFrame = fileCamera.Capture2D3D(settingsFromFrame);

            var fileCameraFrameFile = "FrameFromFileCameraWithNoDiagnostics.zdf";
            Console.WriteLine("Saving file camera frame to: " + fileCameraFrameFile);
            fileCameraFrame.Save(fileCameraFrameFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
