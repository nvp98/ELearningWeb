using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsBangMTCV
{
    public class ChamDiemViewModel
    {
        public int NhanVienID { get; set; }
        public string HoTen { get; set; }
        public string MaNhanVien { get; set; }
        public string ViTriCongViec { get; set; }
        public string KetQuaTracNghiem { get; set; }
        public List<ChamDiemTuLuanModel> CauHoiTuLuan { get; set; } = new List<ChamDiemTuLuanModel>();
    }

    public class ChamDiemTuLuanModel
    {
        public string CauHoi { get; set; }
        public string CauTraLoi { get; set; }
        public decimal? Diem { get; set; }
    }

    public class ChamDiemSaveModel
    {
        public int NhanVienID { get; set; }
        public int CauHoiIndex { get; set; }
        public decimal Diem { get; set; }
    }

    public class KetQuaBaiLamViewModel
    {
        public string TenDe { get; set; }
        public DateTime NgayLam { get; set; }
        public decimal TongDiem { get; set; }
        public List<KetQuaChiTietModel> ChiTietList { get; set; }
    }

    public class KetQuaChiTietModel
    {
        public string CauHoi { get; set; }
        public string CauTraLoi { get; set; }
        public bool LaTracNghiem { get; set; }
        public bool DungSai { get; set; }
        public decimal? Diem { get; set; }
    }
}