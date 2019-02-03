using System.Collections.Generic;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Core;
using BIM.Lmv.Types;

namespace BIM.Lmv
{
    public class DataExport : IDataExport
    {
        private readonly IDataExport _Exporter = new InnerExporter();

        void IDataExport.OnCamera(CameraInfo camera)
        {
            _Exporter.OnCamera(camera);
        }

        void IDataExport.OnElementBegin(int nodeId, Box3F boundingBox)
        {
            _Exporter.OnElementBegin(nodeId, boundingBox);
        }

        void IDataExport.OnElementEnd()
        {
            _Exporter.OnElementEnd();
        }

        void IDataExport.OnFinish()
        {
            _Exporter.OnFinish();
        }

        void IDataExport.OnGeometry(int vertexCount, int triangleCount, bool hasNormal, float[] vertex, int[] indices,
            float[] normals, float[] uvs)
        {
            _Exporter.OnGeometry(vertexCount, triangleCount, hasNormal, vertex, indices, normals, uvs);
        }

        bool IDataExport.OnInstanceBegin(string instanceKey, Transform transform, bool allowReuse)
        {
            return _Exporter.OnInstanceBegin(instanceKey, transform, allowReuse);
        }

        void IDataExport.OnInstanceEnd()
        {
            _Exporter.OnInstanceEnd();
        }

        int IDataExport.OnMaterial(string materialKey, MaterialInfo material)
        {
            return _Exporter.OnMaterial(materialKey, material);
        }

        int IDataExport.OnNode(string key, string name, string uid, int parentNodeId, List<PropItem> props)
        {
            return _Exporter.OnNode(key, name, uid, parentNodeId, props);
        }

        void IDataExport.OnStart(SceneInfo scene, string targetPath, ExportOption option)
        {
            _Exporter.OnStart(scene, targetPath, option);
        }

        void IDataExport.OnTransformBegin(Transform transform)
        {
            _Exporter.OnTransformBegin(transform);
        }

        void IDataExport.OnTransformEnd()
        {
            _Exporter.OnTransformEnd();
        }
    }
}