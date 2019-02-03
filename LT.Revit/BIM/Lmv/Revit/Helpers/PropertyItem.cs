namespace BIM.Lmv.Revit.Helpers
{
    internal class PropertyItem
    {
        public PropertyItem(string sSection, string sName, string sValue, string sUnit, string sType)
        {
            Section = sSection;
            Name = sName;
            Value = sValue;
            Unit = sUnit;
            Type = sType;
        }

        public string Name { get; set; }

        public string Section { get; set; }

        public string Type { get; set; }

        public string Unit { get; set; }

        public string Value { get; set; }
    }
}