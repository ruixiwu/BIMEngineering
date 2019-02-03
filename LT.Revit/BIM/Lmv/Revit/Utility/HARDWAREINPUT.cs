using System.Runtime.InteropServices;

namespace BIM.Lmv.Revit.Utility
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct HARDWAREINPUT
    {
        internal int uMsg;
        internal short wParamL;
        internal short wParamH;
    }
}