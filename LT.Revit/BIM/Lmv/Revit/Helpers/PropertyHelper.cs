namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using BIM.Lmv;
    using BIM.Lmv.Types;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal class PropertyHelper
    {
        private readonly IDataExport _Exporter;
        private readonly bool _IncludeProperty;
        private readonly Dictionary<string, int> _Keys = new Dictionary<string, int>();
        private readonly int _RootId;

        public PropertyHelper(IDataExport exporter, Document document, bool includeProperty)
        {
            if (exporter == null)
            {
                throw new ArgumentNullException("exporter");
            }
            this._Exporter = exporter;
            this._IncludeProperty = includeProperty;
            ProjectInfo projectInformation = document.ProjectInformation;
            if (projectInformation == null)
            {
                this._RootId = this.InsertNode("ROOT", "Model", "doc_null", null, -1, null);
            }
            else
            {
                this._RootId = this.InsertNode("ROOT", "Model", "doc_" + projectInformation.UniqueId, projectInformation.Parameters, -1, null);
            }
        }

        private string GetFamilyName(Element element, string defaultValue)
        {
            foreach (Parameter parameter in element.Parameters)
            {
                if (parameter.Definition is InternalDefinition)
                {
                    InternalDefinition definition = parameter.Definition as InternalDefinition;
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
                value = (p.AsInteger() == 1) ? "true" : "false";
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
                            string[] strArray = value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
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
                            string[] strArray2 = value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
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
                            string name = p.Definition.Name;
                            if ((((name != "类型名称") && (name != "产品编码")) && ((name != "构件编码") && !name.Contains("构件库编码"))) && ((!name.Contains("构件库分类") && !name.Contains("分类编码")) && !name.Contains("单价")))
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
                                int index = value.IndexOf('\r');
                                if (index > 0)
                                {
                                    value = value.Substring(0, index);
                                }
                            }
                            if (value != null)
                            {
                                int length = value.IndexOf('\n');
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
            List<PropItem> list = new List<PropItem>(parameterSet.Size);
            foreach (Parameter parameter in parameterSet)
            {
                PropDef def;
                string str;
                this.ProcessParameter(parameter, out def, out str);
                list.Add(new PropItem(def, str));
            }
            return list;
        }

        private int InsertNode(string key, string name, string uid, ParameterSet parameterSet, int parentId, Element element)
        {
            int num;
            if (!this._Keys.TryGetValue(key, out num))
            {
                List<PropItem> props = this._IncludeProperty ? this.GetPropItems(parameterSet) : null;
                num = this._Exporter.OnNode(key, name, uid, parentId, props);
                this._Keys.Add(key, num);
            }
            return num;
        }

        public int OnElement(Element element)
        {
            if (element is FamilyInstance)
            {
                return this.ProcessElementFamilyInstance(element as FamilyInstance, this._RootId);
            }
            if (element is Wall)
            {
                return this.ProcessElementWall(element as Wall, this._RootId);
            }
            return this.ProcessElementInstance(element, this._RootId);
        }

        private int ProcessElementFamilyInstance(FamilyInstance element, int rootId)
        {
            string name = element.Category.Name;
            string key = name;
            string uid = element.Category.Id.IntegerValue.ToString();
            int parentId = this.InsertNode(key, name, uid, null, rootId, null);
            string str4 = element.Symbol.Family.Name;
            string str5 = key + ":" + str4;
            string uniqueId = element.Symbol.Family.UniqueId;
            int num2 = this.InsertNode(str5, str4, uniqueId, null, parentId, null);
            string str7 = element.Symbol.Name;
            string str8 = str5 + ":" + str7;
            string str9 = element.Symbol.UniqueId;
            int num3 = this.InsertNode(str8, str7, str9, element.Symbol.Parameters, num2, null);
            string str10 = string.Concat(new object[] { str4, " [", element.Id.IntegerValue, "]" });
            string str11 = str8 + ":" + str10;
            string str12 = element.UniqueId;
            return this.InsertNode(str11, str10, str12, element.Parameters, num3, element);
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
                string str2 = element.Category.Name;
                str = str2;
                string str3 = element.Category.Id.IntegerValue.ToString();
                num = this.InsertNode(str, str2, str3, null, rootId, null);
            }
            string name = element.Name;
            string key = str + ":" + name;
            string uid = "";
            int parentId = this.InsertNode(key, name, uid, null, num, null);
            string str7 = string.Concat(new object[] { name, " [", element.Id.IntegerValue, "]" });
            string str8 = key + ":" + str7;
            string uniqueId = element.UniqueId;
            return this.InsertNode(str8, str7, uniqueId, element.Parameters, parentId, element);
        }

        private int ProcessElementWall(Wall element, int rootId)
        {
            string name = element.Category.Name;
            string key = name;
            string uid = element.Category.Id.IntegerValue.ToString();
            int parentId = this.InsertNode(key, name, uid, null, rootId, null);
            string familyName = this.GetFamilyName(element, element.WallType.Kind.ToString());
            string str5 = key + ":" + familyName;
            string str6 = "";
            int num2 = this.InsertNode(str5, familyName, str6, null, parentId, null);
            string str7 = element.WallType.Name;
            string str8 = str5 + ":" + str7;
            string uniqueId = element.WallType.UniqueId;
            int num3 = this.InsertNode(str8, str7, uniqueId, element.WallType.Parameters, num2, null);
            string str10 = string.Concat(new object[] { familyName, " [", element.Id.IntegerValue, "]" });
            string str11 = str8 + ":" + str10;
            string str12 = element.UniqueId;
            return this.InsertNode(str11, str10, str12, element.Parameters, num3, element);
        }

        private void ProcessParameter(Parameter p, out PropDef propDef, out string propValue)
        {
            int num;
            string str3;
            string str4;
            string name = p.Definition.Name;
            string labelFor = LabelUtils.GetLabelFor(p.Definition.ParameterGroup);
            this.GetPropInfo(p, out num, out str3, out str4);
            propDef = new PropDef(name, labelFor, (PropDataType) num, str4, false);
            propValue = str3;
        }

        public int RootId =>
            this._RootId;
    }
}

