namespace BIM.Lmv.Revit.Core
{
    using Autodesk.Revit.DB;
    using BIM.Lmv.Revit.Config;
    using BIM.Lmv.Types;
    using System;
    using System.IO;

    public static class ExportUtility
    {
        public static void Export(View3D view, bool includeTexture, bool includeProperty, ExportTarget target, string targetPath, Stream targetStream)
        {
            AppConfigManager.Load();
            Document document = view.Document;
            ExportContext context = new ExportContext(view, document, targetPath, target, targetStream, includeTexture, includeProperty, null, null);
            new CustomExporter(document, context) { 
                IncludeGeometricObjects = false,
                ShouldStopOnError = false
            }.Export(view);
        }
    }
}

