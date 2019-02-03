using System;

namespace BIM.Lmv.Revit.Core.Cloud
{
    [Serializable]
    internal class InvokeResultLogin : InvokeResultBase
    {
        public bool Success = false;
        public string Token = null;
    }
}