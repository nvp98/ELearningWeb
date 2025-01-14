using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class FCheckValidation
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
        public string NgayDG { get; set; }
        public string fileBMTCV { get; set; }
        public DateTime NgayHanDG { get; set; }
        public string ThangDG { get; set; }
        public int? NgayCanhBao { get; set; }
        public int? Total { get; set; }
        public int? TotalVuot { get; set; }
        public int? TotalDat { get; set; }
        public int? TotalKDat { get; set; }
        public int? TotalKDGia { get; set; }
        public int? TotalChuaDG { get; set; }
        public int? TotalQTHD { get; set; }
        public int? TotalHTQTHD { get; set; }

    }

    public class FValueValidation
    {
        public int? IDKQ { get; set; }
        public Nullable<int> IDNV { get; set; }
        public string TenNV { get; set; }
        public Nullable<int> IDNL { get; set; }
        public string TenNL { get; set; }
        public Nullable<int> IDNVDG { get; set; }
        public string TenNVDG { get; set; }
        public Nullable<int> DiemDG { get; set; }
        public DateTime ThangDG { get; set; }
        public DateTime NgayHanDG { get; set; }
        public DateTime NgayDG { get; set; }
        public Nullable<int> NgayCanhBao { get; set; }
        public string StrNgayDG { get; set; }
        public string Note { get; set; }

        public Nullable<int> IDLoaiNL { get; set; }
        public string TenLoaiNL { get; set; }
        public Nullable<int> IDVT { get; set; }
        public string TenViTri { get; set; }
        public Nullable<int> IDPB { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> DinhMuc { get; set; }
        public Nullable<int> IsDanhGia { get; set; }
        public string ColorKQ { get; set; }
        public string ColorDM { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public Nullable<int> OrderByLoai { get; set; }
        public bool CapNhatDG { get; set; }
        public Nullable<int> DiemCBNVDG { get; set; }
        public Nullable<int> DiemDGLan1 { get; set; }
        public Nullable<int> DiemPheDuyetDG { get; set; }
        public DateTime? NgayCBNVDG { get; set; }
    }


    public class FExportKNLValidation
    {
        public string TenPhongBan { get; set; }
        public string TenViTri { get; set; }
        public List<LoaiKNL> LSLoaiKNL { get; set; }
        public List<FValueValidation> LSFValue = new List<FValueValidation>();
    }

    public class FViewValidation
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
        public string NgayDG { get; set; }
        public DateTime? ThangDG { get; set; }
        public string fileBMTCV { get; set; }

        public int? Total { get; set; }
        public int? TotalVuot { get; set; }
        public int? TotalDat { get; set; }
        public int? TotalKDat { get; set; }
        public int? TotalKDGia { get; set; }

    }

}