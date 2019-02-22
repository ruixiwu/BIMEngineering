namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using BIM.Lmv;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

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
            this._Exporter = exporter;
            this._Buffer = new GeometryBuffer(0xffff);
        }

        private Mesh GetMeshFromRPC(Element element)
        {
            GeometryInstance instance = element.get_Geometry(new Options()).FirstOrDefault<GeometryObject>(x => (x is GeometryInstance)) as GeometryInstance;
            if (instance == null)
            {
                return null;
            }
            return (instance.SymbolGeometry.FirstOrDefault<GeometryObject>(x => (x is Mesh)) as Mesh);
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            this._Buffer.OnPolymesh(node);
            this._Exporter.OnGeometry(this._Buffer.vertexCount, this._Buffer.triangleCount, this._Buffer.hasNormal, this._Buffer.vertex, this._Buffer.indices, this._Buffer.normals, this._Buffer.uvs);
        }

        public void OnRPC(Element element)
        {
            Mesh meshFromRPC = this.GetMeshFromRPC(element);
            if (meshFromRPC != null)
            {
                this._Buffer.OnMesh(meshFromRPC);
                this._Exporter.OnGeometry(this._Buffer.vertexCount, this._Buffer.triangleCount, this._Buffer.hasNormal, this._Buffer.vertex, this._Buffer.indices, this._Buffer.normals, this._Buffer.uvs);
            }
        }

        private class GeometryBuffer
        {
            public bool hasNormal;
            public int[] indices;
            public readonly int Limit;
            public float[] normals;
            public int triangleCount;
            public float[] uvs;
            public float[] vertex;
            public int vertexCount;

            public GeometryBuffer(int limit = 0xffff)
            {
                this.Limit = limit;
                this.vertex = new float[this.Limit * 3];
                this.indices = new int[this.Limit * 3];
                this.normals = new float[this.Limit * 3];
                this.uvs = new float[this.Limit * 2];
            }

            public void OnMesh(Mesh mesh)
            {
                this.vertexCount = mesh.Vertices.Count;
                if (this.vertexCount > this.Limit)
                {
                    throw new NotSupportedException("VertexCount: " + this.vertexCount);
                }
                int num = 0;
                foreach (XYZ xyz in mesh.Vertices)
                {
                    this.vertex[num * 3] = (float) xyz.X;
                    this.vertex[(num * 3) + 1] = (float) xyz.Y;
                    this.vertex[(num * 3) + 2] = (float) xyz.Z;
                    num++;
                }
                this.triangleCount = mesh.NumTriangles;
                if (this.triangleCount > this.Limit)
                {
                    throw new NotSupportedException("TriangleCount: " + this.triangleCount);
                }
                for (int i = 0; i < mesh.NumTriangles; i++)
                {
                    MeshTriangle triangle = mesh.get_Triangle(i);
                    this.indices[i * 3] = (int) triangle.get_Index(0);
                    this.indices[(i * 3) + 1] = (int) triangle.get_Index(1);
                    this.indices[(i * 3) + 2] = (int) triangle.get_Index(2);
                }
                this.hasNormal = true;
                float[] normals = new GeometryHelper.NormalCalculator(mesh.Vertices, this.indices, mesh.NumTriangles).GetNormals();
                if (normals == null)
                {
                    for (int j = 0; j < this.vertexCount; j++)
                    {
                        this.normals[j * 3] = 1f;
                        this.normals[(j * 3) + 1] = 1f;
                        this.normals[(j * 3) + 2] = 1f;
                    }
                }
                else
                {
                    Array.Copy(normals, this.normals, normals.Length);
                }
            }

            public void OnPolymesh(PolymeshTopology node)
            {
                this.vertexCount = node.NumberOfPoints;
                if (this.vertexCount > this.Limit)
                {
                    throw new NotSupportedException("VertexCount: " + this.vertexCount);
                }
                int num = 0;
                foreach (XYZ xyz in node.GetPoints())
                {
                    this.vertex[num * 3] = (float) xyz.X;
                    this.vertex[(num * 3) + 1] = (float) xyz.Y;
                    this.vertex[(num * 3) + 2] = (float) xyz.Z;
                    num++;
                }
                this.triangleCount = node.NumberOfFacets;
                int num2 = 0;
                foreach (PolymeshFacet facet in node.GetFacets())
                {
                    this.indices[num2 * 3] = facet.V1;
                    this.indices[(num2 * 3) + 1] = facet.V2;
                    this.indices[(num2 * 3) + 2] = facet.V3;
                    num2++;
                }
                this.hasNormal = true;
                switch (node.DistributionOfNormals)
                {
                    case DistributionOfNormals.AtEachPoint:
                    {
                        int num3 = 0;
                        foreach (XYZ xyz2 in node.GetNormals())
                        {
                            this.normals[num3 * 3] = (float) xyz2.X;
                            this.normals[(num3 * 3) + 1] = (float) xyz2.Y;
                            this.normals[(num3 * 3) + 2] = (float) xyz2.Z;
                            num3++;
                        }
                        break;
                    }
                    case DistributionOfNormals.OnePerFace:
                    {
                        XYZ normal = node.GetNormal(0);
                        for (int i = 0; i < this.vertexCount; i++)
                        {
                            this.normals[i * 3] = (float) normal.X;
                            this.normals[(i * 3) + 1] = (float) normal.Y;
                            this.normals[(i * 3) + 2] = (float) normal.Z;
                        }
                        break;
                    }
                    case DistributionOfNormals.OnEachFacet:
                    {
                        int num5 = 0;
                        foreach (XYZ xyz4 in node.GetNormals())
                        {
                            float x = (float) xyz4.X;
                            float y = (float) xyz4.Y;
                            float z = (float) xyz4.Z;
                            int num9 = this.indices[num5 * 3];
                            int num10 = this.indices[(num5 * 3) + 1];
                            int num11 = this.indices[(num5 * 3) + 2];
                            this.normals[num9 * 3] = x;
                            this.normals[(num9 * 3) + 1] = y;
                            this.normals[(num9 * 3) + 2] = z;
                            this.normals[num10 * 3] = x;
                            this.normals[(num10 * 3) + 1] = y;
                            this.normals[(num10 * 3) + 2] = z;
                            this.normals[num11 * 3] = x;
                            this.normals[(num11 * 3) + 1] = y;
                            this.normals[(num11 * 3) + 2] = z;
                            num5++;
                        }
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (node.NumberOfUVs > 0)
                {
                    int num12 = 0;
                    foreach (UV uv in node.GetUVs())
                    {
                        this.uvs[num12 * 2] = (float) uv.U;
                        this.uvs[(num12 * 2) + 1] = (float) uv.V;
                        num12++;
                    }
                }
            }
        }

        private class NormalCalculator
        {
            private int[] _Indices;
            private int _IndicesCount;
            private IList<XYZ> _Verticles;

            public NormalCalculator(IList<XYZ> verticles, int[] indices, int indicesCount)
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
}

