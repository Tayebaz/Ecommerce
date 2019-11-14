using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{   
    public class AdminController : Controller
    {
        //
        // GET: /Switch/
        public ActionResult Index()
        {
            return RedirectToAction("Login", "AdminAccount", new { Area = "Admin" });
        }
	}
}