using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Other;

namespace BIM.Lmv.Processers.Geometry
{
    internal class FileLight
    {
        public const string FILE_PATH_LIGHT_DEFINITIONS = "LightDefinitions.bin";
        public const string FILE_PATH_LIGHT_LIST = "LightList.bin";
        private readonly EntryInstance _EntryInstance;
        private readonly EntryLight _EntryLight;
        private readonly PackFileOutput<EntryInstance> _FileEntryInstance;
        private readonly PackFileOutput<EntryLight> _FileEntryLight;
        private readonly PackEntryType _PackEntryTypeInstance;
        private readonly PackEntryType _PackEntryTypeLight;
        private readonly OutputProcesser _Output;
        private readonly SvfFileProcesser _SvfFile;

        public FileLight(OutputProcesser output, SvfFileProcesser svfFile)
        {
            _Output = output;
            _SvfFile = svfFile;
            _EntryLight = new EntryLight();
            _PackEntryTypeLight = new PackEntryType(0, "Autodesk.CloudPlatform.Light",
                "Autodesk.CloudPlatform.LightDefinition", 2);
            _FileEntryLight = new PackFileOutput<EntryLight>(0x1000);
            _FileEntryLight.OnStart();
            _FileEntryLight.OnEntryType(_PackEntryTypeLight);
            _EntryInstance = new EntryInstance();
            _PackEntryTypeInstance = new PackEntryType(0, "Autodesk.CloudPlatform.Instance",
                "Autodesk.CloudPlatform.InstanceData", 2);
            _FileEntryInstance = new PackFileOutput<EntryInstance>(0x1000);
            _FileEntryInstance.OnStart();
            _FileEntryInstance.OnEntryType(_PackEntryTypeInstance);
        }

        public void OnFinish()
        {
            var entry = new FileEntryStream("LightList.bin");
            var stream2 = new FileEntryStream("LightDefinitions.bin");
            _FileEntryLight.OnFinish(entry.Stream);
            _FileEntryInstance.OnFinish(stream2.Stream);
            _Output.OnAppendFile(entry);
            _Output.OnAppendFile(stream2);
            var size = entry.GetSize();
            var num2 = stream2.GetSize();
            _SvfFile.OnEntry(new EntryAsset("LightDefinitions.bin", "Autodesk.CloudPlatform.PackFile",
                "LightDefinitions.bin", num2, 0, _SvfFile.AssetTypeLight));
            _SvfFile.OnEntry(new EntryAsset("LightList.bin", "Autodesk.CloudPlatform.PackFile", "LightList.bin", size, 0,
                _SvfFile.AssetTypeInstance));
        }
    }
}