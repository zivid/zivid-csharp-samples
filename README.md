# C#-samples

This repository contains **C#** code samples for **Zivid**.

The Windows Zivid installer adds some basic samples in C:\Users\Public\Documents\Zivid\samples and they should build out of the box using Visual Studio 2015 or 2017. Check out our [tutorial on running these **C#** samples](https://zivid.atlassian.net/wiki/spaces/ZividKB/pages/427340/C+Samples+with+Visual+Studio).

## Samples list

There are two main categories of samples: **Camera** and **Applications**. The samples in the **Camera** category focus only on how to use the camera. The samples in the **Applications** category use the output generated by the camera, such as the 3D point cloud, a 2D image or other data from the camera. These samples shows how the data from the camera can be used.

- **Camera**
  - **Basic** ([quick tutorial][QuickCaptureTutorial-url] / [complete tutorial][CompleteCaptureTutorial-url])
    - [**Capture**][Capture-url] - This example shows how to acquire images from the Zivid camera.
    - [**Capture2D**][Capture2D-url] - This example shows how to acquire only 2D images from the Zivid camera.
    - [**CaptureAssistant**][CaptureAssistant-url] - This example shows how to use Capture Assistant to acquire HDR images from the Zivid camera.
    - [**CaptureFromFile**][CaptureFromFile-url] - This example shows how to acquire HDR images from file. This example can be used without access to a physical camera.
    - [**CaptureHDR**][CaptureHDR-url] - This example shows how to acquire HDR images from the Zivid camera.
    - [**CaptureHDRLoop**][CaptureHDRLoop-url] - This example shows how to acquire HDR images from the Zivid camera in a loop, with settings from .yml files.
    - [**CaptureHDRCompleteSettings**][CaptureHDRCompleteSettings-url] - This example shows how to acquire an HDR image from the Zivid camera (with fully configured settings for each frame).
  - **InfoUtilOther**
    - [**CameraUserData**][CameraUserData-url] - This example shows how to store user data on the Zivid camera.
    - [**GetCameraIntrinsics**][GetCameraIntrinsics-url] - This example shows how to get camera intrinsics from the Zivid camera.

- **Applications**
  - **Basic**
    - **Visualization**
      - [**CaptureFromFileVis3D**][CaptureFromFileVis3D-url] - This example shows how capture a Zivid point cloud from file, and visualize it.
      - [**CaptureVis3D**][CaptureVis3D-url] - This example shows how to capture a Zivid point cloud, and visualize it.
      - [**CaptureLiveVis3D**][CaptureLiveVis3D-url] - This example shows how to continuosly capture a Zivid point cloud, and visualize it.
    - **FileFormats**
      - [**ReadZDF**][ReadZDF-url] - This example shows how to import and display a Zivid point cloud from a .ZDF file.
      - [**ZDF2PLY**][ZDF2PLY-url] - This example shows how to convert a Zivid point cloud from a .ZDF file format to a .PLY file format.

## Instructions

1. [**Install Zivid Software**](https://zivid.atlassian.net/wiki/spaces/ZividKB/pages/59080712/Zivid+Software+Installation).
Note: The version tested with Zivid cameras is 1.8.0.

2. Launch Visual Studio 2017.

3. Open and run one of the samples.

## Support
If you need assistance with using Zivid cameras, visit our [**Knowledge Base**](https://help.zivid.com/) or contact us at [customersuccess@zivid.com](mailto:customersuccess@zivid.com).

## Licence
Zivid Samples are distributed under the [BSD license](LICENSE).

[QuickCaptureTutorial-url]: source/Camera/Basic/QuickCaptureTutorial.md
[CompleteCaptureTutorial-url]: source/Camera/Basic/CaptureTutorial.md
[Capture-url]: source/Camera/Basic/Capture/Capture.cs
[Capture2D-url]: source/Camera/Basic/Capture2D/Capture2D.cs
[CaptureAssistant-url]: source/Camera/Basic/CaptureAssistant/CaptureAssistant.cs
[CaptureFromFile-url]: source/Camera/Basic/CaptureFromFile/CaptureFromFile.cs
[CaptureHDR-url]: source/Camera/Basic/CaptureHDR/CaptureHDR.cs
[CaptureHDRCompleteSettings-url]: source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs
[CaptureHDRLoop-url]: source/Camera/Basic/CaptureHDRLoop/CaptureHDRLoop.cs
[CameraUserData-url]: source/Camera/InfoUtilOther/CameraUserData/CameraUserData.cs
[GetCameraIntrinsics-url]: source/Camera/InfoUtilOther/GetCameraIntrinsics/GetCameraIntrinsics.cs
[CaptureFromFileVis3D-url]: https://github.com/zivid/csharp-extra-samples/blob/master/source/Applications/Basic/Visualization/CaptureFromFileVis3D/CaptureFromFileVis3D.cs
[CaptureVis3D-url]: https://github.com/zivid/csharp-extra-samples/blob/master/source/Applications/Basic/Visualization/CaptureVis3D/CaptureVis3D.cs
[CaptureLiveVis3D-url]: https://github.com/zivid/csharp-extra-samples/blob/master/source/Applications/Basic/Visualization/CaptureLiveVis3D/CaptureLiveVis3D.cs
[ReadZDF-url]: source/Applications/Basic/FileFormats/ReadZDF/ReadZDF.cs
[ZDF2PLY-url]: source/Applications/Basic/FileFormats/ZDF2PLY/ZDF2PLY.cs
