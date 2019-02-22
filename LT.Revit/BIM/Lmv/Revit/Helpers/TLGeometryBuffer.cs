namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using System;
    using System.Runtime.InteropServices;

    internal class TLGeometryBuffer
    {
        public static double dScale = 1.0;
        public bool hasNormal;
        public ushort[] indices;
        public readonly int Limit;
        public float[] normals;
        public int triangleCount;
        public float[] uvs;
        public float[] vertex;
        public int vertexCount;

        public TLGeometryBuffer(int limit = 0xffff)
        {
            this.Limit = limit;
            this.vertex = new float[this.Limit * 3];
            this.indices = new ushort[this.Limit * 3];
            this.normals = new float[this.Limit * 3];
            this.uvs = new float[this.Limit * 2];
            this.vertexCount = 0;
            this.triangleCount = 0;
            this.hasNormal = true;
        }

        public int GetAllLength() => 
            ((this.triangleCount * 6) + (this.vertexCount * 0x20));

        public void OnMesh(Mesh mesh)
        {
            int[] indices = new int[this.Limit * 3];
            int vertexCount = this.vertexCount;
            int triangleCount = this.triangleCount;
            this.vertexCount += mesh.Vertices.Count;
            if (this.vertexCount > this.Limit)
            {
                throw new NotSupportedException("VertexCount: " + this.vertexCount);
            }
            int num3 = vertexCount;
            foreach (XYZ xyz in mesh.Vertices)
            {
                this.vertex[num3 * 3] = (float) (xyz.X / dScale);
                this.vertex[(num3 * 3) + 1] = (float) (xyz.Y / dScale);
                this.vertex[(num3 * 3) + 2] = (float) (xyz.Z / dScale);
                TableHelp._EleBoxMin.x = Math.Min(TableHelp._EleBoxMin.x, this.vertex[num3 * 3]);
                TableHelp._EleBoxMin.y = Math.Min(TableHelp._EleBoxMin.y, this.vertex[(num3 * 3) + 1]);
                TableHelp._EleBoxMin.z = Math.Min(TableHelp._EleBoxMin.z, this.vertex[(num3 * 3) + 2]);
                TableHelp._EleBoxMax.x = Math.Max(TableHelp._EleBoxMax.x, this.vertex[num3 * 3]);
                TableHelp._EleBoxMax.y = Math.Max(TableHelp._EleBoxMax.y, this.vertex[(num3 * 3) + 1]);
                TableHelp._EleBoxMax.z = Math.Max(TableHelp._EleBoxMax.z, this.vertex[(num3 * 3) + 2]);
                num3++;
            }
            this.triangleCount += mesh.NumTriangles;
            if (this.triangleCount > this.Limit)
            {
                throw new NotSupportedException("TriangleCount: " + this.triangleCount);
            }
            int num4 = triangleCount;
            for (int i = 0; i < mesh.NumTriangles; i++)
            {
                MeshTriangle triangle = mesh.get_Triangle(i);
                this.indices[num4 * 3] = (ushort) triangle.get_Index(0);
                this.indices[(num4 * 3) + 1] = (ushort) triangle.get_Index(1);
                this.indices[(num4 * 3) + 2] = (ushort) triangle.get_Index(2);
                num4++;
            }
            for (int j = 0; j < mesh.NumTriangles; j++)
            {
                MeshTriangle triangle2 = mesh.get_Triangle(j);
                indices[j * 3] = (int) triangle2.get_Index(0);
                indices[(j * 3) + 1] = (int) triangle2.get_Index(1);
                indices[(j * 3) + 2] = (int) triangle2.get_Index(2);
            }
            this.hasNormal = true;
            float[] normals = new TLNormalCalculator(mesh.Vertices, indices, mesh.NumTriangles).GetNormals();
            if (normals == null)
            {
                int num7 = vertexCount;
                for (int k = 0; k < mesh.Vertices.Count; k++)
                {
                    this.normals[num7 * 3] = 1f;
                    this.normals[(num7 * 3) + 1] = 1f;
                    this.normals[(num7 * 3) + 2] = 1f;
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
            int vertexCount = this.vertexCount;
            int triangleCount = this.triangleCount;
            this.vertexCount += node.NumberOfPoints;
            if (this.vertexCount > this.Limit)
            {
                throw new NotSupportedException("VertexCount: " + this.vertexCount);
            }
            int num3 = vertexCount;
            foreach (XYZ xyz in node.GetPoints())
            {
                this.vertex[num3 * 3] = (float) (xyz.X / dScale);
                this.vertex[(num3 * 3) + 1] = (float) (xyz.Y / dScale);
                this.vertex[(num3 * 3) + 2] = (float) (xyz.Z / dScale);
                TableHelp._EleBoxMin.x = Math.Min(TableHelp._EleBoxMin.x, this.vertex[num3 * 3]);
                TableHelp._EleBoxMin.y = Math.Min(TableHelp._EleBoxMin.y, this.vertex[(num3 * 3) + 1]);
                TableHelp._EleBoxMin.z = Math.Min(TableHelp._EleBoxMin.z, this.vertex[(num3 * 3) + 2]);
                TableHelp._EleBoxMax.x = Math.Max(TableHelp._EleBoxMax.x, this.vertex[num3 * 3]);
                TableHelp._EleBoxMax.y = Math.Max(TableHelp._EleBoxMax.y, this.vertex[(num3 * 3) + 1]);
                TableHelp._EleBoxMax.z = Math.Max(TableHelp._EleBoxMax.z, this.vertex[(num3 * 3) + 2]);
                num3++;
            }
            this.triangleCount += node.NumberOfFacets;
            int num4 = triangleCount;
            foreach (PolymeshFacet facet in node.GetFacets())
            {
                this.indices[num4 * 3] = (ushort) (vertexCount + facet.V1);
                this.indices[(num4 * 3) + 1] = (ushort) (vertexCount + facet.V2);
                this.indices[(num4 * 3) + 2] = (ushort) (vertexCount + facet.V3);
                num4++;
            }
            this.hasNormal = true;
            switch (node.DistributionOfNormals)
            {
                case DistributionOfNormals.AtEachPoint:
                {
                    int num5 = vertexCount;
                    foreach (XYZ xyz2 in node.GetNormals())
                    {
                        this.normals[num5 * 3] = (float) xyz2.X;
                        this.normals[(num5 * 3) + 1] = (float) xyz2.Y;
                        this.normals[(num5 * 3) + 2] = (float) xyz2.Z;
                        num5++;
                    }
                    break;
                }
                case DistributionOfNormals.OnePerFace:
                {
                    XYZ normal = node.GetNormal(0);
                    for (int i = vertexCount; i < this.vertexCount; i++)
                    {
                        this.normals[i * 3] = (float) normal.X;
                        this.normals[(i * 3) + 1] = (float) normal.Y;
                        this.normals[(i * 3) + 2] = (float) normal.Z;
                    }
                    break;
                }
                case DistributionOfNormals.OnEachFacet:
                {
                    int num7 = vertexCount;
                    foreach (XYZ xyz4 in node.GetNormals())
                    {
                        float x = (float) xyz4.X;
                        float y = (float) xyz4.Y;
                        float z = (float) xyz4.Z;
                        ushort num11 = this.indices[num7 * 3];
                        ushort num12 = this.indices[(num7 * 3) + 1];
                        ushort num13 = this.indices[(num7 * 3) + 2];
                        this.normals[num11 * 3] = x;
                        this.normals[(num11 * 3) + 1] = y;
                        this.normals[(num11 * 3) + 2] = z;
                        this.normals[num12 * 3] = x;
                        this.normals[(num12 * 3) + 1] = y;
                        this.normals[(num12 * 3) + 2] = z;
                        this.normals[num13 * 3] = x;
                        this.normals[(num13 * 3) + 1] = y;
                        this.normals[(num13 * 3) + 2] = z;
                        num7++;
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (node.NumberOfUVs > 0)
            {
                int num14 = vertexCount;
                foreach (UV uv in node.GetUVs())
                {
                    this.uvs[num14 * 2] = (float) uv.U;
                    this.uvs[(num14 * 2) + 1] = (float) uv.V;
                    num14++;
                }
            }
        }
    }
}

