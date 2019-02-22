namespace BIM.Lmv.Content.Geometry
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class EntryGeometry : PackEntryBase
    {
        public readonly Dictionary<string, float[]> attributes = new Dictionary<string, float[]>();
        public Box3F box = new Box3F();
        public bool hasNormal;
        public readonly int[] indices;
        public readonly int Limit;
        public readonly float[] normals;
        public Transform transform;
        private Vector3F transformPoint = new Vector3F();
        public int triangleCount;
        public const string UV_alpha = "alpha";
        public const string UV_bump = "bump";
        public const string UV_diffuse = "diffuse";
        public const string UV_specular = "specular";
        public readonly Dictionary<string, UVInfo> uvmaps;
        public static readonly string[] UVNames = new string[] { "diffuse", "bump", "specular", "alpha" };
        public readonly float[] vertex;
        public int vertexCount;

        public EntryGeometry(int limit)
        {
            this.Limit = Math.Max(limit, 0xffff);
            this.indices = new int[this.Limit * 3];
            this.vertex = new float[this.Limit * 3];
            this.normals = new float[this.Limit * 3];
            this.uvmaps = new Dictionary<string, UVInfo>(4);
            for (int i = 0; i < 4; i++)
            {
                string key = UVNames[i];
                this.uvmaps.Add(key, new UVInfo(key, this.Limit));
            }
        }

        public override void Read(PackFileStreamWriter pfw, PackEntryType tse)
        {
            throw new NotSupportedException();
        }

        private void Reset()
        {
            this.vertexCount = 0;
            this.triangleCount = 0;
            this.hasNormal = false;
            foreach (UVInfo info in this.uvmaps.Values)
            {
                info.IsValid = false;
            }
            this.attributes.Clear();
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            int num3;
            int n = this.uvmaps.Values.Count<UVInfo>(uvmap => uvmap.IsValid);
            int count = this.attributes.Count;
            PackFileStream stream = pfw.stream;
            stream.Write(0x4d54434f);
            stream.Write(5);
            stream.Write(0x574152);
            stream.Write(this.vertexCount);
            stream.Write(this.triangleCount);
            stream.Write(n);
            stream.Write(count);
            stream.Write(this.hasNormal ? 1 : 0);
            stream.Write(0);
            stream.Write(0x58444e49);
            for (num3 = 0; num3 < this.triangleCount; num3++)
            {
                stream.Write(this.indices[num3 * 3]);
                stream.Write(this.indices[(num3 * 3) + 1]);
                stream.Write(this.indices[(num3 * 3) + 2]);
            }
            this.box.makeEmpty();
            stream.Write(0x54524556);
            for (num3 = 0; num3 < this.vertexCount; num3++)
            {
                float num4 = this.transformPoint.x = this.vertex[num3 * 3];
                float num5 = this.transformPoint.y = this.vertex[(num3 * 3) + 1];
                float num6 = this.transformPoint.z = this.vertex[(num3 * 3) + 2];
                stream.Write(num4);
                stream.Write(num5);
                stream.Write(num6);
                this.transform.OfPointEx(this.transformPoint);
                this.box.min.x = Math.Min(this.box.min.x, this.transformPoint.x);
                this.box.min.y = Math.Min(this.box.min.y, this.transformPoint.y);
                this.box.min.z = Math.Min(this.box.min.z, this.transformPoint.z);
                this.box.max.x = Math.Max(this.box.max.x, this.transformPoint.x);
                this.box.max.y = Math.Max(this.box.max.y, this.transformPoint.y);
                this.box.max.z = Math.Max(this.box.max.z, this.transformPoint.z);
            }
            if (this.hasNormal)
            {
                stream.Write(0x4d524f4e);
                num3 = 0;
                while (num3 < this.vertexCount)
                {
                    stream.Write(this.normals[num3 * 3]);
                    stream.Write(this.normals[(num3 * 3) + 1]);
                    stream.Write(this.normals[(num3 * 3) + 2]);
                    num3++;
                }
            }
            foreach (UVInfo info in this.uvmaps.Values)
            {
                if (info.IsValid)
                {
                    stream.Write(0x43584554);
                    stream.WriteString(info.Name);
                    stream.WriteString(info.File);
                    num3 = 0;
                    while (num3 < this.vertexCount)
                    {
                        stream.Write(info.uvs[num3 * 2]);
                        stream.Write(info.uvs[(num3 * 2) + 1]);
                        num3++;
                    }
                }
            }
            if ((this.attributes != null) && (this.attributes.Count > 0))
            {
                foreach (string str in this.attributes.Keys)
                {
                    stream.Write(0x52545441);
                    stream.WriteString(str);
                    float[] numArray = this.attributes[str];
                    for (num3 = 0; num3 < this.vertexCount; num3++)
                    {
                        stream.Write(numArray[num3 * 4]);
                        stream.Write(numArray[(num3 * 4) + 1]);
                        stream.Write(numArray[(num3 * 4) + 2]);
                        stream.Write(numArray[(num3 * 4) + 3]);
                    }
                }
            }
            this.Reset();
        }
    }
}

