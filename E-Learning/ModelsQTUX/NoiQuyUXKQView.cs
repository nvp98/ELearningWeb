using E_Learning.ModelsNQLD;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class NoiQuyUXKQView
    {
        [Required(ErrorMessage = "ID")]
        public int IDND { get; set; }

        [Required(ErrorMessage = "Nhập mã Nội dung")]
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

        public int? XNTG { get; set; }

        public int? XNHT { get; set; }

        public int? XNHTFile { get; set; }

        public DateTime? NgayTG { get; set; }

        public DateTime? NgayHT { get; set; }

        public int? TinhTrang { get; set; }

        // Thêm field cho kết quả bài thi
        public int? IDDeThi { get; set; }

        public string TenDeThi { get; set; }

        public double? DiemSo { get; set; }

        public DateTime? NgayThi { get; set; }

        public int? LanThi { get; set; }

        public int? TinhTrangThi { get; set; }
    }
    public class CauHoiViewModel
    {
        public int IDCauHoiDeThi { get; set; }
        public int IDCauHoi { get; set; }
        public string NoiDungCauHoi { get; set; }
        public string DapAnA { get; set; }
        public string DapAnB { get; set; }
        public string DapAnC { get; set; }
        public string DapAnD { get; set; }
        public int? IDDAĐung { get; set; }
        public double? Diem { get; set; }
    }
}
