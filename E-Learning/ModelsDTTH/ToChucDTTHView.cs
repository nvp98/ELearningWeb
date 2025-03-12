using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDTTH
{
    public class ToChucDTTHView
    {
    }
    public class ChiTietToChucDTTHView
    {
        public int ID { get; set; }
        public Nullable<int> CTDT_ID { get; set; }
        public Nullable<int> ID_GVCty { get; set; }
        public string MaNV_GV { get; set; }
        public string HoTenGV { get; set; }
        public string ViTriCV_GV { get; set; }
        public string DonVi_GV { get; set; }
    }
}