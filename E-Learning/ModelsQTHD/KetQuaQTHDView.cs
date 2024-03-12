using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsQTHD
{
    public class KetQuaQTHDView
    {
        public int IDNV { get; set; }
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public Nullable<int> IDVT { get; set; }
        public Nullable<int> IDNhom { get; set; }
        public Nullable<int> IDTo { get; set; }
        public Nullable<int> IDPX { get; set; }
        public string TenVT { get; set; }
        public Nullable<int> IDPB { get; set; }
        public string TenPB { get; set; }
        public Nullable<int> IDVTNDG { get; set; }
        public Nullable<int> IDKip { get; set; }
        public string TenKip { get; set; }
        public string MaViTri { get; set; }

        public Nullable<int> SL_QT { get; set; }
        public Nullable<int> SL_HoanThanh { get; set; }

        public List<ListTestQTView> ListTest { get; set; }

    }
}