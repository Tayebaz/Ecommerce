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
using MvcPaging;

namespace GrihastiWebsite.Controllers
{
    public class ProductController : Controller
    {
        private const int DefaultPageSize = 30;
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private CategoryBusiness _categoryBusiness;
        private SubCategoryBusiness _subCategoryBusiness;
        private BrandBusiness _brandBusiness;
        private ProductBusiness _productBusiness;
        private ProductSizeBusiness _productSizeBusiness;
        private ProductAttributeBusiness _productAttributeBusiness;
        private ImageBusiness _ImageBusiness;
        private AddToCartBusiness _AddToCartBusiness;
        private WishListBusiness _WishListBusiness;
        private CompareListBusiness _CompareListBusiness;
        public ProductController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._categoryBusiness = new CategoryBusiness(_df, this._unitOfWork);
            this._subCategoryBusiness = new SubCategoryBusiness(_df, this._unitOfWork);
            this._brandBusiness = new BrandBusiness(_df, this._unitOfWork);
            this._productBusiness = new ProductBusiness(_df, this._unitOfWork);
            this._productSizeBusiness = new ProductSizeBusiness(_df, this._unitOfWork);
            this._productAttributeBusiness = new ProductAttributeBusiness(_df, this._unitOfWork);
            this._ImageBusiness = new ImageBusiness(_df, this._unitOfWork);
            this._AddToCartBusiness = new AddToCartBusiness(_df, this._unitOfWork);
            this._WishListBusiness = new WishListBusiness(_df, this._unitOfWork);
            this._CompareListBusiness = new CompareListBusiness(_df, this._unitOfWork);
        }
        //
        // GET: /Product/

