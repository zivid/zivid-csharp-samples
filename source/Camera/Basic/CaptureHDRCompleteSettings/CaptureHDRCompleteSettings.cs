/*
Capture point clouds, with color, from the Zivid camera with fully configured settings.

For scenes with high dynamic range we combine multiple acquisitions to get an HDR
point cloud. This example shows how to fully configure settings for each acquisition.
In general, capturing an HDR point cloud is a lot simpler than this. The purpose of
this example is to demonstrate how to configure all the settings.

This sample also demonstrates how to save and load settings from file.

Note: This example uses experimental SDK features, which may be modified, moved, or deleted in the future without notice.
*/

using System;
using Duration = Zivid.NET.Duration;
using ColorModeOption =
    Zivid.NET.Settings.ProcessingGroup.ColorGroup.ExperimentalGroup.ModeOption;
using ReflectionFilterModeOption =
    Zivid.NET.Settings.ProcessingGroup.FiltersGroup.ReflectionGroup.RemovalGroup.ExperimentalGroup.ModeOption;
class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring settings for capture:");
            var settings = new Zivid.NET.Settings()
            {
                Experimental = { Engine = Zivid.NET.Settings.ExperimentalGroup.EngineOption.Phase },
                Sampling = { Color = Zivid.NET.Settings.SamplingGroup.ColorOption.Rgb, Pixel = Zivid.NET.Settings.SamplingGroup.PixelOption.All },
                RegionOfInterest = { Box = {
                                        Enabled = true,
                                        PointO = new Zivid.NET.PointXYZ{ x = 1000, y = 1000, z = 1000 },
                                        PointA = new Zivid.NET.PointXYZ{ x = 1000, y = -1000, z = 1000 },
                                        PointB = new Zivid.NET.PointXYZ{ x = -1000, y = 1000, z = 1000 },
                                        Extents = new Zivid.NET.Range<double>(-1000, 1000),
                                    },
                                    Depth =
                                    {
                                        Enabled = true,
                                        Range = new Zivid.NET.Range<double>(200, 2000),
                                    }
                },
                Processing = { Filters = { Smoothing = { Gaussian = { Enabled = true, Sigma = 1.5 } },
                                           Noise = { Removal = { Enabled = true, Threshold = 7.0 },
                                                     Suppression = { Enabled = true },
                                                     Repair ={ Enabled = true } },
                                           Outlier = { Removal = { Enabled = true, Threshold = 5.0 } },
                                           Reflection = { Removal = { Enabled = true, Experimental = { Mode = ReflectionFilterModeOption.Global} } },
                                           Cluster = { Removal = { Enabled = true, MaxNeighborDistance = 10, MinArea = 100} },
                                           Experimental = { ContrastDistortion = { Correction = { Enabled = true,
                                                                                                  Strength = 0.4 },
                                                                                   Removal = { Enabled = true,
                                                                                               Threshold = 0.5 } },
                                                            HoleFilling = { Enabled = true,
                                                                            HoleSize = 0.2,
                                                                            Strictness = 1 } } },
                               Color = { Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 },
                                         Gamma = 1.0,
                                         Experimental = { Mode = ColorModeOption.Automatic } } }
            };
            Console.WriteLine(settings);

            Console.WriteLine("Configuring base acquisition with settings same for all HDR acquisitions:");
            var baseAcquisition = new Zivid.NET.Settings.Acquisition { };
            Console.WriteLine(baseAcquisition);

            Console.WriteLine("Configuring acquisition settings different for all HDR acquisitions:");
            Tuple<double[], Duration[], double[], double[]> exposureValues = GetExposureValues(camera);
            double[] aperture = exposureValues.Item1;
            Duration[] exposureTime = exposureValues.Item2;
            double[] gain = exposureValues.Item3;
            double[] brightness = exposureValues.Item4;
            for (int i = 0; i < aperture.Length; i++)
            {
                Console.WriteLine("Acquisition {0}:", i + 1);
                Console.WriteLine("  Exposure Time: {0}", exposureTime[i].Microseconds);
                Console.WriteLine("  Aperture: {0}", aperture[i]);
                Console.WriteLine("  Gain: {0}", gain[i]);
                Console.WriteLine("  Brightness: {0}", brightness[i]);
                var acquisitionSettings = baseAcquisition.CopyWith(s =>
                                                                   {
                                                                       s.Aperture = aperture[i];
                                                                       s.ExposureTime = exposureTime[i];
                                                                       s.Gain = gain[i];
                                                                       s.Brightness = brightness[i];
                                                                   });
                settings.Acquisitions.Add(acquisitionSettings);
            }

            Console.WriteLine("Capturing frame (HDR)");
            using (var frame = camera.Capture(settings))
            {
                Console.WriteLine("Complete settings used:");
                Console.WriteLine(frame.Settings);

                var dataFile = "Frame.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
                frame.Save(dataFile);

                var settingsFile = "Settings.yml";
                Console.WriteLine("Saving settings to file: " + settingsFile);
                settings.Save(settingsFile);

                Console.WriteLine("Loading settings from file: " + settingsFile);
                var settingsFromFile = new Zivid.NET.Settings(settingsFile);
                Console.WriteLine(settingsFromFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }

    static Tuple<double[], Duration[], double[], double[]> GetExposureValues(Zivid.NET.Camera camera)
    {
        var model = camera.Info.Model;
        switch (model)
        {
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusSmall:
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusMedium:
            case Zivid.NET.CameraInfo.ModelOption.ZividOnePlusLarge:
                {
                    double[] aperture = { 8.0, 4.0, 1.4 };
                    Duration[] exposureTime = { Duration.FromMicroseconds(6500), Duration.FromMicroseconds(10000), Duration.FromMicroseconds(40000) };
                    double[] gain = { 1.0, 1.0, 2.0 };
                    double[] brightness = { 1.8, 1.8, 1.8 };
                    return Tuple.Create<double[], Duration[], double[], double[]>(aperture, exposureTime, gain, brightness);
                }
            case Zivid.NET.CameraInfo.ModelOption.ZividTwo:
            case Zivid.NET.CameraInfo.ModelOption.ZividTwoL100:
                {
                    double[] aperture = { 5.66, 2.38, 1.8 };
                    Duration[] exposureTime = { Duration.FromMicroseconds(1677), Duration.FromMicroseconds(5000), Duration.FromMicroseconds(100000) };
                    double[] gain = { 1.0, 1.0, 1.0 };
                    double[] brightness = { 1.8, 1.8, 1.8 };
                    return Tuple.Create<double[], Duration[], double[], double[]>(aperture, exposureTime, gain, brightness);
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM130:
                {
                    double[] aperture = { 5.66, 2.38, 2.1 };
                    Duration[] exposureTime = { Duration.FromMicroseconds(1677), Duration.FromMicroseconds(5000), Duration.FromMicroseconds(100000) };
                    double[] gain = { 1.0, 1.0, 1.0 };
                    double[] brightness = { 2.5, 2.5, 2.5 };
                    return Tuple.Create<double[], Duration[], double[], double[]>(aperture, exposureTime, gain, brightness);
                }
            default: throw new System.InvalidOperationException("Unhandled enum value " + camera.Info.Model.ToString());
        }
    }
}
