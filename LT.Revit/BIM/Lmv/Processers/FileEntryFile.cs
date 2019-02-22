namespace BIM.Lmv.Processers
{
    using Ionic.Zip;
    using System;
    using System.IO;

    internal class FileEntryFile : FileEntry
    {
        private string _FilePath;
        private Stream _Stream;

        public FileEntryFile(string entryName, string filePath) : base(entryName)
        {
            this._FilePath = filePath;
        }

        public override void Dispose()
        {
            this._FilePath = null;
            if (this._Stream != null)
            {
                this._Stream.Dispose();
                this._Stream = null;
            }
        }

        public override void OnOutputToDisk(string path)
        {
            if (((this._FilePath != null) && File.Exists(this._FilePath)) && !File.Exists(path))
            {
                File.Copy(this._FilePath, path, true);
            }
        }

        public override void OnOutputToZip(ZipFile zip)
        {
            if ((this._FilePath != null) && File.Exists(this._FilePath))
            {
                this._Stream = File.OpenRead(this._FilePath);
                zip.AddEntry(base.EntryName, this._Stream);
            }
        }
    }
}

