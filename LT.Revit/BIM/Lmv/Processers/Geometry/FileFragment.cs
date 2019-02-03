using System.Collections.Generic;
using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Content.Other;
using BIM.Lmv.Processers.Helper;

namespace BIM.Lmv.Processers.Geometry
{
    internal class FileFragment
    {
        public const string FILE_PATH = "FragmentList.pack";
        private readonly EntryFragment _EntryFragment;
        private readonly PackFileOutput<EntryFragment> _FileEntryFragment;

        private readonly Dictionary<string, InstanceTemplate> _InstanceTemplates =
            new Dictionary<string, InstanceTemplate>(0x400);

        private readonly PackEntryType _PackEntryType;
        private uint _CurrDbId;
        private InstanceTemplate _CurrentInstanceTemplate;
        private Transform _CurrTransform = new Transform();
        private readonly OutputProcesser _Output;
        private readonly SvfFileProcesser _SvfFile;

        public FileFragment(OutputProcesser output, SvfFileProcesser svfFile)
        {
            _Output = output;
            _SvfFile = svfFile;
            _EntryFragment = new EntryFragment();
            _PackEntryType = new PackEntryType(0, "Autodesk.CloudPlatform.Fragment",
                "Autodesk.CloudPlatform.FragmentData", 5);
            _FileEntryFragment = new PackFileOutput<EntryFragment>(0x200000);
            _FileEntryFragment.OnStart();
            _FileEntryFragment.OnEntryType(_PackEntryType);
        }

        public void OnAppendItem(uint materialId, uint metadataId, Box3F bbox)
        {
            _EntryFragment.materialId = materialId;
            _EntryFragment.metadataId = metadataId;
            _EntryFragment.dbId = _CurrDbId;
            _EntryFragment.transform = _CurrTransform;
            SetFragmentBox(_EntryFragment, bbox);
            var num = _FileEntryFragment.OnEntry(_EntryFragment, _PackEntryType);
            if (_CurrentInstanceTemplate != null)
            {
                if (_CurrentInstanceTemplate.Begin < 0)
                {
                    _CurrentInstanceTemplate.Begin = num;
                }
                _CurrentInstanceTemplate.End = num;
            }
        }

        public void OnElementBegin(int nodeId, Transform transform)
        {
            _CurrDbId = (uint) nodeId;
            _CurrentInstanceTemplate = null;
            SetTransform(transform);
        }

        public void OnElementEnd()
        {
        }

        public void OnFinish()
        {
            var entry = new FileEntryStream("FragmentList.pack");
            _FileEntryFragment.OnFinish(entry.Stream);
            _Output.OnAppendFile(entry);
            var size = entry.GetSize();
            _SvfFile.OnEntry(new EntryAsset("FragmentList.pack", "Autodesk.CloudPlatform.FragmentList",
                "FragmentList.pack", size, 0, _SvfFile.AssetTypeFragment));
        }

        public bool OnInstanceBegin(string instanceKey, Transform transform, bool allowReuse = true)
        {
            InstanceTemplate template;
            SetTransform(transform);
            if (allowReuse && _InstanceTemplates.TryGetValue(instanceKey, out template))
            {
                if (template.Begin >= 0)
                {
                    for (var i = template.Begin; i <= template.End; i++)
                    {
                        _FileEntryFragment.ReadEntry(i, _EntryFragment);
                        _EntryFragment.dbId = _CurrDbId;
                        _EntryFragment.transform = _CurrTransform;
                        _FileEntryFragment.OnEntry(_EntryFragment, _PackEntryType);
                    }
                }
                return true;
            }
            if (allowReuse)
            {
                _CurrentInstanceTemplate = new InstanceTemplate(instanceKey);
            }
            return false;
        }

        public void OnInstanceEnd(Transform transform)
        {
            if ((_CurrentInstanceTemplate != null) && (_CurrentInstanceTemplate.Begin >= 0))
            {
                _InstanceTemplates.Add(_CurrentInstanceTemplate.Key, _CurrentInstanceTemplate);
            }
            SetTransform(transform);
            _CurrentInstanceTemplate = null;
        }

        private void SetFragmentBox(EntryFragment frag, Box3F box)
        {
            frag.boxes[0] = box.min.x;
            frag.boxes[1] = box.min.y;
            frag.boxes[2] = box.min.z;
            frag.boxes[3] = box.max.x;
            frag.boxes[4] = box.max.y;
            frag.boxes[5] = box.max.z;
        }

        private void SetTransform(Transform t)
        {
            if (t != null)
            {
                _CurrTransform = t;
            }
            else
            {
                _CurrTransform.SetIdentity();
            }
        }
    }
}