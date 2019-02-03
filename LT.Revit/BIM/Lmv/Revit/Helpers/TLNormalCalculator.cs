using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BIM.Lmv.Revit.Helpers
{
    internal class TLNormalCalculator
    {
        private readonly int[] _Indices;
        private readonly int _IndicesCount;
        private readonly IList<XYZ> _Verticles;

        public TLNormalCalculator(IList<XYZ> verticles, int[] indices, int indicesCount)
        {
            _Verticles = verticles;
            _Indices = indices;
            _IndicesCount = indicesCount;
        }

        private XYZ GetMeshNormal(XYZ p1, XYZ p2, XYZ p3)
        {
            return (p2 - p1).CrossProduct(p3 - p1);
        }

        public float[] GetNormals()
        {
            if ((_Verticles == null) || (_Verticles.Count < 1) || (_Indices.Length < 1))
            {
                return null;
            }
            if (_Indices.Length%3 != 0)
            {
                return null;
            }
            try
            {
                var numArray = new float[_Verticles.Count*3];
                var numArray2 = new int[_IndicesCount, 3];
                for (var i = 0L; i < _IndicesCount; i += 1L)
                {
                    numArray2[(int) (IntPtr) i, (int) (IntPtr) 0L] = _Indices[(int) (IntPtr) (i*3L)];
                    numArray2[(int) (IntPtr) i, (int) (IntPtr) 1L] = _Indices[(int) (IntPtr) (i*3L + 1L)];
                    numArray2[(int) (IntPtr) i, (int) (IntPtr) 2L] = _Indices[(int) (IntPtr) (i*3L + 2L)];
                }
                var numArray3 = new int[_Verticles.Count][];
                for (var j = 0; j < _Verticles.Count; j++)
                {
                    var list = new List<int>();
                    for (var m = 0; m < numArray2.Length/3; m++)
                    {
                        if ((j == numArray2[m, 0]) || (j == numArray2[m, 1]) || (j == numArray2[m, 2]))
                        {
                            list.Add(m);
                        }
                    }
                    numArray3[j] = list.ToArray();
                }
                for (var k = 0; k < _Verticles.Count; k++)
                {
                    var xyz = new XYZ(0.0, 0.0, 0.0);
                    for (var n = 0L; n < numArray3[k].Length; n += 1L)
                    {
                        xyz +=
                            GetMeshNormal(
                                _Verticles[Convert.ToInt32(numArray2[numArray3[k][(int) (IntPtr) n], 0])],
                                _Verticles[Convert.ToInt32(numArray2[numArray3[k][(int) (IntPtr) n], 1])],
                                _Verticles[Convert.ToInt32(numArray2[numArray3[k][(int) (IntPtr) n], 2])]);
                    }
                    var xyz2 = xyz.Normalize();
                    numArray[k*3] = (float) xyz2.X;
                    numArray[k*3 + 1] = (float) xyz2.Y;
                    numArray[k*3 + 2] = (float) xyz2.Z;
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