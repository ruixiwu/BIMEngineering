namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal class FamilyItem
    {
        public FamilyItem(string sSection, string sName, string sValue, string sUnit, string sType, string sFamilyID)
        {
            this.Section = sSection;
            this.Name = sName;
            this.Value = sValue;
            this.Unit = sUnit;
            this.Type = sType;
            this.FamilyID = sFamilyID;
        }

        public string FamilyID { get; set; }

        public string Name { get; set; }

        public string Section { get; set; }

        public string Type { get; set; }

        public string Unit { get; set; }

        public string Value { get; set; }
    }
}

