//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication4.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Galeri
    {
        public Galeri()
        {
            this.Makale = new HashSet<Makale>();
        }
    
        public int GaleriID { get; set; }
        public byte[] resim { get; set; }
        public int EkleyenID { get; set; }
        public string video { get; set; }
    
        public virtual ICollection<Makale> Makale { get; set; }
    }
}
