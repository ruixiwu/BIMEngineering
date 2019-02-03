using System.Collections.Generic;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Types;

namespace BIM.Lmv
{
    public interface IDataExport
    {
        void OnCamera(CameraInfo camera);
        void OnElementBegin(int nodeId, Box3F boundingBox);
        void OnElementEnd();
        void OnFinish();

        void OnGeometry(int vertexCount, int triangleCount, bool hasNormal, float[] vertex, int[] indices,
            float[] normals, float[] uvs);

        bool OnInstanceBegin(string instanceKey, Transform transform, bool allowReuse = true);
        void OnInstanceEnd();
        int OnMaterial(string materialKey, MaterialInfo material);
        int OnNode(string key, string name, string uid, int parentNodeId, List<PropItem> props);
        void OnStart(SceneInfo scene, string targetPath, ExportOption option = null);
        void OnTransformBegin(Transform transform);
        void OnTransformEnd();
    }
}