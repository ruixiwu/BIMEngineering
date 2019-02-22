namespace BIM.Lmv.Revit.License.Standard
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct BIT_DATE_TIME
    {
        public ushort year;
        public byte month;
        public byte dayOfMonth;
        public byte hour;
        public byte minute;
        public byte second;
        public byte unused;
    }
}

