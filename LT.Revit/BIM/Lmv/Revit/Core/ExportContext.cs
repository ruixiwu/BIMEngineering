using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Revit.Helpers;
using BIM.Lmv.Types;
using CameraInfo = Autodesk.Revit.DB.CameraInfo;

namespace BIM.Lmv.Revit.Core
{
    internal class ExportContext : IExportContext
    {
        private readonly Stack<Document> _DocumentStack = new Stack<Document>();
        private readonly Dictionary<int, bool> _ElementIds;
        private readonly bool _IncludeProperty;
        private readonly bool _IncludeTexture;
        private readonly Stream _OutputStream;
        private readonly ExportTarget _Target;
        private readonly string _TargetPath;
        private readonly Action<string, string, string> _Trace;
        private readonly bool _Cancelled;
        private CameraInfo _CurrentCameraInfo;
        private Element _CurrentElement;
        private View3D _CurrentView;
        private Document _Document;
        private BoundingBoxXYZ _ElementBox;
        private bool _ElementHasGeometry;
        private IDataExport _Exporter;
        private GeometryHelper _GeometryHelper;
        private Vector3F _GlobalBoxMax;
        private Vector3F _GlobalBoxMin;
        private bool _IsElementSkiped;
        private MaterialHelper _MaterialHelper;
        private PropertyHelper _PropHelper;
        private bool _TraceElementInvokeSequence;
        public View3D _View;

        public ExportContext(View3D view, Document document, string targetPath, ExportTarget target = 0,
            Stream outputStream = null, bool includeTexture = true, bool includeProperty = true,
            Action<string, string, string> trace = null, Dictionary<int, bool> elementIds = null)
        {
            _View = view;
            _Document = document;
            _Cancelled = false;
            _Target = target;
            _OutputStream = outputStream;
            _IncludeTexture = includeTexture;
            _IncludeProperty = includeProperty;
            _Trace = trace;
            _TargetPath = targetPath;
            _ElementIds = elementIds;
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
        {
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
            _Exporter.OnFinish();
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
            var nodeId = _PropHelper.OnElement(element);
            _ElementBox = element.get_BoundingBox(_CurrentView);
            _Exporter.OnElementBegin(nodeId, TransformHelper.Convert(_ElementBox));
            return RenderNodeAction.Proceed;
        }

        private void OnElementEnd(ElementId elementId)
        {
            var flag1 = _TraceElementInvokeSequence;
            if (!_IsElementSkiped)
            {
                _Exporter.OnElementEnd();
            }
            if (_TraceElementInvokeSequence)
            {
                _TraceElementInvokeSequence = false;
            }
            if (_ElementHasGeometry && (_ElementBox != null))
            {
                var xxyz = _ElementBox;
                _GlobalBoxMin.x = Math.Min(_GlobalBoxMin.x, (float) xxyz.Min.X);
                _GlobalBoxMin.y = Math.Min(_GlobalBoxMin.y, (float) xxyz.Min.Y);
                _GlobalBoxMin.z = Math.Min(_GlobalBoxMin.z, (float) xxyz.Min.Z);
                _GlobalBoxMax.x = Math.Max(_GlobalBoxMax.x, (float) xxyz.Max.X);
                _GlobalBoxMax.y = Math.Max(_GlobalBoxMax.y, (float) xxyz.Max.Y);
                _GlobalBoxMax.z = Math.Max(_GlobalBoxMax.z, (float) xxyz.Max.Z);
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
            var flag1 = _TraceElementInvokeSequence;
            var instanceKey = node.GetSymbolId().IntegerValue.ToString();
            var transform = TransformHelper.Convert(node.GetTransform());
            if (_Exporter.OnInstanceBegin(instanceKey, transform, !_TraceElementInvokeSequence))
            {
                skip = RenderNodeAction.Skip;
                _ElementHasGeometry = true;
            }
            else
            {
                skip = RenderNodeAction.Proceed;
            }
            var flag2 = _TraceElementInvokeSequence;
            return skip;
        }

        private void OnInstanceEnd(InstanceNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
            _Exporter.OnInstanceEnd();
        }

        private void OnLight(LightNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
        }

        private RenderNodeAction OnLinkBegin(LinkNode node)
        {
            _DocumentStack.Push(_Document);
            _Document = node.GetDocument();
            _Exporter.OnTransformBegin(TransformHelper.Convert(node.GetTransform()));
            return RenderNodeAction.Proceed;
        }

        private void OnLinkEnd(LinkNode node)
        {
            _Document = _DocumentStack.Pop();
            _Exporter.OnTransformEnd();
        }

        private void OnMaterial(MaterialNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
            _MaterialHelper.OnMaterial(_Document, node);
        }

        private void OnPolymesh(PolymeshTopology node)
        {
            _ElementHasGeometry = true;
            var flag1 = _TraceElementInvokeSequence;
            _GeometryHelper.OnPolymesh(node);
        }

        private void OnRPC(RPCNode node)
        {
            var flag1 = _TraceElementInvokeSequence;
            _MaterialHelper.OnMaterialForRPC();
            _GeometryHelper.OnRPC(_CurrentElement);
        }

        private RenderNodeAction OnViewBegin(ViewNode node)
        {
            _CurrentView = _Document.GetElement(node.ViewId) as View3D;
            _CurrentCameraInfo = node.GetCameraInfo();
            return RenderNodeAction.Proceed;
        }

        private void OnViewEnd(ElementId elementId)
        {
            _Exporter.OnCamera(ExportHelper.GetCameraInfo(_CurrentView, _CurrentCameraInfo,
                _GlobalBoxMin, _GlobalBoxMax));
            _CurrentView = null;
            _CurrentCameraInfo = null;
        }

        private bool Start()
        {
            _GlobalBoxMin = new Vector3F(float.MaxValue, float.MaxValue, float.MaxValue);
            _GlobalBoxMax = new Vector3F(float.MinValue, float.MinValue, float.MinValue);
            var sceneInfo = ExportHelper.GetSceneInfo(_Document);
            var option = new ExportOption
            {
                Target = _Target,
                OutputStream = _OutputStream
            };
            _Exporter = new DataExport();
            _Exporter.OnStart(sceneInfo, _TargetPath, option);
            _PropHelper = new PropertyHelper(_Exporter, _Document, _IncludeProperty);
            _MaterialHelper = new MaterialHelper(_Exporter, _Target, _IncludeTexture, _View);
            _GeometryHelper = new GeometryHelper(_Exporter);
            return true;
        }
    }
}