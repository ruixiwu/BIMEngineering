﻿namespace BIM.Lmv.Revit.Core.Cloud
{
    using System;

    [Serializable]
    internal class InvokeResultRegisterModel : InvokeResultBase
    {
        public string Password = null;
        public string ShareUrl = null;
    }
}

