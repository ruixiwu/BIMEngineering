using Newtonsoft.Json.Linq;

namespace BIM.Lmv.Content.Other
{
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
            var obj2 = new JObject();
            if (id != null)
            {
                obj2["id"] = id;
            }
            if (type != null)
            {
                obj2["type"] = type;
            }
            if (typeset != null)
            {
                obj2["typeset"] = typeset;
            }
            if (uri != null)
            {
                obj2["URI"] = uri;
            }
            if (size.HasValue)
            {
                obj2["size"] = size;
            }
            if (usize.HasValue)
            {
                obj2["usize"] = usize;
            }
            return obj2;
        }
    }
}