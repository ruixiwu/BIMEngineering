using System.IO;
using System.Text;
using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Content.Other;
using BIM.Lmv.Types;
using Ionic.Zip;
using Newtonsoft.Json.Linq;

namespace BIM.Lmv.Processers
{
    internal class SvfFileProcesser
    {
        private readonly JObject _Manifest;
        private readonly JObject _Metadata;
        private readonly ExportOption _Option;
        private readonly OutputProcesser _Output;
        private SceneInfo _Scene;

        public SvfFileProcesser(OutputProcesser output, SceneInfo scene, ExportOption option)
        {
            _Output = output;
            _Scene = scene;
            _Option = option;
            var obj2 = new JObject();
            obj2["sourceSystem"] = "";
            obj2["type"] = "";
            obj2["id"] = "";
            obj2["version"] = "";
            _Manifest = new JObject();
            _Manifest["name"] = "LMV Manifest";
            _Manifest["toolkitversion"] = "HYLMV 1.0";
            _Manifest["manifestversion"] = 2;
            _Manifest["adskID"] = obj2;
            _Manifest["assets"] = new JArray();
            _Manifest["typesets"] = new JArray();
            AssetTypeCamera =
                OnEntryType(new PackEntryType(0, "Autodesk.CloudPlatform.Camera",
                    "Autodesk.CloudPlatform.CameraDefinition", 2));
            AssetTypeLight =
                OnEntryType(new PackEntryType(0, "Autodesk.CloudPlatform.Light",
                    "Autodesk.CloudPlatform.LightDefinition", 1));
            AssetTypeGeometry =
                OnEntryType(new PackEntryType(0, "Autodesk.CloudPlatform.Geometry", "Autodesk.CloudPlatform.OpenCTM", 1));
            AssetTypeGeometryEx =
                OnEntryType(new PackEntryType(0, "Autodesk.CloudPlatform.Geometry", "Autodesk.CloudPlatform.Lines", 2),
                    new PackEntryType(0, "Autodesk.CloudPlatform.Geometry", "Autodesk.CloudPlatform.OpenCTM", 1));
            AssetTypeGeometryMetadata =
                OnEntryType(new PackEntryType(0, "Autodesk.CloudPlatform.GeometryMetadata",
                    "Autodesk.CloudPlatform.GeometryMetadataData", 3));
            AssetTypeFragment =
                OnEntryType(new PackEntryType(0, "Autodesk.CloudPlatform.Fragment",
                    "Autodesk.CloudPlatform.FragmentData", 5));
            AssetTypeInstance =
                OnEntryType(new PackEntryType(0, "Autodesk.CloudPlatform.Instance",
                    "Autodesk.CloudPlatform.InstanceData", 2));
            AssetTypeInstanceTreeNode =
                OnEntryType(new PackEntryType(0, "Autodesk.CloudPlatform.InstanceTreeNode",
                    "Autodesk.CloudPlatform.InstanceTreeNodeData", 5));
            AssetTypeSet =
                OnEntryType(new PackEntryType(0, "Autodesk.CloudPlatform.Set", "Autodesk.CloudPlatform.SetData", 1));
            var obj3 = new JObject();
            obj3["double sided geometry"] = new JObject();
            obj3["double sided geometry"]["value"] = 0;
            obj3["navigation hint"] = new JObject();
            obj3["navigation hint"]["value"] = "Turntable";
            obj3["world bounding box"] = new JObject();
            obj3["world up vector"] = new JObject();
            obj3["world up vector"]["XYZ"] = Vector3FToJArray(scene.WorldUp);
            obj3["world front vector"] = new JObject();
            obj3["world front vector"]["XYZ"] = Vector3FToJArray(scene.WorldFront);
            obj3["world north vector"] = new JObject();
            obj3["world north vector"]["XYZ"] = Vector3FToJArray(scene.WorldNorth);
            obj3["distance unit"] = new JObject();
            obj3["distance unit"]["value"] = scene.DistanceUnit;
            obj3["default camera"] = new JObject();
            obj3["default camera"]["index"] = 0;
            obj3["view to model transform"] = new JObject();
            obj3["view to model transform"]["type"] = 4;
            obj3["georeference"] = new JObject();
            obj3["georeference"]["positionLL84"] = new JArray(scene.Longitude, scene.Latitude, 0);
            obj3["custom values"] = new JObject();
            obj3["custom values"]["angleToTrueNorth"] = scene.AngleToTrueNorth;
            _Metadata = new JObject();
            _Metadata["version"] = "1.0";
            _Metadata["metadata"] = obj3;
        }

        public string AssetTypeCamera { get; private set; }

        public string AssetTypeFragment { get; private set; }

        public string AssetTypeGeometry { get; private set; }

        public string AssetTypeGeometryEx { get; private set; }

        public string AssetTypeGeometryMetadata { get; private set; }

        public string AssetTypeInstance { get; private set; }

        public string AssetTypeInstanceTreeNode { get; private set; }

        public string AssetTypeLight { get; private set; }

        public string AssetTypeSet { get; private set; }

        public void OnEntry(EntryAsset asset)
        {
            _Manifest.Value<JArray>("assets").Add(asset.GetData());
        }

        private string OnEntryType(params PackEntryType[] entryTypes)
        {
            var array = _Manifest.Value<JArray>("typesets");
            var str = array.Count.ToString();
            var array2 = new JArray();
            foreach (var type in entryTypes)
            {
                var obj2 = new JObject();
                obj2["class"] = type.entryClass;
                obj2["type"] = type.entryType;
                obj2["version"] = type.version;
                array2.Add(obj2);
            }
            var item = new JObject();
            item["id"] = str;
            item["types"] = array2;
            array.Add(item);
            return str;
        }

        public void OnFinish(Vector3D boxMin, Vector3D boxMax)
        {
            _Metadata["metadata"]["world bounding box"]["minXYZ"] = new JArray(boxMin.x, boxMin.y, boxMin.z);
            _Metadata["metadata"]["world bounding box"]["maxXYZ"] = new JArray(boxMax.x, boxMax.y, boxMax.z);
            var entry = new FileEntryStream("3d.svf");
            using (var file = new ZipFile(Encoding.UTF8))
            {
                file.AddDirectoryByName("scene");
                file.AddDirectoryByName("geometry");
                file.AddDirectoryByName("material");
                file.AddEntry("manifest.json", _Manifest.ToString());
                file.AddEntry("metadata.json", _Metadata.ToString());
                file.Save(entry.Stream);
            }
            _Output.OnAppendFile(entry);
            if (_Option.Target != ExportTarget.CloudPackage)
            {
                string str2;
                var name = _Option.Target == ExportTarget.CloudPackage
                    ? "BIM.Lmv.Processers.Resources.index.cloud.html"
                    : "BIM.Lmv.Processers.Resources.index.local.html";
                using (var stream2 = GetType().Assembly.GetManifestResourceStream(name))
                {
                    using (var reader = new StreamReader(stream2, Encoding.UTF8))
                    {
                        str2 = reader.ReadToEnd();
                    }
                }
                _Output.OnAppendFile(new FileEntryString("index.html", str2));
            }
        }

        private JArray Vector3FToJArray(Vector3F v)
        {
            return new JArray(v.x, v.y, v.z);
        }
    }
}