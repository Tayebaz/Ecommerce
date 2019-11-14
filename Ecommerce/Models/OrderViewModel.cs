using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class OrderViewModel
    {
        public string TokenKey { get; set; }
        public long OrderId { get; set; }
        public string OrderedBy { get; set; }
        public string OrderStatus { get; set; }
        public System.DateTime OrderDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string OrderCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerName { get; set; }        
        public string Email { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string FullAddress { get; set; }
        public string ShippingOrder { get; set; }
        public string OrderedItems { get; set; }
        public string Vat { get; set; }
        public string Sat { get; set; }
        public string TotalPrice { get; set; }
        public string TaxTotalPrice { get; set; }
        public List<ItemListViewModel> orderItems { get; set; }
        public List<CartWishlistViewModel> cartwishlist { get; set; }
    }


    public class ItemListViewModel
    {
        public int ProductID { get; set; }
        public string TokenKey { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public Nullable<int> BrandId { get; set; }
        public int CategoryId { get; set; }
        public Nullable<int> ImageId { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }

        public Nullable<int> DiscountPercent { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public bool Availability { get; set; }
        public string Condition { get; set; }
        public List<ImageViewModel> ImageList { get; set; }
        public int quantity { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public string AttributeId { get; set; }
        public string Attributes { get; set; }
    }

    public class OrderedItemViewModel
    {
        public int ID { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int OrderQuantity { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> TotalPriceTAX { get; set; }
        public Nullable<double> SAT { get; set; }
        public Nullable<double> VAT { get; set; }
    }
}