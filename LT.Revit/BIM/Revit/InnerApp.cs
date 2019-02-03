using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using BIM.Lmv.Revit.Batch;
using BIM.Lmv.Revit.Utility;

namespace BIM.Revit
{
    internal class InnerApp : IExternalApplication
    {
        private const string BATCH_EXPORT_BUTTON_NAME = "BIM_Lmv_Export_Command";
        public const string TITLE = "轻量化模型";
        private readonly RevitDialogResponse _DialogHandler = new RevitDialogResponse();
        private bool _ApplicationInitialized;
        private bool _DocumentOpened;
        private bool _IsStartedExport;
        private bool _SwitchToDefault3DView;

        public InnerApp()
        {
            _DialogHandler.AddOverride(RevitDialogType.DefaultFamilyTemplateInvalid, 8);
            _DialogHandler.AddOverride(RevitDialogType.LostOnImport, 2);
            _DialogHandler.AddOverride(RevitDialogType.UnresolvedReferences, 0x3ea);
            _DialogHandler.AddOverride(RevitDialogType.SaveFile, 7);
            _DialogHandler.AddOverride(RevitDialogType.TaskDialog_Model_Opened_By_Another_User, 0x3e9);
            _DialogHandler.AddOverride(RevitDialogType.TaskDialog_Command_Failure_For_Extenal_Command, 1);
            _DialogHandler.AddOverride(RevitDialogType.TaskDialog_Rendering_Library_Not_Installed, 8);
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            Log(new MessageObj("OnShutdown"));
            return Result.Succeeded;
        }

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            Log(new MessageObj("OnStartup"));
            application.ApplicationClosing +=UIControlledApplication_ApplicationClosing;
            application.DialogBoxShowing +=UIControlledApplication_DialogBoxShowing;
            application.DisplayingOptionsDialog +=UIControlledApplication_DisplayingOptionsDialog;
            application.DockableFrameFocusChanged +=UIControlledApplication_DockableFrameFocusChanged;
            application.DockableFrameVisibilityChanged +=UIControlledApplication_DockableFrameVisibilityChanged;
            application.Idling += UIControlledApplication_Idling;
            application.ViewActivating +=UIControlledApplication_ViewActivating;
            application.ViewActivated +=UIControlledApplication_ViewActivated;
            application.ControlledApplication.ApplicationInitialized +=ControlledApplication_ApplicationInitialized;
            application.ControlledApplication.DocumentChanged +=ControlledApplication_DocumentChanged;
            application.ControlledApplication.DocumentClosed +=ControlledApplication_DocumentClosed;
            application.ControlledApplication.DocumentClosing +=ControlledApplication_DocumentClosing;
            application.ControlledApplication.DocumentCreated +=ControlledApplication_DocumentCreated;
            application.ControlledApplication.DocumentCreating +=ControlledApplication_DocumentCreating;
            application.ControlledApplication.DocumentOpened +=ControlledApplication_DocumentOpened;
            application.ControlledApplication.DocumentOpening +=ControlledApplication_DocumentOpening;
            application.ControlledApplication.DocumentPrinted +=ControlledApplication_DocumentPrinted;
            application.ControlledApplication.DocumentPrinting +=ControlledApplication_DocumentPrinting;
            application.ControlledApplication.DocumentSaved +=ControlledApplication_DocumentSaved;
            application.ControlledApplication.DocumentSavedAs +=ControlledApplication_DocumentSavedAs;
            application.ControlledApplication.DocumentSaving +=ControlledApplication_DocumentSaving;
            application.ControlledApplication.DocumentSavingAs +=ControlledApplication_DocumentSavingAs;
            application.ControlledApplication.DocumentSynchronizedWithCentral +=ControlledApplication_DocumentSynchronizedWithCentral;
            application.ControlledApplication.DocumentSynchronizingWithCentral +=ControlledApplication_DocumentSynchronizingWithCentral;
            application.ControlledApplication.FailuresProcessing +=ControlledApplication_FailuresProcessing;
            application.ControlledApplication.ElementTypeDuplicated +=ControlledApplication_ElementTypeDuplicated;
            application.ControlledApplication.ElementTypeDuplicating +=ControlledApplication_ElementTypeDuplicating;
            application.ControlledApplication.FamilyLoadedIntoDocument +=ControlledApplication_FamilyLoadedIntoDocument;
            application.ControlledApplication.FamilyLoadingIntoDocument +=ControlledApplication_FamilyLoadingIntoDocument;
            application.ControlledApplication.FileExported +=ControlledApplication_FileExported;
            application.ControlledApplication.FileExporting +=ControlledApplication_FileExporting;
            application.ControlledApplication.FileImported +=ControlledApplication_FileImported;
            application.ControlledApplication.FileImporting +=ControlledApplication_FileImporting;
            application.ControlledApplication.ProgressChanged +=ControlledApplication_ProgressChanged;
            application.ControlledApplication.ViewPrinted +=ControlledApplication_ViewPrinted;
            application.ControlledApplication.ViewPrinting +=ControlledApplication_ViewPrinting;
            var location = Assembly.GetExecutingAssembly().Location;
            var panel = application.CreateRibbonPanel("轻量化模型");
            if (Router.Instance.IsRunning())
            {
                var itemData = new PushButtonData("BIM_Lmv_Export_Command", "导出模型", location,
                    "BIM.Lmv.Revit.CommandBatch")
                {
                    ToolTip = "自动对当前模型执行轻量化导出操作。"
                };
                panel.AddItem(itemData);
            }
            return Result.Succeeded;
        }

