using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsKCCD
{
    public class SuggestKCCDView
    {
        public int ID { get; set; }
        public Nullable<int> NoiDungKCCDID { get; set; }
        public string TenNDDT { get; set; }
        public Nullable<int> LinhVucID { get; set; }
        public string TenLVDT { get; set; }
        public Nullable<int> NhomNangLucID { get; set; }
        public string TenNhomNL { get; set; }
        public Nullable<int> PhongBanID { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> HuongDan1 { get; set; }
        public string HoTen1 { get; set; }
        public string MaNV1 { get; set; }
        public Nullable<int> ViTriID1 { get; set; }
        public string TenViTri1 { get; set; }
        public Nullable<int> HuongDan2 { get; set; }
        public string HoTen2 { get; set; }
        public string MaNV2 { get; set; }
        public Nullable<int> ViTriID2 { get; set; }
        public string TenViTri2 { get; set; }
        public DateTime NgayTao { get; set; }
        public Nullable<int> TinhTrang { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public DateTime NgayXN { get; set; }
        public int? SLHV { get; set; }
        public int? SLHVHT { get; set; }
        public int? TinhTrangHVXN { get; set; }
        public bool isKiemTra { get; set; }
        public Nullable<int> DeThiID { get; set; }
        public int? TinhTrangThi { get; set; }
    }
}