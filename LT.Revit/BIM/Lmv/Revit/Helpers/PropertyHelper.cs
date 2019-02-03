using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.Revit.DB;
using BIM.Lmv.Types;

namespace BIM.Lmv.Revit.Helpers
{
    internal class PropertyHelper
    {
        private readonly IDataExport _Exporter;
        private readonly bool _IncludeProperty;
        private readonly Dictionary<string, int> _Keys = new Dictionary<string, int>();

        public PropertyHelper(IDataExport exporter, Document document, bool includeProperty)
        {
            if (exporter == null)
            {
                throw new ArgumentNullException("exporter");
            }
            _Exporter = exporter;
            _IncludeProperty = includeProperty;
            var projectInformation = document.ProjectInformation;
            if (projectInformation == null)
            {
                RootId = InsertNode("ROOT", "Model", "doc_null", null, -1, null);
            }
            else
            {
                RootId = InsertNode("ROOT", "Model", "doc_" + projectInformation.UniqueId,
                    projectInformation.Parameters, -1, null);
            }
        }

        public int RootId { get; }

        private string GetFamilyName(Element element, string defaultValue)
        {
            foreach (Parameter parameter in element.Parameters)
            {
                if (parameter.Definition is InternalDefinition)
                {
                    var definition = parameter.Definition as InternalDefinition;
                    if (definition.BuiltInParameter == BuiltInParameter.ELEM_FAMILY_PARAM)
                    {
                        return parameter.AsValueString();
                    }
                }
            }
            return defaultValue;
        }

