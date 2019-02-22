namespace BIM.Lmv.Processers.Geometry
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Content.Other;
    using BIM.Lmv.Processers;
    using BIM.Lmv.Types;
    using System;

    internal class FileCamera
    {
        private readonly EntryCamera _EntryCamera;
        private readonly EntryInstance _EntryInstance;
        private readonly PackFileOutput<EntryCamera> _FileEntryCamera;
        private readonly PackFileOutput<EntryInstance> _FileEntryInstance;
        private OutputProcesser _Output;
        private readonly PackEntryType _PackEntryTypeCamera;
        private readonly PackEntryType _PackEntryTypeInstance;
        private readonly SvfFileProcesser _SvfFile;
        public const string FILE_PATH_CAMERA_DEFINITIONS = "CameraDefinitions.bin";
        public const string FILE_PATH_CAMERA_LIST = "CameraList.bin";

        public FileCamera(OutputProcesser output, SvfFileProcesser svfFile)
        {
            this._Output = output;
            this._SvfFile = svfFile;
            this._EntryCamera = new EntryCamera();
            this._PackEntryTypeCamera = new PackEntryType(0, "Autodesk.CloudPlatform.Camera", "Autodesk.CloudPlatform.CameraDefinition", 2);
            this._FileEntryCamera = new PackFileOutput<EntryCamera>(0x1000);
            this._FileEntryCamera.OnStart();
            this._FileEntryCamera.OnEntryType(this._PackEntryTypeCamera);
            this._EntryInstance = new EntryInstance();
            this._PackEntryTypeInstance = new PackEntryType(0, "Autodesk.CloudPlatform.Instance", "Autodesk.CloudPlatform.InstanceData", 2);
            this._FileEntryInstance = new PackFileOutput<EntryInstance>(0x1000);
            this._FileEntryInstance.OnStart();
            this._FileEntryInstance.OnEntryType(this._PackEntryTypeInstance);
        }

        public void OnCamera(CameraInfo camera)
        {
            this._EntryCamera.isPerspective = camera.IsPerspective ? ((byte) 0) : ((byte) 1);
            this._EntryCamera.position = camera.Position.clone();
            this._EntryCamera.up = camera.Up.clone();
            this._EntryCamera.aspect = camera.Aspect;
            this._EntryCamera.fov = camera.Fov;
            this._EntryCamera.target = camera.Target.clone();
            this._EntryCamera.orthoScale = this._EntryCamera.position.distanceTo(this._EntryCamera.target);
            int num = this._FileEntryCamera.OnEntry(this._EntryCamera, this._PackEntryTypeCamera);
            this._EntryInstance.definition = (uint) num;
            this._EntryInstance.transform = Transform.GetIdentity();
            this._FileEntryInstance.OnEntry(this._EntryInstance, this._PackEntryTypeInstance);
        }

        public void OnFinish()
        {
            FileEntryStream entry = new FileEntryStream("CameraDefinitions.bin");
            this._FileEntryCamera.OnFinish(entry.Stream);
            FileEntryStream stream2 = new FileEntryStream("CameraList.bin");
            this._FileEntryInstance.OnFinish(stream2.Stream);
            this._Output.OnAppendFile(entry);
            this._Output.OnAppendFile(stream2);
            int size = stream2.GetSize();
            int num2 = entry.GetSize();
            this._SvfFile.OnEntry(new EntryAsset("CameraDefinitions.bin", "Autodesk.CloudPlatform.PackFile", "CameraDefinitions.bin", new int?(num2), 0, this._SvfFile.AssetTypeCamera));
            this._SvfFile.OnEntry(new EntryAsset("CameraList.bin", "Autodesk.CloudPlatform.PackFile", "CameraList.bin", new int?(size), 0, this._SvfFile.AssetTypeInstance));
        }
    }
}

