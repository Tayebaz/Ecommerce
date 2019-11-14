using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class ProductAttribute
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Attributes { get; set; }
        public decimal Price { get; set; }
    }
}
