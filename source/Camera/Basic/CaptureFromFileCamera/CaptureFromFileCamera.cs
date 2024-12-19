/*
Capture point clouds, with color, with the Zivid file camera.
This example can be used without access to a physical camera.

The file camera files are found in Zivid Sample Data with ZFC as the file extension.
See the instructions in README.md to download the Zivid Sample Data.
There are five available file cameras to choose from, one for each camera model.
The default file camera used in this sample is the Zivid 2 M70 file camera.
*/

using System;
using ReflectionFilterModeOption =
    Zivid.NET.Settings.ProcessingGroup.FiltersGroup.ReflectionGroup.RemovalGroup.ModeOption;

class Program
{
    static int Main(string[] args)
    {
        try
        {
            var userInput = ParseOptions(args);

            string fileCamera;
            if (userInput != null)
            {
                fileCamera = userInput;
            }
            else
            {
                fileCamera = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/FileCameraZivid2PlusMR60.zfc";
            }

            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Creating virtual camera using file: " + fileCamera);
            var camera = zivid.CreateFileCamera(fileCamera);

            Console.WriteLine("Configuring settings");
            var settings2D = new Zivid.NET.Settings2D
            {
                Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } },
                Processing =
                {
                    Color =
                    {
                        Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 }
                    }
                }
            };
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
                Processing =
                {
                    Filters =
                    {
                        Smoothing =
                        {
                            Gaussian = { Enabled = true, Sigma = 1.5 }
                        },
                        Reflection =
                        {
                            Removal = { Enabled = true, Mode = ReflectionFilterModeOption.Global}
                        }
                    }
                }
            };
            settings.Color = settings2D;

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture2D3D(settings))
            {
                var dataFile = "Frame.zdf";
                Console.WriteLine("Saving frame to file: " + dataFile);
                frame.Save(dataFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static ArgumentException UsageException()
    {
        return new ArgumentException("Usage: --file-camera <Path to the file camera .zfc file>");
    }

    static string ParseOptions(string[] args)
    {
        if (args.Length == 0)
        {
            return null;
        }
        if (args.Length == 2)
        {
            if (args[0].Equals("--file-camera"))
            {
                return args[1];
            }
        }
        throw UsageException();
    }
}
