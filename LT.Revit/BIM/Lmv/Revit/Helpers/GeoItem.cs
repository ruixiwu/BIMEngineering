namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal class GeoItem
    {
        public GeoItem()
        {
        }

        public GeoItem(string sName, string sMatrix, string sMeshIds, string sBlockId, string sBox, string sOrignFile, string sFamilyID)
        {
            this.Name = sName;
            this.Matrix = sMatrix;
            this.MeshIds = sMeshIds;
            this.BlockId = sBlockId;
            this.Box = sBox;
            this.OrignFile = sOrignFile;
            this.FamilyID = sFamilyID;
        }

        public string BlockId { get; set; }

        public string Box { get; set; }

        public string FamilyID { get; set; }

        public string Matrix { get; set; }

        public string MeshIds { get; set; }

        public string Name { get; set; }

        public string OrignFile { get; set; }

        public string TilesetId { get; set; }
    }
}

