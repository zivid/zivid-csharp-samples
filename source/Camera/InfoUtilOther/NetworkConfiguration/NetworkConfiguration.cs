/*
Uses Zivid API to change the IP address of the Zivid camera.
*/

using System;
using System.Net.NetworkInformation;

class NetworkConfiguration
{
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

            var newConfig = new Zivid.NET.NetworkConfiguration()
            {
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
            Console.WriteLine();

            Console.WriteLine("Camera status is '{0}'", camera.State.Status);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}

