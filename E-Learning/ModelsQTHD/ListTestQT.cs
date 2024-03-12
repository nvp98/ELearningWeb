using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsQTHD
{
    public class ListTestQTView
    {
        public int IDPhanQuyen { get; set; }
        public Nullable<int> QTHDID { get; set; }
        public Nullable<int> IDVTKNL { get; set; }
        public Nullable<int> DKID { get; set; }
        public Nullable<int> IDNV { get; set; }
        public string TenNV { get; set; }
        public Nullable<int> TinhTrang { get; set; }
        public Nullable<int> TinhTrangHL { get; set; }
        public Nullable<int> TinhTrangKT { get; set; }

        public Nullable<int> checkQT { get; set; }
        public Nullable<int> SL_CauHoi { get; set; }

        public double? DiemSo { get; set; }

        public DateTime NgayKTTiep { get; set; }
        public DateTime NgayHT { get; set; }
        public QT_NoiDungQT NoiDungQT { get; set; }
        public List<NoiDungQTLienQuan> List_VanBanLQ { get; set; }
        public List<QT_FileQT> List_FileQT { get; set; }
        public List<CauHoiQTView> List_CauHoiQT { get; set; }

        public virtual QT_DinhKy QT_DinhKy { get; set; }
        //public virtual QT_NoiDungQT QT_NoiDungQT { get; set; }
    }

}