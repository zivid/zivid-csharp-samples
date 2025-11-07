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
[**Capture**](#Capture-2D3D) |
[**Save**](#Save) |
[**File**](#File-Camera) |
[**Multithreading**](#Multithreading) |
[**Conclusion**](#Conclusion)

---



## Introduction

This tutorial describes how to use the Zivid SDK to capture point clouds
and 2D images.

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
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L13))

``` sourceCode cs
var zivid = new Zivid.NET.Application();
```

## Connect

Now we can connect to the camera.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L16))

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

> The serial number of your camera is shown in the Zivid Studio.

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

## Configure

As with all cameras there are settings that can be configured.

### Presets

The recommendation is to use
[Presets](https://support.zivid.com/latest/reference-articles/presets-settings.html)
available in Zivid Studio and as .yml files (see below). Presets are
designed to work well for most cases right away, making them a great
starting point. If needed, you can easily fine-tune the settings for
better results. You can edit the YAML files in any text editor or code
the settings manually.

### Load

You can export camera settings to .yml files from Zivid Studio. These
can be loaded and applied in the API.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs#L165-L170))

``` sourceCode cs
var settingsFile = "Settings.yml";
Console.WriteLine("Loading settings from file: " + settingsFile);
var settingsFromFile = new Zivid.NET.Settings(settingsFile);
```

### Save

You can also save settings to .yml file.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs#L165-L167))

``` sourceCode cs
var settingsFile = "Settings.yml";
Console.WriteLine("Saving settings to file: " + settingsFile);
settings.Save(settingsFile);
```

### Manual configuration

Another option is to configure settings manually. For more information
about what each settings does, please see [Camera
Settings](https://support.zivid.com/latest/reference-articles/camera-settings.html).
Then, the next step it's [Capturing High Quality Point
Clouds](https://support.zivid.com/latest/academy/camera/capturing-high-quality-point-clouds.html)

#### Single 2D and 3D Acquisition - Default settings

We can create settings for a single acquisition capture.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L19-L23))

``` sourceCode cs
var settings = new Zivid.NET.Settings
{
	Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
	Color = new Zivid.NET.Settings2D { Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } } }
};
```

#### Multi Acquisition HDR

We may also create settings with multiple acquisitions for an HDR
capture.

([go to source]())

``` sourceCode cs
var settings = new Zivid.NET.Settings();
foreach (var exposure in new Duration[] { Duration.FromMicroseconds(1000), Duration.FromMicroseconds(10000) })
{
	Console.WriteLine("Adding acquisition with exposure time of " + exposure.Microseconds + " microseconds");
	var acquisitionSettings = new Zivid.NET.Settings.Acquisition { ExposureTime = exposure };
	settings.Acquisitions.Add(acquisitionSettings);
}
settings.Color = new Zivid.NET.Settings2D { Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } } };
```

#### Fully Configured Settings

2D Settings, such as color balance and gamma, configured manually:

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs#L32-L59))

``` sourceCode cs
Console.WriteLine("Configuring settings for capture:");
var settings2D = new Zivid.NET.Settings2D()
{
	Sampling =
	{
		Color = Zivid.NET.Settings2D.SamplingGroup.ColorOption.Rgb,
		Pixel = Zivid.NET.Settings2D.SamplingGroup.PixelOption.All,
		Interval =
		{
			Enabled = false,
			Duration = Duration.FromMicroseconds(10000),
		},
	},
	Processing =
	{
		Color =
		{
			Balance =
			{
				Blue = 1.0,
				Green = 1.0,
				Red = 1.0,
			},
			Gamma = 1.0,
			Experimental = { Mode = Zivid.NET.Settings2D.ProcessingGroup.ColorGroup.ExperimentalGroup.ModeOption.Automatic },
		},
	},
};
```

Manually configured 3D settings such as engine, region of interest,
filter settings and more:

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs#L60-L126))

``` sourceCode cs
var settings = new Zivid.NET.Settings()
{
	Engine = Zivid.NET.Settings.EngineOption.Stripe,
RegionOfInterest =
{
	Box = {
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
	},
},
Processing =
{
	Filters =
	{
		Cluster =
		{
			Removal = { Enabled = true, MaxNeighborDistance = 10, MinArea = 100}
		},
		Hole =
		{
			Repair = { Enabled = true, HoleSize = 0.2, Strictness = 1 },
		},
		Noise =
		{
			Removal = { Enabled = true, Threshold = 7.0 },
			Suppression = { Enabled = true },
			Repair = { Enabled = true },
		},
		Outlier =
		{
			Removal = { Enabled = true, Threshold = 5.0 },
		},
		Reflection =
		{
			Removal = { Enabled = true, Mode = ReflectionFilterModeOption.Global },
		},
		Smoothing =
		{
			Gaussian = { Enabled = true, Sigma = 1.5 },
		},
		Experimental =
		{
			ContrastDistortion =
			{
				Correction = { Enabled = true, Strength = 0.4 },
				Removal = { Enabled = true, Threshold = 0.5 },
			},
		},
	},
	Resampling = { Mode = Zivid.NET.Settings.ProcessingGroup.ResamplingGroup.ModeOption.Upsample2x2 },
},
Diagnostics = { Enabled = false },
```

> };
> 
> settings.Color = settings2D;
> 
> SetSamplingPixel(ref settings, camera); Console.WriteLine(settings);

