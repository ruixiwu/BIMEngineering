namespace BIM.Lmv.Types
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class ExportOption
    {
        public ExportOption()
        {
            this.Target = ExportTarget.LocalPackage;
            this.OutputStream = null;
        }

        public Stream OutputStream { get; set; }

        public ExportTarget Target { get; set; }
    }
}

