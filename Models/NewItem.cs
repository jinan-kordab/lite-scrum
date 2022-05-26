using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{

    public class NewItem
    {
        public NewItem()
        {
            Attachments = new List<HttpPostedFileBase>();
        }

        public List<HttpPostedFileBase> Attachments { get; set; }
        public string ItemTitle { get; set; }
        public string ItemCreationDate { get; set; }
        public string ItemDescription { get; set; }
        public string LinkOne { get; set; }
        public string LinkTwo { get; set; }
        public string LinkThree { get; set; }
        public string ItemId { get; set; }
        public string hyujkzsaa { get; set; }
        public string pbhyujkzsaa { get; set; }
        public string CustomLocation { get; set; }
        public string PeopleSelect { get; set; }

        public string ClientSelect { get; set; }
        public string EpicSelect { get; set; }

        public string PBelect { get; set; }
        // Rest of model details
    }


}