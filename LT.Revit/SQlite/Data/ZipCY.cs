namespace SQlite.Data
{
    using ICSharpCode.SharpZipLib.Zip;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    public class ZipCY
    {
        private static void AddZipEntry(string name, string rootPath, ZipOutputStream zos1, out ZipOutputStream zos2)
        {
            ZipEntry entry = null;
            if (Directory.Exists(name))
            {
                DirectoryInfo info = new DirectoryInfo(name);
                foreach (DirectoryInfo info2 in info.GetDirectories())
                {
                    entry = new ZipEntry(GetFilePath(info2.FullName, rootPath) + "/");
                    zos1.PutNextEntry(entry);
                    AddZipEntry(info2.FullName, rootPath, zos1, out zos1);
                }
                foreach (FileInfo info3 in info.GetFiles())
                {
                    AddZipEntry(info3.FullName, rootPath, zos1, out zos2);
                }
            }
            if (File.Exists(name))
            {
                zos1.SetLevel(9);
                FileStream stream = File.OpenRead(name);
                int count = 0;
                byte[] buffer = new byte[0x800];
                entry = new ZipEntry(GetFilePath(name, rootPath + "/"));
                zos1.PutNextEntry(entry);
                while ((count = stream.Read(buffer, 0, 0x800)) != 0)
                {
                    zos1.Write(buffer, 0, count);
                }
                stream.Close();
            }
            zos2 = zos1;
        }

        public static string GetFilePath(string file) => 
            file.Replace('/', '\\').Substring(0, file.LastIndexOf('\\') + 1);

        private static string GetFilePath(string filePath, string rootPath)
        {
            rootPath = rootPath.Replace(@"\", "/");
            filePath = filePath.Replace(@"\", "/");
            return filePath.Replace(rootPath, "");
        }

        public static string UnZipFile(string zipFile, string dePath)
        {
            string message = "";
            try
            {
                if (!File.Exists(zipFile))
                {
                    throw new Exception("待解压文件不存在！");
                }
                if (!Directory.Exists(dePath))
                {
                    Directory.CreateDirectory(dePath);
                }
                ZipInputStream stream = new ZipInputStream(File.OpenRead(zipFile));
                ZipEntry entry = null;
                string path = "";
                string str3 = "";
                FileStream stream2 = null;
                byte[] buffer = new byte[0x800];
                int count = 0;
                while ((entry = stream.GetNextEntry()) != null)
                {
                    if (entry.IsDirectory)
                    {
                        path = dePath + @"\" + entry.Name.Substring(0, entry.Name.LastIndexOf("/"));
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    }
                    else if (entry.Crc != 0L)
                    {
                        str3 = dePath + @"\" + entry.Name;
                        path = Path.GetDirectoryName(str3);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        stream2 = File.Open(str3, FileMode.Create, FileAccess.Write);
                        while ((count = stream.Read(buffer, 0, 0x800)) != 0)
                        {
                            stream2.Write(buffer, 0, count);
                        }
                        stream2.Close();
                    }
                }
                stream.Close();
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }
            return message;
        }
        //压缩目录为一个文件
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

        public static string ZipDirectory(string directory, string zipName)
        {
            directory = directory.Replace(@"\", "/");
            zipName = zipName.Replace(@"\", "/");
            string message = "";
            try
            {
                if (!Directory.Exists(directory))
                {
                    throw new Exception("指定的压缩目录不存在！");
                }
                ZipOutputStream stream = new ZipOutputStream(File.Create(zipName));
                DirectoryInfo info = new DirectoryInfo(directory);
                foreach (DirectoryInfo info2 in info.GetDirectories())
                {
                    AddZipEntry(info2.FullName, directory, stream, out stream);
                }
                foreach (FileInfo info3 in info.GetFiles())
                {
                    AddZipEntry(info3.FullName, directory, stream, out stream);
                }
                stream.Close();
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }
            return message;
        }

        public static string ZipFile(string srcName, string zipName)
        {
            srcName = new Regex(@"[\/]+").Replace(srcName, "/");
            zipName = new Regex(@"[\/]+").Replace(zipName, "/");
            string message = "";
            try
            {
                if (!File.Exists(srcName))
                {
                    throw new Exception("指定的压缩文件不存在！");
                }
                ZipOutputStream stream = new ZipOutputStream(File.Create(zipName));
                AddZipEntry(srcName, srcName.Substring(0, srcName.LastIndexOf("/")), stream, out stream);
                stream.Close();
            }
            catch (Exception exception)
            {
                message = exception.Message;
            }
            return message;
        }
    }
}

