namespace BIM.Lmv.Types
{
    using System;
    using System.Runtime.CompilerServices;

    public class PropDef
    {
        public static PropDef DefCategory = new PropDef("Category", "__category__", PropDataType.String, null, true);
        public static PropDef DefChild = new PropDef("child", "__child__", PropDataType.Ref, null, true);
        public static PropDef DefHastable = new PropDef("hastable", "__hastable__", PropDataType.Ref, null, true);
        public static PropDef DefHyperlink = new PropDef("hyperlink", "__hyperlink__", PropDataType.String, null, true);
        public static PropDef DefInstanceof = new PropDef("instanceof_objid", "__instanceof__", PropDataType.Ref, null, true);
        public static PropDef DefIsDocument = new PropDef("is_doc_property", "__document__", PropDataType.Boolean, null, true);
        public static PropDef DefMark = new PropDef("技术支持", "标识", PropDataType.String, null, false);
        public static PropDef DefName = new PropDef("name", "__name__", PropDataType.String, null, false);
        public static PropDef DefParent = new PropDef("parent", "__parent__", PropDataType.Ref, null, true);
        public static PropDef DefSchemaName = new PropDef("schema_name", "__document__", PropDataType.String, null, true);
        public static PropDef DefSchemaVersion = new PropDef("schema_version", "__document__", PropDataType.String, null, true);
        public static PropDef DefViewableIn = new PropDef("viewable_in", "__viewable_in__", PropDataType.String, null, true);
        public static PropDef DefXrefId = new PropDef("xref", "__externalref__", PropDataType.String, null, true);
        public static PropDef[] PredefList = new PropDef[] { DefName, DefChild, DefParent, DefInstanceof, DefHastable, DefViewableIn, DefXrefId, DefIsDocument, DefSchemaName, DefSchemaVersion, DefHyperlink, DefCategory };

        public PropDef(string name, string category, PropDataType type, string unit, bool hide)
        {
            this.Name = name;
            this.Category = category;
            this.Unit = unit;
            this.Type = type;
            this.Hide = hide;
        }

        public string Category { get; set; }

        public bool Hide { get; set; }

        public string Name { get; set; }

        public PropDataType Type { get; set; }

        public string Unit { get; set; }
    }
}

