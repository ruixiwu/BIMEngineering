namespace BIM.Lmv.Revit.Helpers
{
    internal class GeoBlockItem
    {
        public GeoBlockItem()
        {
        }

        public GeoBlockItem(string sName, string sDesc, string sMeshIds)
        {
            Name = sName;
            Desc = sDesc;
            MeshIds = sMeshIds;
        }

        public string Desc { get; set; }

        public string MeshIds { get; set; }

        public string Name { get; set; }
    }
}