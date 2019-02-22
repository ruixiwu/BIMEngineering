namespace SQlite.Data
{
    using System;
    using System.IO;

    public class ZipSimple
    {
        public static void test()
        {
            string path = "d:/BIM-SSH111/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path).Attributes = 0;
            }
        }
    }
}

