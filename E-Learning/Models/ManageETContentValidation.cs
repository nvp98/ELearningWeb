using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class ManageETContentValidation
    {
        [Required(ErrorMessage = "ID")]
        public int IDND { get; set; }

        [Required(ErrorMessage = "Nhập mã Nội dung Đào tạo")]
        public string MaND { get; set; }

        [Required(ErrorMessage = "Nhập Nội dung Đào tạo")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Nhập Video Nội Dung")]
        public string VideoND { get; set; }

        [Required(ErrorMessage = "Nhập Thumbnail Video")]
        public string ImageND { get; set; }

        [Required(ErrorMessage = "Nhập Lĩnh Vực Đào Tạo")]
        public string LinhVuc { get; set; }

        [Required(ErrorMessage = "Nhập ID Bộ Phận")]
        public int BPLID { get; set; }

        [Required(ErrorMessage = "Nhập Bộ Phận Lập Nhu Cầu")]
        public string BPLNC { get; set; }

        [Required(ErrorMessage = "Nhập ID Lĩnh Vực Đào Tạo")]
        public int LVDTID { get; set; }

        public int IDCTLVDT { get; set; }

        public string LVChiTiet { get; set; }

        [Required(ErrorMessage = "Nhập Thời Lượng Đào Tạo")]
        public int ThoiLuongDT { get; set; }
        public int IDNhomNL { get; set; }
        public string TenNhomNL { get; set; }
        public int? SLLH { get; set; }

        public string FileDinhKem { get; set; }
        public DateTime? NgayTao { get; set; }

        public HttpPostedFileBase PDFEduFile { get; set; }
    }
}