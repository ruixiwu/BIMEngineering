namespace BIM.Lmv.Revit.Batch
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    internal class MessageObj
    {
        public List<string> Items;
        public string Key;

        public MessageObj(string key, params string[] items)
        {
            this.Key = key;
            this.Items = new List<string>(items.Length);
            this.Items.AddRange(items);
        }
    }
}

