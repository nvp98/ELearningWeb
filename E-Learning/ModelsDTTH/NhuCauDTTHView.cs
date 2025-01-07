using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDTTH
{
    public class ChiTietNhuCauDTTHView
    {
        public int ID { get; set; }
        public Nullable<int> Nam { get; set; }
        public Nullable<int> Quy { get; set; }
        public Nullable<int> NguoiNhan { get; set; }
        public string TenNoiDungDT { get; set; }
        public Nullable<int> BoPhanLNC_ID { get; set; }
        public string TenBoPhan { get; set; }
        public Nullable<int> Nhom_ID { get; set; }
        public string TenNhom { get; set; }
        public Nullable<int> NoiDung_ID { get; set; }
        public string DoiTuongDT { get; set; }
        public Nullable<int> SoLuongNguoi { get; set; }
        public Nullable<int> NguonGV_ID { get; set; }
        public Nullable<int> GiangVien_ID { get; set; }
        public string TenGiangVien { get; set; }
        public string MaNV { get; set; }
        public string TenViTri { get; set; }
        //public Nullable<int> ViTri_ID { get; set; }
        public Nullable<int> BoPhan_ID { get; set; }
        public string DonViDT { get; set; }
        public Nullable<int> PhuongPhapDT_ID { get; set; }
        public string TenPPDT { get; set; }
        public Nullable<int> LVDT_ID { get; set; }
        public string TenLVDT { get; set; }
        public Nullable<int> ThoiGianDT { get; set; }
        public Nullable<int> ThoiLuong { get; set; }
        public Nullable<int> LoaiNhacLai_ID { get; set; }
        public Nullable<int> DinhKyNhacLai_ID { get; set; }
        public string DiaDiemDT { get; set; }
        public string DonVi_ThueNgoai { get; set; }
        public string GhiChu { get; set; }
        public string LyDoDTNhacLai { get; set; }
        public Nullable<int> LoaiNCDT_ID { get; set; }
        public Nullable<DateTime> NgayTao { get; set; }
        public List<int> ListVTKNL { get; set; }
        //public List<ViTriKNL> ViTriKNLs { get; set; }
    }

    public class NhuCauDTTHView
    {
        public int ID_NCDT { get; set; }
        public Nullable<int> Nam { get; set; }
        public Nullable<int> Quy { get; set; }
        public Nullable<int> TrinhKy_ID { get; set; }
        public string TenTrinhKy { get; set; }
        public Nullable<int> BoPhanLNC_ID { get; set; }
        public string TenBoPhan_LNC { get; set; }
        public Nullable<int> NoiDungDT_ID { get; set; }
        public string TenNoiDungDT { get; set; }
        public string FileDinhKem { get; set; }

        public int? PhanLoaiNCDT_ID { get; set; }
        public string TenLoai_NCDT { get; set; }
        public Nullable<int> PhuongPhapDT_ID { get; set; }
        public string TenPPDT { get; set; }

        public string NguoiTao { get; set; }
        public int ID_NguoiTao { get; set; }
        public DateTime NgayTao { get;set; }

        public Nullable<int> MaDinhKy { get; set; }
        public string TenDinhKy { get; set; }
        public Nullable<int> TinhTrang { get; set; }

        public string DSNhanVien { get; set; }
        public string DSViTri { get; set; }

        public ChiTietNhuCauDTTHView chiTietNhuCauDTTHView { get; set; }
        public CapDuyetView CapDuyetView { get; set; }    

        public HttpPostedFileBase File { get; set; }
    }

}