using System.IO;
using Ionic.Zip;

namespace BIM.Lmv.Processers
{
    internal class FileEntryFolderName : FileEntry
    {
        public FileEntryFolderName(string entryName) : base(entryName)
        {
        }

        public override void Dispose()
        {
        }

        public override void OnOutputToDisk(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public override void OnOutputToZip(ZipFile zip)
        {
            zip.AddDirectoryByName(EntryName);
        }
    }
}