using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Models
{
    public class CauHoiKCCDView
    {
        public int IDCH { get; set; }
        public string NoiDungCH { get; set; }
        public string DapAnA { get; set; }
        public string DapAnB { get; set; }
        public string DapAnC { get; set; }
        public string DapAnD { get; set; }
        public int IDDAĐung { get; set; }
        public string DapAnĐung { get; set; }
        public double Diem { get; set; }
        public int? DeThiID { get; set; }
        public string TenDeThi { get; set; }
        public int? KCCDID { get; set; }
        public string TenNoiDung { get; set; }
    }
}