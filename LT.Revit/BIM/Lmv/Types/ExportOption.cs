using System.IO;

namespace BIM.Lmv.Types
{
    public class ExportOption
    {
        public ExportOption()
        {
            Target = ExportTarget.LocalPackage;
            OutputStream = null;
        }

        public Stream OutputStream { get; set; }

        public ExportTarget Target { get; set; }
    }
}