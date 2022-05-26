using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ClientAttachment
    {
        public long CATTACHMENTID { get; set; }
        public string CATTACHMENTNAME { get; set; }
        public string CATTACHMENTTYPE { get; set; }
        public long CATTACHMENTLENGTH { get; set; }
        public byte[] CATTACHMENTCONTENT { get; set; }
        public long FKCLIENTID { get; set; }
    }
}