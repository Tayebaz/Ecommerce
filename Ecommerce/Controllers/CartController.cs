using Business;
using Commom.GlobalMethods;
using Common.GlobalData;
using Entities.Models;
using Filters.AuthenticationModel;
using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;

namespace GrihastiWebsite.Controllers
{
    public class CartController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private ProductBusiness _productBusiness;
        private ImageBusiness _ImageBusiness;
        private AddToCartBusiness _AddToCartBusiness;
        private WishListBusiness _WishListBusiness;
        public CartController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._productBusiness = new ProductBusiness(_df, this._unitOfWork);
            this._ImageBusiness = new ImageBusiness(_df, this._unitOfWork);
            this._AddToCartBusiness = new AddToCartBusiness(_df, this._unitOfWork);
            this._WishListBusiness = new WishListBusiness(_df, this._unitOfWork);
        }
        //
        // GET: /Cart/
        public ActionResult Index()
        {
            var productList = _productBusiness.GetListWT();
            var assignedProductList = new List<AddToCart>();
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            if (currentUserId > 0)
            {
                assignedProductList = _AddToCartBusiness.GetListWT(c => c.UserId == currentUserId);
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.AddToCart.ToString());
                if (!string.IsNullOrEmpty(products))
                {
                    assignedProductList = (from p in products.Split(',')
                                           select new AddToCart
                                           {
                                               ProductId = Convert.ToInt32(p.Split('~')[0]),
                                               Quantity = Convert.ToInt32(p.Split('~')[1]),
                                               Size = Convert.ToInt32(p.Split('~')[2]),
                                               Attributes = p.Split('~')[3]
                                           }).ToList();
                }
            }

            var imgList = _ImageBusiness.GetListWT();
            var vmProductList = (from c in productList
                                 join ap in assignedProductList
                                 on c.ProductID equals ap.ProductId
                                 select new CartWishlistViewModel
                                 {
                                     ProductID = c.ProductID,
                                     ProductCode = c.ProductCode,
                                     ProductName = c.ProductName,
                                     TokenKey = c.TokenKey,
                                     ShortDescription = c.ShortDescription,
                                     Price = _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes),
                                     DiscountPercent = c.DiscountPercent,
                                     DiscountedPrice = Math.Round(_productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes) - Decimal.Divide(c.DiscountPercent ?? 0, 100) * _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes)),
                                     SizeId = ap.Size.Value,
                                     Size = _productBusiness.GetSizeName(c.ProductID, ap.Size.Value),
                                     AttributeId = ap.Attributes,
                                     Attributes = _productBusiness.GetAttributes(c.ProductID, ap.Attributes),
                                     quantity = ap.Quantity,

                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images = "/ProductImage/" + il.Images
                                                  }).ToList()

                                 }).ToList();
            return View(vmProductList);
        }
    }
}
