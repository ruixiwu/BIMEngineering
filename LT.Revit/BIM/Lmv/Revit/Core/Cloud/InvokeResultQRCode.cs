namespace BIM.Lmv.Revit.Core.Cloud
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    internal class InvokeResultQRCode
    {
        public string QRCodeUrl { get; set; }

        public string SceneId { get; set; }

        public string WsUrl { get; set; }
    }
}

