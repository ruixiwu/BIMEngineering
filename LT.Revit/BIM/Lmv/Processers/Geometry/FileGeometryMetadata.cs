namespace BIM.Lmv.Processers.Geometry
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry;
    using BIM.Lmv.Content.Other;
    using BIM.Lmv.Processers;
    using System;
    using System.Runtime.CompilerServices;

    internal class FileGeometryMetadata
    {
        private readonly EntryGeometryMetadata _EntryGeometryMetadata;
        private readonly PackFileOutput<EntryGeometryMetadata> _FileEntryGeometryMetadata;
        private OutputProcesser _Output;
        private readonly PackEntryType _PackEntryType;
        private readonly SvfFileProcesser _SvfFile;
        public const string FILE_PATH = "GeometryMetadata.pf";

        public FileGeometryMetadata(OutputProcesser output, SvfFileProcesser svfFile)
        {
            this._Output = output;
            this._SvfFile = svfFile;
            this._EntryGeometryMetadata = new EntryGeometryMetadata();
            this._PackEntryType = new PackEntryType(0, "Autodesk.CloudPlatform.GeometryMetadata", "Autodesk.CloudPlatform.GeometryMetadataData", 3);
            this._FileEntryGeometryMetadata = new PackFileOutput<EntryGeometryMetadata>(0x800000);
            this._FileEntryGeometryMetadata.OnStart();
            this._FileEntryGeometryMetadata.OnEntryType(this._PackEntryType);
            this.PackEntryIndex = -1;
        }

        public int OnAppendItem(string packFile, uint packEntityIndex, ushort primCount)
        {
            EntryGeometryMetadata entry = this._EntryGeometryMetadata;
            entry.packFile = packFile;
            entry.entityIndex = packEntityIndex;
            entry.primCount = primCount;
            return this._FileEntryGeometryMetadata.OnEntry(entry, this._PackEntryType);
        }

        public void OnFinish()
        {
            FileEntryStream entry = new FileEntryStream("GeometryMetadata.pf");
            this._FileEntryGeometryMetadata.OnFinish(entry.Stream);
            this._Output.OnAppendFile(entry);
            int size = entry.GetSize();
            this._SvfFile.OnEntry(new EntryAsset("GeometryMetadata.pf", "Autodesk.CloudPlatform.GeometryMetadataList", "GeometryMetadata.pf", new int?(size), 0, this._SvfFile.AssetTypeGeometryMetadata));
        }

        public int PackEntryIndex { get; private set; }
    }
}

