using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsKCCD
{
    public class NoiDungDTKCCDView
    {
        public int ID { get; set; }
        public string TenND { get; set; }
        public int NhomNLID { get; set; }
        public string TenNhomNL { get; set; }
        public int LVDTID { get; set; }
        public string TenLVDT { get; set; }
        public int PhongBanID { get; set; }
        public string TenPhongBan { get; set; }
        public DateTime NgayTao { get; set; }
        public int? SLLH { get; set; }
        public int? SLCH { get;set; }
        public int? SLDT { get; set; }
    }
}