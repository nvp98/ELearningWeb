using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDMST
{
    public class ThanhVienView
    {
        public int ID { get; set; }
        public int NhanVienID { get; set; }

        public string MaNhanVien { get; set; }

        public string HoTen { get; set; }

        public int BoPhanID { get; set; }

        public string BoPhan { get; set; }

        public string ViTriCongViec { get; set; }

        public int ChucVuID { get; set; }

        public string ChucVuTrongTieuBan { get; set; }

        public string Email { get; set; }

        public string GhiChu { get; set; }

        public List<ThanhVienChiTiet> ThanhVienList { get; set; }
    }

    public class ThanhVienChiTiet
    {
        public int NhanVienID { get; set; }
        public int ChucVuID { get; set; }
        public string Email { get; set; }
    }
}