namespace BIM.Lmv.Processers.Helper
{
    internal class InstanceTemplate
    {
        public readonly string Key;
        public int Begin;
        public int End;

        public InstanceTemplate(string key)
        {
            Key = key;
            Begin = -1;
            End = -1;
        }
    }
}