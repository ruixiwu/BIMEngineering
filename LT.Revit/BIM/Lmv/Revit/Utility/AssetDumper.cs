using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.Utility;
using Newtonsoft.Json.Linq;

namespace BIM.Lmv.Revit.Utility
{
    internal static class AssetDumper
    {
        public static void Dump(this Asset value, string filePath)
        {
            var json = new JObject();
            value.FillJson(json);
            var contents = json.ToString();
            File.WriteAllText(filePath, contents, Encoding.UTF8);
        }

        public static void FillJson(this Asset value, JObject json)
        {
            json["AssetType"] = value.AssetType.ToString();
            json["LibraryName"] = value.LibraryName;
            json["Title"] = value.Title;
            ((AssetProperties) value).FillJson(json);
        }

        public static void FillJson(this AssetProperties value, JObject json)
        {
            ((AssetProperty) value).FillJson(json);
            if (value.Size > 0)
            {
                var obj2 = new JObject();
                for (var i = 0; i < value.Size; i++)
                {
                    var property = value[i];
                    var obj3 = new JObject();
                    property.FillJson(obj3);
                    obj2[property.Name] = obj3;
                }
                json["Properties"] = obj2;
            }
        }

        public static void FillJson(this AssetProperty value, JObject json)
        {
            json["Name"] = value.Name;
            json["Type"] = AssetProperty.GetTypeName(value.Type);
            if (value.Type != AssetPropertyType.APT_Asset)
            {
                value.FillJsonValue(json, true);
            }
            if (value.NumberOfConnectedProperties > 0)
            {
                var allConnectedProperties = value.GetAllConnectedProperties();
                var obj2 = new JObject();
                foreach (var property in allConnectedProperties)
                {
                    var obj3 = new JObject();
                    property.FillJsonValue(obj3, false);
                    obj2[property.Name] = obj3;
                }
                json["ConnectedProperties"] = obj2;
            }
        }

        public static void FillJsonValue(this AssetProperty value, JObject json, bool onlyValue)
        {
            switch (value.Type)
            {
                case AssetPropertyType.APT_Properties:
                    if (!onlyValue)
                    {
                        ((AssetProperties) value).FillJson(json);
                    }
                    return;

                case AssetPropertyType.APT_Boolean:
                {
                    var flag = (AssetPropertyBoolean) value;
                    json["Value"] = flag.Value;
                    return;
                }
                case AssetPropertyType.APT_Enum:
                {
                    var enum2 = (AssetPropertyEnum) value;
                    json["Value"] = enum2.Value;
                    return;
                }
                case AssetPropertyType.APT_Integer:
                {
                    var integer = (AssetPropertyInteger) value;
                    json["Value"] = integer.Value;
                    return;
                }
                case AssetPropertyType.APT_Float:
                {
                    var num = (AssetPropertyFloat) value;
                    json["Value"] = num.Value;
                    return;
                }
                case AssetPropertyType.APT_Double:
                {
                    var num2 = (AssetPropertyDouble) value;
                    json["Value"] = num2.Value;
                    return;
                }
                case AssetPropertyType.APT_DoubleArray2d:
                {
                    var arrayd = (AssetPropertyDoubleArray2d) value;
                    json["Value"] = arrayd.Value.ToJson();
                    return;
                }
                case AssetPropertyType.APT_DoubleArray3d:
                {
                    var arrayd2 = (AssetPropertyDoubleArray3d) value;
                    json["Value"] = arrayd2.Value.ToJson();
                    return;
                }
                case AssetPropertyType.APT_DoubleArray4d:
                {
                    var arrayd3 = (AssetPropertyDoubleArray4d) value;
                    json["Value"] = arrayd3.Value.ToJson();
                    return;
                }
                case AssetPropertyType.APT_Double44:
                {
                    var num3 = (AssetPropertyDouble) value;
                    json["Value"] = num3.Value;
                    return;
                }
                case AssetPropertyType.APT_String:
                {
                    var str = (AssetPropertyString) value;
                    json["Value"] = str.Value;
                    return;
                }
                case AssetPropertyType.APT_Time:
                {
                    var time = (AssetPropertyTime) value;
                    json["Value"] = time.Value;
                    return;
                }
                case AssetPropertyType.APT_Distance:
                {
                    var distance = (AssetPropertyDistance) value;
                    json["Value"] = distance.Value;
                    json["DisplayUnitType"] = distance.DisplayUnitType.ToString();
                    return;
                }
                case AssetPropertyType.APT_Asset:
                    if (!onlyValue)
                    {
                        ((Asset) value).FillJson(json);
                    }
                    return;

                case AssetPropertyType.APT_Reference:
                    if (!onlyValue)
                    {
                        ((AssetPropertyReference) value).FillJson(json);
                    }
                    return;

                case AssetPropertyType.APT_Int64:
                {
                    var num4 = (AssetPropertyInt64) value;
                    json["Value"] = num4.Value;
                    return;
                }
                case AssetPropertyType.APT_UInt64:
                {
                    var num5 = (AssetPropertyUInt64) value;
                    json["Value"] = num5.Value;
                    return;
                }
                case AssetPropertyType.APT_List:
                {
                    var list = (AssetPropertyList) value;
                    var array2 = new JArray();
                    foreach (var property in list.GetValue())
                    {
                        var obj2 = new JObject();
                        property.FillJson(obj2);
                        array2.Add(obj2);
                    }
                    json["Value"] = array2;
                    value.FillJson(json);
                    return;
                }
                case AssetPropertyType.APT_FloatArray:
                    json["Value"] = ((AssetPropertyFloatArray) value).GetValue().ToJson();
                    return;
            }
            throw new ArgumentOutOfRangeException();
        }

        public static JObject ToJson(this Color value)
        {
            if (value == null)
            {
                return null;
            }
            var obj2 = new JObject();
            obj2["Red"] = value.Red;
            obj2["Green"] = value.Green;
            obj2["Blue"] = value.Blue;
            return obj2;
        }

        public static JArray ToJson(this DoubleArray value)
        {
            if (value == null)
            {
                return null;
            }
            var array = new JArray();
            foreach (double num in value)
            {
                array.Add(num);
            }
            return array;
        }

        public static JArray ToJson(this IList<float> value)
        {
            if (value == null)
            {
                return null;
            }
            var array = new JArray();
            foreach (var num in value)
            {
                array.Add(num);
            }
            return array;
        }
    }
}