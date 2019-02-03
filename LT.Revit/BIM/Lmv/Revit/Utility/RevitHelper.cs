using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using BIM.Lmv.Revit.Batch;

namespace BIM.Lmv.Revit.Utility
{
    internal static class RevitHelper
    {
        public static void Exit()
        {
            WinApi.SendMessage(GetRevitHandle(), 0x10, 0, 0);
        }

        public static AutomationElement GetAdAppButton()
        {
            var revitWindow = GetRevitWindow();
            if (revitWindow == null)
            {
                return null;
            }
            var condition = new PropertyCondition(AutomationElement.NameProperty, "AdApplicationButton");
            return revitWindow.FindFirst(TreeScope.Children, condition);
        }

        private static IntPtr GetRevitHandle()
        {
            var currentPropertyValue =
                (int) GetRevitWindow().GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);
            return (IntPtr) currentPropertyValue;
        }

        public static AutomationElement GetRevitWindow()
        {
            var condition = new PropertyCondition(AutomationElement.IsContentElementProperty, true);
            var condition2 = new PropertyCondition(AutomationElement.ProcessIdProperty,
                Process.GetCurrentProcess().Id);
            foreach (
                AutomationElement element2 in
                    AutomationElement.RootElement.FindAll(TreeScope.Children,
                        new AndCondition(condition2, condition)))
            {
                if (
                    element2.GetCurrentPropertyValue(AutomationElement.NameProperty)
                        .ToString()
                        .StartsWith("Autodesk Revit "))
                {
                    return element2;
                }
            }
            return null;
        }

        public static bool OpenDefault3DView(Action<MessageObj> log)
        {
            var revitWindow = GetRevitWindow();
            if (revitWindow == null)
            {
                log(new MessageObj("UIB", "Info", "获取 RevitWindow 失败!"));
                return false;
            }
            var element2 = revitWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty,
                    "ID_VIEW_DEFAULT_3DVIEW_FlyoutButtonExecute"));
            if (element2 == null)
            {
                log(new MessageObj("UIB", "Info", "获取 Default3DViewButton 失败!"));
                return false;
            }
            var currentPattern = element2.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (currentPattern == null)
            {
                log(new MessageObj("UIB", "Info", "获取 Default3DViewButton.Pattern 失败!"));
                return false;
            }
            currentPattern.Invoke();
            return true;
        }

        public static bool StartExport(string panelName, string buttonName)
        {
            var revitWindow = GetRevitWindow();
            if (revitWindow == null)
            {
                return false;
            }
            var element2 = revitWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, "RibbonHostWindow"));
            if (element2 == null)
            {
                return false;
            }
            var element3 = element2.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "Add-Ins"));
            if (element3 == null)
            {
                return false;
            }
            var currentPattern = element3.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (currentPattern == null)
            {
                return false;
            }
            currentPattern.Invoke();
            var element4 = element2.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "Add-Ins_PanelBarScrollViewer"));
            if (element4 == null)
            {
                return false;
            }
            var num = 0;
            while (element4.Current.IsOffscreen)
            {
                num++;
                Thread.Sleep(100);
                if (num > 100)
                {
                    return false;
                }
            }
            Thread.Sleep(200);
            var element5 = element4.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "CustomCtrl_%Add-Ins%" + panelName));
            if (element5 == null)
            {
                return false;
            }
            var element6 = element5.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty,
                    "CustomCtrl_%CustomCtrl_%Add-Ins%" + panelName + "%" + buttonName));
            if (element6 == null)
            {
                return false;
            }
            var pattern2 = element6.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if (pattern2 == null)
            {
                return false;
            }
            pattern2.Invoke();
            return true;
        }
    }
}