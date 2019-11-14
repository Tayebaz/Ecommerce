using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GrihastiWebsite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult GetLogo()
        {
            EcommerceContext db = new EcommerceContext();
            string filepath = "";
            string ImagePath = string.Empty;

            var setting = db.Settings.AsNoTracking().FirstOrDefault();
            if (setting != null)
            {
                ImagePath = "~/Logo/" + setting.Logo;
            }

            filepath = !System.IO.File.Exists(Server.MapPath(ImagePath)) ? Server.MapPath("~/Content/Admin/Images/default-image.png")
                                                           : Server.MapPath(ImagePath);

            return File(filepath, "image/jpg/gif/png");
        }
    }
}
