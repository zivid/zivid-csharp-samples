/*
Automatically set the IP addresses of any number of cameras to be in the same subnet as the provided IP address of the network interface.
*/

using System;
using System.Net;
using Zivid.NET;

class Program
{

    static int Main(string[] args)
    {
        try
        {
            var (ipAddress, subnetMask) = ParseOptions(args);
            AssertUserInput(ipAddress, subnetMask);
            var ipAddressOctets = IPAddress.Parse(ipAddress).GetAddressBytes();

            // defines the last octet of the ip address of the first Zivid camera. Eg.: x.x.x.2
            int nextIpAddressLastOctet = 2;
            var newIpAddressOctets = ipAddressOctets;

            var zivid = new Zivid.NET.Application();
            var cameras = zivid.Cameras;

            if (cameras.Count == 0)
            {
                throw new InvalidOperationException("No cameras connected");
            }

            foreach (var camera in cameras)
            {
                if (nextIpAddressLastOctet == ipAddressOctets[3])
                {
                    nextIpAddressLastOctet += 1;
                }

                newIpAddressOctets[3] = (byte)nextIpAddressLastOctet;
                IPAddress newIpAddress = new IPAddress(newIpAddressOctets);

                var newConfig = new Zivid.NET.NetworkConfiguration()
                {
                    IPV4 = {
                        Mode = Zivid.NET.NetworkConfiguration.IPV4Group.ModeOption.Manual,
                        Address = newIpAddress.ToString(),
                        SubnetMask = subnetMask,
                    }
                };

                nextIpAddressLastOctet += 1;

                Console.WriteLine($"Applying network configuration to camera {camera.Info.SerialNumber}");
                camera.ApplyNetworkConfiguration(newConfig);
                Console.WriteLine($"New {camera.NetworkConfiguration}\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static void UsageException(string message)
    {
        throw new ArgumentException(message + "\nUsage: --interface-ipv4 <IP address of the PC network interface> [--subnet-mask <Subnet Mask>]");
    }

    static (string, string) ParseOptions(string[] args)
    {
        string ipAddress = null;
        string subnetMask = "255.255.255.0";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("--interface-ipv4"))
            {
                if (i + 1 < args.Length)
                {
                    ipAddress = args[i + 1];
                    i++;
                    continue;
                }
                UsageException("Expected IP address of the PC network interface after --interface-ipv4");
            }
            if (args[i].Equals("--subnet-mask"))
            {
                if (i + 1 < args.Length)
                {
                    subnetMask = args[i + 1];
                    i++;
                    continue;
                }
                UsageException("Expected subnet mask after --subnet-mask");
            }
            UsageException("Unrecognized argument");
        }
        if (ipAddress == null)
        {
            UsageException("IPV4 address of the network interface is required");
        }
        return (ipAddress, subnetMask);
    }

    static void AssertUserInput(string ipAddress, string subnetMask)
    {
        new Zivid.NET.NetworkConfiguration()
        {
            IPV4 = {
                Address = ipAddress,
                SubnetMask = subnetMask,
            }
        };
    }
}
