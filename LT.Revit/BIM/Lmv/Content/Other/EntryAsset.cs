namespace BIM.Lmv.Content.Other
{
    using Newtonsoft.Json.Linq;
    using System;

    internal class EntryAsset
    {
        public string id;
        public int? size;
        public string type;
        public string typeset;
        public string uri;
        public int? usize;

        public EntryAsset(string id, string type, string uri, int? size, int? usize, string typeset)
        {
            this.id = id;
            this.type = type;
            this.uri = uri;
            this.size = size;
            this.usize = usize;
            this.typeset = typeset;
        }

        public JObject GetData()
        {
            JObject obj2 = new JObject();
            if (this.id != null)
            {
                obj2["id"] = this.id;
            }
            if (this.type != null)
            {
                obj2["type"] = this.type;
            }
            if (this.typeset != null)
            {
                obj2["typeset"] = this.typeset;
            }
            if (this.uri != null)
            {
                obj2["URI"] = this.uri;
            }
            if (this.size.HasValue)
            {
                obj2["size"] = this.size;
            }
            if (this.usize.HasValue)
            {
                obj2["usize"] = this.usize;
            }
            return obj2;
        }
    }
}

