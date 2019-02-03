using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BIM.Lmv.Revit.Core.Cloud
{
    internal class CloudClient
    {
        private readonly Uri _Endpoint;
        private readonly Uri _WXEndpoint;
        private string _SessionToken;

        public CloudClient(Uri endpoint, Uri wxEndpoint)
        {
            _Endpoint = endpoint;
            _WXEndpoint = wxEndpoint;
        }

        public string GetQRCode()
        {
            try
            {
                var uri = new Uri(_WXEndpoint, "wx/getqrcode");
                return HttpHelper.HttpPost(uri.ToString(), null);
            }
            catch (Exception exception)
            {
                var error = new InvokeError
                {
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
                var fields = new Dictionary<string, string>();
                fields["session"] = _SessionToken;
                var uri = new Uri(_Endpoint, "api/model/upload");
                return JsonConvert.DeserializeObject<InvokeResultUpload>(HttpHelper.HttpPost(uri.ToString(), fields));
            }
            catch (Exception exception)
            {
                var error = new InvokeError
                {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return new InvokeResultUpload {Error = error};
            }
        }

        public bool IsLogined()
        {
            return !string.IsNullOrEmpty(_SessionToken);
        }

        public string Login(string token)
        {
            try
            {
                var uri = new Uri(_WXEndpoint, "WX/Login");
                var fields = new Dictionary<string, string>();
                fields["token"] = token;
                var str = HttpHelper.HttpPost(uri.ToString(), fields);
                _SessionToken = str;
                return str;
            }
            catch (Exception exception)
            {
                var error = new InvokeError
                {
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
                var fields = new Dictionary<string, string>();
                fields["userName"] = userName;
                fields["userPwd"] = userPwd;
                var uri = new Uri(_Endpoint, "api/user/login");
                var login =
                    JsonConvert.DeserializeObject<InvokeResultLogin>(HttpHelper.HttpPost(uri.ToString(), fields));
                if (login.Success)
                {
                    _SessionToken = login.Token;
                }
                else
                {
                    _SessionToken = string.Empty;
                }
                return login;
            }
            catch (Exception exception)
            {
                var error = new InvokeError
                {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return new InvokeResultLogin
                {
                    Error = error,
                    Success = false
                };
            }
        }

        public InvokeResultRegisterModel RegisterModel(string token, string modelName, DateTime expirationDate,
            bool hasPwd, string workId, string options)
        {
            try
            {
                var fields = new Dictionary<string, string>
                {
                    {
                        "session",
                        _SessionToken
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
                var uri = new Uri(_Endpoint, "api/model/register");
                var model =
                    JsonConvert.DeserializeObject<InvokeResultRegisterModel>(HttpHelper.HttpPost(uri.ToString(), fields));
                model.ShareUrl = new Uri(_Endpoint, model.ShareUrl).ToString();
                return model;
            }
            catch (Exception exception)
            {
                var error = new InvokeError
                {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return new InvokeResultRegisterModel {Error = error};
            }
        }

        public bool SendTemplateMsg(string token, string link)
        {
            try
            {
                var uri = new Uri(_WXEndpoint, "wx/SendTemplateMsg");
                var fields = new Dictionary<string, string>
                {
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
                var error = new InvokeError
                {
                    ErrorCode = -1,
                    ErrorMsg = exception.ToString()
                };
                return false;
            }
        }
    }
}