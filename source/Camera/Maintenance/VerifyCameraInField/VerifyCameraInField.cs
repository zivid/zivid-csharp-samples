/*
Check the dimension trueness of a Zivid camera.

This example shows how to verify the local dimension trueness of a camera.
If the trueness is much worse than expected, the camera may have been damaged by
shock in shipping or handling. If so, look at the CorrectCameraInField sample.

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

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            // For convenience, print the timestamp of the latest correction
            if (Zivid.NET.Calibration.Calibrator.HasCameraCorrection(camera))
            {
                var timestamp = Zivid.NET.Calibration.Calibrator.CameraCorrectionTimestamp(camera);
                Console.WriteLine("Timestamp of curent camera correction: " + timestamp.ToString());
            }
            else
            {
                Console.WriteLine("This camera has no infield correction written to it.");
            }

            // Gather data
            Console.WriteLine("Capturing calibration board");
            var detectionResult = Zivid.NET.Calibration.Detector.DetectCalibrationBoard(camera);
            if (!detectionResult.Valid())
            {
                throw new Exception("Detection failed! Feedback: " + detectionResult.StatusDescription());
            }

            // Prepare data and check that it is appropriate for infield verification
            var input = new Zivid.NET.Calibration.InfieldCorrectionInput(detectionResult);
            if (!input.Valid)
            {
                throw new Exception("Capture not valid for infield verification! Feedback: " + input.StatusDescription());
            }

            // Show results
            Console.WriteLine("Successful measurement at " + detectionResult.Centroid().ToString());
            var verification = Zivid.NET.Calibration.Calibrator.VerifyCamera(input);
            Console.WriteLine("Estimated dimenstion trueness error at measured position: " + (verification.LocalDimensionTrueness * 100).ToString("0.00") + "%");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString());
            return 1;
        }
        return 0;
    }
}
