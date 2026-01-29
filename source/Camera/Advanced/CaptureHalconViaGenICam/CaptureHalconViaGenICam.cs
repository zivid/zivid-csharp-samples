/*
Capture and save a point cloud, with colors, using GenICam interface and Halcon C++ SDK.
*/

using System;
using System.IO;
using HalconDotNet;
using Zivid.NET;

class Program
{
    static int Main()
    {
        try
        {
            Console.WriteLine("Connecting to camera");
            var zividDevice = GetFirstAvailableZividDevice();
            var framegrabber = new HalconDotNet.HTuple();
            HalconDotNet.HOperatorSet.OpenFramegrabber("GenICamTL",
                                                       1,
                                                       1,
                                                       0,
                                                       0,
                                                       0,
                                                       0,
                                                       "progressive",
                                                       -1,
                                                       "default",
                                                       -1,
                                                       "false",
                                                       "default",
                                                       zividDevice,
                                                       0,
                                                       0,
                                                       out framegrabber);

            Console.WriteLine("Configuring 3D-settings");
            HalconDotNet.HOperatorSet.SetFramegrabberParam(framegrabber, "create_objectmodel3d", "enable");
            HalconDotNet.HOperatorSet.SetFramegrabberParam(framegrabber, "add_objectmodel3d_overlay_attrib", "enable");

            HalconDotNet.HTuple modelTuple;
            HalconDotNet.HOperatorSet.GetFramegrabberParam(framegrabber, "CameraInfoModel", out modelTuple);
            var settingFile = PresetPath(modelTuple.ToString());

            HalconDotNet.HOperatorSet.SetFramegrabberParam(framegrabber, "LoadSettingsFromFile", settingFile);

            Console.WriteLine("Capturing frame");
            var frame = new HalconDotNet.HObject();
            var region = new HalconDotNet.HObject();
            var contours = new HalconDotNet.HObject();
            var data = new HalconDotNet.HTuple();
            HalconDotNet.HOperatorSet.GrabData(out frame, out region, out contours, framegrabber, out data);

            var x = frame.SelectObj(1);
            var y = frame.SelectObj(2);
            var z = frame.SelectObj(3);
            var snr = frame.SelectObj(4);
            var rgb = frame.SelectObj(5);

            Console.WriteLine("Removing invalid 3D points (zeroes)");
            var reducedRegion = new HalconDotNet.HObject();
            var zReduced = new HalconDotNet.HObject();
            HalconDotNet.HOperatorSet.Threshold(z, out reducedRegion, 0.0001, 10000);
            HalconDotNet.HOperatorSet.ReduceDomain(z, reducedRegion, out zReduced);

            Console.WriteLine("Constructing ObjetModel3D based on XYZ data");
            var objectModel3D = new HalconDotNet.HTuple();
            HalconDotNet.HOperatorSet.XyzToObjectModel3d(x, y, zReduced, out objectModel3D);

            Console.WriteLine("Adding RGB to ObjectModel3D");
            SetColorsInObjectModel3D(objectModel3D, rgb, zReduced);

            string pointCloudFile = "Zivid3D.ply";
            Console.WriteLine("Saving point cloud to file: " + pointCloudFile);
            SaveHalconPointCloud(objectModel3D, pointCloudFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }

    private static string PresetPath(string model)
    {
        var presetsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                    + "/Zivid/Settings/";

        if (model.Contains("zividTwoL100")) return presetsPath + "Zivid_Two_L100_ManufacturingSpecular.yml";
        if (model.Contains("zividTwo")) return presetsPath + "Zivid_Two_M70_ManufacturingSpecular.yml";
        if (model.Contains("zivid2PlusM130")) return presetsPath + "Zivid_Two_Plus_M130_ConsumerGoodsQuality.yml";
        if (model.Contains("zivid2PlusM60")) return presetsPath + "Zivid_Two_Plus_M60_ConsumerGoodsQuality.yml";
        if (model.Contains("zivid2PlusL110")) return presetsPath + "Zivid_Two_Plus_L110_ConsumerGoodsQuality.yml";
        if (model.Contains("zivid2PlusMR130")) return presetsPath + "Zivid_Two_Plus_MR130_ConsumerGoodsQuality.yml";
        if (model.Contains("zivid2PlusMR60")) return presetsPath + "Zivid_Two_Plus_MR60_ConsumerGoodsQuality.yml";
        if (model.Contains("zivid2PlusLR110")) return presetsPath + "Zivid_Two_Plus_LR110_ConsumerGoodsQuality.yml";
        if (model.Contains("zivid3XL250")) return presetsPath + "Zivid_Three_XL250_DepalletizationQuality.yml";
        if (model.Contains("zividOnePlus")) throw new NotSupportedException($"Unsupported Zivid One+ model: {model}");
        throw new ArgumentException($"Invalid camera model: {model}");
    }

    private static void SaveHalconPointCloud(HalconDotNet.HTuple model, string fileName)
    {
        HalconDotNet.HOperatorSet.WriteObjectModel3d(model, "ply", fileName, "invert_normals", "false");
    }

    private static void SetColorsInObjectModel3D(HalconDotNet.HTuple objectModel3D,
                                                 HalconDotNet.HObject RGB,
                                                 HalconDotNet.HObject zReduced)
    {
        var domain = new HalconDotNet.HObject();
        var rows = new HalconDotNet.HTuple();
        var cols = new HalconDotNet.HTuple();

        HalconDotNet.HOperatorSet.GetDomain(zReduced, out domain);
        HalconDotNet.HOperatorSet.GetRegionPoints(domain, out rows, out cols);

        var objectRed = new HalconDotNet.HObject();
        var objectGreen = new HalconDotNet.HObject();
        var objectBlue = new HalconDotNet.HObject();

        HalconDotNet.HOperatorSet.AccessChannel(RGB, out objectRed, 1);
        HalconDotNet.HOperatorSet.AccessChannel(RGB, out objectGreen, 2);
        HalconDotNet.HOperatorSet.AccessChannel(RGB, out objectBlue, 3);

        var tupleRed = new HalconDotNet.HTuple();
        var tupleGreen = new HalconDotNet.HTuple();
        var tupleBlue = new HalconDotNet.HTuple();

        HalconDotNet.HOperatorSet.GetGrayval(objectRed, rows, cols, out tupleRed);
        HalconDotNet.HOperatorSet.GetGrayval(objectGreen, rows, cols, out tupleGreen);
        HalconDotNet.HOperatorSet.GetGrayval(objectBlue, rows, cols, out tupleBlue);

        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "red", "points", tupleRed);
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "green", "points", tupleGreen);
        HalconDotNet.HOperatorSet.SetObjectModel3dAttribMod(objectModel3D, "blue", "points", tupleBlue);
    }

    private static HalconDotNet.HTuple GetFirstAvailableZividDevice()
    {
        var devices = new HalconDotNet.HTuple();
        var information = new HalconDotNet.HTuple();
        HalconDotNet.HOperatorSet.InfoFramegrabber("GenICamTL", "device", out information, out devices);

        var zividDevices = devices.TupleRegexpSelect("Zivid");
        if (zividDevices.Length == 0)
        {
            throw new System.InvalidOperationException("No Zivid devices found. Please check your setup.");
        }
        return zividDevices[0];
    }
}
