/*
Perform Hand-Eye calibration.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zivid.NET.Calibration;
using Duration = Zivid.NET.Duration;

class Program
{
    static int Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            Console.WriteLine("Connecting to camera");
            var camera = zivid.ConnectCamera();

            var handEyeInput = readHandEyeInputs(camera);

            var calibrationResult = performCalibration(handEyeInput);

            Console.WriteLine("Zivid primarily operates with a (4x4) transformation matrix. To convert");
            Console.WriteLine("to axis-angle, rotation vector, roll-pitch-yaw, or quaternion, check out");
            Console.WriteLine("our PoseConversions sample.");

            if (calibrationResult.Valid())
            {
                Console.WriteLine("{0}\n{1}\n{2}", "Hand-Eye calibration OK", "Result: ", calibrationResult);
            }
            else
            {
                Console.WriteLine("Hand-Eye calibration FAILED");
                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.ToString());
            return 1;
        }
        return 0;
    }

    static List<HandEyeInput> readHandEyeInputs(Zivid.NET.Camera camera)
    {
        var handEyeInput = new List<HandEyeInput>();
        var currentPoseId = 0U;
        var beingInput = true;

        var calibrationObject = "";
        while (true)
        {
            Console.WriteLine("Enter calibration object you are using, m (for ArUco marker(s)) or c (for Zivid checkerboard): ");
            calibrationObject = Console.ReadLine();

            if (calibrationObject.Equals("m", StringComparison.CurrentCultureIgnoreCase) ||
                calibrationObject.Equals("c", StringComparison.CurrentCultureIgnoreCase))
            {
                break;
            }
        }


        Interaction.ExtendInputBuffer(2048);

        Console.WriteLine("Zivid primarily operates with a (4x4) transformation matrix. To convert");
        Console.WriteLine("from axis-angle, rotation vector, roll-pitch-yaw, or quaternion, check out");
        Console.WriteLine("our PoseConversions sample.");

        do
        {
            switch (Interaction.EnterCommand())
            {
                case CommandType.AddPose:
                    try
                    {
                        HandleAddPose(ref currentPoseId, ref handEyeInput, camera, calibrationObject);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0}", ex.ToString());
                        continue;
                    }
                    break;

                case CommandType.Calibrate: beingInput = false; break;

                case CommandType.Unknown: Console.WriteLine("Error: Unknown command"); break;
            }
        } while (beingInput);
        return handEyeInput;
    }

    public static void HandleAddPose(ref uint currentPoseId, ref List<HandEyeInput> handEyeInput, Zivid.NET.Camera camera, string calibrationObject)
    {
        var robotPose = Interaction.EnterRobotPose(currentPoseId);

        Console.Write("Detecting calibration object in point cloud");
        if (calibrationObject.Equals("c", StringComparison.CurrentCultureIgnoreCase))
        {
            using (var frame = Zivid.NET.Calibration.Detector.CaptureCalibrationBoard(camera))
            {
                var detectionResult = Detector.DetectCalibrationBoard(frame);

                if (detectionResult.Valid())
                {
                    Console.WriteLine("Calibration board detected");
                    handEyeInput.Add(new HandEyeInput(robotPose, detectionResult));
                    ++currentPoseId;
                }
                else
                {
                    Console.WriteLine("Failed to detect calibration board, ensure that the entire board is in the view of the camera");
                }
            }
        }
        else if (calibrationObject.Equals("m", StringComparison.CurrentCultureIgnoreCase))
        {
            using (var frame = AssistedCapture(camera))
            {
                var markerDictionary = Zivid.NET.MarkerDictionary.Aruco4x4_50;
                var markerIds = new List<int> { 1, 2, 3 };

                Console.WriteLine("Detecting arUco marker IDs " + string.Join(", ", markerIds));
                var detectionResult = Detector.DetectMarkers(frame, markerIds, markerDictionary);

                if (detectionResult.Valid())
                {
                    Console.WriteLine("ArUco marker(s) detected: " + detectionResult.DetectedMarkers().Length);
                    handEyeInput.Add(new HandEyeInput(robotPose, detectionResult));
                    ++currentPoseId;
                }
                else
                {
                    Console.WriteLine("Failed to detect any ArUco markers, ensure that at least one ArUco marker is in the view of the camera");
                }
            }
        }
    }

    static Zivid.NET.Calibration.HandEyeOutput performCalibration(List<HandEyeInput> handEyeInput)
    {
        while (true)
        {
            Console.WriteLine("Enter type of calibration, eth (for eye-to-hand) or eih (for eye-in-hand): ");
            var calibrationType = Console.ReadLine();
            if (calibrationType.Equals("eth", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("Performing eye-to-hand calibration with " + handEyeInput.Count + " dataset pairs");
                Console.WriteLine("The resulting transform is the camera pose in robot base frame");
                return Calibrator.CalibrateEyeToHand(handEyeInput);
            }
            if (calibrationType.Equals("eih", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("Performing eye-in-hand calibration with " + handEyeInput.Count + " dataset pairs");
                Console.WriteLine("The resulting transform is the camera pose in flange (end-effector) frame");
                return Calibrator.CalibrateEyeInHand(handEyeInput);
            }
            Console.WriteLine("Entered unknown method");
        }
    }
    public static Zivid.NET.Frame AssistedCapture(Zivid.NET.Camera camera)
    {
        var suggestSettingsParameters = new Zivid.NET.CaptureAssistant.SuggestSettingsParameters
        {
            AmbientLightFrequency =
                Zivid.NET.CaptureAssistant.SuggestSettingsParameters.AmbientLightFrequencyOption.none,
            MaxCaptureTime = Duration.FromMilliseconds(800)
        };
        var settings = Zivid.NET.CaptureAssistant.Assistant.SuggestSettings(camera, suggestSettingsParameters);
        return camera.Capture2D3D(settings);
    }
}

enum CommandType
{
    AddPose,
    Calibrate,
    Unknown
}

class Interaction
{
    // Console.ReadLine only supports reading 256 characters, by default. This limit is modified
    // by calling ExtendInputBuffer with the maximum length of characters to be read.
    public static void ExtendInputBuffer(int size)
    {
        Console.SetIn(new StreamReader(Console.OpenStandardInput(), Console.InputEncoding, false, size));
    }

    public static CommandType EnterCommand()
    {
        Console.Write("Enter command, p (to add robot pose) or c (to perform calibration): ");
        var command = Console.ReadLine().ToLower();

        switch (command)
        {
            case "p": return CommandType.AddPose;
            case "c": return CommandType.Calibrate;
            default: return CommandType.Unknown;
        }
    }

    public static Pose EnterRobotPose(ulong index)
    {
        var elementCount = 16;
        Console.WriteLine(
            "Enter pose with id (a line with {0} space separated values describing 4x4 row-major matrix) : {1}",
            elementCount,
            index);
        var input = Console.ReadLine();

        var elements = input.Split().Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => float.Parse(x)).ToArray();

        var robotPose = new Pose(elements); Console.WriteLine("The following pose was entered: \n{0}", robotPose);
        return robotPose;
    }
}
