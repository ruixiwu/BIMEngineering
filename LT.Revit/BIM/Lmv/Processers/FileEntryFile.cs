using System.IO;
using Ionic.Zip;

namespace BIM.Lmv.Processers
{
    internal class FileEntryFile : FileEntry
    {
        private string _FilePath;
        private Stream _Stream;

        public FileEntryFile(string entryName, string filePath) : base(entryName)
        {
            _FilePath = filePath;
        }

        public override void Dispose()
        {
            _FilePath = null;
            if (_Stream != null)
            {
                _Stream.Dispose();
                _Stream = null;
            }
        }

        public override void OnOutputToDisk(string path)
        {
            if ((_FilePath != null) && File.Exists(_FilePath) && !File.Exists(path))
            {
                File.Copy(_FilePath, path, true);
            }
        }

        public override void OnOutputToZip(ZipFile zip)
        {
            if ((_FilePath != null) && File.Exists(_FilePath))
            {
                _Stream = File.OpenRead(_FilePath);
                zip.AddEntry(EntryName, _Stream);
            }
        }
    }
}