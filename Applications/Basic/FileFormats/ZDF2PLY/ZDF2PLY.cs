/*
Convert point cloud from a ZDF file to a PLY file.
*/

using System;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var dataFile =
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/Zivid3D.zdf";
            var pointCloudFile = "Zivid3D.ply";

            var frame = new Zivid.NET.Frame(dataFile);

            Console.WriteLine("Saving point cloud to file: " + pointCloudFile);
            frame.Save(pointCloudFile);
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }
}
