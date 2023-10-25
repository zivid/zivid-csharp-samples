/*
Capture a point cloud, with colors, using Zivid SDK, transform it to a Halcon point cloud and save it using Halcon C++ SDK.
*/

using System;
using Duration = Zivid.NET.Duration;

class Program
{
    static int Main()
    {
        try
        {
            DateTime t01 = DateTime.Now;
            HalconDotNet.HSystem.GetSystem("is_license_valid");
            DateTime t02 = DateTime.Now; PrintTime(t01, t02, "Check license");

            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            Console.WriteLine("Configuring settings");
            var settings = new Zivid.NET.Settings
            {
                Acquisitions = { new Zivid.NET.Settings.Acquisition { Aperture = 5.66,
                                                                      ExposureTime =
                                                                          Duration.FromMicroseconds(10000) } },
                Processing = { Filters = { Outlier = { Removal = { Enabled = true, Threshold = 5.0 } } } }
            };

            Console.WriteLine("Capturing frame");
            for (int i = 0; i < 25; i++)
            {
                var frame = camera.Capture(settings);
                var pointCloud = frame.PointCloud;

                //Console.WriteLine("Converting to Halcon point cloud");
                HalconDotNet.HObjectModel3D objectModel3D = ZividToHalconPointCloud(pointCloud);
            
                string pointCloudFile = "Zivid3D.ply";
                //Console.WriteLine("Saving point cloud to: " + pointCloudFile);
                SaveHalconPointCloud(objectModel3D, pointCloudFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return 1;
        }
        return 0;
    }

    private static void PrintTime(DateTime beg, DateTime end, string label)
    {
        Console.WriteLine("[time]: {0} took {1} ms", label, Math.Round((end - beg).TotalMilliseconds));
    }

    private static HalconDotNet.HTuple arrayToHalconD(params double[] arr)
    {
        var htup = new HalconDotNet.HTuple((double)0);
        htup.DArr = arr;
        return htup;
    }
    private static HalconDotNet.HTuple arrayToHalconI(params int[] arr)
    {
        var htup = new HalconDotNet.HTuple((int)0);
        htup.IArr = arr;
        return htup;
    }

    private static void SaveHalconPointCloud(HalconDotNet.HObjectModel3D model, string fileName)
    {
        model.WriteObjectModel3d("ply", fileName, "invert_normals", "false");
    }

    private static int FindNumberOfValidPoints(float[,,] pointCloud, ulong height, ulong width)
    {
        var numberOfValidPoints = 0;
        for (ulong i = 0; i < height; i++)
        {
            for (ulong j = 0; j < width; j++)
            {
                float x = pointCloud[i, j, 0];
                if (!float.IsNaN(x))
                {
                    numberOfValidPoints = numberOfValidPoints + 1;
                }
            }
        }
        return numberOfValidPoints;
    }

    private static HalconDotNet.HObjectModel3D ZividToHalconPointCloud(Zivid.NET.PointCloud pointCloud)
    {
        var height = pointCloud.Height;
        var width = pointCloud.Width;
        //Console.WriteLine("Image size: {0}x{1}", width, height);
        //Console.WriteLine("We have {0} number of points", width * height);

        DateTime t01 = DateTime.Now;
        _ = new HalconDotNet.HObjectModel3D(); // exclude HALCON license check from time measurements
        DateTime t02 = DateTime.Now; //PrintTime(t01, t02, "First time halcon");

        DateTime t1 = DateTime.Now;
        var pointsXYZ = pointCloud.CopyPointsXYZ();
        DateTime t2 = DateTime.Now; //PrintTime(t1, t2, "CopyPointsXYZ");
        var normalsXYZ = pointCloud.CopyNormalsXYZ();
        DateTime t3 = DateTime.Now; //PrintTime(t2, t3, "CopyNormalsXYZ");
        var colorsRGBA = pointCloud.CopyColorsRGBA();
        DateTime t4 = DateTime.Now; //PrintTime(t3, t4, "CopyColorsRGBA");

        var numberOfValidPoints = FindNumberOfValidPoints(pointsXYZ, height, width);
        DateTime t5 = DateTime.Now; //PrintTime(t4, t5, "FindNumberOfValidPoints");
        // Initializing HTuples which are later filled with data from the Zivid point cloud.
        // tupleXYZMapping is of shape [width, height, rows[], cols[]], and is used for creating xyz mapping.
        // See more at: https://www.mvtec.com/doc/halcon/13/en/set_object_model_3d_attrib.html

        var tuplePointsX = new double[numberOfValidPoints];
        var tuplePointsY = new double[numberOfValidPoints];
        var tuplePointsZ = new double[numberOfValidPoints];

        var tupleNormalsX = new double[numberOfValidPoints];
        var tupleNormalsY = new double[numberOfValidPoints];
        var tupleNormalsZ = new double[numberOfValidPoints];

        var tupleColorsR = new int[numberOfValidPoints];
        var tupleColorsG = new int[numberOfValidPoints];
        var tupleColorsB = new int[numberOfValidPoints];

        var tupleXYZMapping = new int[2 * numberOfValidPoints + 2];

        tupleXYZMapping[0] = (int)width;
        tupleXYZMapping[1] = (int)height;

        DateTime t6 = DateTime.Now; //PrintTime(t5, t6, "Allocate memory for HALCON objects");

        var validPointIndex = 0;
        for (uint i = 0; i < height; i++)
        {
            for (uint j = 0; j < width; j++)
            {
                float x = pointsXYZ[i, j, 0];
                float normal = normalsXYZ[i, j, 0];

                if (!float.IsNaN(x))
                {
                    tuplePointsX[validPointIndex] = pointsXYZ[i, j, 0];
                    tuplePointsY[validPointIndex] = pointsXYZ[i, j, 1];
                    tuplePointsZ[validPointIndex] = pointsXYZ[i, j, 2];
                    tupleColorsR[validPointIndex] = colorsRGBA[i, j, 0];
                    tupleColorsG[validPointIndex] = colorsRGBA[i, j, 1];
                    tupleColorsB[validPointIndex] = colorsRGBA[i, j, 2];
                    tupleXYZMapping[2 + validPointIndex] = (int)i;
                    tupleXYZMapping[2 + numberOfValidPoints + validPointIndex] = (int)j;

                    if (!float.IsNaN(normal))
                    {
                        tupleNormalsX[validPointIndex] = normalsXYZ[i, j, 0];
                        tupleNormalsY[validPointIndex] = normalsXYZ[i, j, 1];
                        tupleNormalsZ[validPointIndex] = normalsXYZ[i, j, 2];
                    }
                    validPointIndex++;
                }
            }
        }

        DateTime t7 = DateTime.Now; //PrintTime(t6, t7, "Convert Zivid PC to HALCON format");

        var tuplePointsXH = arrayToHalconD(tuplePointsX);
        var tuplePointsYH = arrayToHalconD(tuplePointsY);
        var tuplePointsZH = arrayToHalconD(tuplePointsZ);
        var tupleNormalsXH = arrayToHalconD(tupleNormalsX);
        var tupleNormalsYH = arrayToHalconD(tupleNormalsY);
        var tupleNormalsZH = arrayToHalconD(tupleNormalsZ);
        var tupleColorsRH = arrayToHalconI(tupleColorsR);
        var tupleColorsGH = arrayToHalconI(tupleColorsG);
        var tupleColorsBH = arrayToHalconI(tupleColorsB);

        DateTime t7a = DateTime.Now; //PrintTime(t7, t7a, "Moving arrays to HTuples");


        //Console.WriteLine("Constructing ObjectModel3D based on XYZ data");
        var objectModel3D = new HalconDotNet.HObjectModel3D(tuplePointsXH, tuplePointsYH, tuplePointsZH);
        DateTime t8 = DateTime.Now; //PrintTime(t7a, t8, "Constructing ObjectModel3D based on XYZ HTuples data");

        //Console.WriteLine("Mapping ObjectModel3D data");
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "xyz_mapping", "object", arrayToHalconI(tupleXYZMapping));
        DateTime t9 = DateTime.Now; //PrintTime(t8, t9, "Mapping ObjectModel3D data");


        //Console.WriteLine("Adding normals to ObjectModel3D");
        var normalsAttribNames = new HalconDotNet.HTuple("point_normal_x", "point_normal_y", "point_normal_z");
        var normalsAttribValues = new HalconDotNet.HTuple(tupleNormalsXH, tupleNormalsYH, tupleNormalsZH);
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, normalsAttribNames, "points", normalsAttribValues);
        DateTime t10 = DateTime.Now; //PrintTime(t9, t10, "Adding normals to ObjectModel3D");

        //Console.WriteLine("Adding RGB to ObjectModel3D");
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "red", "points", tupleColorsRH);
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "green", "points", tupleColorsGH);
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "blue", "points", tupleColorsBH);
        DateTime t11 = DateTime.Now; //PrintTime(t10, t11, "Adding RGB to ObjectModel3D");

        PrintTime(t4, t11, "Total");

        return objectModel3D;
    }
}
