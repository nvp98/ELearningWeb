using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsTieuBanDaoTao
{
    public class ThanhVienTieuBanViewModel
    {
        public int TieuBan_ID { get; set; }
        public string TenTieuBan { get; set; }
        public List<ThanhVienItem> ThanhVienList { get; set; }
    }

    public class ThanhVienItem
    {
        public int Id { get; set; }
        public int ViTri { get; set; }
    }

}