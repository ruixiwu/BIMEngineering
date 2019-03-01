namespace BIM.Revit
{
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using BIM.Lmv.Revit.Batch;
    using BIM.Lmv.Revit.Config;
    using BIM.Lmv.Revit.Core;
    using BIM.Lmv.Revit.Helpers;
    using BIM.Lmv.Revit.UI;
    using BIM.Lmv.Types;
    using SQlite.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using Utils;

    [Transaction(TransactionMode.Manual)]
    internal class TLCommand : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string directoryName = Path.GetDirectoryName(base.GetType().Assembly.Location);
            if (!TableHelp.InitConfig(Path.Combine(directoryName, @"config\config.txt")))
            {
                return Result.Failed;
            }
            string path = directoryName.Substring(0, directoryName.LastIndexOf('\\') + 1) + "tempcache";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (TableHelp._configDic.ContainsKey("urlangkun"))
            {
                WebServiceClient.HTTPHEADER = TableHelp._configDic["urlangkun"];
            }
            new FormLogin { StartPosition = FormStartPosition.CenterScreen }.ShowDialog();
            if (string.IsNullOrEmpty(TableHelp.sUser))
            {
                return Result.Failed;
            }
            new FormModel(TableHelp.sUser) { StartPosition = FormStartPosition.CenterScreen }.ShowDialog();
            if (string.IsNullOrEmpty(TableHelp.g_workspaceId))
            {
                MessageBox.Show("请输入模型信息！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                return Result.Failed;
            }
            TableHelp.g_bFree = false;
            UIDocument activeUIDocument = commandData.Application.ActiveUIDocument;
            Document document = activeUIDocument.Document;
            string str5 = "";
            AppConfigManager.Load();
            Result succeeded = Result.Succeeded;
            try
            {
                Func<UIView, bool> predicate = null;
                this.Log(new string[] { "Start", "转换输出开始..." });
                View3D view3d = GetTargetView(document);
                if (view3d == null)
                {
                    message = "无法找到待转换的 3D 视图!";
                    succeeded = Result.Failed;
                }
                else
                {
                    if ((!document.IsModifiable && !document.IsReadOnly) && (activeUIDocument.ActiveView != view3d))
                    {
                        activeUIDocument.ActiveView = view3d;
                    }
                    if (predicate == null)
                    {
                        predicate = x => x.ViewId == view3d.Id;
                    }
                    UIView view = activeUIDocument.GetOpenUIViews().First<UIView>(predicate);
                    IList<XYZ> zoomCorners = view.GetZoomCorners();
                    XYZ xyz = zoomCorners[0];
                    XYZ xyz2 = zoomCorners[1];
                    XYZ xyz3 = xyz2 - xyz;
                    XYZ xyz4 = xyz + ((XYZ) (0.5 * xyz3));
                    xyz3 = (XYZ) (xyz3 * 0.45);
                    xyz = xyz4 - xyz3;
                    xyz2 = xyz4 + xyz3;
                    view.ZoomAndCenterRectangle(xyz, xyz2);
                    Application.DoEvents();
                    if (TableHelp._configDic.ContainsKey("urlroot"))
                    {
                        TableHelp._sUrlRoot = TableHelp._configDic["urlroot"];
                    }
                    try
                    {
                        string sRetData = "Prefixion";
                        TableHelp.exeRequest("projectprefix/create", "GET", "", ref sRetData, false);
                        TableHelp._sProjectPrefix = sRetData;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("网络访问错误！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                        return Result.Failed;
                    }
                    str5 = this.SelectFilePath(document);
                    this.SelectFilePathDB(path);
                    if (string.IsNullOrEmpty(str5))
                    {
                        message = "无法获取输出文件位置!";
                        succeeded = Result.Cancelled;
                    }
                    else
                    {
                        this.Log(new string[] { "Info", "导出目标:" + str5 });
                        this.StartExport(view3d, str5, path, null);
                        succeeded = Result.Succeeded;
                    }
                }
            }
            catch (Exception exception)
            {
                message = exception.ToString();
                succeeded = Result.Failed;
            }
            finally
            {
                switch (succeeded)
                {
                    case Result.Failed:
                        this.Log(new string[] { "End", "Failed", "转换输出结束...失败 " + message });
                        break;

                    case Result.Succeeded:
                        this.Log(new string[] { "End", "Succeeded", "转换输出结束...成功" });
                        break;

                    case Result.Cancelled:
                        this.Log(new string[] { "End", "Cancelled", "转换输出结束...取消 " + message });
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                while (stopwatch.ElapsedMilliseconds <= 0x7d0L)
                {
                    Application.DoEvents();
                }
            }
            catch (Exception exception2)
            {
                this.Log(new string[] { "Log", "关闭 Revit 时发生异常:" + exception2 });
            }
            TableHelp.g_bFree = true;
            return Result.Succeeded;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Newtonsoft"))
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Newtonsoft.Json.dll");
                if (File.Exists(path))
                {
                    return Assembly.LoadFrom(path);
                }
            }
            else if (args.Name.Contains("Ionic"))
            {
                string str2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Ionic.Zip.dll");
                if (File.Exists(str2))
                {
                    return Assembly.LoadFrom(str2);
                }
            }
            return null;
        }

        //internal static View3D GetTargetView(Document document)
        //{
        //    View3D viewd;
        //    FilteredElementCollector collector = new FilteredElementCollector(document);
        //    List<View3D> source = (from x in collector.OfClass(typeof(View3D))
        //                           where (x is View3D) && !(x as View3D).IsTemplate
        //                           select x as View3D).ToList<View3D>();
        //    if (CS$<> 9__CachedAnonymousMethodDelegatea == null)
        //    {
        //        CS$<> 9__CachedAnonymousMethodDelegatea = x => x.Name == "{3D}";
        //    }
        //    if (source.FirstOrDefault<View3D>(CS$<> 9__CachedAnonymousMethodDelegatea))
        //    {
        //        viewd = source.FirstOrDefault<View3D>(CS$<> 9__CachedAnonymousMethodDelegatea);
        //    }
        //    else
        //    {
        //        viewd = source.FirstOrDefault<View3D>(x => (x.Name == "{三维}")) ?? ((document.ActiveView as View3D) ?? source.FirstOrDefault<View3D>());
        //    }
        //    return viewd;
        //}

        internal static View3D GetTargetView(Document document)
        {
            var collector = new FilteredElementCollector(document);
            var source = (from x in collector.OfClass(typeof(View3D))
                          where x is View3D && !(x as View3D).IsTemplate
                          select x as View3D).ToList();
            var local1 = source.FirstOrDefault(x => x.Name == "{3D}");
            if (local1 != null)
            {
                return local1;
            }
            var local2 = source.FirstOrDefault(x => x.Name == "{三维}");
            if (local2 != null)
            {
                return local2;
            }
            var activeView = document.ActiveView as View3D;
            if (activeView != null)
            {
                return activeView;
            }
            return source.FirstOrDefault();
        }

        private void Log(params string[] items)
        {
            Router.Instance.SendMessage(new MessageObj("LmvExport", items));
        }

        private void LogEx(string s1, string s2, string s3)
        {
            this.Log(new string[] { s1, s2, s3 });
        }

        private string SelectFilePath(Document document)
        {
            if (string.IsNullOrEmpty(document.PathName))
            {
                return null;
            }
            string path = Path.Combine(Path.GetDirectoryName(document.PathName), Path.GetFileNameWithoutExtension(document.PathName));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        private string SelectFilePathDB(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string str = filePath;
            string path = str + @"\database.db";
            TableHelp._sPathData2 = str + @"\database2.db";
            TableHelp.delFile(TableHelp._sPathData2);
            TableHelp._sPathData = path;
            if (File.Exists(path))
            {
                string str3 = "";
                for (int i = 1; i < 100; i++)
                {
                    str3 = str + @"\database" + i.ToString() + ".db";
                    if (!File.Exists(str3))
                    {
                        break;
                    }
                }
                File.Move(path, str3);
            }
            TableHelp.delFile(str + @"\database1.db");
            TableHelp.delFile(TableHelp._sPathData2);
            return str;
        }

        [DllImport("shell32.dll")]
        public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, ShowCommands nShowCmd);
        private void StartExport(View3D view, string filePath, string strCachePath, Action<string, string, string> trace = null)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
            Document document = view.Document;
            ExportContext context = new ExportContext(view, document, filePath, ExportTarget.LocalFolder, null, true, true, new Action<string, string, string>(this.LogEx), null);
            new CustomExporter(document, context) { 
                IncludeGeometricObjects = false,
                ShouldStopOnError = false
            }.Export(view);
            Thread.Sleep(0x3e8);
            string sZipath = strCachePath;
            TableHelp._sZipPath = ZipCY.ZipDir(filePath, sZipath, Path.GetFileNameWithoutExtension(document.PathName));
            TLExportContext context2 = new TLExportContext(view, document, filePath, ExportTarget.LocalFolder, null, true, true, new Action<string, string, string>(this.LogEx), null);
            new CustomExporter(document, context2) { 
                IncludeGeometricObjects = false,
                ShouldStopOnError = false
            }.Export(view);
        }

        public enum ShowCommands
        {
            SW_FORCEMINIMIZE = 11,
            SW_HIDE = 0,
            SW_MAX = 11,
            SW_MAXIMIZE = 3,
            SW_MINIMIZE = 6,
            SW_NORMAL = 1,
            SW_RESTORE = 9,
            SW_SHOW = 5,
            SW_SHOWDEFAULT = 10,
            SW_SHOWMAXIMIZED = 3,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOWNORMAL = 1
        }
    }
}

