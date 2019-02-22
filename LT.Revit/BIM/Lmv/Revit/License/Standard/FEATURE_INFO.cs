namespace BIM.Lmv.Revit.License.Standard
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct FEATURE_INFO
    {
        public uint featureId;
        public FEATURE_TYPE type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x80)]
        public byte[] featureName;
        public BIT_DATE_TIME endDateTime;
        public uint expirationDays;
        public uint users;
    }
}

