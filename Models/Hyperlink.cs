using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.Models
{
    public class Hyperlink
    {
        public long LINKITEMID { get; set; }
        public string LINKDESCRIPTION { get; set; }
        public string LINKBODY { get; set; }
        public long FKITEMID { get; set; }

        public virtual Item ITEM { get; set; }
    }
}