using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Picture
    {
        public long PERSONPICTUREID { get; set; }
        public string PERSONPICTURENAME { get; set; }
        public string PERSONPICTURETYPE { get; set; }
        public long PERSONPICTURELENGTH { get; set; }
        public byte[] PERSONPICTURECONTENT { get; set; }
        public long FKPERSONID { get; set; }
    }
}