namespace BIM.Lmv.Revit.Helpers
{
    internal class FamilyItem
    {
        public FamilyItem(string sSection, string sName, string sValue, string sUnit, string sType, string sFamilyID)
        {
            Section = sSection;
            Name = sName;
            Value = sValue;
            Unit = sUnit;
            Type = sType;
            FamilyID = sFamilyID;
        }

        public string FamilyID { get; set; }

        public string Name { get; set; }

        public string Section { get; set; }

        public string Type { get; set; }

        public string Unit { get; set; }

        public string Value { get; set; }
    }
}