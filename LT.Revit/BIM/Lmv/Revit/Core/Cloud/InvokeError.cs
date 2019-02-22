namespace BIM.Lmv.Revit.Core.Cloud
{
    using System;

    [Serializable]
    internal class InvokeError
    {
        public int ErrorCode = -1;
        public string ErrorMsg = null;
    }
}

