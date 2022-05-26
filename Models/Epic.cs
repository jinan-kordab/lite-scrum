using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Epic
    {
        public long EPICID { get; set; }
        public string EPICTITLE { get; set; }
        public string EPICDESCRIPTION { get; set; }
        public long FKCLIENTID { get; set; }
        public string epichyujkzsaa { get; set; }
        public string LoggedInUser { get; set; }
        public List<Epic> Clientlist { get; set; }
        public string ClientName { get; set; }

        public string PersonImagePath { get; set; }
    }
}