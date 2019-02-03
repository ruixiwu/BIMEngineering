using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry;
using BIM.Lmv.Content.Other;

namespace BIM.Lmv.Processers.Geometry
{
    internal class FileGeometryMetadata
    {
        public const string FILE_PATH = "GeometryMetadata.pf";
        private readonly EntryGeometryMetadata _EntryGeometryMetadata;
        private readonly PackFileOutput<EntryGeometryMetadata> _FileEntryGeometryMetadata;
        private readonly PackEntryType _PackEntryType;
        private readonly SvfFileProcesser _SvfFile;
        private readonly OutputProcesser _Output;

        public FileGeometryMetadata(OutputProcesser output, SvfFileProcesser svfFile)
        {
            _Output = output;
            _SvfFile = svfFile;
            _EntryGeometryMetadata = new EntryGeometryMetadata();
            _PackEntryType = new PackEntryType(0, "Autodesk.CloudPlatform.GeometryMetadata",
                "Autodesk.CloudPlatform.GeometryMetadataData", 3);
            _FileEntryGeometryMetadata = new PackFileOutput<EntryGeometryMetadata>(0x800000);
            _FileEntryGeometryMetadata.OnStart();
            _FileEntryGeometryMetadata.OnEntryType(_PackEntryType);
            PackEntryIndex = -1;
        }

        public int PackEntryIndex { get; private set; }

        public int OnAppendItem(string packFile, uint packEntityIndex, ushort primCount)
        {
            var entry = _EntryGeometryMetadata;
            entry.packFile = packFile;
            entry.entityIndex = packEntityIndex;
            entry.primCount = primCount;
            return _FileEntryGeometryMetadata.OnEntry(entry, _PackEntryType);
        }

        public void OnFinish()
        {
            var entry = new FileEntryStream("GeometryMetadata.pf");
            _FileEntryGeometryMetadata.OnFinish(entry.Stream);
            _Output.OnAppendFile(entry);
            var size = entry.GetSize();
            _SvfFile.OnEntry(new EntryAsset("GeometryMetadata.pf", "Autodesk.CloudPlatform.GeometryMetadataList",
                "GeometryMetadata.pf", size, 0, _SvfFile.AssetTypeGeometryMetadata));
        }
    }
}