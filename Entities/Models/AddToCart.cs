using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class AddToCart
    {
        public int ID { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
        public string Attributes { get; set; }
        public Nullable<int> Size { get; set; }
    }
}
