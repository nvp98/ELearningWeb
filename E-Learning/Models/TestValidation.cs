using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Models
{
    public class TestValidation
    {
        public int IDCH { get; set; }
        [AllowHtml]
        public string NoiDungCH { get; set; }
        public int IDLH { get; set; }
        public int IDDeThi { get; set; }
        public int IDND { get; set; }
        public int IDNV { get; set; }
        public int Answer { get; set; }
        public string DapAnA { get; set; }
        public string DapAnB { get; set; }
        public string DapAnC { get; set; }  
        public string DapAnD { get; set; }  
        public int IDDAĐung { get; set; }
        public string DapAnĐung { get; set; }
        public double Diem { get; set; }

    }

}