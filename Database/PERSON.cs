//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication4.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class PERSON
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERSON()
        {
            this.ITEMs = new HashSet<ITEM>();
            this.PICTUREs = new HashSet<PICTURE>();
            this.TEAMs = new HashSet<TEAM>();
        }
    
        public long USERID { get; set; }
        public string NICKNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string EMAIL { get; set; }
        public System.DateTime REGISTRATIONDATE { get; set; }
        public byte[] PROFILEPICTURE { get; set; }
        public string PASSWORD { get; set; }
        public byte[] PKEY { get; set; }
        public byte[] PIV { get; set; }
        public bool ISTEAMCREATOR { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ITEM> ITEMs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PICTURE> PICTUREs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEAM> TEAMs { get; set; }
    }
}
