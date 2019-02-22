namespace BIM.Lmv.Revit.Core
{
    using Autodesk.Revit.DB;
    using BIM.Lmv;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Revit.Helpers;
    using BIM.Lmv.Types;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class ExportContext : IExportContext
    {
        private bool _Cancelled;
        private Autodesk.Revit.DB.CameraInfo _CurrentCameraInfo;
        private Element _CurrentElement;
        private View3D _CurrentView;
        private Document _Document;
        private readonly Stack<Document> _DocumentStack = new Stack<Document>();
        private BoundingBoxXYZ _ElementBox;
        private bool _ElementHasGeometry;
        private readonly Dictionary<int, bool> _ElementIds;
        private IDataExport _Exporter;
        private GeometryHelper _GeometryHelper;
        private Vector3F _GlobalBoxMax;
        private Vector3F _GlobalBoxMin;
        private readonly bool _IncludeProperty;
        private readonly bool _IncludeTexture;
        private bool _IsElementSkiped;
        private MaterialHelper _MaterialHelper;
        private readonly Stream _OutputStream;
        private PropertyHelper _PropHelper;
        private readonly ExportTarget _Target;
        private readonly string _TargetPath;
        private readonly Action<string, string, string> _Trace;
        private bool _TraceElementInvokeSequence;
        public View3D _View;

        public ExportContext(View3D view, Document document, string targetPath, ExportTarget target = 0, Stream outputStream = null, bool includeTexture = true, bool includeProperty = true, Action<string, string, string> trace = null, Dictionary<int, bool> elementIds = null)
        {
            this._View = view;
            this._Document = document;
            this._Cancelled = false;
            this._Target = target;
            this._OutputStream = outputStream;
            this._IncludeTexture = includeTexture;
            this._IncludeProperty = includeProperty;
            this._Trace = trace;
            this._TargetPath = targetPath;
            this._ElementIds = elementIds;
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
            this._Exporter.OnFinish();
        }

        private bool IsCanceled() => 
            this._Cancelled;

        private void OnDaylightPortal(DaylightPortalNode node)
        {
        }

        private RenderNodeAction OnElementBegin(ElementId elementId)
        {
            this._ElementHasGeometry = false;
            this._IsElementSkiped = false;
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
            int nodeId = this._PropHelper.OnElement(element);
            this._ElementBox = element.get_BoundingBox(this._CurrentView);
            this._Exporter.OnElementBegin(nodeId, TransformHelper.Convert(this._ElementBox));
            return RenderNodeAction.Proceed;
        }

        private void OnElementEnd(ElementId elementId)
        {
            bool flag1 = this._TraceElementInvokeSequence;
            if (!this._IsElementSkiped)
            {
                this._Exporter.OnElementEnd();
            }
            if (this._TraceElementInvokeSequence)
            {
                this._TraceElementInvokeSequence = false;
            }
            if (this._ElementHasGeometry && (this._ElementBox != null))
            {
                BoundingBoxXYZ xxyz = this._ElementBox;
                this._GlobalBoxMin.x = Math.Min(this._GlobalBoxMin.x, (float) xxyz.Min.X);
                this._GlobalBoxMin.y = Math.Min(this._GlobalBoxMin.y, (float) xxyz.Min.Y);
                this._GlobalBoxMin.z = Math.Min(this._GlobalBoxMin.z, (float) xxyz.Min.Z);
                this._GlobalBoxMax.x = Math.Max(this._GlobalBoxMax.x, (float) xxyz.Max.X);
                this._GlobalBoxMax.y = Math.Max(this._GlobalBoxMax.y, (float) xxyz.Max.Y);
                this._GlobalBoxMax.z = Math.Max(this._GlobalBoxMax.z, (float) xxyz.Max.Z);
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
            bool flag1 = this._TraceElementInvokeSequence;
            string instanceKey = node.GetSymbolId().IntegerValue.ToString();
            BIM.Lmv.Content.Geometry.Types.Transform transform = TransformHelper.Convert(node.GetTransform());
            if (this._Exporter.OnInstanceBegin(instanceKey, transform, !this._TraceElementInvokeSequence))
            {
                skip = RenderNodeAction.Skip;
                this._ElementHasGeometry = true;
            }
            else
            {
                skip = RenderNodeAction.Proceed;
            }
            bool flag2 = this._TraceElementInvokeSequence;
            return skip;
        }

        private void OnInstanceEnd(InstanceNode node)
        {
            bool flag1 = this._TraceElementInvokeSequence;
            this._Exporter.OnInstanceEnd();
        }

        private void OnLight(LightNode node)
        {
            bool flag1 = this._TraceElementInvokeSequence;
        }

        private RenderNodeAction OnLinkBegin(LinkNode node)
        {
            this._DocumentStack.Push(this._Document);
            this._Document = node.GetDocument();
            this._Exporter.OnTransformBegin(TransformHelper.Convert(node.GetTransform()));
            return RenderNodeAction.Proceed;
        }

        private void OnLinkEnd(LinkNode node)
        {
            this._Document = this._DocumentStack.Pop();
            this._Exporter.OnTransformEnd();
        }

        private void OnMaterial(MaterialNode node)
        {
            bool flag1 = this._TraceElementInvokeSequence;
            this._MaterialHelper.OnMaterial(this._Document, node);
        }

        private void OnPolymesh(PolymeshTopology node)
        {
            this._ElementHasGeometry = true;
            bool flag1 = this._TraceElementInvokeSequence;
            this._GeometryHelper.OnPolymesh(node);
        }

        private void OnRPC(RPCNode node)
        {
            bool flag1 = this._TraceElementInvokeSequence;
            this._MaterialHelper.OnMaterialForRPC();
            this._GeometryHelper.OnRPC(this._CurrentElement);
        }

        private RenderNodeAction OnViewBegin(ViewNode node)
        {
            this._CurrentView = this._Document.GetElement(node.ViewId) as View3D;
            this._CurrentCameraInfo = node.GetCameraInfo();
            return RenderNodeAction.Proceed;
        }

        private void OnViewEnd(ElementId elementId)
        {
            this._Exporter.OnCamera(ExportHelper.GetCameraInfo(this._CurrentView, this._CurrentCameraInfo, this._GlobalBoxMin, this._GlobalBoxMax));
            this._CurrentView = null;
            this._CurrentCameraInfo = null;
        }

        private bool Start()
        {
            this._GlobalBoxMin = new Vector3F(float.MaxValue, float.MaxValue, float.MaxValue);
            this._GlobalBoxMax = new Vector3F(float.MinValue, float.MinValue, float.MinValue);
            SceneInfo sceneInfo = ExportHelper.GetSceneInfo(this._Document);
            ExportOption option = new ExportOption {
                Target = this._Target,
                OutputStream = this._OutputStream
            };
            this._Exporter = new DataExport();
            this._Exporter.OnStart(sceneInfo, this._TargetPath, option);
            this._PropHelper = new PropertyHelper(this._Exporter, this._Document, this._IncludeProperty);
            this._MaterialHelper = new MaterialHelper(this._Exporter, this._Target, this._IncludeTexture, this._View);
            this._GeometryHelper = new GeometryHelper(this._Exporter);
            return true;
        }
    }
}

