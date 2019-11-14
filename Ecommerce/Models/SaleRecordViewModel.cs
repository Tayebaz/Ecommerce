using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class SaleRecordViewModel
    {
        public long OrderId { get; set; }
        
        public System.DateTime OrderDate { get; set; }
        public double Vat { get; set; }
        public double Sat { get; set; }
        public double TotalPrice { get; set; }
        public double TaxTotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string OrderCode { get; set; }
        public double Discount { get; set; }
        public string OrderStatus { get; set; }

    }
}