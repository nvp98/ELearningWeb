using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class ManageTestExamValidation
    {
        public int IDDeThi { get; set; }

        public string MaDe { get; set; }
       
        public string TenDe { get; set; }

        public double DiemChuan { get; set; }

        public int TongSoCau { get; set; }
      
        public int ThoiGianLamBai { get; set; }

        public int IDND { get; set; }

        public string TenND { get; set; }

        public int IDLH { get; set; }

        public string TenLH { get; set; }
        public string NoiDung { get; set; }
        public string HoTen { get; set; }
        public int GVID { get; set; }

        public DateTime TGBDLH { get; set; }

        public DateTime TGKTLH { get; set; }
        public bool IsGV { get; set; }

    }
}