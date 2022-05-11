/*
Read intrinsic parameters from the Zivid camera (OpenCV model).

Note: This example uses experimental SDK features, which may be modified, moved, or deleted in the future without notice.
*/

using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Getting camera intrinsics");
            var intrinsics = Zivid.NET.Experimental.Calibration.Calibrator.Intrinsics(camera);

            Console.WriteLine(intrinsics.ToString());

            Console.WriteLine("Separated camera intrinsic parameters:");

            Console.WriteLine("    CX = " + intrinsics.CameraMatrix.CX.ToString());
            Console.WriteLine("    CY = " + intrinsics.CameraMatrix.CY.ToString());
            Console.WriteLine("    FX = " + intrinsics.CameraMatrix.FX.ToString());
            Console.WriteLine("    FY = " + intrinsics.CameraMatrix.FY.ToString());

            Console.WriteLine("    K1 = " + intrinsics.Distortion.K1.ToString());
            Console.WriteLine("    K2 = " + intrinsics.Distortion.K2.ToString());
            Console.WriteLine("    K3 = " + intrinsics.Distortion.K3.ToString());
            Console.WriteLine("    P1 = " + intrinsics.Distortion.P1.ToString());
            Console.WriteLine("    P2 = " + intrinsics.Distortion.P2.ToString());

            var intrinsicsFile = "Intrinsics.yml";
            Console.WriteLine("Saving camera intrinsics to file: " + intrinsicsFile);
            intrinsics.Save(intrinsicsFile);
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}