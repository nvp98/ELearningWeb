using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDMST
{
    public class DeTaiDMSTView
    {
        public int ID { get; set; }
        public string TenYTuong { get; set; }
        public string NoiDungYTuong { get; set; }
        public string ViTriTrienKhai { get; set; }
        public string HieuQuaKyVong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int ID_NhanVienDaiDien { get; set; }
    }
}