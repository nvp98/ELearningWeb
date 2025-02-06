using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class FResultValidation
    {
        public int? IDNV { get; set; }
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string DGThang { get; set; }
        public DateTime DGThangDate { get; set; }
        public int? Total { get; set; }
        public int? TotalVuot { get; set; }
        public int? TotalDat { get; set; }
        public int? TotalKDat { get; set; }
        public int? TotalKDGia { get; set; }
        public int? TotalChuaDGia { get; set; }

        public int? TotalVuotTu { get; set; }
        public int? TotalDatTu { get; set; }
        public int? TotalKDatTu { get; set; }
        public int? TotalKDGiaTu { get; set; }
        public int? TotalChuaDGiaTu { get; set; }

        public int? IDVT { get; set; }
        public string FilePath { get; set; }
        public string TenViTri { get; set; }
    }
}