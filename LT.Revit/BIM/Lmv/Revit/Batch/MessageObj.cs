using System;
using System.Collections.Generic;

namespace BIM.Lmv.Revit.Batch
{
    [Serializable]
    internal class MessageObj
    {
        public List<string> Items;
        public string Key;

        public MessageObj(string key, params string[] items)
        {
            Key = key;
            Items = new List<string>(items.Length);
            Items.AddRange(items);
        }
    }
}