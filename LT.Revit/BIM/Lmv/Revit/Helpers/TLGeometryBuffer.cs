using System;
using Autodesk.Revit.DB;

namespace BIM.Lmv.Revit.Helpers
{
    internal class TLGeometryBuffer
    {
        public static double dScale = 1000.0;
        public readonly int Limit;
        public bool hasNormal;
        public ushort[] indices;
        public float[] normals;
        public int triangleCount;
        public float[] uvs;
        public float[] vertex;
        public int vertexCount;

        public TLGeometryBuffer(int limit = 0xffff)
        {
            Limit = limit;
            vertex = new float[Limit*3];
            indices = new ushort[Limit*3];
            normals = new float[Limit*3];
            uvs = new float[Limit*2];
            vertexCount = 0;
            triangleCount = 0;
            hasNormal = true;
        }

        public int GetAllLength()
        {
            return triangleCount*6 + vertexCount*0x20;
        }

        public void OnMesh(Mesh mesh)
        {
            var indices = new int[Limit*3];
            var vertexCount = this.vertexCount;
            var triangleCount = this.triangleCount;
            this.vertexCount += mesh.Vertices.Count;
            if (this.vertexCount > Limit)
            {
                throw new NotSupportedException("VertexCount: " + this.vertexCount);
            }
            var num3 = vertexCount;
            foreach (var xyz in mesh.Vertices)
            {
                vertex[num3*3] = (float) (xyz.X/dScale);
                vertex[num3*3 + 1] = (float) (xyz.Y/dScale);
                vertex[num3*3 + 2] = (float) (xyz.Z/dScale);
                TableHelp._EleBoxMin.x = Math.Min(TableHelp._EleBoxMin.x, vertex[num3*3]);
                TableHelp._EleBoxMin.y = Math.Min(TableHelp._EleBoxMin.y, vertex[num3*3 + 1]);
                TableHelp._EleBoxMin.z = Math.Min(TableHelp._EleBoxMin.z, vertex[num3*3 + 2]);
                TableHelp._EleBoxMax.x = Math.Max(TableHelp._EleBoxMax.x, vertex[num3*3]);
                TableHelp._EleBoxMax.y = Math.Max(TableHelp._EleBoxMax.y, vertex[num3*3 + 1]);
                TableHelp._EleBoxMax.z = Math.Max(TableHelp._EleBoxMax.z, vertex[num3*3 + 2]);
                num3++;
            }
            this.triangleCount += mesh.NumTriangles;
            if (this.triangleCount > Limit)
            {
                throw new NotSupportedException("TriangleCount: " + this.triangleCount);
            }
            var num4 = triangleCount;
            for (var i = 0; i < mesh.NumTriangles; i++)
            {
                var triangle = mesh.get_Triangle(i);
                this.indices[num4*3] = (ushort) triangle.get_Index(0);
                this.indices[num4*3 + 1] = (ushort) triangle.get_Index(1);
                this.indices[num4*3 + 2] = (ushort) triangle.get_Index(2);
                num4++;
            }
            for (var j = 0; j < mesh.NumTriangles; j++)
            {
                var triangle2 = mesh.get_Triangle(j);
                indices[j*3] = (int) triangle2.get_Index(0);
                indices[j*3 + 1] = (int) triangle2.get_Index(1);
                indices[j*3 + 2] = (int) triangle2.get_Index(2);
            }
            hasNormal = true;
            var normals = new TLNormalCalculator(mesh.Vertices, indices, mesh.NumTriangles).GetNormals();
            if (normals == null)
            {
                var num7 = vertexCount;
                for (var k = 0; k < mesh.Vertices.Count; k++)
                {
                    this.normals[num7*3] = 1f;
                    this.normals[num7*3 + 1] = 1f;
                    this.normals[num7*3 + 2] = 1f;
                    num7++;
                }
            }
            else
            {
                Array.Copy(normals, 0, this.normals, vertexCount, normals.Length);
            }
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            var vertexCount = this.vertexCount;
            var triangleCount = this.triangleCount;
            this.vertexCount += node.NumberOfPoints;
            if (this.vertexCount > Limit)
            {
                throw new NotSupportedException("VertexCount: " + this.vertexCount);
            }
            var num3 = vertexCount;
            foreach (var xyz in node.GetPoints())
            {
                vertex[num3*3] = (float) (xyz.X/dScale);
                vertex[num3*3 + 1] = (float) (xyz.Y/dScale);
                vertex[num3*3 + 2] = (float) (xyz.Z/dScale);
                TableHelp._EleBoxMin.x = Math.Min(TableHelp._EleBoxMin.x, vertex[num3*3]);
                TableHelp._EleBoxMin.y = Math.Min(TableHelp._EleBoxMin.y, vertex[num3*3 + 1]);
                TableHelp._EleBoxMin.z = Math.Min(TableHelp._EleBoxMin.z, vertex[num3*3 + 2]);
                TableHelp._EleBoxMax.x = Math.Max(TableHelp._EleBoxMax.x, vertex[num3*3]);
                TableHelp._EleBoxMax.y = Math.Max(TableHelp._EleBoxMax.y, vertex[num3*3 + 1]);
                TableHelp._EleBoxMax.z = Math.Max(TableHelp._EleBoxMax.z, vertex[num3*3 + 2]);
                num3++;
            }
            this.triangleCount += node.NumberOfFacets;
            var num4 = triangleCount;
            foreach (var facet in node.GetFacets())
            {
                indices[num4*3] = (ushort) (vertexCount + facet.V1);
                indices[num4*3 + 1] = (ushort) (vertexCount + facet.V2);
                indices[num4*3 + 2] = (ushort) (vertexCount + facet.V3);
                num4++;
            }
            hasNormal = true;
            switch (node.DistributionOfNormals)
            {
                case DistributionOfNormals.AtEachPoint:
                {
                    var num5 = vertexCount;
                    foreach (var xyz2 in node.GetNormals())
                    {
                        normals[num5*3] = (float) xyz2.X;
                        normals[num5*3 + 1] = (float) xyz2.Y;
                        normals[num5*3 + 2] = (float) xyz2.Z;
                        num5++;
                    }
                    break;
                }
                case DistributionOfNormals.OnePerFace:
                {
                    var normal = node.GetNormal(0);
                    for (var i = vertexCount; i < this.vertexCount; i++)
                    {
                        normals[i*3] = (float) normal.X;
                        normals[i*3 + 1] = (float) normal.Y;
                        normals[i*3 + 2] = (float) normal.Z;
                    }
                    break;
                }
                case DistributionOfNormals.OnEachFacet:
                {
                    var num7 = vertexCount;
                    foreach (var xyz4 in node.GetNormals())
                    {
                        var x = (float) xyz4.X;
                        var y = (float) xyz4.Y;
                        var z = (float) xyz4.Z;
                        var num11 = indices[num7*3];
                        var num12 = indices[num7*3 + 1];
                        var num13 = indices[num7*3 + 2];
                        normals[num11*3] = x;
                        normals[num11*3 + 1] = y;
                        normals[num11*3 + 2] = z;
                        normals[num12*3] = x;
                        normals[num12*3 + 1] = y;
                        normals[num12*3 + 2] = z;
                        normals[num13*3] = x;
                        normals[num13*3 + 1] = y;
                        normals[num13*3 + 2] = z;
                        num7++;
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (node.NumberOfUVs > 0)
            {
                var num14 = vertexCount;
                foreach (var uv in node.GetUVs())
                {
                    uvs[num14*2] = (float) uv.U;
                    uvs[num14*2 + 1] = (float) uv.V;
                    num14++;
                }
            }
        }
    }
}