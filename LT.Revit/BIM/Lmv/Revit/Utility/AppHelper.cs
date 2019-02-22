namespace BIM.Lmv.Revit.Utility
{
    using System;
    using System.IO;
    using System.Reflection;

    internal static class AppHelper
    {
        public static string GetPath(string relatePath) => 
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), relatePath);
    }
}

