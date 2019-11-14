using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class OrderedItem
    {
        public int ID { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int OrderQuantity { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> DiscountPercent { get; set; }
        public Nullable<int> Size { get; set; }
        public string Attributes { get; set; }
    }
}
