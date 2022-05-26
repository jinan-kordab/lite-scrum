using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

namespace WebApplication4.Controllers
{
    public class SprintBacklogController : Controller
    {
        // GET: SprintBacklog
        public ActionResult Index()
        {
            //https://docs.microsoft.com/en-us/aspnet/web-pages/overview/data/7-displaying-data-in-a-chart
            //Chart
            return View();
        }
    }
}