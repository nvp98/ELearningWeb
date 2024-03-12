using DocumentFormat.OpenXml.Wordprocessing;
using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace E_Learning.ModelsQTHD
{
    public class NoiDungQTView
    {
        public int IDQTHD { get; set; }

        [Required(ErrorMessage = "Nhập Mã hiệu")]
        public string MaHieu { get; set; }
        [Required(ErrorMessage = "Nhập tên QT/HD")]
        public string TenQTHD { get; set; }
        public Nullable<int> IDLoaiQTHD { get; set; }
        public string TenLoaiQTHD { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Phòng ban")]
        public Nullable<int> IDPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn LVĐT")]
        public Nullable<int> IDLVDT { get; set; }
        public string TenLVDT { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày hiệu lực")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayHieuLuc { get; set; }
        //[Required(ErrorMessage = "Vui lòng chọn ngày hết hiệu lực")]
        //[DataType(DataType.Date, ErrorMessage = "Date only")]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayHetHieuLuc { get; set; }
        public Nullable<double> DiemChuan { get; set; }
        public Nullable<int> LanCapNhat { get; set; }
        public Nullable<int> LanCapNhatOld { get; set; }
        public string NoiDungCapNhat { get; set; }
        public Nullable<int> TinhTrang { get; set; }
        public Nullable<int> TinhTrangHL { get; set; }
        public bool isActive { get; set; }
        public Nullable<int> IDDinhKy { get; set; }
        public string TenDinhKy { get; set; }
        public Nullable<int> SL_VTCVLQ { get; set; }
        public Nullable<int> SL_NVHT { get; set; }
        public Nullable<int> Total_SL_NV { get; set; }
        public Nullable<int> SL_CauHoi { get; set; }

        public int[] SelectedQT { get; set; }

        public List<QT_FileQT> List_FileQT { get; set; }
        public List<NoiDungQTLienQuan> List_VanBanLQ { get; set; }
        [Required(ErrorMessage = "Please select file.")]
        public HttpPostedFileBase FileUpload { get; set; }

        [Required(ErrorMessage = "Please select file.")]
        public List<HttpPostedFileBase> FileUploads { get; set; }
        public HttpPostedFileBase[] files { get; set; }
    }
    public class NoiDungQTLienQuan
    {
        public int IDQTHD { get; set; }
        public string MaHieu { get; set; }
        public string TenQTHD { get; set; }

        public DateTime NgayHieuLuc { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }

        public Nullable<int> TinhTrangHL { get; set; }
        public Nullable<int> TinhTrangQT { get; set; }

    }
    public class NoiDungPhanQuyenView
    {
        public int IDQTHD { get; set; }
        public string MaHieu { get; set; }
        public string TenQTHD { get; set; }
        public Nullable<int> IDDinhKy { get; set; }

        public string TenDinhKy { get; set; }
        public int? IDPhanQuyen { get; set; }
        public int? IDVTKNL { get; set; }

        public int? TinhTrangQT { get; set; }
        public Nullable<int> TinhTrangHL { get; set; }

    }

}