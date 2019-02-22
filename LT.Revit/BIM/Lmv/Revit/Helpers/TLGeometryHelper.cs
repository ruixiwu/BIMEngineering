namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using System;
    using System.Linq;

    internal class TLGeometryHelper
    {
        private readonly BFileHelp _BfileHelp;
        private readonly TLGeometryBuffer _Buffer = new TLGeometryBuffer(0xffff);
        private int _Capacity = 0x200000;
        public static int _CurMaterialId = 0;
        public static string strMeshIds;

        public TLGeometryHelper()
        {
            this._BfileHelp = new BFileHelp(this._Capacity);
        }

        private Mesh GetMeshFromRPC(Element element)
        {
            GeometryInstance instance = element.get_Geometry(new Options()).FirstOrDefault<GeometryObject>(x => (x is GeometryInstance)) as GeometryInstance;
            if (instance == null)
            {
                return null;
            }
            return (instance.SymbolGeometry.FirstOrDefault<GeometryObject>(x => (x is Mesh)) as Mesh);
        }

        public static int getPolyMeshLength(PolymeshTopology node) => 
            ((node.NumberOfFacets * 6) + (node.NumberOfPoints * 0x20));

        public void OnEndElement()
        {
            this.WriteData();
        }

        public void OnFinish()
        {
            this._BfileHelp.retStart();
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            this.OnPrePolymesh(node);
            this._Buffer.OnPolymesh(node);
            if (this._Buffer.GetAllLength() > 0x100000)
            {
                this.WriteData();
            }
        }

        public void OnPrePolyMatrial(int nMaterialId)
        {
            if (nMaterialId != _CurMaterialId)
            {
                if (_CurMaterialId != 0)
                {
                    this.WriteData();
                }
                _CurMaterialId = nMaterialId;
            }
        }

        public void OnPrePolymesh(PolymeshTopology node)
        {
            int num = this._BfileHelp.getLength();
            int allLength = this._Buffer.GetAllLength();
            int num3 = getPolyMeshLength(node);
            if (((num + allLength) + num3) > this._Capacity)
            {
                this._BfileHelp.retStart();
            }
        }

        public void OnRPC(Element element)
        {
            Mesh meshFromRPC = this.GetMeshFromRPC(element);
            if (meshFromRPC != null)
            {
                this._Buffer.OnMesh(meshFromRPC);
            }
        }

        public void WriteData()
        {
            this._BfileHelp.addData(this._Buffer);
            this._Buffer.vertexCount = 0;
            this._Buffer.triangleCount = 0;
        }
    }
}

