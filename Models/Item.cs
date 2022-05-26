using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;
using WebApplication4.Database;

namespace WebApplication4.Models
{
    public class Item
    {
        public Item()
        {
            this.ATTACHMENTs = new HashSet<ATTACHMENT>();
            this.HYPERLINKs = new HashSet<HYPERLINK>();
        }

        public long ITEMID { get; set; }
        public string ITEMTITLE { get; set; }
        public System.DateTime CREATIONDATE { get; set; }
        public int ATTACHMENTSCOUNT { get; set; }
        public int LINKCOUNT { get; set; }
        public string ITEMNOTES { get; set; }
        public string ITEMLOCATION { get; set; }

        public string LINKONE { get; set; }
        public string LINKTWO { get; set; }
        public string LINKTHREE { get; set; }

        public string LoggedInUser { get; set; }

        public virtual ICollection<ATTACHMENT> ATTACHMENTs { get; set; }
        public virtual ICollection<HYPERLINK> HYPERLINKs { get; set; }
        public List<Item> Itemlist { get; set; }
        public List<Attachment> AttList {get;set;}
        public string PersonImagePath { get; set; }
        public string FullName { get; set; }

        public string FullNamePlusRank { get; set; }
        public string UsersTeam { get; set; }

        public long PERSONID { get; set; }

    }
}