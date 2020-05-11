/*
This example shows how to convert a Zivid point cloud from a .ZDF file format
to a .PLY file format.
*/

using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var FilenameZDF = Zivid.NET.Environment.DataPath + "/Zivid3D.zdf";
            var FilenamePLY = "Zivid3D.ply";

            var frame = new Zivid.NET.Frame(FilenameZDF);

            Console.WriteLine("Saving the frame to " + FilenamePLY);
            frame.Save(FilenamePLY);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}