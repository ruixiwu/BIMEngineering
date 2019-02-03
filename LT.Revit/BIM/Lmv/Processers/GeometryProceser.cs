using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Processers.Geometry;
using BIM.Lmv.Types;

namespace BIM.Lmv.Processers
{
    internal class GeometryProceser
    {
        private readonly FileCamera _FileCamera;
        private readonly FileFragment _FileFragment;
        private readonly FileGeometry _FileGeometry;
        private readonly FileGeometryMetadata _FileGeometryMetadata;
        private readonly FileLight _FileLight;
        private string _PackFileName;
        private int _PackId;

        public GeometryProceser(OutputProcesser output, SvfFileProcesser svfFile, int limit)
        {
            _FileCamera = new FileCamera(output, svfFile);
            _FileLight = new FileLight(output, svfFile);
            _FileGeometry = new FileGeometry(output, svfFile, limit);
            _FileGeometryMetadata = new FileGeometryMetadata(output, svfFile);
            _FileFragment = new FileFragment(output, svfFile);
            _PackId = -1;
        }

        public void OnElementBegin(int nodeId, Transform transform)
        {
            _FileFragment.OnElementBegin(nodeId, transform);
        }

        public void OnElementEnd()
        {
            var materialId = _FileGeometry.MaterialId;
            if (_FileGeometry.OnFlush())
            {
                SaveFragAndMeta((uint) materialId);
            }
            _FileFragment.OnElementEnd();
        }

        public void OnFinish()
        {
            _FileGeometry.OnFinish();
            _FileGeometryMetadata.OnFinish();
            _FileFragment.OnFinish();
            _FileLight.OnFinish();
            _FileCamera.OnFinish();
        }

        public bool OnInstanceBegin(string instanceKey, Transform transform, bool allowReuse = true)
        {
            var materialId = _FileGeometry.MaterialId;
            if (_FileGeometry.OnFlush())
            {
                SaveFragAndMeta((uint) materialId);
            }
            return _FileFragment.OnInstanceBegin(instanceKey, transform, allowReuse);
        }

        public void OnInstanceEnd(Transform transform)
        {
            var materialId = _FileGeometry.MaterialId;
            if (_FileGeometry.OnFlush())
            {
                SaveFragAndMeta((uint) materialId);
            }
            _FileFragment.OnInstanceEnd(transform);
        }

        public void OnMaterial(int materialId)
        {
            var num = _FileGeometry.MaterialId;
            if (_FileGeometry.OnMaterial(materialId))
            {
                SaveFragAndMeta((uint) num);
            }
        }

        public void OnPolymesh(int vertexCount, int triangleCount, bool hasNormal, float[] vertex, int[] indices,
            float[] normals, float[] uv, uint materialId, Transform transform)
        {
            var num = _FileGeometry.MaterialId;
            if (_FileGeometry.OnPrePolymesh(vertexCount, triangleCount))
            {
                SaveFragAndMeta((uint) num);
            }
            _FileGeometry.OnGeometry(vertexCount, triangleCount, hasNormal, vertex, indices, normals, uv, transform);
        }

        public void OnViewEnd(CameraInfo cameraInfo)
        {
            _FileCamera.OnCamera(cameraInfo);
        }

        private void SaveFragAndMeta(uint materialId)
        {
            if (_PackId != _FileGeometry.PackId)
            {
                _PackId = _FileGeometry.PackId;
                _PackFileName = _PackId + ".pf";
            }
            var primCount = _FileGeometry.PackEntryTriangleCount > 0xffff
                ? (ushort) 0xffff
                : (ushort) _FileGeometry.PackEntryTriangleCount;
            var num2 = _FileGeometryMetadata.OnAppendItem(_PackFileName, (uint) _FileGeometry.PackEntryIndex, primCount);
            _FileFragment.OnAppendItem(materialId, (uint) num2, _FileGeometry.PackEngryBoundingBox);
        }
    }
}