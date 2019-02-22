namespace BIM.Lmv.Processers
{
    using Ionic.Zip;
    using System;
    using System.IO;
    using System.Text;

    internal class FileEntryString : FileEntry
    {
        private string _Content;

        public FileEntryString(string entryName, string content) : base(entryName)
        {
            this._Content = content;
        }

        public override void Dispose()
        {
            this._Content = null;
        }

        public override void OnOutputToDisk(string path)
        {
            if (this._Content != null)
            {
                File.WriteAllText(path, this._Content, Encoding.UTF8);
            }
        }

        public override void OnOutputToZip(ZipFile zip)
        {
            if (this._Content != null)
            {
                zip.AddEntry(base.EntryName, this._Content, Encoding.UTF8);
            }
        }
    }
}

