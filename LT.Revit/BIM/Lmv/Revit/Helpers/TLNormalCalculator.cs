namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using System;
    using System.Collections.Generic;

    internal class TLNormalCalculator
    {
        private int[] _Indices;
        private int _IndicesCount;
        private IList<XYZ> _Verticles;

        public TLNormalCalculator(IList<XYZ> verticles, int[] indices, int indicesCount)
        {
            this._Verticles = verticles;
            this._Indices = indices;
            this._IndicesCount = indicesCount;
        }

        private XYZ GetMeshNormal(XYZ p1, XYZ p2, XYZ p3) => 
            ((p2 - p1)).CrossProduct(p3 - p1);

        public float[] GetNormals()
        {
            if (((this._Verticles == null) || (this._Verticles.Count < 1)) || (this._Indices.Length < 1))
            {
                return null;
            }
            if ((this._Indices.Length % 3) != 0)
            {
                return null;
            }
            try
            {
                float[] numArray = new float[this._Verticles.Count * 3];
                int[,] numArray2 = new int[this._IndicesCount, 3];
                for (long i = 0L; i < this._IndicesCount; i += 1L)
                {
                    numArray2[(int) ((IntPtr) i), (int) ((IntPtr) 0L)] = this._Indices[(int) ((IntPtr) (i * 3L))];
                    numArray2[(int) ((IntPtr) i), (int) ((IntPtr) 1L)] = this._Indices[(int) ((IntPtr) ((i * 3L) + 1L))];
                    numArray2[(int) ((IntPtr) i), (int) ((IntPtr) 2L)] = this._Indices[(int) ((IntPtr) ((i * 3L) + 2L))];
                }
                int[][] numArray3 = new int[this._Verticles.Count][];
                for (int j = 0; j < this._Verticles.Count; j++)
                {
                    List<int> list = new List<int>();
                    for (int m = 0; m < (numArray2.Length / 3); m++)
                    {
                        if (((j == numArray2[m, 0]) || (j == numArray2[m, 1])) || (j == numArray2[m, 2]))
                        {
                            list.Add(m);
                        }
                    }
                    numArray3[j] = list.ToArray();
                }
                for (int k = 0; k < this._Verticles.Count; k++)
                {
                    XYZ xyz = new XYZ(0.0, 0.0, 0.0);
                    for (long n = 0L; n < numArray3[k].Length; n += 1L)
                    {
                        xyz += this.GetMeshNormal(this._Verticles[Convert.ToInt32(numArray2[numArray3[k][(int) ((IntPtr) n)], 0])], this._Verticles[Convert.ToInt32(numArray2[numArray3[k][(int) ((IntPtr) n)], 1])], this._Verticles[Convert.ToInt32(numArray2[numArray3[k][(int) ((IntPtr) n)], 2])]);
                    }
                    XYZ xyz2 = xyz.Normalize();
                    numArray[k * 3] = (float) xyz2.X;
                    numArray[(k * 3) + 1] = (float) xyz2.Y;
                    numArray[(k * 3) + 2] = (float) xyz2.Z;
                }
                return numArray;
            }
            catch
            {
                return null;
            }
        }
    }
}

