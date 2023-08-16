# C\# samples

This repository contains csharp code samples for Zivid SDK v2.10.0. For
tested compatibility with earlier SDK versions, please check out
[accompanying
releases](https://github.com/zivid/zivid-csharp-samples/tree/master/../../releases).

![image](https://www.zivid.com/hubfs/softwarefiles/images/zivid-generic-github-header.png)



---

*Contents:*
[**Tutorials**](#Tutorials-list) |
[**Samples**](#Samples-list) |
[**Installation**](#Installation) |
[**Support**](#Support) |
[**License**](#License)

---



## Tutorials list

  - [QuickCaptureTutorial](https://github.com/zivid/zivid-csharp-samples/tree/master/source/Camera/Basic/QuickCaptureTutorial.md)
  - [CaptureTutorial](https://github.com/zivid/zivid-csharp-samples/tree/master/source/Camera/Basic/CaptureTutorial.md)
  - [PointCloudTutorial](https://github.com/zivid/zivid-csharp-samples/tree/master/source/Applications/PointCloudTutorial.md)

## Samples list

There are two main categories of samples: **Camera** and
**Applications**. The samples in the **Camera** category focus only on
how to use the camera. The samples in the **Applications** category use
the output generated by the camera, such as the 3D point cloud, a 2D
image or other data from the camera. These samples shows how the data
from the camera can be used.

  - **Camera**
      - **Basic**
          - [Capture](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs) - Capture point clouds, with color, from the Zivid camera.
          - [Capture2D](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture2D/Capture2D.cs) - Capture 2D images from the Zivid camera.
          - [CaptureAssistant](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureAssistant/CaptureAssistant.cs) - Use Capture Assistant to capture point clouds, with color,
            from the Zivid camera.
          - [CaptureFromFileCamera](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureFromFileCamera/CaptureFromFileCamera.cs) - Capture point clouds, with color, with the Zivid file
            camera.
          - [CaptureHDR](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDR/CaptureHDR.cs) - Capture point clouds, with color, from the Zivid camera.
          - [CaptureHDRCompleteSettings](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs) - Capture point clouds, with color, from the Zivid camera
            with fully configured settings.
          - [CaptureWithSettingsFromYML](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs) - Capture point clouds, with color, from the Zivid camera,
            with settings from YML file.
      - **Advanced**
          - [CaptureHalconViaGenICam](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Advanced/CaptureHalconViaGenICam/CaptureHalconViaGenICam.cs) - Capture and save a point cloud, with colors, using GenICam
            interface and Halcon C++ SDK.
          - [CaptureHalconViaZivid](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Advanced/CaptureHalconViaZivid/CaptureHalconViaZivid.cs) - Capture a point cloud, with colors, using Zivid SDK,
            transform it to a Halcon point cloud and save it using
            Halcon C++ SDK.
          - [CaptureHDRLoop](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Advanced/CaptureHDRLoop/CaptureHDRLoop.cs) - Cover the same dynamic range in a scene with different
            acquisition settings to optimize for quality, speed, or to
            find a compromise.
          - [CaptureHDRPrintNormals](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Advanced/CaptureHDRPrintNormals/CaptureHDRPrintNormals.cs) - Capture Zivid point clouds, compute normals and print a
            subset.
      - **InfoUtilOther**
          - [CameraUserData](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/InfoUtilOther/CameraUserData/CameraUserData.cs) - Store user data on the Zivid camera.
          - [CaptureWithDiagnostics](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/InfoUtilOther/CaptureWithDiagnostics/CaptureWithDiagnostics.cs) - Capture point clouds, with color, from the Zivid camera,
            with settings from YML file and diagnostics enabled.
          - [FirmwareUpdater](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/InfoUtilOther/FirmwareUpdater/FirmwareUpdater.cs) - Update firmware on the Zivid camera.
          - [GetCameraIntrinsics](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/InfoUtilOther/GetCameraIntrinsics/GetCameraIntrinsics.cs) - Read intrinsic parameters from the Zivid camera (OpenCV
            model) or estimate them from the point cloud.
          - [PrintVersionInfo](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/InfoUtilOther/PrintVersionInfo/PrintVersionInfo.cs) - List connected cameras and print version information.
          - [Warmup](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/InfoUtilOther/Warmup/Warmup.cs) - A basic warm-up method for a Zivid camera with specified
            time and capture cycle.
  - **Applications**
      - **Basic**
          - **Visualization**
              - [CaptureFromFileCameraVis3D](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/Visualization/CaptureFromFileCameraVis3D/CaptureFromFileCameraVis3D.cs) - Capture point clouds, with color, with the Zivid file
                camera and visualize them.
              - [CaptureVis3D](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/Visualization/CaptureVis3D/CaptureVis3D.cs) - Capture point clouds, with color, from the Zivid
                camera, and visualize them.
          - **FileFormats**
              - [ReadIterateZDF](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/FileFormats/ReadIterateZDF/ReadIterateZDF.cs) - Read point cloud data from a ZDF file, iterate through
                it, and extract individual points.
              - [ZDF2PLY](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/FileFormats/ZDF2PLY/ZDF2PLY.cs) - Convert point cloud from a ZDF file to a PLY file.
      - **Advanced**
          - [Downsample](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/Downsample/Downsample.cs) - Downsample point cloud from a ZDF file.
          - [HandEyeCalibration](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/HandEyeCalibration/HandEyeCalibration/HandEyeCalibration.cs) - Perform Hand-Eye calibration.
          - [MultiCameraCalibration](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/MultiCameraCalibration/MultiCameraCalibration.cs) - Use captures of a calibration object to generate
            transformation matrices to a single coordinate frame.
          - [TransformPointCloudFromMillimetersToMeters](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/TransformPointCloudFromMillimetersToMeters/TransformPointCloudFromMillimetersToMeters.cs) - Transform point cloud data from millimeters to meters.
          - [TransformPointCloudViaCheckerboard](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/TransformPointCloudViaCheckerboard/TransformPointCloudViaCheckerboard.cs) - Transform a point cloud from camera to checkerboard (Zivid
            Calibration Board) coordinate frame by getting checkerboard
            pose from the API.
          - **HandEyeCalibration**
              - [PoseConversions](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/HandEyeCalibration/PoseConversions/PoseConversions.cs) - Convert to/from Transformation Matrix (Rotation Matrix
                + Translation Vector)
              - [UtilizeHandEyeCalibration](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/HandEyeCalibration/UtilizeHandEyeCalibration/UtilizeHandEyeCalibration.cs) - Transform single data point or entire point cloud from
                camera frame to robot base frame using Hand-Eye
                calibration

## Installation

1.  [Install Zivid
    Software](https://support.zivid.com/latest//getting-started/software-installation.html)
2.  [Download Zivid Sample
    Data](https://support.zivid.com/latest//api-reference/samples/sample-data.html)

Launch the Command Prompt by pressing `Win` + `R` keys on the keyboard,
then type `cmd` and press `Enter`.

Navigate to a location where you want to clone the repository, then run
to following command:

``` sourceCode bat
git clone https://github.com/zivid/zivid-csharp-samples
```

Open ZividNETSamples.sln in Visual Studio, build it and run it. If you
are uncertain about doing this, check out [Build C\# Samples using
Visual
Studio](https://support.zivid.com/latest/api-reference/samples/csharp/build-c-sharp-samples-using-visual-studio.html).

Some of the samples depend on external libraries, in particular
`MathNet.Numerics`. These libraries will be installed automatically
through NuGet when building the sample.

## Support

For more information about the Zivid cameras, please visit our
[Knowledge Base](https://support.zivid.com/latest). If you run into any
issues please check out
[Troubleshooting](https://support.zivid.com/latest/support/troubleshooting.html).

## License

Zivid Samples are distributed under the [BSD
license](https://github.com/zivid/zivid-csharp-samples/tree/master/LICENSE).
