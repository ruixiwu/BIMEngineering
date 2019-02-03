using System.Collections.Generic;
using BIM.Lmv.Common.JsonGz;
using BIM.Lmv.Content.Other;
using BIM.Lmv.Types;

namespace BIM.Lmv.Processers
{
    internal class PropertyProcesser
    {
        public const string FILE_PATH_ATTRS = "objects_attrs.json.gz";
        public const string FILE_PATH_AVS = "objects_avs.json.gz";
        public const string FILE_PATH_IDS = "objects_ids.json.gz";
        public const string FILE_PATH_OFFS = "objects_offs.json.gz";
        public const string FILE_PATH_VALS = "objects_vals.json.gz";
        private readonly Dictionary<string, int> _NodeIds = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _PropDefs = new Dictionary<string, int>(0x400);
        private readonly List<List<int>> _PropItems = new List<List<int>>(0x5000);
        private readonly Dictionary<string, int> _PropVals = new Dictionary<string, int>(0x2800);
        private readonly SvfFileProcesser _SvfFile;
        private JsonGzWriter _FileAttr;
        private JsonGzWriter _FileIds;
        private JsonGzWriter _FileVals;
        private readonly OutputProcesser _Output;
        private FileEntryStream _OutputAttr;
        private FileEntryStream _OutputIds;
        private FileEntryStream _OutputVals;
        private int _PropDefChildId;
        private int _PropDefMarkId;
        private int _PropDefNameId;
        private int _PropDefNextIndex;
        private int _PropDefParentId;
        private int _PropValNextIndex;

        public PropertyProcesser(OutputProcesser output, SvfFileProcesser svfFile)
        {
            _Output = output;
            _SvfFile = svfFile;
        }

        private int GetPropDefIndex(PropDef def)
        {
            var type = (int) def.Type;
            return GetPropDefIndex(def.Name, def.Category, type, def.Unit, def.Hide);
        }

        private int GetPropDefIndex(string name, string category, int type, string unit, bool hide)
        {
            int num;
            var str = hide ? "1" : "0";
            if ((string.CompareOrdinal("is_doc_property", name) == 0) &&
                (string.CompareOrdinal("__document__", category) == 0))
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
            var key = string.Concat("[\"", name, "\",\"", category, "\",", type, ",",
                unit == null ? "null" : "\"" + unit + "\"", ",null,null,", str, "]");
            if (!_PropDefs.TryGetValue(key, out num))
            {
                num = _PropDefNextIndex++;
                _PropDefs.Add(key, num);
                _FileAttr.AppendLine(",");
                _FileAttr.Append(key);
            }
            return num;
        }

        private int GetPropValueIndex(string value, int type)
        {
            int num;
            var key = value + "_" + type;
            if (!_PropVals.TryGetValue(key, out num))
            {
                num = _PropValNextIndex++;
                _PropVals.Add(key, num);
                _FileVals.AppendLine(",");
                if (type == 20)
                {
                    _FileVals.Append("\"" + value.Replace("\"", "\\\"") + "\"");
                }
                else
                {
                    _FileVals.Append(value);
                }
            }
            return num;
        }

        private int InsertNode(string key, string name, string uid, List<PropItem> props, int parentId)
        {
            int count;
            if (!_NodeIds.TryGetValue(key, out count))
            {
                _FileIds.AppendLine(",");
                _FileIds.Append("\"");
                _FileIds.Append(uid);
                _FileIds.Append("\"");
                var item = props == null ? new List<int>(0x20) : ProcessParameters(props);
                item.Add(_PropDefNameId);
                item.Add(GetPropValueIndex(name, 20));
                count = _PropItems.Count;
                _PropItems.Add(item);
                _NodeIds.Add(key, count);
                if (parentId == -1)
                {
                    item.Add(_PropDefMarkId);
                    item.Add(GetPropValueIndex("比目鱼工程咨询", 20));
                }
                if (parentId != -1)
                {
                    item.Add(_PropDefParentId);
                    item.Add(GetPropValueIndex(parentId.ToString(), 2));
                    var list2 = _PropItems[parentId];
                    list2.Add(_PropDefChildId);
                    list2.Add(GetPropValueIndex(count.ToString(), 2));
                }
            }
            return count;
        }

