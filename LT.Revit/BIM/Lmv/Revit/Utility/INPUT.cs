namespace BIM.Lmv.Revit.Utility
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT
    {
        internal uint type;
        internal InputUnion U;
        internal static int Size =>
            Marshal.SizeOf(typeof(INPUT));
    }
}

