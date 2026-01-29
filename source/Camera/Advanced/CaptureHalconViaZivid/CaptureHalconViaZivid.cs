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
            var settingsPath = PresetPath(camera);
            var settings = new Zivid.NET.Settings(settingsPath);

            Console.WriteLine("Capturing frame");
            using (var frame = camera.Capture2D3D(settings))
            {
                Console.WriteLine("Converting to Halcon point cloud");
                HalconDotNet.HObjectModel3D objectModel3D = ZividToHalconPointCloud(frame);

                string pointCloudFile = "Zivid3D.ply";
                Console.WriteLine("Saving point cloud to: " + pointCloudFile);
                SaveHalconPointCloud(objectModel3D, pointCloudFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    static string PresetPath(Zivid.NET.Camera camera)
    {
        var presetsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            + "/Zivid/Settings/";

        switch (camera.Info.Model)
        {
            case Zivid.NET.CameraInfo.ModelOption.ZividTwo:
                {
                    return presetsPath + "Zivid_Two_M70_ManufacturingSpecular.yml";
                }
            case Zivid.NET.CameraInfo.ModelOption.ZividTwoL100:
                {
                    return presetsPath + "Zivid_Two_L100_ManufacturingSpecular.yml";
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM130:
                {
                    return presetsPath + "Zivid_Two_Plus_M130_ConsumerGoodsQuality.yml";
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusM60:
                {
                    return presetsPath + "Zivid_Two_Plus_M60_ConsumerGoodsQuality.yml";
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusL110:
                {
                    return presetsPath + "Zivid_Two_Plus_L110_ConsumerGoodsQuality.yml";
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR130:
                {
                    return presetsPath + "Zivid_Two_Plus_MR130_ConsumerGoodsQuality.yml";
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusMR60:
                {
                    return presetsPath + "Zivid_Two_Plus_MR60_ConsumerGoodsQuality.yml";
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid2PlusLR110:
                {
                    return presetsPath + "Zivid_Two_Plus_LR110_ConsumerGoodsQuality.yml";
                }
            case Zivid.NET.CameraInfo.ModelOption.Zivid3XL250:
                {
                    return presetsPath + "Zivid_Three_XL250_DepalletizationQuality.yml";
                }
            default: throw new System.InvalidOperationException("Unhandled camera model: " + camera.Info.Model.ToString());
        }
        throw new System.InvalidOperationException("Invalid camera model");
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

    private static HalconDotNet.HObjectModel3D ZividToHalconPointCloud(Zivid.NET.Frame frame)
    {
        var pointCloud = frame.PointCloud;
        var height = pointCloud.Height;
        var width = pointCloud.Width;

        var pointsXYZ = pointCloud.CopyPointsXYZ();
        var normalsXYZ = pointCloud.CopyNormalsXYZ();
        var colorsRGBA = frame.Frame2D.ImageRGBA_SRGB().ToByteArray();

        if ((ulong)pointsXYZ.GetLength(0) != height || (ulong)pointsXYZ.GetLength(1) != width)
        {
            throw new System.InvalidOperationException("Color image size does not match point cloud size");
        }

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
