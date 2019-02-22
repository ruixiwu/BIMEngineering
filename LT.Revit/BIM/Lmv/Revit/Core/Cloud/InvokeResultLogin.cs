namespace BIM.Lmv.Revit.Core.Cloud
{
    using System;

    [Serializable]
    internal class InvokeResultLogin : InvokeResultBase
    {
        public bool Success = false;
        public string Token = null;
    }
}

