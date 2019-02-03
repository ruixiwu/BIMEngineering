using System;
using System.Collections.Generic;
using System.IO;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Processers;
using BIM.Lmv.Types;

namespace BIM.Lmv.Core
{
    internal class InnerExporter : IDataExport
    {
        private readonly Stack<Transform> _TransformStack = new Stack<Transform>();
        private Box3F _ElementBox;
        private ExportOption _ExportOption;
        private GeometryProceser _GeometryProceser;
        private Vector3D _GlobalBoxMax;
        private Vector3D _GlobalBoxMin;
        private bool _IsStart;
        private int _MaterialId;
        private MaterialProcesser _MaterialNodeProcesser;
        private OutputProcesser _OutputProcesser;
        private PropertyProcesser _PropertyProcesser;
        private SvfFileProcesser _SvfFileProcesser;
        private string _TargetPath;
        private Transform _Transform;
        private int _TransformDepth;

        void IDataExport.OnCamera(CameraInfo camera)
        {
            _GeometryProceser.OnViewEnd(camera);
        }

        void IDataExport.OnElementBegin(int nodeId, Box3F boundingBox)
        {
            if (!_IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            _ElementBox = boundingBox;
            if (_ElementBox != null)
            {
                _GlobalBoxMin.x = Math.Min(_GlobalBoxMin.x, boundingBox.min.x);
                _GlobalBoxMin.y = Math.Min(_GlobalBoxMin.y, boundingBox.min.y);
                _GlobalBoxMin.z = Math.Min(_GlobalBoxMin.z, boundingBox.min.z);
                _GlobalBoxMax.x = Math.Max(_GlobalBoxMax.x, boundingBox.max.x);
                _GlobalBoxMax.y = Math.Max(_GlobalBoxMax.y, boundingBox.max.y);
                _GlobalBoxMax.z = Math.Max(_GlobalBoxMax.z, boundingBox.max.z);
            }
            _GeometryProceser.OnElementBegin(nodeId, _Transform);
        }

        void IDataExport.OnElementEnd()
        {
            if (!_IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            _GeometryProceser.OnElementEnd();
        }

        void IDataExport.OnFinish()
        {
            if (!_IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            _IsStart = false;
            _PropertyProcesser.OnFinish();
            _PropertyProcesser = null;
            _GeometryProceser.OnFinish();
            _GeometryProceser = null;
            _MaterialNodeProcesser.OnFinish();
            _MaterialNodeProcesser = null;
            _SvfFileProcesser.OnFinish(_GlobalBoxMin, _GlobalBoxMax);
            _SvfFileProcesser = null;
            if (_ExportOption.Target != ExportTarget.LocalFolder)
            {
                if (_ExportOption.OutputStream == null)
                {
                    var mode = File.Exists(_TargetPath) ? FileMode.Truncate : FileMode.Create;
                    using (var stream = File.Open(_TargetPath, mode, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        _OutputProcesser.OnFinish(stream);
                    }
                }
                else
                {
                    _OutputProcesser.OnFinish(_ExportOption.OutputStream);
                }
            }
            _OutputProcesser = null;
        }

        void IDataExport.OnGeometry(int vertexCount, int triangleCount, bool hasNormal, float[] vertex, int[] indices,
            float[] normals, float[] uvs)
        {
            if (!_IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            if (_ElementBox == null)
            {
                var point = new Vector3F();
                for (var i = 0; i < vertexCount; i++)
                {
                    point.x = vertex[i*3];
                    point.y = vertex[i*3 + 1];
                    point.z = vertex[i*3 + 2];
                    if (_Transform.type != TransformType.Identity)
                    {
                        point = _Transform.OfPoint(point);
                    }
                    _GlobalBoxMin.x = Math.Min(_GlobalBoxMin.x, point.x);
                    _GlobalBoxMin.y = Math.Min(_GlobalBoxMin.y, point.y);
                    _GlobalBoxMin.z = Math.Min(_GlobalBoxMin.z, point.z);
                    _GlobalBoxMax.x = Math.Max(_GlobalBoxMax.x, point.x);
                    _GlobalBoxMax.y = Math.Max(_GlobalBoxMax.y, point.y);
                    _GlobalBoxMax.z = Math.Max(_GlobalBoxMax.z, point.z);
                }
            }
            _GeometryProceser.OnPolymesh(vertexCount, triangleCount, hasNormal, vertex, indices, normals, uvs,
                (uint) _MaterialId, _Transform);
        }

        bool IDataExport.OnInstanceBegin(string instanceKey, Transform transform, bool allowReuse)
        {
            if (!_IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            _TransformDepth++;
            _TransformStack.Push(_Transform);
            _Transform = _Transform.Clone().Multiply(transform);
            return _GeometryProceser.OnInstanceBegin(instanceKey, _Transform, allowReuse);
        }

        void IDataExport.OnInstanceEnd()
        {
            if (!_IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            if (_TransformDepth == 0)
            {
                throw new InvalidOperationException("TransformDepth = 0");
            }
            _TransformDepth--;
            _Transform = _TransformStack.Pop();
            _GeometryProceser.OnInstanceEnd(_Transform);
        }

        int IDataExport.OnMaterial(string materialKey, MaterialInfo material)
        {
            if (!_IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            _MaterialId = _MaterialNodeProcesser.OnMaterial(materialKey, material);
            _GeometryProceser.OnMaterial(_MaterialId);
            return _MaterialId;
        }

        int IDataExport.OnNode(string key, string name, string uid, int parentNodeId, List<PropItem> props)
        {
            if (!_IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            return _PropertyProcesser.OnNode(key, name, uid, parentNodeId, props);
        }

        void IDataExport.OnStart(SceneInfo scene, string targetPath, ExportOption option)
        {
            if (_IsStart)
            {
                throw new InvalidOperationException();
            }
            _IsStart = true;
            scene = scene ?? new SceneInfo();
            option = option ?? new ExportOption();
            _TargetPath = targetPath;
            _ExportOption = option;
            _OutputProcesser = new OutputProcesser(targetPath, option);
            _SvfFileProcesser = new SvfFileProcesser(_OutputProcesser, scene, option);
            _PropertyProcesser = new PropertyProcesser(_OutputProcesser, _SvfFileProcesser);
            _MaterialNodeProcesser = new MaterialProcesser(_OutputProcesser, _SvfFileProcesser);
            _GeometryProceser = new GeometryProceser(_OutputProcesser, _SvfFileProcesser, scene.VertexLimit);
            _PropertyProcesser.OnStart();
            _TransformStack.Clear();
            _Transform = Transform.GetIdentity();
            _TransformDepth = 0;
            _GlobalBoxMin = new Vector3D(double.MaxValue, double.MaxValue, double.MaxValue);
            _GlobalBoxMax = new Vector3D(double.MinValue, double.MinValue, double.MinValue);
        }

        void IDataExport.OnTransformBegin(Transform transform)
        {
            _TransformDepth++;
            _TransformStack.Push(_Transform);
            _Transform = _Transform.Clone().Multiply(transform);
        }

        void IDataExport.OnTransformEnd()
        {
            if (!_IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            if (_TransformDepth == 0)
            {
                throw new InvalidOperationException("TransformDepth = 0");
            }
            _TransformDepth--;
            _Transform = _TransformStack.Pop();
        }
    }
}