Different values per acquisition are also possible:

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs#L128-L154))

``` sourceCode cs
Console.WriteLine("Configuring acquisition settings different for all HDR acquisitions:");
var baseAcquisition = new Zivid.NET.Settings.Acquisition { };
Console.WriteLine(baseAcquisition);
var baseAcquisition2D = new Zivid.NET.Settings2D.Acquisition { };
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
var aquisitionSettings2D = MakeSettings2D(camera);
settings.Color.Acquisitions = aquisitionSettings2D.Acquisitions;
```

## Capture 2D3D

Now we can capture a 2D and 3D image (point cloud with color). Whether
there is a single acquisition or multiple acquisitions (HDR) is given by
the number of `acquisitions` in `settings`.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L26))

``` sourceCode cs
using (var frame = camera.Capture2D3D(settings))
```

The `Zivid.NET.Frame` contains the point cloud, the color image, the
capture, and the camera information (all of which are stored on the
compute device memory).

### Capture 3D

If we only want to capture 3D, the points cloud without color, we can do
so via the `capture3D` API.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs#L86))

``` sourceCode cs
using (var frame3D = camera.Capture3D(settings))
```

### Capture 2D

If we only want to capture a 2D image, which is faster than 3D, we can
do so via the `capture2D` API.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs#L42))

``` sourceCode cs
using (var frame2D = camera.Capture2D(settings))
```

## Save

We can now save our results.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L33-L35))

``` sourceCode cs
var dataFile = "Frame.zdf";
frame.Save(dataFile);
```

-----

Tip:

> You can open and view `Frame.zdf` file in [Zivid
> Studio](https://support.zivid.com/latest//getting-started/studio-guide.html).

### Export

In the next code example, the point cloud is exported to the .ply
format. For other exporting options, see [Point
Cloud](https://support.zivid.com/latest//reference-articles/point-cloud-structure-and-output-formats.html)
for a list of supported formats.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L37-L39))

``` sourceCode cs
var dataFilePLY = "PointCloud.ply";
frame.Save(dataFilePLY);
```

### Load

Once saved, the frame can be loaded from a ZDF file.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/FileFormats/ReadIterateZDF/ReadIterateZDF.cs#L15-L20))

``` sourceCode cs
var dataFile =
	Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/Zivid3D.zdf";
Console.WriteLine("Reading ZDF frame from file: " + dataFile);
using (var frame = new Zivid.NET.Frame(dataFile))
{
```

