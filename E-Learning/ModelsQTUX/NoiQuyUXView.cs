using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class NoiQuyUXView
    {
        [Required(ErrorMessage = "ID")]
        public int IDND { get; set; }

        [Required(ErrorMessage = "Nhập mã Nội dung Ứng Xử")]
        public string MaND { get; set; }

        [Required(ErrorMessage = "Nhập tên nội quy ứng xử")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Nhập Video Nội Dung")]
        public string VideoND { get; set; }

        [Required(ErrorMessage = "Nhập Thumbnail Video")]
        public string ImageND { get; set; }

        [Required(ErrorMessage = "Nhập Thời Lượng Video")]
        public int ThoiLuongDT { get; set; }

        public int? isOrder { get; set; }

        public string FileDinhKem { get; set; }

        public DateTime? NgayTao { get; set; }

        public HttpPostedFileBase PDFEduFile { get; set; }

        public int? SLHT { get; set; }

        public int? SLHTFile { get; set; }

        // Thêm các field cho bài thi
        public int? SLHoanThanhThi { get; set; }

        // Định kỳ (tháng)
        public int? DinhKy { get; set; }
    }
}
