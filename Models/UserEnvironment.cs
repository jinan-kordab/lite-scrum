using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class UserEnvironment
    {
        public string IP { get; set; }
        public string Browser { get; set; }

        public string Platform { get; set; }

        public string ClientInnerWidth { get; set; }

        public string ClientInnerHeight { get; set; }

        public string ClientUserAgent { get; set;}

        public string ClientScreenWidth { get; set; }

        public string ClientScreenHeight { get; set;}

        public string ClientScreenAvailableWidth { get; set; }

        public string ClientSreenAvailableHeight { get; set; }

    }
}