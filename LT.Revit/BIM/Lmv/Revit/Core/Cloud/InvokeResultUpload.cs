namespace BIM.Lmv.Revit.Core.Cloud
{
    using System;

    [Serializable]
    internal class InvokeResultUpload : InvokeResultBase
    {
        public string AccessKeyId = null;
        public string ObjectKey = null;
        public string OssServer = null;
        public string Policy = null;
        public string Signature = null;
        public string Token = null;
    }
}

