using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Brand
    {
        public int BrandId { get; set; }
        public string TokenKey { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
