namespace BIM.Lmv.Types
{
    using System;

    public class PropItem
    {
        public PropDef Def;
        public const string FALSE = "false";
        public const string TRUE = "true";
        public string Value;

        public PropItem(PropDef def, string value)
        {
            this.Def = def;
            this.Value = value;
        }
    }
}

