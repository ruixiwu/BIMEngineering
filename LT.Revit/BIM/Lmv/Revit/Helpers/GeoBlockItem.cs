namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal class GeoBlockItem
    {
        public GeoBlockItem()
        {
        }

        public GeoBlockItem(string sName, string sDesc, string sMeshIds)
        {
            this.Name = sName;
            this.Desc = sDesc;
            this.MeshIds = sMeshIds;
        }

        public string Desc { get; set; }

        public string MeshIds { get; set; }

        public string Name { get; set; }
    }
}

