namespace BIM.Lmv.Revit.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
    internal class Resources
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal Resources()
        {
        }

        //internal static Icon app
        //{
        //    get { return (Icon) ResourceManager.GetObject("app", resourceCulture); }  
        //}
        internal static Icon app
        {
            get { return (Icon)ResourceManager.GetObject("app", resourceCulture); }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get { return  resourceCulture;}
           
            set
            {
                resourceCulture = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("BIM.Lmv.Revit.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static Bitmap share_128px =>
            ((Bitmap) ResourceManager.GetObject("share_128px", resourceCulture));

        internal static Bitmap share_128px_1201342 =>
            ((Bitmap) ResourceManager.GetObject("share_128px_1201342", resourceCulture));

        internal static Bitmap share_16px =>
            ((Bitmap) ResourceManager.GetObject("share_16px", resourceCulture));

        internal static Bitmap share_16px_1201342 =>
            ((Bitmap) ResourceManager.GetObject("share_16px_1201342", resourceCulture));

        internal static Bitmap share_32px =>
            ((Bitmap) ResourceManager.GetObject("share_32px", resourceCulture));

        internal static Bitmap share_32px_1201342 =>
            ((Bitmap) ResourceManager.GetObject("share_32px_1201342", resourceCulture));
    }
}

