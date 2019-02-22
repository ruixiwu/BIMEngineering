namespace BIM.Lmv.Processers
{
    using BIM.Lmv.Common.JsonGz;
    using BIM.Lmv.Content.Other;
    using BIM.Lmv.Types;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class PropertyProcesser
    {
        private JsonGzWriter _FileAttr;
        private JsonGzWriter _FileIds;
        private JsonGzWriter _FileVals;
        private readonly Dictionary<string, int> _NodeIds = new Dictionary<string, int>();
        private OutputProcesser _Output;
        private FileEntryStream _OutputAttr;
        private FileEntryStream _OutputIds;
        private FileEntryStream _OutputVals;
        private int _PropDefChildId;
        private int _PropDefMarkId;
        private int _PropDefNameId;
        private int _PropDefNextIndex;
        private int _PropDefParentId;
        private readonly Dictionary<string, int> _PropDefs = new Dictionary<string, int>(0x400);
        private readonly List<List<int>> _PropItems = new List<List<int>>(0x5000);
        private int _PropValNextIndex;
        private readonly Dictionary<string, int> _PropVals = new Dictionary<string, int>(0x2800);
        private readonly SvfFileProcesser _SvfFile;
        public const string FILE_PATH_ATTRS = "objects_attrs.json.gz";
        public const string FILE_PATH_AVS = "objects_avs.json.gz";
        public const string FILE_PATH_IDS = "objects_ids.json.gz";
        public const string FILE_PATH_OFFS = "objects_offs.json.gz";
        public const string FILE_PATH_VALS = "objects_vals.json.gz";

        public PropertyProcesser(OutputProcesser output, SvfFileProcesser svfFile)
        {
            this._Output = output;
            this._SvfFile = svfFile;
        }

        private int GetPropDefIndex(PropDef def)
        {
            int type = (int) def.Type;
            return this.GetPropDefIndex(def.Name, def.Category, type, def.Unit, def.Hide);
        }

        private int GetPropDefIndex(string name, string category, int type, string unit, bool hide)
        {
            int num;
            string str = hide ? "1" : "0";
            if ((string.CompareOrdinal("is_doc_property", name) == 0) && (string.CompareOrdinal("__document__", category) == 0))
            {
                str = "3";
            }
            if ((name != null) && name.Contains("\""))
            {
                name = name.Replace("\"", "\\\"");
            }
            if ((category != null) && category.Contains("\""))
            {
                category = category.Replace("\"", "\\\"");
            }
            if ((unit != null) && unit.Contains("\""))
            {
                unit = unit.Replace("\"", "\\\"");
            }
            string key = string.Concat(new object[] { "[\"", name, "\",\"", category, "\",", type, ",", (unit == null) ? "null" : ("\"" + unit + "\""), ",null,null,", str, "]" });
            if (!this._PropDefs.TryGetValue(key, out num))
            {
                num = this._PropDefNextIndex++;
                this._PropDefs.Add(key, num);
                this._FileAttr.AppendLine(",");
                this._FileAttr.Append(key);
            }
            return num;
        }

        private int GetPropValueIndex(string value, int type)
        {
            int num;
            string key = value + "_" + type;
            if (!this._PropVals.TryGetValue(key, out num))
            {
                num = this._PropValNextIndex++;
                this._PropVals.Add(key, num);
                this._FileVals.AppendLine(",");
                if (type == 20)
                {
                    this._FileVals.Append("\"" + value.Replace("\"", "\\\"") + "\"");
                }
                else
                {
                    this._FileVals.Append(value);
                }
            }
            return num;
        }

        private int InsertNode(string key, string name, string uid, List<PropItem> props, int parentId)
        {
            int count;
            if (!this._NodeIds.TryGetValue(key, out count))
            {
                this._FileIds.AppendLine(",");
                this._FileIds.Append("\"");
                this._FileIds.Append(uid);
                this._FileIds.Append("\"");
                List<int> item = (props == null) ? new List<int>(0x20) : this.ProcessParameters(props);
                item.Add(this._PropDefNameId);
                item.Add(this.GetPropValueIndex(name, 20));
                count = this._PropItems.Count;
                this._PropItems.Add(item);
                this._NodeIds.Add(key, count);
                if (parentId == -1)
                {
                    item.Add(this._PropDefMarkId);
                    item.Add(this.GetPropValueIndex("比目鱼工程咨询", 20));
                }
                if (parentId != -1)
                {
                    item.Add(this._PropDefParentId);
                    item.Add(this.GetPropValueIndex(parentId.ToString(), 2));
                    List<int> list2 = this._PropItems[parentId];
                    list2.Add(this._PropDefChildId);
                    list2.Add(this.GetPropValueIndex(count.ToString(), 2));
                }
            }
            return count;
        }

        public void OnFinish()
        {
            this._FileAttr.AppendLine();
            this._FileAttr.Append("]");
            this._FileAttr.Dispose();
            this._FileAttr = null;
            this._FileIds.AppendLine();
            this._FileIds.Append("]");
            this._FileIds.Dispose();
            this._FileIds = null;
            this._FileVals.AppendLine();
            this._FileVals.Append("]");
            this._FileVals.Dispose();
            this._FileVals = null;
            FileEntryStream entry = new FileEntryStream("objects_offs.json.gz");
            FileEntryStream stream2 = new FileEntryStream("objects_avs.json.gz");
            using (JsonGzWriter writer = new JsonGzWriter(entry.Stream, true))
            {
                using (JsonGzWriter writer2 = new JsonGzWriter(stream2.Stream, true))
                {
                    writer.Append("[0,0");
                    writer2.Append("[");
                    int num = 0;
                    bool flag = true;
                    for (int i = 1; i < this._PropItems.Count; i++)
                    {
                        List<int> list = this._PropItems[i];
                        writer.Append("," + (num + (list.Count / 2)));
                        foreach (int num3 in list)
                        {
                            if (flag)
                            {
                                flag = false;
                                writer2.Append(num3.ToString());
                            }
                            else
                            {
                                writer2.Append("," + num3);
                            }
                        }
                    }
                    writer2.Append("]");
                    writer.Append("]");
                }
            }
            this._PropItems.Clear();
            int size = this._OutputAttr.GetSize();
            int num5 = this._OutputIds.GetSize();
            int num6 = this._OutputVals.GetSize();
            int num7 = entry.GetSize();
            int num8 = stream2.GetSize();
            this._Output.OnAppendFile(this._OutputAttr);
            this._Output.OnAppendFile(this._OutputIds);
            this._Output.OnAppendFile(this._OutputVals);
            this._Output.OnAppendFile(entry);
            this._Output.OnAppendFile(stream2);
            this._SvfFile.OnEntry(new EntryAsset("objects_attrs.json", "Autodesk.CloudPlatform.PropertyAttributes", "objects_attrs.json.gz", new int?(size), 0, null));
            this._SvfFile.OnEntry(new EntryAsset("objects_vals.json", "Autodesk.CloudPlatform.PropertyValues", "objects_vals.json.gz", new int?(num6), 0, null));
            this._SvfFile.OnEntry(new EntryAsset("objects_ids.json", "Autodesk.CloudPlatform.PropertyIDs", "objects_ids.json.gz", new int?(num5), 0, null));
            this._SvfFile.OnEntry(new EntryAsset("objects_offs.json", "Autodesk.CloudPlatform.PropertyOffsets", "objects_offs.json.gz", new int?(num7), 0, null));
            this._SvfFile.OnEntry(new EntryAsset("objects_avs.json", "Autodesk.CloudPlatform.PropertyAVs", "objects_avs.json.gz", new int?(num8), 0, null));
        }

        public int OnNode(string key, string name, string uid, int parentNodeId, List<PropItem> props) => 
            this.InsertNode(key, name, uid, props, parentNodeId);

        public void OnStart()
        {
            this._PropDefs.Clear();
            this._PropDefNextIndex = 1;
            this._PropVals.Clear();
            this._PropValNextIndex = 1;
            this._PropItems.Clear();
            this._PropItems.Add(null);
            this._OutputAttr = new FileEntryStream("objects_attrs.json.gz");
            this._FileAttr = new JsonGzWriter(this._OutputAttr.Stream, true);
            this._FileAttr.Append("[0");
            this._OutputIds = new FileEntryStream("objects_ids.json.gz");
            this._FileIds = new JsonGzWriter(this._OutputIds.Stream, true);
            this._FileIds.Append("[0");
            this._OutputVals = new FileEntryStream("objects_vals.json.gz");
            this._FileVals = new JsonGzWriter(this._OutputVals.Stream, true);
            this._FileVals.Append("[0");
            this._PropDefNameId = this.GetPropDefIndex(PropDef.DefName);
            this._PropDefChildId = this.GetPropDefIndex(PropDef.DefChild);
            this._PropDefParentId = this.GetPropDefIndex(PropDef.DefParent);
            this._PropDefMarkId = this.GetPropDefIndex(PropDef.DefMark);
            foreach (PropDef def in PropDef.PredefList)
            {
                this.GetPropDefIndex(def);
            }
        }

        private void ProcessParameter(PropItem p, out int propDefIndex, out int propValueIndex)
        {
            PropDef def = p.Def;
            int type = (int) def.Type;
            propDefIndex = this.GetPropDefIndex(def.Name, def.Category, type, def.Unit, def.Hide);
            propValueIndex = this.GetPropValueIndex(p.Value, type);
        }

        private List<int> ProcessParameters(List<PropItem> props)
        {
            List<int> list = new List<int>((props.Count * 2) + 0x20);
            foreach (PropItem item in props)
            {
                int num;
                int num2;
                this.ProcessParameter(item, out num, out num2);
                list.Add(num);
                list.Add(num2);
            }
            return list;
        }
    }
}

