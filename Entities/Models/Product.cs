using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Product
    {
        public Product()
        {
            this.Images = new List<Image>();
        }

        public int ProductID { get; set; }
        public string TokenKey { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public Nullable<int> BrandId { get; set; }
        public int CategoryId { get; set; }
        public Nullable<int> SubCategoryId { get; set; }
        public Nullable<int> DiscountPercent { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public int Availability { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public decimal Price { get; set; }
    }
}
