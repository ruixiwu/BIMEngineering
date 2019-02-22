namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using BIM.Lmv.Content.Geometry.Types;
    using System;
    using System.Runtime.CompilerServices;

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

        public static BIM.Lmv.Content.Geometry.Types.Transform Convert(Autodesk.Revit.DB.Transform t)
        {
            if (t == null)
            {
                return BIM.Lmv.Content.Geometry.Types.Transform.GetIdentity();
            }
            if (t.IsIdentity)
            {
                return BIM.Lmv.Content.Geometry.Types.Transform.GetIdentity();
            }
            if (t.IsTranslation)
            {
                XYZ xyz = t.Origin;
                Vector3D vectord = new Vector3D(xyz.X, xyz.Y, xyz.Z);
                return BIM.Lmv.Content.Geometry.Types.Transform.GetTranslation(vectord);
            }
            if (t.IsConformal && !t.HasReflection)
            {
                if (IsAlmostEqual(t.Scale, 1.0))
                {
                    XYZ xyz2 = t.Origin;
                    Vector3D vectord2 = new Vector3D(xyz2.X, xyz2.Y, xyz2.Z);
                    return BIM.Lmv.Content.Geometry.Types.Transform.GetRotationTranslation(GetQuaternion(t), vectord2);
                }
                Matrix4F matrixf = GetMatrixFrom(t);
                float x = (float) (1.0 / t.Scale);
                matrixf.scale(new Vector3F(x, x, x));
                XYZ xyz3 = t.Origin;
                Vector3D vectord3 = new Vector3D(xyz3.X, xyz3.Y, xyz3.Z);
                Vector4F quaternion = GetQuaternion(matrixf);
                return BIM.Lmv.Content.Geometry.Types.Transform.GetUniformScaleRotationTranslation((float) t.Scale, quaternion, vectord3);
            }
            Matrix4F matrixFrom = GetMatrixFrom(t);
            XYZ origin = t.Origin;
            Vector3D translation = new Vector3D(origin.X, origin.Y, origin.Z);
            return BIM.Lmv.Content.Geometry.Types.Transform.GetAffineMatrix(matrixFrom, translation);
        }

        public static Vector3F Convert(this XYZ p) => 
            new Vector3F((float) p.X, (float) p.Y, (float) p.Z);

        public static Matrix4F GetMatrixFrom(Autodesk.Revit.DB.Transform t)
        {
            Matrix4F matrixf = new Matrix4F().identity();
            XYZ basisX = t.BasisX;
            matrixf.elements[0] = (float) basisX.X;
            matrixf.elements[1] = (float) basisX.Y;
            matrixf.elements[2] = (float) basisX.Z;
            XYZ basisY = t.BasisY;
            matrixf.elements[4] = (float) basisY.X;
            matrixf.elements[5] = (float) basisY.Y;
            matrixf.elements[6] = (float) basisY.Z;
            XYZ basisZ = t.BasisZ;
            matrixf.elements[8] = (float) basisZ.X;
            matrixf.elements[9] = (float) basisZ.Y;
            matrixf.elements[10] = (float) basisZ.Z;
            matrixf.elements[15] = 1f;
            return matrixf;
        }

        public static Vector4F GetQuaternion(Autodesk.Revit.DB.Transform t)
        {
            XYZ basisX = t.BasisX;
            XYZ basisY = t.BasisY;
            XYZ basisZ = t.BasisZ;
            double[] numArray2 = Matrix3ToQuaternion(new double[][] { new double[] { basisX.X, basisY.X, basisZ.X }, new double[] { basisX.Y, basisY.Y, basisZ.Y }, new double[] { basisX.Z, basisY.Z, basisZ.Z } });
            return new Vector4F((float) numArray2[0], (float) numArray2[1], (float) numArray2[2], (float) numArray2[3]);
        }

        public static Vector4F GetQuaternion(Matrix4F t)
        {
            float[] elements = t.elements;
            double[] numArray3 = Matrix3ToQuaternion(new double[][] { new double[] { (double) elements[0], (double) elements[4], (double) elements[8] }, new double[] { (double) elements[1], (double) elements[5], (double) elements[9] }, new double[] { (double) elements[2], (double) elements[6], (double) elements[10] } });
            return new Vector4F((float) numArray3[0], (float) numArray3[1], (float) numArray3[2], (float) numArray3[3]);
        }

        public static Vector4F GetQuaternionEx(Autodesk.Revit.DB.Transform t)
        {
            XYZ basisX = t.BasisX;
            XYZ basisY = t.BasisY;
            XYZ basisZ = t.BasisZ;
            double x = basisX.X;
            double y = basisX.Y;
            double z = basisX.Z;
            double num4 = basisY.X;
            double num5 = basisY.Y;
            double num6 = basisY.Z;
            double num7 = basisZ.X;
            double num8 = basisZ.Y;
            double num9 = basisZ.Z;
            double num10 = 0.0;
            double num11 = 0.0;
            double num12 = 0.0;
            double num13 = 0.0;
            double num14 = (x + num5) + num9;
            double num15 = (x - num5) - num9;
            double num16 = (num5 - x) - num9;
            double num17 = (num9 - x) - num5;
            int num18 = 0;
            double num19 = num14;
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
            double num20 = Math.Sqrt(num19 + 1.0) * 0.5;
            double num21 = 0.25 / num20;
            switch (num18)
            {
                case 0:
                    num10 = num20;
                    num11 = (num6 - num8) * num21;
                    num12 = (num7 - z) * num21;
                    num13 = (y - num4) * num21;
                    break;

                case 1:
                    num11 = num20;
                    num10 = (num6 - num8) * num21;
                    num12 = (y + num4) * num21;
                    num13 = (num7 + z) * num21;
                    break;

                case 2:
                    num12 = num20;
                    num10 = (num7 - z) * num21;
                    num11 = (y + num4) * num21;
                    num13 = (num6 + num8) * num21;
                    break;

                case 3:
                    num13 = num20;
                    num10 = (y - num4) * num21;
                    num11 = (num7 + z) * num21;
                    num12 = (num6 + num8) * num21;
                    break;
            }
            return new Vector4F((float) num11, (float) num12, (float) num13, (float) num10);
        }

        public static bool IsAlmostEqual(double a, double y) => 
            (Math.Abs((double) (a - y)) < 1E-08);

        private static double[] Matrix3ToQuaternion(double[][] mat)
        {
            double num;
            double num2;
            double[] numArray = new double[4];
            int[] numArray3 = new int[3];
            numArray3[0] = 1;
            numArray3[1] = 2;
            int[] numArray2 = numArray3;
            double num3 = (mat[0][0] + mat[1][1]) + mat[2][2];
            if (num3 > 0.0)
            {
                num2 = num3 + 1.0;
                num = 0.5 / Math.Sqrt(num2);
                numArray[3] = num * num2;
                numArray[0] = (mat[2][1] - mat[1][2]) * num;
                numArray[1] = (mat[0][2] - mat[2][0]) * num;
                numArray[2] = (mat[1][0] - mat[0][1]) * num;
                return numArray;
            }
            int index = 0;
            if (mat[1][1] > mat[0][0])
            {
                index = 1;
            }
            if (mat[2][2] > mat[index][index])
            {
                index = 2;
            }
            int num5 = numArray2[index];
            int num6 = numArray2[num5];
            num2 = (mat[index][index] - (mat[num5][num5] + mat[num6][num6])) + 1.0;
            num = 0.5 / Math.Sqrt(num2);
            numArray[index] = num * num2;
            numArray[3] = (mat[num6][num5] - mat[num5][num6]) * num;
            numArray[num5] = (mat[num5][index] + mat[index][num5]) * num;
            numArray[num6] = (mat[num6][index] + mat[index][num6]) * num;
            return numArray;
        }
    }
}

