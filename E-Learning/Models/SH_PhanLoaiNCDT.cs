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
    
    public partial class SH_PhanLoaiNCDT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SH_PhanLoaiNCDT()
        {
            this.SH_NhuCauDT = new HashSet<SH_NhuCauDT>();
        }
    
        public int IDLoai { get; set; }
        public string TenLoaiNCDT { get; set; }
        public Nullable<int> LoaiHinhDT_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SH_NhuCauDT> SH_NhuCauDT { get; set; }
    }
}
