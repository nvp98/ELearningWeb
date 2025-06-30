using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace E_Learning.ModelsDMST
{
    public class PhieuDangKyView
    {
        public int ID { get; set; }

        public int ID_LinhVuc { get; set; }

        public int ID_NguoiTao { get; set; }

        public DateTime? NgayTao { get; set; }

        public int? TinhTrang { get; set; }

        public string TenYTuong { get; set; }

        public int? ID_NhanVienDaiDien { get; set; }

        public DateTime? NgayDeXuat { get; set; }

        public string NoiDungYTuong { get; set; }

        public string LyDoThucHien { get; set; }

        public string ViTriTrienKhai { get; set; }

        public DateTime? TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }

        public DateTime? ThoiGianTrienKhai { get; set; }

        [AllowHtml]
        public string CachThucThucHien { get; set; }

        public int? ThucHienKhi { get; set; }

        public int CanThuNghiemThem { get; set; }

        public string CanHoTro { get; set; }

        [AllowHtml]
        public string HieuQuaKyVong { get; set; }

        public string DeXuatNguoiBoPhan { get; set; }

        public List<ChiPhiDuKienModel> KinhPhiDuKien { get; set; }

        public List<NguoiThamGiaModel> NguoiThamGia { get; set; }

        public string NguoiThamGiaJson { get; set; }

        public KeHoachThucHienModel KeHoachThucHien { get; set; }

        public List<NhanVienThucHienModel> NhanVienThucHien { get; set; }
    }

    public class ChiPhiDuKienModel
    {
        public string TenNguonLuc { get; set; }
        public float? ChiPhi { get; set; }
        public string NguonThongTin { get; set; }
        public string GhiChu { get; set; }
    }

    public class NguoiThamGiaModel
    {
        public int? ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string VaiTro { get; set; }
    }

    public class KeHoachThucHienModel
    {
        public int ID_Phieu { get; set; }

        public DateTime? NgayBatDauTrienKhai { get; set; }

        public DateTime? NgayDuKienHoanThanh { get; set; }

        public string MucTieuThucHien { get; set; }

        public double ChiPhiDuKien { get; set; }

        public string ThuanLoiKhoKhan { get; set; }

        public string DeNghiHoTro { get; set; }
    }

    public class NhanVienThucHienModel
    {
        public int ID_Phieu { get; set; }

        public int ID_NhanVien { get; set; }

        public string NoiDungCongViec { get; set; }

        public int SoLuong { get; set; }

        public string ThoiGian { get; set; }

        public string GhiChu { get; set; }
    }
}