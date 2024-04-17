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
[**Save**](#Save)

---



## Introduction

This tutorial describes the most basic way to use the Zivid SDK to
capture point clouds.

For MATLAB see [Zivid Quick Capture Tutorial for
MATLAB](https://github.com/zivid/zivid-matlab-samples/blob/master/source/Camera/Basic/QuickCaptureTutorial.md)

**Prerequisites**

  - Install [Zivid
    Software](https://support.zivid.com/latest//getting-started/software-installation.html).
  - For Python: install
    [zivid-python](https://github.com/zivid/zivid-python#installation)

## Initialize

Calling any of the APIs in the Zivid SDK requires initializing the Zivid
application and keeping it alive while the program runs.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L14))

``` sourceCode cs
var zivid = new Zivid.NET.Application();
```

## Connect

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L17))

``` sourceCode cs
var camera = zivid.ConnectCamera();
```

## Configure

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureAssistant/CaptureAssistant.cs#L21-L29))

``` sourceCode cs
var suggestSettingsParameters = new Zivid.NET.CaptureAssistant.SuggestSettingsParameters
{
	AmbientLightFrequency =
		Zivid.NET.CaptureAssistant.SuggestSettingsParameters.AmbientLightFrequencyOption.none,
	MaxCaptureTime = Duration.FromMilliseconds(1200)
};
Console.WriteLine("Running Capture Assistant with parameters:\n{0}", suggestSettingsParameters);
var settings = Zivid.NET.CaptureAssistant.Assistant.SuggestSettings(camera, suggestSettingsParameters);
```

## Capture

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L26))

``` sourceCode cs
using (var frame = camera.Capture(settings))
```

## Save

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L28-L30))

``` sourceCode cs
var dataFile = "Frame.zdf";
frame.Save(dataFile);
```

The API detects which format to use. See [Point
Cloud](https://support.zivid.com/latest//reference-articles/point-cloud-structure-and-output-formats.html)
for a list of supported formats.

-----

Tip:

You can open and view `Frame.zdf` file in [Zivid
Studio](https://support.zivid.com/latest//getting-started/studio-guide.html).
.. rubric:: Conclusion

This tutorial shows the most basic way to use the Zivid SDK to connect
to, capture, and save from the Zivid camera.

For a more in-depth tutorial check out the complete
[CaptureTutorial](https://github.com/zivid/zivid-csharp-samples/tree/master/source/Camera/Basic/CaptureTutorial.md).
