//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace satranc_v27.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class SiparisDetaylari
    {
        public int detay_id { get; set; }
        public Nullable<int> siparis_id { get; set; }
        public Nullable<int> urun_id { get; set; }
        public int miktar { get; set; }
        public decimal birim_fiyat { get; set; }
    
        public virtual Siparisler Siparisler { get; set; }
        public virtual Urunler Urunler { get; set; }
    }
}
