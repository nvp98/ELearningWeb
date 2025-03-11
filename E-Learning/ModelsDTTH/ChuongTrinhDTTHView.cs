using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDTTH
{
    public class ChuongTrinhDTTHView
    {
        public int IDCTDT { get; set; }
        public string TenChuongTrinhDT { get; set; }
        public string NoiDungTrichYeu { get; set; }
        public Nullable<int> ThoiLuongDT { get; set; }
        public string FileDinhKem { get; set; }
        public Nullable<int> IDPhuongPhapDT { get; set; }
        public Nullable<int> IDNhomNDDT { get; set; }
        public string TenPPDT { get; set; }
        public Nullable<int> IDPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string FileCTDT { get; set; }
        public Nullable<int> IDKiemTra { get; set; }
        public Nullable<int> ID_LVDT { get; set; }
        public string TenLVDT { get; set; }
        public Nullable<int> ID_NhomNL { get; set; }
        public string TenNhomNL { get; set; }
        public string DoiTuongDT { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> ID_NguoiTao { get; set; }
        public string HoTen_NT { get; set; }
        public string MaNhanVien_NT { get; set; }
        public Nullable<int> ID_NoiDungDT { get; set; }
        public Nullable<int> TinhTrang { get; set; }
        public string TenNoiDungDT { get; set; }
        public NoiDungDT noiDungDT { get; set; }
        public SH_KyDuyetCTDT sH_KyDuyetCTDTs { get; set; }
        public List<SH_ChiTietCTDT> sH_ChiTietCTDTs { get;set; }
        public List<CauHoiDeThiCTDTTH> cauHoiDeThiCTDTTHs { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
    public class CauHoiDeThiCTDTTH
    {
        public int ID { get; set; }
        public string MaDeThi { get; set; }
        public string TenDeThi { get; set; }
        public double? DiemChuan { get;set; }
        public int? ThoiGianLamBai { get; set; }
        public int? TongSoCau { get; set; }
        public HttpPostedFileBase FileDeThi { get; set; }
        public HttpPostedFileBase FileScanDeThi { get; set; }
        public string fileDe { get; set; }
    }
}