using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.Utility;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Types;

namespace BIM.Lmv.Revit.Helpers
{
    internal class TLMaterialHelper
    {
        public const string DEFAULT_TEXTURE_PATH =
            @"C:\Program Files (x86)\Common Files\Autodesk Shared\Materials\Textures\";

        private readonly bool _IncludeTexture;
        private readonly Dictionary<string, int> _Keys = new Dictionary<string, int>();
        private readonly ExportTarget _Target;
        private readonly bool _UseRealColor;

        public TLMaterialHelper(ExportTarget target, bool includeTexture, View3D view)
        {
            _Target = target;
            _IncludeTexture = includeTexture;
            _UseRealColor = (view.DisplayStyle == DisplayStyle.Realistic) ||
                            (view.DisplayStyle == DisplayStyle.RealisticWithEdges) ||
                            (view.DisplayStyle == DisplayStyle.Raytrace);
        }

        private void CollectProperties(AssetProperty property, IList<string> excluded, DigestProperties digest)
        {
            if ((excluded == null) ||
                !excluded.Any(str => property.Name.IndexOf(str, StringComparison.Ordinal) != -1))
            {
                if (property.Type == AssetPropertyType.APT_List)
                {
                    foreach (var property2 in ((AssetPropertyList) property).GetValue())
                    {
                        CollectProperties(property2, excluded, digest);
                    }
                }
                else if (property.Type == AssetPropertyType.APT_Properties)
                {
                    var properties = property as AssetProperties;
                    for (var j = 0; j < properties.Size; j++)
                    {
                        CollectProperties(properties[j], excluded, digest);
                    }
                }
                else if (property.Type == AssetPropertyType.APT_Asset)
                {
                    var asset = property as Asset;
                    for (var k = 0; k < asset.Size; k++)
                    {
                        CollectProperties(asset[k], excluded, digest);
                    }
                }
                else
                {
                    digest.digest(property);
                }
                for (var i = 0; i < property.NumberOfConnectedProperties; i++)
                {
                    CollectProperties(property.GetConnectedProperty(i), excluded, digest);
                }
            }
        }

        private DigestDiffuseColor FindColor(Asset asset)
        {
            var str = asset.Name.Replace("Schema", "").ToLower();
            var colorNames = new List<string>
            {
                str + "_color",
                str + "_diffuse",
                str + "_transmittance_map",
                str + "_tintcolor"
            };
            var transparencyNames = new List<string>
            {
                str + "_reflectance"
            };
            var digest = new DigestDiffuseColor(colorNames, transparencyNames);
            for (var i = 0; i < asset.Size; i++)
            {
                CollectProperties(asset[i], null, digest);
            }
            return digest;
        }

        private Vector3D FromColor(Color c)
        {
            return new Vector3D(c.Red/255.0, c.Green/255.0, c.Blue/255.0);
        }

        private bool GetBoolFromBoolean(Asset asset, string propName, out bool result)
        {
            result = false;
            var property = asset[propName];
            if (property == null)
            {
                return false;
            }
            result = ((AssetPropertyBoolean) property).Value;
            return true;
        }

        private bool GetColorFromDoubleArray4d(Asset asset, string propName, out Vector3D result)
        {
            var property = asset[propName];
            if (property == null)
            {
                result = null;
                return false;
            }
            var array = ((AssetPropertyDoubleArray4d) property).Value;
            result = new Vector3D(array.get_Item(0), array.get_Item(1), array.get_Item(2));
            return true;
        }

        private string GetKey(Document document, MaterialNode node)
        {
            if (node.MaterialId == ElementId.InvalidElementId)
            {
                return
                    string.Concat("M_", node.Color.Red, "_", node.Color.Green, "_", node.Color.Blue, "_",
                        node.Transparency, "_", node.Glossiness, "_", node.Smoothness);
            }
            return document.Title + "_" + node.MaterialId.IntegerValue;
        }

        private MaterialInfo GetMaterialInfo(MaterialNode node)
        {
            if (!(node.MaterialId == ElementId.InvalidElementId))
            {
                return GetTextureMaterialInfo(node);
            }
            return GetSimpleMaterialInfo(node);
        }

        private MaterialInfo GetSimpleMaterialInfo(MaterialNode node)
        {
            return new MaterialInfo
            {
                Color = FromColor(node.Color),
                IsMetal = false,
                BackfaceCulling = false,
                Shininess = node.Glossiness,
                Transparent = node.Transparency
            };
        }

