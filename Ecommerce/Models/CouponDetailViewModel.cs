using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace umartcms.Models
{
    public class CouponDetailViewModel
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public Nullable<int> DiscountVal { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTill { get; set; }
        public string ApplicableOn { get; set; }
        public List<System.Web.Mvc.SelectListItem> CategoryList { get; set; }
        public string NotApplicableOn { get; set; }
        public List<System.Web.Mvc.SelectListItem> CategoryListForNotApplicable { get; set; }
        public string TokenKey { get; set; }
    }
}