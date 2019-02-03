using System;

namespace BIM.Lmv.Revit.Utility
{
    [Flags]
    internal enum KEYEVENTF : uint
    {
        EXTENDEDKEY = 1,
        KEYUP = 2,
        SCANCODE = 8,
        UNICODE = 4
    }
}