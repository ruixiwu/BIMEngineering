using System.Runtime.InteropServices;

namespace BIM.Lmv.Revit.Utility
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct InputUnion
    {
        [FieldOffset(0)] internal HARDWAREINPUT hi;
        [FieldOffset(0)] internal KEYBDINPUT ki;
        [FieldOffset(0)] internal MOUSEINPUT mi;
    }
}