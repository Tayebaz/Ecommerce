using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using Ecommerce.Models;
using AutoMapper;
using Business;
using Repository.RepositoryFactoryCore;
using Commom.GlobalMethods;
using Common.Cryptography;

namespace GrihastiWebsite.Controllers
{
    public class HomeController : Controller
    { 

         EcommerceContext db =null;
        UserBusiness UsersList = null;
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;

        public ActionResult Index()
        {
            bool IsUsedLocalLoginPage = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["UserLocalLoginPage"]);


            if (!IsUsedLocalLoginPage)
            {
                #region IntergartionCode
                if (Request.QueryString["uid"] == null)
                {
                    string LoginUrl = System.Configuration.ConfigurationManager.AppSettings["LoginPageUrl"];
                    return Redirect(LoginUrl);
                }
                else
                {
                   

                    string Fname = Request.QueryString["first"];
                    string Lname = Request.QueryString["last"];
                    string Email = Request.QueryString["email"];

                    db = new EcommerceContext();
                    this._unitOfWork = new UnitOfWork(_df);

                    UsersList = new UserBusiness(_df, _unitOfWork);

                    User CurrentUserInfo = new User() { FirstName = Fname, LastName = Lname, Email = Email };


                    var IsUserExist = UsersList.GetUserByemail(CurrentUserInfo.Email);

                    if (IsUserExist == null)
                    {
                        

                        User newUser = new User();
                        newUser.TokenKey = GlobalMethods.GetToken();
                        
                        newUser.FirstName = CurrentUserInfo.FirstName;
                        newUser.LastName = CurrentUserInfo.LastName;
                        newUser.Email = CurrentUserInfo.Email;
                        newUser.Password = Md5Encryption.Encrypt(System.Configuration.ConfigurationManager.AppSettings["UserPassword"]);
                        newUser.UserType = "Customer";
                        newUser.IsBlocked = false;
                        newUser.IsConfirmed = true;

                        UsersList.Insert(newUser);
                        _unitOfWork.SaveChanges();

                         Session["CurrentUserInfo"] = newUser;
                    }
                    else
                    {
                        Session["CurrentUserInfo"] = IsUserExist;

                    }

                }

                #endregion
            }
            else
            {

            }

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

        public ActionResult LogOut()
        {
            Session["CurrentUserInfo"] = null;
            Session.Abandon();
            Session.Clear();

            bool IsUsedLocalLoginPage = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["UserLocalLoginPage"]);


            if (!IsUsedLocalLoginPage)
            {
                string LoginUrl = System.Configuration.ConfigurationManager.AppSettings["LoginPageUrl"];
                return Redirect(LoginUrl);
            }
            
               return RedirectToAction("Index", "Login");
            
        }
    }
}
