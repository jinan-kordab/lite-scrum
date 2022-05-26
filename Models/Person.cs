using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Person
    {
        public string USERID { get; set; }
        public string NICKNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string CONFIRMPASSWORD { get; set; }
        public System.DateTime REGISTRATIONDATE { get; set; }
        public HttpPostedFileBase PROFILEPICTURE { get; set; }

        public string TEAMNAME { get; set; }
        public bool ISTEAMCREATOR { get; set; }

        public string ClientImagePath { get; set; }
        public List<HttpPostedFileBase> Attachments { get; set; }

    }
}