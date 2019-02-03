using System;
using Ionic.Zip;

namespace BIM.Lmv.Processers
{
    internal abstract class FileEntry : IDisposable
    {
        public readonly string EntryName;

        protected FileEntry(string entryName)
        {
            EntryName = entryName;
        }

        public abstract void Dispose();
        public abstract void OnOutputToDisk(string path);
        public abstract void OnOutputToZip(ZipFile zip);

        public override string ToString()
        {
            return GetType().Name + "(" + EntryName + ")";
        }
    }
}