using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string TokenKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsBlocked { get; set; }
    }
}
