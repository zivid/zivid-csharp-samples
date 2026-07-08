/*
Automatically configure the IP addresses of connected cameras to match the network of the user's PC.

Usage:
- By default, the program applies the new configuration directly to the cameras.
- Use the [--display-only] argument to simulate the configuration and display the
  proposed IP addresses without making actual changes. Note: camera-side Ethernet
  link speed will also be skipped, as it requires connecting to the camera.

For more information on network configuration, check out this tutorial:
https://support.zivid.com/en/latest/camera/getting-started/software-installation/zivid-two-network-configuration.html
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Zivid.NET;

class Program
{
    static int Main(string[] args)
    {
        try
        {
            var displayOnly = ParseOptions(args);

            var zivid = new Zivid.NET.Application();
            var cameras = zivid.Cameras;

            if (cameras.Count == 0)
            {
                throw new InvalidOperationException("No cameras connected");
            }

            Dictionary<string, List<Zivid.NET.Camera>> localInterfaceIpToCameras = new Dictionary<string, List<Camera>>();

            foreach (var camera in cameras)
            {
                try
                {
                    var (localInterfaceIpAddress, localInterfaceSubnetMask) = GetUsersLocalInterfaceNetworkConfiguration(camera);

                    var ipAddressOctets = IPAddress.Parse(localInterfaceIpAddress).GetAddressBytes();

                    int nextIpAddressLastOctet = (int)ipAddressOctets[ipAddressOctets.Length - 1];

                    // Identifying the last octet of the new ip address for the current camera
                    if (!localInterfaceIpToCameras.ContainsKey(localInterfaceIpAddress))
                    {
                        localInterfaceIpToCameras.Add(localInterfaceIpAddress, new List<Zivid.NET.Camera>());
                        nextIpAddressLastOctet += 1;
                    }
                    else
                    {
                        nextIpAddressLastOctet += localInterfaceIpToCameras[localInterfaceIpAddress].Count + 1;
                    }

                    localInterfaceIpToCameras[localInterfaceIpAddress].Add(camera);

                    ipAddressOctets[3] = (byte)nextIpAddressLastOctet;
                    IPAddress newIpAddress = new IPAddress(ipAddressOctets);

                    var newConfig = new Zivid.NET.NetworkConfiguration()
                    {
                        IPV4 = {
                            Mode = Zivid.NET.NetworkConfiguration.IPV4Group.ModeOption.Manual,
                            Address = newIpAddress.ToString(),
                            SubnetMask = localInterfaceSubnetMask,
                        }
                    };

                    if (displayOnly)
                    {
                        Console.WriteLine("Current camera serial number :{0} \n" +
                            "Current camera {2}" +
                            "Current local interface detected: {1} \n" +
                            "Simulated new camera address ip: {3} \n\n",
                            camera.Info.SerialNumber, localInterfaceIpAddress, camera.NetworkConfiguration, newIpAddress.ToString());
                    }
                    else
                    {
                        Console.WriteLine("Applying network configuration to camera with serial number: {0} \n" +
                        "Current local interface detected: {1}",
                        camera.Info.SerialNumber, localInterfaceIpAddress);

                        camera.ApplyNetworkConfiguration(newConfig);
                        Console.WriteLine("New {0}\n", camera.NetworkConfiguration);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error when configuring the camera : " + camera.Info.SerialNumber + " " + ex.ToString());
                    return 1;
                }
            }

            foreach (var camera in cameras)
            {
                try
                {
                    PrintHostSideEthernetLinkSpeed(camera);
                    if (!displayOnly)
                    {
                        PrintCameraSideEthernetLinkSpeed(camera);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error when reading Ethernet link speed for camera : " + camera.Info.SerialNumber + " " + ex.ToString());
                    return 1;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static Zivid.NET.CameraState.NetworkGroup.LocalInterface GetUsersLocalInterface(Zivid.NET.Camera camera)
    {
        var localInterfaces = camera.State.Network.LocalInterfaces;

        if (localInterfaces.Count == 0)
        {
            throw new Exception("No user local interface detected from the camera " + camera.Info.SerialNumber);
        }

        if (localInterfaces.Count > 1)
        {
            throw new Exception("More than one local interface detected from the camera " + camera.Info.SerialNumber + ". Please, reorganize your network.");
        }

        return localInterfaces.ElementAt(0);
    }

    static void PrintHostSideEthernetLinkSpeed(Zivid.NET.Camera camera)
    {
        // Reading the host-side (local interface) Ethernet link speed does not require a connection to the camera.
        var localInterfaceLinkSpeed = GetUsersLocalInterface(camera).Ethernet.LinkSpeed;

        Console.WriteLine("Camera {0}: local interface Ethernet link speed={1}",
            camera.Info.SerialNumber,
            localInterfaceLinkSpeed);
    }

    static void PrintCameraSideEthernetLinkSpeed(Zivid.NET.Camera camera)
    {
        // Reading the camera-side Ethernet link speed requires being connected to the camera.
        camera.Connect();
        try
        {
            var cameraLinkSpeed = camera.State.Network.Ethernet.LinkSpeed;
            Console.WriteLine("Camera {0}: camera Ethernet link speed={1}",
                camera.Info.SerialNumber,
                cameraLinkSpeed);
        }
        finally
        {
            camera.Disconnect();
        }
    }

    static (string, string) GetUsersLocalInterfaceNetworkConfiguration(Zivid.NET.Camera camera)
    {
        var localInterface = GetUsersLocalInterface(camera);

        if (localInterface.IPV4.Subnets.Count == 0)
        {
            throw new Exception("No valid subnets found for camera " + camera.Info.SerialNumber);
        }

        if (localInterface.IPV4.Subnets.Count > 1)
        {
            throw new Exception("More than one ip address found for the local interface from the camera " + camera.Info.SerialNumber);
        }

        var subnet = localInterface.IPV4.Subnets.First();
        return (subnet.Address, subnet.Mask);
    }

    static bool ParseOptions(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            return false;
        }

        if (args.Length > 1)
        {
            throw new ArgumentException("Expected maximum one argument \nUsage: --display-only Only display the new " +
                "network configurations of the camera(s) without applying changes");
        }

        if (args[0].Equals("--display-only"))
        {
            return true;
        }
        else
        {
            throw new ArgumentException($"Unknown argument: {args[0]}\nExpected: --display-only Only display the new " +
                "network configurations of the camera(s) without applying changes");
        }
    }
}
