﻿namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal class TLPropertyHelper
    {
        public List<PropertyItem> _ListData = new List<PropertyItem>();
        public string _strElementID;
        public string _strElementType;
        public string _strElementUID;

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

        public static void getParameterVar(ParameterSet parameterSets, List<PropertyItem> tListData, bool bFilterNull = false)
        {
            Dictionary<string, PropertyItem> dictionary = new Dictionary<string, PropertyItem>();
            foreach (Parameter parameter in parameterSets)
            {
                string name = parameter.Definition.Name;
                string labelFor = LabelUtils.GetLabelFor(parameter.Definition.ParameterGroup);
                if (labelFor != "其他")
                {
                    int num;
                    string str4;
                    string str3 = "";
                    GetPropInfo(parameter, out num, out str3, out str4);
                    if (str3 != null)
                    {
                        str3 = str3.Replace('\'', '`');
                    }
                    if ((((name == "类型名称") || (name == "产品编码")) || ((name == "构件编码") || name.Contains("构件库编码"))) || ((name.Contains("构件库分类") || name.Contains("分类编码")) || ((name.Contains("单价") || !bFilterNull) || !string.IsNullOrEmpty(str3))))
                    {
                        string sType = "3";
                        switch (num)
                        {
                            case 2:
                                sType = "1";
                                break;

                            case 3:
                                sType = "2";
                                break;
                        }
                        PropertyItem item = new PropertyItem(labelFor, name, str3, str4, sType);
                        if (dictionary.ContainsKey(labelFor + name))
                        {
                            if (!string.IsNullOrEmpty(str3))
                            {
                                dictionary[labelFor + name] = item;
                            }
                        }
                        else
                        {
                            dictionary.Add(labelFor + name, item);
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, PropertyItem> pair in dictionary)
            {
                tListData.Add(pair.Value);
            }
        }

        public static void GetPropInfo(Parameter p, out int type, out string value, out string unit)
        {
            unit = string.Empty;
            if ((p.StorageType == StorageType.Integer) && (p.Definition.ParameterType == ParameterType.YesNo))
            {
                type = 2;
                value = (p.AsInteger() == 1) ? "1" : "0";
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

        public int OnElement(Element element)
        {
            this._ListData.Clear();
            if (element is FamilyInstance)
            {
                return this.ProcessElementFamilyInstance(element as FamilyInstance);
            }
            if (element is Wall)
            {
                return this.ProcessElementWall(element as Wall);
            }
            return this.ProcessElementInstance(element);
        }

        public void OnWiteTable(string sGeoID)
        {
            TableHelp.OnWitePropertyTable(this._ListData, this._strElementType, sGeoID);
        }

        private int ProcessElementFamilyInstance(FamilyInstance element)
        {
            this._strElementType = element.Category.Name + ":" + element.Symbol.Family.Name + ":" + element.Symbol.Name;
            this._strElementID = element.Id.ToString();
            this._strElementUID = element.UniqueId;
            getParameterVar(element.Parameters, this._ListData, false);
            return 1;
        }

        private int ProcessElementInstance(Element element)
        {
            if (element.Category == null)
            {
                this._strElementType = element.Name;
            }
            else
            {
                this._strElementType = element.Category.Name + ":" + element.Name;
            }
            this._strElementID = element.Id.ToString();
            this._strElementUID = element.UniqueId;
            getParameterVar(element.Parameters, this._ListData, false);
            return 1;
        }

        private int ProcessElementWall(Wall element)
        {
            this._strElementType = element.Category.Name + ":" + this.GetFamilyName(element, element.WallType.Kind.ToString()) + ":" + element.WallType.Name;
            this._strElementID = element.Id.ToString();
            this._strElementUID = element.UniqueId;
            getParameterVar(element.Parameters, this._ListData, false);
            return 1;
        }

        private void transMark(ref string str)
        {
            str = str.Replace('/', '0');
            str = str.Replace('\\', '1');
            str = str.Replace('|', '2');
            str = str.Replace('*', '3');
            str = str.Replace('\'', '5');
            str = str.Replace('"', '6');
        }
    }
}

