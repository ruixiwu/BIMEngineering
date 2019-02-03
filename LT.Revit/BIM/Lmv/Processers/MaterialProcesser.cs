using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BIM.Lmv.Common.JsonGz;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Content.Other;
using BIM.Lmv.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BIM.Lmv.Processers
{
    internal class MaterialProcesser
    {
        public const string FILE_PATH_MATERIALS = "Materials.json.gz";
        public const string FILE_PATH_MATERIALS_PLAIN = "Materials_Plain.json.gz";
        public const string FILE_PATH_PROTEIN_MATERIALS = "ProteinMaterials.json.gz";
        private readonly Dictionary<string, int> _MaterialMapping = new Dictionary<string, int>();
        private readonly JObject _MaterialsSimple;
        private readonly SvfFileProcesser _SvfFile;
        private readonly Dictionary<string, string> _MatDirectories = new Dictionary<string, string>();
        private int _MaterialId;
        private readonly Dictionary<string, string> _MatFiles = new Dictionary<string, string>();
        private readonly MD5CryptoServiceProvider _Md5 = new MD5CryptoServiceProvider();
        private int _NextMaterialId;
        private readonly OutputProcesser _Output;

        public MaterialProcesser(OutputProcesser output, SvfFileProcesser svfFile)
        {
            _Output = output;
            _SvfFile = svfFile;
            _MaterialsSimple = new JObject();
            _MaterialsSimple["scene"] = new JObject();
            _MaterialsSimple["materials"] = new JObject();
            _NextMaterialId = 0;
        }

        private JToken GetJsonForBoolean(bool b)
        {
            var obj2 = new JObject();
            obj2["values"] = new JArray(b);
            return obj2;
        }

        private JToken GetJsonForColor(Vector3D c)
        {
            var content = new JObject();
            content["r"] = c.x;
            content["g"] = c.y;
            content["b"] = c.z;
            var obj3 = new JObject();
            obj3["values"] = new JArray(content);
            return obj3;
        }

        private JToken GetJsonForScalar(double n)
        {
            var obj2 = new JObject();
            obj2["values"] = new JArray(n);
            return obj2;
        }

        private JToken GetJsonForString(string s)
        {
            var obj2 = new JObject();
            obj2["values"] = new JArray(s);
            return obj2;
        }

        private string GetMd5(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            bytes = _Md5.ComputeHash(bytes);
            var str = "";
            foreach (var num in bytes)
            {
                str = str + num.ToString("x2");
            }
            return str;
        }

        private JToken GetPropertiesForTexture(TextureInfo t)
        {
            string textureFilePath;
            if ((t == null) || string.IsNullOrEmpty(t.TextureFilePath))
            {
                return null;
            }
            if (t.IsShared)
            {
                textureFilePath = t.TextureFilePath;
            }
            else
            {
                if (!File.Exists(t.TextureFilePath))
                {
                    return null;
                }
                textureFilePath = TransFile(t.TextureFilePath, true);
            }
            var obj2 = new JObject();
            obj2["unifiedbitmap_Bitmap"] = GetJsonForString(textureFilePath);
            var obj3 = new JObject();
            obj3["unifiedbitmap_Invert"] = t.Invert;
            obj3["texture_URepeat"] = t.URepeat;
            obj3["texture_VRepeat"] = t.VRepeat;
            var obj4 = new JObject();
            obj4["texture_UScale"] = GetJsonForScalar(t.UScale);
            obj4["texture_VScale"] = GetJsonForScalar(t.VScale);
            obj4["texture_UOffset"] = GetJsonForScalar(t.UOffset);
            obj4["texture_VOffset"] = GetJsonForScalar(t.VOffset);
            obj4["texture_WAngle"] = GetJsonForScalar(t.WAngle);
            var obj5 = new JObject();
            var obj6 = new JObject();
            obj6["uris"] = obj2;
            obj6["booleans"] = obj3;
            obj6["scalars"] = obj4;
            obj6["integers"] = obj5;
            return obj6;
        }

        public void OnFinish()
        {
            var entry = new FileEntryStream("Materials.json.gz");
            var stream2 = new FileEntryStream("ProteinMaterials.json.gz");
            JsonGzWriter.Save(_MaterialsSimple.ToString(Formatting.Indented), entry.Stream, true);
            JsonGzWriter.Save(_MaterialsSimple.ToString(Formatting.Indented), stream2.Stream, true);
            _Output.OnAppendFile(entry);
            _Output.OnAppendFile(stream2);
            var size = entry.GetSize();
            var num2 = stream2.GetSize();
            _SvfFile.OnEntry(new EntryAsset("ProteinMaterials.json.gz", "ProteinMaterials", "ProteinMaterials.json.gz",
                num2, 0, null));
            _SvfFile.OnEntry(new EntryAsset("Materials.json.gz", "ProteinMaterials", "Materials.json.gz", size, 0, null));
        }

        public int OnMaterial(string materialKey, MaterialInfo material)
        {
            if (_MaterialMapping.ContainsKey(materialKey))
            {
                _MaterialId = _MaterialMapping[materialKey];
                return _MaterialId;
            }
            _MaterialId = _NextMaterialId++;
            _MaterialMapping.Add(materialKey, _MaterialId);
            var obj2 = new JObject();
            if (material.Ambient != null)
            {
                obj2["generic_ambient"] = GetJsonForColor(material.Ambient);
            }
            if (material.Color != null)
            {
                obj2["generic_diffuse"] = GetJsonForColor(material.Color);
            }
            if (material.Specular != null)
            {
                obj2["generic_specular"] = GetJsonForColor(material.Specular);
            }
            if (material.Emissive != null)
            {
                obj2["generic_emissive"] = GetJsonForColor(material.Emissive);
            }
            var obj3 = new JObject();
            obj3["generic_bump_is_normal"] = material.BumpIsNormal;
            obj3["generic_is_metal"] = material.IsMetal;
            obj3["generic_backface_cull"] = material.BackfaceCulling;
            var obj4 = new JObject();
            if (!(material.Reflectivity == 0.0))
            {
                obj4["generic_reflectivity_at_0deg"] = GetJsonForScalar(material.Reflectivity);
            }
            if (!(material.BumpAmount == 0.0))
            {
                obj4["generic_bump_amount"] = GetJsonForScalar(material.BumpAmount);
            }
            obj4["generic_glossiness"] = GetJsonForScalar(material.Shininess);
            if (!(material.Transparent == 0.0))
            {
                obj4["generic_transparency"] = GetJsonForScalar(material.Transparent);
            }
            var obj5 = new JObject();
            if ((material.Textures != null) && (material.Textures.Count > 0))
            {
                obj5["booleans"] = obj3;
                obj5["scalars"] = obj4;
                obj5["colors"] = obj2;
            }
            else
            {
                obj5["colors"] = obj2;
                obj5["scalars"] = obj4;
                obj5["booleans"] = obj3;
            }
            var obj6 = new JObject();
            var obj7 = new JObject();
            obj7["tag"] = _MaterialId.ToString();
            obj7["definition"] = "SimplePhong";
            obj7["transparent"] = material.Transparent > 0.0;
            obj7["textures"] = obj6;
            obj7["properties"] = obj5;
            var obj8 = new JObject();
            obj8["0"] = obj7;
            var obj9 = new JObject();
            obj9["userassets"] = new JArray("0");
            obj9["materials"] = obj8;
            if ((material.Textures != null) && (material.Textures.Count > 0))
            {
                var num = 0;
                foreach (var info in material.Textures)
                {
                    var propertiesForTexture = GetPropertiesForTexture(info);
                    if (propertiesForTexture != null)
                    {
                        var obj10 = new JObject();
                        obj10["tag"] = _MaterialId.ToString();
                        obj10["properties"] = propertiesForTexture;
                        var textureTypeText = info.GetTextureTypeText();
                        var str2 = ++num + "_" + textureTypeText;
                        obj8[str2] = obj10;
                        var obj11 = new JObject();
                        var array = new JArray
                        {
                            str2
                        };
                        obj11["connections"] = array;
                        obj6[textureTypeText] = obj11;
                    }
                }
            }
            _MaterialsSimple["materials"][_MaterialId.ToString()] = obj9;
            return _MaterialId;
        }

        private string TransFile(string filePath, bool reducePath = true)
        {
            string str2;
            string str3;
            if (reducePath)
            {
                if (!_MatDirectories.ContainsKey("mat"))
                {
                    _Output.OnAppendFile(new FileEntryFolderName("mat"));
                    _MatDirectories["mat"] = null;
                }
                var str = GetMd5(filePath);
                str2 = Path.GetFileNameWithoutExtension(filePath).ToLower() + "_" + str + Path.GetExtension(filePath);
                str3 = "mat/" + str2;
                if (!_MatFiles.ContainsKey(str3))
                {
                    _Output.OnAppendFile(new FileEntryFile(str3, filePath));
                    _MatFiles[str3] = null;
                }
                return str3;
            }
            var str4 = Path.GetDirectoryName(filePath).ToLower().Replace(":", "").Trim('/', '\\').Replace(@"\", "/");
            var key = "";
            var strArray = str4.Split(new[] {@"\", "/"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var str6 in strArray)
            {
                key = key == "" ? str6 : key + "/" + str6;
                if (!_MatDirectories.ContainsKey(key))
                {
                    _Output.OnAppendFile(new FileEntryFolderName(key));
                    _MatDirectories[key] = null;
                }
            }
            str2 = Path.GetFileName(filePath).ToLower();
            str3 = key + "/" + str2;
            if (!_MatFiles.ContainsKey(str3))
            {
                _Output.OnAppendFile(new FileEntryFile(str3, filePath));
                _MatFiles[str3] = null;
            }
            return str3;
        }
    }
}