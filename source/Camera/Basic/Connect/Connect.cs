/*
Connect to a Zivid camera.

Without arguments, the sample connects to the first available camera. To connect to a specific camera, provide its
serial number with --serial, its IP address with --ip, or its hostname with --hostname. To connect to all available
cameras, use --all. The sample connects with every method you provide, and fails if any of them fails.

Examples:
    Connect --serial 2020C0DE
    Connect --ip 172.28.60.5
    Connect --hostname zivid-2020C0DE.local
    Connect --serial 2020C0DE --ip 172.28.60.5
    Connect --all
*/

using System;
using System.Collections.Generic;

class Program
{
    class ConnectOptions
    {
        public string SerialNumber;
        public string IPAddress;
        public string Hostname;
        public bool AllCameras;
    }

    static void PrintDiscoveredCameras(Zivid.NET.Application zivid)
    {
        Console.WriteLine("Discovered cameras:");
        foreach (var camera in zivid.Cameras)
        {
            Console.WriteLine("Serial number: " + camera.Info.SerialNumber
                              + ", IP address: " + camera.State.Network.IPV4.Address
                              + ", hostname: " + camera.State.Network.Hostname);
        }
    }

    static void ConnectToFirstAvailableCamera(Zivid.NET.Application zivid)
    {
        Console.WriteLine("Connecting to the first available camera");
        var camera = zivid.ConnectCamera();
        camera.Disconnect();
    }

    static void ConnectBySerialNumber(Zivid.NET.Application zivid, string serialNumber)
    {
        Console.WriteLine("Connecting to the camera with serial number " + serialNumber);
        var camera = zivid.ConnectCamera(serialNumber);
        camera.Disconnect();
    }

    static void ConnectByIPAddress(Zivid.NET.Application zivid, string ipAddress)
    {
        Console.WriteLine("Connecting to the camera at IP address " + ipAddress);
        var camera = zivid.ConnectCamera(new Zivid.NET.CameraAddress(ipAddress));
        camera.Disconnect();
    }

    static void ConnectByHostname(Zivid.NET.Application zivid, string hostname)
    {
        Console.WriteLine("Connecting to the camera at hostname " + hostname);
        // The default hostname format is "zivid-<serial-number>"; connect to it with the ".local" suffix.
        // The hostname is set through the network configuration, see the NetworkConfiguration sample.
        var camera = zivid.ConnectCamera(new Zivid.NET.CameraAddress(hostname));
        camera.Disconnect();
    }

    static void ConnectToAllAvailableCameras(Zivid.NET.Application zivid)
    {
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

    static int Main(string[] args)
    {
        try
        {
            var options = ParseOptions(args);

            var zivid = new Zivid.NET.Application();

            PrintDiscoveredCameras(zivid);

            var noMethodGiven = options.SerialNumber == null && options.IPAddress == null && options.Hostname == null
                                && !options.AllCameras;
            if (noMethodGiven)
            {
                ConnectToFirstAvailableCamera(zivid);
            }
            if (options.SerialNumber != null)
            {
                ConnectBySerialNumber(zivid, options.SerialNumber);
            }
            if (options.IPAddress != null)
            {
                ConnectByIPAddress(zivid, options.IPAddress);
            }
            if (options.Hostname != null)
            {
                ConnectByHostname(zivid, options.Hostname);
            }
            if (options.AllCameras)
            {
                ConnectToAllAvailableCameras(zivid);
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
        return new ArgumentException(
            "Usage: [--serial <serial number>] [--ip <IP address>] [--hostname <hostname>] [--all]");
    }

    static string ValueAfter(string[] args, int index)
    {
        if (index + 1 >= args.Length)
        {
            throw UsageException();
        }
        return args[index + 1];
    }

    static ConnectOptions ParseOptions(string[] args)
    {
        var options = new ConnectOptions();
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--serial":
                    options.SerialNumber = ValueAfter(args, i++);
                    break;
                case "--ip":
                    options.IPAddress = ValueAfter(args, i++);
                    break;
                case "--hostname":
                    options.Hostname = ValueAfter(args, i++);
                    break;
                case "--all":
                    options.AllCameras = true;
                    break;
                default:
                    throw UsageException();
            }
        }
        return options;
    }
}
