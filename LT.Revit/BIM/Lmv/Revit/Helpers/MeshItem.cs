namespace BIM.Lmv.Revit.Helpers
{
    internal class MeshItem
    {
        public static int _nCurMesh;

        public MeshItem()
        {
            _nCurMesh++;
        }

        public MeshItem(string sName, string sMaterialId, string sAcc_Positon, string sAcc_Index, string sAcc_Normal,
            string sAcc_Texcoord)
        {
            Name = sName;
            MaterialId = sMaterialId;
            Acc_Positon = sAcc_Positon;
            Acc_Index = sAcc_Index;
            Acc_Normal = sAcc_Normal;
            Acc_Texcoord = sAcc_Texcoord;
            _nCurMesh++;
        }

        public string Acc_Index { get; set; }

        public string Acc_Normal { get; set; }

        public string Acc_Positon { get; set; }

        public string Acc_Texcoord { get; set; }

        public string Acc_Texcoord1 { get; set; }

        public string MaterialId { get; set; }

        public string Name { get; set; }

        public string ObjectId { get; set; }
    }
}