# Capture Tutorial

Note\! This tutorial has been generated for use on Github. For original
tutorial see:
[CaptureTutorial](https://support.zivid.com/latest/academy/camera/capture-tutorial.html)



---

*Contents:*
[**Introduction**](#Introduction) |
[**Initialize**](#Initialize) |
[**Connect**](#Connect) |
[**Configure**](#Configure) |
[**Capture**](#Capture) |
[**Multithreading**](#Multithreading) |
[**Conclusion**](#Conclusion)

---



## Introduction

This tutorial describes how to use the Zivid SDK to capture point clouds
and 2D images.

For MATLAB see [Zivid Capture Tutorial for
MATLAB](https://github.com/zivid/zivid-matlab-samples/blob/master/source/Camera/Basic/CaptureTutorial.md).

-----

Tip:

If you prefer watching a video, our webinar [Making 3D captures easy - A
tour of Zivid Studio and Zivid
SDK](https://www.zivid.com/webinars-page?wchannelid=ffpqbqc7sg&wmediaid=ce68dbjldk)
covers the same content as the Capture Tutorial. .. rubric::
Prerequisites

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
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L17))

``` sourceCode cs
var camera = zivid.ConnectCamera();
```

### Specific Camera

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
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/InfoUtilOther/CameraInfo/CameraInfo.cs#L15-L21))

``` sourceCode cs
var cameras = zivid.Cameras;
Console.WriteLine("Number of cameras found: {0}", cameras.Count);
foreach (var camera in cameras)
{
	Console.WriteLine("Camera Info: {0}", camera.Info);
	Console.WriteLine("Camera State: {0}", camera.State);
}
```

### File Camera

The file camera option allows you to experiment with the SDK without
access to a physical camera. The file cameras can be found in [Sample
Data](https://support.zivid.com/latest/api-reference/samples/sample-data.html)
where there are multiple file cameras to choose from. Each file camera
demonstrates a use case within one of the main applications of the
respective camera model. The example below shows how to create a file
camera using the Zivid 2 M70 file camera from [Sample
Data](https://support.zivid.com/latest/api-reference/samples/sample-data.html).

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureFromFileCamera/CaptureFromFileCamera.cs#L30))

``` sourceCode cs
fileCamera = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/FileCameraZivid2M70.zfc";
```

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureFromFileCamera/CaptureFromFileCamera.cs#L36))

``` sourceCode cs
var camera = zivid.CreateFileCamera(fileCamera);
```

The acquisition settings should be initialized like shown below, but you
are free to alter the processing settings.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureFromFileCamera/CaptureFromFileCamera.cs#L39-L45))

``` sourceCode cs
var settings = new Zivid.NET.Settings
{
	Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
	Processing = { Filters = { Smoothing = { Gaussian = { Enabled = true, Sigma = 1.5 } },
							Reflection = { Removal = { Enabled = true, Experimental = { Mode = ReflectionFilterModeOption.Global} } } },
				Color = { Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 } } }
};
```

You can read more about the file camera option in [File
Camera](https://support.zivid.com/latest/academy/camera/file-camera.html).

## Configure

As with all cameras there are settings that can be configured.

### Presets

The recommendation is to use
[Presets](https://support.zivid.com/latest/reference-articles/presets-settings.html)
available in Zivid Studio and as .yml files (see `load_yml_label`
below). Alternatively, you can use our Capture Assistant, or you may
configure the settings manually.

### Capture Assistant

It can be difficult to know what settings to configure. Luckily we have
the Capture Assistant. This is available in the Zivid SDK to help
configure camera settings.

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
Settings](https://support.zivid.com/latest/reference-articles/camera-settings.html).
Note that Zivid 2 has a set of [standard
settings](https://support.zivid.com/latest//reference-articles/standard-acquisition-settings-zivid-two.html).

#### Single Acquisition

We can create settings for a single acquisition capture.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L20-L23))

``` sourceCode cs
var settings = new Zivid.NET.Settings
{
	Acquisitions = { new Zivid.NET.Settings.Acquisition { } }
};
```

#### Multi Acquisition HDR

We may also create settings to be used in a multi-acquisition HDR
capture.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDR/CaptureHDR.cs#L22-L28))

``` sourceCode cs
var settings = new Zivid.NET.Settings();
foreach (var aperture in new double[] { 9.57, 4.76, 2.59 })
{
	Console.WriteLine("Adding acquisition with aperture = " + aperture);
	var acquisitionSettings = new Zivid.NET.Settings.Acquisition { Aperture = aperture };
	settings.Acquisitions.Add(acquisitionSettings);
}
```

Fully configured settings are demonstrated below.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs#L31-L94))

``` sourceCode cs
Console.WriteLine("Configuring settings for capture:");
var settings = new Zivid.NET.Settings()
{
	Experimental = { Engine = Zivid.NET.Settings.ExperimentalGroup.EngineOption.Phase },
	Sampling = { Color = Zivid.NET.Settings.SamplingGroup.ColorOption.Rgb, Pixel = Zivid.NET.Settings.SamplingGroup.PixelOption.All },
	RegionOfInterest = { Box = {
							Enabled = true,
							PointO = new Zivid.NET.PointXYZ{ x = 1000, y = 1000, z = 1000 },
							PointA = new Zivid.NET.PointXYZ{ x = 1000, y = -1000, z = 1000 },
							PointB = new Zivid.NET.PointXYZ{ x = -1000, y = 1000, z = 1000 },
							Extents = new Zivid.NET.Range<double>(-1000, 1000),
						},
						Depth =
						{
							Enabled = true,
							Range = new Zivid.NET.Range<double>(200, 2000),
						}
	},
	Processing = { Filters = { Smoothing = { Gaussian = { Enabled = true, Sigma = 1.5 } },
							Noise = { Removal = { Enabled = true, Threshold = 7.0 },
										Suppression = { Enabled = true },
										Repair ={ Enabled = true } },
							Outlier = { Removal = { Enabled = true, Threshold = 5.0 } },
							Reflection = { Removal = { Enabled = true, Experimental = { Mode = ReflectionFilterModeOption.Global} } },
							Cluster = { Removal = { Enabled = true, MaxNeighborDistance = 10, MinArea = 100} },
							Experimental = { ContrastDistortion = { Correction = { Enabled = true,
																					Strength = 0.4 },
																	Removal = { Enabled = true,
																				Threshold = 0.5 } },
												HoleFilling = { Enabled = true,
																HoleSize = 0.2,
																Strictness = 1 } } },
				Color = { Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 },
							Gamma = 1.0,
							Experimental = { Mode = ColorModeOption.Automatic } } }
};
Console.WriteLine(settings);
Console.WriteLine("Configuring base acquisition with settings same for all HDR acquisitions:");
var baseAcquisition = new Zivid.NET.Settings.Acquisition { };
Console.WriteLine(baseAcquisition);

Console.WriteLine("Configuring acquisition settings different for all HDR acquisitions:");
Tuple<double[], Duration[], double[], double[]> exposureValues = GetExposureValues(camera);
double[] aperture = exposureValues.Item1;
Duration[] exposureTime = exposureValues.Item2;
double[] gain = exposureValues.Item3;
double[] brightness = exposureValues.Item4;
for (int i = 0; i < aperture.Length; i++)
{
	Console.WriteLine("Acquisition {0}:", i + 1);
	Console.WriteLine("  Exposure Time: {0}", exposureTime[i].Microseconds);
	Console.WriteLine("  Aperture: {0}", aperture[i]);
	Console.WriteLine("  Gain: {0}", gain[i]);
	Console.WriteLine("  Brightness: {0}", brightness[i]);
	var acquisitionSettings = baseAcquisition.CopyWith(s =>
													{
														s.Aperture = aperture[i];
														s.ExposureTime = exposureTime[i];
														s.Gain = gain[i];
														s.Brightness = brightness[i];
													});
	settings.Acquisitions.Add(acquisitionSettings);
}
```

#### 2D Settings

It is possible to only capture a 2D image. This is faster than a 3D
capture. 2D settings are configured as follows.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture2D/Capture2D.cs#L21-L27))

``` sourceCode cs
var settings2D = new Zivid.NET.Settings2D
{
	Acquisitions = { new Zivid.NET.Settings2D.Acquisition {
		Aperture = 11.31, ExposureTime = Duration.FromMicroseconds(30000), Gain = 2.0, Brightness = 1.80
	} },
	Processing = { Color = { Balance = { Red = 1.0, Blue = 1.0, Green = 1.0 } } }
};
```

### Load

Zivid Studio can store the current settings to .yml files. These can be
read and applied in the API. You may find it easier to modify the
settings in these (human-readable) yaml-files in your preferred editor.
Check out
[Presets](https://support.zivid.com/latest/reference-articles/presets-settings.html)
for recommended .yml files tuned for your application.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs#L106-L111))

``` sourceCode cs
var settingsFile = "Settings.yml";
Console.WriteLine("Loading settings from file: " + settingsFile);
var settingsFromFile = new Zivid.NET.Settings(settingsFile);
```

### Save

You can also save settings to .yml file.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs#L106-L108))

``` sourceCode cs
var settingsFile = "Settings.yml";
Console.WriteLine("Saving settings to file: " + settingsFile);
settings.Save(settingsFile);
```

-----

Caution\!:

> Zivid settings files must use .yml file extension ( not .yaml).

## Capture

Now we can capture a 3D image. Whether there is a single acquisition or
multiple acquisitions (HDR) is given by the number of `acquisitions` in
`settings`.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L26))

``` sourceCode cs
using (var frame = camera.Capture(settings))
```

The `Zivid.NET.Frame` contains the point cloud and color image (stored
on compute device memory) and the capture and camera information.

### Load

Once saved, the frame can be loaded from a ZDF file.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/FileFormats/ReadIterateZDF/ReadIterateZDF.cs#L15-L18))

``` sourceCode cs
var dataFile =
	Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/Zivid3D.zdf";
Console.WriteLine("Reading ZDF frame from file: " + dataFile);
var frame = new Zivid.NET.Frame(dataFile);
```

Saving to a ZDF file is addressed later in the tutorial.

### Capture 2D

If we only want to capture a 2D image, which is faster than 3D, we can
do so via the 2D API.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture2D/Capture2D.cs#L30))

``` sourceCode cs
using (var frame2D = camera.Capture(settings2D))
```

-----

Caution\!:

> Zivid One+ camera has a time penalty when changing the capture mode
> (2D and 3D) if the 2D capture settings use brightness \> 0.

You can read more about it in [2D and 3D switching
limitation](https://support.zivid.com/latest//support/2d-3d-switching-limitation.html).
Save ----

We can now save our results.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L28-L30))

``` sourceCode cs
var dataFile = "Frame.zdf";
frame.Save(dataFile);
```

-----

Tip:

You can open and view `Frame.zdf` file in [Zivid
Studio](https://support.zivid.com/latest//getting-started/studio-guide.html).
Export ^^^^^^

The API detects which format to use. See [Point
Cloud](https://support.zivid.com/latest//reference-articles/point-cloud-structure-and-output-formats.html)
for a list of supported formats. For example, we can export the point
cloud to .ply format.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L32-L34))

``` sourceCode cs
var dataFilePLY = "PointCloud.ply";
frame.Save(dataFilePLY);
```

### Save 2D

We can get 2D color image from a 3D capture.

No source available for {language\_name} 2D captures also produce 2D
color images.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture2D/Capture2D.cs#L33))

``` sourceCode cs
var image = frame2D.ImageRGBA();
```

Then, we can save the 2D image.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture2D/Capture2D.cs#L62-L64))

``` sourceCode cs
var imageFile = "Image.png";
Console.WriteLine("Saving 2D color image to file: {0}", imageFile);
image.Save(imageFile);
```

## Multithreading

Operations on camera objects are thread-safe, but other operations like
listing cameras and connecting to cameras should be executed in
sequence. Find out more in
[CaptureTutorial](https://github.com/zivid/zivid-csharp-samples/tree/master/source/Camera/Basic/CaptureTutorial.md).

## Conclusion

This tutorial shows how to use the Zivid SDK to connect to, configure,
capture, and save from the Zivid camera.
