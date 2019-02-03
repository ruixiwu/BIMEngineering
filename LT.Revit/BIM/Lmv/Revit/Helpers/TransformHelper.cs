using System;
using Autodesk.Revit.DB;
using BIM.Lmv.Content.Geometry.Types;
using Transform = BIM.Lmv.Content.Geometry.Types.Transform;

namespace BIM.Lmv.Revit.Helpers
{
    internal static class TransformHelper
    {
        public static Box3F Convert(BoundingBoxXYZ box)
        {
            if (box == null)
            {
                return null;
            }
            return new Box3F(box.Min.Convert(), box.Max.Convert());
        }

        public static Transform Convert(Autodesk.Revit.DB.Transform t)
        {
            if (t == null)
            {
                return Transform.GetIdentity();
            }
            if (t.IsIdentity)
            {
                return Transform.GetIdentity();
            }
            if (t.IsTranslation)
            {
                var xyz = t.Origin;
                var vectord = new Vector3D(xyz.X, xyz.Y, xyz.Z);
                return Transform.GetTranslation(vectord);
            }
            if (t.IsConformal && !t.HasReflection)
            {
                if (IsAlmostEqual(t.Scale, 1.0))
                {
                    var xyz2 = t.Origin;
                    var vectord2 = new Vector3D(xyz2.X, xyz2.Y, xyz2.Z);
                    return Transform.GetRotationTranslation(GetQuaternion(t), vectord2);
                }
                var matrixf = GetMatrixFrom(t);
                var x = (float) (1.0/t.Scale);
                matrixf.scale(new Vector3F(x, x, x));
                var xyz3 = t.Origin;
                var vectord3 = new Vector3D(xyz3.X, xyz3.Y, xyz3.Z);
                var quaternion = GetQuaternion(matrixf);
                return Transform.GetUniformScaleRotationTranslation((float) t.Scale,
                    quaternion, vectord3);
            }
            var matrixFrom = GetMatrixFrom(t);
            var origin = t.Origin;
            var translation = new Vector3D(origin.X, origin.Y, origin.Z);
            return Transform.GetAffineMatrix(matrixFrom, translation);
        }

        public static Vector3F Convert(this XYZ p)
        {
            return new Vector3F((float) p.X, (float) p.Y, (float) p.Z);
        }

        public static Matrix4F GetMatrixFrom(Autodesk.Revit.DB.Transform t)
        {
            var matrixf = new Matrix4F().identity();
            var basisX = t.BasisX;
            matrixf.elements[0] = (float) basisX.X;
            matrixf.elements[1] = (float) basisX.Y;
            matrixf.elements[2] = (float) basisX.Z;
            var basisY = t.BasisY;
            matrixf.elements[4] = (float) basisY.X;
            matrixf.elements[5] = (float) basisY.Y;
            matrixf.elements[6] = (float) basisY.Z;
            var basisZ = t.BasisZ;
            matrixf.elements[8] = (float) basisZ.X;
            matrixf.elements[9] = (float) basisZ.Y;
            matrixf.elements[10] = (float) basisZ.Z;
            matrixf.elements[15] = 1f;
            return matrixf;
        }

        public static Vector4F GetQuaternion(Autodesk.Revit.DB.Transform t)
        {
            var basisX = t.BasisX;
            var basisY = t.BasisY;
            var basisZ = t.BasisZ;
            var numArray2 =
                Matrix3ToQuaternion(new[]
                {
                    new[] {basisX.X, basisY.X, basisZ.X}, new[] {basisX.Y, basisY.Y, basisZ.Y},
                    new[] {basisX.Z, basisY.Z, basisZ.Z}
                });
            return new Vector4F((float) numArray2[0], (float) numArray2[1], (float) numArray2[2], (float) numArray2[3]);
        }

