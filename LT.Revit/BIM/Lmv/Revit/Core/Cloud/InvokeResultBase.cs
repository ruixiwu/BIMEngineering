using System;

namespace BIM.Lmv.Revit.Core.Cloud
{
    [Serializable]
    internal abstract class InvokeResultBase
    {
        public InvokeError Error;
    }
}