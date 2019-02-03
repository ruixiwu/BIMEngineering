using System.Linq;
using Autodesk.Revit.DB;

namespace BIM.Lmv.Revit.Helpers
{
    internal class TLGeometryHelper
    {
        public static int _CurMaterialId;
        public static string strMeshIds;
        private readonly BFileHelp _BfileHelp;
        private readonly TLGeometryBuffer _Buffer = new TLGeometryBuffer(0xffff);
        private readonly int _Capacity = 0x200000;

        public TLGeometryHelper()
        {
            _BfileHelp = new BFileHelp(_Capacity);
        }

        private Mesh GetMeshFromRPC(Element element)
        {
            var instance =
                element.get_Geometry(new Options()).FirstOrDefault(x => x is GeometryInstance) as
                    GeometryInstance;
            if (instance == null)
            {
                return null;
            }
            return instance.SymbolGeometry.FirstOrDefault(x => x is Mesh) as Mesh;
        }

        public static int getPolyMeshLength(PolymeshTopology node)
        {
            return node.NumberOfFacets*6 + node.NumberOfPoints*0x20;
        }

        public void OnEndElement()
        {
            WriteData();
        }

        public void OnFinish()
        {
            _BfileHelp.retStart();
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            OnPrePolymesh(node);
            _Buffer.OnPolymesh(node);
            if (_Buffer.GetAllLength() > 0x100000)
            {
                WriteData();
            }
        }

        public void OnPrePolyMatrial(int nMaterialId)
        {
            if (nMaterialId != _CurMaterialId)
            {
                if (_CurMaterialId != 0)
                {
                    WriteData();
                }
                _CurMaterialId = nMaterialId;
            }
        }

        public void OnPrePolymesh(PolymeshTopology node)
        {
            var num = _BfileHelp.getLength();
            var allLength = _Buffer.GetAllLength();
            var num3 = getPolyMeshLength(node);
            if (num + allLength + num3 > _Capacity)
            {
                _BfileHelp.retStart();
            }
        }

        public void OnRPC(Element element)
        {
            var meshFromRPC = GetMeshFromRPC(element);
            if (meshFromRPC != null)
            {
                _Buffer.OnMesh(meshFromRPC);
            }
        }

        public void WriteData()
        {
            _BfileHelp.addData(_Buffer);
            _Buffer.vertexCount = 0;
            _Buffer.triangleCount = 0;
        }
    }
}