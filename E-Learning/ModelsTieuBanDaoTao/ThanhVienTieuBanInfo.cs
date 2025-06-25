using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsTieuBanDaoTao
{
    public class ThanhVienTieuBanInfo
    {
        public int Id { get; set; }
        public int MaViTriKNL { get; set; }
        public string TenViTriKNL { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public int ViTriTieuBan_ID { get; set; }
        public string TenViTriTieuBan { get; set; }
        public DateTime NgayCapNhatGanNhat { get; set; }
        public DateTime NgayDenHanCapNhat { get; set; }
        public int TrangThai { get; set; }
        public int PhongBanID { get; set; }
        public DateTime? NgayPheDuyet { get; set; }
        public string HoTenNguoiTrinhKy { get; set; }
        public string MaNhanVienNguoiPheDuyet { get; set; }
        public string HoTenNguoiPheDuyet { get;set; }
        public string HoTenNguoiThem { get; set; }
        public string HoTenNguoiSua { get; set; }
    } 

}