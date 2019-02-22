namespace BIM.Lmv.Common.Pack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.CompilerServices;

    internal class PackFileOutput<TEntry> where TEntry: PackEntryBase
    {
        private readonly MemoryStream _FileStream;
        private readonly List<uint> entryOffsets;
        private readonly List<PackEntryType> entryTypes;
        private const string PACK_TYPE = "Autodesk.CloudPlatform.PackFile";
        private const int PACK_VERSION = 2;
        private PackFileStreamWriter pfw;
        private PackFileStream stream;

        public PackFileOutput(int initCapacity)
        {
            this.entryTypes = new List<PackEntryType>(0x10);
            this.entryOffsets = new List<uint>(0x200);
            int capacity = Math.Min(0x2000000, Math.Max(0x4000, initCapacity));
            this._FileStream = new MemoryStream(capacity);
        }

        public int OnEntry(TEntry entry, PackEntryType entryType)
        {
            if (!this.IsRunning)
            {
                throw new NotSupportedException();
            }
            int count = this.entryOffsets.Count;
            this.entryOffsets.Add((uint) this.stream.offset);
            this.stream.Write(entryType.index);
            entry.Write(this.pfw, entryType);
            return count;
        }

        public int OnEntryType(PackEntryType entryType)
        {
            if (!this.IsRunning)
            {
                throw new NotSupportedException();
            }
            if (entryType == null)
            {
                throw new ArgumentNullException("entryType");
            }
            entryType.index = this.entryTypes.Count;
            this.entryTypes.Add(entryType);
            return entryType.index;
        }

        public void OnFinish(Stream outputStream)
        {
            if (!this.IsRunning)
            {
                throw new NotSupportedException();
            }
            int offset = (int) this.stream.offset;
            if (this.entryTypes.Count > 0)
            {
                this.stream.WriteVarints((uint) this.entryTypes.Count);
                foreach (PackEntryType type in this.entryTypes)
                {
                    type.Write(this.pfw);
                }
            }
            int n = (int) this.stream.offset;
            this.stream.WriteVarints((uint) this.entryOffsets.Count);
            foreach (uint num3 in this.entryOffsets)
            {
                this.stream.Write(num3);
            }
            this.stream.Write(n);
            this.stream.Write(offset);
            System.IO.Compression.CompressionLevel optimal = System.IO.Compression.CompressionLevel.Optimal;
            using (GZipStream stream = new GZipStream(outputStream, optimal, true))
            {
                this._FileStream.Position = 0L;
                this._FileStream.CopyTo(stream);
            }
            this.Reset();
        }

        public void OnStart()
        {
            if (this.IsRunning)
            {
                throw new NotSupportedException();
            }
            this.Reset();
            this.pfw = new PackFileStreamWriter(this._FileStream);
            this.stream = this.pfw.stream;
            this.stream.WriteString("Autodesk.CloudPlatform.PackFile");
            this.stream.Write(2);
            this.IsRunning = true;
        }

        public void ReadEntry(int index, TEntry entry)
        {
            long position = this._FileStream.Position;
            this._FileStream.Seek((long) ((ulong) this.entryOffsets[index]), SeekOrigin.Begin);
            int num2 = this.pfw.stream.getInt32();
            PackEntryType tse = this.entryTypes[num2];
            entry.Read(this.pfw, tse);
            this._FileStream.Seek(position, SeekOrigin.Begin);
        }

        private void Reset()
        {
            this._FileStream.Position = 0L;
            this._FileStream.SetLength(0L);
            this.entryTypes.Clear();
            this.entryOffsets.Clear();
            this.stream = null;
            this.pfw = null;
            this.IsRunning = false;
        }

        public bool IsRunning { get; private set; }

        public int Position =>
            ((int) this.stream.offset);
    }
}

