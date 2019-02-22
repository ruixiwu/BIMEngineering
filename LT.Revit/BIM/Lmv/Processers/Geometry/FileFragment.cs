namespace BIM.Lmv.Processers.Geometry
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Content.Other;
    using BIM.Lmv.Processers;
    using BIM.Lmv.Processers.Helper;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class FileFragment
    {
        private uint _CurrDbId;
        private InstanceTemplate _CurrentInstanceTemplate;
        private Transform _CurrTransform = new Transform();
        private readonly EntryFragment _EntryFragment;
        private readonly PackFileOutput<EntryFragment> _FileEntryFragment;
        private readonly Dictionary<string, InstanceTemplate> _InstanceTemplates = new Dictionary<string, InstanceTemplate>(0x400);
        private OutputProcesser _Output;
        private readonly PackEntryType _PackEntryType;
        private SvfFileProcesser _SvfFile;
        public const string FILE_PATH = "FragmentList.pack";

        public FileFragment(OutputProcesser output, SvfFileProcesser svfFile)
        {
            this._Output = output;
            this._SvfFile = svfFile;
            this._EntryFragment = new EntryFragment();
            this._PackEntryType = new PackEntryType(0, "Autodesk.CloudPlatform.Fragment", "Autodesk.CloudPlatform.FragmentData", 5);
            this._FileEntryFragment = new PackFileOutput<EntryFragment>(0x200000);
            this._FileEntryFragment.OnStart();
            this._FileEntryFragment.OnEntryType(this._PackEntryType);
        }

        public void OnAppendItem(uint materialId, uint metadataId, Box3F bbox)
        {
            this._EntryFragment.materialId = materialId;
            this._EntryFragment.metadataId = metadataId;
            this._EntryFragment.dbId = this._CurrDbId;
            this._EntryFragment.transform = this._CurrTransform;
            this.SetFragmentBox(this._EntryFragment, bbox);
            int num = this._FileEntryFragment.OnEntry(this._EntryFragment, this._PackEntryType);
            if (this._CurrentInstanceTemplate != null)
            {
                if (this._CurrentInstanceTemplate.Begin < 0)
                {
                    this._CurrentInstanceTemplate.Begin = num;
                }
                this._CurrentInstanceTemplate.End = num;
            }
        }

        public void OnElementBegin(int nodeId, Transform transform)
        {
            this._CurrDbId = (uint) nodeId;
            this._CurrentInstanceTemplate = null;
            this.SetTransform(transform);
        }

        public void OnElementEnd()
        {
        }

        public void OnFinish()
        {
            FileEntryStream entry = new FileEntryStream("FragmentList.pack");
            this._FileEntryFragment.OnFinish(entry.Stream);
            this._Output.OnAppendFile(entry);
            int size = entry.GetSize();
            this._SvfFile.OnEntry(new EntryAsset("FragmentList.pack", "Autodesk.CloudPlatform.FragmentList", "FragmentList.pack", new int?(size), 0, this._SvfFile.AssetTypeFragment));
        }

        public bool OnInstanceBegin(string instanceKey, Transform transform, bool allowReuse = true)
        {
            InstanceTemplate template;
            this.SetTransform(transform);
            if (allowReuse && this._InstanceTemplates.TryGetValue(instanceKey, out template))
            {
                if (template.Begin >= 0)
                {
                    for (int i = template.Begin; i <= template.End; i++)
                    {
                        this._FileEntryFragment.ReadEntry(i, this._EntryFragment);
                        this._EntryFragment.dbId = this._CurrDbId;
                        this._EntryFragment.transform = this._CurrTransform;
                        this._FileEntryFragment.OnEntry(this._EntryFragment, this._PackEntryType);
                    }
                }
                return true;
            }
            if (allowReuse)
            {
                this._CurrentInstanceTemplate = new InstanceTemplate(instanceKey);
            }
            return false;
        }

        public void OnInstanceEnd(Transform transform)
        {
            if ((this._CurrentInstanceTemplate != null) && (this._CurrentInstanceTemplate.Begin >= 0))
            {
                this._InstanceTemplates.Add(this._CurrentInstanceTemplate.Key, this._CurrentInstanceTemplate);
            }
            this.SetTransform(transform);
            this._CurrentInstanceTemplate = null;
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
                this._CurrTransform = t;
            }
            else
            {
                this._CurrTransform.SetIdentity();
            }
        }
    }
}

