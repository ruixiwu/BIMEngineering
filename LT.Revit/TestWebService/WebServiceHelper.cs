using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Net;
using System.Text;
using System.Web.Services.Description;
using Microsoft.CSharp;

namespace TestWebService
{
    public class WebServiceHelper
    {
        private static string GetWsClassName(string wsUrl)
        {
            var strArray = wsUrl.Split('/');
            return strArray[strArray.Length - 1].Split('.')[0];
        }

        public static object InvokeWebService(string url, string methodname, object[] args)
        {
            return InvokeWebService(url, null, methodname, args);
        }

        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            object obj3;
            var name = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = GetWsClassName(url);
            }
            try
            {
                var client = new WebClient();
                var serviceDescription = ServiceDescription.Read(client.OpenRead(url + "?WSDL"));
                var importer = new ServiceDescriptionImporter();
                importer.AddServiceDescription(serviceDescription, "", "");
                var namespace2 = new CodeNamespace(name);
                var codeCompileUnit = new CodeCompileUnit();
                codeCompileUnit.Namespaces.Add(namespace2);
                importer.Import(namespace2, codeCompileUnit);
                new CSharpCodeProvider();
                var options = new CompilerParameters
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true,
                    ReferencedAssemblies =
                    {
                        "System.dll",
                        "System.XML.dll",
                        "System.Web.Services.dll",
                        "System.Data.dll"
                    }
                };
                var results = new CSharpCodeProvider().CompileAssemblyFromDom(options, codeCompileUnit);
                if (results.Errors.HasErrors)
                {
                    var builder = new StringBuilder();
                    foreach (CompilerError error in results.Errors)
                    {
                        builder.Append(error);
                        builder.Append(Environment.NewLine);
                    }
                    throw new Exception(builder.ToString());
                }
                var type = results.CompiledAssembly.GetType(name + "." + classname, true, true);
                var obj2 = Activator.CreateInstance(type);
                obj3 = type.GetMethod(methodname).Invoke(obj2, args);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.InnerException.Message, new Exception(exception.InnerException.StackTrace));
            }
            return obj3;
        }
    }
}