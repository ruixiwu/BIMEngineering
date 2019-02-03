using System;

namespace BIM.Lmv.Revit.Core.Cloud
{
    [Serializable]
    internal class InvokeResultRegisterModel : InvokeResultBase
    {
        public string Password = null;
        public string ShareUrl = null;
    }
}