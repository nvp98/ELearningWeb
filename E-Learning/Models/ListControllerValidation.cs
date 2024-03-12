using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class ListControllerValidation
    {
        public int ID { get; set; }
        public string Controller { get; set; }
        public string Mota { get; set; }
        public Nullable<int> isActive { get; set; }
        public bool isCheck { get; set; }
        public string DSQuyenCN { get; set; }
        public string DSTenQuyen { get; set; }
        public string[] Selected { get; set; }
        public List<ItemCheck> LSChecked { get; set; }
    }
    public class ItemCheck
    {
        public int IDCN { get; set; }
        public string Name { get; set; }
        public bool isChecked { get; set; }
    }
}