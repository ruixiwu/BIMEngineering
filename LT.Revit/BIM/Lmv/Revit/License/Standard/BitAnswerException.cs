namespace BIM.Lmv.Revit.License.Standard
{
    using System;
    using System.Runtime.CompilerServices;

    internal class BitAnswerException : Exception
    {
        public BitAnswerException(int status)
        {
            this.ErrorCode = status;
        }

        public int ErrorCode { get; set; }

        public override string Message =>
            ("ErrorCode: " + this.ErrorCode.ToString());
    }
}

