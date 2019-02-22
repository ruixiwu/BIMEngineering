namespace BIM.Lmv.Revit.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal class PropertyItem
    {
        public PropertyItem(string sSection, string sName, string sValue, string sUnit, string sType)
        {
            this.Section = sSection;
            this.Name = sName;
            this.Value = sValue;
            this.Unit = sUnit;
            this.Type = sType;
        }

        public string Name { get; set; }

        public string Section { get; set; }

        public string Type { get; set; }

        public string Unit { get; set; }

        public string Value { get; set; }
    }
}

