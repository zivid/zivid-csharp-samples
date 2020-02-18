# C#-samples

This repository contains additional **C#** code samples for **Zivid**.

The Windows Zivid installer adds some basic samples in C:\Users\Public\Documents\Zivid\samples and they should build out of the box using Visual Studio 2015 or 2017. Check out our [tutorial on running these **C#** samples](https://zivid.atlassian.net/wiki/spaces/ZividKB/pages/427340/C+Samples+with+Visual+Studio).

## Samples list

- [**CaptureHDRCompleteSettings**](https://github.com/zivid/csharp-extra-samples/blob/master/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs) - This example shows how to acquire an HDR image from the Zivid camera (with fully configured settings for each frame).
- [**CaptureHDRLoop**](https://github.com/zivid/csharp-extra-samples/blob/master/CaptureHDRLoop/CaptureHDRLoop/CaptureHDRLoop.cs) - This example shows how to acquire HDR images from the Zivid camera in a loop (while actively changing some HDR settings).
- [**CaptureSavePLY**](https://github.com/zivid/csharp-extra-samples/blob/master/CaptureSavePLY/CaptureSavePLY/CaptureSavePLY.cs) - This example shows how to capture a Zivid point cloud and save it to a .PLY file format.
- [**ConnectToSerialNumberCamera**](https://github.com/zivid/csharp-extra-samples/blob/master/ConnectToSerialNumberCamera/ConnectToSerialNumberCamera/ConnectToSerialNumberCamera.cs) - This example shows how to connect to a specific Zivid camera based on its serial number.
- [**GetCameraIntrinsics**](https://github.com/zivid/csharp-extra-samples/blob/master/GetCameraIntrinsics/GetCameraIntrinsics/GetCameraIntrinsics.cs) - This example shows how to read the intrinsic calibration parameters of the Zivid camera (OpenCV model).
- [**ReadZDF**](https://github.com/zivid/csharp-extra-samples/blob/master/ReadZDF/ReadZDF/ReadZDF.cs) - This example shows how to import and display a Zivid point cloud from a .ZDF file.
- [**ZDF2PLY**](https://github.com/zivid/csharp-extra-samples/blob/master/ZDF2PLY/ZDF2PLY/ZDF2PLY.cs) - This example shows how to convert a Zivid point cloud from a .ZDF file format to a .PLY file format.

## Instructions

1. [**Install Zivid Software**](https://www.zivid.com/downloads)
Note: The version tested with Zivid cameras is 1.8.0.

2. Launch Visual Studio 2017.

3. Open and run one of the samples.

## Support
If you need assistance with using Zivid cameras, visit our Knowledge Base at [https://help.zivid.com/](https://help.zivid.com/) or contact us at [customersuccess@zivid.com](mailto:customersuccess@zivid.com).

## Licence
Zivid Samples are distributed under the [BSD license](LICENSE).