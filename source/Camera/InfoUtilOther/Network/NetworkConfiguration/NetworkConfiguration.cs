/*
Uses Zivid API to change the IP address and hostname of the Zivid camera.

The new configuration keeps the camera's current hostname unless you enter a new one. A configuration without a
hostname makes the camera use its default hostname, zivid-<serial number>. To connect to the camera by its hostname,
add the .local suffix, for example zivid-2020C0DE.local.

For how to configure a static IP address, check out this tutorial:
https://support.zivid.com/en/latest/camera/getting-started/software-installation/network-configuration/static-ip-network-configuration.html

For how to configure DHCP, check out this tutorial:
https://support.zivid.com/en/latest/camera/getting-started/software-installation/network-configuration/dhcp-network-configuration.html
*/

using System;
using System.Net.NetworkInformation;

class NetworkConfiguration
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            if (zivid.Cameras.Count == 0)
            {
                throw new System.Exception("Failed to connect to camera. No cameras found.");
            }

            var camera = zivid.Cameras[0];

            var originalConfig = camera.NetworkConfiguration;

            Console.WriteLine("Current network configuration of camera {0}:", camera.Info.SerialNumber);
            Console.WriteLine(originalConfig.ToString());
            Console.WriteLine("Current hostname: {0}", camera.State.Network.Hostname);
            Console.WriteLine();

            var mode = Zivid.NET.NetworkConfiguration.IPV4Group.ModeOption.Manual;
            var address = originalConfig.IPV4.Address;
            var subnetMask = originalConfig.IPV4.SubnetMask;

            if (Confirm("Do you want to use DHCP?"))
            {
                mode = Zivid.NET.NetworkConfiguration.IPV4Group.ModeOption.Dhcp;
            }
            else
            {
                Console.Write("Enter IPv4 Address [{0}]: ", originalConfig.IPV4.Address);
                var inputAddress = Console.ReadLine();
                if (!string.IsNullOrEmpty(inputAddress))
                {
                    address = inputAddress;
                }
                Console.Write("Enter new Subnet mask [{0}]: ", originalConfig.IPV4.SubnetMask);
                var inputSubnetMask = Console.ReadLine();
                if (!string.IsNullOrEmpty(inputSubnetMask))
                {
                    subnetMask = inputSubnetMask;
                }
            }

            var hostname = originalConfig.Hostname;
            Console.Write(
                "Enter new hostname, or leave empty to keep the current one [{0}]: ",
                camera.State.Network.Hostname);
            var inputHostname = Console.ReadLine();
            if (!string.IsNullOrEmpty(inputHostname))
            {
                hostname = inputHostname;
            }

            var newConfig = new Zivid.NET.NetworkConfiguration()
            {
                Hostname = hostname,
                IPV4 = {
                    Mode = mode,
                    Address = address,
                    SubnetMask = subnetMask,
                }
            };

            Console.WriteLine();
            Console.WriteLine("New network configuration:");
            Console.WriteLine(newConfig.ToString());
            if (!Confirm(string.Format("Do you want to apply the new network configuration to camera {0}?", camera.Info.SerialNumber)))
            {
                return 0;
            }

            Console.WriteLine("Applying network configuration...");
            camera.ApplyNetworkConfiguration(newConfig);

            Console.WriteLine("Updated network configuration of camera {0}:", camera.Info.SerialNumber);
            Console.WriteLine(camera.NetworkConfiguration.ToString());
            Console.WriteLine("Hostname: {0}", camera.State.Network.Hostname);
            Console.WriteLine();

            Console.WriteLine("Camera status is '{0}'", camera.State.Status);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static bool Confirm(string message)
    {
        while (true)
        {
            Console.Write(message + " [Y/n] ");
            string input = Console.ReadLine();
            if (input.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (input.Equals("n", StringComparison.OrdinalIgnoreCase) ||
                     input.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 'Y' or 'n'.");
            }
        }
    }
}

