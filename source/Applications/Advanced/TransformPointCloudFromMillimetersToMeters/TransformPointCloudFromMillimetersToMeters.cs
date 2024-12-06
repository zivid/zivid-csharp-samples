/*
Transform point cloud data from millimeters to meters.

The ZDF file for this sample can be found under the main instructions for Zivid samples.
*/

using System;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            var dataFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                           + "/Zivid/CalibrationBoardInCameraOrigin.zdf";
            Console.WriteLine("Reading " + dataFile + " point cloud");
            var frame = new Zivid.NET.Frame(dataFile);
            var pointCloud = frame.PointCloud;

            var millimetersToMetersTransform =
                new float[,] { { 0.001F, 0, 0, 0 }, { 0, 0.001F, 0, 0 }, { 0, 0, 0.001F, 0 }, { 0, 0, 0, 1 } };

            Console.WriteLine("Transforming point cloud from mm to m");
            pointCloud.Transform(millimetersToMetersTransform);

            var transformedFile = "FrameInMeters.zdf";
            Console.WriteLine("Saving frame to file: " + transformedFile);
            frame.Save(transformedFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
