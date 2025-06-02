using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDMST
{
    public class DeXuatDMSTView
    {
        public int ID { get; set; }
        public int ID_HoatDong { get; set; }
        public string TenHoatDong { get;set; }
        public int ID_LinhVuc { get; set; }
        public string TenLinhVuc { get; set; }
        public int ID_NguoiTao { get; set; }
        public string TenNguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public int TinhTrang { get; set; }
        public DeXuat_ChiTiet_DMSTView DeXuat_ChiTiet_DMSTView { get; set; }
        public List<FileDinhKemDMSTView> ListfileDinhKemDMSTViews { get; set; }
        public List<HieuQuaKinhTeDMSTView> ListHieuQuaKinhTeDMSTViews { get; set; }
        public List<NguonLucDMSTView> ListNguonLucDMSTView { get; set; }
    }
    public class DeXuat_ChiTiet_DMSTView
    {
        public int IDCT { get; set; }
        public string TenDeTai { get; set; }
        public DateTime ThoiGian_ApDung { get; set; }
        public int ID_PhamVi_ApDung { get; set; }
        public int ID_TinhTrang_ApDung { get; set; }
        public int TinhTrang_TacGia { get; set; }
        public string ThucTrang { get; set; }
        public string CachThucThucHien { get; set; }
        public string VanDeGapPhai { get; set; }
        public string UuDiem { get; set; }
        public string NhuocDiem { get; set; }
        public string ThuanLoi { get; set; }
        public string KhoKhan { get; set; }
        public int ID_TinhTrangHieuQua { get; set; }
        public string LoiIchXaHoi { get; set; }
        public string DeXuatKienNghi { get; set; }
        public int TinhTrang_CamKet { get; set; }
    }
    public class FileDinhKemDMSTView
    {
        public int ID { get; set; }
        public string FileDinhKem { get; set; }
        public string GhiChu { get; set; }
        public string TenFileDinhKem { get; set; }
        public int ID_DeXuat { get; set; }
        public string FileHinhAnh { get; set; }
    }
    public class HieuQuaKinhTeDMSTView
    {
        public int ID { get; set; }
        public string NoiDung { get; set; }
        public string GiaTri { get; set; }
        public string NguonThongTin { get; set; }
        public string GhiChu { get; set; }
        public int ID_DeXuat { get; set; }
    }
    public class NguonLucDMSTView
    {
        public int ID { get; set; }
        public string TenNguonLuc { get; set; }
        public double ChiPhi { get; set; }
        public string NguonThongTin { get; set; }
        public string GhiChu { get; set; }
        public int ID_DeXuat { get; set; }
    }
}