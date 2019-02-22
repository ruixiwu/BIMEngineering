namespace SQlite.Data
{
    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Zip;
    using System;
    using System.IO;

    public class ZipHelper
    {
        public static void test()
        {
            string folderToZip = "c:/BIM-SSH";
            string zipedFolder = "d:/BIM-SSH";
            string zipedFile = "d:/BIMTest.zip";
            ZipDirectory(folderToZip, zipedFile);
            UnZip(zipedFile, zipedFolder);
        }

        public static void test11()
        {
            string folderToZip = "d:/9999/222/BIM-SSH";
            string zipedFile = "d:/B7IM-SSH.zip";
            ZipDirectory(folderToZip, zipedFile);
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

        public static bool Zip(string fileToZip, string zipedFile) => 
            Zip(fileToZip, zipedFile, null);

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

        public static string ZipDir(string sDir, string sZipath, string sZipName)
        {
            if (!Directory.Exists(sZipath))
            {
                Directory.CreateDirectory(sZipath);
            }
            string path = sZipath + @"\" + sZipName + ".zip";
            if (File.Exists(path))
            {
                string str2 = "";
                for (int i = 1; i < 100; i++)
                {
                    str2 = sZipath + @"\" + sZipName + i.ToString() + ".zip";
                    if (!File.Exists(str2))
                    {
                        break;
                    }
                }
                File.Move(path, str2);
            }
            ZipDirectory(sDir, path);
            return path;
        }

        public static bool ZipDirectory(string folderToZip, string zipedFile) => 
            ZipDirectory(folderToZip, zipedFile, null);

        private static bool ZipDirectory(string folderToZip, ZipOutputStream zipStream, string parentFolderName)
        {
            bool flag = true;
            ZipEntry entry = null;
            FileStream stream = null;
            Crc32 crc = new Crc32();
            try
            {
                entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/"));
                zipStream.PutNextEntry(entry);
                zipStream.Flush();
                foreach (string str in Directory.GetFiles(folderToZip))
                {
                    stream = File.OpenRead(str);
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/" + Path.GetFileName(str))) {
                        DateTime = DateTime.Now,
                        Size = stream.Length
                    };
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
            foreach (string str2 in Directory.GetDirectories(folderToZip))
            {
                if (!ZipDirectory(str2, zipStream, folderToZip))
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

