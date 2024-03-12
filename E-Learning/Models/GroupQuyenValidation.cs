using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class GroupQuyenDetailsValidation
    {
        public int IDQuyen { get; set; }
        public string TenQuyen { get; set; }
        public List<ListControllerValidation> ListController { get; set; }

    }
}