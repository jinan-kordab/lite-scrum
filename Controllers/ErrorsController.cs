using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;

            return View();
        }
        public ActionResult BadRequest()
        {
            Response.StatusCode = 400;

            return View();
        }
        public ActionResult InternalServerError()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}