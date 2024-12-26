using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDTTH
{
    public class TrinhKyDTTHView
    {
        public int ID { get; set; }
        public Nullable<int> NCDT_ID { get; set; }
        public Nullable<int> NguoiDuyet_ID { get; set; }
        public Nullable<int> CapDuyet { get; set; }
        public Nullable<int> TinhTrangDuyet { get; set; }
    }
    public class SH_KyDuyetNCDTView
    {
        public int ID { get; set; }
        public Nullable<int> NCDT_ID { get; set; }
        public Nullable<int> NguoiDuyet_ID { get; set; }
        public Nullable<int> CapDuyet { get; set; }
        public Nullable<int> TinhTrangDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string GhiChu { get; set; }
    }
    public class CapDuyetView
    {
        //public int ID { get; set; }
        public Nullable<int> BPSD { get; set; }
        public Nullable<int> PCHN { get; set; }
        public Nullable<int> PNS { get; set; }
        public Nullable<int> BGD { get; set; }
    }
}