namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal class DataSetItem
    {
        public DataSetItem(string sTilesetTable, string sMaterialTable, string sGeoBlockTable, string sKind, string sName, string sCoordSysId)
        {
            this.TilesetTable = sTilesetTable;
            this.MaterialTable = sMaterialTable;
            this.GeoBlockTable = sGeoBlockTable;
            this.Kind = sKind;
            this.Name = sName;
            this.CoordSysId = sCoordSysId;
        }

        public string CoordSysId { get; set; }

        public string GeoBlockTable { get; set; }

        public string Kind { get; set; }

        public string MaterialTable { get; set; }

        public string Name { get; set; }

        public string TilesetTable { get; set; }
    }
}

