using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsBangMTCV
{
    public class DeKiemTraModel
    {
        public int ID { get; set; }
        public string TenDe { get; set; }
        public string GhiChu { get; set; }
        public List<CauHoiModel> CauHoiList { get; set; } = new List<CauHoiModel>();
        public DateTime? NgayDenHan { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool LaDeChinhThuc { get; set; }
    }

    public class CauHoiModel
    {
        public int CauHoiID { get; set; }
        public string LoaiCauHoi { get; set; }
        public string CauTraLoi { get; set; }
        public string NoiDung { get; set; }
        public List<LuaChonModel> LuaChonList { get; set; } = new List<LuaChonModel>();
        public bool LaTracNghiem { get; set; }
        public bool DungSai { get; set; }
    }

    public class LuaChonModel
    {
        public int LuaChonID { get; set; }
        public string NoiDung { get; set; }
        public bool LaDapAnDung { get; set; }
    }

}