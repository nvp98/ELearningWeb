using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class DeThiKCCDView
    {
        public int ID { get; set; }

        public string MaDe { get; set; }

        public string TenDe { get; set; }

        public double? DiemChuan { get; set; }

        public int? TongSoCau { get; set; }

        public int? ThoiGianLamBai { get; set; }

        public int? KCCDID { get; set; }

        public string TenND { get; set; }

    }
}