using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class WikiController : Controller
    {
        // GET: Wiki
        public ActionResult Index()
        {
            if (Session["LANG"] != null)
            {
                if (Session["LANG"].ToString() == "fr")
                {
                    return View("fr/Index");
                }
                else
                {
                    return View("Index");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        public RedirectToRouteResult Culture(Language lang)
        {
            var languageSelected = lang.lAction;
            Session["LANG"] = languageSelected;


            return RedirectToAction("Index");
        }
    }
}