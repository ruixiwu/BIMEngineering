using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using BIM.Lmv.Revit.Helpers;
using BIM.Lmv.Types;
using CameraInfo = Autodesk.Revit.DB.CameraInfo;

namespace BIM.Lmv.Revit.Core
{
    internal class TLExportContext : IExportContext
    {
        private readonly Stack<Document> _DocumentStack = new Stack<Document>();
        private readonly Dictionary<int, bool> _ElementIds;
        private readonly bool _IncludeProperty;
        private readonly bool _IncludeTexture;
        private readonly Stream _OutputStream;
        private readonly ExportTarget _Target;
        private readonly Action<string, string, string> _Trace;
        private readonly bool _Cancelled;
        private CameraInfo _CurrentCameraInfo;
        private Element _CurrentElement;
        private View3D _CurrentView;
        private Document _Document;
        private bool _ElementHasGeometry;
        private bool _IsElementSkiped;
        private TLGeometryHelper _TLGeometryHelper;
        private TLMaterialHelper _TLMaterialHelper;
        private TLPropertyHelper _TLPropHelper;
        private TableHelp _TLTableHelper;
        private bool _TraceElementInvokeSequence;
        private Transform _Transform;
        private readonly Stack<Transform> _TransformStack = new Stack<Transform>();
        public View3D _View;

        public TLExportContext(View3D view, Document document, string targetPath, ExportTarget target = 0,
            Stream outputStream = null, bool includeTexture = true, bool includeProperty = true,
            Action<string, string, string> trace = null, Dictionary<int, bool> elementIds = null)
        {
            TableHelp._sDirPath = targetPath;
            _View = view;
            _Document = document;
            _Cancelled = false;
            _Target = target;
            _OutputStream = outputStream;
            _IncludeTexture = includeTexture;
            _IncludeProperty = includeProperty;
            _Trace = trace;
            _ElementIds = elementIds;
            _TraceElementInvokeSequence = false;
            _Transform = Transform.Identity;
        }

