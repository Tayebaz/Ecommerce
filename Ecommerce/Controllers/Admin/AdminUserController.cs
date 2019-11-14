using AutoMapper;
using Common.Cryptography;
using Entities.Models;
using Filters.ActionFilters;
using Ecommerce.Models;
using Repository.RepositoryFactoryCore;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Filters.AuthenticationModel;
using System.Threading.Tasks;
using Ecommerce.Helper;
using Commom.GlobalMethods;
using System.IO;

namespace Ecommerce.Controllers
{
    [EcommerceAuthorize("Admin")]
    [RouteArea("Admin")]
    [RoutePrefix("User")]
    public class AdminUserController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private UserBusiness _userBusiness;

        public AdminUserController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._userBusiness = new UserBusiness(_df, this._unitOfWork);

        }
        //
        //
        // GET: /User/
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUsers(string sidx, string sord, int page, int rows, string colName, string colValue)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var userList = _userBusiness.GetListWT().Where(u => u.UserType == "Admin" || u.UserType =="Customer");
            List<UserViewModel> userViewModelList = new List<UserViewModel>();

            var records = (from p in userList
                           select new UserViewModel
                           {
                               TokenKey = p.TokenKey,
                               UserName = p.FirstName + " " + p.LastName,
                               Email = p.Email,
                               Phone = p.Phone,
                               IsBlocked =p.IsBlocked
                               //Gender = p.Gender,
                              // Address = p.Address + ", " + p.Pincode + " " + p.City
                           }).AsQueryable();

            //applying filter
            if (!string.IsNullOrEmpty(colName) && !string.IsNullOrEmpty(colValue))
            {
                records = records.Where(c => c.GetType().GetProperty(colName).GetValue(c, null).ToString().ToLower().Contains(colValue.ToLower()));
            }

            int totalRecords = records.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (!string.IsNullOrEmpty(sidx) && !string.IsNullOrEmpty(sord))
            {
                if (sord.Trim().ToLower() == "asc")
                {
                    records = SortHelper.OrderBy(records, sidx);
                }
                else
                {
                    records = SortHelper.OrderByDescending(records, sidx);
                }
            }

            //applying paging
            records = records.Skip(pageIndex * pageSize).Take(pageSize);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = records
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Route("GetUserImage")]
        public ActionResult GetUserImage(string tkn = "")
        {
            string filepath = "";
            string ImagePath = string.Empty;
            if (!string.IsNullOrEmpty(tkn))
            {
                if (tkn == "loggedinUser")
                {
                    var currentUserId = GlobalUser.getGlobalUser().UserId;
                    var user = _userBusiness.GetListWT(c => c.UserId == currentUserId).FirstOrDefault();
                    ImagePath = "~/ProfileImage/" + user.ProfileImage;
                }
                else
                {
                    var user = _userBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
                    ImagePath = "~/ProfileImage/" + user.ProfileImage;
                }

                filepath = !System.IO.File.Exists(Server.MapPath(ImagePath)) ? Server.MapPath("~/Content/Admin/Images/icon-user-default.png")
                                                               : Server.MapPath(ImagePath);
            }
            else
            {
                filepath = Server.MapPath("~/Content/Admin/Images/icon-user-default.png");
            }

            return File(filepath, "image/jpg/gif/png");

        }

        [Route("UserProfile")]
        public ActionResult UserProfile()
        {
            var userid = Filters.AuthenticationModel.GlobalUser.getGlobalUser().UserId;
            var user = _userBusiness.GetListWT(c => c.UserId == userid).FirstOrDefault();
            UserViewModel userViewModel = new UserViewModel();
            userViewModel.TokenKey = user.TokenKey;
            userViewModel.UserName = user.FirstName + " " + user.LastName;
            return View(userViewModel);
        }

        [Route("ProfileOverview/{tkn}")]
        public ActionResult ProfileOverview(string tkn)
        {
            var user = _userBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<User, UserViewModel>();
            var vmUser = Mapper.Map<User, UserViewModel>(user);
            return PartialView("_ProfileOverview", vmUser);
        }

        [Route("ProfileEdit/{tkn}")]
        public ActionResult ProfileEdit(string tkn)
        {
            var user = _userBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<User, UserProfileViewModel>();
            var vmUser = Mapper.Map<User, UserProfileViewModel>(user);
            vmUser.genderList = _userBusiness.GetGenderList();
            return PartialView("_ProfileEdit", vmUser);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("ProfileEdit")]
        public ActionResult ProfileEdit(UserProfileViewModel userViewModel)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Update Successful!!";

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _userBusiness.GetListWT(c => c.TokenKey == userViewModel.TokenKey).FirstOrDefault();
                    user.FirstName = userViewModel.FirstName;
                    user.LastName = userViewModel.LastName;
                    user.Gender = userViewModel.Gender;
                    user.BirthDate = userViewModel.BirthDate;

                    user.Email = userViewModel.EMail;
                    user.Phone = userViewModel.Phone;
                    user.Mobile = userViewModel.Mobile;

                    user.Address = userViewModel.Address;
                    user.Pincode = userViewModel.Pincode;
                    user.City = userViewModel.City;

                    _userBusiness.Update(user);
                    _unitOfWork.SaveChanges();
                }
                catch
                {
                    message = "Update Failed!!";
                    isSuccess = false;
                    _unitOfWork.Dispose();
                }
            }
            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [Route("ChangeProfileImage/{tkn}")]
        public ActionResult ChangeProfileImage(string tkn)
        {
            var changeProfileImage = new ChangeProfileImageViewModel();
            changeProfileImage.TokenKey = tkn;
            return PartialView("_ChangeProfileImage", changeProfileImage);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("ChangeProfileImage")]
        public ActionResult ChangeProfileImage(ChangeProfileImageViewModel changeProfileImage)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Update Successful!";
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _userBusiness.GetListWT(c => c.TokenKey == changeProfileImage.TokenKey).FirstOrDefault();

                    FileOperations.CreateDirectory(Server.MapPath("~/ProfileImage"));
                    if (changeProfileImage.ProfileImageUpload != null)
                    {
                        string ext = Path.GetExtension(changeProfileImage.ProfileImageUpload.FileName).ToLower();
                        string filename = changeProfileImage.TokenKey + ext;

                        string filePath = Server.MapPath("~/ProfileImage/") + filename;
                        changeProfileImage.ProfileImageUpload.SaveAs(filePath);
                        user.ProfileImage = filename;
                        _userBusiness.Update(user);
                        _unitOfWork.SaveChanges();
                    }


                }
                catch (Exception ex)
                {
                    message = "Update Failed!!";
                    isSuccess = false;
                    _unitOfWork.Dispose();
                }
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [Route("ChangePassword/{tkn}")]
        public ActionResult ChangePassword(string tkn)
        {
            var changePassword = new ChangePasswordViewModel();
            changePassword.TokenKey = tkn;
            return PartialView("_ChangePassword", changePassword);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordViewModel changePassword)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Password changed successfully!!";
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _userBusiness.GetListWT(c => c.TokenKey == changePassword.TokenKey).FirstOrDefault();
                    var validpassword = _userBusiness.ValidatePassword(user, changePassword.OldPassword, changePassword.Password);
                    if (validpassword.Key)
                    {
                        user.Password = Md5Encryption.Encrypt(changePassword.Password);
                        _userBusiness.Update(user);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        isSuccess = false;
                        message = validpassword.Value;
                    }
                }
                catch (Exception ex)
                {
                    message = "Failed to change password!!";
                    isSuccess = false;
                    _unitOfWork.Dispose();
                }
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [Route("{tkn}")]
        public ActionResult Detail(string tkn)
        {
            if (string.IsNullOrEmpty(tkn))
            {
                return RedirectToAction("Index");
            }

            var user = _userBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<User, UserViewModel>();
            var vmUser = Mapper.Map<User, UserViewModel>(user);
            return View(vmUser);
        }


        [Route("Overview/{tkn}")]
        public ActionResult Overview(string tkn)
        {
            var user = _userBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<User, UserViewModel>();
            var vmUser = Mapper.Map<User, UserViewModel>(user);
            return PartialView("_Overview", vmUser);
        }

        [Route("Create")]
        public ActionResult Create()
        {
            UserViewModel userViewModel = new UserViewModel();
            userViewModel.genderList = _userBusiness.GetGenderList();
            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create(UserViewModel userViewModel)
        {
            userViewModel.genderList = _userBusiness.GetGenderList();
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<UserViewModel, User>();
                User user = Mapper.Map<UserViewModel, User>(userViewModel);
                var result = _userBusiness.ValidateUser(user, "I");
                if (!string.IsNullOrEmpty(result))
                {
                    TempData["Success"] = result;
                    TempData["isSuccess"] = "false";
                    return View(userViewModel);
                }

                //saving profile image
                user.TokenKey = GlobalMethods.GetToken();
                user.UserType = "Admin";
                user.Password = Md5Encryption.Encrypt(userViewModel.Password);
                FileOperations.CreateDirectory(Server.MapPath("~/ProfileImage"));
                if (userViewModel.ProfileImageUpload != null)
                {
                    string ext = Path.GetExtension(userViewModel.ProfileImageUpload.FileName).ToLower();
                    string filename = user.TokenKey + ext;

                    string filePath = Server.MapPath("~/ProfileImage/") + filename;
                    userViewModel.ProfileImageUpload.SaveAs(filePath);
                    user.ProfileImage = filename;
                }
                user.IsBlocked = false;
                bool isSuccess = _userBusiness.AddUpdateDeleteUser(user, "I");
                if (isSuccess)
                {
                    TempData["Success"] = "User Created Successfully!!";
                    TempData["isSuccess"] = "true";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Success"] = "Failed to create User!!";
                    TempData["isSuccess"] = "false";
                }
            }
            else
            {
                TempData["Success"] = ModelState.Values.SelectMany(m => m.Errors).FirstOrDefault().ErrorMessage;
                TempData["isSuccess"] = "false";
            }

            return View(userViewModel);
        }

        // GET: /Event/Edit/5    
        [Route("Edit/{tkn}")]
        public ActionResult Edit(string tkn)
        {
            var user = _userBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<User, UserProfileViewModel>();
            UserProfileViewModel userViewModel = Mapper.Map<User, UserProfileViewModel>(user);
            userViewModel.genderList = _userBusiness.GetGenderList();
            return PartialView("_Edit", userViewModel);
        }

        //
        // POST: /Event/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit(UserProfileViewModel userViewModel)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "User Updated Successfully!!";

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _userBusiness.GetListWT(c => c.TokenKey == userViewModel.TokenKey).FirstOrDefault();
                    user.FirstName = userViewModel.FirstName;
                    user.LastName = userViewModel.LastName;
                    user.Gender = userViewModel.Gender;
                    user.BirthDate = userViewModel.BirthDate;

                    user.Email = userViewModel.EMail;
                    user.Phone = userViewModel.Phone;
                    user.Mobile = userViewModel.Mobile;

                    user.Address = userViewModel.Address;
                    user.Pincode = userViewModel.Pincode;
                    user.City = userViewModel.City;

                    FileOperations.CreateDirectory(Server.MapPath("~/ProfileImage"));
                    if (userViewModel.ProfileImageUpload != null)
                    {
                        string ext = Path.GetExtension(userViewModel.ProfileImageUpload.FileName).ToLower();
                        string filename = user.TokenKey + ext;

                        string filePath = Server.MapPath("~/ProfileImage/") + filename;
                        userViewModel.ProfileImageUpload.SaveAs(filePath);
                        user.ProfileImage = filename;
                    }

                    _userBusiness.Update(user);
                    _unitOfWork.SaveChanges();
                }
                catch
                {
                    message = "Failed to update User!!";
                    isSuccess = false;
                    _unitOfWork.Dispose();
                }
            }
            else
            {
                message = ModelState.Values.SelectMany(m => m.Errors).FirstOrDefault().ErrorMessage;
                isSuccess = false;
            }
            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(string stkns)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Delete Successful!";
            _unitOfWork.BeginTransaction();
            try
            {
                foreach (string tokenkey in stkns.Substring(1).Split('~'))
                {
                    List<User> userList = _userBusiness.GetListWT(x => x.TokenKey == tokenkey).ToList();
                    foreach (User usr in userList)
                    {
                        int deleteId = Convert.ToInt32(usr.UserId);
                        var br = _userBusiness.Find(deleteId);
                        _userBusiness.Delete(br);
                        _unitOfWork.SaveChanges();
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                message = "Delete Unsuccessful!";
                throw ex;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }


        public JsonResult BlockUser(string stkns)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Block User Successful!";
            _unitOfWork.BeginTransaction();
            try
            {
                foreach (string tokenkey in stkns.Substring(1).Split('~'))
                {
                    List<User> userList = _userBusiness.GetListWT(x => x.TokenKey == tokenkey).ToList();
                    foreach (User usr in userList)
                    {
                        int UserId = Convert.ToInt32(usr.UserId);
                        var br = _userBusiness.Find(UserId);

                        if (br.IsBlocked == false)
                            br.IsBlocked = true;
                        else
                            br.IsBlocked = false;

                        _userBusiness.Update(br);

                        _unitOfWork.SaveChanges();
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                message = "Block User Unsuccessful!";
                throw ex;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }




    }




}
