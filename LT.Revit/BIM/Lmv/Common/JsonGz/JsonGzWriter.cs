using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BIM.Lmv.Common.JsonGz
{
    internal class JsonGzWriter : IDisposable
    {
        private static readonly byte[] _LineBreak = Encoding.UTF8.GetBytes("\r\n");
        private Stream _FileStream;
        private GZipStream _GZipStream;
        private readonly bool _UseOutputStream;

        public JsonGzWriter(Stream stream, bool useGzip)
        {
            _FileStream = stream;
            if (useGzip)
            {
                _GZipStream = new GZipStream(_FileStream, CompressionLevel.Optimal, true);
                Writer = new BinaryWriter(_GZipStream, Encoding.UTF8, true);
            }
            else
            {
                Writer = new BinaryWriter(_FileStream, Encoding.UTF8, true);
            }
            _UseOutputStream = true;
        }

        public JsonGzWriter(string filePath, bool useGzip)
        {
            var mode = File.Exists(filePath) ? FileMode.Truncate : FileMode.Create;
            _FileStream = File.Open(filePath, mode, FileAccess.Write);
            if (useGzip)
            {
                _GZipStream = new GZipStream(_FileStream, CompressionLevel.Optimal);
                Writer = new BinaryWriter(_GZipStream, Encoding.UTF8);
            }
            else
            {
                Writer = new BinaryWriter(_FileStream, Encoding.UTF8);
            }
        }

        public BinaryWriter Writer { get; private set; }

        public void Dispose()
        {
            if (Writer != null)
            {
                Writer.Dispose();
                Writer = null;
            }
            if (_GZipStream != null)
            {
                _GZipStream.Dispose();
                _GZipStream = null;
            }
            if (_FileStream != null)
            {
                if (!_UseOutputStream)
                {
                    _FileStream.Dispose();
                }
                _FileStream = null;
            }
        }

        public void Append(string s)
        {
            Writer.Write(Encoding.UTF8.GetBytes(s));
        }

        public void AppendLine()
        {
            Writer.Write(_LineBreak);
        }

        public void AppendLine(string s)
        {
            if (s != null)
            {
                Append(s);
            }
            AppendLine();
        }

        public static void Save(string s, Stream outputStream, bool useGzip)
        {
            using (var writer = new JsonGzWriter(outputStream, useGzip))
            {
                var bytes = Encoding.UTF8.GetBytes(s);
                writer.Writer.Write(bytes, 0, bytes.Length);
            }
        }

        public static void Save(string s, string filePath, bool useGzip)
        {
            using (var writer = new JsonGzWriter(filePath, useGzip))
            {
                var bytes = Encoding.UTF8.GetBytes(s);
                writer.Writer.Write(bytes, 0, bytes.Length);
            }
        }

        public static void Save(StringBuilder sb, string filePath, bool useGzip)
        {
            using (var writer = new JsonGzWriter(filePath, useGzip))
            {
                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                writer.Writer.Write(bytes, 0, bytes.Length);
            }
        }
    }
}