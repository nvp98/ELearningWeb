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
    
    public partial class SH_KyDuyetNCDT
    {
        public int ID { get; set; }
        public Nullable<int> NCDT_ID { get; set; }
        public Nullable<int> NguoiDuyet_ID { get; set; }
        public Nullable<int> CapDuyet { get; set; }
        public Nullable<int> TinhTrangDuyet { get; set; }
        public Nullable<System.DateTime> NgayDuyet { get; set; }
        public string GhiChu { get; set; }
    }
}
