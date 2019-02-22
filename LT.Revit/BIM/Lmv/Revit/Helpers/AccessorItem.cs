namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal class AccessorItem
    {
        public AccessorItem(string sAccType, string sType, string sByteOffSet, string sByteStride, string sComponentType, string sCount, string sBFileId)
        {
            this.Type = sType;
            this.ByteOffSet = sByteOffSet;
            this.ByteStride = sByteStride;
            this.ComponentType = sComponentType;
            this.Count = sCount;
            this.BFileId = sBFileId;
            this.AccType = sAccType;
        }

        public string AccType { get; set; }

        public string BFileId { get; set; }

        public string ByteOffSet { get; set; }

        public string ByteStride { get; set; }

        public string ComponentType { get; set; }

        public string Count { get; set; }

        public string ObjectId { get; set; }

        public string Type { get; set; }
    }
}

