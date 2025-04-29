using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsQLKhenThuong
{
    public class LoaiKhenThuongDTO
    {
        public int ID { get; set; }
        public string TenLoaiThuong {  get; set; }
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public string TenPhongBan { get; set; }
        public float GiaTriThuong { get; set; }
        public string TenDeTai { get; set; }
        public string TapThe { get; set; }
    }
}