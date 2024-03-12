using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsQTHD
{
    public class CauHoiQTView
    {
        public int IDCH { get; set; }
        public string NoiDungCH { get; set; }
        public string DapAnA { get; set; }
        public string DapAnB { get; set; }
        public string DapAnC { get; set; }
        public string DapAnD { get; set; }
        public Nullable<int> IDDAĐung { get; set; }
        public string DapAn { get; set; }
        public Nullable<int> QTHDID { get; set; }
        public int Answer { get; set; }

        public string TenQT { get; set; }
        public string MaHieu { get; set; }
        public double? Diem { get; set; }
    }

    public class CauHoiQTListView
    {
        public int IDCH { get; set; }
        public string NoiDungCH { get; set; }
        public string DapAnA { get; set; }
        public string DapAnB { get; set; }
        public string DapAnC { get; set; }
        public string DapAnD { get; set; }
        public Nullable<int> IDDAĐung { get; set; }
        public string DapAn { get; set; }
        public Nullable<int> QTHDID { get; set; }

        public string TenQT { get; set; }
        public string MaHieu { get; set; }
        public int? CountBaiThi { get; set; }
        public int? CountDat { get; set; }
        public int? CountKhongDat { get; set; }
    }

}