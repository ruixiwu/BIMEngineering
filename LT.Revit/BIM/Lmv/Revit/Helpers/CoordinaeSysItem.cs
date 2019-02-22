namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal class CoordinaeSysItem
    {
        public CoordinaeSysItem()
        {
        }

        public CoordinaeSysItem(string sName, string sSemimaJoraxis, string sInverseFlattening, string sScaleFacetor, string sFalse_easting, string sFalse_northing, string sCentralmeridian)
        {
            this.Name = sName;
            this.SemimaJoraxis = sSemimaJoraxis;
            this.InverseFlattening = sInverseFlattening;
            this.ScaleFacetor = sScaleFacetor;
            this.False_easting = sFalse_easting;
            this.False_northing = sFalse_northing;
            this.Centralmeridian = sCentralmeridian;
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

