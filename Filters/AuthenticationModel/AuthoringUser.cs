using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Filters.AuthenticationModel
{
    [Serializable]
    public class AuthoringUser:IIdentity
    {
        public Int64 UserId { get; set; }
        public string Name { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string AuthenticationType { get { return "Ecommerce"; } }
        public bool IsAuthenticated { get { return true; } }
        public AuthoringUser() { }
        public AuthoringUser(string name, Int64 userId, string FirstName, string LastName, string Email, string UserType)
        {
            this.Name = name;
            this.UserId = userId;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.UserType = UserType;
        }

        public AuthoringUser(string name, UserInfo userInfo)
            : this(name, userInfo.Id, userInfo.FirstName, userInfo.LastName, userInfo.Email, userInfo.UserType)
        {
            this.UserId = userInfo.Id;
        }
        public AuthoringUser(FormsAuthenticationTicket ticket)
            :this(ticket.Name,UserInfo.FromString(ticket.UserData))
        {
            if (ticket == null) throw new ArgumentNullException("ticket");
        }
    }
}
