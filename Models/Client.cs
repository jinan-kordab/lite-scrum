using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Client
    {
        public long CLIENTID { get; set; }

        public string CLIENTNAME { get; set; }
        public string CLIENTLOCATION { get; set; }
        public string CLIENTDESCRIPTION { get; set; }
        public string CLIENTPHONENUMBER { get; set; }
        public int ATTACHMENTSCOUNT { get; set; }

        public List<Client> Clientlist { get; set; }
        public string LoggedInUser { get; set; }
        public List<HttpPostedFileBase> Attachments { get; set; }
        public List<ClientAttachment> AttList { get; set; }

        public string ClientImagePath { get; set; }

        public string hyujkzsaa { get; set; }

        public string PersonImagePath { get; set; }


    }
}