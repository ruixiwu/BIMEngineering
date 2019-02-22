namespace BIM.Lmv.Revit.Utility
{
    using System;

    [Flags]
    internal enum MOUSEEVENTF : uint
    {
        ABSOLUTE = 0x8000,
        HWHEEL = 0x1000,
        LEFTDOWN = 2,
        LEFTUP = 4,
        MIDDLEDOWN = 0x20,
        MIDDLEUP = 0x40,
        MOVE = 1,
        MOVE_NOCOALESCE = 0x2000,
        RIGHTDOWN = 8,
        RIGHTUP = 0x10,
        VIRTUALDESK = 0x4000,
        WHEEL = 0x800,
        XDOWN = 0x80,
        XUP = 0x100
    }
}

