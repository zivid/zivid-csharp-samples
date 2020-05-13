/*
This example shows how to convert point cloud from ZDF file to PLY format.
*/

using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var dataFile =
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/Zivid3D.zdf";
            var pointCloudFile = "Zivid/Zivid3D.ply";

            var frame = new Zivid.NET.Frame(dataFile);

            Console.WriteLine("Saving point cloud to file: " + pointCloudFile);
            frame.Save(pointCloudFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}