        public static Vector4F GetQuaternion(Matrix4F t)
        {
            var elements = t.elements;
            var numArray3 =
                Matrix3ToQuaternion(new[]
                {
                    new[] {elements[0], elements[4], (double) elements[8]},
                    new[] {elements[1], elements[5], (double) elements[9]},
                    new[] {elements[2], elements[6], (double) elements[10]}
                });
            return new Vector4F((float) numArray3[0], (float) numArray3[1], (float) numArray3[2], (float) numArray3[3]);
        }

        public static Vector4F GetQuaternionEx(Autodesk.Revit.DB.Transform t)
        {
            var basisX = t.BasisX;
            var basisY = t.BasisY;
            var basisZ = t.BasisZ;
            var x = basisX.X;
            var y = basisX.Y;
            var z = basisX.Z;
            var num4 = basisY.X;
            var num5 = basisY.Y;
            var num6 = basisY.Z;
            var num7 = basisZ.X;
            var num8 = basisZ.Y;
            var num9 = basisZ.Z;
            var num10 = 0.0;
            var num11 = 0.0;
            var num12 = 0.0;
            var num13 = 0.0;
            var num14 = x + num5 + num9;
            var num15 = x - num5 - num9;
            var num16 = num5 - x - num9;
            var num17 = num9 - x - num5;
            var num18 = 0;
            var num19 = num14;
            if (num15 > num19)
            {
                num19 = num15;
                num18 = 1;
            }
            if (num16 > num19)
            {
                num19 = num16;
                num18 = 2;
            }
            if (num17 > num19)
            {
                num19 = num17;
                num18 = 3;
            }
            var num20 = Math.Sqrt(num19 + 1.0)*0.5;
            var num21 = 0.25/num20;
            switch (num18)
            {
                case 0:
                    num10 = num20;
                    num11 = (num6 - num8)*num21;
                    num12 = (num7 - z)*num21;
                    num13 = (y - num4)*num21;
                    break;

                case 1:
                    num11 = num20;
                    num10 = (num6 - num8)*num21;
                    num12 = (y + num4)*num21;
                    num13 = (num7 + z)*num21;
                    break;

                case 2:
                    num12 = num20;
                    num10 = (num7 - z)*num21;
                    num11 = (y + num4)*num21;
                    num13 = (num6 + num8)*num21;
                    break;

                case 3:
                    num13 = num20;
                    num10 = (y - num4)*num21;
                    num11 = (num7 + z)*num21;
                    num12 = (num6 + num8)*num21;
                    break;
            }
            return new Vector4F((float) num11, (float) num12, (float) num13, (float) num10);
        }

        public static bool IsAlmostEqual(double a, double y)
        {
            return Math.Abs(a - y) < 1E-08;
        }

        private static double[] Matrix3ToQuaternion(double[][] mat)
        {
            double num;
            double num2;
            var numArray = new double[4];
            var numArray3 = new int[3];
            numArray3[0] = 1;
            numArray3[1] = 2;
            var numArray2 = numArray3;
            var num3 = mat[0][0] + mat[1][1] + mat[2][2];
            if (num3 > 0.0)
            {
                num2 = num3 + 1.0;
                num = 0.5/Math.Sqrt(num2);
                numArray[3] = num*num2;
                numArray[0] = (mat[2][1] - mat[1][2])*num;
                numArray[1] = (mat[0][2] - mat[2][0])*num;
                numArray[2] = (mat[1][0] - mat[0][1])*num;
                return numArray;
            }
            var index = 0;
            if (mat[1][1] > mat[0][0])
            {
                index = 1;
            }
            if (mat[2][2] > mat[index][index])
            {
                index = 2;
            }
            var num5 = numArray2[index];
            var num6 = numArray2[num5];
            num2 = mat[index][index] - (mat[num5][num5] + mat[num6][num6]) + 1.0;
            num = 0.5/Math.Sqrt(num2);
            numArray[index] = num*num2;
            numArray[3] = (mat[num6][num5] - mat[num5][num6])*num;
            numArray[num5] = (mat[num5][index] + mat[index][num5])*num;
            numArray[num6] = (mat[num6][index] + mat[index][num6])*num;
            return numArray;
        }
    }
}