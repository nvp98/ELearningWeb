using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Learning.Models
{
    public class ManageQuestionValidation
    {
        public int IDCH { get; set; }
        public string MaCH { get; set; }
        [AllowHtml]
        public string NoiDungCH { get; set; }
        [AllowHtml]
        public string DapAnA { get; set; }
        [AllowHtml]
        public string DapAnB { get; set; }
        [AllowHtml]
        public string DapAnC { get; set; }
        [AllowHtml]
        public string DapAnD { get; set; }
        public int IDDAĐung { get; set; }
        public string DapAnĐung { get; set; }
        public int IDLVDT { get; set; }
        public string TenLVDT { get; set; }
        public int IDCTLVDT { get; set; }
        public string TenCTLVDT { get; set; }
        public double Diem { get; set; }
        public int IDCauHoiDeThi { get; set; }
        public int IDND { get; set; }
        public int GVID { get; set; }
        public string TenNoiDung { get; set; }
        public string DapAnHV { get; set; }
    }
}