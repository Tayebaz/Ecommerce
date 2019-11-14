using AutoMapper;
using Business;
using Ecommerce.Models;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace umartcms.Controllers
{
    public class LoginController : Controller
    {
        EcommerceContext db = new EcommerceContext();
        UserBusiness UsersList = new UserBusiness();
        //
        // GET: /Login/
        public ActionResult Index()
        {
            var setting = db.Settings.AsNoTracking().FirstOrDefault();
            Mapper.CreateMap<Setting, SettingViewModel>();
            var settingViewModel = Mapper.Map<Setting, SettingViewModel>(setting);
            if (settingViewModel.isClosed)
            {
               return RedirectToAction("ShopClosed");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel loginVm)
        {
            var setting = db.Settings.AsNoTracking().FirstOrDefault();
            Mapper.CreateMap<Setting, SettingViewModel>();
            var settingViewModel = Mapper.Map<Setting, SettingViewModel>(setting);

            Mapper.CreateMap<LoginViewModel, User>();
            var user = Mapper.Map<LoginViewModel, User>(loginVm);
            user.UserType = "Customer";

            var result = UsersList.CheckUser(user, this.HttpContext);

            if (result == "invalid")
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }
            else
            {
                if (string.IsNullOrEmpty(loginVm.ReturnUrl))
                {
                    Session["CurrentUserEmail"] = loginVm.Email;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Server.ClearError();
                    Response.Redirect(loginVm.ReturnUrl, false);
                }
            }
        
            return View(loginVm);

        //if (settingViewModel.isClosed)
        //{
        //    return RedirectToAction("ShopClosed");
        //}

        //var Email = loginVm.Email;
        //var PW = loginVm.Password;
        //gannon auth
        // check if user exists in db
        //var currentUser = UsersList.

        //if (currentUser == null)
        //{

        //if null its mean user not authinticated should view msg 
        //return View("Index");

        //}
        //else
        //{
        // user has uthinticatetion and you should check the user info in your database system

        //   var IsExsit =   UsersList.GetUserByemail(Email);

        //    if (IsExsit == null)
        //    {
        //        User newUser = new User();
        //        newUser.Email = Email;
        //        newUser.Password = PW;
        //        newUser.UserType = "Customer";
        //        UsersList.Insert(newUser);
        //    }


        //    Session["CurrentUserEmail"] = Email;

        //    return RedirectToAction("Index", "Home");
        //}


        // return View("Index");
    }

        public ActionResult ShopClosed()
        {
            return View();
        }
	}
}