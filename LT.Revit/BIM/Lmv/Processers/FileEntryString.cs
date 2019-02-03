using System.IO;
using System.Text;
using Ionic.Zip;

namespace BIM.Lmv.Processers
{
    internal class FileEntryString : FileEntry
    {
        private string _Content;

        public FileEntryString(string entryName, string content) : base(entryName)
        {
            _Content = content;
        }

        public override void Dispose()
        {
            _Content = null;
        }

        public override void OnOutputToDisk(string path)
        {
            if (_Content != null)
            {
                File.WriteAllText(path, _Content, Encoding.UTF8);
            }
        }

        public override void OnOutputToZip(ZipFile zip)
        {
            if (_Content != null)
            {
                zip.AddEntry(EntryName, _Content, Encoding.UTF8);
            }
        }
    }
}