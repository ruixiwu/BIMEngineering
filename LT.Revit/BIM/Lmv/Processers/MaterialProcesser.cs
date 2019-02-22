namespace BIM.Lmv.Processers
{
    using BIM.Lmv.Common.JsonGz;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Content.Other;
    using BIM.Lmv.Types;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;

    internal class MaterialProcesser
    {
        private Dictionary<string, string> _MatDirectories = new Dictionary<string, string>();
        private int _MaterialId;
        private readonly Dictionary<string, int> _MaterialMapping = new Dictionary<string, int>();
        private readonly JObject _MaterialsSimple;
        private Dictionary<string, string> _MatFiles = new Dictionary<string, string>();
        private MD5CryptoServiceProvider _Md5 = new MD5CryptoServiceProvider();
        private int _NextMaterialId;
        private OutputProcesser _Output;
        private readonly SvfFileProcesser _SvfFile;
        public const string FILE_PATH_MATERIALS = "Materials.json.gz";
        public const string FILE_PATH_MATERIALS_PLAIN = "Materials_Plain.json.gz";
        public const string FILE_PATH_PROTEIN_MATERIALS = "ProteinMaterials.json.gz";

        public MaterialProcesser(OutputProcesser output, SvfFileProcesser svfFile)
        {
            this._Output = output;
            this._SvfFile = svfFile;
            this._MaterialsSimple = new JObject();
            this._MaterialsSimple["scene"] = new JObject();
            this._MaterialsSimple["materials"] = new JObject();
            this._NextMaterialId = 0;
        }

        private JToken GetJsonForBoolean(bool b) => 
            new JObject { ["values"] = new JArray(b) };

        private JToken GetJsonForColor(Vector3D c)
        {
            JObject content = new JObject {
                ["r"] = c.x,
                ["g"] = c.y,
                ["b"] = c.z
            };
            return new JObject { ["values"] = new JArray(content) };
        }

        private JToken GetJsonForScalar(double n) => 
            new JObject { ["values"] = new JArray(n) };

        private JToken GetJsonForString(string s) => 
            new JObject { ["values"] = new JArray(s) };

        private string GetMd5(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            bytes = this._Md5.ComputeHash(bytes);
            string str = "";
            foreach (byte num in bytes)
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
                textureFilePath = this.TransFile(t.TextureFilePath, true);
            }
            JObject obj2 = new JObject {
                ["unifiedbitmap_Bitmap"] = this.GetJsonForString(textureFilePath)
            };
            JObject obj3 = new JObject {
                ["unifiedbitmap_Invert"] = t.Invert,
                ["texture_URepeat"] = t.URepeat,
                ["texture_VRepeat"] = t.VRepeat
            };
            JObject obj4 = new JObject {
                ["texture_UScale"] = this.GetJsonForScalar(t.UScale),
                ["texture_VScale"] = this.GetJsonForScalar(t.VScale),
                ["texture_UOffset"] = this.GetJsonForScalar(t.UOffset),
                ["texture_VOffset"] = this.GetJsonForScalar(t.VOffset),
                ["texture_WAngle"] = this.GetJsonForScalar(t.WAngle)
            };
            JObject obj5 = new JObject();
            return new JObject { 
                ["uris"] = obj2,
                ["booleans"] = obj3,
                ["scalars"] = obj4,
                ["integers"] = obj5
            };
        }

        public void OnFinish()
        {
            FileEntryStream entry = new FileEntryStream("Materials.json.gz");
            FileEntryStream stream2 = new FileEntryStream("ProteinMaterials.json.gz");
            JsonGzWriter.Save(this._MaterialsSimple.ToString(Formatting.Indented, new JsonConverter[0]), entry.Stream, true);
            JsonGzWriter.Save(this._MaterialsSimple.ToString(Formatting.Indented, new JsonConverter[0]), stream2.Stream, true);
            this._Output.OnAppendFile(entry);
            this._Output.OnAppendFile(stream2);
            int size = entry.GetSize();
            int num2 = stream2.GetSize();
            this._SvfFile.OnEntry(new EntryAsset("ProteinMaterials.json.gz", "ProteinMaterials", "ProteinMaterials.json.gz", new int?(num2), 0, null));
            this._SvfFile.OnEntry(new EntryAsset("Materials.json.gz", "ProteinMaterials", "Materials.json.gz", new int?(size), 0, null));
        }

        public int OnMaterial(string materialKey, MaterialInfo material)
        {
            if (this._MaterialMapping.ContainsKey(materialKey))
            {
                this._MaterialId = this._MaterialMapping[materialKey];
                return this._MaterialId;
            }
            this._MaterialId = this._NextMaterialId++;
            this._MaterialMapping.Add(materialKey, this._MaterialId);
            JObject obj2 = new JObject();
            if (material.Ambient != null)
            {
                obj2["generic_ambient"] = this.GetJsonForColor(material.Ambient);
            }
            if (material.Color != null)
            {
                obj2["generic_diffuse"] = this.GetJsonForColor(material.Color);
            }
            if (material.Specular != null)
            {
                obj2["generic_specular"] = this.GetJsonForColor(material.Specular);
            }
            if (material.Emissive != null)
            {
                obj2["generic_emissive"] = this.GetJsonForColor(material.Emissive);
            }
            JObject obj3 = new JObject {
                ["generic_bump_is_normal"] = material.BumpIsNormal,
                ["generic_is_metal"] = material.IsMetal,
                ["generic_backface_cull"] = material.BackfaceCulling
            };
            JObject obj4 = new JObject();
            if (!(material.Reflectivity == 0.0))
            {
                obj4["generic_reflectivity_at_0deg"] = this.GetJsonForScalar(material.Reflectivity);
            }
            if (!(material.BumpAmount == 0.0))
            {
                obj4["generic_bump_amount"] = this.GetJsonForScalar(material.BumpAmount);
            }
            obj4["generic_glossiness"] = this.GetJsonForScalar(material.Shininess);
            if (!(material.Transparent == 0.0))
            {
                obj4["generic_transparency"] = this.GetJsonForScalar(material.Transparent);
            }
            JObject obj5 = new JObject();
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
            JObject obj6 = new JObject();
            JObject obj7 = new JObject {
                ["tag"] = this._MaterialId.ToString(),
                ["definition"] = "SimplePhong",
                ["transparent"] = material.Transparent > 0.0,
                ["textures"] = obj6,
                ["properties"] = obj5
            };
            JObject obj8 = new JObject {
                ["0"] = obj7
            };
            JObject obj9 = new JObject {
                ["userassets"] = new JArray("0"),
                ["materials"] = obj8
            };
            if ((material.Textures != null) && (material.Textures.Count > 0))
            {
                int num = 0;
                foreach (TextureInfo info in material.Textures)
                {
                    JToken propertiesForTexture = this.GetPropertiesForTexture(info);
                    if (propertiesForTexture != null)
                    {
                        JObject obj10 = new JObject {
                            ["tag"] = this._MaterialId.ToString(),
                            ["properties"] = propertiesForTexture
                        };
                        string textureTypeText = info.GetTextureTypeText();
                        string str2 = ++num + "_" + textureTypeText;
                        obj8[str2] = obj10;
                        JObject obj11 = new JObject();
                        JArray array = new JArray {
                            str2
                        };
                        obj11["connections"] = array;
                        obj6[textureTypeText] = obj11;
                    }
                }
            }
            this._MaterialsSimple["materials"][this._MaterialId.ToString()] = obj9;
            return this._MaterialId;
        }

        private string TransFile(string filePath, bool reducePath = true)
        {
            string str2;
            string str3;
            if (reducePath)
            {
                if (!this._MatDirectories.ContainsKey("mat"))
                {
                    this._Output.OnAppendFile(new FileEntryFolderName("mat"));
                    this._MatDirectories["mat"] = null;
                }
                string str = this.GetMd5(filePath);
                str2 = Path.GetFileNameWithoutExtension(filePath).ToLower() + "_" + str + Path.GetExtension(filePath);
                str3 = "mat/" + str2;
                if (!this._MatFiles.ContainsKey(str3))
                {
                    this._Output.OnAppendFile(new FileEntryFile(str3, filePath));
                    this._MatFiles[str3] = null;
                }
                return str3;
            }
            string str4 = Path.GetDirectoryName(filePath).ToLower().Replace(":", "").Trim(new char[] { '/', '\\' }).Replace(@"\", "/");
            string key = "";
            string[] strArray = str4.Split(new string[] { @"\", "/" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str6 in strArray)
            {
                key = (key == "") ? str6 : (key + "/" + str6);
                if (!this._MatDirectories.ContainsKey(key))
                {
                    this._Output.OnAppendFile(new FileEntryFolderName(key));
                    this._MatDirectories[key] = null;
                }
            }
            str2 = Path.GetFileName(filePath).ToLower();
            str3 = key + "/" + str2;
            if (!this._MatFiles.ContainsKey(str3))
            {
                this._Output.OnAppendFile(new FileEntryFile(str3, filePath));
                this._MatFiles[str3] = null;
            }
            return str3;
        }
    }
}

