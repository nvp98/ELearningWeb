using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class EGuideValidation
    {
        public int ID { get; set; }
        public string MoTa { get; set; }
        public string FilePath { get; set; }
        public HttpPostedFileBase FileUpload { get; set; }
        public Nullable<int> OrderBy { get; set; }
    }
}