using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class KhungNangLucValidation
    {
        public int IDNL { get; set; }
        public string TenNL { get; set; }
        public Nullable<int> IDLoaiNL { get; set; }
        public string TenLoaiNL { get; set; }
        public Nullable<int> IDVT { get; set; }
        public string TenViTri { get; set; }
        public Nullable<int> IDPB { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> DinhMuc { get; set; }
        public Nullable<int> IsDanhGia { get; set; }
        public bool DanhGia { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public Nullable<int> OrderByLoai { get; set; }

    }
    public class KNLDGValidation
    {
        public int IDNL { get; set; }
        public string TenNL { get; set; }
        public Nullable<int> IDLoaiNL { get; set; }
        public string TenLoaiNL { get; set; }
        public Nullable<int> IDVT { get; set; }
        public string TenViTri { get; set; }
        public Nullable<int> IDPB { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> DinhMuc { get; set; }
        public Nullable<int> IsDanhGia { get; set; }
        public string MaNV { get; set; }
        public string NVDG { get; set; }
        public Nullable<int> DiemDG { get; set; }
        public string Note { get; set; }

        public DateTime NgayDG { get; set; }
        public int IDKQ { get; set; }
        public DateTime ThangDG { get; set; }
        public string MauKQ { get; set; }
        public string MauDM { get; set; }
    }


}