        private void GetPropInfo(Parameter p, out int type, out string value, out string unit)
        {
            unit = string.Empty;
            if ((p.StorageType == StorageType.Integer) && (p.Definition.ParameterType == ParameterType.YesNo))
            {
                type = 1;
                value = p.AsInteger() == 1 ? "true" : "false";
            }
            else
            {
                value = p.AsValueString();
                switch (p.StorageType)
                {
                    case StorageType.None:
                        if (value == null)
                        {
                            value = string.Empty;
                        }
                        type = 20;
                        return;

                    case StorageType.Integer:
                        if (value != null)
                        {
                            int num;
                            var strArray = value.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                            if (strArray.Length == 0)
                            {
                                value = p.AsInteger().ToString();
                                type = 2;
                                return;
                            }
                            value = strArray[0];
                            if (strArray.Length > 1)
                            {
                                unit = strArray[1];
                            }
                            type = int.TryParse(value, out num) ? 2 : 20;
                            return;
                        }
                        value = p.AsInteger().ToString();
                        type = 2;
                        return;

                    case StorageType.Double:
                        if (value != null)
                        {
                            double num2;
                            var strArray2 = value.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                            if (strArray2.Length == 0)
                            {
                                value = p.AsDouble().ToString(CultureInfo.InvariantCulture);
                                type = 3;
                                return;
                            }
                            value = strArray2[0];
                            if (strArray2.Length > 1)
                            {
                                unit = strArray2[1];
                            }
                            type = double.TryParse(value, out num2) ? 3 : 20;
                            return;
                        }
                        value = p.AsDouble().ToString(CultureInfo.InvariantCulture);
                        type = 3;
                        return;

                    case StorageType.String:
                        if (value == null)
                        {
                            var name = p.Definition.Name;
                            if ((name != "类型名称") && (name != "产品编码") && (name != "构件编码") && !name.Contains("构件库编码") &&
                                !name.Contains("构件库分类") && !name.Contains("分类编码") && !name.Contains("单价"))
                            {
                                value = string.Empty;
                                break;
                            }
                            value = p.AsString();
                            if (value == null)
                            {
                                value = string.Empty;
                            }
                            else
                            {
                                var index = value.IndexOf('\r');
                                if (index > 0)
                                {
                                    value = value.Substring(0, index);
                                }
                            }
                            if (value != null)
                            {
                                var length = value.IndexOf('\n');
                                if (length > 0)
                                {
                                    value = value.Substring(0, length);
                                }
                            }
                        }
                        break;

                    case StorageType.ElementId:
                        if (value == null)
                        {
                            value = string.Empty;
                        }
                        type = 20;
                        return;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                type = 20;
            }
        }

        private List<PropItem> GetPropItems(ParameterSet parameterSet)
        {
            if (parameterSet == null)
            {
                return null;
            }
            var list = new List<PropItem>(parameterSet.Size);
            foreach (Parameter parameter in parameterSet)
            {
                PropDef def;
                string str;
                ProcessParameter(parameter, out def, out str);
                list.Add(new PropItem(def, str));
            }
            return list;
        }

        private int InsertNode(string key, string name, string uid, ParameterSet parameterSet, int parentId,
            Element element)
        {
            int num;
            if (!_Keys.TryGetValue(key, out num))
            {
                var props = _IncludeProperty ? GetPropItems(parameterSet) : null;
                num = _Exporter.OnNode(key, name, uid, parentId, props);
                _Keys.Add(key, num);
            }
            return num;
        }

        public int OnElement(Element element)
        {
            if (element is FamilyInstance)
            {
                return ProcessElementFamilyInstance(element as FamilyInstance, RootId);
            }
            if (element is Wall)
            {
                return ProcessElementWall(element as Wall, RootId);
            }
            return ProcessElementInstance(element, RootId);
        }

        private int ProcessElementFamilyInstance(FamilyInstance element, int rootId)
        {
            var name = element.Category.Name;
            var key = name;
            var uid = element.Category.Id.IntegerValue.ToString();
            var parentId = InsertNode(key, name, uid, null, rootId, null);
            var str4 = element.Symbol.Family.Name;
            var str5 = key + ":" + str4;
            var uniqueId = element.Symbol.Family.UniqueId;
            var num2 = InsertNode(str5, str4, uniqueId, null, parentId, null);
            var str7 = element.Symbol.Name;
            var str8 = str5 + ":" + str7;
            var str9 = element.Symbol.UniqueId;
            var num3 = InsertNode(str8, str7, str9, element.Symbol.Parameters, num2, null);
            var str10 = string.Concat(str4, " [", element.Id.IntegerValue, "]");
            var str11 = str8 + ":" + str10;
            var str12 = element.UniqueId;
            return InsertNode(str11, str10, str12, element.Parameters, num3, element);
        }

        private int ProcessElementInstance(Element element, int rootId)
        {
            int num;
            string str;
            if (element.Category == null)
            {
                str = "ROOT";
                num = rootId;
            }
            else
            {
                var str2 = element.Category.Name;
                str = str2;
                var str3 = element.Category.Id.IntegerValue.ToString();
                num = InsertNode(str, str2, str3, null, rootId, null);
            }
            var name = element.Name;
            var key = str + ":" + name;
            var uid = "";
            var parentId = InsertNode(key, name, uid, null, num, null);
            var str7 = string.Concat(name, " [", element.Id.IntegerValue, "]");
            var str8 = key + ":" + str7;
            var uniqueId = element.UniqueId;
            return InsertNode(str8, str7, uniqueId, element.Parameters, parentId, element);
        }

        private int ProcessElementWall(Wall element, int rootId)
        {
            var name = element.Category.Name;
            var key = name;
            var uid = element.Category.Id.IntegerValue.ToString();
            var parentId = InsertNode(key, name, uid, null, rootId, null);
            var familyName = GetFamilyName(element, element.WallType.Kind.ToString());
            var str5 = key + ":" + familyName;
            var str6 = "";
            var num2 = InsertNode(str5, familyName, str6, null, parentId, null);
            var str7 = element.WallType.Name;
            var str8 = str5 + ":" + str7;
            var uniqueId = element.WallType.UniqueId;
            var num3 = InsertNode(str8, str7, uniqueId, element.WallType.Parameters, num2, null);
            var str10 = string.Concat(familyName, " [", element.Id.IntegerValue, "]");
            var str11 = str8 + ":" + str10;
            var str12 = element.UniqueId;
            return InsertNode(str11, str10, str12, element.Parameters, num3, element);
        }

        private void ProcessParameter(Parameter p, out PropDef propDef, out string propValue)
        {
            int num;
            string str3;
            string str4;
            var name = p.Definition.Name;
            var labelFor = LabelUtils.GetLabelFor(p.Definition.ParameterGroup);
            GetPropInfo(p, out num, out str3, out str4);
            propDef = new PropDef(name, labelFor, (PropDataType) num, str4, false);
            propValue = str3;
        }
    }
}