using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class ProductSize
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
    }
}
