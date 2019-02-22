namespace BIM.Lmv.Revit.Core.Cloud
{
    using System;

    [Serializable]
    internal abstract class InvokeResultBase
    {
        public InvokeError Error;

        protected InvokeResultBase()
        {
        }
    }
}

