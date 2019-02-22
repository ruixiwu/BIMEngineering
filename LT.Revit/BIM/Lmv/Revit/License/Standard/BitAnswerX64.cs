namespace BIM.Lmv.Revit.License.Standard
{
    using System;
    using System.Runtime.InteropServices;

    internal class BitAnswerX64 : BitAnswerInterface
    {
        private static byte[] applicationData = new byte[] { 
            0x40, 0x80, 0xbc, 0x49, 8, 0xcc, 0xd1, 0x84, 0x24, 0xde, 80, 180, 0x1b, 0x29, 0x9c, 0x36,
            0xbf, 0xd4, 180, 0x58, 0x9d, 0xa9, 0x26, 0xf4, 0x60, 0xb9, 230, 14, 0xbb, 0xf2, 0x6d, 0x3b,
            0x38, 0xdb, 70, 0xf1, 0x4f, 0x51, 0xa9, 0xe4, 0xf8, 0x48, 0x24, 0xe8, 0xd7, 0x61, 0xd5, 0xc9,
            0xe4, 0xc3, 0xbc, 0x9d, 0x70, 0x33, 5, 0xba, 12, 0x41, 0x40, 0xfe, 0x79, 4, 0x35, 0x89,
            0x6c, 0x9d, 0xfd, 0x4e, 8, 0x3b, 0x79, 0xfe, 0xd6, 0xad, 0xe9, 0x25, 0x2f, 0x52, 0x2d, 0xd8,
            0x4e, 0xb1, 0xc0, 20, 40, 0xee, 0x6f, 0x87, 0xde, 0x43, 0xdb, 0xc7, 0x94, 0xc4, 120, 0x87,
            0x85, 0xcc, 250, 0x9c, 0x25, 0xb9, 0x10, 0x1a, 0x76, 0x60, 0x37, 0xee, 0x81, 0x6f, 0x24, 0x9c,
            0x19, 0x58, 0xe1, 0x92, 0xba, 14, 0x20, 0x92, 0xbf, 0x98, 0x36, 0x89, 0x99, 0x68, 0x1a, 0x8b,
            0x9a, 0x85, 0xcb, 0x8f, 0x5e, 0x6b, 0x12, 0x4d, 0x5d, 0x8e, 0xab, 0xb5, 0x88, 0x94, 0x57, 0xd7,
            0x89, 0xb8, 4, 0xbf, 0x9f, 0x94, 0xb2, 0x8b, 0x67, 9, 0x45, 0x77, 60, 0xff, 0x85, 70,
            0x5d, 0x80, 0xb1, 0x73, 0xc7, 0xcb, 0xad, 0xe8, 12, 80, 0, 0xfe, 0x79, 0xfc, 0x25, 0xb7,
            0xf7, 0x81, 15, 0xc3, 0xd1, 0xea, 0x88, 100, 0x68, 0xca, 0x7a, 0x1b, 0x60, 0, 0xc9
        };
        public const string BitAnswerDllName = "00002e81_00000001_x64";
        private uint handle;

        public int ApplyUpdateInfo(string updateInfo, byte[] receiptInfo, ref uint receiptInfoSize) => 
            Bit_ApplyUpdateInfo(applicationData, updateInfo, receiptInfo, ref receiptInfoSize);

        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_ApplyUpdateInfo(byte[] applicationData, string updateInfo, byte[] receiptInfo, ref uint receiptInfoSize);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_CheckIn(string url, uint featureId, byte[] applicationData);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_CheckOutFeatures(string url, byte[] applicationData, uint[] features, uint featuresSize, uint durationDays);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_CheckOutSn(string url, uint featureId, byte[] applicationData, uint durationDays);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_ConvertFeature(uint handle, uint featureId, uint para1, uint para2, uint para3, uint para4, ref uint result);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_DecryptFeature(uint handle, uint featureId, byte[] plainBuffer, byte[] cipherBuffer, uint bufferLen);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_EncryptFeature(uint handle, uint featureId, byte[] plainBuffer, byte[] cipherBuffer, uint bufferLen);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetDataItem(uint handle, string dataItemName, byte[] dataItemValue, ref uint dataItemValueLen);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetDataItemName(uint handle, uint index, byte[] dataItemName, ref uint DataItemNameLen);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetDataItemNum(uint handle, ref uint number);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetFeatureInfo(uint handle, uint featureId, ref FEATURE_INFO featureInfo);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetInfo(string sn, byte[] applicationData, uint type, byte[] info, ref uint infoSize);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetProductPath(byte[] applicationData, byte[] productPath, ref uint productPathSize);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetRequestInfo(string sn, byte[] applicationData, uint bindingType, byte[] requestInfo, ref uint requestInfoSize);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetSessionInfo(uint handle, uint type, byte[] sessionInfo, ref uint sessionInfoLen);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetUpdateInfo(string url, string sn, byte[] applicationData, string requestInfo, byte[] updateInfo, ref uint updateInfoSize);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_GetVersion(ref uint version);
        [DllImport("00002e81_00000001_x64")]
        public static extern int Bit_Login(string url, string sn, byte[] applicationData, ref uint handle, int mode);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_LoginEx(string url, string sn, uint featureId, string xmlScope, byte[] applicationData, ref uint handle, int mode);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_Logout(uint handle);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_QueryFeature(uint handle, uint featureId, ref uint capacity);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_ReadFeature(uint handle, uint featureId, ref uint featureValue);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_ReleaseFeature(uint handle, uint featureId, ref uint capacity);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_RemoveDataItem(uint handle, string dataItemName);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_RemoveSn(string sn, byte[] applicationData);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_Revoke(string url, string sn, byte[] applicationData, byte[] revocationInfo, ref uint revocationInfoSize);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_SetDataItem(uint handle, string dataItemName, byte[] dataItemValue, uint dataItemValueLen);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_SetDbPath(string szPath);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_SetLocalServer(byte[] applicationData, string host, uint port, uint timeoutSeconds);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_SetProxy(byte[] applicationData, string hostName, uint port, string userId, string password);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_TestBitService(string url, string sn, uint featureId, byte[] applicationData);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_UpdateOnline(string url, string sn, byte[] applicationData);
        [DllImport("00002e81_00000001_x64")]
        private static extern int Bit_WriteFeature(uint handle, uint featureId, uint featureValue);
        public int CheckIn(string url, uint featureId) => 
            Bit_CheckIn(url, featureId, applicationData);

        public int CheckOutFeatures(string url, uint[] featureIds, uint durationDays)
        {
            uint featuresSize = 0;
            if (featureIds != null)
            {
                featuresSize = (uint) featureIds.Length;
            }
            return Bit_CheckOutFeatures(url, applicationData, featureIds, featuresSize, durationDays);
        }

        public int CheckOutSn(string url, uint featureId, uint durationDays) => 
            Bit_CheckOutSn(url, featureId, applicationData, durationDays);

        public int ConvertFeature(uint featureId, uint para1, uint para2, uint para3, uint para4, ref uint result) => 
            Bit_ConvertFeature(this.handle, featureId, para1, para2, para3, para4, ref result);

        public int DecryptFeature(uint featureId, byte[] cipherBuffer, byte[] plainBuffer) => 
            Bit_DecryptFeature(this.handle, featureId, cipherBuffer, plainBuffer, (uint) cipherBuffer.Length);

        public int EncryptFeature(uint featureId, byte[] plainBuffer, byte[] cipherBuffer) => 
            Bit_EncryptFeature(this.handle, featureId, plainBuffer, cipherBuffer, (uint) plainBuffer.Length);

        public int GetDataItem(string dataItemName, byte[] dataItemValue, ref uint dataItemValueLen) => 
            Bit_GetDataItem(this.handle, dataItemName, dataItemValue, ref dataItemValueLen);

        public int GetDataItemName(uint index, byte[] name, ref uint nameLen) => 
            Bit_GetDataItemName(this.handle, index, name, ref nameLen);

        public int GetDataItemNum(ref uint number) => 
            Bit_GetDataItemNum(this.handle, ref number);

        public int GetFeatureInfo(uint featureId, ref FEATURE_INFO featureInfo) => 
            Bit_GetFeatureInfo(this.handle, featureId, ref featureInfo);

        public int GetInfo(string sn, uint type, byte[] info, ref uint infoSize) => 
            Bit_GetInfo(sn, applicationData, type, info, ref infoSize);

        public int GetProductPath(byte[] productPath, ref uint productPathSize) => 
            Bit_GetProductPath(applicationData, productPath, ref productPathSize);

        public int GetRequestInfo(string sn, uint bindingType, byte[] requestInfo, ref uint requestInfoSize) => 
            Bit_GetRequestInfo(sn, applicationData, bindingType, requestInfo, ref requestInfoSize);

        public int GetSessionInfo(SessionType type, byte[] sessionInfo, ref uint sessionInfoLen) => 
            Bit_GetSessionInfo(this.handle, (uint) type, sessionInfo, ref sessionInfoLen);

        public int GetUpdateInfo(string url, string sn, string requestInfo, byte[] updateInfo, ref uint updateInfoSize) => 
            Bit_GetUpdateInfo(url, sn, applicationData, requestInfo, updateInfo, ref updateInfoSize);

        public int GetVersion(ref uint version) => 
            Bit_GetVersion(ref version);

        public int Login(string url, string sn, int mode) => 
            Bit_Login(url, sn, applicationData, ref this.handle, mode);

        public int LoginEx(string url, string sn, uint featureId, string xmlScope, int mode) => 
            Bit_LoginEx(url, sn, featureId, xmlScope, applicationData, ref this.handle, mode);

        public int Logout() => 
            Bit_Logout(this.handle);

        public int QueryFeature(uint featureId, ref uint capacity) => 
            Bit_QueryFeature(this.handle, featureId, ref capacity);

        public int ReadFeature(uint featureId, ref uint featureValue) => 
            Bit_ReadFeature(this.handle, featureId, ref featureValue);

        public int ReleaseFeature(uint featureId, ref uint capacity) => 
            Bit_ReleaseFeature(this.handle, featureId, ref capacity);

        public int RemoveDataItem(string dataItemName) => 
            Bit_RemoveDataItem(this.handle, dataItemName);

        public int RemoveSn(string sn) => 
            Bit_RemoveSn(sn, applicationData);

        public int Revoke(string url, string sn, byte[] revocationInfo, ref uint revocationInfoSize) => 
            Bit_Revoke(url, sn, applicationData, revocationInfo, ref revocationInfoSize);

        public int SetDataItem(string dataItemName, byte[] dataItemValue) => 
            Bit_SetDataItem(this.handle, dataItemName, dataItemValue, (uint) dataItemValue.Length);

        public int SetDbPath(string path) => 
            Bit_SetDbPath(path);

        public int SetLocalServer(string host, uint port, uint timeoutSeconds) => 
            Bit_SetLocalServer(applicationData, host, port, timeoutSeconds);

        public int SetProxy(string hostName, uint port, string userId, string password) => 
            Bit_SetProxy(applicationData, hostName, port, userId, password);

        public int TestBitService(string url, string sn, uint featureId) => 
            Bit_TestBitService(url, sn, featureId, applicationData);

        public int UpdateOnline(string url, string sn) => 
            Bit_UpdateOnline(url, sn, applicationData);

        public int WriteFeature(uint featureId, uint featureValue) => 
            Bit_WriteFeature(this.handle, featureId, featureValue);
    }
}

