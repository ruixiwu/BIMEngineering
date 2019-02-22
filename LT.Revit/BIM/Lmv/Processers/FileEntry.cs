namespace BIM.Lmv.Processers
{
    using Ionic.Zip;
    using System;

    internal abstract class FileEntry : IDisposable
    {
        public readonly string EntryName;
        protected FileEntry(string entryName)
        {
            this.EntryName = entryName;
        }

        public abstract void Dispose();
        public abstract void OnOutputToDisk(string path);
        public abstract void OnOutputToZip(ZipFile zip);
        public override string ToString() => 
            (base.GetType().Name + "(" + this.EntryName + ")");
    }
}

