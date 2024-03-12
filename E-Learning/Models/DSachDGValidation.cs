using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class DSachDGValidation
    {
        public int IDDS { get; set; }
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public string NVDG { get; set; }
        public Nullable<int> IDKNL { get; set; }
        public string TenKNL { get; set; }
        public string[] Selected { get; set; }
        public int[] SelectedKN { get; set; }

        public string[] Selec { get; set; }
        public string[] SelectTC { get; set; }

        public string ViTriNguon { get; set; }

        public string ViTriDen { get; set; }

    }
}