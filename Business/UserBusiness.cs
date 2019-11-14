using Common.Cryptography;
using Common.GlobalData;
using Entities.Models;
using Filters.AuthenticationCore;
using Filters.AuthenticationModel;
using Repository.RepositoryFactoryCore;
using Repository.RepositoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace Business
{
    public class UserBusiness
    {
        private UserRepository _userRepository;
        private FormsAuthenticationFactory _formsAuthenticationFactory;
        private UnitOfWork _unitOfWork;

        public UserBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _userRepository = new UserRepository(dfactory);

            this._formsAuthenticationFactory = new FormsAuthenticationFactory();
        }

        public User Insert(User user)
        {
            _userRepository.Insert(user);
            return user;
        }

        public User Update(User user)
        {
            _userRepository.Update(user);
            return user;
        }

        public void Delete(long? id)
        {
            _userRepository.Delete(id);
        }

        public void Delete(User user)
        {
            _userRepository.Delete(user);
        }

        public User Find(long? id)
        {
            return _userRepository.Find(id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userRepository.Query().Select();
        }

        public Task<List<User>> GetListWTAsync(Expression<Func<User, bool>> exp = null, Func<IQueryable<User>, IOrderedQueryable<User>> orderby = null)
        {
            return _userRepository.GetListWithNoTrackingAsync(exp, orderby);
        }

        public List<User> GetListWT(Expression<Func<User, bool>> exp = null, Func<IQueryable<User>, IOrderedQueryable<User>> orderby = null)
        {
            return _userRepository.GetListWithNoTracking(exp, orderby);
        }


        public User GetUserById(int id)
        {
            return _userRepository.Query(u => u.UserId == id).Select().FirstOrDefault();
        }
        public User GetUserByemail(string email)
        {
            return _userRepository.Query(u => u.Email == email).Select().FirstOrDefault();
        }

        public User IsValidUser(string email, string password)
        {
            return _userRepository.Query(u => u.Email == email && u.Password == password).Select().FirstOrDefault();
        }

        public string CheckUser(User userchk, HttpContextBase httpContext)
        {
            var result = string.Empty;
            var email = userchk.Email;
            var password = Md5Encryption.Encrypt(userchk.Password);
            var usertype = userchk.UserType;

            var user = _userRepository.Query(u => u.Email == email && u.Password == password && u.UserType == usertype).Select().FirstOrDefault();
            if (user == null)
            {
                result = "invalid";
            }
            else
            {
                if (user.UserType == "Customer" && !user.IsConfirmed)
                {
                    result = "notconfirmed";
                }
                else
                {
                    _formsAuthenticationFactory.SetAuthCookie(httpContext, UserAuthenticationTicketBuilder.CreateAuthenticationTicket(user));
                    result = "valid";                   
                }
            }
            return result;
        }

        public bool AddUpdateDeleteUser(User user, string action)
        {
            bool isSuccess = true;
            try
            {               
                if (action == "I")
                {
                    Insert(user);
                }
                else if (action == "U")
                {

                    Update(user);
                }
                else if (action == "D")
                {
                    Delete(user);
                }
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                throw ex;
            }
            return isSuccess;
        }

        public bool ProfileUpdate(User user, string action, int vid)
        {
            bool isSuccess = true;
            try
            {
                user.Password = Md5Encryption.Encrypt(user.Password);




                if (action == "I")
                {
                    Insert(user);
                }
                else if (action == "U")
                {
                    Update(user);
                }
                else if (action == "D")
                {
                    Delete(user);
                }
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                throw ex;
            }
            return isSuccess;
        }

        //public List<SelectListItem> GetUserType()
        //{
        //    List<SelectListItem> userType = new List<SelectListItem>();
        //    if (Filters.AuthenticationModel.GlobalUser.getGlobalUser().UserType != "Admin")
        //        userType.Add(new SelectListItem { Text = "Admin", Value = "Admin" });

        //    if (Filters.AuthenticationModel.GlobalUser.getGlobalUser().UserType != "Super Admin")
        //        userType.Add(new SelectListItem { Text = "App User", Value = "App User" });
        //    return userType;
        //}


        public string ValidateUser(User userchk, string action)
        {
            string result = string.Empty;
            if (action == "I")
            {
                var user = _userRepository.Query(u => u.Email.ToLower() == userchk.Email.ToLower()).Select().FirstOrDefault();

                if (user != null)
                {
                    result = "Email already exists!";
                    return result;
                }

            }
            else if (action == "U")
            {
                var user = _userRepository.Query(u => u.Email.ToLower() == userchk.Email.ToLower()).Select().FirstOrDefault();


                if (user != null)
                {
                    result = "Email already exists!";
                    return result;
                }

            }
            return result;
        }

        public KeyValuePair<bool, string> ValidatePassword(User userchk, string oldPassword, string newPassword)
        {
            if (oldPassword == newPassword)
            {
                return new KeyValuePair<bool, string>(false, "Existing password and the new password are same, please change the password.");
            }

            if (userchk.Password != Md5Encryption.Encrypt(oldPassword))
            {
                return new KeyValuePair<bool, string>(false, "Entered old password is not valid.");
            }
            return new KeyValuePair<bool, string>(true, "valid");
        }

        public KeyValuePair<bool, string> ValidatenewPassword(User userchk, string newPassword)
        {
            //if (oldPassword == newPassword)
            //{
            //    return new KeyValuePair<bool, string>(false, "Existing password and the new password are same, please change the password.");
            //}

            //if (userchk.Password != Md5Encryption.Encrypt(oldPassword))
            //{
            //    return new KeyValuePair<bool, string>(false, "Entered old password is not valid.");
            //}
            return new KeyValuePair<bool, string>(true, "valid");
        }
        public List<SelectListItem> GetGenderList()
        {
            List<SelectListItem> questionType = new List<SelectListItem>();
            questionType.Add(new SelectListItem { Text = "Male", Value = "Male" });
            questionType.Add(new SelectListItem { Text = "Female", Value = "Female" });
            return questionType;
        }

        public void logOff()
        {
            _formsAuthenticationFactory.SignOut();
        }

        public User FindUserWT(Expression<Func<User, bool>> exp)
        {
            return _userRepository.GetWithNoTracking(exp);
        }
    }
}
