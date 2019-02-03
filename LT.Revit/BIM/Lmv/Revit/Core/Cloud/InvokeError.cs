using System;

namespace BIM.Lmv.Revit.Core.Cloud
{
    [Serializable]
    internal class InvokeError
    {
        public int ErrorCode = -1;
        public string ErrorMsg = null;
    }
}