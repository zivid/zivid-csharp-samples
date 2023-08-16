/*
Read intrinsic parameters from the Zivid camera (OpenCV model) or estimate them from the point cloud.

Note: This example uses experimental SDK features, which may be modified, moved, or deleted in the future without notice.
*/

using System;

class Program
{
    static void PrintParameterDelta(String label, Double fixedValue, Double estimatedValue)
    {
        var delta = fixedValue - estimatedValue;
        if (delta != 0)
        {
            var deltaInPercentage = (100 * delta) / fixedValue;
            Console.WriteLine($"{label,6:s}: {delta,6:f2} ({deltaInPercentage,6:f2}% )");
        }
    }

    static void PrintIntrinsicParametersDelta(
        Zivid.NET.CameraIntrinsics fixedIntrinsics,
        Zivid.NET.CameraIntrinsics estimatedIntrinsics)
    {
        PrintParameterDelta("CX", fixedIntrinsics.CameraMatrix.CX, estimatedIntrinsics.CameraMatrix.CX);
        PrintParameterDelta("CY", fixedIntrinsics.CameraMatrix.CY, estimatedIntrinsics.CameraMatrix.CY);
        PrintParameterDelta("FX", fixedIntrinsics.CameraMatrix.FX, estimatedIntrinsics.CameraMatrix.FX);
        PrintParameterDelta("FY", fixedIntrinsics.CameraMatrix.FY, estimatedIntrinsics.CameraMatrix.FY);

        PrintParameterDelta("K1", fixedIntrinsics.Distortion.K1, estimatedIntrinsics.Distortion.K1);
        PrintParameterDelta("K2", fixedIntrinsics.Distortion.K2, estimatedIntrinsics.Distortion.K2);
        PrintParameterDelta("K3", fixedIntrinsics.Distortion.K3, estimatedIntrinsics.Distortion.K3);
        PrintParameterDelta("P1", fixedIntrinsics.Distortion.P1, estimatedIntrinsics.Distortion.P1);
        PrintParameterDelta("P2", fixedIntrinsics.Distortion.P2, estimatedIntrinsics.Distortion.P2);
    }

    static void PrintIntrinsicParameters(Zivid.NET.CameraIntrinsics intrinsics)
    {
        Console.WriteLine("Separated camera intrinsic parameters:");

        Console.WriteLine($"    CX = {intrinsics.CameraMatrix.CX,-7:f2}");
        Console.WriteLine($"    CY = {intrinsics.CameraMatrix.CY,-7:f2}");
        Console.WriteLine($"    FX = {intrinsics.CameraMatrix.FX,-7:f2}");
        Console.WriteLine($"    FY = {intrinsics.CameraMatrix.FY,-7:f2}");

        Console.WriteLine($"    K1 = {intrinsics.Distortion.K1,-7:f4}");
        Console.WriteLine($"    K2 = {intrinsics.Distortion.K2,-7:f4}");
        Console.WriteLine($"    K3 = {intrinsics.Distortion.K3,-7:f4}");
        Console.WriteLine($"    P1 = {intrinsics.Distortion.P1,-7:f4}");
        Console.WriteLine($"    P2 = {intrinsics.Distortion.P2,-7:f4}");
    }

    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Getting camera intrinsics");
            var intrinsics = Zivid.NET.Experimental.Calibration.Calibrator.Intrinsics(camera);

            Console.WriteLine(intrinsics.ToString());

            var intrinsicsFile = "Intrinsics.yml";
            Console.WriteLine("Saving camera intrinsics to file: " + intrinsicsFile);
            intrinsics.Save(intrinsicsFile);

            PrintIntrinsicParameters(intrinsics);

            Console.WriteLine("\nDifference between fixed intrinsics and estimated intrinsics for different apertures and temperatures:");

            foreach (var aperture in new double[] { 11.31, 5.66, 2.83 })
            {
                var settings = new Zivid.NET.Settings();
                settings.Acquisitions.Add(new Zivid.NET.Settings.Acquisition { Aperture = aperture });
                var frame = camera.Capture(settings);
                var estimatedIntrinsics = Zivid.NET.Experimental.Calibration.Calibrator.EstimateIntrinsics(frame);
                var temperature = frame.State.Temperature.Lens;
                Console.WriteLine($"\nAperture: {aperture,5:f2}, Lens Temperature: {temperature,5:f2}°C");
                PrintIntrinsicParametersDelta(intrinsics, estimatedIntrinsics);
            }

            if (camera.Info.Model != Zivid.NET.CameraInfo.ModelOption.ZividOnePlusSmall &&
                 camera.Info.Model != Zivid.NET.CameraInfo.ModelOption.ZividOnePlusMedium &&
                 camera.Info.Model != Zivid.NET.CameraInfo.ModelOption.ZividOnePlusLarge &&
                 camera.Info.Model != Zivid.NET.CameraInfo.ModelOption.ZividTwo &&
                 camera.Info.Model != Zivid.NET.CameraInfo.ModelOption.ZividTwoL100)
            {
                var settingsSubsampled = new Zivid.NET.Settings();
                settingsSubsampled.Acquisitions.Add(new Zivid.NET.Settings.Acquisition { });
                settingsSubsampled.Sampling.Pixel = Zivid.NET.Settings.Sampling.Pixel.blueSubsample2x2;
                var fixedIntrinsicsForSubsampledSettingsPath = "FixedIntrinsicsSubsampledBlue2x2.yml";
                Console.WriteLine("Saving camera intrinsics for subsampled capture to file: " + fixedIntrinsicsForSubsampledSettingsPath);
                var fixedIntrinsicsForSubsampledSettings = Zivid.NET.Experimental.Calibration.Intrinsics(camera, settingsSubsampled);
                fixedIntrinsicsForSubsampledSettings.Save(fixedIntrinsicsForSubsampledSettingsPath);
                var frame = camera.Capture(settingsSubsampled);
                var estimatedIntrinsicsForSubsampledSettings = Zivid.NET.Experimental.Calibration.EstimateIntrinsics(frame);
                const std::string estimatedIntrinsicsForSubsampledSettingsPath = "EstimatedIntrinsicsFromSubsampledBlue2x2Capture.yml";
                Console.WriteLine("Saving estimated camera intrinsics for subsampled capture to file: " + fixedIntrinsicsForSubsampledSettingsPath);
                estimatedIntrinsicsForSubsampledSettings.Save(estimatedIntrinsicsForSubsampledSettingsPath);
                Console.WriteLine("\nDifference between fixed and estimated intrinsics for subsampled point cloud:");
                PrintIntrinsicParametersDelta(fixedIntrinsicsForSubsampledSettings, estimatedIntrinsicsForSubsampledSettings);
            }
            else
            {
                Console.WriteLine(camera.Info.ModelName + " does not support sub-sampled mode.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
