/*
Check the dimension trueness of a Zivid camera from a ZDF file.

This example shows how to verify the local dimension trueness of a camera from a ZDF file. If the trueness is much worse
than expected, the camera may have been damaged by shock in shipping or handling. If so, look at the CorrectCameraInField
sample.

Why is verifying camera accuracy from a ZDF file useful?

Let us assume that your system is in production. You want to verify the accuracy of the camera while the system is running.
At the same time, you want to minimize the time the robot and the camera are used for anything else than their main task,
e.g., bin picking. Instead of running a full infield verification live, which consists of capturing, detecting, and
estimating accuracy, you can instead only capture and save results to ZDF files on disk. As the robot and the camera go
back to their main tasks, you can load the ZDF files and verify the accuracy offline, using a different PC than the one
used in production. In addition, you can send these ZDF files to Zivid Customer Success for investigation.

Note: This example uses experimental SDK features, which may be modified, moved, or deleted in the future without notice.
*/

using System;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            string fileCamera = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/BinWithCalibrationBoard.zfc";
            Console.WriteLine("Creating virtual camera using file: " + fileCamera);
            var camera = zivid.CreateFileCamera(fileCamera);

            // Calibration board can be captured live, while the system is in production, and saved to ZDF file, for later use in
            // offline infield verification

            Console.WriteLine("Capturing calibration board");

            var dataFile = "FrameWithCalibrationBoard.zdf";

            using (var frame = Zivid.NET.Calibration.Detector.CaptureCalibrationBoard(camera))
            {
                Console.WriteLine("Saving frame to file: " + dataFile + ", for later use in offline infield verification");
                frame.Save(dataFile);
            }

            // The ZDF captured with captureCalibrationBoard(camera) that contains the calibration board can be loaded for
            // offline infield verification

            Console.WriteLine("Reading ZDF frame from file: " + dataFile + ", for offline infield verification");

            using (var loadedFrame = new Zivid.NET.Frame(dataFile))
            {

                Console.WriteLine("Detecting calibration board");
                var detectionResult = Zivid.NET.Calibration.Detector.DetectCalibrationBoard(loadedFrame);

                var input = new Zivid.NET.Calibration.InfieldCorrectionInput(detectionResult);
                if (!input.Valid)
                {
                    throw new Exception("Capture not valid for infield verification! Feedback: " + input.StatusDescription());
                }

                Console.WriteLine("Successful measurement at " + detectionResult.Centroid().ToString());
                var verification = Zivid.NET.Calibration.Calibrator.VerifyCamera(input);
                Console.WriteLine("Estimated dimension trueness error at measured position: " + (verification.LocalDimensionTrueness * 100).ToString("0.00") + "%");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
