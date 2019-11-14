using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Purchase
    {
        public int ID { get; set; }
        public Nullable<int> Productid { get; set; }
        public string Quantity { get; set; }
        public Nullable<double> TotalPrice { get; set; }
        public string Name { get; set; }
        public string PurchaseDate { get; set; }
    }
}
