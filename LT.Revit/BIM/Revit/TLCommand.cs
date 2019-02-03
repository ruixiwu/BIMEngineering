using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
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

namespace BIM.Revit
{
    [Transaction(TransactionMode.Manual)]
    internal class TLCommand : IExternalCommand
    {
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

        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            new FormLogin {StartPosition = FormStartPosition.CenterScreen}.ShowDialog();
            if (string.IsNullOrEmpty(TableHelp.sUser))
            {
                return Result.Failed;
            }
            new FormModel(TableHelp.sUser) {StartPosition = FormStartPosition.CenterScreen}.ShowDialog();
            if (string.IsNullOrEmpty(TableHelp.g_modelTypeNo))
            {
                MessageBox.Show("请输入模型信息！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                return Result.Failed;
            }
            TableHelp.g_bFree = false;
            var activeUIDocument = commandData.Application.ActiveUIDocument;
            var document = activeUIDocument.Document;
            var str = "";
            AppConfigManager.Load();
            var succeeded = Result.Succeeded;
            try
            {
                Func<UIView, bool> predicate = null;
                Log("Start", "转换输出开始...");
                var view3D = GetTargetView(document);
                if (view3D == null)
                {
                    message = "无法找到待转换的 3D 视图!";
                    succeeded = Result.Failed;
                }
                else
                {
                    if (!document.IsModifiable && !document.IsReadOnly && (activeUIDocument.ActiveView != view3D))
                    {
                        activeUIDocument.ActiveView = view3D;
                    }
                    if (predicate == null)
                    {
                        predicate = x => x.ViewId == view3D.Id;
                    }
                    var view = activeUIDocument.GetOpenUIViews().First(predicate);
                    var zoomCorners = view.GetZoomCorners();
                    var xyz = zoomCorners[0];
                    var xyz2 = zoomCorners[1];
                    var xyz3 = xyz2 - xyz;
                    var xyz4 = xyz + 0.5*xyz3;
                    xyz3 = xyz3*0.45;
                    xyz = xyz4 - xyz3;
                    xyz2 = xyz4 + xyz3;
                    view.ZoomAndCenterRectangle(xyz, xyz2);
                    Application.DoEvents();
                    TableHelp.InitConfig();
                    TableHelp._sUrlRoot = TableHelp._configDic["urlroot"];
                    try
                    {
                        var sRetData = "Prefixion";
                        TableHelp.exeRequest("projectprefix/create", "GET", "", ref sRetData, false);
                        TableHelp._sProjectPrefix = sRetData;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("网络访问错误！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    }
                    str = SelectFilePath(document);//打开的文件的路径
                    SelectFilePathDB(@"C:\temp");
                    if (string.IsNullOrEmpty(str))
                    {
                        message = "无法获取输出文件位置!";
                        succeeded = Result.Cancelled;
                    }
                    else
                    {
                        Log("Info", "导出目标:" + str);
                        StartExport(view3D, str, null);
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
                        Log("End", "Failed", "转换输出结束...失败 " + message);
                        break;

                    case Result.Succeeded:
                        Log("End", "Succeeded", "转换输出结束...成功");
                        break;

                    case Result.Cancelled:
                        Log("End", "Cancelled", "转换输出结束...取消 " + message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            try
            {
                var stopwatch = Stopwatch.StartNew();
                while (stopwatch.ElapsedMilliseconds <= 0x7d0L)
                {
                    Application.DoEvents();
                }
            }
            catch (Exception exception2)
            {
                Log("Log", "关闭 Revit 时发生异常:" + exception2);
            }
            TableHelp.g_bFree = true;
            return Result.Succeeded;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Newtonsoft"))
            {
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "Newtonsoft.Json.dll");
                if (File.Exists(path))
                {
                    return Assembly.LoadFrom(path);
                }
            }
            else if (args.Name.Contains("Ionic"))
            {
                var str2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "Ionic.Zip.dll");
                if (File.Exists(str2))
                {
                    return Assembly.LoadFrom(str2);
                }
            }
            return null;
        }

        internal static View3D GetTargetView(Document document)
        {
            var collector = new FilteredElementCollector(document);
            var source = (from x in collector.OfClass(typeof (View3D))
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

        //internal static View3D GetTargetView(Document document)
        //{
        //    View3D viewd;
        //    FilteredElementCollector collector = new FilteredElementCollector(document);
        //    List<View3D> source = (from x in collector.OfClass(typeof(View3D))
        //        where (x is View3D) && !(x as View3D).IsTemplate
        //        select x as View3D).ToList<View3D>();
        //    if (CS$<>9__CachedAnonymousMethodDelegatea == null)
        //    {
        //        CS$<>9__CachedAnonymousMethodDelegatea = x => x.Name == "{3D}";
        //    }
        //    if (source.FirstOrDefault<View3D>(CS$<>9__CachedAnonymousMethodDelegatea))
        //    {
        //        viewd = source.FirstOrDefault<View3D>(CS$<>9__CachedAnonymousMethodDelegatea);
        //    }
        //    else
        //    {
        //        viewd = source.FirstOrDefault<View3D>(x => (x.Name == "{三维}")) ?? ((document.ActiveView as View3D) ?? source.FirstOrDefault<View3D>());
        //    }
        //    return viewd;
        //}

        private void Log(params string[] items)
        {
            Router.Instance.SendMessage(new MessageObj("LmvExport", items));
        }

        private void LogEx(string s1, string s2, string s3)
        {
            Log(s1, s2, s3);
        }

        private string SelectFilePath(Document document)
        {
            if (string.IsNullOrEmpty(document.PathName))
            {
                return null;
            }
            var path = Path.Combine(Path.GetDirectoryName(document.PathName),
                Path.GetFileNameWithoutExtension(document.PathName));
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
            var str = filePath;
            var path = str + @"\database.db";
            TableHelp._sPathData2 = str + @"\database2.db";
            TableHelp.delFile(TableHelp._sPathData2);
            TableHelp._sPathData = path;
            if (File.Exists(path))
            {
                var str3 = "";
                for (var i = 1; i < 100; i++)
                {
                    str3 = str + @"\database" + i + ".db";
                    if (!File.Exists(str3))
                    {
                        break;
                    }
                }
                File.Move(path, str3);
            }
            var str4 = AppDomain.CurrentDomain.BaseDirectory + @"\sqlite3RW.py";
            if (!File.Exists(str4))
            {
                MessageBox.Show(str4 + ":配置文件不存在!");
                return null;
            }
            var lpParameters = TableHelp._sProjectPrefix + "," + path;
            ShellExecute(IntPtr.Zero, "open", str4, lpParameters, "", ShowCommands.SW_HIDE);
            return str;
        }

        [DllImport("shell32.dll")]
        public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters,
            string lpDirectory, ShowCommands nShowCmd);

        private void StartExport(View3D view, string filePath, Action<string, string, string> trace = null)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            var document = view.Document;
            var context = new ExportContext(view, document, filePath, ExportTarget.LocalFolder, null, true,
                true, LogEx, null);
            new CustomExporter(document, context)
            {
                IncludeGeometricObjects = false,
                ShouldStopOnError = false
            }.Export(view);
            Thread.Sleep(0x3e8);
            var sZipath = @"C:\temp";
            TableHelp._sZipPath = ZipCY.ZipDir(filePath, sZipath, Path.GetFileNameWithoutExtension(document.PathName));
            var context2 = new TLExportContext(view, document, filePath, ExportTarget.LocalFolder, null,
                true, true, LogEx, null);
            new CustomExporter(document, context2)
            {
                IncludeGeometricObjects = false,
                ShouldStopOnError = false
            }.Export(view);
        }
    }
}