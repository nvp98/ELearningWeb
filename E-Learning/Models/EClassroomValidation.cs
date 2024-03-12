using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class EClassroomValidation
    {
        public int IDHT { get; set; }

        public int PBID { get; set; }

        public int NVID { get; set; }

        public string MaNV { get; set; }

        public string HoTenHV { get; set; }

        public string TenPB { get; set; }

        public string TenVT { get; set; }

        public int LHID { get; set; }

        public string TenLH { get; set; }

        public string MaLH { get; set; }

        public string TenND { get; set; }

        public string LinhVuc { get; set; }

        public string VideoLH { get; set; }

        public string ImageLH { get; set; }

        public string FileCTDT { get; set; }

        public DateTime TGBDLH { get; set; }

        public DateTime TGKTLH { get; set; }

        public DateTime NgayTG { get; set; }

        public DateTime NgayHT { get; set; }

        public bool XNTG { get; set; }

        public bool XNHT { get; set; }

        public int IDBaiThi { get; set; }

        public string TenBaiThi { get; set; }
        public bool ToChucThi { get; set; }

    }
}