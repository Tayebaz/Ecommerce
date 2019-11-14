using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Common.ErrorLog
{
    public static class ErrorLog
    {
        public static void ErrorLogFileWrite(HandleErrorInfo errorInfo)
        {

            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                string logFilePath = HttpContext.Current.Server.MapPath("~") + "ErrorHistory\\ErrorLog.txt";

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
                streamWriter.WriteLine("------------------------------Date: " + DateTime.Now.ToString() + "--------------------------------");
                streamWriter.WriteLine("Controller: " + errorInfo.ControllerName);
                streamWriter.WriteLine("Action: " + errorInfo.ActionName);
                streamWriter.WriteLine("HttpCode: " + (errorInfo.Exception is HttpException ? ((HttpException)errorInfo.Exception).GetHttpCode() : 500).ToString());
                streamWriter.WriteLine("Error Message: " + errorInfo.Exception + Environment.NewLine);
            }
            finally
            {
                if (streamWriter != null) streamWriter.Close();
                if (fileStream != null) fileStream.Close();
            }
        }
    }
}
