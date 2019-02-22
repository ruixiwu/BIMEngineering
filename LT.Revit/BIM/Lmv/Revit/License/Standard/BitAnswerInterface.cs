namespace BIM.Lmv.Revit.License.Standard
{
    using System;

    internal interface BitAnswerInterface
    {
        int ApplyUpdateInfo(string updateInfo, byte[] receiptInfo, ref uint receiptInfoSize);
        int CheckIn(string url, uint featureId);
        int CheckOutFeatures(string url, uint[] featureIds, uint durationDays);
        int CheckOutSn(string url, uint featureId, uint durationDays);
        int ConvertFeature(uint featureId, uint para1, uint para2, uint para3, uint para4, ref uint result);
        int DecryptFeature(uint featureId, byte[] cipherBuffer, byte[] plainBuffer);
        int EncryptFeature(uint featureId, byte[] plainBuffer, byte[] cipherBuffer);
        int GetDataItem(string dataItemName, byte[] dataItemValue, ref uint dataItemValueLen);
        int GetDataItemName(uint index, byte[] name, ref uint nameLen);
        int GetDataItemNum(ref uint number);
        int GetFeatureInfo(uint featureId, ref FEATURE_INFO featureInfo);
        int GetInfo(string sn, uint type, byte[] info, ref uint infoSize);
        int GetProductPath(byte[] productPath, ref uint productPathSize);
        int GetRequestInfo(string sn, uint bindingType, byte[] requestInfo, ref uint requestInfoSize);
        int GetSessionInfo(SessionType type, byte[] sessionInfo, ref uint sessionInfoLen);
        int GetUpdateInfo(string url, string sn, string requestInfo, byte[] updateInfo, ref uint updateInfoSize);
        int GetVersion(ref uint version);
        int Login(string url, string sn, int mode);
        int LoginEx(string url, string sn, uint featureId, string xmlScope, int mode);
        int Logout();
        int QueryFeature(uint featureId, ref uint capacity);
        int ReadFeature(uint featureId, ref uint featureValue);
        int ReleaseFeature(uint featureId, ref uint capacity);
        int RemoveDataItem(string dataItemName);
        int RemoveSn(string sn);
        int Revoke(string url, string sn, byte[] revocationInfo, ref uint revocationInfoSize);
        int SetDataItem(string dataItemName, byte[] dataItemValue);
        int SetDbPath(string path);
        int SetLocalServer(string host, uint port, uint timeoutSeconds);
        int SetProxy(string hostName, uint port, string userId, string password);
        int TestBitService(string url, string sn, uint featureId);
        int UpdateOnline(string url, string sn);
        int WriteFeature(uint featureId, uint featureValue);
    }
}

