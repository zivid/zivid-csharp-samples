/*
Read intrinsic parameters from the Zivid camera (OpenCV model) or estimate them from the point cloud.

Note: This example uses experimental SDK features, which may be modified, moved, or deleted in the future without notice.
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

            Console.WriteLine("Getting camera intrinsics");
            var intrinsics = Zivid.NET.Experimental.Calibration.Calibrator.Intrinsics(camera);

            Console.WriteLine(intrinsics.ToString());

            var intrinsicsFile = "Intrinsics.yml";
            Console.WriteLine("Saving camera intrinsics to file: " + intrinsicsFile);
            intrinsics.Save(intrinsicsFile);

            PrintIntrinsicParameters(intrinsics);

            Console.WriteLine("\nDifference between fixed intrinsics and estimated intrinsics for different apertures and temperatures:");

            var apertures = new double[] { 5.66, 4.00, 2.83 };
            if (camera.Info.Model == Zivid.NET.CameraInfo.ModelOption.Zivid3XL250)
            {
                apertures = new double[] { 3.00, 3.00, 3.00 };
            }

            foreach (var aperture in apertures)
            {
                var settings = new Zivid.NET.Settings
                {
                    Acquisitions = { new Zivid.NET.Settings.Acquisition { Aperture = aperture } },
                    Color = new Zivid.NET.Settings2D { Acquisitions = { new Zivid.NET.Settings2D.Acquisition { Aperture = aperture } } }
                };
                using (var frame = camera.Capture2D3D(settings))
                {
                    var estimatedIntrinsics = Zivid.NET.Experimental.Calibration.Calibrator.EstimateIntrinsics(frame);
                    var temperature = frame.State.Temperature.Lens;
                    Console.WriteLine($"\nAperture: {aperture,5:f2}, Lens Temperature: {temperature,5:f2}°C");
                    PrintIntrinsicParametersDelta(intrinsics, estimatedIntrinsics);
                }
            }
            var settingsSubsampled = SubsampledSettingsForCamera(camera);
            var fixedIntrinsicsForSubsampledSettingsPath = "FixedIntrinsicsSubsampled2x2.yml";
            Console.WriteLine("Saving camera intrinsics for subsampled capture to file: " + fixedIntrinsicsForSubsampledSettingsPath);
            var fixedIntrinsicsForSubsampledSettings = Zivid.NET.Experimental.Calibration.Calibrator.Intrinsics(camera, settingsSubsampled);
            fixedIntrinsicsForSubsampledSettings.Save(fixedIntrinsicsForSubsampledSettingsPath);

            using (var frameSubsampled = camera.Capture2D3D(settingsSubsampled))
            {
                var estimatedIntrinsicsForSubsampledSettings = Zivid.NET.Experimental.Calibration.Calibrator.EstimateIntrinsics(frameSubsampled);
                var estimatedIntrinsicsForSubsampledSettingsPath = "EstimatedIntrinsicsFromSubsampled2x2Capture.yml";
                Console.WriteLine("Saving estimated camera intrinsics for subsampled capture to file: " + fixedIntrinsicsForSubsampledSettingsPath);
                estimatedIntrinsicsForSubsampledSettings.Save(estimatedIntrinsicsForSubsampledSettingsPath);
                Console.WriteLine("\nDifference between fixed and estimated intrinsics for subsampled point cloud:");
                PrintIntrinsicParametersDelta(fixedIntrinsicsForSubsampledSettings, estimatedIntrinsicsForSubsampledSettings);
            }

            var settings2D = new Zivid.NET.Settings2D
            {
                Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } }
            };
            Console.WriteLine("Getting camera intrinsics for 2D settings");
            var fixedIntrinsicsForSettings2D = Zivid.NET.Experimental.Calibration.Calibrator.Intrinsics(camera, settings2D);
            Console.WriteLine(fixedIntrinsicsForSettings2D.ToString());
            var fixedIntrinsicsForSettings2DPath = "FixedIntrinsicsSettings2D.yml";
            Console.WriteLine("Saving camera intrinsics for 2D settings to file: " + fixedIntrinsicsForSettings2DPath);
            fixedIntrinsicsForSettings2D.Save(fixedIntrinsicsForSettings2DPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

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

    static Zivid.NET.Settings SubsampledSettingsForCamera(Zivid.NET.Camera camera)
    {
        var settingsSubsampled = new Zivid.NET.Settings
        {
            Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
            Color = new Zivid.NET.Settings2D { Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } } }
        };
        var model = camera.Info.Model;
        switch (model)
        {
            case Zivid.NET.CameraInfo.ModelOption.ZividTwo:
            case Zivid.NET.CameraInfo.ModelOption.ZividTwoL100:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM130:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM60:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusL110:
                {
                    settingsSubsampled.Sampling.Pixel = Zivid.NET.Settings.SamplingGroup.PixelOption.BlueSubsample2x2;
                    settingsSubsampled.Color.Sampling.Pixel = Zivid.NET.Settings2D.SamplingGroup.PixelOption.BlueSubsample2x2;
                    break;
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR130:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR60:
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusLR110:
            case Zivid.NET.CameraInfo.ModelOption.Zivid3XL250:
                {

                    settingsSubsampled.Sampling.Pixel = Zivid.NET.Settings.SamplingGroup.PixelOption.By2x2;
                    settingsSubsampled.Color.Sampling.Pixel = Zivid.NET.Settings2D.SamplingGroup.PixelOption.By2x2;
                    break;
                }
            default: throw new System.InvalidOperationException("Unhandled enum value " + camera.Info.Model.ToString());
        }

        return settingsSubsampled;
    }
}
