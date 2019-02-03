namespace BIM.Lmv.Types
{
    public class PropItem
    {
        public const string FALSE = "false";
        public const string TRUE = "true";
        public PropDef Def;
        public string Value;

        public PropItem(PropDef def, string value)
        {
            Def = def;
            Value = value;
        }
    }
}