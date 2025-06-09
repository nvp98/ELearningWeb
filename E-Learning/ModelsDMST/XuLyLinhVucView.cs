using System.Collections.Generic;

namespace E_Learning.ModelsDMST
{
    public class XuLyLinhVucViewModel
    {
        public int ID { get; set; }
        public int ID_LinhVuc { get; set; }
        public string TenLinhVuc { get; set; }
        public List<PhongBanXuLyViewModel> PhongBans { get; set; }
        public List<XuLyItem> XuLyList { get; set; }
    }

    public class XuLyItem
    {
        public int ID_PhongBanXL { get; set; }
        public int ID_NhanVienXL { get; set; }
        public int ID_ChucVuXL { get; set; }
    }

    public class PhongBanXuLyViewModel
    {
        public string TenPhongBan { get; set; }
        public List<string> ChucVus { get; set; }
        //public List<string> NhanViens { get; set; }
        public List<NhanVienXuLyViewModel> NhanViens { get; set; } = new List<NhanVienXuLyViewModel>();
    }

    public class NhanVienXuLyViewModel
    {
        public string HoTen { get; set; }
        public List<string> ChucVus { get; set; } = new List<string>();
    }

}