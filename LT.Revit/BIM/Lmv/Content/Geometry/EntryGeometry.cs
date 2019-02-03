using System;
using System.Collections.Generic;
using System.Linq;
using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry.Types;

namespace BIM.Lmv.Content.Geometry
{
    internal class EntryGeometry : PackEntryBase
    {
        public const string UV_alpha = "alpha";
        public const string UV_bump = "bump";
        public const string UV_diffuse = "diffuse";
        public const string UV_specular = "specular";
        public static readonly string[] UVNames = {"diffuse", "bump", "specular", "alpha"};
        public readonly Dictionary<string, float[]> attributes = new Dictionary<string, float[]>();
        public readonly int[] indices;
        public readonly int Limit;
        public readonly float[] normals;
        public readonly Dictionary<string, UVInfo> uvmaps;
        public readonly float[] vertex;
        public Box3F box = new Box3F();
        public bool hasNormal;
        public Transform transform;
        private readonly Vector3F transformPoint = new Vector3F();
        public int triangleCount;
        public int vertexCount;

        public EntryGeometry(int limit)
        {
            Limit = Math.Max(limit, 0xffff);
            indices = new int[Limit*3];
            vertex = new float[Limit*3];
            normals = new float[Limit*3];
            uvmaps = new Dictionary<string, UVInfo>(4);
            for (var i = 0; i < 4; i++)
            {
                var key = UVNames[i];
                uvmaps.Add(key, new UVInfo(key, Limit));
            }
        }

        public override void Read(PackFileStreamWriter pfw, PackEntryType tse)
        {
            throw new NotSupportedException();
        }

        private void Reset()
        {
            vertexCount = 0;
            triangleCount = 0;
            hasNormal = false;
            foreach (var info in uvmaps.Values)
            {
                info.IsValid = false;
            }
            attributes.Clear();
        }

        public override void Write(PackFileStreamWriter pfw, PackEntryType tse)
        {
            int num3;
            var n = uvmaps.Values.Count(uvmap => uvmap.IsValid);
            var count = attributes.Count;
            var stream = pfw.stream;
            stream.Write(0x4d54434f);
            stream.Write(5);
            stream.Write(0x574152);
            stream.Write(vertexCount);
            stream.Write(triangleCount);
            stream.Write(n);
            stream.Write(count);
            stream.Write(hasNormal ? 1 : 0);
            stream.Write(0);
            stream.Write(0x58444e49);
            for (num3 = 0; num3 < triangleCount; num3++)
            {
                stream.Write(indices[num3*3]);
                stream.Write(indices[num3*3 + 1]);
                stream.Write(indices[num3*3 + 2]);
            }
            box.makeEmpty();
            stream.Write(0x54524556);
            for (num3 = 0; num3 < vertexCount; num3++)
            {
                var num4 = transformPoint.x = vertex[num3*3];
                var num5 = transformPoint.y = vertex[num3*3 + 1];
                var num6 = transformPoint.z = vertex[num3*3 + 2];
                stream.Write(num4);
                stream.Write(num5);
                stream.Write(num6);
                transform.OfPointEx(transformPoint);
                box.min.x = Math.Min(box.min.x, transformPoint.x);
                box.min.y = Math.Min(box.min.y, transformPoint.y);
                box.min.z = Math.Min(box.min.z, transformPoint.z);
                box.max.x = Math.Max(box.max.x, transformPoint.x);
                box.max.y = Math.Max(box.max.y, transformPoint.y);
                box.max.z = Math.Max(box.max.z, transformPoint.z);
            }
            if (hasNormal)
            {
                stream.Write(0x4d524f4e);
                num3 = 0;
                while (num3 < vertexCount)
                {
                    stream.Write(normals[num3*3]);
                    stream.Write(normals[num3*3 + 1]);
                    stream.Write(normals[num3*3 + 2]);
                    num3++;
                }
            }
            foreach (var info in uvmaps.Values)
            {
                if (info.IsValid)
                {
                    stream.Write(0x43584554);
                    stream.WriteString(info.Name);
                    stream.WriteString(info.File);
                    num3 = 0;
                    while (num3 < vertexCount)
                    {
                        stream.Write(info.uvs[num3*2]);
                        stream.Write(info.uvs[num3*2 + 1]);
                        num3++;
                    }
                }
            }
            if ((attributes != null) && (attributes.Count > 0))
            {
                foreach (var str in attributes.Keys)
                {
                    stream.Write(0x52545441);
                    stream.WriteString(str);
                    var numArray = attributes[str];
                    for (num3 = 0; num3 < vertexCount; num3++)
                    {
                        stream.Write(numArray[num3*4]);
                        stream.Write(numArray[num3*4 + 1]);
                        stream.Write(numArray[num3*4 + 2]);
                        stream.Write(numArray[num3*4 + 3]);
                    }
                }
            }
            Reset();
        }
    }
}