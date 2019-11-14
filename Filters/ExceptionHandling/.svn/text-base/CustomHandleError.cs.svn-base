using Common.ErrorLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Filters.ExceptionHandling
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {     
        public CustomHandleErrorAttribute(){}       

        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                // if the request is AJAX return JSON else view.
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            error = true,
                            message = filterContext.Exception.Message
                        }
                    };
                }
                else
                {
                    var controllerName = (string)filterContext.RouteData.Values["controller"];
                    var actionName = (string)filterContext.RouteData.Values["action"];
                    var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                    //ExceptionHandling.ErrorLogFileWrite(model);
                    filterContext.Result = new ViewResult
                    {
                        ViewName = View,
                        MasterName = Master,
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                        TempData = filterContext.Controller.TempData
                    };
                    ErrorLog.ErrorLogFileWrite(model);
                }

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;               
            }
        }
    }
}
