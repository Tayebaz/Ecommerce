using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Cryptography;
using Common.GlobalData;
using Entities.Models;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.ComponentModel;

namespace Commom.GlobalMethods
{
    public class GlobalMethods
    {
        public static GlobalMethods NewInstance()
        {
            return new GlobalMethods();
        }
        // private UnitOfWork _unitOfWork;

        /// <summary>
        /// Get 32 character Token string 
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            return string.Format("{0:ddMMyyyyhhmmssffffff}", DateTime.Now) + Guid.NewGuid().ToString("N").Substring(0, 12);
        }

        public static List<SelectListItem> GetAvailabilityList()
        {
            var selectList = new List<SelectListItem>();

            // Get all values of the Industry enum
            var enumValues = Enum.GetValues(typeof(Commom.GlobalMethods.Enumerator.Availability)) as Commom.GlobalMethods.Enumerator.Availability[];
            if (enumValues == null)
                return null;

            foreach (var enumValue in enumValues)
            {
                // Create a new SelectListItem element and set its 
                // Value and Text to the enum value and description.
                selectList.Add(new SelectListItem
                {
                    Value = ((int)enumValue).ToString(),
                    // GetIndustryName just returns the Display.Name value
                    // of the enum - check out the next chapter for the code of this function.
                    Text = enumValue.DescriptionAttr()
                });
            }

            return selectList;
        }

        public Setting GetSetting()
        {
            EcommerceContext db = new EcommerceContext();
            var setting = db.Settings.AsNoTracking().FirstOrDefault();
            if (setting == null)
            {
                setting = new Entities.Models.Setting();
            }
            return setting;
        }
    }
}
