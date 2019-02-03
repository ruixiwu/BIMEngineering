using System.IO;
using System.Reflection;

namespace BIM.Lmv.Revit.Utility
{
    internal static class AppHelper
    {
        public static string GetPath(string relatePath)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), relatePath);
        }
    }
}