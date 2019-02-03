using System.Runtime.InteropServices;

namespace BIM.Lmv.Revit.Utility
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT
    {
        internal uint type;
        internal InputUnion U;

        internal static int Size
        {
            get { return Marshal.SizeOf(typeof (INPUT)); }
        }
    }
}