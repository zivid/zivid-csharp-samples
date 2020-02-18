using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    enum Mode { read, write, clear };

    static void Main(string[] args)
    {
        try
        {
            Mode mode = ParseMode(args);
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();
            CheckUserDataSupport(camera);

            switch (mode)
            {
                case Mode.read:
                    Console.WriteLine("Reading user data from camera");
                    Console.WriteLine("Done. User data: '" + Read(camera) + "'");
                    break;
                case Mode.write:
                    var userData = ParseWriteData(args);
                    Console.WriteLine("Writing '" + userData + "' to the camera");
                    Write(camera, userData);
                    Console.WriteLine("Done");
                    break;
                case Mode.clear:
                    Console.WriteLine("Clearing user data from camera");
                    Clear(camera);
                    Console.WriteLine("Done");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }

    static ArgumentException UsageException()
    {
        return new ArgumentException("Usage: "
            + System.Reflection.Assembly.GetEntryAssembly().Location
            + " <read|write <string>|clear>");
    }

    static Mode ParseMode(string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0].Equals("read"))
            {
                return Mode.read;
            }
            else if (args[0].Equals("write"))
            {
                return Mode.write;
            }
            else if (args[0].Equals("clear"))
            {
                return Mode.clear;
            }
        }
        throw UsageException();
    }

    static void CheckUserDataSupport(Zivid.NET.Camera camera)
    {
        if (camera.UserDataMaxSizeBytes == 0)
        {
            throw new System.InvalidOperationException("This camera does not support user data");
        }
    }

    static string Read(Zivid.NET.Camera camera)
    {
        return System.Text.Encoding.ASCII.GetString(camera.UserData.ToArray());
    }

    static void Write(Zivid.NET.Camera camera, string dataString)
    {
        camera.WriteUserData(System.Text.Encoding.ASCII.GetBytes(dataString));
    }

    static void Clear(Zivid.NET.Camera camera)
    {
        Write(camera, "");
    }

    static string ParseWriteData(string[] args)
    {
        if (args.Length > 1)
        {
            return args[1];
        }
        throw UsageException();
    }
}
