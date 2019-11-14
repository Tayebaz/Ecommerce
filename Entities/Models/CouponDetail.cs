using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class CouponDetail
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public Nullable<int> DiscountVal { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTill { get; set; }
        public string ApplicableOn { get; set; }
        public string NotApplicableOn { get; set; }
        public string TokenKey { get; set; }
    }
}
