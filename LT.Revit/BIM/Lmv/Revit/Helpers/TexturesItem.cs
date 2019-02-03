namespace BIM.Lmv.Revit.Helpers
{
    internal class TexturesItem
    {
        public TexturesItem()
        {
        }

        public TexturesItem(string sName, string sFormat, string sMagfilter, string sMinfilter, string sWraps,
            string sWrapt, string sImagename)
        {
            Name = sName;
            Format = sFormat;
            Magfilter = sMagfilter;
            Minfilter = sMinfilter;
            Wraps = sWraps;
            Wrapt = sWrapt;
            Imagename = sImagename;
        }

        public string Format { get; set; }

        public string Imagename { get; set; }

        public string Magfilter { get; set; }

        public string Minfilter { get; set; }

        public string Name { get; set; }

        public string ObjectId { get; set; }

        public string Wraps { get; set; }

        public string Wrapt { get; set; }
    }
}