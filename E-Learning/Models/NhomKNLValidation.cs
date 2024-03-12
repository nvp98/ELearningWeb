using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class NhomKNLValidation
    {
        public int IDNhom { get; set; }
        public string TenNhom { get; set; }
        public string MaNhom { get; set; }
        public Nullable<int> IDPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> IDPhanXuong { get; set; }
        public string TenPhanXuong { get; set; }
        public Nullable<int> IDKhoi { get; set; }
        public string TenKhoi { get; set; }
    }
}