/*
This example shows how to read the intrinsic calibration parameters of the
Zivid camera (OpenCV model).
*/

using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to the camera");
            var camera = zivid.ConnectCamera();

            var fileNameIntrinsics = "intrinsics.yml";
            Console.WriteLine("Saving camera intrinsics to " + fileNameIntrinsics);
            camera.Intrinsics.save(fileNameIntrinsics);

            Console.WriteLine(camera.Intrinsics.ToString());

            Console.WriteLine("CX = " + camera.Intrinsics.CameraMatrix.CX.ToString());
            Console.WriteLine("CY = " + camera.Intrinsics.CameraMatrix.CY.ToString());
            Console.WriteLine("FX = " + camera.Intrinsics.CameraMatrix.FX.ToString());
            Console.WriteLine("FY = " + camera.Intrinsics.CameraMatrix.FY.ToString());

            Console.WriteLine("K1 = " + camera.Intrinsics.Distortion.K1.ToString());
            Console.WriteLine("K2 = " + camera.Intrinsics.Distortion.K2.ToString());
            Console.WriteLine("K3 = " + camera.Intrinsics.Distortion.K3.ToString());
            Console.WriteLine("P1 = " + camera.Intrinsics.Distortion.K1.ToString());
            Console.WriteLine("P2 = " + camera.Intrinsics.Distortion.K2.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}