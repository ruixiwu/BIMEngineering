using BIM.Lmv.Core;

namespace BIM.Lmv
{
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Types;
    using System;
    using System.Collections.Generic;

    public class DataExport : IDataExport
    {
        private readonly IDataExport _Exporter = new InnerExporter();

        void IDataExport.OnCamera(CameraInfo camera)
        {
            this._Exporter.OnCamera(camera);
        }

        void IDataExport.OnElementBegin(int nodeId, Box3F boundingBox)
        {
            this._Exporter.OnElementBegin(nodeId, boundingBox);
        }

        void IDataExport.OnElementEnd()
        {
            this._Exporter.OnElementEnd();
        }

        void IDataExport.OnFinish()
        {
            this._Exporter.OnFinish();
        }

        void IDataExport.OnGeometry(int vertexCount, int triangleCount, bool hasNormal, float[] vertex, int[] indices, float[] normals, float[] uvs)
        {
            this._Exporter.OnGeometry(vertexCount, triangleCount, hasNormal, vertex, indices, normals, uvs);
        }

        bool IDataExport.OnInstanceBegin(string instanceKey, Transform transform, bool allowReuse) => 
            this._Exporter.OnInstanceBegin(instanceKey, transform, allowReuse);

        void IDataExport.OnInstanceEnd()
        {
            this._Exporter.OnInstanceEnd();
        }

        int IDataExport.OnMaterial(string materialKey, MaterialInfo material) => 
            this._Exporter.OnMaterial(materialKey, material);

        int IDataExport.OnNode(string key, string name, string uid, int parentNodeId, List<PropItem> props) => 
            this._Exporter.OnNode(key, name, uid, parentNodeId, props);

        void IDataExport.OnStart(SceneInfo scene, string targetPath, ExportOption option)
        {
            this._Exporter.OnStart(scene, targetPath, option);
        }

        void IDataExport.OnTransformBegin(Transform transform)
        {
            this._Exporter.OnTransformBegin(transform);
        }

        void IDataExport.OnTransformEnd()
        {
            this._Exporter.OnTransformEnd();
        }
    }
}

