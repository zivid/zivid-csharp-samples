// DOCTAG-START-ALL-1
/*
Convert to/from Transformation Matrix (Rotation Matrix + Translation Vector)

Zivid primarily operate with a (4x4) transformation matrix. This example implements functions to convert to and from:
AxisAngle, Rotation Vector, Roll-Pitch-Yaw, Quaternion.

The convenience functions from this example can be reused in applicable applications. The YAML files for this sample
can be found under the main instructions for Zivid samples.
*/

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;

class Program
{
    static void Main()
    {
        try
        {
            var zivid = new Zivid.NET.Application();

            PrintHeader("This example shows conversions to/from Transformation Matrix");

            var transformationMatrixZivid = new Zivid.NET.Matrix4x4(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Zivid/RobotTransform.yaml");
            var transformationMatrix = ZividToMathDotNet(transformationMatrixZivid);
            Console.WriteLine(MatrixToString(transformationMatrix));

            // Extract Rotation Matrix and Translation Vector from Transformation Matrix
            var rotationMatrix = transformationMatrix.SubMatrix(0, 3, 0, 3);
            var translationVector = transformationMatrix.SubMatrix(0, 3, 3, 1);
            Console.WriteLine("RotationMatrix:\n" + MatrixToString(rotationMatrix));
            Console.WriteLine("TranslationVector:\n" + MatrixToString(translationVector.Transpose()));

            /*
             * Convert from Rotation Matrix (Zivid) to other representations of orientation (Robot)
             */
            PrintHeader("Convert from Zivid (Rotation Matrix) to Robot");
            var axisAngle = RotationMatrixToAxisAngle(rotationMatrix);
            Console.WriteLine("AxisAngle:\n" + MatrixToString(axisAngle.Axis.Transpose()) + ", " + String.Format(" {0:G4} ", axisAngle.Angle));
            var rotationVector = axisAngle.Axis * axisAngle.Angle;
            Console.WriteLine("Rotation Vector:\n" + MatrixToString(rotationVector.Transpose()));
            var quaternion = RotationMatrixToQuaternion(rotationMatrix);
            Console.WriteLine("Quaternion:\n" + MatrixToString(quaternion.Transpose()));
            var rpyList = RotationMatrixToRollPitchYawList(rotationMatrix);

            /*
             * Convert to Rotation Matrix (Zivid) from other representations of orientation (Robot)
             */
            PrintHeader("Convert from Robot to Zivid (Rotation matrix)");
            var rotationMatrixFromAxisAngle = AxisAngleToRotationMatrix(axisAngle);
            Console.WriteLine("Rotation Matrix from Axis Angle:\n" + MatrixToString(rotationMatrixFromAxisAngle));
            var rotationMatrixFromRotationVector = RotationVectorToRotationMatrix(rotationVector);
            Console.WriteLine("Rotation Matrix from Rotation Vector:\n" + MatrixToString(rotationMatrixFromRotationVector));
            var rotationMatrixFromQuaternion = QuaternionToRotationMatrix(quaternion);
            Console.WriteLine("Rotation Matrix from Quaternion:\n" + MatrixToString(rotationMatrixFromQuaternion));
            RollPitchYawListToRotationMatrix(rpyList);

            // Combine Rotation Matrix with Translation Vector to form Transformation Matrix
            var transformationMatrixFromQuaternion = Matrix<double>.Build.Dense(4, 4);
            transformationMatrixFromQuaternion.SetSubMatrix(0, 3, 0, 3, rotationMatrixFromQuaternion);
            transformationMatrixFromQuaternion.SetSubMatrix(0, 3, 3, 1, translationVector);
            var transformationMatrixFromQuaternionZivid = MathDotNetToZivid(transformationMatrixFromQuaternion);
            transformationMatrixFromQuaternionZivid.Save("RobotTransformOut.yaml");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex.ToString());
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

    public class AxisAngle
    {
        public AxisAngle(Matrix<double> axis, double angle)
        {
            this.Axis = axis;
            this.Angle = angle;
        }

        public Matrix<double> Axis { get; set; }

        public double Angle { get; set; }
    }

    static Matrix<double> Skew(Matrix<double> vector)
    // Assumes vector to be [3x1]
    {
        return CreateMatrix.DenseOfArray<double>(new double[,]
        {
            { 0, -vector[2,0], vector[1,0] },
            { vector[2,0], 0, -vector[0,0] },
            { -vector[1,0], vector[0,0], 0 }
        });
    }

    static AxisAngle RotationMatrixToAxisAngle(Matrix<double> rotationMatrix)
    {
        // See Rodrigues' formula or Skew-symmetric method
        var skewSymmetricMatrix = (rotationMatrix - rotationMatrix.Transpose()) / 2;
        Matrix<double> skewElements = CreateMatrix.DenseOfArray<double>(new double[,] { { skewSymmetricMatrix[2, 1] }, { skewSymmetricMatrix[0, 2] }, { skewSymmetricMatrix[1, 0] } });
        var skewNorm = skewElements.Column(0, 0, 3).L2Norm();
        Matrix<double> u = skewElements / skewNorm;
        var theta = Math.Atan2(skewNorm, (rotationMatrix.Trace() - 1) / 2);

        return new AxisAngle(u, theta);
    }

    static Matrix<double> AxisAngleToRotationMatrix(AxisAngle axisAngle)
    {
        //See Rodrigues' formula or Skew-symmetric method
        var u = axisAngle.Axis;
        var firstTerm = CreateMatrix.DenseIdentity<double>(3) * Math.Cos(axisAngle.Angle);
        var secondTerm = u.Multiply(u.Transpose()) * (1 - Math.Cos(axisAngle.Angle));
        var thirdTerm = Skew(u) * Math.Sin(axisAngle.Angle);
        return firstTerm + secondTerm + thirdTerm;
    }

    static Matrix<double> RotationVectorToRotationMatrix(Matrix<double> rotationVector)
    {
        double theta = rotationVector.L2Norm();
        return AxisAngleToRotationMatrix(new AxisAngle(rotationVector / theta, theta));
    }

    static Matrix<double> RotationMatrixToQuaternion(Matrix<double> rotationMatrix)
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

    static Matrix<double> QuaternionToRotationMatrix(Matrix<double> quaternion)
    {
        // Normalize quaternion
        var nQ = quaternion / quaternion.L2Norm();
        var firstTerm = CreateMatrix.DenseIdentity<double>(3);
        var secondTerm = 2 * Skew(nQ.SubMatrix(0, 3, 0, 1)) * Skew(nQ.SubMatrix(0, 3, 0, 1));
        var thirdTerm = 2 * nQ[3, 0] * Skew(nQ.SubMatrix(0, 3, 0, 1));

        return firstTerm + secondTerm + thirdTerm;
    }

    static Matrix CreateRotationMatrix(string axis, double angle)
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
            default: throw new Exception("Wrong axis; options are x, y, z.");
        }
        return matrix;
    }

