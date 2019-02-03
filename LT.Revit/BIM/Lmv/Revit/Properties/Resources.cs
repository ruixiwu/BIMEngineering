using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace BIM.Lmv.Revit.Properties
{
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode,
     CompilerGenerated]
    internal class Resources
    {
        private static ResourceManager resourceMan;

        internal static Icon app
        {
            get { return (Icon) ResourceManager.GetObject("app", Culture); }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture { get; set; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (ReferenceEquals(resourceMan, null))
                {
                    var manager =
                        new ResourceManager("BIM.Lmv.Revit.Properties.Resources",
                            typeof (Resources).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static Bitmap share_128px
        {
            get { return (Bitmap) ResourceManager.GetObject("share_128px", Culture); }
        }

        internal static Bitmap share_128px_1201342
        {
            get { return (Bitmap) ResourceManager.GetObject("share_128px_1201342", Culture); }
        }

        internal static Bitmap share_16px
        {
            get { return (Bitmap) ResourceManager.GetObject("share_16px", Culture); }
        }

        internal static Bitmap share_16px_1201342
        {
            get { return (Bitmap) ResourceManager.GetObject("share_16px_1201342", Culture); }
        }

        internal static Bitmap share_32px
        {
            get { return (Bitmap) ResourceManager.GetObject("share_32px", Culture); }
        }

        internal static Bitmap share_32px_1201342
        {
            get { return (Bitmap) ResourceManager.GetObject("share_32px_1201342", Culture); }
        }
    }
}