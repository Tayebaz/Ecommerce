using Business;
using Commom.GlobalMethods;
using Common.GlobalData;
using Entities.Models;
using Filters.AuthenticationModel;
using Ecommerce.Models;
using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace umart.Controllers
{
    public class CompareListController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private ProductBusiness _productBusiness;
        private ImageBusiness _ImageBusiness;
        private AddToCartBusiness _AddToCartBusiness;
        private CompareListBusiness _CompareListBusiness;
        private CategoryBusiness _CategoryBusiness;
        public CompareListController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._productBusiness = new ProductBusiness(_df, this._unitOfWork);
            this._ImageBusiness = new ImageBusiness(_df, this._unitOfWork);
            this._AddToCartBusiness = new AddToCartBusiness(_df, this._unitOfWork);
            this._CompareListBusiness = new CompareListBusiness(_df, this._unitOfWork);
            this._CategoryBusiness = new CategoryBusiness(_df, this._unitOfWork);
        }
        //
        // GET: /Cart/

        public ActionResult Index()
        {
            var productList = _productBusiness.GetListWT();

            var assignedProductList = new List<CompareList>();
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            if (currentUserId > 0)
            {
                assignedProductList = _CompareListBusiness.GetListWT(c => c.UserId == currentUserId);
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.CompareList.ToString());
                if (!string.IsNullOrEmpty(products))
                {
                    assignedProductList = (from p in products.Split(',')
                                           select new CompareList
                                           {
                                               ProductId = Convert.ToInt32(p.Split('~')[0]),
                                               Size = Convert.ToInt32(p.Split('~')[1]),
                                               Attributes = p.Split('~')[2]
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
                                     Availability = c.Availability,
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