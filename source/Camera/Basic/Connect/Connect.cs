/*
Connect to a Zivid camera using the different available methods.

Replace the IP address, serial number and hostname in the code with the ones of your camera,
or provide them with --serial, --ip and --hostname.
*/

using System;
using System.Collections.Generic;

class Program
{
    class CameraIdentifiers
    {
        public string SerialNumber = "2020C0DE";
        public string IPAddress = "172.28.60.5";
        public string Hostname = "zivid-2020C0DE.local";
    }

    static void PrintDiscoveredCameras(Zivid.NET.Application zivid)
    {
        Console.WriteLine("Discovered cameras:");
        foreach (var camera in zivid.Cameras)
        {
            Console.WriteLine("Serial number: " + camera.Info.SerialNumber
                              + ", IP address: " + camera.State.Network.IPV4.Address);
        }
    }

    static int Main(string[] args)
    {
        try
        {
            var identifiers = ParseOptions(args);

            var zivid = new Zivid.NET.Application();

            PrintDiscoveredCameras(zivid);

            {
                Console.WriteLine("Connecting to the first available camera");
                var camera = zivid.ConnectCamera();
                camera.Disconnect();
            }

            {
                Console.WriteLine("Connecting to the camera with serial number " + identifiers.SerialNumber);
                var camera = zivid.ConnectCamera(identifiers.SerialNumber);
                camera.Disconnect();
            }

            {
                Console.WriteLine("Connecting to the camera at IP address " + identifiers.IPAddress);
                var camera = zivid.ConnectCamera(new Zivid.NET.CameraAddress(identifiers.IPAddress));
                camera.Disconnect();
            }

            {
                Console.WriteLine("Connecting to the camera at hostname " + identifiers.Hostname);
                // The default hostname format is "zivid-<serial-number>.local".
                // The hostname cannot be read or set through the SDK.
                var camera = zivid.ConnectCamera(new Zivid.NET.CameraAddress(identifiers.Hostname));
                camera.Disconnect();
            }

            Console.WriteLine("Connecting to all available cameras");
            var connectedCameras = new List<Zivid.NET.Camera>();
            foreach (var camera in zivid.Cameras)
            {
                if (camera.State.Status == Zivid.NET.CameraState.StatusOption.Available)
                {
                    Console.WriteLine("Connecting to camera: " + camera.Info.SerialNumber);
                    camera.Connect();
                    connectedCameras.Add(camera);
                }
                else
                {
                    Console.WriteLine("Camera " + camera.Info.SerialNumber + " is not available. "
                                      + "Camera status: " + camera.State.Status);
                }
            }
            foreach (var camera in connectedCameras)
            {
                camera.Disconnect();
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
        return new ArgumentException("Usage: [--serial <serial number>] [--ip <IP address>] [--hostname <hostname>]");
    }

    static CameraIdentifiers ParseOptions(string[] args)
    {
        var identifiers = new CameraIdentifiers();
        if (args.Length % 2 != 0)
        {
            throw UsageException();
        }
        for (int i = 0; i < args.Length; i += 2)
        {
            switch (args[i])
            {
                case "--serial":
                    identifiers.SerialNumber = args[i + 1];
                    break;
                case "--ip":
                    identifiers.IPAddress = args[i + 1];
                    break;
                case "--hostname":
                    identifiers.Hostname = args[i + 1];
                    break;
                default:
                    throw UsageException();
            }
        }
        return identifiers;
    }
}