        private void ControlledApplication_ApplicationInitialized(object sender, ApplicationInitializedEventArgs e)
        {
            WaitCallback callBack = null;
            if (_DocumentOpened)
            {
                _ApplicationInitialized = true;
            }
            Log(new MessageObj("ApplicationInitialized"));
            if (Router.Instance.IsRunning() && _SwitchToDefault3DView)
            {
                Log(new MessageObj("UIB", "Start", "自动切换到默认视图..."));
                if (callBack == null)
                {
                    callBack = delegate
                    {
                        if (RevitHelper.OpenDefault3DView(Log))
                        {
                            Log(new MessageObj("UIB", "Success", "自动切换到默认视图...成功!"));
                        }
                        else
                        {
                            Log(new MessageObj("UIB", "Fail", "自动切换到默认视图...失败!!!"));
                        }
                    };
                }
                ThreadPool.QueueUserWorkItem(callBack);
            }
        }

        private void ControlledApplication_DocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            Log(new MessageObj("DocumentChanged"));
        }

        private void ControlledApplication_DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            Log(new MessageObj("DocumentClosed"));
        }

        private void ControlledApplication_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            Log(new MessageObj("DocumentClosing"));
        }

        private void ControlledApplication_DocumentCreated(object sender, DocumentCreatedEventArgs e)
        {
            Log(new MessageObj("DocumentCreated"));
        }

        private void ControlledApplication_DocumentCreating(object sender, DocumentCreatingEventArgs e)
        {
            Log(new MessageObj("DocumentCreating"));
        }

        private void ControlledApplication_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            _DocumentOpened = true;
            Log(new MessageObj("DocumentOpened", e.Document.PathName));
            if (Router.Instance.IsRunning())
            {
                var activeView = e.Document.ActiveView as View3D;
                if ((activeView != null) && activeView.IsPerspective)
                {
                    _SwitchToDefault3DView = true;
                }
            }
        }

        private void ControlledApplication_DocumentOpening(object sender, DocumentOpeningEventArgs e)
        {
            Log(new MessageObj("DocumentOpening", e.PathName));
        }

        private void ControlledApplication_DocumentPrinted(object sender, DocumentPrintedEventArgs e)
        {
            Log(new MessageObj("DocumentPrinted"));
        }

        private void ControlledApplication_DocumentPrinting(object sender, DocumentPrintingEventArgs e)
        {
            Log(new MessageObj("DocumentPrinting"));
        }

        private void ControlledApplication_DocumentSaved(object sender, DocumentSavedEventArgs e)
        {
            Log(new MessageObj("DocumentSaved"));
        }

        private void ControlledApplication_DocumentSavedAs(object sender, DocumentSavedAsEventArgs e)
        {
            Log(new MessageObj("DocumentSavedAs"));
        }

        private void ControlledApplication_DocumentSaving(object sender, DocumentSavingEventArgs e)
        {
            Log(new MessageObj("DocumentSaving"));
        }

        private void ControlledApplication_DocumentSavingAs(object sender, DocumentSavingAsEventArgs e)
        {
            Log(new MessageObj("DocumentSavingAs"));
        }

        private void ControlledApplication_DocumentSynchronizedWithCentral(object sender,
            DocumentSynchronizedWithCentralEventArgs e)
        {
            Log(new MessageObj("DocumentSynchronizedWithCentral"));
        }

        private void ControlledApplication_DocumentSynchronizingWithCentral(object sender,
            DocumentSynchronizingWithCentralEventArgs e)
        {
            Log(new MessageObj("DocumentSynchronizingWithCentral"));
        }

        private void ControlledApplication_ElementTypeDuplicated(object sender, ElementTypeDuplicatedEventArgs e)
        {
            Log(new MessageObj("ElementTypeDuplicated"));
        }

        private void ControlledApplication_ElementTypeDuplicating(object sender, ElementTypeDuplicatingEventArgs e)
        {
            Log(new MessageObj("ElementTypeDuplicating"));
        }

        private void ControlledApplication_FailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
        }

        private void ControlledApplication_FamilyLoadedIntoDocument(object sender, FamilyLoadedIntoDocumentEventArgs e)
        {
            Log(new MessageObj("FamilyLoadedIntoDocument"));
        }

        private void ControlledApplication_FamilyLoadingIntoDocument(object sender, FamilyLoadingIntoDocumentEventArgs e)
        {
            Log(new MessageObj("FamilyLoadingIntoDocument"));
        }

        private void ControlledApplication_FileExported(object sender, FileExportedEventArgs e)
        {
            Log(new MessageObj("FileExported"));
        }

        private void ControlledApplication_FileExporting(object sender, FileExportingEventArgs e)
        {
            Log(new MessageObj("FileExporting"));
        }

        private void ControlledApplication_FileImported(object sender, FileImportedEventArgs e)
        {
            Log(new MessageObj("FileImported"));
        }

        private void ControlledApplication_FileImporting(object sender, FileImportingEventArgs e)
        {
            Log(new MessageObj("FileImporting"));
        }

        private void ControlledApplication_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.Caption.StartsWith("打印") || e.Caption.StartsWith("Print"))
            {
                Log(new MessageObj("ProgressChanged", e.Caption, e.Stage.ToString(), e.LowerRange.ToString(),
                    e.UpperRange.ToString(), e.Position.ToString()));
            }
        }

        private void ControlledApplication_ViewPrinted(object sender, ViewPrintedEventArgs e)
        {
            Log(new MessageObj("ViewPrinted"));
        }

        private void ControlledApplication_ViewPrinting(object sender, ViewPrintingEventArgs e)
        {
            Log(new MessageObj("ViewPrinting"));
        }

        private ImageSource GetImageSource(Bitmap img)
        {
            var stream = new MemoryStream();
            img.Save(stream, ImageFormat.Png);
            var converter = new ImageSourceConverter();
            return (ImageSource) converter.ConvertFrom(stream);
        }

        private void Log(MessageObj msg)
        {
            Router.Instance.SendMessage(msg);
        }

        private void UIControlledApplication_ApplicationClosing(object sender, ApplicationClosingEventArgs e)
        {
            Log(new MessageObj("ApplicationClosing"));
        }

        private void UIControlledApplication_DialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
        {
            var args = e as TaskDialogShowingEventArgs;
            if (args == null)
            {
                var args2 = e as MessageBoxShowingEventArgs;
                if (args2 == null)
                {
                    Log(new MessageObj("DialogBoxShowing", e.HelpId.ToString(), e.GetType().ToString(),
                        e.Cancellable.ToString()));
                }
                else
                {
                    if (args2.DialogType == 0)
                    {
                        e.OverrideResult(2);
                    }
                    Log(new MessageObj("DialogBoxShowing", e.HelpId.ToString(), e.GetType().ToString(),
                        e.Cancellable.ToString(), args2.DialogType.ToString(), args2.Message));
                }
            }
            else
            {
                Log(new MessageObj("DialogBoxShowing", e.HelpId.ToString(), e.GetType().ToString(),
                    e.Cancellable.ToString(), args.DialogId, args.Message));
            }
            if ((args != null) && Router.Instance.IsRunning() && _DialogHandler.HasOverride(args.DialogId))
            {
                args.OverrideResult(_DialogHandler.GetOverride(args.DialogId));
            }
        }

        private void UIControlledApplication_DisplayingOptionsDialog(object sender, DisplayingOptionsDialogEventArgs e)
        {
            Log(new MessageObj("DisplayingOptionsDialog"));
        }

        private void UIControlledApplication_DockableFrameFocusChanged(object sender,
            DockableFrameFocusChangedEventArgs e)
        {
            Log(new MessageObj("DockableFrameFocusChanged"));
        }

        private void UIControlledApplication_DockableFrameVisibilityChanged(object sender,
            DockableFrameVisibilityChangedEventArgs e)
        {
            Log(new MessageObj("DockableFrameVisibilityChanged"));
        }

        private void UIControlledApplication_Idling(object sender, IdlingEventArgs e)
        {
            WaitCallback callBack = null;
            if (!_IsStartedExport && _ApplicationInitialized && _DocumentOpened)
            {
                _IsStartedExport = true;
                if (Router.Instance.IsRunning())
                {
                    Log(new MessageObj("UIA", "Start", "自动启动导出插件..."));
                    if (callBack == null)
                    {
                        callBack = delegate
                        {
                            if (RevitHelper.StartExport("轻量化模型", "BIM_Lmv_Export_Command"))
                            {
                                Log(new MessageObj("UIA", "Success", "自动启动导出插件...成功!"));
                            }
                            else
                            {
                                Log(new MessageObj("UIA", "Fail", "自动启动导出插件...失败!!!"));
                            }
                        };
                    }
                    ThreadPool.QueueUserWorkItem(callBack);
                }
            }
        }

        private void UIControlledApplication_ViewActivated(object sender, ViewActivatedEventArgs e)
        {
            Log(new MessageObj("ViewActivated"));
        }

        private void UIControlledApplication_ViewActivating(object sender, ViewActivatingEventArgs e)
        {
            Log(new MessageObj("ViewActivating"));
        }
    }
}