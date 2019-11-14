using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string TokenKey { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])((?=.*?[0-9])|(?=.*?[#?!@$%^&*-])).{8,}$", ErrorMessage = "Minimum 8 characters at least 1 Uppercase, 1 Lowercase letter and 1 Number Or 1 Special Character")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Gender { get; set; }
        public List<System.Web.Mvc.SelectListItem> genderList { get; set; }
        [Display(Name = "Birth Date")]
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string UserType { get; set; }
        public string ProfileImage { get; set; }       
        [DataType(DataType.Upload)]
        [Display(Name = "Profile Picture")]
        public HttpPostedFileBase ProfileImageUpload { get; set; }
        public string UserName { get; set; }
        
        [Required]
        public bool IsBlocked { get; set; }

    }

    public class UserProfileViewModel
    {
        public int UserId { get; set; }
        public string TokenKey { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EMail { get; set; }
        [Required]       
        public string Gender { get; set; }
        public List<System.Web.Mvc.SelectListItem> genderList { get; set; }
        [Display(Name = "Birth Date")]
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }

        public string ProfileImage { get; set; }       
        [DataType(DataType.Upload)]
        [Display(Name = "Profile Picture")]
        public HttpPostedFileBase ProfileImageUpload { get; set; }
    }

    public class ChangeProfileImageViewModel
    {
        public string TokenKey { get; set; }            
      
        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Profile Picture")]
        public HttpPostedFileBase ProfileImageUpload { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public string TokenKey { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])((?=.*?[0-9])|(?=.*?[#?!@$%^&*-])).{8,}$", ErrorMessage = "Minimum 8 characters at least 1 Uppercase, 1 Lowercase letter and 1 Number Or 1 Special Character")]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])((?=.*?[0-9])|(?=.*?[#?!@$%^&*-])).{8,}$", ErrorMessage = "Minimum 8 characters at least 1 Uppercase, 1 Lowercase letter and 1 Number Or 1 Special Character")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    
}