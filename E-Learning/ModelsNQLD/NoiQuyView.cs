using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class NoiQuyView
    {
        [Required(ErrorMessage = "ID")]
        public int IDND { get; set; }

        [Required(ErrorMessage = "Nhập mã Nội dung Đào tạo")]
        public string MaND { get; set; }

        [Required(ErrorMessage = "Nhập tên nội quy")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Nhập Video Nội Dung")]
        public string VideoND { get; set; }

        [Required(ErrorMessage = "Nhập Thumbnail Video")]
        public string ImageND { get; set; }


        [Required(ErrorMessage = "Nhập Thời Lượng Video")]
        public int ThoiLuongDT { get; set; }

        public int? isOrder { get; set; }
        public int? isNQ  { get; set; }

        public string FileDinhKem { get; set; }
        public DateTime? NgayTao { get; set; }

        public HttpPostedFileBase PDFEduFile { get; set; }

        public int? SLHT { get; set; }
        public int? SLHTFile { get; set;}

    }
}