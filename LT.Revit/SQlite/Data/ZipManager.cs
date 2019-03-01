namespace SQlite.Data
{
    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Zip;
    using System;
    using System.IO;

    public class ZipManager
    {
        private static string getReplace(string folderToZip) => 
            folderToZip.Replace("d:/", "");

        public void test()
        {
            string zipedFolder = "d:/BIM-SSH-11";
            string fileToUnZip = @"e:\test.zip";
            UnZip(fileToUnZip, zipedFolder);
        }

        public static bool UnZip(string fileToUnZip, string zipedFolder) => 
            UnZip(fileToUnZip, zipedFolder, null);

        public static bool UnZip(string fileToUnZip, string zipedFolder, string password)
        {
            bool flag = true;
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
                        string path = Path.Combine(zipedFolder, entry.Name).Replace('/', '\\');
                        if (path.EndsWith(@"\"))
                        {
                            Directory.CreateDirectory(path);
                        }
                        else
                        {
                            stream = File.Create(path);
                            int num = 0x800;
                            byte[] buffer = new byte[num];
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

        public static bool Zip(string fileToZip, string zipedFile) => Zip(fileToZip, zipedFile, null);

        public static bool Zip(string fileToZip, string zipedFile, string password)
        {
            bool flag = false;
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

        public static bool ZipDirectory(string folderToZip, string zipedFile) => ZipDirectory(folderToZip, zipedFile, null);

        private static bool ZipDirectory(string folderToZip, ZipOutputStream zipStream, string parentFolderName)
        {
            bool flag = true;
            ZipEntry entry = null;
            FileStream stream = null;
            Crc32 crc = new Crc32();
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
                foreach (string itemToZip in Directory.GetFiles(folderToZip))
                {
                    stream = File.OpenRead(itemToZip);
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    if (parentFolderName == "")
                    {
                        entry = new ZipEntry(getReplace(Path.Combine(new string[] { folderToZip + "/" + Path.GetFileName(itemToZip) })));
                    }
                    else
                    {
                        entry = new ZipEntry(getReplace(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/" + Path.GetFileName(itemToZip))));
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
            foreach (string itemToZip in Directory.GetDirectories(folderToZip))
            {
                if (!ZipDirectory(itemToZip, zipStream, folderToZip))
                {
                    return false;
                }
            }
            return flag;
        }

        public static bool ZipDirectory(string folderToZip, string zipedFile, string password)
        {
            bool flag = false;
            if (Directory.Exists(folderToZip))
            {
                ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile));
                zipStream.SetLevel(6);
                if (!string.IsNullOrEmpty(password))
                {
                    zipStream.Password = password;
                }
                flag = ZipDirectory(folderToZip, zipStream, "");
                zipStream.Finish();
                zipStream.Close();
            }
            Console.WriteLine("压缩svfzip文件成功完成");
            return flag;
        }

        public static bool ZipFile(string fileToZip, string zipedFile) => 
            ZipFile(fileToZip, zipedFile, null);

        public static bool ZipFile(string fileToZip, string zipedFile, string password)
        {
            bool flag = true;
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
                byte[] buffer = new byte[baseOutputStream.Length];
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

