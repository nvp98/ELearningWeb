using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsKCCD
{
    public class KCCD_CauHoiView
    {
        public int IDCH { get; set; }
        public string NoiDungCH { get; set; }
        public string DapAnA { get; set; }
        public string DapAnB { get; set; }
        public string DapAnC { get; set; }
        public string DapAnD { get; set; }
        public Nullable<int> IDDAĐung { get; set; }
        public string DapAn { get; set; }
        public Nullable<int> KCCDID { get; set; }

        public Nullable<int> DeThiID { get; set; }
        public int Answer { get; set; }

        public string TenDeThi { get; set; }
        public string TenNoiDungKCCD { get; set; }
        public double? Diem { get; set; }
    }
    public class BaiThiKCCDView
    {
        public Nullable<int> IDNV { get; set; }
        public string TenNV { get; set; }
        public Nullable<int> KCCDID { get; set; }
        public Nullable<int> DeNghiID { get; set; }
        public Nullable<int> DeThiID { get; set; }
        public double? DiemChuan { get; set; }
        public List<KCCD_CauHoiView> List_CauHoiKCCD { get; set; }
        public KCCD_DeThi DeThiKCCDView { get; set; }
    }
}