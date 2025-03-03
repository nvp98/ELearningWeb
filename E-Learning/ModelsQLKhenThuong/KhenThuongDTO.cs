using System;

namespace E_Learning.ModelsQLKhenThuong
{
    public class KhenThuongDTO
    {
        public int ID { get; set; }
        public int IDLoaiKhenThuong { get; set; }
        public string NoiDungKhenThuong { get; set; }
        public string SoQuyetDinh { get; set; }
        public DateTime? NgayQuyetDinh { get; set; }
        public decimal GiaTriLamLoi { get; set; }
        public decimal TongTienThuong { get; set; }
    }
}