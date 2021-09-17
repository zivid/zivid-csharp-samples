# C# samples

This repository contains **C#** code samples for **Zivid**.

![Zivid Image][header-image]

---

*Contents:*
[**Samples**](#Samples-list) |
[**Instructions**](#Instructions) |
[**Support**](#Support) |
[**Licence**](#Licence)

---

## Samples list

There are two main categories of samples: **Camera** and **Applications**. The samples in the **Camera** category focus only on how to use the camera. The samples in the **Applications** category use the output generated by the camera, such as the 3D point cloud, a 2D image, or other data from the camera. These samples show how the data from the camera can be used.

- **Camera**
  - **Basic** ([quick tutorial][QuickCaptureTutorial-url] / [complete tutorial][CompleteCaptureTutorial-url])
    - [**Capture**][Capture-url] - Capture point clouds, with color, from the Zivid camera.
    - [**Capture2D**][Capture2D-url] - Capture 2D images from the Zivid camera.
    - [**CaptureAssistant**][CaptureAssistant-url] - Use Capture Assistant to capture point clouds, with color, from the Zivid camera.
    - [**CaptureFromFile**][CaptureFromFile-url] - Capture point clouds, with color, from the Zivid file camera.
    - [**CaptureWithSettingsFromYML**][CaptureWithSettingsFromYML-url] - Capture point clouds, with color, from the Zivid camera, with settings from YML file.
    - [**CaptureHDR**][CaptureHDR-url] - Capture HDR point clouds, with color, from the Zivid camera.
    - [**CaptureHDRCompleteSettings**][CaptureHDRCompleteSettings-url] - Capture point clouds, with color, from the Zivid camera with fully configured settings.
  - **Advanced**
    - [**CaptureHDRLoop**][CaptureHDRLoop-url] - Cover the same dynamic range in a scene with different acquisition settings to optimize for quality, speed, or to find a compromise.
  - **InfoUtilOther**
    - [**CameraUserData**][CameraUserData-url] - Store user data on the Zivid camera.
    - [**GetCameraIntrinsics**][GetCameraIntrinsics-url] - Read intrinsic parameters from the Zivid camera.	
    - [**Warmup**][Warmup-url] - Warm up the Zivid camera.

- **Applications**
  - **Basic**
    - **Visualization**
      - [**CaptureFromFileVis3D**][CaptureFromFileVis3D-url] - Capture point clouds, with color, from the Zivid file camera, and visualize them.
      - [**CaptureVis3D**][CaptureVis3D-url] - Capture point clouds, with color, from the Zivid camera, and visualize them.
    - **FileFormats**
      - [**ReadIterateZDF**][ReadIterateZDF-url] - Read point cloud data from a ZDF file, iterate through it, and extract individual points.
      - [**ZDF2PLY**][ZDF2PLY-url] - Convert point cloud from ZDF a file to a PLY file.
  - **Advanced**
    - [**HandEyeCalibration**][HandEyeCalibrationReadme-url]
        - [**HandEyeCalibration**][HandEyeCalibration-url] - Perform Hand-Eye calibration.
        - [**UtilizeEyeInHandCalibration**][UtilizeEyeInHandCalibration-url] - Transform single data point or entire point cloud from camera frame to robot base frame using Eye-in-Hand calibration matrix.
          - **Dependencies:** These will be installed automatically through NuGet.
            - [Math.NET Numerics](https://numerics.mathdotnet.com/#Math-NET-Numerics) version 4.12.0 or newer
            - [YamlDotNet](https://github.com/aaubry/YamlDotNet/wiki) version 8.1.2 or newer
    - [**MultiCameraCalibration**][MultiCameraCalibration-url] - Use captures of a calibration object to generate transformation matrices to a single coordinate frame.
    - [**Downsample**][Downsample-url] - Perform downsampling on a zivid point cloud.

## Instructions

1. [**Install Zivid Software**][zivid-software-installation-url].
Note: The samples require Zivid SDK v2 (minor version 2.2 or newer).

2. [**Download Zivid Sample Data**][zivid-sample-data-url].


Launch the Command Prompt by pressing *Win + R* keys on the keyboard, then type cmd and press Enter.

Navigate to a location where you want to clone the repository, then run the following command:

```bash
git clone https://github.com/zivid/zivid-csharp-samples
```

Open *ZividNETSamples.sln* in Visual Studio, build it and run it. If you are uncertain about doing this, check out our [**tutorials for configuring and building C# Samples**](https://support.zivid.com/latest/academy/samples/c-sharp/build-c-sharp-samples-using-visual-studio.html).

Some of the samples depend on external libraries, in particular *YamlDotNet* and *MathNet.Numerics*. These libraries will be installed automatically through NuGet when building the sample.


## Support
If you need assistance with using Zivid cameras, visit our [**Knowledge Base**][knowledge-base-url] or contact us at [customersuccess@zivid.com](mailto:customersuccess@zivid.com).

## Licence
Zivid Samples are distributed under the [BSD license](LICENSE).

[header-image]: https://www.zivid.com/hubfs/softwarefiles/images/zivid-generic-github-header.png

[QuickCaptureTutorial-url]: source/Camera/Basic/QuickCaptureTutorial.md
[CompleteCaptureTutorial-url]: source/Camera/Basic/CaptureTutorial.md
[Capture-url]: source/Camera/Basic/Capture/Capture.cs
[Capture2D-url]: source/Camera/Basic/Capture2D/Capture2D.cs
[CaptureAssistant-url]: source/Camera/Basic/CaptureAssistant/CaptureAssistant.cs
[CaptureFromFile-url]: source/Camera/Basic/CaptureFromFile/CaptureFromFile.cs
[CaptureWithSettingsFromYML-url]: source/Camera/Basic/CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs
[CaptureHDR-url]: source/Camera/Basic/CaptureHDR/CaptureHDR.cs
[CaptureHDRCompleteSettings-url]: source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs
[CaptureHDRLoop-url]: source/Camera/Advanced/CaptureHDRLoop/CaptureHDRLoop.cs
[CameraUserData-url]: source/Camera/InfoUtilOther/CameraUserData/CameraUserData.cs
[GetCameraIntrinsics-url]: source/Camera/InfoUtilOther/GetCameraIntrinsics/GetCameraIntrinsics.cs
[Warmup-url]: source/Camera/InfoUtilOther/Warmup/Warmup.cs
[CaptureFromFileVis3D-url]: source/Applications/Basic/Visualization/CaptureFromFileVis3D/CaptureFromFileVis3D.cs
[CaptureVis3D-url]: source/Applications/Basic/Visualization/CaptureVis3D/CaptureVis3D.cs
[ReadIterateZDF-url]: source/Applications/Basic/FileFormats/ReadIterateZDF/ReadIterateZDF.cs
[ZDF2PLY-url]: source/Applications/Basic/FileFormats/ZDF2PLY/ZDF2PLY.cs
[HandEyeCalibrationReadme-url]: source/Applications/Advanced/HandEyeCalibration/README.md
[HandEyeCalibration-url]: source/Applications/Advanced/HandEyeCalibration/HandEyeCalibration/HandEyeCalibration.cs
[UtilizeEyeInHandCalibration-url]: source/Applications/Advanced/HandEyeCalibration/UtilizeEyeInHandCalibration/UtilizeEyeInHandCalibration.cs
[MultiCameraCalibration-url]: source/Applications/Advanced/MultiCameraCalibration/MultiCameraCalibration.cs
[Downsample-url]: source/Applications/Advanced/Downsample/Downsample.cs

[knowledge-base-url]: https://support.zivid.com/
[zivid-software-installation-url]: https://support.zivid.com/latest/academy/getting-started/zivid-software-installation.html
[zivid-sample-data-url]: https://support.zivid.com/latest/academy/samples/sample-data.html