        void IExportContext.Finish()
        {
            try
            {
                Finish();
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "Finish", exception.ToString());
                }
                throw;
            }
        }

        bool IExportContext.IsCanceled()
        {
            bool flag;
            try
            {
                flag = IsCanceled();
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "IsCanceled", exception.ToString());
                }
                throw;
            }
            return flag;
        }

        void IExportContext.OnDaylightPortal(DaylightPortalNode node)
        {
            try
            {
                OnDaylightPortal(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnDaylightPortal", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnElementBegin(ElementId elementId)
        {
            RenderNodeAction action;
            try
            {
                action = OnElementBegin(elementId);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnElementBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnElementEnd(ElementId elementId)
        {//遍历元素结束
            try
            {
                OnElementEnd(elementId);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnElementEnd", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnFaceBegin(FaceNode node)
        {
            RenderNodeAction action;
            try
            {
                action = OnFaceBegin(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnFaceBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnFaceEnd(FaceNode node)
        {
            try
            {
                OnFaceEnd(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnFaceEnd", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnInstanceBegin(InstanceNode node)
        {
            RenderNodeAction action;
            try
            {
                action = OnInstanceBegin(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnInstanceBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnInstanceEnd(InstanceNode node)
        {
            try
            {
                OnInstanceEnd(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnInstanceEnd", exception.ToString());
                }
                throw;
            }
        }

        void IExportContext.OnLight(LightNode node)
        {
            try
            {
                OnLight(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnLight", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnLinkBegin(LinkNode node)
        {
            RenderNodeAction action;
            try
            {
                action = OnLinkBegin(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnLinkBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnLinkEnd(LinkNode node)
        {
            try
            {
                OnLinkEnd(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnLinkEnd", exception.ToString());
                }
                throw;
            }
        }

        void IExportContext.OnMaterial(MaterialNode node)
        {
            try
            {
                OnMaterial(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnMaterial", exception.ToString());
                }
                throw;
            }
        }

        void IExportContext.OnPolymesh(PolymeshTopology node)
        {
            try
            {
                OnPolymesh(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnPolymesh", exception.ToString());
                }
                throw;
            }
        }

        void IExportContext.OnRPC(RPCNode node)
        {
            try
            {
                OnRPC(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnRPC", exception.ToString());
                }
                throw;
            }
        }

        RenderNodeAction IExportContext.OnViewBegin(ViewNode node)
        {
            RenderNodeAction action;
            try
            {
                action = OnViewBegin(node);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnViewBegin", exception.ToString());
                }
                throw;
            }
            return action;
        }

        void IExportContext.OnViewEnd(ElementId elementId)
        {
            try
            {
                OnViewEnd(elementId);
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "OnViewEnd", exception.ToString());
                }
                throw;
            }
        }

        bool IExportContext.Start()
        {
            bool flag;
            try
            {
                flag = Start();
            }
            catch (Exception exception)
            {
                if (_Trace != null)
                {
                    _Trace("Exception", "Start", exception.ToString());
                }
                throw;
            }
            return flag;
        }

        private void Finish()
        {
            _TLGeometryHelper.OnFinish();
            _TLTableHelper.Finish();
        }

        private bool IsCanceled()
        {
            return _Cancelled;
        }

        private void OnDaylightPortal(DaylightPortalNode node)
        {
        }

        private RenderNodeAction OnElementBegin(ElementId elementId)
        {
            _TLTableHelper.ElementBegin();
            _ElementHasGeometry = false;
            _IsElementSkiped = false;
            var element = _CurrentElement = _Document.GetElement(elementId);
            var integerValue = elementId.IntegerValue;
            if ((element.Category != null) && (element.Category.Id.IntegerValue == -2000500))
            {
                _IsElementSkiped = true;
                return RenderNodeAction.Skip;
            }
            if ((_ElementIds != null) && !_ElementIds.ContainsKey(integerValue))
            {
                _IsElementSkiped = true;
                return RenderNodeAction.Skip;
            }
            var flag1 = _TraceElementInvokeSequence;
            _TLPropHelper.OnElement(element);
            return RenderNodeAction.Proceed;
        }

        private void OnElementEnd(ElementId elementId) 
        {
            _TLGeometryHelper.OnEndElement();
            _TLTableHelper.ElementEnd(_ElementHasGeometry);
            var str = _TLTableHelper.ElemWriteTable(_CurrentElement, _Document, _Transform);
            if (!string.IsNullOrEmpty(str))
            {
                _TLPropHelper.OnWiteTable(str);
            }
            var flag1 = _TraceElementInvokeSequence;
            if (_TraceElementInvokeSequence)
            {
                _TraceElementInvokeSequence = false;
            }
            _ElementHasGeometry = false;
            Application.DoEvents();
        }

        private RenderNodeAction OnFaceBegin(FaceNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
            return RenderNodeAction.Proceed;
        }

        private void OnFaceEnd(FaceNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
        }

        private RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            RenderNodeAction skip;
            _TransformStack.Push(_Transform);
            _Transform = _Transform.Multiply(node.GetTransform());
            if (_TLTableHelper.InstanceBegin(_Transform, node.GetSymbolId().IntegerValue))
            {
                skip = RenderNodeAction.Skip;
                _ElementHasGeometry = true;
            }
            else
            {
                skip = RenderNodeAction.Proceed;
            }
            var flag1 = _TraceElementInvokeSequence;
            return skip;
        }

        private void OnInstanceEnd(InstanceNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
            _TLTableHelper.InstanceEnd(_Transform, node.GetSymbolId().IntegerValue);
            _Transform = _TransformStack.Pop();
        }

        private void OnLight(LightNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
        }

        private RenderNodeAction OnLinkBegin(LinkNode node)
        {
            _DocumentStack.Push(_Document);
            _Document = node.GetDocument();
            _TransformStack.Push(_Transform);
            _Transform = _Transform.Multiply(node.GetTransform());
            return RenderNodeAction.Proceed;
        }

        private void OnLinkEnd(LinkNode node)
        {
            _Document = _DocumentStack.Pop();
            _Transform = _TransformStack.Pop();
        }

        private void OnMaterial(MaterialNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
            var nMaterialId = _TLMaterialHelper.OnMaterial(_Document, node);
            _TLGeometryHelper.OnPrePolyMatrial(nMaterialId);
        }

        private void OnPolymesh(PolymeshTopology node)
        {
            _ElementHasGeometry = true;
            var flag1 = _TraceElementInvokeSequence;
            _TLGeometryHelper.OnPolymesh(node);
        }

        private void OnRPC(RPCNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
            _TLMaterialHelper.OnMaterialForRPC();
            _TLGeometryHelper.OnRPC(_CurrentElement);
        }

        private RenderNodeAction OnViewBegin(ViewNode node)
        {
            _CurrentView = _Document.GetElement(node.ViewId) as View3D;
            _CurrentCameraInfo = node.GetCameraInfo();
            return RenderNodeAction.Proceed;
        }

        private void OnViewEnd(ElementId elementId)
        {
            _CurrentView = null;
            _CurrentCameraInfo = null;
        }

        private bool Start()
        {
            ExportHelper.GetSceneInfo(_Document);
            var option = new ExportOption
            {
                Target = _Target,
                OutputStream = _OutputStream
            };
            _TLTableHelper = new TableHelp(_Document);
            _TLPropHelper = new TLPropertyHelper();
            _TLMaterialHelper = new TLMaterialHelper(_Target, _IncludeTexture, _View);
            _TLGeometryHelper = new TLGeometryHelper();
            _TLTableHelper.AddProject(_Document.PathName);
            return true;
        }
    }
}