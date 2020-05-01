## Introduction

This tutorial shows how few API calls are required to capture a point cloud with Zivid SDK.

1. [Connect](#connect)
2. [Capture](#capture)
3. [Save](#save)

### Prerequisites

You should have installed Zivid SDK and C++ samples. For more details see [Instructions][installation-instructions-url].

Before calling any of the APIs in the Zivid SDK, we have to start up the Zivid Application. This is done through a simple instantiation of the application ([go to source][start_app-url]).
```csharp
var zivid = new Zivid.NET.Application();
```

## Connect

First we have to connect to the camera ([go to source][connect-url]).
```csharp
var camera = zivid.ConnectCamera();
```

## Capture

Now we can capture a frame. The default capture is a single 3D point cloud ([go to source][capture-url]).
```csharp
var frame = camera.Capture();
```

## Save

We can now save our results. By default the 3D point cloud is saved in Zivid format `.zdf` ([go to source][save-url]).
```csharp
frame.Save("Result.zdf");
```

## Conclusion

This tutorial showed how few API calls are required to capture a point cloud with Zivid SDK.

### Recommended further reading

[The complete Capture Tutorial](CaptureTutorial.md)

[installation-instructions-url]: ../../../README.md#instructions
[start_app-url]: Capture/Capture.cs#L10
[connect-url]: Capture/Capture.cs#L15
[capture-url]: Capture/Capture.cs#L27
[save-url]: Capture/Capture.cs#L30
