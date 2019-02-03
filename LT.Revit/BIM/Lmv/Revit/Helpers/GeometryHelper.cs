using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace BIM.Lmv.Revit.Helpers
{
    internal class GeometryHelper
    {
        private readonly GeometryBuffer _Buffer;
        private readonly IDataExport _Exporter;

        public GeometryHelper(IDataExport exporter)
        {
            if (exporter == null)
            {
                throw new ArgumentNullException("exporter");
            }
            _Exporter = exporter;
            _Buffer = new GeometryBuffer(0xffff);
        }

        private Mesh GetMeshFromRPC(Element element)
        {
            var instance =
                element.get_Geometry(new Options()).FirstOrDefault(x => x is GeometryInstance) as
                    GeometryInstance;
            if (instance == null)
            {
                return null;
            }
            return instance.SymbolGeometry.FirstOrDefault(x => x is Mesh) as Mesh;
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            _Buffer.OnPolymesh(node);
            _Exporter.OnGeometry(_Buffer.vertexCount, _Buffer.triangleCount, _Buffer.hasNormal,
                _Buffer.vertex, _Buffer.indices, _Buffer.normals, _Buffer.uvs);
        }

        public void OnRPC(Element element)
        {
            var meshFromRPC = GetMeshFromRPC(element);
            if (meshFromRPC != null)
            {
                _Buffer.OnMesh(meshFromRPC);
                _Exporter.OnGeometry(_Buffer.vertexCount, _Buffer.triangleCount, _Buffer.hasNormal,
                    _Buffer.vertex, _Buffer.indices, _Buffer.normals, _Buffer.uvs);
            }
        }

        private class GeometryBuffer
        {
            public readonly int Limit;
            public bool hasNormal;
            public readonly int[] indices;
            public readonly float[] normals;
            public int triangleCount;
            public readonly float[] uvs;
            public readonly float[] vertex;
            public int vertexCount;

            public GeometryBuffer(int limit = 0xffff)
            {
                Limit = limit;
                vertex = new float[Limit*3];
                indices = new int[Limit*3];
                normals = new float[Limit*3];
                uvs = new float[Limit*2];
            }

            public void OnMesh(Mesh mesh)
            {
                vertexCount = mesh.Vertices.Count;
                if (vertexCount > Limit)
                {
                    throw new NotSupportedException("VertexCount: " + vertexCount);
                }
                var num = 0;
                foreach (var xyz in mesh.Vertices)
                {
                    vertex[num*3] = (float) xyz.X;
                    vertex[num*3 + 1] = (float) xyz.Y;
                    vertex[num*3 + 2] = (float) xyz.Z;
                    num++;
                }
                triangleCount = mesh.NumTriangles;
                if (triangleCount > Limit)
                {
                    throw new NotSupportedException("TriangleCount: " + triangleCount);
                }
                for (var i = 0; i < mesh.NumTriangles; i++)
                {
                    var triangle = mesh.get_Triangle(i);
                    indices[i*3] = (int) triangle.get_Index(0);
                    indices[i*3 + 1] = (int) triangle.get_Index(1);
                    indices[i*3 + 2] = (int) triangle.get_Index(2);
                }
                hasNormal = true;
                var normals =
                    new NormalCalculator(mesh.Vertices, indices, mesh.NumTriangles).GetNormals();
                if (normals == null)
                {
                    for (var j = 0; j < vertexCount; j++)
                    {
                        this.normals[j*3] = 1f;
                        this.normals[j*3 + 1] = 1f;
                        this.normals[j*3 + 2] = 1f;
                    }
                }
                else
                {
                    Array.Copy(normals, this.normals, normals.Length);
                }
            }

            public void OnPolymesh(PolymeshTopology node)
            {
                vertexCount = node.NumberOfPoints;
                if (vertexCount > Limit)
                {
                    throw new NotSupportedException("VertexCount: " + vertexCount);
                }
                var num = 0;
                foreach (var xyz in node.GetPoints())
                {
                    vertex[num*3] = (float) xyz.X;
                    vertex[num*3 + 1] = (float) xyz.Y;
                    vertex[num*3 + 2] = (float) xyz.Z;
                    num++;
                }
                triangleCount = node.NumberOfFacets;
                var num2 = 0;
                foreach (var facet in node.GetFacets())
                {
                    indices[num2*3] = facet.V1;
                    indices[num2*3 + 1] = facet.V2;
                    indices[num2*3 + 2] = facet.V3;
                    num2++;
                }
                hasNormal = true;
                switch (node.DistributionOfNormals)
                {
                    case DistributionOfNormals.AtEachPoint:
                    {
                        var num3 = 0;
                        foreach (var xyz2 in node.GetNormals())
                        {
                            normals[num3*3] = (float) xyz2.X;
                            normals[num3*3 + 1] = (float) xyz2.Y;
                            normals[num3*3 + 2] = (float) xyz2.Z;
                            num3++;
                        }
                        break;
                    }
                    case DistributionOfNormals.OnePerFace:
                    {
                        var normal = node.GetNormal(0);
                        for (var i = 0; i < vertexCount; i++)
                        {
                            normals[i*3] = (float) normal.X;
                            normals[i*3 + 1] = (float) normal.Y;
                            normals[i*3 + 2] = (float) normal.Z;
                        }
                        break;
                    }
                    case DistributionOfNormals.OnEachFacet:
                    {
                        var num5 = 0;
                        foreach (var xyz4 in node.GetNormals())
                        {
                            var x = (float) xyz4.X;
                            var y = (float) xyz4.Y;
                            var z = (float) xyz4.Z;
                            var num9 = indices[num5*3];
                            var num10 = indices[num5*3 + 1];
                            var num11 = indices[num5*3 + 2];
                            normals[num9*3] = x;
                            normals[num9*3 + 1] = y;
                            normals[num9*3 + 2] = z;
                            normals[num10*3] = x;
                            normals[num10*3 + 1] = y;
                            normals[num10*3 + 2] = z;
                            normals[num11*3] = x;
                            normals[num11*3 + 1] = y;
                            normals[num11*3 + 2] = z;
                            num5++;
                        }
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (node.NumberOfUVs > 0)
                {
                    var num12 = 0;
                    foreach (var uv in node.GetUVs())
                    {
                        uvs[num12*2] = (float) uv.U;
                        uvs[num12*2 + 1] = (float) uv.V;
                        num12++;
                    }
                }
            }
        }

        private class NormalCalculator
        {
            private readonly int[] _Indices;
            private readonly int _IndicesCount;
            private readonly IList<XYZ> _Verticles;

            public NormalCalculator(IList<XYZ> verticles, int[] indices, int indicesCount)
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
                        numArray2[(int) (IntPtr) i, (int) (IntPtr) 1L] =
                            _Indices[(int) (IntPtr) (i*3L + 1L)];
                        numArray2[(int) (IntPtr) i, (int) (IntPtr) 2L] =
                            _Indices[(int) (IntPtr) (i*3L + 2L)];
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
}