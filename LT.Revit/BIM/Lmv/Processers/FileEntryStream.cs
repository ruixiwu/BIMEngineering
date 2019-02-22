namespace BIM.Lmv.Processers
{
    using Ionic.Zip;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal class FileEntryStream : FileEntry
    {
        public FileEntryStream(string entryName) : base(entryName)
        {
            this.Stream = new MemoryStream();
        }

        public FileEntryStream(string entryName, System.IO.Stream stream) : base(entryName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            this.Stream = stream;
        }

        public override void Dispose()
        {
            if (this.Stream != null)
            {
                this.Stream.Dispose();
                this.Stream = null;
            }
        }

        public int GetSize()
        {
            if (this.Stream == null)
            {
                return 0;
            }
            return (int) this.Stream.Position;
        }

        public override void OnOutputToDisk(string path)
        {
            if (this.Stream != null)
            {
                this.Stream.Seek(0L, SeekOrigin.Begin);
                using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write))
                {
                    this.Stream.CopyTo(stream);
                }
            }
        }

        public override void OnOutputToZip(ZipFile zip)
        {
            if (this.Stream != null)
            {
                this.Stream.Seek(0L, SeekOrigin.Begin);
                zip.AddEntry(base.EntryName, this.Stream);
            }
        }

        public System.IO.Stream Stream { get; private set; }
    }
}

