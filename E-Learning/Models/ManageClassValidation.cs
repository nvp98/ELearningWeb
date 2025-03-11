using E_Learning.ModelsDTTH;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class ManageClassValidation
    {

        public int IDLH { get; set; }
        [Required(ErrorMessage = "Nhập mã lớp học")]
        public string MaLH { get; set; }
        [Required(ErrorMessage = "Nhập tên lớp học")]
        public string TenLH { get; set; }

        public int NDID { get; set; }

        public int QuyDT { get; set; }

        public int NamDT { get; set; }

        public DateTime TGBDLH { get; set; }

        public DateTime TGKTLH { get; set; }

        public string MaND { get; set; }

        public string TenND { get; set; }

        public string FileCTDT { get; set; }

        public int LVDTID { get; set; }

        public string LinhVuc { get; set; }

        public string BPLNC { get; set; }

        public string VideoLH { get; set; }

        public string ImageLH { get; set; }

        public int GVID { get; set; }

        public string MaGV { get; set; }

        public string TenGV { get; set; }
        public int? IDPB { get; set; }
        public string TenPhongBan { get; set; }

        public int SLHT { get; set; }

        public int TSLHV { get; set; }

        public int IDNV { get; set; }

        public bool? IsGV { get; set; }

        public int IDDeThi { get; set; }

        public string TenDeThi { get; set; }

        public Nullable<int> NCDT_ID { get; set; }
        public string TenNCDT { get; set; }
        public Nullable<int> CTDT_ID { get; set; }
        public string TenCTDT { get; set; }
        public string NoiDungTrichYeu { get; set; }
        public string DiaDiemDT { get; set; }
        public string ThoiLuongDT { get; set; }
        public Nullable<int> DonVi_GV { get; set; }
        public Nullable<int> BoPhan_ID { get; set; }
        public string TenBoPhan { get; set; }
        public Nullable<int> NguoiTao_ID { get; set; }

        public Nullable<int> ID_NhomNL { get; set; }
        public Nullable<int> ID_LVDT { get; set; }
        public string TenNguoiTao { get; set; }
        public Nullable<int> NguoiKiemTra_ID { get; set; }
        public string TenNguoiKiemTra { get; set; }
        public Nullable<int> TinhTrang { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.DateTime> NgayKiemTra { get; set; }

        public Nullable<int> IsCoCTDT { get; set; }

        public bool IsAll { get; set; }
        public string DSNhanVien { get; set; }

        public string DSHocVien { get; set; }
        public ChiTietToChucDTTHView chiTietToChucDTTH { get; set; }

    }
}