/*
Connect to a Zivid camera using the different available methods.

Replace the IP address and serial number in the code with the ones of your camera.
*/

using System;
using System.Collections.Generic;

class Program
{
    static void PrintDiscoveredCameras(Zivid.NET.Application zivid)
    {
        Console.WriteLine("Discovered cameras:");
        foreach (var camera in zivid.Cameras)
        {
            Console.WriteLine("Serial number: " + camera.Info.SerialNumber
                              + ", IP address: " + camera.State.Network.IPV4.Address);
        }
    }

    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            PrintDiscoveredCameras(zivid);

            Console.WriteLine("The serial number, IP address and hostname below are placeholders. Replace them with the ones of your camera.");

            {
                Console.WriteLine("Connecting to the first available camera");
                var camera = zivid.ConnectCamera();
                camera.Disconnect();
            }

            {
                Console.WriteLine("Connecting to the camera with a specific serial number");
                var camera = zivid.ConnectCamera("2020C0DE");
                camera.Disconnect();
            }

            {
                Console.WriteLine("Connecting to the camera at a specific IP address");
                var camera = zivid.ConnectCamera(new Zivid.NET.CameraAddress("172.28.60.5"));
                camera.Disconnect();
            }

            {
                Console.WriteLine("Connecting to the camera at a specific hostname");
                // The default hostname format is "zivid-<serial-number>.local".
                // The hostname cannot be read or set through the SDK.
                var camera = zivid.ConnectCamera(new Zivid.NET.CameraAddress("zivid-2020C0DE.local"));
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
}