        private bool GetTextureFilePath(Asset asset, string propName, out string textureFilePath, out bool isShared)
        {
            textureFilePath = null;
            isShared = false;
            var property = asset[propName];
            if (property == null)
            {
                return false;
            }
            var str = ((AssetPropertyString) property).Value;
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            if (_Target == ExportTarget.CloudPackage)
            {
                var str2 = ParseTextureFilePathForCloud(str,
                    @"C:\Program Files (x86)\Common Files\Autodesk Shared\Materials\Textures\", out isShared);
                if (str2 == null)
                {
                    return false;
                }
                textureFilePath = str2;
            }
            else
            {
                var str3 = ParseTextureFilePathForLocal(str,
                    @"C:\Program Files (x86)\Common Files\Autodesk Shared\Materials\Textures\");
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
            var asset = node.HasOverriddenAppearance ? node.GetAppearanceOverride() : node.GetAppearance();
            var info = new MaterialInfo();
            if (node.Color.IsValid)
            {
                info.Color = FromColor(node.Color);
            }
            else if (GetColorFromDoubleArray4d(asset, "generic_diffuse", out vectord))
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
            if (_UseRealColor)
            {
                bool flag;
                if (GetColorFromDoubleArray4d(asset, "generic_diffuse", out vectord))
                {
                    info.Color = vectord;
                }
                if (GetBoolFromBoolean(asset, "generic_backface_cull", out flag))
                {
                    info.BackfaceCulling = flag;
                }
            }
            if (GetColorFromDoubleArray4d(asset, "generic_ambient", out vectord))
            {
                info.Ambient = vectord;
            }
            if (GetColorFromDoubleArray4d(asset, "generic_specular", out vectord))
            {
                info.Specular = vectord;
            }
            if (GetColorFromDoubleArray4d(asset, "generic_emissive", out vectord))
            {
                info.Emissive = vectord;
            }
            if (GetValueFromDouble(asset, "generic_reflectivity_at_0deg", out num) ||
                GetValueFromDouble(asset, "generic_reflectivity_at_90deg", out num))
            {
                info.Reflectivity = num;
            }
            if (GetValueFromDouble(asset, "generic_bump_amount", out num))
            {
                info.BumpAmount = num/10.0;
            }
            if (GetValueFromDouble(asset, "generic_glossiness", out num))
            {
                info.Shininess = num;
            }
            if (GetValueFromDouble(asset, "generic_transparency", out num))
            {
                info.Transparent = num;
            }
            var color = FindColor(asset);
            if (color.foundColor)
            {
                info.Color = new Vector3D(Math.Min(1.0, color.color[0]*1.0),
                    Math.Min(1.0, color.color[1]*1.0),
                    Math.Min(1.0, color.color[2]*1.0));
            }
            if (color.foundTransparency)
            {
                info.Transparent = color.color[3];
            }
            else if (asset.Name == "PlasticVinylSchema")
            {
                int num2;
                if (GetValueFromInteger(asset, "plasticvinyl_type", out num2) && (num2 == 1))
                {
                    info.Transparent = 0.4825;
                }
            }
            else
            {
                var flag1 = asset.Name == "WaterSchema";
            }
            if (_IncludeTexture)
            {
                for (var i = 0; i < asset.Size; i++)
                {
                    var property = asset[i];
                    if ((property.Name == "generic_bump") || property.Name.EndsWith("_bump_map") ||
                        property.Name.EndsWith("_pattern_shader") || property.Name.EndsWith("_pattern_map") ||
                        property.Name.EndsWith("_bm_map"))
                    {
                        var property2 = property.GetConnectedProperty("UnifiedBitmapSchema") ??
                                        (property.NumberOfConnectedProperties > 0
                                            ? property.GetConnectedProperty(0)
                                            : null);
                        if (property2 != null)
                        {
                            var item = GextTextureInfo(property2 as Asset, TextureType.Bump);
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
                for (var j = 0; j < asset.Size; j++)
                {
                    var property3 = asset[j];
                    if ((property3.Name == "generic_diffuse") || property3.Name.EndsWith("_color"))
                    {
                        var property4 = property3.GetConnectedProperty("UnifiedBitmapSchema") ??
                                        (property3.NumberOfConnectedProperties > 0
                                            ? property3.GetConnectedProperty(0)
                                            : null);
                        if (property4 != null)
                        {
                            var info3 = GextTextureInfo(property4 as Asset, TextureType.Diffuse);
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
                if ((info.Textures != null) &&
                    info.Textures.Any(x => x.TextureType == TextureType.Diffuse))
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
            var property = asset[propName];
            if (property == null)
            {
                return false;
            }
            var num = ((AssetPropertyDistance) property).Value;
            var displayUnitType = ((AssetPropertyDistance) property).DisplayUnitType;
            result = 12.0/UnitUtils.Convert(num, displayUnitType, DisplayUnitType.DUT_DECIMAL_INCHES);
            return true;
        }

        private bool GetValueFromDouble(Asset asset, string propName, out double result)
        {
            result = 0.0;
            var property = asset[propName];
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
            var property = asset[propName];
            if ((property != null) && property is AssetPropertyInteger)
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
            if (!GetTextureFilePath(asset, "unifiedbitmap_Bitmap", out str, out flag))
            {
                return null;
            }
            var info = new TextureInfo
            {
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
            if (GetBoolFromBoolean(asset, "unifiedbitmap_Invert", out flag2) ||
                GetBoolFromBoolean(asset, "texture_Invert", out flag2))
            {
                info.Invert = flag2;
            }
            if (GetBoolFromBoolean(asset, "unifiedbitmap_URepeat", out flag2) ||
                GetBoolFromBoolean(asset, "texture_URepeat", out flag2))
            {
                info.URepeat = flag2;
            }
            if (GetBoolFromBoolean(asset, "unifiedbitmap_VRepeat", out flag2) ||
                GetBoolFromBoolean(asset, "texture_VRepeat", out flag2))
            {
                info.VRepeat = flag2;
            }
            if (GetValueFromDistance(asset, "unifiedbitmap_RealWorldScaleX", out num) ||
                GetValueFromDistance(asset, "texture_RealWorldScaleX", out num) ||
                GetValueFromDouble(asset, "texture_UScale", out num) ||
                GetValueFromDouble(asset, "unifiedbitmap_UScale", out num))
            {
                info.UScale = num;
            }
            if (GetValueFromDistance(asset, "unifiedbitmap_RealWorldScaleY", out num) ||
                GetValueFromDistance(asset, "texture_RealWorldScaleY", out num) ||
                GetValueFromDouble(asset, "texture_VScale", out num) ||
                GetValueFromDouble(asset, "unifiedbitmap_VScale", out num))
            {
                info.VScale = num;
            }
            if (GetValueFromDouble(asset, "texture_UOffset", out num) ||
                GetValueFromDouble(asset, "unifiedbitmap_UOffset", out num))
            {
                info.UOffset = num;
            }
            if (GetValueFromDouble(asset, "texture_VOffset", out num) ||
                GetValueFromDouble(asset, "unifiedbitmap_VOffset", out num))
            {
                info.VOffset = num;
            }
            if (GetValueFromDouble(asset, "unifiedbitmap_WAngle", out num) ||
                GetValueFromDouble(asset, "texture_WAngle", out num))
            {
                info.WAngle = num;
            }
            return info;
        }

        public int OnMaterial(Document document, MaterialNode node)
        {
            var key = GetKey(document, node);
            if (_Keys.ContainsKey(key))
            {
                return _Keys[key];
            }
            var num = TableHelp.WriteMaterial(GetMaterialInfo(node), key);
            _Keys.Add(key, num);
            return num;
        }

        public int OnMaterialForRPC()
        {
            var key = "rpc";
            if (_Keys.ContainsKey(key))
            {
                return _Keys[key];
            }
            var lmvItem = new MaterialInfo
            {
                Color = new Vector3D(0.6, 0.6, 0.6),
                IsMetal = false,
                BackfaceCulling = false
            };
            var num = TableHelp.WriteMaterial(lmvItem, key);
            _Keys.Add(key, num);
            return num;
        }

        private string ParseTextureFilePathForCloud(string path, string defaultTexturePath, out bool isShared)
        {
            string str2;
            string str3;
            var str = path.Split('|').First().Replace(@"\", "/");
            if (str.Contains(@"/\"))
            {
                str = str.Split(new[] {@"/\"}, StringSplitOptions.RemoveEmptyEntries).Last();
            }
            if (str.StartsWith("1/Mats/", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("2/Mats/", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("3/Mats/", StringComparison.OrdinalIgnoreCase))
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
            var str = path.Split('|').First();
            if (str.Contains(@"/\"))
            {
                str = str.Split(new[] {@"/\"}, StringSplitOptions.RemoveEmptyEntries).Last();
            }
            str = str.Replace(@"\", "/");
            if (str.StartsWith("1/Mats/", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("2/Mats/", StringComparison.OrdinalIgnoreCase) ||
                str.StartsWith("3/Mats/", StringComparison.OrdinalIgnoreCase))
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

        private class DigestDiffuseColor : DigestProperties
        {
            public readonly IList<string> alphaNames;
            public readonly double[] color = new double[4];
            public readonly IList<string> colorNames;
            public bool foundColor;
            public bool foundTransparency;

            public DigestDiffuseColor(IList<string> colorNames, IList<string> transparencyNames)
            {
                this.colorNames = colorNames;
                alphaNames = transparencyNames;
                foundColor = false;
                foundTransparency = false;
                color[3] = 1.0;
            }

            public override void digest(AssetProperty property)
            {
                if (colorNames.IndexOf(property.Name) != -1)
                {
                    if (property.Type == AssetPropertyType.APT_DoubleArray4d)
                    {
                        var arrayd = property as AssetPropertyDoubleArray4d;
                        color[0] = arrayd.Value.get_Item(0);
                        color[1] = arrayd.Value.get_Item(1);
                        color[2] = arrayd.Value.get_Item(2);
                        foundColor = true;
                    }
                }
                else if ((alphaNames.IndexOf(property.Name) != -1) && (property.Type == AssetPropertyType.APT_Float))
                {
                    color[3] = 1.0 - (property as AssetPropertyFloat).Value;
                    foundTransparency = true;
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