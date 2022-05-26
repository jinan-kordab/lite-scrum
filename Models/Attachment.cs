using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.Models
{
    public class Attachment
    {
        public long ATTITEMID { get; set; }
        public string ATTACHMENTNAME { get; set; }
        public string ATTACHMENTTYPE { get; set; }
        public long ATTACHMENTLENGTH { get; set; }
        public byte[] ATTACHMENTCONTENT { get; set; }
        public long FKITEMID { get; set; }

        public virtual Item ITEM { get; set; }
    }
}