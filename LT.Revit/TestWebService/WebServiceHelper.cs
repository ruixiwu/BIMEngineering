namespace TestWebService
{
    using Microsoft.CSharp;
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Net;
    using System.Text;
    using System.Web.Services.Description;

    public class WebServiceHelper
    {
        private static string GetWsClassName(string wsUrl)
        {
            string[] strArray = wsUrl.Split(new char[] { '/' });
            return strArray[strArray.Length - 1].Split(new char[] { '.' })[0];
        }

        public static object InvokeWebService(string url, string methodname, object[] args) => 
            InvokeWebService(url, null, methodname, args);

        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            object obj3;
            string name = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = GetWsClassName(url);
            }
            try
            {
                WebClient client = new WebClient();
                ServiceDescription serviceDescription = ServiceDescription.Read(client.OpenRead(url + "?WSDL"));
                ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
                importer.AddServiceDescription(serviceDescription, "", "");
                CodeNamespace namespace2 = new CodeNamespace(name);
                CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
                codeCompileUnit.Namespaces.Add(namespace2);
                importer.Import(namespace2, codeCompileUnit);
                new CSharpCodeProvider();
                CompilerParameters options = new CompilerParameters {
                    GenerateExecutable = false,
                    GenerateInMemory = true,
                    ReferencedAssemblies = { 
                        "System.dll",
                        "System.XML.dll",
                        "System.Web.Services.dll",
                        "System.Data.dll"
                    }
                };
                CompilerResults results = new CSharpCodeProvider().CompileAssemblyFromDom(options, new CodeCompileUnit[] { codeCompileUnit });
                if (results.Errors.HasErrors)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (CompilerError error in results.Errors)
                    {
                        builder.Append(error.ToString());
                        builder.Append(Environment.NewLine);
                    }
                    throw new Exception(builder.ToString());
                }
                Type type = results.CompiledAssembly.GetType(name + "." + classname, true, true);
                object obj2 = Activator.CreateInstance(type);
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

