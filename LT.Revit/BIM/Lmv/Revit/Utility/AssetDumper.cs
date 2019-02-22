namespace BIM.Lmv.Revit.Utility
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.Utility;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal static class AssetDumper
    {
        public static void Dump(this Asset value, string filePath)
        {
            JObject json = new JObject();
            value.FillJson(json);
            string contents = json.ToString();
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
                JObject obj2 = new JObject();
                for (int i = 0; i < value.Size; i++)
                {
                    AssetProperty property = value[i];
                    JObject obj3 = new JObject();
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
                IList<AssetProperty> allConnectedProperties = value.GetAllConnectedProperties();
                JObject obj2 = new JObject();
                foreach (AssetProperty property in allConnectedProperties)
                {
                    JObject obj3 = new JObject();
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
                    AssetPropertyBoolean flag = (AssetPropertyBoolean) value;
                    json["Value"] = flag.Value;
                    return;
                }
                case AssetPropertyType.APT_Enum:
                {
                    AssetPropertyEnum enum2 = (AssetPropertyEnum) value;
                    json["Value"] = enum2.Value;
                    return;
                }
                case AssetPropertyType.APT_Integer:
                {
                    AssetPropertyInteger integer = (AssetPropertyInteger) value;
                    json["Value"] = integer.Value;
                    return;
                }
                case AssetPropertyType.APT_Float:
                {
                    AssetPropertyFloat num = (AssetPropertyFloat) value;
                    json["Value"] = num.Value;
                    return;
                }
                case AssetPropertyType.APT_Double:
                {
                    AssetPropertyDouble num2 = (AssetPropertyDouble) value;
                    json["Value"] = num2.Value;
                    return;
                }
                case AssetPropertyType.APT_DoubleArray2d:
                {
                    AssetPropertyDoubleArray2d arrayd = (AssetPropertyDoubleArray2d) value;
                    json["Value"] = arrayd.Value.ToJson();
                    return;
                }
                case AssetPropertyType.APT_DoubleArray3d:
                {
                    AssetPropertyDoubleArray3d arrayd2 = (AssetPropertyDoubleArray3d) value;
                    json["Value"] = arrayd2.Value.ToJson();
                    return;
                }
                case AssetPropertyType.APT_DoubleArray4d:
                {
                    AssetPropertyDoubleArray4d arrayd3 = (AssetPropertyDoubleArray4d) value;
                    json["Value"] = arrayd3.Value.ToJson();
                    return;
                }
                case AssetPropertyType.APT_Double44:
                {
                    AssetPropertyDouble num3 = (AssetPropertyDouble) value;
                    json["Value"] = num3.Value;
                    return;
                }
                case AssetPropertyType.APT_String:
                {
                    AssetPropertyString str = (AssetPropertyString) value;
                    json["Value"] = str.Value;
                    return;
                }
                case AssetPropertyType.APT_Time:
                {
                    AssetPropertyTime time = (AssetPropertyTime) value;
                    json["Value"] = time.Value;
                    return;
                }
                case AssetPropertyType.APT_Distance:
                {
                    AssetPropertyDistance distance = (AssetPropertyDistance) value;
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
                    AssetPropertyInt64 num4 = (AssetPropertyInt64) value;
                    json["Value"] = num4.Value;
                    return;
                }
                case AssetPropertyType.APT_UInt64:
                {
                    AssetPropertyUInt64 num5 = (AssetPropertyUInt64) value;
                    json["Value"] = num5.Value;
                    return;
                }
                case AssetPropertyType.APT_List:
                {
                    AssetPropertyList list = (AssetPropertyList) value;
                    JArray array2 = new JArray();
                    foreach (AssetProperty property in list.GetValue())
                    {
                        JObject obj2 = new JObject();
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
            return new JObject { 
                ["Red"] = value.Red,
                ["Green"] = value.Green,
                ["Blue"] = value.Blue
            };
        }

        public static JArray ToJson(this DoubleArray value)
        {
            if (value == null)
            {
                return null;
            }
            JArray array = new JArray();
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
            JArray array = new JArray();
            foreach (float num in value)
            {
                array.Add(num);
            }
            return array;
        }
    }
}

