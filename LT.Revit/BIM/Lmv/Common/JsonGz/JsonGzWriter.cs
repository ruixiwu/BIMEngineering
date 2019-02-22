namespace BIM.Lmv.Common.JsonGz
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    internal class JsonGzWriter : IDisposable
    {
        private Stream _FileStream;
        private GZipStream _GZipStream;
        private static readonly byte[] _LineBreak = Encoding.UTF8.GetBytes("\r\n");
        private bool _UseOutputStream;
        private BinaryWriter _Writer;

        public JsonGzWriter(Stream stream, bool useGzip)
        {
            this._FileStream = stream;
            if (useGzip)
            {
                this._GZipStream = new GZipStream(this._FileStream, System.IO.Compression.CompressionLevel.Optimal, true);
                this._Writer = new BinaryWriter(this._GZipStream, Encoding.UTF8, true);
            }
            else
            {
                this._Writer = new BinaryWriter(this._FileStream, Encoding.UTF8, true);
            }
            this._UseOutputStream = true;
        }

        public JsonGzWriter(string filePath, bool useGzip)
        {
            FileMode mode = File.Exists(filePath) ? FileMode.Truncate : FileMode.Create;
            this._FileStream = File.Open(filePath, mode, FileAccess.Write);
            if (useGzip)
            {
                this._GZipStream = new GZipStream(this._FileStream, System.IO.Compression.CompressionLevel.Optimal);
                this._Writer = new BinaryWriter(this._GZipStream, Encoding.UTF8);
            }
            else
            {
                this._Writer = new BinaryWriter(this._FileStream, Encoding.UTF8);
            }
        }

        public void Append(string s)
        {
            this._Writer.Write(Encoding.UTF8.GetBytes(s));
        }

        public void AppendLine()
        {
            this._Writer.Write(_LineBreak);
        }

        public void AppendLine(string s)
        {
            if (s != null)
            {
                this.Append(s);
            }
            this.AppendLine();
        }

        public void Dispose()
        {
            if (this._Writer != null)
            {
                this._Writer.Dispose();
                this._Writer = null;
            }
            if (this._GZipStream != null)
            {
                this._GZipStream.Dispose();
                this._GZipStream = null;
            }
            if (this._FileStream != null)
            {
                if (!this._UseOutputStream)
                {
                    this._FileStream.Dispose();
                }
                this._FileStream = null;
            }
        }

        public static void Save(string s, Stream outputStream, bool useGzip)
        {
            using (JsonGzWriter writer = new JsonGzWriter(outputStream, useGzip))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                writer.Writer.Write(bytes, 0, bytes.Length);
            }
        }

        public static void Save(string s, string filePath, bool useGzip)
        {
            using (JsonGzWriter writer = new JsonGzWriter(filePath, useGzip))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                writer.Writer.Write(bytes, 0, bytes.Length);
            }
        }

        public static void Save(StringBuilder sb, string filePath, bool useGzip)
        {
            using (JsonGzWriter writer = new JsonGzWriter(filePath, useGzip))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                writer.Writer.Write(bytes, 0, bytes.Length);
            }
        }

        public BinaryWriter Writer =>
            this._Writer;
    }
}

