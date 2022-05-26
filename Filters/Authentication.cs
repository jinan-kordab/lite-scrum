using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication4.Filters
{
    public class Authentication : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["UserLoggedIn"] == null)
            {
                filterContext.Result = new 
                    RedirectToRouteResult(
                                            new RouteValueDictionary{
                                                { "controller", "Login" },
                                                { "action", "Index" }
                                            }
                                          );
            }
            base.OnActionExecuting(filterContext);
        }
    }
}