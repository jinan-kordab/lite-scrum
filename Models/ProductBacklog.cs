using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Database;

namespace WebApplication4.Models
{
    public class ProductBacklog
    {
        public long ITEMID { get; set; }
        public string ITEMTITLE { get; set; }
        public System.DateTime CREATIONDATE { get; set; }
        public int ATTACHMENTSCOUNT { get; set; }
        public int LINKCOUNT { get; set; }
        public string ITEMNOTES { get; set; }
        public string ITEMLOCATION { get; set; }
        public long FKEPICID { get; set; }
        public List<ProductBacklog> Itemlist { get; set; }
        public List<Item> IndividualItems { get; set; }




        public string LINKONE { get; set; }
        public string LINKTWO { get; set; }
        public string LINKTHREE { get; set; }

        public string LoggedInUser { get; set; }
        public List<Attachment> AttList { get; set; }

        public int cIndividualItemsColumnCount { get; set; }
        public string PersonImagePath { get; set; }

        public string FullNamePlusRank { get; set; }

        public string SPRINTARCHIVESTARTDATE { get; set; }
        public string SPRINTARCHIVEENDDATE { get; set; }



        public List<long > ChartSprintList { get; set; }
        public List<int> ChartTasksDoneList { get; set; }
        public List<double> ChartVelocityList { get; set; }


        public string SChartSprintList { get; set; }
        public string SChartTasksDoneList { get; set; }
        public string SChartVelocityList { get; set; }
}
}