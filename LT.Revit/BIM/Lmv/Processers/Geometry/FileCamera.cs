using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Content.Other;
using BIM.Lmv.Types;

namespace BIM.Lmv.Processers.Geometry
{
    internal class FileCamera
    {
        public const string FILE_PATH_CAMERA_DEFINITIONS = "CameraDefinitions.bin";
        public const string FILE_PATH_CAMERA_LIST = "CameraList.bin";
        private readonly EntryCamera _EntryCamera;
        private readonly EntryInstance _EntryInstance;
        private readonly PackFileOutput<EntryCamera> _FileEntryCamera;
        private readonly PackFileOutput<EntryInstance> _FileEntryInstance;
        private readonly PackEntryType _PackEntryTypeCamera;
        private readonly PackEntryType _PackEntryTypeInstance;
        private readonly SvfFileProcesser _SvfFile;
        private readonly OutputProcesser _Output;

        public FileCamera(OutputProcesser output, SvfFileProcesser svfFile)
        {
            _Output = output;
            _SvfFile = svfFile;
            _EntryCamera = new EntryCamera();
            _PackEntryTypeCamera = new PackEntryType(0, "Autodesk.CloudPlatform.Camera",
                "Autodesk.CloudPlatform.CameraDefinition", 2);
            _FileEntryCamera = new PackFileOutput<EntryCamera>(0x1000);
            _FileEntryCamera.OnStart();
            _FileEntryCamera.OnEntryType(_PackEntryTypeCamera);
            _EntryInstance = new EntryInstance();
            _PackEntryTypeInstance = new PackEntryType(0, "Autodesk.CloudPlatform.Instance",
                "Autodesk.CloudPlatform.InstanceData", 2);
            _FileEntryInstance = new PackFileOutput<EntryInstance>(0x1000);
            _FileEntryInstance.OnStart();
            _FileEntryInstance.OnEntryType(_PackEntryTypeInstance);
        }

        public void OnCamera(CameraInfo camera)
        {
            _EntryCamera.isPerspective = camera.IsPerspective ? (byte) 0 : (byte) 1;
            _EntryCamera.position = camera.Position.clone();
            _EntryCamera.up = camera.Up.clone();
            _EntryCamera.aspect = camera.Aspect;
            _EntryCamera.fov = camera.Fov;
            _EntryCamera.target = camera.Target.clone();
            _EntryCamera.orthoScale = _EntryCamera.position.distanceTo(_EntryCamera.target);
            var num = _FileEntryCamera.OnEntry(_EntryCamera, _PackEntryTypeCamera);
            _EntryInstance.definition = (uint) num;
            _EntryInstance.transform = Transform.GetIdentity();
            _FileEntryInstance.OnEntry(_EntryInstance, _PackEntryTypeInstance);
        }

        public void OnFinish()
        {
            var entry = new FileEntryStream("CameraDefinitions.bin");
            _FileEntryCamera.OnFinish(entry.Stream);
            var stream2 = new FileEntryStream("CameraList.bin");
            _FileEntryInstance.OnFinish(stream2.Stream);
            _Output.OnAppendFile(entry);
            _Output.OnAppendFile(stream2);
            var size = stream2.GetSize();
            var num2 = entry.GetSize();
            _SvfFile.OnEntry(new EntryAsset("CameraDefinitions.bin", "Autodesk.CloudPlatform.PackFile",
                "CameraDefinitions.bin", num2, 0, _SvfFile.AssetTypeCamera));
            _SvfFile.OnEntry(new EntryAsset("CameraList.bin", "Autodesk.CloudPlatform.PackFile", "CameraList.bin", size,
                0, _SvfFile.AssetTypeInstance));
        }
    }
}