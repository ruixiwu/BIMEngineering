namespace BIM.Lmv.Revit.Helpers
{
    internal class DataSetItem
    {
        public DataSetItem(string sTilesetTable, string sMaterialTable, string sGeoBlockTable, string sKind,
            string sName, string sCoordSysId)
        {
            TilesetTable = sTilesetTable;
            MaterialTable = sMaterialTable;
            GeoBlockTable = sGeoBlockTable;
            Kind = sKind;
            Name = sName;
            CoordSysId = sCoordSysId;
        }

        public string CoordSysId { get; set; }

        public string GeoBlockTable { get; set; }

        public string Kind { get; set; }

        public string MaterialTable { get; set; }

        public string Name { get; set; }

        public string TilesetTable { get; set; }
    }
}