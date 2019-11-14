using AutoMapper;
using Commom.GlobalMethods;
using Ecommerce.Models;
using Entities;
using Entities.Models;
using Filters.ActionFilters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;

namespace Ecommerce.Controllers
{
    [EcommerceAuthorize("Admin")]
    [RouteArea("Admin")]
    [RoutePrefix("Setting")]
    public class AdminSettingController : Controller
    {
        EcommerceContext db = new EcommerceContext();
     
        [Route("")]
        // GET: AdminAbout
        public ActionResult Index()
        {           
            var setting = db.Settings.AsNoTracking().FirstOrDefault();
            Mapper.CreateMap<Setting, SettingViewModel>();
            var settingViewModel = Mapper.Map<Setting, SettingViewModel>(setting);
           
            return View(settingViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("")]
        public ActionResult Index(SettingViewModel settingViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    Mapper.CreateMap<SettingViewModel, Setting>();
                    Setting setting = Mapper.Map<SettingViewModel, Setting>(settingViewModel);
                   

                    var settingdb = db.Settings.AsNoTracking().FirstOrDefault();

                    var dirPath = Server.MapPath("~/Logo/");
                    var filePath = string.Empty;
                    if (settingViewModel.LogoUpload != null)
                    {
                        string ext = Path.GetExtension(settingViewModel.LogoUpload.FileName).ToLower();
                        string filename = setting.Id + ext;
                        filePath = dirPath + filename;
                        setting.Logo = filename;
                    }

                    if (settingdb == null)
                    {
                        db.Settings.Add(setting);
                    }
                    else
                    {
                        setting.Id = settingdb.Id;
                        db.Entry(setting).State = EntityState.Modified;
                    }

                    if (settingViewModel.LogoUpload != null)
                    {
                        FileOperations.CreateDirectory(Server.MapPath("~/Logo/"));
                        settingViewModel.LogoUpload.SaveAs(filePath);
                    }

                    db.SaveChanges();


                    TempData["Success"] = "Setting saved Successfully!!";
                    TempData["isSuccess"] = "true";

                    return RedirectToAction("Index", "AdminSetting", new { area = "Admin" });

                }
                catch (Exception ex)
                {
                    TempData["Success"] = ex.Message;
                    TempData["isSuccess"] = "false";
                }
            }
            else
            {
                TempData["Success"] = ModelState.Values.SelectMany(m => m.Errors).FirstOrDefault().ErrorMessage;
                TempData["isSuccess"] = "false";
            }
            return View(settingViewModel);
        }

        [Route("GetLogo")]
        public ActionResult GetLogo()
        {
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