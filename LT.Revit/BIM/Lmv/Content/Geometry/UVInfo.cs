namespace BIM.Lmv.Content.Geometry
{
    using System;

    internal class UVInfo
    {
        public readonly string File;
        public bool IsValid = false;
        public readonly string Name;
        public readonly float[] uvs;

        public UVInfo(string name, int limit)
        {
            this.Name = name;
            this.File = "";
            this.uvs = new float[limit * 2];
        }
    }
}

