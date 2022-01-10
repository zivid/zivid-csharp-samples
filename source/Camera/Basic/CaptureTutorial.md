# Capture Tutorial



---

*Contents:*
[**Introduction**](#Introduction) |
[**Initialize**](#Initialize) |
[**Connect**](#Connect) |
[**Connect**](#Connect---Specific-Camera) |
[**Connect**](#Connect---File-Camera) |
[**Configure**](#Configure) |
[**Capture**](#Capture-Assistant) |
[**Manual**](#Manual-configuration) |
[**Single**](#Single-Acquisition) |
[**Multi**](#Multi-Acquisition-HDR) |
[**2D**](#2D-Settings) |
[**From**](#From-File) |
[**Capture**](#Capture) |
[**Capture2D**](#Capture2D) |
[**Save**](#Save) |
[**Save**](#Save-2D) |
[**Conclusion**](#Conclusion)

---
## Introduction

This tutorial describes how to use the Zivid SDK to capture point clouds
and 2D images.

For MATLAB see [Zivid Capture Tutorial for
MATLAB](https://github.com/zivid/zivid-matlab-samples/blob/master/source/Camera/Basic/CaptureTutorial.md).

**Prerequisites**

  - Install [Zivid
    Software](https://support.zivid.com/latest//getting-started/software-installation.html).
  - For Python: install
    [zivid-python](https://github.com/zivid/zivid-python#installation)

## Initialize

Calling any of the APIs in the Zivid SDK requires initializing the Zivid
application and keeping it alive while the program runs.

-----

Note:

`Zivid::Application` must be kept alive while operating the Zivid
Camera. This is essentially the Zivid driver.

-----

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L14))

``` sourceCode cs
var zivid = new Zivid.NET.Application();
```

## Connect

Now we can connect to the camera.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L18))

``` sourceCode cs
var camera = zivid.ConnectCamera();
```

### Connect - Specific Camera

Sometimes multiple cameras are connected to the same computer, but it
might be necessary to work with a specific camera in the code. This can
be done by providing the serial number of the wanted camera.

``` sourceCode csharp
var camera = zivid.ConnectCamera(new Zivid.NET.CameraInfo.SerialNumber("2020C0DE"));
```

-----

Note:

The serial number of your camera is shown in the Zivid Studio.

-----

You may also list all cameras connected to the computer, and view their
serial numbers through

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/InfoUtilOther/PrintVersionInfo/PrintVersionInfo.cs#L15-L20))

``` sourceCode cs
var cameras = zivid.Cameras;
Console.WriteLine("Number of cameras found: {0}", cameras.Count);
foreach(var camera in cameras)
{
	Console.WriteLine("Camera Info: {0}", camera.Info);
}
```

### Connect - File Camera

You may want to experiment with the SDK, without access to a physical
camera. Minor changes are required to keep the sample working.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureFromFileCamera/CaptureFromFileCamera.cs#L18-L23))

``` sourceCode cs
var fileCamera = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
				+ "/Zivid/FileCameraZividOne.zfc";
var camera = zivid.CreateFileCamera(fileCamera);
```

-----

Note:

The quality of the point cloud you get from FileCameraZividOne.zfc is
not representative of the Zivid 3D cameras.

-----

## Configure

As with all cameras there are settings that can be configured. These may
be set manually, or you use our Capture Assistant.

### Capture Assistant

It can be difficult to know what settings to configure. Luckily we have
the Capture Assistant. This is available in the Zivid SDK to help
configure camera settings.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureAssistant/CaptureAssistant.cs#L19-L26))

``` sourceCode cs
var suggestSettingsParameters = new Zivid.NET.CaptureAssistant.SuggestSettingsParameters {
	AmbientLightFrequency =
		Zivid.NET.CaptureAssistant.SuggestSettingsParameters.AmbientLightFrequencyOption.none,
	MaxCaptureTime = Duration.FromMilliseconds(1200)
};
Console.WriteLine("Running Capture Assistant with parameters:\n{0}", suggestSettingsParameters);
var settings = Zivid.NET.CaptureAssistant.Assistant.SuggestSettings(camera, suggestSettingsParameters);
```

There are only two parameters to configure with Capture Assistant:

1.  **Maximum Capture Time** in number of milliseconds.
    1.  Minimum capture time is 200 ms. This allows only one
        acquisition.
    2.  The algorithm will combine multiple acquisitions if the budget
        allows.
    3.  The algorithm will attempt to cover as much of the dynamic range
        in the scene as possible.
    4.  A maximum capture time of more than 1 second will get good
        coverage in most scenarios.
2.  **Ambient light compensation**
    1.  May restrict capture assistant to exposure periods that are
        multiples of the ambient light period.
    2.  60Hz is found in Japan, Americas, Taiwan, South Korea and
        Philippines.
    3.  50Hz is common in the rest of the world.

### Manual configuration

Another option is to configure settings manually. For more information
about what each settings does, please see [Camera
Settings](https://support.zivid.com/latest/rst/reference-articles/camera-settings.html).
Note that Zivid Two has a set of [standard
settings](https://support.zivid.com/latest//reference-articles/standard-acquisition-settings-zivid-two.html).

#### Single Acquisition

We can create settings for a single capture.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L22-L27))

``` sourceCode cs
var settings = new Zivid.NET.Settings {
	Acquisitions = { new Zivid.NET.Settings.Acquisition { Aperture = 5.66,
														ExposureTime =
															Duration.FromMicroseconds(6500) } },
	Processing = { Filters = { Outlier = { Removal = { Enabled = true, Threshold = 5.0 } } } }
};
```

#### Multi Acquisition HDR

We may also create settings to be used in an HDR capture.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDR/CaptureHDR.cs#L22-L28))

``` sourceCode cs
var settings = new Zivid.NET.Settings();
foreach(var aperture in new double[] { 9.57, 4.76, 2.59 })
{
	Console.WriteLine("Adding acquisition with aperture = " + aperture);
	var acquisitionSettings = new Zivid.NET.Settings.Acquisition { Aperture = aperture };
	settings.Acquisitions.Add(acquisitionSettings);
}
```

#### 2D Settings

It is possible to only capture a 2D image. This is faster than a 3D
capture. 2D settings are configured as follows.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture2D/Capture2D.cs#L21-L26))

``` sourceCode cs
var settings2D = new Zivid.NET.Settings2D {
	Acquisitions = { new Zivid.NET.Settings2D.Acquisition {
		Aperture = 11.31, ExposureTime = Duration.FromMicroseconds(30000), Gain = 2.0, Brightness = 1.80
	} },
	Processing = { Color = { Balance = { Red = 1.0, Blue = 1.0, Green = 1.0 } } }
};
```

### From File

Zivid Studio can store the current settings to .yml files. These can be
read and applied in the API. You may find it easier to modify the
settings in these (human-readable) yaml-files in your preferred editor.

``` sourceCode csharp
var settings = new Zivid.NET.Settings("Settings.yml");
```

-----

Caution\!:

Zivid settings files must use .yml file extension ( not .yaml).

## Capture

Now we can capture a 3D image. Whether there is a single acquisition or
multiple acquisitions (HDR) is given by the number of `acquisitions` in
`settings`.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L31))

``` sourceCode cs
using(var frame = camera.Capture(settings))
```

### Capture2D

If we only want to capture a 2D image, which is faster than 3D, we can
do so via the 2D API.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture2D/Capture2D.cs#L30))

``` sourceCode cs
using(var frame2D = camera.Capture(settings2D))
```

-----

Caution\!:

  - Zivid One+ camera has a time penalty when changing the capture mode
    (2D and 3D) if the 2D capture settings use brightness \> 0.  
    You can read more about it in [2D and 3D switching
    limitation](https://support.zivid.com/latest//support/2d-3d-switching-limitation.html).

## Save

We can now save our results.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L34-L37))

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

### Save 2D

If we capture a 2D image, we can save it.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture2D/Capture2D.cs#L34-L66))

``` sourceCode cs
var image = frame2D.ImageRGBA();
var imageFile = "Image.png";
Console.WriteLine("Saving image to file: {0}", imageFile);
image.Save(imageFile);
```

## Conclusion

This tutorial shows how to use the Zivid SDK to connect to, configure,
capture, and save from the Zivid camera.
