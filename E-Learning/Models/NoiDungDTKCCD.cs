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
    
    public partial class NoiDungDTKCCD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NoiDungDTKCCD()
        {
            this.DeNghiKCCD = new HashSet<DeNghiKCCD>();
        }
    
        public int ID { get; set; }
        public string TenND { get; set; }
        public Nullable<int> NhomNLID { get; set; }
        public Nullable<int> LVDTID { get; set; }
        public Nullable<int> PhongBanID { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeNghiKCCD> DeNghiKCCD { get; set; }
        public virtual NhomNLKCCD NhomNLKCCD { get; set; }
    }
}
