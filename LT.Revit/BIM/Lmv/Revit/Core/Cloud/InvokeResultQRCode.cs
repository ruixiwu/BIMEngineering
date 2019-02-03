using System;

namespace BIM.Lmv.Revit.Core.Cloud
{
    [Serializable]
    internal class InvokeResultQRCode
    {
        public string QRCodeUrl { get; set; }

        public string SceneId { get; set; }

        public string WsUrl { get; set; }
    }
}