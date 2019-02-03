using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace BIM.Lmv.Common.Pack
{
    internal class PackFileOutput<TEntry> where TEntry : PackEntryBase
    {
        private const string PACK_TYPE = "Autodesk.CloudPlatform.PackFile";
        private const int PACK_VERSION = 2;
        private readonly MemoryStream _FileStream;
        private readonly List<uint> entryOffsets;
        private readonly List<PackEntryType> entryTypes;
        private PackFileStreamWriter pfw;
        private PackFileStream stream;

        public PackFileOutput(int initCapacity)
        {
            entryTypes = new List<PackEntryType>(0x10);
            entryOffsets = new List<uint>(0x200);
            var capacity = Math.Min(0x2000000, Math.Max(0x4000, initCapacity));
            _FileStream = new MemoryStream(capacity);
        }

        public bool IsRunning { get; private set; }

        public int Position
        {
            get { return (int) stream.offset; }
        }

        public int OnEntry(TEntry entry, PackEntryType entryType)
        {
            if (!IsRunning)
            {
                throw new NotSupportedException();
            }
            var count = entryOffsets.Count;
            entryOffsets.Add((uint) stream.offset);
            stream.Write(entryType.index);
            entry.Write(pfw, entryType);
            return count;
        }

        public int OnEntryType(PackEntryType entryType)
        {
            if (!IsRunning)
            {
                throw new NotSupportedException();
            }
            if (entryType == null)
            {
                throw new ArgumentNullException("entryType");
            }
            entryType.index = entryTypes.Count;
            entryTypes.Add(entryType);
            return entryType.index;
        }

        public void OnFinish(Stream outputStream)
        {
            if (!IsRunning)
            {
                throw new NotSupportedException();
            }
            var offset = (int) this.stream.offset;
            if (entryTypes.Count > 0)
            {
                stream.WriteVarints((uint) entryTypes.Count);
                foreach (var type in entryTypes)
                {
                    type.Write(pfw);
                }
            }
            var n = (int) this.stream.offset;
            this.stream.WriteVarints((uint) entryOffsets.Count);
            foreach (var num3 in entryOffsets)
            {
                stream.Write(num3);
            }
            this.stream.Write(n);
            this.stream.Write(offset);
            var optimal = CompressionLevel.Optimal;
            using (var stream = new GZipStream(outputStream, optimal, true))
            {
                _FileStream.Position = 0L;
                _FileStream.CopyTo(stream);
            }
            Reset();
        }

        public void OnStart()
        {
            if (IsRunning)
            {
                throw new NotSupportedException();
            }
            Reset();
            pfw = new PackFileStreamWriter(_FileStream);
            stream = pfw.stream;
            stream.WriteString("Autodesk.CloudPlatform.PackFile");
            stream.Write(2);
            IsRunning = true;
        }

        public void ReadEntry(int index, TEntry entry)
        {
            var position = _FileStream.Position;
            _FileStream.Seek(entryOffsets[index], SeekOrigin.Begin);
            var num2 = pfw.stream.getInt32();
            var tse = entryTypes[num2];
            entry.Read(pfw, tse);
            _FileStream.Seek(position, SeekOrigin.Begin);
        }

        private void Reset()
        {
            _FileStream.Position = 0L;
            _FileStream.SetLength(0L);
            entryTypes.Clear();
            entryOffsets.Clear();
            stream = null;
            pfw = null;
            IsRunning = false;
        }
    }
}