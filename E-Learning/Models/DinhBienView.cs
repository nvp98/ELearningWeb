using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class DinhBienView
    {
        public int IDVT { get; set; }
        public int? IDDB { get; set; }
        public string TenViTri { get; set; }
        public string TenViTriSe { get; set; }
        public Nullable<int> IDPB { get; set; }
        public string MaViTri { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> IDKhoi { get; set; }
        public string TenKhoi { get; set; }
        public Nullable<int> IDPX { get; set; }
        public string TenPX { get; set; }
        public Nullable<int> IDNhom { get; set; }
        public string TenNhom { get; set; }
        public Nullable<int> IDTo { get; set; }
        public string TenTo { get; set; }
        public string FilePath { get; set; }
        public Nullable<int> CountNV { get; set; }
        public Nullable<int> CountKNL { get; set; }
        public Nullable<int> CountNVDDG { get; set; }
        public string NhapBMTCV { get; set; }
        public int? DinhBienNS { get; set; }
        public string NSTapSu { get; set; }
        public bool isNSTapSu { get; set; }
        public string ChuaCoNS { get; set; }
        public bool isChuaCoNS { get; set; }
        public string NSVTKhac { get; set; }
        public bool isNSVTKhac { get; set; }
        public DateTime thoigianBoNhiem { get; set; }
        public string ghichu { get; set; }
        public DateTime NgayTao { get; set; }
        public int ThuaThieu { get; set; }  
    }
}