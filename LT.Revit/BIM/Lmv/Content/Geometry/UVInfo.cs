namespace BIM.Lmv.Content.Geometry
{
    internal class UVInfo
    {
        public readonly string File;
        public readonly string Name;
        public readonly float[] uvs;
        public bool IsValid = false;

        public UVInfo(string name, int limit)
        {
            Name = name;
            File = "";
            uvs = new float[limit*2];
        }
    }
}