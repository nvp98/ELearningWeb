using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class ResultQTHDView
    {
        public int? IDNV { get; set; }
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public int? TongQTHD { get; set; }
        public int? SLHTQT { get; set; }
        public int? SLCHUAHTQT { get; set; }
        public int? IDVTKNL { get; set; }
        public string TenVTKNL { get; set; }
        public int? IDQTHD { get; set; }
        public int? IDPB { get; set; }
        public string TenPhongBan { get; set; }
        public string MaViTri { get; set; }
        public string TenQTHD { get;set; }
        public string MaHieu { get; set; }
        public int? IDDK { get; set; }
        public double? diem { get; set; }
        public int? TinhTrang { get; set; }
        public DateTime? NgaHT { get;set; }
        public DateTime? NgayKTTT { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public DateTime? NgayHetHieuLuc { get; set; }
        public int? TinhTrangHL { get; set; }
        public int? TinhTrangKT { get; set; }
    }
}