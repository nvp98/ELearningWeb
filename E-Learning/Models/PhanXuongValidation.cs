using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class PhanXuongValidation
    {
        public int ID { get; set; }
        public string MaPX { get; set; }
        public string TenPX { get; set; }
        public Nullable<int> IDPhongBan { get; set; }
        public string TenPhongBan { get; set; }
        public Nullable<int> IDKhoi { get; set; }
        public string TenKhoi { get; set; }
    }
}