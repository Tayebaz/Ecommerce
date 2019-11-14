using Common.GlobalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Commom
{
    public static class MailManagement
    {
        public static bool SendEmail(string subject, string toEmail, string message, string fromEmail = "", string fromPassword = "", string smtpAddress = "", bool isBodyHtml = true, HttpPostedFileBase[] FileCollection = null)
        {
            bool isSuccess = true;
            try
            {
                int portNumber = Convert.ToInt32(GetPort());
                bool enableSSL = false;
                string emailFrom = fromEmail;
                string password = fromPassword;

                if (smtpAddress == "")
                {
                    smtpAddress = GetIPAddress();
                }

                if (fromEmail == "")
                {
                    emailFrom = GetDefaultFromEmail();                   
                }

                if(fromPassword == "")
                {
                    password = GetDefaultPassword();
                }

                string emailTo = toEmail;
                string body = message;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = isBodyHtml;
                    //If attachment Exists
                    if (FileCollection != null)
                    {
                        foreach (var item in FileCollection)
                        {
                            mail.Attachments.Add(new Attachment(GetFileServerAddress() + item.FileName));
                        }
                    }
                    ////
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception e)
            {
                var trace = e.StackTrace;
                // AssignOperationMessage.messageList.Add(new MessageList { IsSuccess = false, Message = "failed sending mail!!", OperationName = "Mail Send" });
                isSuccess = false;
            }
            return isSuccess;
        }

        public static bool SendEmailWithCC(string subject, string toEmail, string message, string fromEmail = "", string fromPassword = "", string smtpAddress = "", bool isBodyHtml = true, HttpPostedFileBase[] FileCollection = null, List<string> emailGroup = null)
        {
            bool isSuccess = true;
            try
            {               
                int portNumber = Convert.ToInt32(GetPort());
                bool enableSSL = true;
                string emailFrom = fromEmail;
                string password = fromPassword;

                if (smtpAddress == "")
                {
                    smtpAddress = GetIPAddress();
                }                

                if (fromEmail == "")
                {
                    emailFrom = GetDefaultFromEmail();
                }

                if (fromPassword == "")
                {
                    password = GetDefaultPassword();
                }

                string emailTo = toEmail;
                string body = message;
                int ccCount = 1;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    //mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = isBodyHtml;
                    if (emailGroup != null)
                    {
                        foreach (var item in emailGroup)
                        {
                            if (ccCount > 1)
                            {
                                MailAddress mAddress = new MailAddress(item);
                                mail.CC.Add(mAddress);
                                ccCount++;
                            }
                            else
                            {
                                mail.To.Add(item);
                                ccCount++;
                            }
                        }
                    }
                    //If attachment Exists
                    if (FileCollection != null)
                    {
                        foreach (var item in FileCollection)
                        {
                            mail.Attachments.Add(new Attachment(GetFileServerAddress() + item.FileName));
                        }
                    }
                    ////
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception e)
            {
                var trace = e.StackTrace;
                //  AssignOperationMessage.messageList.Add(new MessageList { IsSuccess = false, Message = "failed sending mail!!", OperationName = "Mail Send" });
                isSuccess = false;
            }
            return isSuccess;
        }

        private static string GetDefaultFromEmail()
        {
            string fromEmail = ReadConfigData.GetAppSettingsValue("FromEmail");
            return fromEmail;
        }

        private static string GetDefaultPassword()
        {
            string fromEmailPassword = ReadConfigData.GetAppSettingsValue("EmailPassword");
            return fromEmailPassword;
        }

        private static string GetDefaultfileName()
        {
            string defaultFileName = "TestAttachment";
            return defaultFileName;
        }

        private static string GetFileServerAddress()
        {
            string serverAddress = "C:\\";
            return serverAddress;
        }

        private static string GetIPAddress()
        {
            string nedIp = ReadConfigData.GetAppSettingsValue("Host");
            return nedIp;
        }

        private static string GetPort()
        {
            string nedIp = ReadConfigData.GetAppSettingsValue("Port");
            return nedIp;
        }
    }
}
