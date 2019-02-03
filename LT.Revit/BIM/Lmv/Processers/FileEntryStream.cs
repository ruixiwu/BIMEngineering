using System;
using System.IO;
using Ionic.Zip;

namespace BIM.Lmv.Processers
{
    internal class FileEntryStream : FileEntry
    {
        public FileEntryStream(string entryName) : base(entryName)
        {
            Stream = new MemoryStream();
        }

        public FileEntryStream(string entryName, Stream stream) : base(entryName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            Stream = stream;
        }

        public Stream Stream { get; private set; }

        public override void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }
        }

        public int GetSize()
        {
            if (Stream == null)
            {
                return 0;
            }
            return (int) Stream.Position;
        }

        public override void OnOutputToDisk(string path)
        {
            if (Stream != null)
            {
                Stream.Seek(0L, SeekOrigin.Begin);
                using (var stream = File.Open(path, FileMode.Create, FileAccess.Write))
                {
                    Stream.CopyTo(stream);
                }
            }
        }

        public override void OnOutputToZip(ZipFile zip)
        {
            if (Stream != null)
            {
                Stream.Seek(0L, SeekOrigin.Begin);
                zip.AddEntry(EntryName, Stream);
            }
        }
    }
}