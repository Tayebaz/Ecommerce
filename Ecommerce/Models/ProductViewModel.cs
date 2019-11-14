using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string TokenKey { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ProductCode { get; set; }        
        public Nullable<int> BrandId { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public Nullable<int> SubCategoryId { get; set; }
       
        public Nullable<int> DiscountPercent { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        [Required]
        public int Availability { get; set; }
        public List<System.Web.Mvc.SelectListItem> AvailabilityList { get; set; }
        public string Brand { get; set; }
        public List<System.Web.Mvc.SelectListItem> BrandList { get; set; }
        public string Category { get; set; }
        public List<System.Web.Mvc.SelectListItem> CategoryList { get; set; }

        public List<System.Web.Mvc.SelectListItem> SubCategoryList { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase ImageUpload { get; set; }
        public List<ImageViewModel> ImageList { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }

        public string Attributes { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int ProductQuantity { get; set; }

        public decimal DiscountedPrice { get; set; }
        public List<ProductSizeViewModel> ProductSizeList { get; set; }
        public List<ProductAttributeViewModel> ProductAttributeList { get; set; }
    }

    public class ProductSizeViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
    }

    public partial class ProductAttributeViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Attributes { get; set; }
        public decimal Price { get; set; }
    }
    
    public class ImageViewModel
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public string Images { get; set; }
    }

    public class ReviewViewModel
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Review1 { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int Rating { get; set; }
        public int ProductId { get; set; }
    }
    public class CartWishlistViewModel
    {
        public int ProductID { get; set; }
        public string TokenKey { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public Nullable<int> BrandId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> ImageId { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public Nullable<int> DiscountPercent { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public int Availability { get; set; }
        public List<ImageViewModel> ImageList { get; set; }
        public int quantity { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public string AttributeId { get; set; }
        public string Attributes { get; set; }
    }
}