        public ActionResult Index(int? page, int categoryid = 0, int subcategoryid = 0, string brand = "", string price = "", string color = "")
        {
            var breadcrumb = new List<KeyValuePair<string, string>>();
            breadcrumb.Add(new KeyValuePair<string, string>("Home", "/Home/index"));
            var productList = new List<Product>();
            if (categoryid != 0)
            {
                var category = _categoryBusiness.GetListWT(c => c.CategoryId == categoryid).FirstOrDefault();
                breadcrumb.Add(new KeyValuePair<string, string>(category.CategoryName, ""));
                productList = _productBusiness.GetListWT(c => c.CategoryId == categoryid);
            }
            if (subcategoryid != 0)
            {
                var subCategory = _subCategoryBusiness.GetListWT(c => c.SubCategoryId == subcategoryid).FirstOrDefault();
                var category = _categoryBusiness.GetListWT(c => c.CategoryId == subCategory.CategoryId).FirstOrDefault();
                breadcrumb.Add(new KeyValuePair<string, string>(category.CategoryName, "/Product/index?categoryid=" + category.CategoryId));
                breadcrumb.Add(new KeyValuePair<string, string>(subCategory.SubCategoryName, ""));
                productList = _productBusiness.GetListWT(c => c.SubCategoryId == subcategoryid);
            }

            if (!string.IsNullOrEmpty(brand))
            {
                var brands = brand.Split(',');
                productList = productList.Where(c => brands.Contains(c.BrandId.ToString())).ToList();
            }

            //if (string.IsNullOrEmpty(brand))
            //{
            //    var brand = _brandBusiness.GetListWT(c => c.BrandId == brandid).FirstOrDefault();              
            //    productList = _productBusiness.GetListWT(c => c.BrandId == brandid);               
            //}

            if (!string.IsNullOrEmpty(price))
            {
                var pricerange = price.Split(',');
                var lowerLimit = Convert.ToDecimal(pricerange[0]);
                var upperLimit = Convert.ToDecimal(pricerange[1]);
                productList = productList.Where(c => _productBusiness.GetDefaultPrice(c.ProductID) >= lowerLimit && _productBusiness.GetDefaultPrice(c.ProductID) <= upperLimit).ToList();
            }

            var imgList = _ImageBusiness.GetListWT();
            var vmProductList = (from c in productList
                                 select new ProductViewModel
                                 {

                                     ProductID = c.ProductID,
                                     ProductName = c.ProductName,
                                     TokenKey = c.TokenKey,
                                     ShortDescription = c.ShortDescription,
                                     Price = _productBusiness.GetDefaultPrice(c.ProductID),
                                     DiscountPercent = c.DiscountPercent,
                                     DiscountedPrice = Math.Round(_productBusiness.GetDefaultPrice(c.ProductID) - Decimal.Divide(c.DiscountPercent ?? 0, 100) * _productBusiness.GetDefaultPrice(c.ProductID)),
                                     Availability = c.Availability,
                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images = "/ProductImage/" + il.Images
                                                  }).ToList()

                                 }).ToList();

            ViewBag.BreadCrumb = breadcrumb.ToList();

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(vmProductList.ToPagedList(currentPageIndex, DefaultPageSize));
            //return View(vmProductList);
        }

        public ActionResult ProductAddToCart(int productid, int quantity, int size = 0, string attributes = "")
        {
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            var productSize = _productSizeBusiness.GetListWT(c => c.ProductId == productid).FirstOrDefault();
            var defaultSize = productSize == null ? 0 : productSize.Id;
            if (size == 0)
            {
                size = defaultSize;
            }
            if (currentUserId > 0)
            {
                var cartList = _AddToCartBusiness.GetListWT(c => c.UserId == currentUserId && c.ProductId == productid);
                AddToCart addToCart = new AddToCart();
                addToCart.ProductId = productid;
                addToCart.UserId = currentUserId;
                addToCart.Quantity = quantity;
                addToCart.Size = size;
                addToCart.Attributes = attributes;
                if (cartList.Count() <= 0)
                {
                    _AddToCartBusiness.Insert(addToCart);
                }
                else
                {
                    addToCart.ID = cartList.FirstOrDefault().ID;
                    _AddToCartBusiness.Update(addToCart);
                }
                _unitOfWork.SaveChanges();
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.AddToCart.ToString());
                var newValue = productid.ToString() + "~" + quantity.ToString() + "~" + size.ToString() + "~" + attributes.ToString();
                var value = mycookie.FormatCartCookieValue(products, newValue);
                var expireCookieTimeHr = Convert.ToInt32(ReadConfigData.GetAppSettingsValue("ExpireCookieTimeHr"));
                mycookie.SetCookie(Enumerator.CustomerAction.AddToCart.ToString(), value, expireCookieTimeHr);
            }

            return new EmptyResult();
        }
        public ActionResult AddToWishList(int productid, int size = 0, string attributes = "")
        {
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            var productSize = _productSizeBusiness.GetListWT(c => c.ProductId == productid).FirstOrDefault();
            var defaultSize = productSize == null ? 0 : productSize.Id;
            if (size == 0)
            {
                size = defaultSize;
            }
            if (currentUserId > 0)
            {
                var count = _WishListBusiness.GetListWT(c => c.UserId == currentUserId && c.ProductId == productid).Count();
                if (count <= 0)
                {
                    WishList wishList = new WishList();
                    wishList.ProductId = productid;
                    wishList.UserId = currentUserId;
                    _WishListBusiness.Insert(wishList);
                    _unitOfWork.SaveChanges();
                }
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.WishList.ToString());
                var value = string.Empty;
                if (string.IsNullOrEmpty(products))
                    value = productid.ToString() + "~" + size.ToString() + "~" + attributes.ToString();
                else
                {
                    if (!products.Split(',').Select(c => c.Split('~')[0]).Contains(productid.ToString()))
                        value = products + "," + productid.ToString() + "~" + size.ToString() + "~" + attributes.ToString();
                }
                var expireCookieTimeHr = Convert.ToInt32(ReadConfigData.GetAppSettingsValue("ExpireCookieTimeHr"));
                mycookie.SetCookie(Enumerator.CustomerAction.WishList.ToString(), value, expireCookieTimeHr);
            }

            return new EmptyResult();
        }

        public ActionResult AddToCompareList(int productid, int size = 0, string attributes = "")
        {
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            var productSize = _productSizeBusiness.GetListWT(c => c.ProductId == productid).FirstOrDefault();
            var defaultSize = productSize == null ? 0 : productSize.Id;
            if (size == 0)
            {
                size = defaultSize;
            }
            if (currentUserId > 0)
            {
                var count = _CompareListBusiness.GetListWT(c => c.UserId == currentUserId && c.ProductId == productid).Count();
                if (count <= 0)
                {
                    CompareList compareList = new CompareList();
                    compareList.ProductId = productid;
                    compareList.UserId = currentUserId;
                    _CompareListBusiness.Insert(compareList);
                    _unitOfWork.SaveChanges();
                }
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.CompareList.ToString());
                var value = string.Empty;
                if (string.IsNullOrEmpty(products))
                    value = productid.ToString() + "~" + size.ToString() + "~" + attributes.ToString();
                else
                {
                    if (!products.Split(',').Select(c => c.Split('~')[0]).Contains(productid.ToString()))
                        value = products + "," + productid.ToString() + "~" + size.ToString() + "~" + attributes.ToString();
                }
                var expireCookieTimeHr = Convert.ToInt32(ReadConfigData.GetAppSettingsValue("ExpireCookieTimeHr"));
                mycookie.SetCookie(Enumerator.CustomerAction.CompareList.ToString(), value, expireCookieTimeHr);
            }

            return new EmptyResult();
        }

        public ActionResult DeleteFromCartList(int productid)
        {
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            if (currentUserId > 0)
            {
                var cartProduct = _AddToCartBusiness.GetList(c => c.UserId == currentUserId && c.ProductId == productid).FirstOrDefault();
                if (cartProduct != null)
                {
                    _AddToCartBusiness.Delete(cartProduct);
                    _unitOfWork.SaveChanges();
                }
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.AddToCart.ToString());
                var productId = productid.ToString();
                var value = mycookie.FormatCartCookieValueAfterDelete(products, productId);
                var expireCookieTimeHr = Convert.ToInt32(ReadConfigData.GetAppSettingsValue("ExpireCookieTimeHr"));
                mycookie.SetCookie(Enumerator.CustomerAction.AddToCart.ToString(), value, expireCookieTimeHr);
            }
            return new EmptyResult();
        }

        public ActionResult DeleteFromWishList(int productid)
        {
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            if (currentUserId > 0)
            {
                var wishListProduct = _WishListBusiness.GetList(c => c.UserId == currentUserId && c.ProductId == productid).FirstOrDefault();
                if (wishListProduct != null)
                {
                    _WishListBusiness.Delete(wishListProduct);
                    _unitOfWork.SaveChanges();
                }
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.WishList.ToString());
                var newCookieValueString = string.Empty;
                foreach (var prdId in products.Split(','))
                {
                    if (prdId.Split('~')[0] != productid.ToString())
                    {
                        newCookieValueString = newCookieValueString + "," + prdId;
                    }
                }
                var value = string.Empty;
                if (!string.IsNullOrEmpty(newCookieValueString))
                    value = newCookieValueString.Substring(1);
                var expireCookieTimeHr = Convert.ToInt32(ReadConfigData.GetAppSettingsValue("ExpireCookieTimeHr"));
                mycookie.SetCookie(Enumerator.CustomerAction.WishList.ToString(), value, expireCookieTimeHr);
            }

            return new EmptyResult();
        }

        public ActionResult DeleteFromCompareList(int productid)
        {
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            if (currentUserId > 0)
            {
                var compareListProduct = _CompareListBusiness.GetList(c => c.UserId == currentUserId && c.ProductId == productid).FirstOrDefault();
                if (compareListProduct != null)
                {
                    _CompareListBusiness.Delete(compareListProduct);
                    _unitOfWork.SaveChanges();
                }
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.CompareList.ToString());
                var newCookieValueString = string.Empty;
                foreach (var prdId in products.Split(','))
                {
                    if (prdId.Split('~')[0] != productid.ToString())
                    {
                        newCookieValueString = newCookieValueString + "," + prdId;
                    }
                }
                var value = string.Empty;
                if (!string.IsNullOrEmpty(newCookieValueString))
                    value = newCookieValueString.Substring(1);
                var expireCookieTimeHr = Convert.ToInt32(ReadConfigData.GetAppSettingsValue("ExpireCookieTimeHr"));
                mycookie.SetCookie(Enumerator.CustomerAction.CompareList.ToString(), value, expireCookieTimeHr);
            }

            return new EmptyResult();
        }


        public List<ProductViewModel> GetWishList()
        {
            var productList = _productBusiness.GetListWT();
            var imgList = _ImageBusiness.GetListWT();
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            var assignedProductList = new List<WishList>();
            if (currentUserId > 0)
            {
                assignedProductList = _WishListBusiness.GetListWT(c => c.UserId == currentUserId).ToList();
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.WishList.ToString());
                if (!string.IsNullOrEmpty(products))
                {
                    assignedProductList = (from p in products.Split(',')
                                           select new WishList
                                           {
                                               ProductId = Convert.ToInt32(p.Split('~')[0]),
                                               Size = Convert.ToInt32(p.Split('~')[1]),
                                               Attributes = p.Split('~')[2]
                                           }).ToList();
                }
            }
            var vmProductList = (from c in productList
                                 join ap in assignedProductList
                                 on c.ProductID equals ap.ProductId
                                 select new ProductViewModel
                                 {
                                     ProductID = c.ProductID,
                                     ProductName = c.ProductName,
                                     TokenKey = c.TokenKey,
                                     ShortDescription = c.ShortDescription,
                                     Price = _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes),
                                     DiscountPercent = c.DiscountPercent,
                                     DiscountedPrice = Math.Round(_productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes) - Decimal.Divide(c.DiscountPercent ?? 0, 100) * _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes)),
                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images = "/ProductImage/" + il.Images
                                                  }).ToList()
                                 }).ToList();
            return vmProductList;
        }

        public List<CartWishlistViewModel> GetCartList()
        {
            var productList = _productBusiness.GetListWT();
            var assignedProductList = new List<AddToCart>();
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.AddToCart.ToString());
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            if (currentUserId > 0)
            {
                assignedProductList = _AddToCartBusiness.GetListWT(c => c.UserId == currentUserId);
            }
            else
            {
                if (!string.IsNullOrEmpty(products))
                {
                    assignedProductList = (from p in products.Split(',')
                                           select new AddToCart
                                           {
                                               ProductId = Convert.ToInt32(p.Split('~')[0]),
                                               Quantity = Convert.ToInt32(p.Split('~')[1]),
                                               Size = Convert.ToInt32(p.Split('~')[2]),
                                               Attributes = p.Split('~')[3] ?? ""
                                           }).ToList();
                }
            }

            var imgList = _ImageBusiness.GetListWT();
            // var AvailabilityList = _ProductSizeBusiness.GetListWT();
            var vmProductList = new List<CartWishlistViewModel>();
            vmProductList = (from c in productList
                                 join ap in assignedProductList
                                 on c.ProductID equals ap.ProductId
                                 
                                 select new CartWishlistViewModel
                                 {
                                     ProductID = c.ProductID,
                                     ProductCode = c.ProductCode,
                                     ProductName = c.ProductName,
                                     TokenKey = c.TokenKey,
                                     ShortDescription = c.ShortDescription,
                                     Price = _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.GetValueOrDefault(), ap.Attributes),
                                     DiscountPercent = 0,//c.DiscountPercent,
                                     DiscountedPrice = 0,//Math.Round(_productBusiness.GetSelectedPrice(c.ProductID, ap.Size.GetValueOrDefault(), ap.Attributes) - Decimal.Divide(c.DiscountPercent ?? 0, 100) * _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes)),
                                     quantity = ap.Quantity,
                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images = "/ProductImage/" + il.Images
                                                  }).ToList()

                                 }).ToList();
            return vmProductList;
        }

        public List<ProductViewModel> GetCompareList()
        {
            var productList = _productBusiness.GetListWT();
            var imgList = _ImageBusiness.GetListWT();
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            var assignedProductList = new List<WishList>();
            if (currentUserId > 0)
            {
                assignedProductList = _WishListBusiness.GetListWT(c => c.UserId == currentUserId).ToList();
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var products = mycookie.GetCookie(Enumerator.CustomerAction.CompareList.ToString());
                if (!string.IsNullOrEmpty(products))
                {
                    assignedProductList = (from p in products.Split(',')
                                           select new WishList
                                           {
                                               ProductId = Convert.ToInt32(p.Split('~')[0]),
                                               Size = Convert.ToInt32(p.Split('~')[1]),
                                               Attributes = p.Split('~')[2]
                                           }).ToList();
                }
            }
            var vmProductList = (from c in productList
                                 join ap in assignedProductList
                                 on c.ProductID equals ap.ProductId
                                 select new ProductViewModel
                                 {
                                     ProductID = c.ProductID,
                                     ProductName = c.ProductName,
                                     TokenKey = c.TokenKey,
                                     ShortDescription = c.ShortDescription,
                                     Price = _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes),
                                     DiscountPercent = c.DiscountPercent,
                                     DiscountedPrice = Math.Round(_productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes) - Decimal.Divide(c.DiscountPercent ?? 0, 100) * _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes)),
                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images = "/ProductImage/" + il.Images
                                                  }).ToList()
                                 }).ToList();
            return vmProductList;
        }


        public ActionResult ClearShoppingCart()
        {
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            if (currentUserId > 0)
            {
                var cartData = _AddToCartBusiness.GetList(c => c.UserId == currentUserId);
                foreach (var product in cartData)
                {
                    _AddToCartBusiness.Delete(product);
                    _unitOfWork.SaveChanges();
                }
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var expireCookieTimeHr = Convert.ToInt32(ReadConfigData.GetAppSettingsValue("ExpireCookieTimeHr"));
                mycookie.SetCookie(Enumerator.CustomerAction.AddToCart.ToString(), "", expireCookieTimeHr);
            }
            return RedirectToAction("Index", "Cart");
        }

        public ActionResult Search(int? page, string searchstring = "", string price = "", string color = "")
        {
            var categoryList = _categoryBusiness.GetListWT(c => c.CategoryName.Contains(searchstring)).Select(col => col.CategoryId).ToList();
            var subcategoryList = _subCategoryBusiness.GetListWT(c => c.SubCategoryName.Contains(searchstring)).Select(col => col.SubCategoryId).ToList();
            var brandList = _brandBusiness.GetListWT(c => c.BrandName.Contains(searchstring)).Select(col => col.BrandId).ToList();

            var productList = _productBusiness.GetListWT(c => c.ProductName.Contains(searchstring) || c.Description.Contains(searchstring) || categoryList.Contains(c.CategoryId)
                || subcategoryList.Contains(c.CategoryId) || brandList.Contains(c.BrandId ?? 0));

            if (!string.IsNullOrEmpty(price))
            {
                var pricerange = price.Split(',');
                var lowerLimit = Convert.ToDecimal(pricerange[0]);
                var upperLimit = Convert.ToDecimal(pricerange[1]);
                productList = productList.Where(c => _productBusiness.GetDefaultPrice(c.ProductID) >= lowerLimit && _productBusiness.GetDefaultPrice(c.ProductID) <= upperLimit).ToList();
            }

            var imgList = _ImageBusiness.GetListWT();
            var vmProductList = (from c in productList
                                 select new ProductViewModel
                                 {
                                     ProductID = c.ProductID,
                                     ProductName = c.ProductName,
                                     TokenKey = c.TokenKey,
                                     ShortDescription = c.ShortDescription,
                                     Price = _productBusiness.GetDefaultPrice(c.ProductID),
                                     DiscountPercent = c.DiscountPercent,
                                     DiscountedPrice = Math.Round(_productBusiness.GetDefaultPrice(c.ProductID) - Decimal.Divide(c.DiscountPercent ?? 0, 100) * _productBusiness.GetDefaultPrice(c.ProductID)),
                                     Availability = c.Availability,
                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images = "/ProductImage/" + il.Images
                                                  }).ToList()

                                 }).ToList();

            ViewBag.SearchText = searchstring;


            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(vmProductList.ToPagedList(currentPageIndex, DefaultPageSize));
            //return View(vmProductList);
        }
    }
}
