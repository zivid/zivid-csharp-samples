/*
This example shows how to read point cloud data from a ZDF file, iterate through it, and extract individual points.
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
            Console.WriteLine("Reading ZDF frame from file: " + dataFile);
            var frame = new Zivid.NET.Frame(dataFile);

            Console.WriteLine("Getting point cloud from frame");
            var pointCloud = frame.PointCloud;
            var pointCloudData = pointCloud.CopyPointsXYZColorsRGBA();
            var pointCloudSNR = pointCloud.CopySNRs();

            var height = pointCloud.Height;
            var width = pointCloud.Width;
            Console.WriteLine("Point cloud information:");
            Console.WriteLine("Number of points: " + pointCloud.Size + "\n" + "Height: " + height
                              + ", Width: " + width);

            // Iterating over the point cloud and displaying X, Y, Z, R, G, B, and SNR for central 10 x 10 pixels
            const ulong pixelsToDisplay = 10;
            Console.WriteLine(
                "Iterating over point cloud and extracting X, Y, Z, R, G, B, and SNR for central {0} x {1} pixels",
                pixelsToDisplay,
                pixelsToDisplay);
            ulong iStart = (height - pixelsToDisplay) / 2;
            ulong iStop = (height + pixelsToDisplay) / 2;
            ulong jStart = (width - pixelsToDisplay) / 2;
            ulong jStop = (width + pixelsToDisplay) / 2;
            for (ulong i = iStart; i < iStop; i++)
            {
                for (ulong j = jStart; j < jStop; j++)
                {
                    Console.WriteLine(string.Format(
                        "{0} {1} {2,-7} {3} {4,-7} {5} {6,-7} {7} {8,-7} {9} {10,-7} {11} {12,-7} {13} {14,-7}",
                        "Values at pixel (" + i + "," + j + "):  ",
                        "X:",
                        pointCloudData[i, j].point.x.ToString("F1"),
                        "Y:",
                        pointCloudData[i, j].point.y.ToString("F1"),
                        "Z:",
                        pointCloudData[i, j].point.z.ToString("F1"),
                        "R:",
                        pointCloudData[i, j].color.r.ToString(),
                        "G:",
                        pointCloudData[i, j].color.g.ToString(),
                        "B:",
                        pointCloudData[i, j].color.b.ToString(),
                        "SNR:",
                        pointCloudSNR[i, j]
                            .ToString("F1")));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }
}