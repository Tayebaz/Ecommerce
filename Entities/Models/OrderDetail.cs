using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class OrderDetail
    {
        public long OrderId { get; set; }
        public string OrderedBy { get; set; }
        public string OrderStatus { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        //public string MobilePhone { get; set; }
        //public string Address { get; set; }
        //public string City { get; set; }
        //public string Pincode { get; set; }
        //public string ShippingOrder { get; set; }
        public string OrderCode { get; set; }
        public string TotalPrice { get; set; }
        //public Nullable<double> Discount { get; set; }
        public string PaymentType { get; set; }
        public string PaymentId { get; set; }
        public string TokenKey { get; set; }
    }
}
