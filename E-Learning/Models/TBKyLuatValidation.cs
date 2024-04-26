using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class TBKyLuatValidation
    {
        public int ID { get; set; }
        public string TB_TieuDe { get; set; }
        public int? TB_Thang { get; set; }
        public int? TB_Nam { get; set; }
        public string TB_File { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
    }
}