    static Matrix CreateXRotation(double angle)
    {
        return CreateRotationMatrix("x", angle);
    }

    static Matrix CreateYRotation(double angle)
    {
        return CreateRotationMatrix("y", angle);
    }

    static Matrix CreateZRotation(double angle)
    {
        return CreateRotationMatrix("z", angle);
    }

    static Vector<double> RotationMatrixToRollPitchYaw(Matrix<double> rotationMatrix, RotationConvention convention)
    {
        double roll = -1;
        double pitch = -1;
        double yaw = -1;

        switch (convention)
        {
            case RotationConvention.zyxExtrinsic:
            case RotationConvention.xyzIntrinsic:
                if (rotationMatrix[0, 2] < 1)
                {
                    if (rotationMatrix[0, 2] > -1)
                    {
                        roll = Math.Atan2(-rotationMatrix[1, 2], rotationMatrix[2, 2]);
                        pitch = Math.Asin(rotationMatrix[0, 2]);
                        yaw = Math.Atan2(-rotationMatrix[0, 1], rotationMatrix[0, 0]);
                    }
                    else // R02 = −1

                    {
                        // Not a unique solution: yaw − roll = atan2(R01, R11)
                        roll = -Math.Atan2(rotationMatrix[1, 0], rotationMatrix[1, 1]);
                        pitch = -Math.PI / 2;
                        yaw = 0;
                    }

                }
                else // R02 = +1
                {
                    // Not a unique solution: yaw + roll = atan2(R10, R11)
                    roll = Math.Atan2(rotationMatrix[1, 0], rotationMatrix[1, 1]);
                    pitch = Math.PI / 2;
                    yaw = 0;
                }
                break;
            case RotationConvention.xyzExtrinsic:
            case RotationConvention.zyxIntrinsic:
                if (rotationMatrix[2, 0] < 1)
                {
                    if (rotationMatrix[2, 0] > -1)
                    {
                        roll = Math.Atan2(rotationMatrix[1, 0], rotationMatrix[0, 0]);
                        pitch = Math.Asin(-rotationMatrix[2, 0]);
                        yaw = Math.Atan2(rotationMatrix[2, 1], rotationMatrix[2, 2]);
                    }
                    else // R20 = −1

                    {
                        // Not a unique solution: yaw − roll = atan2(−R12, R11)
                        roll = -Math.Atan2(rotationMatrix[1, 2], rotationMatrix[1, 1]);
                        pitch = Math.PI / 2;
                        yaw = 0;
                    }

                }
                else // R20 = +1
                {
                    // Not a unique solution: yaw + roll = atan2(−R12, R11)
                    roll = Math.Atan2(rotationMatrix[1, 2], rotationMatrix[1, 1]);
                    pitch = -Math.PI / 2;
                    yaw = 0;
                }
                break;
        }
        switch (convention)
        {
            case RotationConvention.zyxExtrinsic:
            case RotationConvention.xyzExtrinsic:
                return CreateVector.DenseOfArray<double>(new double[] { yaw, pitch, roll });
            case RotationConvention.xyzIntrinsic:
            case RotationConvention.zyxIntrinsic:
                return CreateVector.DenseOfArray<double>(new double[] { roll, pitch, yaw });
        }
        throw new ArgumentException("Invalid RotationConvention");
    }

