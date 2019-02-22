namespace BIM.Lmv.Core
{
    using BIM.Lmv;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Processers;
    using BIM.Lmv.Types;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class InnerExporter : IDataExport
    {
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
        private readonly Stack<Transform> _TransformStack = new Stack<Transform>();

        void IDataExport.OnCamera(CameraInfo camera)
        {
            this._GeometryProceser.OnViewEnd(camera);
        }

        void IDataExport.OnElementBegin(int nodeId, Box3F boundingBox)
        {
            if (!this._IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            this._ElementBox = boundingBox;
            if (this._ElementBox != null)
            {
                this._GlobalBoxMin.x = Math.Min(this._GlobalBoxMin.x, (double) boundingBox.min.x);
                this._GlobalBoxMin.y = Math.Min(this._GlobalBoxMin.y, (double) boundingBox.min.y);
                this._GlobalBoxMin.z = Math.Min(this._GlobalBoxMin.z, (double) boundingBox.min.z);
                this._GlobalBoxMax.x = Math.Max(this._GlobalBoxMax.x, (double) boundingBox.max.x);
                this._GlobalBoxMax.y = Math.Max(this._GlobalBoxMax.y, (double) boundingBox.max.y);
                this._GlobalBoxMax.z = Math.Max(this._GlobalBoxMax.z, (double) boundingBox.max.z);
            }
            this._GeometryProceser.OnElementBegin(nodeId, this._Transform);
        }

        void IDataExport.OnElementEnd()
        {
            if (!this._IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            this._GeometryProceser.OnElementEnd();
        }

        void IDataExport.OnFinish()
        {
            if (!this._IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            this._IsStart = false;
            this._PropertyProcesser.OnFinish();
            this._PropertyProcesser = null;
            this._GeometryProceser.OnFinish();
            this._GeometryProceser = null;
            this._MaterialNodeProcesser.OnFinish();
            this._MaterialNodeProcesser = null;
            this._SvfFileProcesser.OnFinish(this._GlobalBoxMin, this._GlobalBoxMax);
            this._SvfFileProcesser = null;
            if (this._ExportOption.Target != ExportTarget.LocalFolder)
            {
                if (this._ExportOption.OutputStream == null)
                {
                    FileMode mode = File.Exists(this._TargetPath) ? FileMode.Truncate : FileMode.Create;
                    using (FileStream stream = File.Open(this._TargetPath, mode, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        this._OutputProcesser.OnFinish(stream);
                    }
                }
                else
                {
                    this._OutputProcesser.OnFinish(this._ExportOption.OutputStream);
                }
            }
            this._OutputProcesser = null;
        }

        void IDataExport.OnGeometry(int vertexCount, int triangleCount, bool hasNormal, float[] vertex, int[] indices, float[] normals, float[] uvs)
        {
            if (!this._IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            if (this._ElementBox == null)
            {
                Vector3F point = new Vector3F();
                for (int i = 0; i < vertexCount; i++)
                {
                    point.x = vertex[i * 3];
                    point.y = vertex[(i * 3) + 1];
                    point.z = vertex[(i * 3) + 2];
                    if (this._Transform.type != TransformType.Identity)
                    {
                        point = this._Transform.OfPoint(point);
                    }
                    this._GlobalBoxMin.x = Math.Min(this._GlobalBoxMin.x, (double) point.x);
                    this._GlobalBoxMin.y = Math.Min(this._GlobalBoxMin.y, (double) point.y);
                    this._GlobalBoxMin.z = Math.Min(this._GlobalBoxMin.z, (double) point.z);
                    this._GlobalBoxMax.x = Math.Max(this._GlobalBoxMax.x, (double) point.x);
                    this._GlobalBoxMax.y = Math.Max(this._GlobalBoxMax.y, (double) point.y);
                    this._GlobalBoxMax.z = Math.Max(this._GlobalBoxMax.z, (double) point.z);
                }
            }
            this._GeometryProceser.OnPolymesh(vertexCount, triangleCount, hasNormal, vertex, indices, normals, uvs, (uint) this._MaterialId, this._Transform);
        }

        bool IDataExport.OnInstanceBegin(string instanceKey, Transform transform, bool allowReuse)
        {
            if (!this._IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            this._TransformDepth++;
            this._TransformStack.Push(this._Transform);
            this._Transform = this._Transform.Clone().Multiply(transform);
            return this._GeometryProceser.OnInstanceBegin(instanceKey, this._Transform, allowReuse);
        }

        void IDataExport.OnInstanceEnd()
        {
            if (!this._IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            if (this._TransformDepth == 0)
            {
                throw new InvalidOperationException("TransformDepth = 0");
            }
            this._TransformDepth--;
            this._Transform = this._TransformStack.Pop();
            this._GeometryProceser.OnInstanceEnd(this._Transform);
        }

        int IDataExport.OnMaterial(string materialKey, MaterialInfo material)
        {
            if (!this._IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            this._MaterialId = this._MaterialNodeProcesser.OnMaterial(materialKey, material);
            this._GeometryProceser.OnMaterial(this._MaterialId);
            return this._MaterialId;
        }

        int IDataExport.OnNode(string key, string name, string uid, int parentNodeId, List<PropItem> props)
        {
            if (!this._IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            return this._PropertyProcesser.OnNode(key, name, uid, parentNodeId, props);
        }

        void IDataExport.OnStart(SceneInfo scene, string targetPath, ExportOption option)
        {
            if (this._IsStart)
            {
                throw new InvalidOperationException();
            }
            this._IsStart = true;
            scene = scene ?? new SceneInfo();
            option = option ?? new ExportOption();
            this._TargetPath = targetPath;
            this._ExportOption = option;
            this._OutputProcesser = new OutputProcesser(targetPath, option);
            this._SvfFileProcesser = new SvfFileProcesser(this._OutputProcesser, scene, option);
            this._PropertyProcesser = new PropertyProcesser(this._OutputProcesser, this._SvfFileProcesser);
            this._MaterialNodeProcesser = new MaterialProcesser(this._OutputProcesser, this._SvfFileProcesser);
            this._GeometryProceser = new GeometryProceser(this._OutputProcesser, this._SvfFileProcesser, scene.VertexLimit);
            this._PropertyProcesser.OnStart();
            this._TransformStack.Clear();
            this._Transform = Transform.GetIdentity();
            this._TransformDepth = 0;
            this._GlobalBoxMin = new Vector3D(double.MaxValue, double.MaxValue, double.MaxValue);
            this._GlobalBoxMax = new Vector3D(double.MinValue, double.MinValue, double.MinValue);
        }

        void IDataExport.OnTransformBegin(Transform transform)
        {
            this._TransformDepth++;
            this._TransformStack.Push(this._Transform);
            this._Transform = this._Transform.Clone().Multiply(transform);
        }

        void IDataExport.OnTransformEnd()
        {
            if (!this._IsStart)
            {
                throw new InvalidOperationException("IsStart = false");
            }
            if (this._TransformDepth == 0)
            {
                throw new InvalidOperationException("TransformDepth = 0");
            }
            this._TransformDepth--;
            this._Transform = this._TransformStack.Pop();
        }
    }
}

