/*
Convert point cloud from a ZDF file to a PLY file.

For more information on supported formats and options, check out this article:
https://support.zivid.com/en/latest/camera/reference-articles/point-cloud-structure-and-output-formats.html
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

            using (var frame = new Zivid.NET.Frame(dataFile))
            {

                Console.WriteLine("Saving point cloud to file: " + pointCloudFile);
                frame.Save(pointCloudFile);
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
