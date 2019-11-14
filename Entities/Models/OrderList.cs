using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class OrderList
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string Contact { get; set; }
        public string OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string DeliveryDate { get; set; }
        public string OrderQuantity { get; set; }
    }
}
