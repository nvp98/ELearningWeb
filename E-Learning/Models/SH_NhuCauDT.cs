//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace E_Learning.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SH_NhuCauDT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SH_NhuCauDT()
        {
            this.SH_ChiTiet_NCDT = new HashSet<SH_ChiTiet_NCDT>();
        }
    
        public int ID { get; set; }
        public Nullable<int> Nam { get; set; }
        public Nullable<int> Quy { get; set; }
        public Nullable<int> NoiDungDT_ID { get; set; }
        public Nullable<int> MaTrinhKy { get; set; }
        public Nullable<int> BoPhanLNC_ID { get; set; }
        public string FileDinhKem { get; set; }
        public Nullable<int> NguoiTao_ID { get; set; }
        public Nullable<int> PhanLoaiNCDT_ID { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> MaDinhKy { get; set; }
        public Nullable<int> PhuongPhapDT_ID { get; set; }
        public Nullable<int> TinhTrang { get; set; }
        public string GhiChu { get; set; }
    
        public virtual NoiDungDT NoiDungDT { get; set; }
        public virtual PhongBan PhongBan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SH_ChiTiet_NCDT> SH_ChiTiet_NCDT { get; set; }
        public virtual SH_PhanLoaiNCDT SH_PhanLoaiNCDT { get; set; }
    }
}