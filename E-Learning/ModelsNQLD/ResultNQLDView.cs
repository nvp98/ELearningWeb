using E_Learning.ModelsQTHD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsNQLD
{
    public class ResultNQLDView
    {
        public int IDNV { get; set; }
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public int IDPhongBan { get; set; }
        public string PhongBan { get; set; }

        public string TenViTri { get; set; }
        public List<KQuaNQ> ListKQuaNQLD { get; set; }

    }
    public class KQuaNQ
    {
        public int? NDDTID { get; set; }
        public string TenNDDT { get; set; }

        public int? IDChuong { get; set; }
        public int? XNTG { get; set; }
        public int? XNHT { get; set; }
        public int? XNHTFile { get; set; }
        public DateTime? NgayTG { get; set; }
        public DateTime? NgayHT { get; set; }
        public int? TinhTrang { get; set; }
    }
}