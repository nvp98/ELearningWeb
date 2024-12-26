using E_Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsDTTH
{
    public class NoiDungDTTHView
    {
        public int IDND { get; set; }
        public string MaND { get; set; }
        public string NoiDung { get; set; }
        public string VideoND { get; set; }
        public string ImageND { get; set; }
        public Nullable<int> BPLID { get; set; }
        public Nullable<int> LVDTID { get; set; }
        public string TenLVDT { get; set; }
        public Nullable<int> IDCTLVDT { get; set; }
        public string TenCTLVDT { get; set; }
        public Nullable<int> ThoiLuongDT { get; set; }
        public string FileDinhKem { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> isOrder { get; set; }
        public Nullable<int> isNQ { get; set; }
        public Nullable<int> IDNguonGV { get; set; }
        public string TenNguonGV { get; set; }
        public Nullable<int> IDHoatDongDT { get; set; }
        public string TenHoatDongDT { get; set; }
        public Nullable<int> IDPPDT { get; set; }
        public string TenPPDT { get; set; }
        public Nullable<int> IDNhomNL { get; set; }
        public string TenNhomNL { get; set; }
        public Nullable<int> DuyetNDDT { get; set; }
        public Nullable<int> IDPhanLoaiDT { get; set; }
        public string TenLoaiDT { get; set; }

        public Nullable<int> SLViTri { get; set; }

        //public virtual LinhVucDT LinhVucDT { get; set; }
        //public virtual HoatDongDT HoatDongDT { get; set; }
        //public virtual HoatDongDT HoatDongDT { get; set; }

    }
    public class PhanQuyenVT_ND
    {
        public int IDND { get; set; }
        public string MaND { get; set; }
        public string NoiDung { get; set; }
        public string VideoND { get; set; }
        public string ImageND { get; set; }
        public Nullable<int> BPLID { get; set; }
        public Nullable<int> LVDTID { get; set; }
        public string TenLVDT { get; set; }
        public Nullable<int> IDCTLVDT { get; set; }
        public string TenCTLVDT { get; set; }
        public Nullable<int> ThoiLuongDT { get; set; }
        public string FileDinhKem { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> isOrder { get; set; }
        public Nullable<int> isNQ { get; set; }
        public Nullable<int> IDNguonGV { get; set; }
        public string TenNguonGV { get; set; }
        public Nullable<int> IDHoatDongDT { get; set; }
        public string TenHoatDongDT { get; set; }
        public Nullable<int> IDPPDT { get; set; }
        public string TenPPDT { get; set; }
        public Nullable<int> IDNhomNL { get; set; }
        public string TenNhomNL { get; set; }
        public Nullable<int> DuyetNDDT { get; set; }
        public Nullable<int> IDPhanLoaiDT { get; set; }
        public string TenLoaiDT { get; set; }

        public Nullable<int> SLViTri { get; set; }

        //public virtual LinhVucDT LinhVucDT { get; set; }
        //public virtual HoatDongDT HoatDongDT { get; set; }
        //public virtual HoatDongDT HoatDongDT { get; set; }

    }

    public class SH_ViTri_NDDTView
    {
        public int ID { get; set; }
        public Nullable<int> ViTri_ID { get; set; }
        public string TenViTri { get; set; }
        public int? NoiDungDT_ID { get; set; }
        public string TenNoiDungDT { get; set; }
        public Nullable<int> DinhKy_ID { get; set; }
        public string TenDinhKy { get; set; }
        public Nullable<int> PhuongPhapDT_ID { get; set; }
        public string TenPhuongPhapDT { get; set; }
        public string TenPhongBan { get; set; }
        public string IDVTCopy { get; set; }
        public string IDNDCopy { get; set; }
        public string[] Selected { get; set; }

        public Nullable<int> IDPhanLoaiDT { get; set; }
        public string TenLoaiDT { get; set; }
        public Nullable<int> Tinhtrang { get; set; }
    }

    public class SH_ViTri_NDDTList
    {
        public int ID { get; set; }
        public Nullable<int> ViTri_ID { get; set; }
        public string TenViTri { get; set; }
        public Nullable<int> NoiDungDT_ID { get; set; }
        public string TenNoiDungDT { get; set; }
        public  List<SH_ViTri_NDDTView> ListVTNDDTView { get; set; }
    }
}