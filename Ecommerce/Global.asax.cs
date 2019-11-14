using Filters.AuthenticationCore;
using Filters.AuthenticationModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Ecommerce
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CultureInfo culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        private void Application_AuthenticateRequest(object Sender, EventArgs e)
        {
            HttpCookie authCookie = this.Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (IsValidAuthCookie(authCookie))
            {
                var formsAuthentication = DependencyResolver.Current.GetService<FormsAuthenticationFactory>();
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                FormsAuthentication.RenewTicketIfOld(ticket);
                var authUser = new AuthoringUser(ticket);
                string[] userRoles = { authUser.UserType };
                this.Context.User = new GenericPrincipal(authUser, userRoles);
                //formsAuthentication.SetAuthCookie(this.Context, ticket);
            }
        }

        private static bool IsValidAuthCookie(HttpCookie authCookie)
        {
            return authCookie != null && !String.IsNullOrEmpty(authCookie.Value);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            string filename = "ErrorLog.txt";
            Exception exception = Server.GetLastError();
            CreateTextFile(exception.Message, filename);
            Server.ClearError();
        }

        public void CreateTextFile(string message, string filename)
        {

            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                string logFilePath = HttpContext.Current.Server.MapPath("~") + "ErrorHistory\\" + filename;

                if (logFilePath.Equals("")) return;

                #region Create the Log file directory if it does not exists
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo = new FileInfo(logFilePath);
                logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                #endregion Create the Log file directory if it does not exists

                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append);
                }
                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine("------------------------------Date: " + DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss") + "--------------------------------");
                streamWriter.WriteLine(message + Environment.NewLine);
            }
            finally
            {
                if (streamWriter != null) streamWriter.Close();
                if (fileStream != null) fileStream.Close();
            }
        }
    }
}
    