using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class ViTriKNLValidation
    {
        public int IDVT { get; set; }
        public string TenViTri { get; set; }
        public Nullable<int> IDPB { get; set; }
        public string MaViTri { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> IDKhoi { get; set; }
        public string TenKhoi { get; set; }
        public Nullable<int> IDPX { get; set; }
        public string TenPX { get; set; }
        public Nullable<int> IDNhom { get; set; }
        public string TenNhom { get; set; }
        public Nullable<int> IDTo { get; set; }
        public string TenTo { get; set; }
        public string FilePath { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
        public Nullable<int> CountNV { get; set; }
        public Nullable<int> CountKNL { get; set; }

        public Nullable<int> CountDGTC { get; set; }

        public Nullable<int> IDVTParent { get; set; }
        public Nullable<int> CountNVDDG { get; set; }
        public Nullable<int> TinhTrang { get; set; }
        public Nullable<int> CountSLNDDT { get; set; }
    }
    public class KNLDGiaTCValidation
    {
        public int ID { get; set; }
        public int IDVT { get; set; }
        public string TenViTri { get; set; }
        public Nullable<int> IDPB { get; set; }
        public string MaViTri { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> IDVTDGTT { get; set; }
        public string tenVTTT { get; set; }
        public Nullable<int> IDVTDGTC { get; set; }

        public string tenVTTC { get; set; }

    }
    public class SetPermisionKNLValidation
    {
        public int IDVT { get; set; }
        public string TenViTri { get; set; }
        public Nullable<int> IDPB { get; set; }
        public string MaViTri { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> IDKhoi { get; set; }
        public string TenKhoi { get; set; }
        public Nullable<int> IDPX { get; set; }
        public string TenPX { get; set; }
        public Nullable<int> IDNhom { get; set; }
        public string TenNhom { get; set; }
        public Nullable<int> IDTo { get; set; }
        public string TenTo { get; set; }
        public string FilePath { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
        public Nullable<int> CountNV { get; set; }
        public Nullable<int> CountKNL { get; set; }
        public Nullable<int> IDVTParent { get; set; }
        public List<ViTriKNLValidation> ListVTAuto { get; set; }
        public List<ViTriKNLValidation> ListVTTT { get; set; }
        public List<ViTriKNLValidation> ListVTSet { get; set; }
        public Nullable<int> CountDGTT { get; set; }
        public Nullable<int> CountDGTC { get; set; }
        public Nullable<int> CountDGAuto { get; set; }
    }

    public class ExportNhanVienKNL
    {
        public int IDNV { get; set; }
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public Nullable<int> IDVT { get; set; }
        public string TenViTri { get; set; }
        public string TenPhongBan { get; set; }
        public string TenPhongBanKNL { get; set; }
        public string TenKhoi { get; set; }
        public string TenPX { get; set; }
        public string TenNhom { get; set; }
        public string TenTo { get; set; }
        public Nullable<int> IDNhom { get; set; }
        public Nullable<int> IDTo { get; set; }
        public Nullable<int> IDPX { get; set; }
        public Nullable<int> IDPB { get; set; }
        public Nullable<int> IDVTNDG { get; set; }
        public Nullable<int> IDKip { get; set; }
        public string TenKip { get; set; }
        public string MaViTri { get; set; }
    }

    public class ExportBoPhanKNL
    {
        public string TenPhongBan { get; set; }
        public Nullable<int> TotalVT { get; set; }
        public Nullable<int> VTDDG { get; set; }
        public Nullable<int> TotalNV { get; set; }
        public Nullable<int> NVDDG { get; set; }
        public string MaPhongBan { get; set; }
    }

    public class ExportNhanVienKQKNL
    {
        public int IDNV { get; set; }
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public Nullable<int> IDVT { get; set; }
        public string TenViTri { get; set; }
        public string TenPhongBan { get; set; }
        public string TenKhoi { get; set; }
        public string TenPX { get; set; }
        public string TenNhom { get; set; }
        public string TenTo { get; set; }
        public Nullable<int> IDNhom { get; set; }
        public Nullable<int> IDTo { get; set; }
        public Nullable<int> IDPX { get; set; }
        public Nullable<int> IDPB { get; set; }
        public Nullable<int> IDVTNDG { get; set; }
        public Nullable<int> IDKip { get; set; }
        public string TenKip { get; set; }
        public string MaViTri { get; set; }
        public Nullable<int> TotalNL { get; set; }
        public Nullable<int> KDAT { get; set; }
        public Nullable<int> DAT { get; set; }
        public Nullable<int> VUOT { get; set; }
        public Nullable<int> KDGIA { get; set; }
        public Nullable<int> CHUADG { get; set; }
        public string NgayDG { get; set; }
        public string HanDG { get; set; }
        public string NgayHanDG { get; set; }
    }

    public class ExportViTriKNL
    {
        public int IDVT { get; set; }
        public string TenViTri { get; set; }
        public string MaViTri { get; set; }
        public string NhapBMTCV { get; set; }
        public Nullable<int> IDPB { get; set; }
        public string TenPhongBan { get; set; }
        public string TenKhoi { get; set; }
        public string TenPX { get; set; }
        public string TenNhom { get; set; }
        public string TenTo { get; set; }
        public string TenNL { get; set; }
        public Nullable<int> IDLoaiNL { get; set; }
        public string TenLoaiNL { get; set; }
        public Nullable<int> SLDinhMucChung { get; set; }
        public Nullable<int> SLisDanhGiaChung { get; set; }
        public Nullable<int> SLDinhMucCMon { get; set; }
        public Nullable<int> SLisDanhGiaCMon { get; set; }

        public Nullable<int> SLDinhMucQLy { get; set; }
        public Nullable<int> SLisDanhGiaQLy { get; set; }
        public Nullable<int> CountChung { get; set; }
        public Nullable<int> CountCMon { get; set; }
        public Nullable<int> CountQLy { get; set; }
        public Nullable<int> CountTotal { get; set; }
        public Nullable<int> CountNV { get; set; }
        public Nullable<int> CountVTTT { get; set; }
        public Nullable<int> CountVTXem { get; set; }
        public Nullable<int> CountNVDGTT { get; set; }
        public Nullable<int> SLNVDuocDG { get; set; }
        public Nullable<int> SLNVKDuocDG { get; set; }
        public Nullable<int> CountVTAuto { get; set; }
        public Nullable<int> TinhTrang { get;set; }
    }
    public class AddNL
    {
        public string NLuc { get; set; }
        public string DinhMuc { get; set; }
        public string IsDanhGia { get; set; }
        public string IDLoai { get; set; }
    }
    public class NVKNVTValidation
    {
        public int ID { get; set; }
        public int IDNV { get; set; }
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public Nullable<int> IDVT { get; set; }
        public Nullable<int> IDNhom { get; set; }
        public Nullable<int> IDTo { get; set; }
        public Nullable<int> IDPX { get; set; }
        public string TenVT { get; set; }
        public Nullable<int> IDPB { get; set; }
        public Nullable<int> IDVTKN { get; set; }
        public Nullable<int> IDKip { get; set; }
        public string TenVTKN { get; set; }
        public string MaViTri { get; set; }
    }
    public class NVKNSelect
    {
        public int? IDNV { get; set; }
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public string NVDG { get; set; }
        public Nullable<int> IDKNL { get; set; }
        public string TenKNL { get; set; }
        public int[] Selected { get; set; }
        public int[] SelectedKN { get; set; }
    }
}