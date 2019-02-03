using System;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

namespace SQlite.Data
{
    public class ZipManager
    {
        private static string getReplace(string folderToZip)
        {
            return folderToZip.Replace("d:/", "");
        }

        public void test()
        {
            var zipedFolder = "d:/BIM-SSH-11";
            var fileToUnZip = @"e:\test.zip";
            UnZip(fileToUnZip, zipedFolder);
        }

        public static bool UnZip(string fileToUnZip, string zipedFolder)
        {
            return UnZip(fileToUnZip, zipedFolder, null);
        }

        public static bool UnZip(string fileToUnZip, string zipedFolder, string password)
        {
            var flag = true;
            FileStream stream = null;
            ZipInputStream stream2 = null;
            ZipEntry entry = null;
            if (!File.Exists(fileToUnZip))
            {
                return false;
            }
            if (!Directory.Exists(zipedFolder))
            {
                Directory.CreateDirectory(zipedFolder);
            }
            try
            {
                stream2 = new ZipInputStream(File.OpenRead(fileToUnZip));
                if (!string.IsNullOrEmpty(password))
                {
                    stream2.Password = password;
                }
                while ((entry = stream2.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(entry.Name))
                    {
                        var path = Path.Combine(zipedFolder, entry.Name).Replace('/', '\\');
                        if (path.EndsWith(@"\"))
                        {
                            Directory.CreateDirectory(path);
                        }
                        else
                        {
                            stream = File.Create(path);
                            var num = 0x800;
                            var buffer = new byte[num];
                            while (stream2.Read(buffer, 0, buffer.Length) > 0)
                            {
                                stream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }
            }
            catch
            {
                flag = false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
                if (stream2 != null)
                {
                    stream2.Close();
                    stream2.Dispose();
                }
                if (entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return flag;
        }

        public static bool Zip(string fileToZip, string zipedFile)
        {
            return Zip(fileToZip, zipedFile, null);
        }

        public static bool Zip(string fileToZip, string zipedFile, string password)
        {
            var flag = false;
            if (Directory.Exists(fileToZip))
            {
                return ZipDirectory(fileToZip, zipedFile, password);
            }
            if (File.Exists(fileToZip))
            {
                flag = ZipFile(fileToZip, zipedFile, password);
            }
            return flag;
        }

        public static bool ZipDirectory(string folderToZip, string zipedFile)
        {
            return ZipDirectory(folderToZip, zipedFile, null);
        }

        private static bool ZipDirectory(string folderToZip, ZipOutputStream zipStream, string parentFolderName)
        {
            var flag = true;
            ZipEntry entry = null;
            FileStream stream = null;
            var crc = new Crc32();
            try
            {
                if (parentFolderName == "")
                {
                    entry = new ZipEntry(getReplace(folderToZip + "/"));
                    zipStream.PutNextEntry(entry);
                    zipStream.Flush();
                }
                else
                {
                    entry = new ZipEntry(getReplace(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/")));
                    zipStream.PutNextEntry(entry);
                    zipStream.Flush();
                }
                foreach (var str3 in Directory.GetFiles(folderToZip))
                {
                    stream = File.OpenRead(str3);
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    if (parentFolderName == "")
                    {
                        entry =
                            new ZipEntry(
                                getReplace(Path.Combine(folderToZip + "/" + Path.GetFileName(str3))));
                    }
                    else
                    {
                        entry =
                            new ZipEntry(
                                getReplace(Path.Combine(parentFolderName,
                                    Path.GetFileName(folderToZip) + "/" + Path.GetFileName(str3))));
                    }
                    entry.DateTime = DateTime.Now;
                    entry.Size = stream.Length;
                    stream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);
                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                flag = false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
                if (entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            foreach (var str6 in Directory.GetDirectories(folderToZip))
            {
                if (!ZipDirectory(str6, zipStream, folderToZip))
                {
                    return false;
                }
            }
            return flag;
        }

        public static bool ZipDirectory(string folderToZip, string zipedFile, string password)
        {
            var flag = false;
            if (Directory.Exists(folderToZip))
            {
                var zipStream = new ZipOutputStream(File.Create(zipedFile));
                zipStream.SetLevel(6);
                if (!string.IsNullOrEmpty(password))
                {
                    zipStream.Password = password;
                }
                flag = ZipDirectory(folderToZip, zipStream, "");
                zipStream.Finish();
                zipStream.Close();
            }
            return flag;
        }

        public static bool ZipFile(string fileToZip, string zipedFile)
        {
            return ZipFile(fileToZip, zipedFile, null);
        }

        public static bool ZipFile(string fileToZip, string zipedFile, string password)
        {
            var flag = true;
            ZipOutputStream stream = null;
            FileStream baseOutputStream = null;
            ZipEntry entry = null;
            if (!File.Exists(fileToZip))
            {
                return false;
            }
            try
            {
                baseOutputStream = File.OpenRead(fileToZip);
                var buffer = new byte[baseOutputStream.Length];
                baseOutputStream.Read(buffer, 0, buffer.Length);
                baseOutputStream.Close();
                baseOutputStream = File.Create(zipedFile);
                stream = new ZipOutputStream(baseOutputStream);
                if (!string.IsNullOrEmpty(password))
                {
                    stream.Password = password;
                }
                entry = new ZipEntry(Path.GetFileName(fileToZip));
                stream.PutNextEntry(entry);
                stream.SetLevel(6);
                stream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                flag = false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Finish();
                    stream.Close();
                }
                if (entry != null)
                {
                    entry = null;
                }
                if (baseOutputStream != null)
                {
                    baseOutputStream.Close();
                    baseOutputStream.Dispose();
                }
            }
            GC.Collect();
            GC.Collect(1);
            return flag;
        }
    }
}