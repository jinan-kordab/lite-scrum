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
    
    public partial class TEAM
    {
        public long TEAMID { get; set; }
        public string TEAMNAME { get; set; }
        public long FKPERSONID { get; set; }
    
        public virtual PERSON PERSON { get; set; }
    }
}
