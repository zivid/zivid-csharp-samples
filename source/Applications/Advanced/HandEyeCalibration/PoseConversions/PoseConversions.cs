/*
This example shows how to convert to/from transformation matrix (rotation matrix + translation vector).

Zivid primarily operate with a (4x4) transformation matrix. This example implements functions to convert to and from:
AxisAngle, Rotation Vector, Roll-Pitch-Yaw, Quaternion.

The convenience functions from this example can be reused in applicable applications. The YAML files for this sample
can be found under the main instructions for Zivid samples.
*/

using System;
using System.IO;
using YamlDotNet.RepresentationModel;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

class Program
{
    static void Main()
    {
        try
        {
            printHeader("This example shows conversions to/from Transformation Matrix");

            var transformationMatrixFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                               + "/Zivid/RobotTransform.yaml";
            var transformationMatrix = readTransform(transformationMatrixFile);
            Console.WriteLine(matrixToString(transformationMatrix));

            // Extract rotation matrix and translation vector
            var rotationMatrix = transformationMatrix.SubMatrix(0, 3, 0, 3);
            var translationVector = transformationMatrix.SubMatrix(0, 3, 3, 1);
            Console.WriteLine("RotationMatrix:\n" + matrixToString(rotationMatrix));
            Console.WriteLine("TranslationVector:\n" + matrixToString(translationVector.Transpose()));

            /*
             * Convert from Rotation Matrix (Zivid) to other representations of orientation (Robot)
             */
            printHeader("Convert from Zivid (Rotation Matrix) to Robot");
            var axisAngle = rotationMatrixToAngleAxis(rotationMatrix);
            Console.WriteLine("AxisAngle:\n" + matrixToString(axisAngle.Item1.Transpose()) + ", " + String.Format(" {0:G4} ", axisAngle.Item2));
            var rotationVector = axisAngle.Item1 * axisAngle.Item2;
            Console.WriteLine("Rotation Vector:\n" + matrixToString(rotationVector.Transpose()));
            var quaternion = rotationMatrixToQuaternion(rotationMatrix);
            Console.WriteLine("Quaternion:\n" + matrixToString(quaternion.Transpose()));
            var rpyList = rotationMatrixToRollPitchYawList(rotationMatrix);

            /*
             * Convert to Rotation Matrix (Zivid) from other representations of orientation (Robot)
             */
            printHeader("Convert from Robot to Zivid (Rotation matrix)");
            var rotationMatrixFromAxisAngle = axisAngleToRotationMatrix(axisAngle);
            Console.WriteLine("Rotation Matrix from Axis Angle:\n" + matrixToString(rotationMatrixFromAxisAngle));
            var rotationMatrixFromRotationVector = rotationVectorToRotationMatrix(rotationVector);
            Console.WriteLine("Rotation Matrix from Rotation Vector:\n" + matrixToString(rotationMatrixFromRotationVector));
            var rotationMatrixFromQuaternion = quaternionToRotationMatrix(quaternion);
            Console.WriteLine("Rotation Matrix from Quaternion:\n" + matrixToString(rotationMatrixFromQuaternion));
            rollPitchYawListToRotationMatrix(rpyList);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.Message);
            Environment.ExitCode = 1;
        }
    }

    enum RotationConvention
    {
        zyxIntrinsic,
        xyzExtrinsic,
        xyzIntrinsic,
        zyxExtrinsic
    };
    const int nrOfRotationConventions = 4;
    static string conventionToString(RotationConvention convention)
    {
        switch (convention)
        {
            case RotationConvention.xyzIntrinsic: return "XYZ_Intrinsic";
            case RotationConvention.xyzExtrinsic: return "XYZ_Extrinsic";
            case RotationConvention.zyxIntrinsic: return "ZYX_Intrinsic";
            case RotationConvention.zyxExtrinsic: return "ZYX_Extrinsic";
        }
        throw new ArgumentException("Invalid RotationConvention");
    }

    static Matrix<double> skew(Matrix<double> vector)
    // Assumes vector to be [3x1]
    {
        return CreateMatrix.DenseOfArray<double>(new double[,]
        {
            { 0, -vector[2,0], vector[1,0] },
            { vector[2,0], 0, -vector[0,0] },
            { -vector[1,0], vector[0,0], 0 }
        });
    }

    static Matrix<double> readTransform(string transformFile)
    {
        using (var reader = new StreamReader(transformFile))
        {
            // OpenCV writes yaml 1.0 with legacy first line
            // YamlDotNet does not support this legacy first line
            // so to work around we modify the first line if it is legacy
            var first_line = reader.ReadLine();
            if (first_line.Contains("%YAML:1.0")) {
                first_line = first_line.Replace(":", " ");
            }
            var new_reader = new StringReader(first_line + "\n" + reader.ReadToEnd());

            var yaml = new YamlStream();
            yaml.Load(new_reader);

            var poseStateNode = (YamlMappingNode)yaml.Documents[0].RootNode;
            var poseStateData = poseStateNode["PoseState"]["data"].ToString();
            var matrixAsString = poseStateData.Trim('[', ']').Split(',');

            int rows = int.Parse(poseStateNode["PoseState"]["rows"].ToString());
            int cols = int.Parse(poseStateNode["PoseState"]["cols"].ToString());

            double[,] transformationArray = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    transformationArray[i, j] = double.Parse(matrixAsString[i * rows + j]);
                }
            }
            return CreateMatrix.DenseOfArray(transformationArray);
        }
    }

    static Tuple<Matrix<double>, double> rotationMatrixToAngleAxis(Matrix<double> rotationMatrix)
    {
        var A = (rotationMatrix - rotationMatrix.Transpose()) / 2;
        Matrix<double> rho = CreateMatrix.DenseOfArray<double>(new double[,] { { A[2, 1] }, { A[0, 2] }, { A[1, 0] } });
        var s = rho.Column(0, 0, 3).L2Norm();
        var c = (rotationMatrix.Trace() - 1) / 2;
        Matrix<double> u = rho / s;
        var theta = Math.Atan2(s, c);

        return Tuple.Create(u, theta);
    }

    static Matrix<double> axisAngleToRotationMatrix(Tuple<Matrix<double>, double> axisAngle)
    {
        //See rodriques formula
        var u = axisAngle.Item1;
        var firstTerm = CreateMatrix.DenseIdentity<double>(3) * Math.Cos(axisAngle.Item2);
        var secondTerm = (u.Multiply(u.Transpose())) * (1 - Math.Cos(axisAngle.Item2));
        var thirdTerm = skew(u);
        return firstTerm + secondTerm + thirdTerm * Math.Sin(axisAngle.Item2);
    }

    static Matrix<double> rotationVectorToRotationMatrix(Matrix<double> rotationVector)
    {
        double theta = rotationVector.L2Norm();
        return axisAngleToRotationMatrix(Tuple.Create(rotationVector / theta, theta));
    }

    static Matrix<double> rotationMatrixToQuaternion(Matrix<double> rotationMatrix)
    {
        var qw = (Math.Sqrt(1 + rotationMatrix[0, 0] + rotationMatrix[1, 1] + rotationMatrix[2, 2])) / 2;
        Matrix<double> quaternion = CreateMatrix.DenseOfArray<double>(new double[,]
        {
            { (rotationMatrix[2,1] - rotationMatrix[1,2])/(4*qw) },
            { (rotationMatrix[0,2] - rotationMatrix[2,0])/(4*qw)  },
            { (rotationMatrix[1,0] - rotationMatrix[0,1])/(4*qw) },
            { qw } // REAL PART
        });

        return quaternion;
    }

    static Matrix<double> quaternionToRotationMatrix(Matrix<double> quaternion)
    {
        // Normalize quaternion
        var nQ = quaternion / quaternion.L2Norm();
        var firstTerm = CreateMatrix.DenseIdentity<double>(3);
        var secondTerm = 2 * skew(nQ.SubMatrix(0, 3, 0, 1)) * skew(nQ.SubMatrix(0, 3, 0, 1));
        var thirdTerm = 2 * nQ[3, 0] * skew(nQ.SubMatrix(0, 3, 0, 1));

        return firstTerm + secondTerm + thirdTerm;
    }

    static Matrix createRotationMatrix(string axis, double angle)
    {
        var matrix = DenseMatrix.CreateIdentity(3);
        var cosAngle = Math.Cos(angle);
        var sinAngle = Math.Sin(angle);
        switch (axis)
        {
            case ("x"):
                matrix[1, 1] = cosAngle;
                matrix[2, 2] = cosAngle;
                matrix[1, 2] = -sinAngle;
                matrix[2, 1] = sinAngle;
                break;
            case ("y"):
                matrix[0, 0] = cosAngle;
                matrix[2, 2] = cosAngle;
                matrix[0, 2] = sinAngle;
                matrix[2, 0] = -sinAngle;
                break;
            case ("z"):
                matrix[0, 0] = cosAngle;
                matrix[1, 1] = cosAngle;
                matrix[0, 1] = -sinAngle;
                matrix[1, 0] = sinAngle;
                break;
        }
        return matrix;
    }

    static Matrix createXRotation(double angle)
    {
        return createRotationMatrix("x", angle);
    }

    static Matrix createYRotation(double angle)
    {
        return createRotationMatrix("y", angle);
    }

    static Matrix createZRotation(double angle)
    {
        return createRotationMatrix("z", angle);
    }

    static Matrix<double> rotationMatrixToRollPitchYaw(Matrix<double> rotationMatrix, RotationConvention convention)
    {
        double roll = -1; // here defined as angle around x axis
        double pitch = -1; // here defined as angle around y axis
        double yaw = -1; // here defined as angle around z axis

        switch (convention)
        {
            case RotationConvention.xyzIntrinsic:
            case RotationConvention.zyxExtrinsic:
                pitch = -Math.Asin(rotationMatrix[0, 2]) + Math.PI;
                roll = -Math.Atan2(rotationMatrix[1, 2] / Math.Cos(pitch),
                    rotationMatrix[2, 2] / Math.Cos(pitch));
                yaw = -Math.Atan2(rotationMatrix[0, 1] / Math.Cos(pitch),
                    rotationMatrix[0, 0] / Math.Cos(pitch));

                break;
            case RotationConvention.zyxIntrinsic:
            case RotationConvention.xyzExtrinsic:
                pitch = -Math.Asin(rotationMatrix[2, 0]);
                roll = Math.Atan2(rotationMatrix[2, 1] / Math.Cos(pitch),
                    rotationMatrix[2, 2] / Math.Cos(pitch));
                yaw = Math.Atan2(rotationMatrix[1, 0] / Math.Cos(pitch),
                    rotationMatrix[0, 0] / Math.Cos(pitch));
                break;
        }
        switch (convention)
        {
            case RotationConvention.xyzIntrinsic:
            case RotationConvention.xyzExtrinsic:
                return CreateMatrix.DenseOfArray<double>(new double[,]
                    {
                        { roll }, { pitch }, { yaw }
                    });
            case RotationConvention.zyxIntrinsic:
            case RotationConvention.zyxExtrinsic:
                return CreateMatrix.DenseOfArray<double>(new double[,]
                    {
                        { yaw }, { pitch }, { roll }
                    });
        }
        throw new ArgumentException("Invalid RotationConvention");
    }

    static Matrix<double>[] rotationMatrixToRollPitchYawList(Matrix<double> rotationMatrix)
    {
        Matrix<double>[] rpyList = new Matrix<double>[4];
        for (int i = 0; i < nrOfRotationConventions; i++)
        {
            RotationConvention convention = (RotationConvention)i;
            Console.WriteLine("Roll-Pitch-Yaw angles (" + conventionToString(convention) + "):");
            rpyList[i] = rotationMatrixToRollPitchYaw(rotationMatrix, convention);
            Console.WriteLine(matrixToString(rpyList[i].Transpose()));
        }
        return rpyList;
    }

    static Matrix<double> rollPitchYawToRotationMatrix(Matrix<double> rollPitchYaw, RotationConvention convention)
    {
        switch (convention)
        {
            case RotationConvention.xyzIntrinsic:
                return createXRotation(rollPitchYaw[0, 0]) * createYRotation(rollPitchYaw[1, 0]) * createZRotation(rollPitchYaw[2, 0]);
            case RotationConvention.zyxIntrinsic:
                return createZRotation(rollPitchYaw[0, 0]) * createYRotation(rollPitchYaw[1, 0]) * createXRotation(rollPitchYaw[2, 0]);
            case RotationConvention.zyxExtrinsic:
                return createXRotation(rollPitchYaw[2, 0]) * createYRotation(rollPitchYaw[1, 0]) * createZRotation(rollPitchYaw[0, 0]);
            case RotationConvention.xyzExtrinsic:
                return createZRotation(rollPitchYaw[2, 0]) * createYRotation(rollPitchYaw[1, 0]) * createXRotation(rollPitchYaw[0, 0]);
        }
        throw new ArgumentException("Invalid RotationConvention");
    }

    static void rollPitchYawListToRotationMatrix(Matrix<double>[] rpyList)
    {
        for (int i = 0; i < nrOfRotationConventions; i++)
        {
            RotationConvention convention = (RotationConvention)i;
            Console.WriteLine("Rotation Matrix from Roll-Pitch-Yaw angles (" + conventionToString(convention) + "):");
            Console.WriteLine(matrixToString(rollPitchYawToRotationMatrix(rpyList[i], convention)));
        }
    }

    static void printHeader(string text)
    {
        string asterixLine = "****************************************************************";
        Console.WriteLine(asterixLine + "\n* " + text + "\n" + asterixLine);
    }

    static string matrixToString(Matrix<double> matrix)
    {
        string matrixString = "";
        if (matrix.RowCount != 1) {
            matrixString = "[";
        }
        for (var i = 0; i < matrix.RowCount; i++)
        {
            matrixString += "[";// + matrix.SubMatrix(i, 1, 0, 3).ToMatrixString() + "]";
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                matrixString += String.Format(" {0,9:G4} ", matrix[i, j]);
            }
            matrixString += "]\n ";
        }
        matrixString = matrixString.TrimEnd(' ');
        matrixString = matrixString.TrimEnd('\n');
        if (matrix.RowCount != 1)
        {
            matrixString += "]";
        }
        return matrixString;
    }
}
