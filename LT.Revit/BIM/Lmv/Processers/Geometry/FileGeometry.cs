namespace BIM.Lmv.Processers.Geometry
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Content.Geometry;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Content.Other;
    using BIM.Lmv.Processers;
    using BIM.Lmv.Processers.Helper;
    using System;
    using System.Runtime.CompilerServices;

    internal class FileGeometry
    {
        private int _CurrentGeometryPackId;
        private readonly EntryGeometry _EntryGeometry;
        private readonly PackFileOutput<EntryGeometry> _FileEntryGeometry;
        private int _MaterialId;
        private readonly OutputProcesser _Output;
        private readonly PackEntryType _PackEntryTypeOpenCTM;
        private readonly SvfFileProcesser _SvfFile;
        private float[] _TempNormals;
        private readonly int _VertexLimit;

        public FileGeometry(OutputProcesser output, SvfFileProcesser svfFile, int limit)
        {
            this._Output = output;
            this._SvfFile = svfFile;
            this._VertexLimit = limit;
            this._EntryGeometry = new EntryGeometry(this._VertexLimit);
            this._PackEntryTypeOpenCTM = new PackEntryType(0, "Autodesk.CloudPlatform.Geometry", "Autodesk.CloudPlatform.OpenCTM", 1);
            this._FileEntryGeometry = new PackFileOutput<EntryGeometry>(0x400000);
            this._CurrentGeometryPackId = -1;
            this._MaterialId = -1;
            this.PackEntryIndex = -1;
            this.PackEntryTriangleCount = -1;
        }

        private void CheckGeometryFile()
        {
            if (!this._FileEntryGeometry.IsRunning)
            {
                this._CurrentGeometryPackId++;
                this._FileEntryGeometry.OnStart();
                this._FileEntryGeometry.OnEntryType(this._PackEntryTypeOpenCTM);
            }
        }

        private string GetPackFileName(int packId) => 
            (packId + ".pf");

        public void OnFinish()
        {
            if (this._FileEntryGeometry.IsRunning)
            {
                string packFileName = this.GetPackFileName(this._CurrentGeometryPackId);
                FileEntryStream entry = new FileEntryStream(packFileName);
                this._FileEntryGeometry.OnFinish(entry.Stream);
                this._Output.OnAppendFile(entry);
                this.RegisterAsset(packFileName, entry.GetSize());
            }
        }

        public bool OnFlush()
        {
            bool flag = false;
            if (this._EntryGeometry.vertexCount > 0)
            {
                this.SaveGeometry();
                flag = true;
            }
            if (this._FileEntryGeometry.IsRunning && ((this._FileEntryGeometry.Position >= 0x200000) || (this.PackEntryIndex >= 0x7fff)))
            {
                string packFileName = this.GetPackFileName(this._CurrentGeometryPackId);
                FileEntryStream entry = new FileEntryStream(packFileName);
                this._FileEntryGeometry.OnFinish(entry.Stream);
                this._Output.OnAppendFile(entry);
                this.RegisterAsset(packFileName, entry.GetSize());
            }
            this._MaterialId = -1;
            return flag;
        }

        public void OnGeometry(int vertexCount, int triangleCount, bool hasNormal, float[] vertex, int[] indices, float[] normals, float[] uvs, Transform transform)
        {
            this.CheckGeometryFile();
            EntryGeometry geometry = this._EntryGeometry;
            geometry.transform = transform;
            int num = geometry.vertexCount;
            int num2 = geometry.triangleCount;
            geometry.vertexCount += vertexCount;
            if (geometry.vertexCount > geometry.Limit)
            {
                throw new NotSupportedException("VertexCount: " + geometry.vertexCount);
            }
            Array.Copy(vertex, 0, geometry.vertex, num * 3, vertexCount * 3);
            geometry.triangleCount += triangleCount;
            int index = num2 * 3;
            for (int i = 0; i < (triangleCount * 3); i++)
            {
                geometry.indices[index] = num + indices[i];
                index++;
            }
            geometry.hasNormal = true;
            if (hasNormal)
            {
                Array.Copy(normals, 0, geometry.normals, num * 3, vertexCount * 3);
            }
            else
            {
                if (this._TempNormals == null)
                {
                    this._TempNormals = new float[this._VertexLimit * 3];
                }
                NormalsCalc.Calc(vertexCount, triangleCount, vertex, indices, this._TempNormals);
                Array.Copy(this._TempNormals, 0, geometry.normals, num * 3, vertexCount * 3);
            }
            UVInfo info = geometry.uvmaps["diffuse"];
            info.IsValid = true;
            Array.Copy(uvs, 0, info.uvs, num * 2, vertexCount * 2);
        }

        public bool OnMaterial(int materialId)
        {
            if ((this._EntryGeometry.vertexCount > 0) && (materialId != this._MaterialId))
            {
                this.SaveGeometry();
                this._MaterialId = materialId;
                return true;
            }
            this._MaterialId = materialId;
            return false;
        }

        public bool OnPrePolymesh(int vertexCount, int triangleCount)
        {
            if ((this._EntryGeometry.vertexCount > 0) && (((this._EntryGeometry.vertexCount + vertexCount) >= this._EntryGeometry.Limit) || ((this._EntryGeometry.triangleCount + triangleCount) >= this._EntryGeometry.Limit)))
            {
                this.SaveGeometry();
                return true;
            }
            return false;
        }

        private void RegisterAsset(string filePath, int fileSize)
        {
            this._SvfFile.OnEntry(new EntryAsset(filePath, "Autodesk.CloudPlatform.PackFile", filePath, new int?(fileSize), 0, this._SvfFile.AssetTypeGeometry));
        }

        private void SaveGeometry()
        {
            EntryGeometry entry = this._EntryGeometry;
            this.PackEntryTriangleCount = entry.triangleCount;
            this.PackEntryIndex = this._FileEntryGeometry.OnEntry(entry, this._PackEntryTypeOpenCTM);
            this.PackEngryBoundingBox = entry.box;
        }

        public int MaterialId =>
            this._MaterialId;

        public Box3F PackEngryBoundingBox { get; private set; }

        public int PackEntryIndex { get; private set; }

        public int PackEntryTriangleCount { get; private set; }

        public int PackId =>
            this._CurrentGeometryPackId;
    }
}

