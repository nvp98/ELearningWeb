using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class ToKNLValidation
    {
        public int IDTo { get; set; }
        public string TenTo { get; set; }
        public string MaTo { get; set; }
        public Nullable<int> IDPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> IDPhanXuong { get; set; }
        public string TenPhanXuong { get; set; }
        public Nullable<int> IDKhoi { get; set; }
        public string TenKhoi { get; set; }
    }
}