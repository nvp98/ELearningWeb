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
    
    public partial class QT_PhanQuyen
    {
        public int IDPhanQuyen { get; set; }
        public Nullable<int> QTHDID { get; set; }
        public Nullable<int> IDVTKNL { get; set; }
        public Nullable<int> DKID { get; set; }
    
        public virtual QT_DinhKy QT_DinhKy { get; set; }
        public virtual QT_NoiDungQT QT_NoiDungQT { get; set; }
    }
}
