using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class FGenResultValidation
    {
        public int? IDKQ { get; set; }
        public Nullable<int> IDNL { get; set; }
        public string TenNL { get; set; }
        public Nullable<int> IDNVDG { get; set; }
        public Nullable<int> DiemDG { get; set; }
        public DateTime ThangDG { get; set; }
        public DateTime NgayDG { get; set; }
        public string StrNgayDG { get; set; }
        public string Note { get; set; }

        public Nullable<int> IDLoaiNL { get; set; }
        public string TenLoaiNL { get; set; }
        public Nullable<int> IDVT { get; set; }
        public string TenViTri { get; set; }
        public Nullable<int> IDPB { get; set; }
        public Nullable<int> IDPX { get; set; }
        public Nullable<int> IDNhom { get; set; }
        public Nullable<int> IDTo { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> DinhMuc { get; set; }
        public Nullable<int> IsDanhGia { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public Nullable<int> OrderByLoai { get; set; }
        public int? Total { get; set; }
        public int? SumNL { get; set; }
        public int? TotalVuot { get; set; }
        public int? TotalDat { get; set; }
        public int? TotalKDat { get; set; }
        public int? TotalKDGia { get; set; }
        public Double? TileVuot { get; set; }
        public Double? TileDat { get; set; }
        public Double? TileKDat { get; set; }
        public Double? TileKDGia { get; set; }
        public string FilePath { get; set; }
    }
    public class GenKNLValidation
    {
        public int? TotalDG { get; set; }
        public int? TotalVuot { get; set; }
        public int? TotalDat { get; set; }
        public int? TotalKDat { get; set; }
        public int? TotalKDGia { get; set; }
        public int? TileVuot { get; set; }
        public int? TileDat { get; set; }
        public int? TileKDat { get; set; }
        public int? TileKDGia { get; set; }
        public  List<FGenResultValidation> ListGenNL { get; set; }
    }
}