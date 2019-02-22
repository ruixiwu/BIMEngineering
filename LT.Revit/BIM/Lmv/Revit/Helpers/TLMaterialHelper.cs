namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.Utility;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Types;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal class TLMaterialHelper
    {
        private readonly bool _IncludeTexture;
        private readonly Dictionary<string, int> _Keys = new Dictionary<string, int>();
        private readonly ExportTarget _Target;
        private readonly bool _UseRealColor;
        public const string DEFAULT_TEXTURE_PATH = @"C:\Program Files (x86)\Common Files\Autodesk Shared\Materials\Textures\";

        public TLMaterialHelper(ExportTarget target, bool includeTexture, View3D view)
        {
            this._Target = target;
            this._IncludeTexture = includeTexture;
            this._UseRealColor = ((view.DisplayStyle == DisplayStyle.Realistic) || (view.DisplayStyle == DisplayStyle.RealisticWithEdges)) || (view.DisplayStyle == DisplayStyle.Raytrace);
        }

        private void CollectProperties(AssetProperty property, IList<string> excluded, DigestProperties digest)
        {
            if ((excluded == null) || !excluded.Any<string>(str => (property.Name.IndexOf(str, StringComparison.Ordinal) != -1)))
            {
                if (property.Type == AssetPropertyType.APT_List)
                {
                    foreach (AssetProperty property2 in ((AssetPropertyList) property).GetValue())
                    {
                        this.CollectProperties(property2, excluded, digest);
                    }
                }
                else if (property.Type == AssetPropertyType.APT_Properties)
                {
                    AssetProperties properties = property as AssetProperties;
                    for (int j = 0; j < properties.Size; j++)
                    {
                        this.CollectProperties(properties[j], excluded, digest);
                    }
                }
                else if (property.Type == AssetPropertyType.APT_Asset)
                {
                    Asset asset = property as Asset;
                    for (int k = 0; k < asset.Size; k++)
                    {
                        this.CollectProperties(asset[k], excluded, digest);
                    }
                }
                else
                {
                    digest.digest(property);
                }
                for (int i = 0; i < property.NumberOfConnectedProperties; i++)
                {
                    this.CollectProperties(property.GetConnectedProperty(i), excluded, digest);
                }
            }
        }

        private DigestDiffuseColor FindColor(Asset asset)
        {
            string str = asset.Name.Replace("Schema", "").ToLower();
            List<string> colorNames = new List<string> {
                str + "_color",
                str + "_diffuse",
                str + "_transmittance_map",
                str + "_tintcolor"
            };
            List<string> transparencyNames = new List<string> {
                str + "_reflectance"
            };
            DigestDiffuseColor digest = new DigestDiffuseColor(colorNames, transparencyNames);
            for (int i = 0; i < asset.Size; i++)
            {
                this.CollectProperties(asset[i], null, digest);
            }
            return digest;
        }

        private Vector3D FromColor(Color c) => 
            new Vector3D(((double) c.Red) / 255.0, ((double) c.Green) / 255.0, ((double) c.Blue) / 255.0);

        private bool GetBoolFromBoolean(Asset asset, string propName, out bool result)
        {
            result = false;
            AssetProperty property = asset[propName];
            if (property == null)
            {
                return false;
            }
            result = ((AssetPropertyBoolean) property).Value;
            return true;
        }

        private bool GetColorFromDoubleArray4d(Asset asset, string propName, out Vector3D result)
        {
            AssetProperty property = asset[propName];
            if (property == null)
            {
                result = null;
                return false;
            }
            DoubleArray array = ((AssetPropertyDoubleArray4d) property).Value;
            result = new Vector3D(array.get_Item(0), array.get_Item(1), array.get_Item(2));
            return true;
        }

        private string GetKey(Document document, MaterialNode node)
        {
            if (node.MaterialId == ElementId.InvalidElementId)
            {
                return string.Concat(new object[] { "M_", node.Color.Red, "_", node.Color.Green, "_", node.Color.Blue, "_", node.Transparency, "_", node.Glossiness, "_", node.Smoothness });
            }
            return (document.Title + "_" + node.MaterialId.IntegerValue);
        }

        private MaterialInfo GetMaterialInfo(MaterialNode node)
        {
            if (!(node.MaterialId == ElementId.InvalidElementId))
            {
                return this.GetTextureMaterialInfo(node);
            }
            return this.GetSimpleMaterialInfo(node);
        }

        private MaterialInfo GetSimpleMaterialInfo(MaterialNode node) => 
            new MaterialInfo { 
                Color = this.FromColor(node.Color),
                IsMetal = false,
                BackfaceCulling = false,
                Shininess = node.Glossiness,
                Transparent = node.Transparency
            };

        private bool GetTextureFilePath(Asset asset, string propName, out string textureFilePath, out bool isShared)
        {
            textureFilePath = null;
            isShared = false;
            AssetProperty property = asset[propName];
            if (property == null)
            {
                return false;
            }
            string str = ((AssetPropertyString) property).Value;
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            if (this._Target == ExportTarget.CloudPackage)
            {
                string str2 = this.ParseTextureFilePathForCloud(str, @"C:\Program Files (x86)\Common Files\Autodesk Shared\Materials\Textures\", out isShared);
                if (str2 == null)
                {
                    return false;
                }
                textureFilePath = str2;
            }
            else
            {
                string str3 = this.ParseTextureFilePathForLocal(str, @"C:\Program Files (x86)\Common Files\Autodesk Shared\Materials\Textures\");
                if (str3 == null)
                {
                    return false;
                }
                textureFilePath = str3;
            }
            return true;
        }

        private MaterialInfo GetTextureMaterialInfo(MaterialNode node)
        {
            Vector3D vectord;
            double num;
            Asset asset = node.HasOverriddenAppearance ? node.GetAppearanceOverride() : node.GetAppearance();
            MaterialInfo info = new MaterialInfo();
            if (node.Color.IsValid)
            {
                info.Color = this.FromColor(node.Color);
            }
            else if (this.GetColorFromDoubleArray4d(asset, "generic_diffuse", out vectord))
            {
                info.Color = vectord;
            }
            else
            {
                info.Color = new Vector3D(0.6, 0.6, 0.6);
            }
            info.Shininess = node.Glossiness;
            info.Transparent = node.Transparency;
            info.BumpIsNormal = false;
            info.BumpAmount = 1.0;
            info.IsMetal = false;
            info.BackfaceCulling = false;
            if (this._UseRealColor)
            {
                bool flag;
                if (this.GetColorFromDoubleArray4d(asset, "generic_diffuse", out vectord))
                {
                    info.Color = vectord;
                }
                if (this.GetBoolFromBoolean(asset, "generic_backface_cull", out flag))
                {
                    info.BackfaceCulling = flag;
                }
            }
            if (this.GetColorFromDoubleArray4d(asset, "generic_ambient", out vectord))
            {
                info.Ambient = vectord;
            }
            if (this.GetColorFromDoubleArray4d(asset, "generic_specular", out vectord))
            {
                info.Specular = vectord;
            }
            if (this.GetColorFromDoubleArray4d(asset, "generic_emissive", out vectord))
            {
                info.Emissive = vectord;
            }
            if (this.GetValueFromDouble(asset, "generic_reflectivity_at_0deg", out num) || this.GetValueFromDouble(asset, "generic_reflectivity_at_90deg", out num))
            {
                info.Reflectivity = num;
            }
            if (this.GetValueFromDouble(asset, "generic_bump_amount", out num))
            {
                info.BumpAmount = num / 10.0;
            }
            if (this.GetValueFromDouble(asset, "generic_glossiness", out num))
            {
                info.Shininess = num;
            }
            if (this.GetValueFromDouble(asset, "generic_transparency", out num))
            {
                info.Transparent = num;
            }
            DigestDiffuseColor color = this.FindColor(asset);
            if (color.foundColor)
            {
                info.Color = new Vector3D(Math.Min((double) 1.0, (double) (color.color[0] * 1.0)), Math.Min((double) 1.0, (double) (color.color[1] * 1.0)), Math.Min((double) 1.0, (double) (color.color[2] * 1.0)));
            }
            if (color.foundTransparency)
            {
                info.Transparent = color.color[3];
            }
            else if (asset.Name == "PlasticVinylSchema")
            {
                int num2;
                if (this.GetValueFromInteger(asset, "plasticvinyl_type", out num2) && (num2 == 1))
                {
                    info.Transparent = 0.4825;
                }
            }
            else
            {
                bool flag1 = asset.Name == "WaterSchema";
            }
            if (this._IncludeTexture)
            {
                for (int i = 0; i < asset.Size; i++)
                {
                    AssetProperty property = asset[i];
                    if (((property.Name == "generic_bump") || property.Name.EndsWith("_bump_map")) || ((property.Name.EndsWith("_pattern_shader") || property.Name.EndsWith("_pattern_map")) || property.Name.EndsWith("_bm_map")))
                    {
                        AssetProperty property2 = property.GetConnectedProperty("UnifiedBitmapSchema") ?? ((property.NumberOfConnectedProperties > 0) ? property.GetConnectedProperty(0) : null);
                        if (property2 != null)
                        {
                            TextureInfo item = this.GextTextureInfo(property2 as Asset, TextureType.Bump);
                            if (item != null)
                            {
                                if (info.Textures == null)
                                {
                                    info.Textures = new List<TextureInfo>();
                                }
                                info.Textures.Add(item);
                                break;
                            }
                        }
                    }
                }
                for (int j = 0; j < asset.Size; j++)
                {
                    AssetProperty property3 = asset[j];
                    if ((property3.Name == "generic_diffuse") || property3.Name.EndsWith("_color"))
                    {
                        AssetProperty property4 = property3.GetConnectedProperty("UnifiedBitmapSchema") ?? ((property3.NumberOfConnectedProperties > 0) ? property3.GetConnectedProperty(0) : null);
                        if (property4 != null)
                        {
                            TextureInfo info3 = this.GextTextureInfo(property4 as Asset, TextureType.Diffuse);
                            if (info3 != null)
                            {
                                if (info.Textures == null)
                                {
                                    info.Textures = new List<TextureInfo>();
                                }
                                info.Textures.Add(info3);
                                break;
                            }
                        }
                    }
                }
                if ((info.Textures != null) && info.Textures.Any<TextureInfo>(x => (x.TextureType == TextureType.Diffuse)))
                {
                    info.Color = new Vector3D(1.0, 1.0, 1.0);
                    info.Specular = new Vector3D(0.2, 0.2, 0.2);
                    info.Reflectivity = 0.2;
                }
            }
            return info;
        }

        private bool GetValueFromDistance(Asset asset, string propName, out double result)
        {
            result = 0.0;
            AssetProperty property = asset[propName];
            if (property == null)
            {
                return false;
            }
            double num = ((AssetPropertyDistance) property).Value;
            DisplayUnitType displayUnitType = ((AssetPropertyDistance) property).DisplayUnitType;
            result = 12.0 / UnitUtils.Convert(num, displayUnitType, DisplayUnitType.DUT_DECIMAL_INCHES);
            return true;
        }

        private bool GetValueFromDouble(Asset asset, string propName, out double result)
        {
            result = 0.0;
            AssetProperty property = asset[propName];
            if (property == null)
            {
                return false;
            }
            if (property is AssetPropertyFloat)
            {
                result = ((AssetPropertyFloat) property).Value;
            }
            else
            {
                result = ((AssetPropertyDouble) property).Value;
            }
            return true;
        }

        private bool GetValueFromInteger(Asset asset, string propName, out int result)
        {
            result = -2147483648;
            AssetProperty property = asset[propName];
            if ((property != null) && (property is AssetPropertyInteger))
            {
                result = ((AssetPropertyInteger) property).Value;
                return true;
            }
            return false;
        }

        private TextureInfo GextTextureInfo(Asset asset, TextureType textureType)
        {
            string str;
            bool flag;
            bool flag2;
            double num;
            if (!this.GetTextureFilePath(asset, "unifiedbitmap_Bitmap", out str, out flag))
            {
                return null;
            }
            TextureInfo info = new TextureInfo {
                TextureType = textureType,
                TextureFilePath = str,
                Invert = false,
                URepeat = true,
                VRepeat = true,
                UScale = 1.0,
                VScale = 1.0,
                UOffset = 0.0,
                VOffset = 0.0,
                WAngle = 0.0,
                IsShared = flag
            };
            if (this.GetBoolFromBoolean(asset, "unifiedbitmap_Invert", out flag2) || this.GetBoolFromBoolean(asset, "texture_Invert", out flag2))
            {
                info.Invert = flag2;
            }
            if (this.GetBoolFromBoolean(asset, "unifiedbitmap_URepeat", out flag2) || this.GetBoolFromBoolean(asset, "texture_URepeat", out flag2))
            {
                info.URepeat = flag2;
            }
            if (this.GetBoolFromBoolean(asset, "unifiedbitmap_VRepeat", out flag2) || this.GetBoolFromBoolean(asset, "texture_VRepeat", out flag2))
            {
                info.VRepeat = flag2;
            }
            if ((this.GetValueFromDistance(asset, "unifiedbitmap_RealWorldScaleX", out num) || this.GetValueFromDistance(asset, "texture_RealWorldScaleX", out num)) || (this.GetValueFromDouble(asset, "texture_UScale", out num) || this.GetValueFromDouble(asset, "unifiedbitmap_UScale", out num)))
            {
                info.UScale = num;
            }
            if ((this.GetValueFromDistance(asset, "unifiedbitmap_RealWorldScaleY", out num) || this.GetValueFromDistance(asset, "texture_RealWorldScaleY", out num)) || (this.GetValueFromDouble(asset, "texture_VScale", out num) || this.GetValueFromDouble(asset, "unifiedbitmap_VScale", out num)))
            {
                info.VScale = num;
            }
            if (this.GetValueFromDouble(asset, "texture_UOffset", out num) || this.GetValueFromDouble(asset, "unifiedbitmap_UOffset", out num))
            {
                info.UOffset = num;
            }
            if (this.GetValueFromDouble(asset, "texture_VOffset", out num) || this.GetValueFromDouble(asset, "unifiedbitmap_VOffset", out num))
            {
                info.VOffset = num;
            }
            if (this.GetValueFromDouble(asset, "unifiedbitmap_WAngle", out num) || this.GetValueFromDouble(asset, "texture_WAngle", out num))
            {
                info.WAngle = num;
            }
            return info;
        }

        public int OnMaterial(Document document, MaterialNode node)
        {
            string key = this.GetKey(document, node);
            if (this._Keys.ContainsKey(key))
            {
                return this._Keys[key];
            }
            int num = TableHelp.WriteMaterial(this.GetMaterialInfo(node), key);
            this._Keys.Add(key, num);
            return num;
        }

        public int OnMaterialForRPC()
        {
            string key = "rpc";
            if (this._Keys.ContainsKey(key))
            {
                return this._Keys[key];
            }
            MaterialInfo lmvItem = new MaterialInfo {
                Color = new Vector3D(0.6, 0.6, 0.6),
                IsMetal = false,
                BackfaceCulling = false
            };
            int num = TableHelp.WriteMaterial(lmvItem, key);
            this._Keys.Add(key, num);
            return num;
        }

        private string ParseTextureFilePathForCloud(string path, string defaultTexturePath, out bool isShared)
        {
            string str2;
            string str3;
            string str = path.Split(new char[] { '|' }).First<string>().Replace(@"\", "/");
            if (str.Contains(@"/\"))
            {
                str = str.Split(new string[] { @"/\" }, StringSplitOptions.RemoveEmptyEntries).Last<string>();
            }
            if ((str.StartsWith("1/Mats/", StringComparison.OrdinalIgnoreCase) || str.StartsWith("2/Mats/", StringComparison.OrdinalIgnoreCase)) || str.StartsWith("3/Mats/", StringComparison.OrdinalIgnoreCase))
            {
                isShared = true;
                str2 = Path.Combine(defaultTexturePath, str);
                str3 = Path.Combine("../../../mat-a/", str).ToLower();
            }
            else if (!str.Contains("/") && !str.Contains(@"\"))
            {
                isShared = true;
                str2 = Path.Combine(Path.Combine(defaultTexturePath, "3/Mats/"), str);
                if (File.Exists(str2))
                {
                    str3 = Path.Combine("../../../mat-a/3/Mats/", str);
                }
                else
                {
                    str2 = Path.Combine(Path.Combine(defaultTexturePath, "2/Mats/"), str);
                    if (File.Exists(str2))
                    {
                        str3 = Path.Combine("../../../mat-a/2/Mats/", str);
                    }
                    else
                    {
                        str2 = Path.Combine(Path.Combine(defaultTexturePath, "1/Mats/"), str);
                        str3 = Path.Combine("../../../mat-a/1/Mats/", str);
                    }
                }
                str3 = str3.ToLower();
            }
            else if (str.StartsWith(defaultTexturePath, StringComparison.OrdinalIgnoreCase))
            {
                isShared = true;
                str2 = str;
                str3 = Path.Combine("../../../mat-a/", str.Substring(defaultTexturePath.Length)).ToLower();
            }
            else
            {
                isShared = false;
                str2 = str3 = str;
            }
            if (!File.Exists(str2))
            {
                return null;
            }
            return str3;
        }

        private string ParseTextureFilePathForLocal(string path, string defaultTexturePath)
        {
            string str2;
            string str = path.Split(new char[] { '|' }).First<string>();
            if (str.Contains(@"/\"))
            {
                str = str.Split(new string[] { @"/\" }, StringSplitOptions.RemoveEmptyEntries).Last<string>();
            }
            str = str.Replace(@"\", "/");
            if ((str.StartsWith("1/Mats/", StringComparison.OrdinalIgnoreCase) || str.StartsWith("2/Mats/", StringComparison.OrdinalIgnoreCase)) || str.StartsWith("3/Mats/", StringComparison.OrdinalIgnoreCase))
            {
                str2 = Path.Combine(defaultTexturePath, str);
            }
            else if (!str.Contains("/") && !str.Contains(@"\"))
            {
                str2 = Path.Combine(Path.Combine(defaultTexturePath, "3/Mats/"), str);
                if (!File.Exists(str2))
                {
                    str2 = Path.Combine(Path.Combine(defaultTexturePath, "2/Mats/"), str);
                    if (!File.Exists(str2))
                    {
                        str2 = Path.Combine(Path.Combine(defaultTexturePath, "1/Mats/"), str);
                    }
                }
            }
            else
            {
                str2 = str;
            }
            if (!File.Exists(str2))
            {
                return null;
            }
            return str2;
        }

        private class DigestDiffuseColor : TLMaterialHelper.DigestProperties
        {
            public IList<string> alphaNames;
            public double[] color = new double[4];
            public IList<string> colorNames;
            public bool foundColor;
            public bool foundTransparency;

            public DigestDiffuseColor(IList<string> colorNames, IList<string> transparencyNames)
            {
                this.colorNames = colorNames;
                this.alphaNames = transparencyNames;
                this.foundColor = false;
                this.foundTransparency = false;
                this.color[3] = 1.0;
            }

            public override void digest(AssetProperty property)
            {
                if (this.colorNames.IndexOf(property.Name) != -1)
                {
                    if (property.Type == AssetPropertyType.APT_DoubleArray4d)
                    {
                        AssetPropertyDoubleArray4d arrayd = property as AssetPropertyDoubleArray4d;
                        this.color[0] = arrayd.Value.get_Item(0);
                        this.color[1] = arrayd.Value.get_Item(1);
                        this.color[2] = arrayd.Value.get_Item(2);
                        this.foundColor = true;
                    }
                }
                else if ((this.alphaNames.IndexOf(property.Name) != -1) && (property.Type == AssetPropertyType.APT_Float))
                {
                    this.color[3] = 1.0 - (property as AssetPropertyFloat).Value;
                    this.foundTransparency = true;
                }
            }
        }

        private class DigestProperties
        {
            public virtual void digest(AssetProperty property)
            {
            }
        }
    }
}

