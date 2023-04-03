/*
Capture Zivid point clouds, compute normals and print a subset.

For scenes with high dynamic range we combine multiple acquisitions to get an HDR point cloud.
*/

using System;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring settings");
            var settings = new Zivid.NET.Settings();
            foreach(var aperture in new double[] { 9.57, 4.76, 2.59 })
            {
                Console.WriteLine("Adding acquisition with aperture = " + aperture);
                var acquisitionSettings = new Zivid.NET.Settings.Acquisition { Aperture = aperture };
                settings.Acquisitions.Add(acquisitionSettings);
            }

            Console.WriteLine("Capturing frame (HDR)");
            using(var frame = camera.Capture(settings))
            {
                var pointCloud = frame.PointCloud;

                Console.WriteLine("Computing normals and copying them to CPU memory");
                var normals = pointCloud.CopyNormalsXYZ();

                int radiusOfPixelsToPrint = 5;
                Console.Write("Printing normals for the central ");
                Console.WriteLine(radiusOfPixelsToPrint * 2 + " x " + radiusOfPixelsToPrint * 2 + " pixels");
                PrintNormals(radiusOfPixelsToPrint, normals);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }

    static void PrintNormals(int radius, float[,,] normals)
    {
        var lineSeparator = new String('-', 50);
        var numOfRows = normals.GetLength(0);
        var numOfCols = normals.GetLength(1);
        Console.WriteLine(lineSeparator);
        for(int row = (numOfRows / 2 - radius); row < (numOfRows / 2 + radius); row++)
        {
            for(int col = (numOfCols / 2 - radius); col < (numOfCols / 2 + radius); col++)
            {
                Console.Write("Normals (" + row + "," + col + "): [");
                Console.Write("x: {0,10:0.0000} ", normals[row, col, 0]);
                Console.Write("y: {0,10:0.0000} ", normals[row, col, 1]);
                Console.Write("z: {0,10:0.0000} ", normals[row, col, 2]);
                Console.WriteLine("]");
            }
        }
        Console.WriteLine(lineSeparator);
    }
}
