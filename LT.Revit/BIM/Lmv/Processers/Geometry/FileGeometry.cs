using System;
using BIM.Lmv.Common.Pack;
using BIM.Lmv.Content.Geometry;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Content.Other;
using BIM.Lmv.Processers.Helper;

namespace BIM.Lmv.Processers.Geometry
{
    internal class FileGeometry
    {
        private readonly EntryGeometry _EntryGeometry;
        private readonly PackFileOutput<EntryGeometry> _FileEntryGeometry;
        private readonly OutputProcesser _Output;
        private readonly PackEntryType _PackEntryTypeOpenCTM;
        private readonly SvfFileProcesser _SvfFile;
        private readonly int _VertexLimit;
        private float[] _TempNormals;

        public FileGeometry(OutputProcesser output, SvfFileProcesser svfFile, int limit)
        {
            _Output = output;
            _SvfFile = svfFile;
            _VertexLimit = limit;
            _EntryGeometry = new EntryGeometry(_VertexLimit);
            _PackEntryTypeOpenCTM = new PackEntryType(0, "Autodesk.CloudPlatform.Geometry",
                "Autodesk.CloudPlatform.OpenCTM", 1);
            _FileEntryGeometry = new PackFileOutput<EntryGeometry>(0x400000);
            PackId = -1;
            MaterialId = -1;
            PackEntryIndex = -1;
            PackEntryTriangleCount = -1;
        }

        public int MaterialId { get; private set; }

        public Box3F PackEngryBoundingBox { get; private set; }

        public int PackEntryIndex { get; private set; }

        public int PackEntryTriangleCount { get; private set; }

        public int PackId { get; private set; }

        private void CheckGeometryFile()
        {
            if (!_FileEntryGeometry.IsRunning)
            {
                PackId++;
                _FileEntryGeometry.OnStart();
                _FileEntryGeometry.OnEntryType(_PackEntryTypeOpenCTM);
            }
        }

        private string GetPackFileName(int packId)
        {
            return packId + ".pf";
        }

        public void OnFinish()
        {
            if (_FileEntryGeometry.IsRunning)
            {
                var packFileName = GetPackFileName(PackId);
                var entry = new FileEntryStream(packFileName);
                _FileEntryGeometry.OnFinish(entry.Stream);
                _Output.OnAppendFile(entry);
                RegisterAsset(packFileName, entry.GetSize());
            }
        }

        public bool OnFlush()
        {
            var flag = false;
            if (_EntryGeometry.vertexCount > 0)
            {
                SaveGeometry();
                flag = true;
            }
            if (_FileEntryGeometry.IsRunning &&
                ((_FileEntryGeometry.Position >= 0x200000) || (PackEntryIndex >= 0x7fff)))
            {
                var packFileName = GetPackFileName(PackId);
                var entry = new FileEntryStream(packFileName);
                _FileEntryGeometry.OnFinish(entry.Stream);
                _Output.OnAppendFile(entry);
                RegisterAsset(packFileName, entry.GetSize());
            }
            MaterialId = -1;
            return flag;
        }

        public void OnGeometry(int vertexCount, int triangleCount, bool hasNormal, float[] vertex, int[] indices,
            float[] normals, float[] uvs, Transform transform)
        {
            CheckGeometryFile();
            var geometry = _EntryGeometry;
            geometry.transform = transform;
            var num = geometry.vertexCount;
            var num2 = geometry.triangleCount;
            geometry.vertexCount += vertexCount;
            if (geometry.vertexCount > geometry.Limit)
            {
                throw new NotSupportedException("VertexCount: " + geometry.vertexCount);
            }
            Array.Copy(vertex, 0, geometry.vertex, num*3, vertexCount*3);
            geometry.triangleCount += triangleCount;
            var index = num2*3;
            for (var i = 0; i < triangleCount*3; i++)
            {
                geometry.indices[index] = num + indices[i];
                index++;
            }
            geometry.hasNormal = true;
            if (hasNormal)
            {
                Array.Copy(normals, 0, geometry.normals, num*3, vertexCount*3);
            }
            else
            {
                if (_TempNormals == null)
                {
                    _TempNormals = new float[_VertexLimit*3];
                }
                NormalsCalc.Calc(vertexCount, triangleCount, vertex, indices, _TempNormals);
                Array.Copy(_TempNormals, 0, geometry.normals, num*3, vertexCount*3);
            }
            var info = geometry.uvmaps["diffuse"];
            info.IsValid = true;
            Array.Copy(uvs, 0, info.uvs, num*2, vertexCount*2);
        }

        public bool OnMaterial(int materialId)
        {
            if ((_EntryGeometry.vertexCount > 0) && (materialId != MaterialId))
            {
                SaveGeometry();
                MaterialId = materialId;
                return true;
            }
            MaterialId = materialId;
            return false;
        }

        public bool OnPrePolymesh(int vertexCount, int triangleCount)
        {
            if ((_EntryGeometry.vertexCount > 0) &&
                ((_EntryGeometry.vertexCount + vertexCount >= _EntryGeometry.Limit) ||
                 (_EntryGeometry.triangleCount + triangleCount >= _EntryGeometry.Limit)))
            {
                SaveGeometry();
                return true;
            }
            return false;
        }

        private void RegisterAsset(string filePath, int fileSize)
        {
            _SvfFile.OnEntry(new EntryAsset(filePath, "Autodesk.CloudPlatform.PackFile", filePath, fileSize, 0,
                _SvfFile.AssetTypeGeometry));
        }

        private void SaveGeometry()
        {
            var entry = _EntryGeometry;
            PackEntryTriangleCount = entry.triangleCount;
            PackEntryIndex = _FileEntryGeometry.OnEntry(entry, _PackEntryTypeOpenCTM);
            PackEngryBoundingBox = entry.box;
        }
    }
}