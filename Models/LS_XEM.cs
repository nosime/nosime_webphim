//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebPhimNosime.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LS_XEM
    {
        public int ID_LS { get; set; }
        public string ID_USER { get; set; }
        public int ID_PHIM { get; set; }
        public int ID_TAP { get; set; }
        public Nullable<System.DateTime> NGAY { get; set; }
    
        public virtual PHIM PHIM { get; set; }
        public virtual TAP_PHIM TAP_PHIM { get; set; }
    }
}
