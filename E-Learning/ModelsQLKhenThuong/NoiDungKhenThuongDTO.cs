using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace E_Learning.ModelsQLKhenThuong
{
    public class NoiDungKhenThuongDTO
    {
        public int ID { get; set; }
        public int? IDLoaiKhenThuong { get; set; }
        public string NoiDungKhenThuong { get; set; }
        public string SoQuyetDinh { get; set; }
        public DateTime? NgayQuyetDinh { get; set; }
        public decimal? GiaTriLamLoi { get; set; }
        public decimal? TongTienThuong { get; set; }
        public string LoaiKhenThuong { get; set; }
        public string DonVi { get; set; }

        // upload img
        [NotMapped]
        public HttpPostedFileBase BannerUpload { get; set; }

        public string BannerBase64 { get; set; }
    }
}