namespace BIM.Lmv.Revit.Core
{
    using Autodesk.Revit.DB;
    using BIM.Lmv.Revit.Helpers;
    using BIM.Lmv.Types;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class TLExportContext : IExportContext
    {
        private bool _bInstanceEnd;
        private bool _bReallyInstance;
        private bool _Cancelled;
        private Autodesk.Revit.DB.CameraInfo _CurrentCameraInfo;
        private Element _CurrentElement;
        private View3D _CurrentView;
        private Document _Document;
        private readonly Stack<Document> _DocumentStack = new Stack<Document>();
        private bool _ElementHasGeometry;
        private readonly Dictionary<int, bool> _ElementIds;
        private readonly bool _IncludeProperty;
        private readonly bool _IncludeTexture;
        private bool _IsElementSkiped;
        private readonly Stream _OutputStream;
        private readonly ExportTarget _Target;
        private TLGeometryHelper _TLGeometryHelper;
        private TLMaterialHelper _TLMaterialHelper;
        private TLPropertyHelper _TLPropHelper;
        private TableHelp _TLTableHelper;
        private readonly Action<string, string, string> _Trace;
        private bool _TraceElementInvokeSequence;
        private Transform _Transform;
        private Stack<Transform> _TransformStack = new Stack<Transform>();
        public View3D _View;

        public TLExportContext(View3D view, Document document, string targetPath, ExportTarget target = 0, Stream outputStream = null, bool includeTexture = true, bool includeProperty = true, Action<string, string, string> trace = null, Dictionary<int, bool> elementIds = null)
        {
            TableHelp._sDirPath = targetPath;
            this._View = view;
            this._Document = document;
            this._Cancelled = false;
            this._Target = target;
            this._OutputStream = outputStream;
            this._IncludeTexture = includeTexture;
            this._IncludeProperty = includeProperty;
            this._Trace = trace;
            this._ElementIds = elementIds;
            this._TraceElementInvokeSequence = false;
            this._Transform = Transform.Identity;
        }

        void IExportContext.Finish()
        {
            try
            {
                this.Finish();
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "Finish", exception.ToString());
                }
                throw;
            }
        }

        bool IExportContext.IsCanceled()
        {
            bool flag;
            try
            {
                flag = this.IsCanceled();
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "IsCanceled", exception.ToString());
                }
                throw;
            }
            return flag;
        }

        void IExportContext.OnDaylightPortal(DaylightPortalNode node)
        {
            try
            {
                this.OnDaylightPortal(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnDaylightPortal", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnElementBegin(ElementId elementId)
        {
            RenderNodeAction action;
            try
            {
                action = this.OnElementBegin(elementId);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnElementBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnElementEnd(ElementId elementId)
        {
            try
            {
                this.OnElementEnd(elementId);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnElementEnd", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnFaceBegin(FaceNode node)
        {
            RenderNodeAction action;
            try
            {
                action = this.OnFaceBegin(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnFaceBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnFaceEnd(FaceNode node)
        {
            try
            {
                this.OnFaceEnd(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnFaceEnd", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnInstanceBegin(InstanceNode node)
        {
            RenderNodeAction action;
            try
            {
                action = this.OnInstanceBegin(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnInstanceBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnInstanceEnd(InstanceNode node)
        {
            try
            {
                this.OnInstanceEnd(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnInstanceEnd", exception.ToString());
                }
                throw;
            }
        }

        void IExportContext.OnLight(LightNode node)
        {
            try
            {
                this.OnLight(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnLight", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnLinkBegin(LinkNode node)
        {
            RenderNodeAction action;
            try
            {
                action = this.OnLinkBegin(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnLinkBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnLinkEnd(LinkNode node)
        {
            try
            {
                this.OnLinkEnd(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnLinkEnd", exception.ToString());
                }
                throw;
            }
        }

        void IExportContext.OnMaterial(MaterialNode node)
        {
            try
            {
                this.OnMaterial(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnMaterial", exception.ToString());
                }
                throw;
            }
        }

        void IExportContext.OnPolymesh(PolymeshTopology node)
        {
            try
            {
                this.OnPolymesh(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnPolymesh", exception.ToString());
                }
                throw;
            }
        }

        void IExportContext.OnRPC(RPCNode node)
        {
            try
            {
                this.OnRPC(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnRPC", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnViewBegin(ViewNode node)
        {
            RenderNodeAction action;
            try
            {
                action = this.OnViewBegin(node);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnViewBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnViewEnd(ElementId elementId)
        {
            try
            {
                this.OnViewEnd(elementId);
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "OnViewEnd", exception.ToString());
                }
                throw;
            }
        }

        bool IExportContext.Start()
        {
            bool flag;
            try
            {
                flag = this.Start();
            }
            catch (Exception exception)
            {
                if (this._Trace != null)
                {
                    this._Trace("Exception", "Start", exception.ToString());
                }
                throw;
            }
            return flag;
        }

        private void Finish()
        {
            this._TLGeometryHelper.OnFinish();
            this._TLTableHelper.Finish();
        }

        private bool IsCanceled() => 
            this._Cancelled;

        private void OnDaylightPortal(DaylightPortalNode node)
        {
        }

        private RenderNodeAction OnElementBegin(ElementId elementId)
        {
            this._TLTableHelper.ElementBegin();
            this._ElementHasGeometry = false;
            this._IsElementSkiped = false;
            this._bReallyInstance = false;
            this._bInstanceEnd = true;
            Element element = this._CurrentElement = this._Document.GetElement(elementId);
            int integerValue = elementId.IntegerValue;
            if ((element.Category != null) && (element.Category.Id.IntegerValue == -2000500))
            {
                this._IsElementSkiped = true;
                return RenderNodeAction.Skip;
            }
            if ((this._ElementIds != null) && !this._ElementIds.ContainsKey(integerValue))
            {
                this._IsElementSkiped = true;
                return RenderNodeAction.Skip;
            }
            bool flag1 = this._TraceElementInvokeSequence;
            this._TLPropHelper.OnElement(element);
            return RenderNodeAction.Proceed;
        }

        private void OnElementEnd(ElementId elementId)
        {
            this._TLGeometryHelper.OnEndElement();
            this._TLTableHelper.ElementEnd(this._ElementHasGeometry);
            string str = this._TLTableHelper.ElemWriteTable(this._CurrentElement, this._Document, this._Transform, this._bReallyInstance);
            if (!string.IsNullOrEmpty(str))
            {
                this._TLPropHelper.OnWiteTable(str);
            }
            bool flag1 = this._TraceElementInvokeSequence;
            if (this._TraceElementInvokeSequence)
            {
                this._TraceElementInvokeSequence = false;
            }
            this._ElementHasGeometry = false;
            Application.DoEvents();
        }

        private RenderNodeAction OnFaceBegin(FaceNode node)
        {
            bool flag1 = this._TraceElementInvokeSequence;
            return RenderNodeAction.Proceed;
        }

        private void OnFaceEnd(FaceNode node)
        {
            bool flag1 = this._TraceElementInvokeSequence;
        }

        private RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            RenderNodeAction skip;
            this._bReallyInstance = true;
            this._bInstanceEnd = false;
            this._TransformStack.Push(this._Transform);
            this._Transform = this._Transform.Multiply(node.GetTransform());
            if (this._TLTableHelper.InstanceBegin(this._Transform, node.GetSymbolId().IntegerValue))
            {
                skip = RenderNodeAction.Skip;
                this._ElementHasGeometry = true;
            }
            else
            {
                skip = RenderNodeAction.Proceed;
            }
            bool flag1 = this._TraceElementInvokeSequence;
            return skip;
        }

        private void OnInstanceEnd(InstanceNode node)
        {
            this._bInstanceEnd = true;
            bool flag1 = this._TraceElementInvokeSequence;
            this._TLTableHelper.InstanceEnd(this._Transform, node.GetSymbolId().IntegerValue);
            this._Transform = this._TransformStack.Pop();
        }

        private void OnLight(LightNode node)
        {
            bool flag1 = this._TraceElementInvokeSequence;
        }

        private RenderNodeAction OnLinkBegin(LinkNode node)
        {
            this._DocumentStack.Push(this._Document);
            this._Document = node.GetDocument();
            this._TransformStack.Push(this._Transform);
            this._Transform = this._Transform.Multiply(node.GetTransform());
            return RenderNodeAction.Proceed;
        }

        private void OnLinkEnd(LinkNode node)
        {
            this._Document = this._DocumentStack.Pop();
            this._Transform = this._TransformStack.Pop();
        }

        private void OnMaterial(MaterialNode node)
        {
            bool flag1 = this._TraceElementInvokeSequence;
            int nMaterialId = this._TLMaterialHelper.OnMaterial(this._Document, node);
            this._TLGeometryHelper.OnPrePolyMatrial(nMaterialId);
        }

        private void OnPolymesh(PolymeshTopology node)
        {
            this._ElementHasGeometry = true;
            if (this._bReallyInstance && this._bInstanceEnd)
            {
                this._bReallyInstance = false;
            }
            bool flag1 = this._TraceElementInvokeSequence;
            this._TLGeometryHelper.OnPolymesh(node);
        }

        private void OnRPC(RPCNode node)
        {
            bool flag1 = this._TraceElementInvokeSequence;
            this._TLMaterialHelper.OnMaterialForRPC();
            this._TLGeometryHelper.OnRPC(this._CurrentElement);
        }

        private RenderNodeAction OnViewBegin(ViewNode node)
        {
            this._CurrentView = this._Document.GetElement(node.ViewId) as View3D;
            this._CurrentCameraInfo = node.GetCameraInfo();
            return RenderNodeAction.Proceed;
        }

        private void OnViewEnd(ElementId elementId)
        {
            this._CurrentView = null;
            this._CurrentCameraInfo = null;
        }

        private bool Start()
        {
            ExportHelper.GetSceneInfo(this._Document);
            ExportOption option = new ExportOption {
                Target = this._Target,
                OutputStream = this._OutputStream
            };
            this._TLTableHelper = new TableHelp(this._Document);
            this._TLPropHelper = new TLPropertyHelper();
            this._TLMaterialHelper = new TLMaterialHelper(this._Target, this._IncludeTexture, this._View);
            this._TLGeometryHelper = new TLGeometryHelper();
            this._TLTableHelper.AddProject(this._Document.PathName);
            return true;
        }
    }
}