### Save 2D

From a `capture2D()` you get a `Frame2D`. There are two color spaces
available for 2D images: linear RGB and sRGB. The `imageRGBA()` will
return an image in the linear RGB color space. If you append `_SRGB` to
the function name then the returned image will be in the sRGB color
space

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs#L49))

``` sourceCode cs
var imageRGBA = frame2D.ImageRGBA();
.. tab-item:: sRGB
```

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs#L67))

``` sourceCode cs
var imageSRGB = frame2D.ImageRGBA_SRGB();
```

Then, we can save the 2D image in linear RGB or sRGB color space.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs#L50-L52))

``` sourceCode cs
var imageFile = "ImageRGBA_linear.png";
Console.WriteLine($"Saving 2D color image (Linear RGB) to file: {imageFile}");
imageRGBA.Save(imageFile);
.. tab-item:: sRGB
```

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs#L68-L70))

``` sourceCode cs
var imageFile = "ImageRGBA_sRGB.png";
Console.WriteLine($"Saving 2D color image (sRGB color space) to file: {imageFile}");
imageSRGB.Save(imageFile);
```

We can get 2D color image directly from the point cloud. This image will
have the same resolution as the point cloud and it will be in the sRGB
color space.

([go to source]())

``` sourceCode cs
var pointCloud = frame.PointCloud;
var image2DInPointCloudResolution = pointCloud.CopyImageRGBA_SRGB();
```

We can get the 2D color image from `Frame2D`, which is part of the
`Frame` object, obtained from `capture2D3D()`. This image will have the
resolution given by the 2D settings inside the 2D3D settings.

([go to source]())

``` sourceCode cs
var image2D = frame.Frame2D.ImageBGRA_SRGB();
```

## File Camera

A [file
camera](https://support.zivid.com/latest//academy/camera/file-camera.html)
allows you to experiment with the SDK without access to a physical
camera. The file cameras can be found in [Sample
Data](https://support.zivid.com/latest/api-reference/samples/sample-data.html)
where there are multiple file cameras to choose from.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureFromFileCamera/CaptureFromFileCamera.cs#L30))

``` sourceCode cs
fileCamera = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/FileCameraZivid2PlusMR60.zfc";
```

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureFromFileCamera/CaptureFromFileCamera.cs#L36))

``` sourceCode cs
var camera = zivid.CreateFileCamera(fileCamera);
```

The acquisition settings should be initialized like shown below, but you
are free to alter the processing settings.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/CaptureFromFileCamera/CaptureFromFileCamera.cs#L39-L68))

``` sourceCode cs
var settings2D = new Zivid.NET.Settings2D
{
	Acquisitions = { new Zivid.NET.Settings2D.Acquisition { } },
	Processing =
	{
		Color =
		{
			Balance = { Red = 1.0, Green = 1.0, Blue = 1.0 }
		}
	}
};
var settings = new Zivid.NET.Settings
{
	Acquisitions = { new Zivid.NET.Settings.Acquisition { } },
	Processing =
	{
		Filters =
		{
			Smoothing =
			{
				Gaussian = { Enabled = true, Sigma = 1.5 }
			},
			Reflection =
			{
				Removal = { Enabled = true, Mode = ReflectionFilterModeOption.Global}
			}
		}
	}
};
settings.Color = settings2D;
```

You can read more about the file camera option in [File
Camera](https://support.zivid.com/latest/academy/camera/file-camera.html).

## Multithreading

Operations on camera objects are thread-safe, but other operations like
listing cameras and connecting to cameras should be executed in
sequence. Find out more in
[CaptureTutorial](https://github.com/zivid/zivid-csharp-samples/tree/master/source/Camera/Basic/CaptureTutorial.md).

## Conclusion

This tutorial shows how to use the Zivid SDK to connect to, configure,
capture, and save from the Zivid camera.
