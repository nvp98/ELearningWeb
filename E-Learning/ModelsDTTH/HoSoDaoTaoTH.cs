using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDTTH
{
    public class HoSoDaoTaoTH
    {
        public int ID { get; set; }
        public Nullable<int> LHID { get; set; }
        public string TenLopHoc { get; set; }
        public Nullable<System.DateTime> NgayBDThucTe { get; set; }
        public Nullable<System.DateTime> NgayKTThucTe { get; set; }
        public Nullable<int> ID_GiangVien { get; set; }
        public string HoTenGV { get; set; }
        public string MaGiangVien { get; set; }
        public string DonViGV { get; set; }
        public Nullable<double> ThoiLuongDT { get; set; }
        public Nullable<int> ID_NguoiNopHS { get; set; }
        public string TenNguoiNop { get; set; }
        public Nullable<System.DateTime> NgayNopHS { get; set; }
        public int? TinhTrang { get; set; }
        public Nullable<int> ID_NguoiXuLy { get; set; }
        public string TenNguoiXuLy { get; set; }
        public Nullable<System.DateTime> NgayXuLy { get; set; }
        public List<FileScanHoSoView> fileScanHoSoViews { get; set; }
        public ManageClassValidation manageClassValidation { get; set; }
    }

    public class FileScanHoSoView
    {
        public int ID { get; set; }
        public int IDLH { get; set; }
        public HttpPostedFileBase FileDinhKem { get; set; }
        public string LinkFile { get; set; }
        public string TenFile { get; set; }
    }
}