# Quick Capture Tutorial

Note\! This tutorial has been generated for use on Github. For original
tutorial see:
[QuickCaptureTutorial](https://support.zivid.com/latest/getting-started/quick-capture-tutorial.html)



---

*Contents:*
[**Introduction**](#Introduction) |
[**Initialize**](#Initialize) |
[**Connect**](#Connect) |
[**Configure**](#Configure) |
[**Capture**](#Capture) |
[**Save**](#Save) |
[**Utilize**](#Utilize)

---



## Introduction

This tutorial describes the most basic way to use the Zivid SDK to
capture point clouds.

**Prerequisites**

  - Install [Zivid
    Software](https://support.zivid.com/latest//getting-started/software-installation.html).
  - For Python: install
    [zivid-python](https://github.com/zivid/zivid-python#installation)

## Initialize

Calling any of the APIs in the Zivid SDK requires initializing the Zivid
application and keeping it alive while the program runs.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L13))

``` sourceCode cs
var zivid = new Zivid.NET.Application();
```

## Connect

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L16))

``` sourceCode cs
var camera = zivid.ConnectCamera();
```

## Configure

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs#L25))

``` sourceCode cs
var settings = new Zivid.NET.Settings(settingsFile);
```

## Capture

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L26))

``` sourceCode cs
using (var frame = camera.Capture2D3D(settings))
```

## Save

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L33-L35))

``` sourceCode cs
var dataFile = "Frame.zdf";
frame.Save(dataFile);
.. tab-item:: Export
```

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L37-L39))

``` sourceCode cs
var dataFilePLY = "PointCloud.ply";
frame.Save(dataFilePLY);
```

For other exporting options, see [Point
Cloud](https://support.zivid.com/latest//reference-articles/point-cloud-structure-and-output-formats.html)
for a list of supported formats

## Utilize

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/FileFormats/ReadIterateZDF/ReadIterateZDF.cs#L23-L24))

``` sourceCode cs
var pointCloud = frame.PointCloud;
var pointCloudData = pointCloud.CopyPointsXYZColorsRGBA();
```

-----

Tip:

1.  You can export Preset settings to YML from [Zivid
    Studio](https://support.zivid.com/latest//getting-started/studio-guide.html)

\#. You can open and view `Frame.zdf` file in [Zivid
Studio](https://support.zivid.com/latest//getting-started/studio-guide.html).
.. rubric:: Conclusion

This tutorial shows the most basic way to use the Zivid SDK to connect
to, capture, and save from the Zivid camera.

For a more in-depth tutorial check out the complete
[CaptureTutorial](https://github.com/zivid/zivid-csharp-samples/tree/master/source/Camera/Basic/CaptureTutorial.md).
