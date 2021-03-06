## Introduction

This tutorial describes how to use Zivid SDK to capture point clouds and 2D images.

1. [Initialize](#initialize)
2. [Connect](#connect)
   1. [Specific Camera](#connect---specific-camera)
   2. [File Camera](#connect---file-camera)
3. [Configure](#configure)
   1. [Capture Assistant](#capture-assistant)
   2. [Manual Configuration](#manual-configuration)
      1. [Single](#single-acquisition)
      2. [Multi](#multi-acquisition-hdr)
      3. [2D](#2d-settings)
   3. [From File](#from-file)
4. [Capture](#capture)
    1. [2D](#capture-2d)
5. [Save](#save)
    1. [2D](#save-2d)

### Prerequisites

You should have installed Zivid SDK and cloned C# samples. For more details see [Instructions][installation-instructions-url].

## Initialize

Before calling any of the APIs in the Zivid SDK, we have to start up the Zivid Application. This is done through a simple instantiation of the application ([go to source][start_app-url]).
```csharp
var zivid = new Zivid.NET.Application();
```

## Connect

Now we can connect to the camera ([go to source][connect-url]).
```csharp
var camera = zivid.ConnectCamera();
```

### Connect - Specific Camera

Sometime multiple cameras are connected to the same computer. It might then be necessary to work with a specific camera in the code. This can be done by providing the serial number of the wanted camera.
```csharp
var camera = zivid.ConnectCamera(new Zivid.NET.CameraInfo.SerialNumber("2020C0DE"));
```

---
**Note** 

The serial number of your camera is shown in Zivid Studio.

---

You may also list all cameras connected to the computer, and view their serial numbers through
```csharp
foreach (var cam in zivid.Cameras)
{
    Console.WriteLine("Available camera: " + cam.Info.SerialNumber);
}
```

### Connect - File Camera

You may want to experiment with the SDK, without access to a physical camera. Minor changes are required to keep the sample working ([go to source][filecamera-url]).
```csharp
var cameraFile = Zivid.NET.Environment.Paths.DataPath + "/FileCameraZividOne.zfc";
var camera = zivid.CreateFileCamera(cameraFile);
```

---
**Note**

The quality of the point cloud you get from *FileCameraZividOne.zfc* is not representative of the Zivid One+.

---

## Configure

As with all cameras there are settings that can be configured. These may be set manually, or you use our Capture Assistant.

### Capture Assistant

It can be difficult to know what settings to configure. Luckily we have the Capture Assistant. This is available in the Zivid SDK to help configure camera settings ([go to source][captureassistant-url]).
```csharp
var suggestSettingsParameters = new Zivid.NET.CaptureAssistant.SuggestSettingsParameters
{
    AmbientLightFrequency =
        Zivid.NET.CaptureAssistant.SuggestSettingsParameters.AmbientLightFrequencyOption.none,
    MaxCaptureTime = Duration.FromMilliseconds(1200)
};
var settings = Zivid.NET.CaptureAssistant.Assistant.SuggestSettings(camera, suggestSettingsParameters);
```

There are only two parameters to configure with Capture Assistant:

1. **Maximum Capture Time** in number of milliseconds.
    1. Minimum capture time is 200 ms. This allows only one acquisition.
    2. The algorithm will combine multiple acquisitions if the budget allows.
    3. The algorithm will attempt to cover as much of the dynamic range in the scene as possible.
    4. A maximum capture time of more than 1 second will get good coverage in most scenarios.
2. **Ambient light compensation**
    1. May restrict capture assistant to exposure periods that are multiples of the ambient light period.
    2. 60Hz is found in (amongst others) Japan, Americas, Taiwan, South Korea and Philippines.
    3. 50Hz is common in the rest of the world.

### Manual configuration

We may choose to configure settings manually. For more information about what each settings does, please see [Zivid One Camera Settings][kb-camera_settings-url].

#### Single Acquisition

We can create settings for a single capture ([go to source][settings-url]).
```csharp
var settings = new Zivid.NET.Settings
{
    Acquisitions = { new Zivid.NET.Settings.Acquisition{
                                           Aperture = 5.66,
                                           ExposureTime = Duration.FromMicroseconds(8333)
                                            } },
    Processing = { Filters = { Outlier = { Removal = { Enabled = true, Threshold = 5.0 } } } }
};
```

#### Multi Acquisition HDR

We may also create settings to be used in an HDR capture ([go to source][settings-hdr-url]).
```csharp
var settings = new Zivid.NET.Settings();
foreach (var aperture in new double[] { 11.31, 5.66, 2.83 })
{
    var acquisitionSettings = new Zivid.NET.Settings.Acquisition { Aperture = aperture };
    settings.Acquisitions.Add(acquisitionSettings);
}
```
For complete settings configuration see [CaptureHDRCompleteSettings][settings-complete-hdr-url].

#### 2D Settings

It is possible to only capture a 2D image. This is faster than a 3D capture. 2D settings are configured as follows ([go to source][settings2d-url]).
```csharp
var settings2D = new Zivid.NET.Settings2D
{
    Acquisitions = { new Zivid.NET.Settings2D.Acquisition{
        Aperture = 2.83,
        ExposureTime = Duration.FromMicroseconds(10000),
        Gain = 1.0,
        Brightness = 1.0 } },
    Processing = { Color = { Balance = { Red = 1.0, Blue = 1.0, Green = 1.0 }, Gamma = 1.0 } }
};
```

### From File

Zivid Studio can store the current settings to .yml files. These can be read and applied in the API. You may find it easier to modify the settings in these (human-readable) yaml-files in your preferred editor  ([go to source][settingsFromFile-url]).
```csharp
var settings = new Zivid.NET.Settings("Settings.yml");
```

## Capture

Now we can capture a 3D image. Whether there is a single acquisition or multiple acquisitions (HDR) is given by the number of `acquisitions` in `settings` ([go to source][capture-url]).
```csharp
var frame = camera.Capture(settings);
```

### Capture 2D

If we only want to capture a 2D image, which is faster than 3D, we can do so via the 2D API ([go to source][capture2d-url]).
```csharp
var frame2D = camera.Capture(settings2D);
```

## Save

We can now save our results ([go to source][save-url]).
```csharp
frame.Save("Frame.zdf");
```
The API detects which format to use. See [Point Cloud][kb-point_cloud-url] for a list of supported formats.

### Save 2D

If we captured a 2D image, we can save it ([go to source][save2d-url]).
```csharp
frame2D.ImageRGBA().Save("Image.png");
```

## Conclusion

This tutorial shows how to use the Zivid SDK to connect to, configure, capture, and save from the Zivid camera.

[//]: ### "Recommended further reading"

[installation-instructions-url]: ../../../README.md#instructions
[start_app-url]: Capture/Capture.cs#L14
[connect-url]: Capture/Capture.cs#L17
[settings-url]: Capture/Capture.cs#L20-L25
[capture-url]: Capture/Capture.cs#L28
[save-url]: Capture/Capture.cs#L30-L32
[captureassistant-url]: CaptureAssistant/CaptureAssistant.cs#L19-L27
[settings2d-url]: Capture2D/Capture2D.cs#L21-L26
[capture2d-url]: Capture2D/Capture2D.cs#L29
[save2d-url]: Capture2D/Capture2D.cs#L62-L64
[filecamera-url]: CaptureFromFile/CaptureFromFile.cs#L17-L18
[settings-hdr-url]: CaptureHDR/CaptureHDR.cs#L21-L27
[settingsFromFile-url]: CaptureWithSettingsFromYML/CaptureWithSettingsFromYML.cs#L21-L23
[settings-complete-hdr-url]: CaptureHDRCompleteSettings/CaptureHDRCompleteSettings.cs#L24-L60
[kb-camera_settings-url]: https://zivid.atlassian.net/wiki/spaces/ZividKB/pages/450265335
[kb-point_cloud-url]: https://zivid.atlassian.net/wiki/spaces/ZividKB/pages/520061383
