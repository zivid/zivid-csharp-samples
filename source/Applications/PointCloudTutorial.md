# Point Cloud Tutorial

Note\! This tutorial has been generated for use on Github. For original
tutorial see:
[PointCloudTutorial](https://support.zivid.com/latest/academy/applications/point-cloud-tutorial.html)



---

*Contents:*
[**Introduction**](#Introduction) |
[**Frame**](#Frame) |
[**Point**](#Point-Cloud) |
[**Downsample**](#Downsample) |
[**Normals**](#Normals) |
[**Visualize**](#Visualize) |
[**Conclusion**](#Conclusion) |
[**Version**](#Version-History)

---



## Introduction

This tutorial describes how to use Zivid SDK to work with [Point
Cloud](https://support.zivid.com/latest//reference-articles/point-cloud-structure-and-output-formats.html)
data.

-----

Tip:

> If you prefer watching a video, our webinar [Getting your point cloud
> ready for your
> application](https://www.zivid.com/webinars-page?wchannelid=ffpqbqc7sg&wmediaid=h66zph71vo)
> covers the Point Cloud Tutorial.

**Prerequisites**

  - Install [Zivid
    Software](https://support.zivid.com/latest//getting-started/software-installation.html).
  - For Python: install
    [zivid-python](https://github.com/zivid/zivid-python#installation)

## Frame

The `Zivid.NET.Frame` contains the point cloud and color image (stored
on compute device memory) and the capture and camera information.

### Capture

When you capture with Zivid, you get a frame in return.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Basic/Capture/Capture.cs#L26))

``` sourceCode cs
using (var frame = camera.Capture2D3D(settings))
```

Check
[CaptureTutorial](https://github.com/zivid/zivid-csharp-samples/tree/master/source/Camera/Basic/CaptureTutorial.md)
for detailed instructions on how to capture.

### Load

The frame can also be loaded from a ZDF file.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/FileFormats/ReadIterateZDF/ReadIterateZDF.cs#L15-L18))

``` sourceCode cs
var dataFile =
	Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/Zivid3D.zdf";
Console.WriteLine("Reading ZDF frame from file: " + dataFile);
var frame = new Zivid.NET.Frame(dataFile);
```

## Point Cloud

### Get handle from Frame

You can now get a handle to the point cloud data on the GPU.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/FileFormats/ReadIterateZDF/ReadIterateZDF.cs#L21))

``` sourceCode cs
var pointCloud = frame.PointCloud;
```

Point cloud contains XYZ, RGB, and SNR, laid out on a 2D grid.

For more info check out [Point Cloud
Structure](https://support.zivid.com/latest//reference-articles/point-cloud-structure-and-output-formats.html).

Getting the property `Zivid.NET.Frame.PointCloud` does not perform any
copying from GPU memory.

-----

Note:

`Zivid.NET.Camera.Capture()` method returns at some moment in time after
the camera completes capturing raw images. The handle from
`Zivid.NET.Frame.PointCloud` is available instantly. However, the actual
point cloud data becomes available only after the processing on the GPU
is finished. Any calls to data-copy methods (section below) will block
and wait for processing to finish before proceeding with the requested
copy operation.

For detailed explanation, see [Point Cloud Capture
Process](https://support.zivid.com/latest/academy/camera/point-cloud-capture-process.html).

-----

### Copy from GPU to CPU memory

You can now selectively copy data based on what is required. This is the
complete list of output data formats and how to copy them from the GPU.

| Return type                                  | Copy methods                           | Data per pixel | Total data |
| -------------------------------------------- | -------------------------------------- | -------------- | ---------- |
| `float[height,width,3]`                      | `PointCloud.CopyPointsXYZ()`           | 12 bytes       | 28 MB      |
| `float[height,width,4]`                      | `PointCloud.CopyPointsXYZW()`          | 16 bytes       | 37 MB      |
| `float[height,width,1]`                      | `PointCloud.CopyPointsZ()`             | 4 bytes        | 9 MB       |
| `byte[height,width,4]`                       | `PointCloud.CopyColorsRGBA()`          | 4 bytes        | 9 MB       |
| `float[height,width]`                        | `PointCloud.CopySNRs()`                | 4 bytes        | 9 MB       |
| `Zivid.NET.PointXYZColorRGBA[height, width]` | `PointCloud.CopyPointsXYZColorsRGBA()` | 16 bytes       | 37 MB      |
| `Zivid.NET.PointXYZColorBGRA[height, width]` | `PointCloud.CopyPointsXYZColorsBGRA()` | 16 bytes       | 37 MB      |
| `Zivid.NET.ImageRGBA`                        | `PointCloud.CopyImageRGBA()`           | 4 bytes        | 9 MB       |
| `Zivid.NET.ImageBGRA`                        | `PointCloud.CopyImageBGRA()`           | 4 bytes        | 9 MB       |
| `Zivid.NET.ImageSRGB`                        | `PointCloud.CopyImageSRGB()`           | 4 bytes        | 9 MB       |

Here is an example of how to copy data.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/FileFormats/ReadIterateZDF/ReadIterateZDF.cs#L22))

``` sourceCode cs
var pointCloudData = pointCloud.CopyPointsXYZColorsRGBA();
```

#### Memory allocation options

In terms of memory allocation, there are two ways to copy data:

  - The Zivid SDK can allocate a memory buffer and copy data to it.
  - A user can pass a pointer to a pre-allocated memory buffer, and the
    Zivid SDK will copy the data to the pre-allocated memory buffer.

-----

You may want to
[transform](https://support.zivid.com/latest//academy/applications/transform.html)
the point cloud to change its origin from the camera to the robot base
frame or, e.g., [scale the point cloud by transforming it from mm to
m](https://support.zivid.com/latest//academy/applications/transform/transform-millimeters-to-meters.html).

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/HandEyeCalibration/UtilizeHandEyeCalibration/UtilizeHandEyeCalibration.cs#L150))

``` sourceCode cs
pointCloud.Transform(transformBaseToCamera);
```

## Downsample

Sometimes you might not need a point cloud with as `high spatial
resolution (High spatial resolution means more detail and less distance
between points)` as given from the camera. You may then
[downsample](https://support.zivid.com/latest//academy/applications/downsampling.html)
the point cloud.

-----

Note:

> [Sampling
> (3D)](https://support.zivid.com/latest/reference-articles/settings/sampling.html)
> describes a hardware-based sub-/downsample method that reduces the
> resolution of the point cloud during capture while also reducing the
> acquisition and capture time.

-----

Downsampling can be done in-place, which modifies the current point
cloud.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/Downsample/Downsample.cs#L36))

``` sourceCode cs
pointCloud.Downsample(Zivid.NET.PointCloud.Downsampling.By2x2);
```

It is also possible to get the downsampled point cloud as a new point
cloud instance, which does not alter the existing point cloud.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/Downsample/Downsample.cs#L30))

``` sourceCode cs
var downsampledPointCloud = pointCloud.Downsampled(Zivid.NET.PointCloud.Downsampling.By2x2);
```

Zivid SDK supports the following downsampling rates: `by2x2`, `by3x3`,
and `by4x4`, with the possibility to perform downsampling multiple
times.

## Normals

Some applications require computing
[normals](https://support.zivid.com/latest//academy/applications/normals.html)
from the point cloud.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Camera/Advanced/CaptureHDRPrintNormals/CaptureHDRPrintNormals.cs#L34-L35))

``` sourceCode cs
Console.WriteLine("Computing normals and copying them to CPU memory");
var normals = pointCloud.CopyNormalsXYZ();
```

The Normals API computes the normal at each point in the point cloud and
copies normals from the GPU memory to the CPU memory. The result is a
matrix of normal vectors, one for each point in the input point cloud.
The size of normals is equal to the size of the input point cloud.

## Visualize

Having the frame allows you to visualize the point cloud.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Basic/Visualization/CaptureVis3D/CaptureVis3D.cs#L25-L35))

``` sourceCode cs
Console.WriteLine("Setting up visualization");
using (var visualizer = new Zivid.NET.Visualization.Visualizer())
{
	Console.WriteLine("Visualizing point cloud");
	visualizer.Show(frame);
	visualizer.ShowMaximized();
	visualizer.ResetToFit();
Console.WriteLine("Running visualizer. Blocking until window closes.");
visualizer.Run();
```

> }

You can visualize the point cloud from the point cloud object as well.

([go to
source](https://github.com/zivid/zivid-csharp-samples/tree/master//source/Applications/Advanced/Downsample/Downsample.cs#L22-L49))

``` sourceCode cs
Console.WriteLine("Getting point cloud from frame");
var pointCloud = frame.PointCloud;
Console.WriteLine("Setting up visualization");
var visualizer = new Zivid.NET.Visualization.Visualizer();

Console.WriteLine("Visualizing point cloud");
visualizer.Show(pointCloud);
visualizer.ShowMaximized();
visualizer.ResetToFit();

Console.WriteLine("Running visualizer. Blocking until window closes.");
visualizer.Run();
```

For more information, check out [Visualization
Tutorial](https://support.zivid.com/latest/academy/applications/visualization-tutorial.html),
where we cover point cloud, color image, depth map, and normals
visualization, with implementations using third party libraries.

## Conclusion

This tutorial shows how to use the Zivid SDK to extract the point cloud,
manipulate it, transform it, and visualize it.

## Version History

| SDK    | Changes                                                                                                                                                   |
| ------ | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 2.11.0 | Added support for SRGB color space.                                                                                                                       |
| 2.10.0 | [:orphan:](https://support.zivid.com/latest/academy/camera/monochrome-capture.html) introduces a faster alternative to `downsample_point_cloud_tutorial`. |
