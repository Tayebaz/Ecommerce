using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Commom.GlobalMethods
{
    public class CookieStore
    {
        public const string CookieName = "GrihastiCookie";
        public void SetCookie(string key, string value, int ExpireCookieTimeHr)
        {
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[CookieName];
                cookieOld.Expires = DateTime.Now.AddHours(ExpireCookieTimeHr);
                cookieOld[key] = value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                HttpCookie cookieNew = new HttpCookie(CookieName);
                cookieNew.Expires = DateTime.Now.AddHours(ExpireCookieTimeHr);
                cookieNew[key] = value;               
                HttpContext.Current.Response.Cookies.Add(cookieNew);
            }
        }
        public string GetCookie(string key) 
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];

            if (cookie != null)
            {
                value = HttpContext.Current.Request.Cookies[CookieName][key] ?? "";
            }
            return value;
        }

        public string FormatCartCookieValue(string oldvalue, string newvalue)
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(oldvalue))
            {
                result = newvalue;
            }
            else
            {
                var oldValues = oldvalue.Split(',');            
                var newCookieValueString = string.Empty;
                var newValueAlreadyExists = false;
                foreach (var value in oldValues)
                {
                    var newCookieValue = string.Empty;
                    var values = value.Split('~');
                    var newValues = newvalue.Split('~');
                    if (values[0] == newValues[0])
                    {
                        newCookieValue = values[0] + "~" + (Convert.ToInt32(newValues[1])).ToString() + "~" + (Convert.ToInt32(newValues[2])).ToString() + "~" + (newValues[3]).ToString();
                        newValueAlreadyExists = true;
                    }
                    else
                    {
                        newCookieValue = value;
                    }
                    newCookieValueString = newCookieValueString + "," + newCookieValue;
                }
                if(!newValueAlreadyExists)
                    newCookieValueString = newCookieValueString + "," + newvalue;
                result = newCookieValueString.Substring(1);
            }
            return result;
        }

        public string FormatCartCookieValueAfterDelete(string oldvalue, string productId)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(oldvalue))
            {
                var oldValues = oldvalue.Split(',');
                var newCookieValueString = string.Empty;              
                foreach (var value in oldValues)
                {
                    var newCookieValue = string.Empty;
                    var values = value.Split('~');
                    if (values[0] != productId)
                    {
                        newCookieValueString = newCookieValueString + "," + value;
                    }
                }
                if (!string.IsNullOrEmpty(newCookieValueString))
                    result = newCookieValueString.Substring(1);
            }
            return result;
        }
    }
}