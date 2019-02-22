namespace BIM.Lmv.Processers
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Content.Other;
    using BIM.Lmv.Types;
    using Ionic.Zip;
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class SvfFileProcesser
    {
        private JObject _Manifest;
        private JObject _Metadata;
        private ExportOption _Option;
        private OutputProcesser _Output;
        private SceneInfo _Scene;

        public SvfFileProcesser(OutputProcesser output, SceneInfo scene, ExportOption option)
        {
            this._Output = output;
            this._Scene = scene;
            this._Option = option;
            JObject obj2 = new JObject {
                ["sourceSystem"] = "",
                ["type"] = "",
                ["id"] = "",
                ["version"] = ""
            };
            this._Manifest = new JObject();
            this._Manifest["name"] = "LMV Manifest";
            this._Manifest["toolkitversion"] = "HYLMV 1.0";
            this._Manifest["manifestversion"] = 2;
            this._Manifest["adskID"] = obj2;
            this._Manifest["assets"] = new JArray();
            this._Manifest["typesets"] = new JArray();
            this.AssetTypeCamera = this.OnEntryType(new PackEntryType[] { new PackEntryType(0, "Autodesk.CloudPlatform.Camera", "Autodesk.CloudPlatform.CameraDefinition", 2) });
            this.AssetTypeLight = this.OnEntryType(new PackEntryType[] { new PackEntryType(0, "Autodesk.CloudPlatform.Light", "Autodesk.CloudPlatform.LightDefinition", 1) });
            this.AssetTypeGeometry = this.OnEntryType(new PackEntryType[] { new PackEntryType(0, "Autodesk.CloudPlatform.Geometry", "Autodesk.CloudPlatform.OpenCTM", 1) });
            this.AssetTypeGeometryEx = this.OnEntryType(new PackEntryType[] { new PackEntryType(0, "Autodesk.CloudPlatform.Geometry", "Autodesk.CloudPlatform.Lines", 2), new PackEntryType(0, "Autodesk.CloudPlatform.Geometry", "Autodesk.CloudPlatform.OpenCTM", 1) });
            this.AssetTypeGeometryMetadata = this.OnEntryType(new PackEntryType[] { new PackEntryType(0, "Autodesk.CloudPlatform.GeometryMetadata", "Autodesk.CloudPlatform.GeometryMetadataData", 3) });
            this.AssetTypeFragment = this.OnEntryType(new PackEntryType[] { new PackEntryType(0, "Autodesk.CloudPlatform.Fragment", "Autodesk.CloudPlatform.FragmentData", 5) });
            this.AssetTypeInstance = this.OnEntryType(new PackEntryType[] { new PackEntryType(0, "Autodesk.CloudPlatform.Instance", "Autodesk.CloudPlatform.InstanceData", 2) });
            this.AssetTypeInstanceTreeNode = this.OnEntryType(new PackEntryType[] { new PackEntryType(0, "Autodesk.CloudPlatform.InstanceTreeNode", "Autodesk.CloudPlatform.InstanceTreeNodeData", 5) });
            this.AssetTypeSet = this.OnEntryType(new PackEntryType[] { new PackEntryType(0, "Autodesk.CloudPlatform.Set", "Autodesk.CloudPlatform.SetData", 1) });
            JObject obj3 = new JObject {
                ["double sided geometry"] = new JObject()
            };
            obj3["double sided geometry"]["value"] = 0;
            obj3["navigation hint"] = new JObject();
            obj3["navigation hint"]["value"] = "Turntable";
            obj3["world bounding box"] = new JObject();
            obj3["world up vector"] = new JObject();
            obj3["world up vector"]["XYZ"] = this.Vector3FToJArray(scene.WorldUp);
            obj3["world front vector"] = new JObject();
            obj3["world front vector"]["XYZ"] = this.Vector3FToJArray(scene.WorldFront);
            obj3["world north vector"] = new JObject();
            obj3["world north vector"]["XYZ"] = this.Vector3FToJArray(scene.WorldNorth);
            obj3["distance unit"] = new JObject();
            obj3["distance unit"]["value"] = scene.DistanceUnit;
            obj3["default camera"] = new JObject();
            obj3["default camera"]["index"] = 0;
            obj3["view to model transform"] = new JObject();
            obj3["view to model transform"]["type"] = 4;
            obj3["georeference"] = new JObject();
            obj3["georeference"]["positionLL84"] = new JArray(new object[] { scene.Longitude, scene.Latitude, 0 });
            obj3["custom values"] = new JObject();
            obj3["custom values"]["angleToTrueNorth"] = scene.AngleToTrueNorth;
            this._Metadata = new JObject();
            this._Metadata["version"] = "1.0";
            this._Metadata["metadata"] = obj3;
        }

        public void OnEntry(EntryAsset asset)
        {
            this._Manifest.Value<JArray>("assets").Add(asset.GetData());
        }

        private string OnEntryType(params PackEntryType[] entryTypes)
        {
            JArray array = this._Manifest.Value<JArray>("typesets");
            string str = array.Count.ToString();
            JArray array2 = new JArray();
            foreach (PackEntryType type in entryTypes)
            {
                JObject obj2 = new JObject {
                    ["class"] = type.entryClass,
                    ["type"] = type.entryType,
                    ["version"] = type.version
                };
                array2.Add(obj2);
            }
            JObject item = new JObject {
                ["id"] = str,
                ["types"] = array2
            };
            array.Add(item);
            return str;
        }

        public void OnFinish(Vector3D boxMin, Vector3D boxMax)
        {
            this._Metadata["metadata"]["world bounding box"]["minXYZ"] = new JArray(new object[] { boxMin.x, boxMin.y, boxMin.z });
            this._Metadata["metadata"]["world bounding box"]["maxXYZ"] = new JArray(new object[] { boxMax.x, boxMax.y, boxMax.z });
            FileEntryStream entry = new FileEntryStream("3d.svf");
            using (ZipFile file = new ZipFile(Encoding.UTF8))
            {
                file.AddDirectoryByName("scene");
                file.AddDirectoryByName("geometry");
                file.AddDirectoryByName("material");
                file.AddEntry("manifest.json", this._Manifest.ToString());
                file.AddEntry("metadata.json", this._Metadata.ToString());
                file.Save(entry.Stream);
            }
            this._Output.OnAppendFile(entry);
            if (this._Option.Target != ExportTarget.CloudPackage)
            {
                string str2;
                string name = (this._Option.Target == ExportTarget.CloudPackage) ? "BIM.Lmv.Processers.Resources.index.cloud.html" : "BIM.Lmv.Processers.Resources.index.local.html";
                using (Stream stream2 = base.GetType().Assembly.GetManifestResourceStream(name))
                {
                    using (StreamReader reader = new StreamReader(stream2, Encoding.UTF8))
                    {
                        str2 = reader.ReadToEnd();
                    }
                }
                this._Output.OnAppendFile(new FileEntryString("index.html", str2));
            }
        }

        private JArray Vector3FToJArray(Vector3F v) => 
            new JArray(new object[] { v.x, v.y, v.z });

        public string AssetTypeCamera { get; private set; }

        public string AssetTypeFragment { get; private set; }

        public string AssetTypeGeometry { get; private set; }

        public string AssetTypeGeometryEx { get; private set; }

        public string AssetTypeGeometryMetadata { get; private set; }

        public string AssetTypeInstance { get; private set; }

        public string AssetTypeInstanceTreeNode { get; private set; }

        public string AssetTypeLight { get; private set; }

        public string AssetTypeSet { get; private set; }
    }
}

