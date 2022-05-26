using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Database;
using WebApplication4.Helpers;

namespace WebApplication4.Filters
{
    public class LocalException : FilterAttribute, IExceptionFilter
    {
        public Encryption encNewTeamMember;
        public void OnException(ExceptionContext filterContext)
            {

                        try
                        {
                            HttpContext context = HttpContext.Current;
                            encNewTeamMember = new Encryption();

                            var controller = filterContext.Controller.ToString();
                            var exception = filterContext.Exception.Message.ToString();
                            var stackTrace = filterContext.Exception.StackTrace.ToString();
                            var datetimeexception = DateTime.Now;
                            var userid = Convert.ToInt64(encNewTeamMember.Decrypt_Aes_With_Custom_Key
                             (context.Server.UrlDecode(context.Session["UserId"].ToString()), (byte[])context.Session["userLoginKey"], (byte[])context.Session["userLoginIV"]));
                            //Log exception to database

                            SCRUMEntities db = new SCRUMEntities();
                            //CLIENTATTACHMENT attchmnt = new CLIENTATTACHMENT();
                            LOG lg = new LOG();

                            lg.CONTROLLER = controller;
                            lg.EXCEPTION = exception;
                            lg.STACKTRACE = stackTrace;
                            lg.EXDATETIME = datetimeexception;
                            lg.USERID = userid;
                            db.LOGs.Add(lg);

                            db.SaveChanges();
                        }
                        catch(Exception exp)
                        {
                            filterContext.ExceptionHandled = true;
                            filterContext.Result = new ViewResult()
                            {
                                ViewName = "Error"
                            };
                        }

                        filterContext.ExceptionHandled = true;
                            filterContext.Result = new ViewResult()
                            {
                                ViewName = "Error"
                            };
            }
    }
}