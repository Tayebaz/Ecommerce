using Filters.AuthenticationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Filters.Extensions
{
    public static class SecurityExtensions
    {
        public static string Name(this IPrincipal user)
        {
            return user.Identity.Name;
        }

        public static bool InAnyRole(this IPrincipal user,params string[] roles)
        {
            foreach(string role in roles)
            {
                if (user.IsInRole(role)) return true;
            }
            return false;
        }

        public static AuthoringUser GetAuthoringUser(this IPrincipal principal)
        {
            if (principal.Identity is AuthoringUser)
                return (AuthoringUser)principal.Identity;
            else
                return new AuthoringUser(string.Empty, new UserInfo());
        }
    }
}
