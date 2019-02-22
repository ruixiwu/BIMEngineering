namespace BIM.Lmv.Processers.Helper
{
    using System;

    internal class InstanceTemplate
    {
        public int Begin;
        public int End;
        public readonly string Key;

        public InstanceTemplate(string key)
        {
            this.Key = key;
            this.Begin = -1;
            this.End = -1;
        }
    }
}

