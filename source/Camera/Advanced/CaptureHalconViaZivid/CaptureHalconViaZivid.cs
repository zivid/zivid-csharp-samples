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
            var frame = camera.Capture(settings);
            var pointCloud = frame.PointCloud;

            Console.WriteLine("Converting to Halcon point cloud");
            HalconDotNet.HObjectModel3D objectModel3D = ZividToHalconPointCloud(pointCloud);

            string pointCloudFile = "Zivid3D.ply";
            Console.WriteLine("Saving point cloud to: " + pointCloudFile);
            SaveHalconPointCloud(objectModel3D, pointCloudFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    private static HalconDotNet.HTuple arrayToHalconDouble(params double[] arr)
    {
        var htup = new HalconDotNet.HTuple((double)0);
        htup.DArr = arr;
        return htup;
    }
    private static HalconDotNet.HTuple arrayToHalconInt(params int[] arr)
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
        var pointsXYZ = pointCloud.CopyPointsXYZ();
        var normalsXYZ = pointCloud.CopyNormalsXYZ();
        var colorsRGBA = pointCloud.CopyColorsRGBA();

        var numberOfValidPoints = FindNumberOfValidPoints(pointsXYZ, height, width);
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

        var tuplePointsXH = arrayToHalconDouble(tuplePointsX);
        var tuplePointsYH = arrayToHalconDouble(tuplePointsY);
        var tuplePointsZH = arrayToHalconDouble(tuplePointsZ);
        var tupleNormalsXH = arrayToHalconDouble(tupleNormalsX);
        var tupleNormalsYH = arrayToHalconDouble(tupleNormalsY);
        var tupleNormalsZH = arrayToHalconDouble(tupleNormalsZ);
        var tupleColorsRH = arrayToHalconInt(tupleColorsR);
        var tupleColorsGH = arrayToHalconInt(tupleColorsG);
        var tupleColorsBH = arrayToHalconInt(tupleColorsB);

        Console.WriteLine("Constructing ObjectModel3D based on XYZ data");
        var objectModel3D = new HalconDotNet.HObjectModel3D(tuplePointsXH, tuplePointsYH, tuplePointsZH);

        Console.WriteLine("Mapping ObjectModel3D data");
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "xyz_mapping", "object", arrayToHalconInt(tupleXYZMapping));

        Console.WriteLine("Adding normals to ObjectModel3D");
        var normalsAttribNames = new HalconDotNet.HTuple("point_normal_x", "point_normal_y", "point_normal_z");
        var normalsAttribValues = new HalconDotNet.HTuple(tupleNormalsXH, tupleNormalsYH, tupleNormalsZH);
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, normalsAttribNames, "points", normalsAttribValues);

        Console.WriteLine("Adding RGB to ObjectModel3D");
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "red", "points", tupleColorsRH);
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "green", "points", tupleColorsGH);
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "blue", "points", tupleColorsBH);

        return objectModel3D;
    }
}
