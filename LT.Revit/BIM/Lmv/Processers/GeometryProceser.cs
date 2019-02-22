namespace BIM.Lmv.Processers
{
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Processers.Geometry;
    using BIM.Lmv.Types;
    using System;
    using System.Runtime.InteropServices;

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
            this._FileCamera = new FileCamera(output, svfFile);
            this._FileLight = new FileLight(output, svfFile);
            this._FileGeometry = new FileGeometry(output, svfFile, limit);
            this._FileGeometryMetadata = new FileGeometryMetadata(output, svfFile);
            this._FileFragment = new FileFragment(output, svfFile);
            this._PackId = -1;
        }

        public void OnElementBegin(int nodeId, Transform transform)
        {
            this._FileFragment.OnElementBegin(nodeId, transform);
        }

        public void OnElementEnd()
        {
            int materialId = this._FileGeometry.MaterialId;
            if (this._FileGeometry.OnFlush())
            {
                this.SaveFragAndMeta((uint) materialId);
            }
            this._FileFragment.OnElementEnd();
        }

        public void OnFinish()
        {
            this._FileGeometry.OnFinish();
            this._FileGeometryMetadata.OnFinish();
            this._FileFragment.OnFinish();
            this._FileLight.OnFinish();
            this._FileCamera.OnFinish();
        }

        public bool OnInstanceBegin(string instanceKey, Transform transform, bool allowReuse = true)
        {
            int materialId = this._FileGeometry.MaterialId;
            if (this._FileGeometry.OnFlush())
            {
                this.SaveFragAndMeta((uint) materialId);
            }
            return this._FileFragment.OnInstanceBegin(instanceKey, transform, allowReuse);
        }

        public void OnInstanceEnd(Transform transform)
        {
            int materialId = this._FileGeometry.MaterialId;
            if (this._FileGeometry.OnFlush())
            {
                this.SaveFragAndMeta((uint) materialId);
            }
            this._FileFragment.OnInstanceEnd(transform);
        }

        public void OnMaterial(int materialId)
        {
            int num = this._FileGeometry.MaterialId;
            if (this._FileGeometry.OnMaterial(materialId))
            {
                this.SaveFragAndMeta((uint) num);
            }
        }

        public void OnPolymesh(int vertexCount, int triangleCount, bool hasNormal, float[] vertex, int[] indices, float[] normals, float[] uv, uint materialId, Transform transform)
        {
            int num = this._FileGeometry.MaterialId;
            if (this._FileGeometry.OnPrePolymesh(vertexCount, triangleCount))
            {
                this.SaveFragAndMeta((uint) num);
            }
            this._FileGeometry.OnGeometry(vertexCount, triangleCount, hasNormal, vertex, indices, normals, uv, transform);
        }

        public void OnViewEnd(CameraInfo cameraInfo)
        {
            this._FileCamera.OnCamera(cameraInfo);
        }

        private void SaveFragAndMeta(uint materialId)
        {
            if (this._PackId != this._FileGeometry.PackId)
            {
                this._PackId = this._FileGeometry.PackId;
                this._PackFileName = this._PackId + ".pf";
            }
            ushort primCount = (this._FileGeometry.PackEntryTriangleCount > 0xffff) ? ((ushort) 0xffff) : ((ushort) this._FileGeometry.PackEntryTriangleCount);
            int num2 = this._FileGeometryMetadata.OnAppendItem(this._PackFileName, (uint) this._FileGeometry.PackEntryIndex, primCount);
            this._FileFragment.OnAppendItem(materialId, (uint) num2, this._FileGeometry.PackEngryBoundingBox);
        }
    }
}

