using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsQTHD
{
    public class PhanQuyenQT
    {
        public int IDPhanQuyen { get; set; }
        public Nullable<int> QTHDID { get; set; }
        public Nullable<int> IDVTKNL { get; set; }
        public Nullable<int> DKID { get; set; }

        public virtual QT_DinhKy QT_DinhKy { get; set; }
        public virtual QT_NoiDungQT QT_NoiDungQT { get; set; }
    }
    public class PhanQuyenView
    {
        public Nullable<int> QTHDID { get; set; }

        public string TenQTHD { get; set; }

        public string MaHieu { get; set; }
        public Nullable<int> IDVTKNL { get; set; }
        public string TenVTKNL { get; set; }

        public Nullable<int> DKID { get; set; }

        public List<NoiDungPhanQuyenView> ListNDQT { get; set; }

        public List<ViTriPhanQuyenView> ListVTKNL { get; set; }

        public int[] SelectedQT { get; set; }

        public virtual QT_DinhKy QT_DinhKy { get; set; }
        public virtual QT_NoiDungQT QT_NoiDungQT { get; set; }
        public string IDVTCopy { get; set; }
    }
    public class ViTriPhanQuyenView
    {
        public int IDPhanQuyen { get; set; }
        public int IDVT { get; set; }
        public string TenViTri { get; set; }
        public string MaViTri { get; set; }

        public Nullable<int> IDDinhKy { get; set; }

        public string TenDinhKy { get; set; }

        public Nullable<int> QTHDID { get; set; }

        public string TenQTHD { get; set; }

        public string MaHieu { get; set; }
    }
}