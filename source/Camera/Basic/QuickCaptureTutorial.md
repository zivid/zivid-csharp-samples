## Introduction

This tutorial shows how few API calls are required to capture a point cloud with Zivid SDK.

1. [Connect](#connect)
2. [Configure](#configure)
3. [Capture](#capture)
4. [Save](#save)

### Prerequisites

You should have installed Zivid SDK and cloned C# samples. For more details see [Instructions][installation-instructions-url].

Before calling any of the APIs in the Zivid SDK, we have to start up the Zivid Application. This is done through a simple instantiation of the application ([go to source][start_app-url]).
```csharp
var zivid = new Zivid.NET.Application();
```

## Connect

First we have to connect to the camera ([go to source][connect-url]).
```csharp
var camera = zivid.ConnectCamera();
```

## Configure

Then we have to create settings ([go to source][settings-url]).
```csharp
var settings = new Zivid.NET.Settings { Acquisitions = { new Zivid.NET.Settings.Acquisition { } } };
```

## Capture

Now we can capture a frame. The default capture is based on a single acquisition ([go to source][capture-url]).
```csharp
var frame = camera.Capture(settings);
```

## Save

We can now save our results. By default the 3D point cloud is saved in Zivid format `.zdf` ([go to source][save-url]).
```csharp
frame.Save("Frame.zdf");
```

## Conclusion

This tutorial showed how few API calls are required to capture a point cloud with Zivid SDK.

### Recommended further reading

[The complete Capture Tutorial](CaptureTutorial.md)

[installation-instructions-url]: ../../../README.md#instructions
[start_app-url]: Capture/Capture.cs#L14
[connect-url]: Capture/Capture.cs#L17
[settings-url]: Capture/Capture.cs#L20-L25
[capture-url]: Capture/Capture.cs#L28
[save-url]: Capture/Capture.cs#L30-L32
