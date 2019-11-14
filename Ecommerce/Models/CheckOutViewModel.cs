using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class CheckOutViewModel
    {
        public long OrderId { get; set; }
        public string OrderedBy { get; set; }
        public string OrderStatus { get; set; }
        public System.DateTime OrderDate { get; set; }
        //public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string OrderCode { get; set; }
        [Required]
        public string FirstNameShopper { get; set; }
        [Required]
        public string LastNameShopper { get; set; }
        [Required]
        [EmailAddress]
        public string EmailShopper { get; set; }
        [Required]
        public string PhoneShopper { get; set; }
        //public string MobilePhoneShopper { get; set; }
       
        //public string AddressShopper { get; set; }
       
        //public string CityShopper { get; set; }
      
        //public string PincodeShopper { get; set; }
        //public string ShippingOrder { get; set; }
        public string OrderListJson { get; set; }
        public List<CartWishlistViewModel> OrderList { get; set; }
        public string PaymentType { get; set; }
        public string PaymentId { get; set; }
        public string BBCode { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsStoreClosed { get; set; }



    }
}