        public void OnFinish()
        {
            _FileAttr.AppendLine();
            _FileAttr.Append("]");
            _FileAttr.Dispose();
            _FileAttr = null;
            _FileIds.AppendLine();
            _FileIds.Append("]");
            _FileIds.Dispose();
            _FileIds = null;
            _FileVals.AppendLine();
            _FileVals.Append("]");
            _FileVals.Dispose();
            _FileVals = null;
            var entry = new FileEntryStream("objects_offs.json.gz");
            var stream2 = new FileEntryStream("objects_avs.json.gz");
            using (var writer = new JsonGzWriter(entry.Stream, true))
            {
                using (var writer2 = new JsonGzWriter(stream2.Stream, true))
                {
                    writer.Append("[0,0");
                    writer2.Append("[");
                    var num = 0;
                    var flag = true;
                    for (var i = 1; i < _PropItems.Count; i++)
                    {
                        var list = _PropItems[i];
                        writer.Append("," + (num + list.Count/2));
                        foreach (var num3 in list)
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
            _PropItems.Clear();
            var size = _OutputAttr.GetSize();
            var num5 = _OutputIds.GetSize();
            var num6 = _OutputVals.GetSize();
            var num7 = entry.GetSize();
            var num8 = stream2.GetSize();
            _Output.OnAppendFile(_OutputAttr);
            _Output.OnAppendFile(_OutputIds);
            _Output.OnAppendFile(_OutputVals);
            _Output.OnAppendFile(entry);
            _Output.OnAppendFile(stream2);
            _SvfFile.OnEntry(new EntryAsset("objects_attrs.json", "Autodesk.CloudPlatform.PropertyAttributes",
                "objects_attrs.json.gz", size, 0, null));
            _SvfFile.OnEntry(new EntryAsset("objects_vals.json", "Autodesk.CloudPlatform.PropertyValues",
                "objects_vals.json.gz", num6, 0, null));
            _SvfFile.OnEntry(new EntryAsset("objects_ids.json", "Autodesk.CloudPlatform.PropertyIDs",
                "objects_ids.json.gz", num5, 0, null));
            _SvfFile.OnEntry(new EntryAsset("objects_offs.json", "Autodesk.CloudPlatform.PropertyOffsets",
                "objects_offs.json.gz", num7, 0, null));
            _SvfFile.OnEntry(new EntryAsset("objects_avs.json", "Autodesk.CloudPlatform.PropertyAVs",
                "objects_avs.json.gz", num8, 0, null));
        }

        public int OnNode(string key, string name, string uid, int parentNodeId, List<PropItem> props)
        {
            return InsertNode(key, name, uid, props, parentNodeId);
        }

        public void OnStart()
        {
            _PropDefs.Clear();
            _PropDefNextIndex = 1;
            _PropVals.Clear();
            _PropValNextIndex = 1;
            _PropItems.Clear();
            _PropItems.Add(null);
            _OutputAttr = new FileEntryStream("objects_attrs.json.gz");
            _FileAttr = new JsonGzWriter(_OutputAttr.Stream, true);
            _FileAttr.Append("[0");
            _OutputIds = new FileEntryStream("objects_ids.json.gz");
            _FileIds = new JsonGzWriter(_OutputIds.Stream, true);
            _FileIds.Append("[0");
            _OutputVals = new FileEntryStream("objects_vals.json.gz");
            _FileVals = new JsonGzWriter(_OutputVals.Stream, true);
            _FileVals.Append("[0");
            _PropDefNameId = GetPropDefIndex(PropDef.DefName);
            _PropDefChildId = GetPropDefIndex(PropDef.DefChild);
            _PropDefParentId = GetPropDefIndex(PropDef.DefParent);
            _PropDefMarkId = GetPropDefIndex(PropDef.DefMark);
            foreach (var def in PropDef.PredefList)
            {
                GetPropDefIndex(def);
            }
        }

        private void ProcessParameter(PropItem p, out int propDefIndex, out int propValueIndex)
        {
            var def = p.Def;
            var type = (int) def.Type;
            propDefIndex = GetPropDefIndex(def.Name, def.Category, type, def.Unit, def.Hide);
            propValueIndex = GetPropValueIndex(p.Value, type);
        }

        private List<int> ProcessParameters(List<PropItem> props)
        {
            var list = new List<int>(props.Count*2 + 0x20);
            foreach (var item in props)
            {
                int num;
                int num2;
                ProcessParameter(item, out num, out num2);
                list.Add(num);
                list.Add(num2);
            }
            return list;
        }
    }
}