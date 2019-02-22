namespace BIM.Lmv.Processers.Geometry
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Other;
    using BIM.Lmv.Processers;
    using System;

    internal class FileLight
    {
        private readonly EntryInstance _EntryInstance;
        private readonly EntryLight _EntryLight;
        private readonly PackFileOutput<EntryInstance> _FileEntryInstance;
        private readonly PackFileOutput<EntryLight> _FileEntryLight;
        private OutputProcesser _Output;
        private readonly PackEntryType _PackEntryTypeInstance;
        private readonly PackEntryType _PackEntryTypeLight;
        private SvfFileProcesser _SvfFile;
        public const string FILE_PATH_LIGHT_DEFINITIONS = "LightDefinitions.bin";
        public const string FILE_PATH_LIGHT_LIST = "LightList.bin";

        public FileLight(OutputProcesser output, SvfFileProcesser svfFile)
        {
            this._Output = output;
            this._SvfFile = svfFile;
            this._EntryLight = new EntryLight();
            this._PackEntryTypeLight = new PackEntryType(0, "Autodesk.CloudPlatform.Light", "Autodesk.CloudPlatform.LightDefinition", 2);
            this._FileEntryLight = new PackFileOutput<EntryLight>(0x1000);
            this._FileEntryLight.OnStart();
            this._FileEntryLight.OnEntryType(this._PackEntryTypeLight);
            this._EntryInstance = new EntryInstance();
            this._PackEntryTypeInstance = new PackEntryType(0, "Autodesk.CloudPlatform.Instance", "Autodesk.CloudPlatform.InstanceData", 2);
            this._FileEntryInstance = new PackFileOutput<EntryInstance>(0x1000);
            this._FileEntryInstance.OnStart();
            this._FileEntryInstance.OnEntryType(this._PackEntryTypeInstance);
        }

        public void OnFinish()
        {
            FileEntryStream entry = new FileEntryStream("LightList.bin");
            FileEntryStream stream2 = new FileEntryStream("LightDefinitions.bin");
            this._FileEntryLight.OnFinish(entry.Stream);
            this._FileEntryInstance.OnFinish(stream2.Stream);
            this._Output.OnAppendFile(entry);
            this._Output.OnAppendFile(stream2);
            int size = entry.GetSize();
            int num2 = stream2.GetSize();
            this._SvfFile.OnEntry(new EntryAsset("LightDefinitions.bin", "Autodesk.CloudPlatform.PackFile", "LightDefinitions.bin", new int?(num2), 0, this._SvfFile.AssetTypeLight));
            this._SvfFile.OnEntry(new EntryAsset("LightList.bin", "Autodesk.CloudPlatform.PackFile", "LightList.bin", new int?(size), 0, this._SvfFile.AssetTypeInstance));
        }
    }
}

