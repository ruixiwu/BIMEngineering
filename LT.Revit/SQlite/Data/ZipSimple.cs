using System.IO;

namespace SQlite.Data
{
    public class ZipSimple
    {
        public static void test()
        {
            var path = "d:/BIM-SSH111/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path).Attributes = 0;
            }
        }
    }
}