    static Vector<double>[] RotationMatrixToRollPitchYawList(Matrix<double> rotationMatrix)
    {
        Vector<double>[] rpyList = new Vector<double>[Enum.GetValues(typeof(RotationConvention)).Length];
        foreach (int i in Enum.GetValues(typeof(RotationConvention)))
        {
            RotationConvention convention = (RotationConvention)i;
            Console.WriteLine("Roll-Pitch-Yaw angles (" + convention + "):");
            rpyList[i] = RotationMatrixToRollPitchYaw(rotationMatrix, convention);
            Console.WriteLine(VectorToString(rpyList[i]));
        }
        return rpyList;
    }

    static Matrix<double> RollPitchYawToRotationMatrix(Vector<double> rollPitchYaw, RotationConvention convention)
    {
        switch (convention)
        {
            case RotationConvention.xyzIntrinsic:
                return CreateXRotation(rollPitchYaw[0]) * CreateYRotation(rollPitchYaw[1]) * CreateZRotation(rollPitchYaw[2]);
            case RotationConvention.zyxIntrinsic:
                return CreateZRotation(rollPitchYaw[0]) * CreateYRotation(rollPitchYaw[1]) * CreateXRotation(rollPitchYaw[2]);
            case RotationConvention.zyxExtrinsic:
                return CreateXRotation(rollPitchYaw[2]) * CreateYRotation(rollPitchYaw[1]) * CreateZRotation(rollPitchYaw[0]);
            case RotationConvention.xyzExtrinsic:
                return CreateZRotation(rollPitchYaw[2]) * CreateYRotation(rollPitchYaw[1]) * CreateXRotation(rollPitchYaw[0]);
            default: throw new ArgumentException("Invalid RotationConvention");
        }
    }

    static void RollPitchYawListToRotationMatrix(Vector<double>[] rpyList)
    {
        foreach (int i in Enum.GetValues(typeof(RotationConvention)))
        {
            RotationConvention convention = (RotationConvention)i;
            Console.WriteLine("Rotation Matrix from Roll-Pitch-Yaw angles (" + convention + "):");
            Console.WriteLine(MatrixToString(RollPitchYawToRotationMatrix(rpyList[i], convention)));
        }
    }

    static Matrix<double> ZividToMathDotNet(Zivid.NET.Matrix4x4 zividMatrix)
    {
        return CreateMatrix.DenseOfArray(zividMatrix.ToArray()).ToDouble();
    }
    static Zivid.NET.Matrix4x4 MathDotNetToZivid(Matrix<double> mathNetMatrix)
    {
        return new Zivid.NET.Matrix4x4(mathNetMatrix.ToSingle().ToArray());
    }

    static void PrintHeader(string text)
    {
        string asterixLine = "****************************************************************";
        Console.WriteLine(asterixLine + "\n* " + text + "\n" + asterixLine);
    }

    static string MatrixToString(Matrix<double> matrix)
    {
        string matrixString = "";
        if (matrix.RowCount != 1)
        {
            matrixString = "[";
        }
        for (var i = 0; i < matrix.RowCount; i++)
        {
            matrixString += "[";
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

    static string VectorToString(Vector<double> vector)
    {
        string vectorString = "[";
        for (var i = 0; i < vector.Count; i++)
        {
            vectorString += String.Format("{0,9:G4}", vector[i]);
        }
        vectorString += "]";

        return vectorString;
    }
}
