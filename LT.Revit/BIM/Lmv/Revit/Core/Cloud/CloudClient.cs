namespace BIM.Lmv.Revit.Core.Cloud
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    internal class CloudClient
    {
        private readonly Uri _Endpoint;
        private string _SessionToken;
        private readonly Uri _WXEndpoint;

        public CloudClient(Uri endpoint, Uri wxEndpoint)
        {
            this._Endpoint = endpoint;
            this._WXEndpoint = wxEndpoint;
        }

        public string GetQRCode()
        {
            try
            {
                Uri uri = new Uri(this._WXEndpoint, "wx/getqrcode");
                return HttpHelper.HttpPost(uri.ToString(), null);
            }
            catch (Exception exception)
            {
                InvokeError error = new InvokeError {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return string.Empty;
            }
        }

        public InvokeResultUpload GetUploadPermit()
        {
            try
            {
                Dictionary<string, string> fields = new Dictionary<string, string> {
                    ["session"] = this._SessionToken
                };
                Uri uri = new Uri(this._Endpoint, "api/model/upload");
                return JsonConvert.DeserializeObject<InvokeResultUpload>(HttpHelper.HttpPost(uri.ToString(), fields));
            }
            catch (Exception exception)
            {
                InvokeError error = new InvokeError {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return new InvokeResultUpload { Error = error };
            }
        }

        public bool IsLogined() => 
            !string.IsNullOrEmpty(this._SessionToken);

        public string Login(string token)
        {
            try
            {
                Uri uri = new Uri(this._WXEndpoint, "WX/Login");
                Dictionary<string, string> fields = new Dictionary<string, string> {
                    ["token"] = token
                };
                string str = HttpHelper.HttpPost(uri.ToString(), fields);
                this._SessionToken = str;
                return str;
            }
            catch (Exception exception)
            {
                InvokeError error = new InvokeError {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return string.Empty;
            }
        }

        public InvokeResultLogin Login(string userName, string userPwd)
        {
            try
            {
                Dictionary<string, string> fields = new Dictionary<string, string> {
                    ["userName"] = userName,
                    ["userPwd"] = userPwd
                };
                Uri uri = new Uri(this._Endpoint, "api/user/login");
                InvokeResultLogin login = JsonConvert.DeserializeObject<InvokeResultLogin>(HttpHelper.HttpPost(uri.ToString(), fields));
                if (login.Success)
                {
                    this._SessionToken = login.Token;
                }
                else
                {
                    this._SessionToken = string.Empty;
                }
                return login;
            }
            catch (Exception exception)
            {
                InvokeError error = new InvokeError {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return new InvokeResultLogin { 
                    Error = error,
                    Success = false
                };
            }
        }

        public InvokeResultRegisterModel RegisterModel(string token, string modelName, DateTime expirationDate, bool hasPwd, string workId, string options)
        {
            try
            {
                Dictionary<string, string> fields = new Dictionary<string, string> {
                    { 
                        "session",
                        this._SessionToken
                    },
                    { 
                        "modelToken",
                        token
                    },
                    { 
                        "modelName",
                        modelName
                    },
                    { 
                        "expirationDate",
                        expirationDate.ToString("yyyy-MM-dd")
                    },
                    { 
                        "hasPwd",
                        hasPwd.ToString()
                    },
                    { 
                        "workId",
                        workId
                    },
                    { 
                        "options",
                        options
                    }
                };
                Uri uri = new Uri(this._Endpoint, "api/model/register");
                InvokeResultRegisterModel model = JsonConvert.DeserializeObject<InvokeResultRegisterModel>(HttpHelper.HttpPost(uri.ToString(), fields));
                model.ShareUrl = new Uri(this._Endpoint, model.ShareUrl).ToString();
                return model;
            }
            catch (Exception exception)
            {
                InvokeError error = new InvokeError {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return new InvokeResultRegisterModel { Error = error };
            }
        }

        public bool SendTemplateMsg(string token, string link)
        {
            try
            {
                Uri uri = new Uri(this._WXEndpoint, "wx/SendTemplateMsg");
                Dictionary<string, string> fields = new Dictionary<string, string> {
                    { 
                        "token",
                        token
                    },
                    { 
                        "link",
                        link
                    }
                };
                HttpHelper.HttpPost(uri.ToString(), fields);
                return true;
            }
            catch (Exception exception)
            {
                InvokeError error = new InvokeError {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return false;
            }
        }
    }
}

