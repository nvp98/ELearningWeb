using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDTTH
{
    public class TrinhKyDTTHView
    {
        public int ID { get; set; }
        public Nullable<int> NCDT_ID { get; set; }
        public Nullable<int> NguoiDuyet_ID { get; set; }
        public Nullable<int> CapDuyet { get; set; }
        public Nullable<int> TinhTrangDuyet { get; set; }
    }
    public class SH_KyDuyetNCDTView
    {
        public int ID { get; set; }
        public Nullable<int> NCDT_ID { get; set; }
        public Nullable<int> ID_Duyet { get; set; }
        public Nullable<int> NguoiDuyet_ID { get; set; }
        public Nullable<int> CapDuyet { get; set; }
        public Nullable<int> TinhTrangDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string HoTen_NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string GhiChu { get; set; }
    }
    public class CapDuyetView
    {
        //public int ID { get; set; }
        public Nullable<int> BPSD { get; set; }
        public Nullable<int> PCHN { get; set; }
        public Nullable<int> PNS { get; set; }
        public Nullable<int> BGD { get; set; }
    }
    public class SH_KyDuyetCTDTView
    {
        public int ID { get; set; }
        public Nullable<int> ID_CTDT { get; set; }
        public string NoiDungTrichYeu { get; set; }
        public string NoiDungCTDT { get; set; }
        public Nullable<int> ID_NguoiTao { get; set; }
        public string TenNguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> ID_NguoiKiemTra { get; set; }
        public string TenNguoiKiemTra { get; set; }
        public Nullable<int> ID_NguoiDangNDDT { get; set; }
        public string TenNguoiDangNDDT { get; set; }
        public Nullable<System.DateTime> NgayDangNDDT { get; set; }
        public Nullable<System.DateTime> NgayKTDuyet { get; set; }
        public Nullable<int> ID_TPBP { get; set; }
        public string TenTPBP { get; set; }
        public Nullable<System.DateTime> NgayTPBP { get; set; }
        public Nullable<int> ID_PCHN { get; set; }
        public string TenPCHN { get; set; }
        public Nullable<System.DateTime> NgayPCHN { get; set; }
        public Nullable<System.DateTime> NgayDuyetNDDT { get; set; }
        public Nullable<int> ID_NguoiDuyetNDDT { get; set; }
        public string TenNguoiDuyetNDDT { get; set; }
        public Nullable<int> ID_TinhTrangCTDT { get; set; }
        public Nullable<int> IsDuyet { get; set; }
        public Nullable<int> CapDuyet { get; set; }
        public NoiDungDT noidungdt { get; set; }
    }
}