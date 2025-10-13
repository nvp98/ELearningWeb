using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.ModelsDMST
{
    public class DeTaiDMSTView
    {
        public int ID { get; set; }
        public string TenDeTai { get; set; }
        public int LinhVucID { get; set; }
        public IEnumerable<SelectListItem> LinhVucList { get; set; }
        public int PhamViApDung { get; set; }
        public int? DonViSelect { get; set; }
        public IEnumerable<SelectListItem> PhongBanList { get; set; }
        public List<int> PhongBanThamGiaIDs { get; set; }
        public List<int> PhongBanApDungIDs { get; set; }
        public string MoTaNgan { get; set; }
        public HttpPostedFileBase TepDinhKem { get; set; }
        public int NguoiDangKyID { get; set; }
        public string BoPhanThamGia { get; set; }
        public int TrangThai { get; set; }
        public List<DeTaiListItemViewModel> DanhSachDeTai { get; set; }
    }

    public class DeTaiListItemViewModel
    {
        public string TenDeTai { get; set; }
        public string BoPhanThamGia { get; set; }
        public string BoPhanApDung { get; set; }
        public int TrangThai { get; set; }
        public string TepDinhKem { get; set; }
    }
}