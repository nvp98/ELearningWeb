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
    
    public partial class BDT_TieuBan
    {
        public int ID { get; set; }
        public string TenTieuBan { get; set; }
        public Nullable<int> TrangThai { get; set; }
        public Nullable<System.DateTime> NgayCapNhatGanNhat { get; set; }
        public Nullable<System.DateTime> NgayDenHanCapNhatLai { get; set; }
        public Nullable<int> NguoiTrinhKy_ID { get; set; }
        public Nullable<int> PhongBan_ID { get; set; }
    }
}
