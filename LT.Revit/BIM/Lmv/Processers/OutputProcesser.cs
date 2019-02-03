using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BIM.Lmv.Types;
using Ionic.Zip;

namespace BIM.Lmv.Processers
{
    internal class OutputProcesser
    {
        private readonly List<FileEntry> _EntryList;
        private readonly ExportOption _ExportOption;
        private readonly string _TargetPath;

        public OutputProcesser(string targetPath, ExportOption option)
        {
            _TargetPath = targetPath;
            _ExportOption = option;
            _EntryList = new List<FileEntry>();
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
            if (_ExportOption.Target == ExportTarget.LocalFolder)
            {
                var path = Path.Combine(_TargetPath, entry.EntryName);
                entry.OnOutputToDisk(path);
                entry.Dispose();
            }
            else
            {
                _EntryList.Add(entry);
            }
        }

        public void OnFinish(Stream outputStream)
        {
            if (_ExportOption.Target == ExportTarget.LocalFolder)
            {
                throw new InvalidOperationException();
            }
            try
            {
                using (var file = new ZipFile(Encoding.UTF8))
                {
                    foreach (var entry in _EntryList)
                    {
                        entry.OnOutputToZip(file);
                    }
                    file.Save(outputStream);
                }
            }
            finally
            {
                foreach (var entry in _EntryList)
                {
                    entry.Dispose();
                }
                _EntryList.Clear();
            }
        }
    }
}