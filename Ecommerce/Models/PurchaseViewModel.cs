using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class PurchaseViewModel
    {
        public long Id { get; set; }
        public int ProductId { get; set; }
        public string Product { get; set; }
        public List<System.Web.Mvc.SelectListItem> ProductList { get; set; }
        public string PurchaseDate { get; set; }
        public double Vat { get; set; }
        public double Sat { get; set; }
        public double TotalPrice { get; set; }
        public double TaxTotalPrice { get; set; }
        public string Name { get; set; }
        public string OrderCode { get; set; }
        public double Discount { get; set; }
        public string Quantity { get; set; }

    }
}