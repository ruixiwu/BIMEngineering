namespace BIM.Lmv.Revit.Helpers
{
    internal class CoordinaeSysItem
    {
        public CoordinaeSysItem()
        {
        }

        public CoordinaeSysItem(string sName, string sSemimaJoraxis, string sInverseFlattening, string sScaleFacetor,
            string sFalse_easting, string sFalse_northing, string sCentralmeridian)
        {
            Name = sName;
            SemimaJoraxis = sSemimaJoraxis;
            InverseFlattening = sInverseFlattening;
            ScaleFacetor = sScaleFacetor;
            False_easting = sFalse_easting;
            False_northing = sFalse_northing;
            Centralmeridian = sCentralmeridian;
        }

        public string Centralmeridian { get; set; }

        public string False_easting { get; set; }

        public string False_northing { get; set; }

        public string InverseFlattening { get; set; }

        public string Name { get; set; }

        public string ScaleFacetor { get; set; }

        public string SemimaJoraxis { get; set; }
    }
}