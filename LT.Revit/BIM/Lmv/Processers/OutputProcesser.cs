namespace BIM.Lmv.Processers
{
    using BIM.Lmv.Types;
    using Ionic.Zip;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal class OutputProcesser
    {
        private readonly List<FileEntry> _EntryList;
        private readonly ExportOption _ExportOption;
        private readonly string _TargetPath;

        public OutputProcesser(string targetPath, ExportOption option)
        {
            this._TargetPath = targetPath;
            this._ExportOption = option;
            this._EntryList = new List<FileEntry>();
            if (!((option.Target != ExportTarget.LocalFolder) || Directory.Exists(targetPath)))
            {
                Directory.CreateDirectory(targetPath);
            }
        }

        public void OnAppendFile(FileEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }
            if (this._ExportOption.Target == ExportTarget.LocalFolder)
            {
                string path = Path.Combine(this._TargetPath, entry.EntryName);
                entry.OnOutputToDisk(path);
                entry.Dispose();
            }
            else
            {
                this._EntryList.Add(entry);
            }
        }

        public void OnFinish(Stream outputStream)
        {
            if (this._ExportOption.Target == ExportTarget.LocalFolder)
            {
                throw new InvalidOperationException();
            }
            try
            {
                using (ZipFile file = new ZipFile(Encoding.UTF8))
                {
                    foreach (FileEntry entry in this._EntryList)
                    {
                        entry.OnOutputToZip(file);
                    }
                    file.Save(outputStream);
                }
            }
            finally
            {
                foreach (FileEntry entry in this._EntryList)
                {
                    entry.Dispose();
                }
                this._EntryList.Clear();
            }
        }
    }
}

