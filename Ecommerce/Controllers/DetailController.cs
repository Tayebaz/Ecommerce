using AutoMapper;
using Business;
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
using Commom.GlobalMethods;

namespace GrihastiWebsite.Controllers
{
    public class DetailController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private ProductBusiness _productBusiness;
        private ProductSizeBusiness _productSizeBusiness;
        private ProductAttributeBusiness _productAttributeBusiness;
        private ImageBusiness _ImageBusiness;
        private ReviewBusiness _ReviewBusiness;
        private UserBusiness _userBusiness;
        private CategoryBusiness _CategoryBusiness;
        private SubCategoryBusiness _SubCategoryBusiness;

        public DetailController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._productBusiness = new ProductBusiness(_df, this._unitOfWork);
            this._ImageBusiness = new ImageBusiness(_df, this._unitOfWork);
            this._ReviewBusiness = new ReviewBusiness(_df, this._unitOfWork);
            this._userBusiness = new UserBusiness(_df, this._unitOfWork);
            this._CategoryBusiness = new CategoryBusiness(_df, this._unitOfWork);
            this._SubCategoryBusiness = new SubCategoryBusiness(_df, this._unitOfWork);
            this._productSizeBusiness = new ProductSizeBusiness(_df, this._unitOfWork);
            this._productAttributeBusiness = new ProductAttributeBusiness(_df, this._unitOfWork);
        }
        //
        // GET: /ProductDetail/

        public ActionResult Index(int productId)
        {
            var product = _productBusiness.GetListWT(c => c.ProductID == productId).FirstOrDefault();
            var imgList = _ImageBusiness.GetListWT(c => c.ProductId == productId);

            Mapper.CreateMap<Product, ProductViewModel>();
            var vmProduct = Mapper.Map<Product, ProductViewModel>(product);

            CookieStore mycookie = new CookieStore();
            var assignedProductList = new List<AddToCart>();
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

            var productAssigned = assignedProductList.Where(c => c.ProductId == productId).FirstOrDefault();
            if (productAssigned != null)
            {
                vmProduct.SizeId = productAssigned.Size.Value;
                vmProduct.Attributes = productAssigned.Attributes;
                vmProduct.Price = _productBusiness.GetSelectedPrice(productId, productAssigned.Size.Value, productAssigned.Attributes) * productAssigned.Quantity;
                vmProduct.ProductQuantity = productAssigned.Quantity;
            }
            else
            {
                var defaultSize = _productSizeBusiness.GetListWT(c => c.ProductId == productId).FirstOrDefault();
                vmProduct.SizeId = defaultSize == null ? 0 : defaultSize.Id;
                vmProduct.Attributes = "";
                vmProduct.Price = _productBusiness.GetDefaultPrice(productId);
                vmProduct.ProductQuantity = 1;
            }
         


            vmProduct.DiscountPercent = vmProduct.DiscountPercent ?? 0;
            vmProduct.DiscountedPrice = Math.Round(vmProduct.Price - Decimal.Divide(vmProduct.DiscountPercent ?? 0, 100) * vmProduct.Price);
            vmProduct.ImageList = (from il in imgList
                                   where (il.ProductId == product.ProductID)
                                   select new ImageViewModel
                                   {
                                       ProductId = product.ProductID,
                                       Images = "/ProductImage/" + il.Images
                                   }).ToList();

            vmProduct.ProductSizeList = (from ps in _productSizeBusiness.GetListWT(c => c.ProductId == productId).ToList()
                                         select new ProductSizeViewModel
                                         {
                                             Id = ps.Id,
                                             ProductId = ps.ProductId,
                                             Price = ps.Price,
                                             Size = ps.Size
                                         }).ToList();
            vmProduct.ProductAttributeList = (from ps in _productAttributeBusiness.GetListWT(c => c.ProductId == productId).ToList()
                                              select new ProductAttributeViewModel
                                              {
                                                  Id = ps.Id,
                                                  ProductId = ps.ProductId,
                                                  Price = ps.Price,
                                                  Attributes = ps.Attributes
                                              }).ToList();




            var breadcrumb = new List<KeyValuePair<string, string>>();
            breadcrumb.Add(new KeyValuePair<string, string>("Home", "/Home/index"));
            var category = _CategoryBusiness.GetListWT(c => c.CategoryId == vmProduct.CategoryId).FirstOrDefault();
            if (category != null)
                breadcrumb.Add(new KeyValuePair<string, string>(category.CategoryName, "/Product/index?categoryid=" + category.CategoryId));
            var subcategory = _SubCategoryBusiness.GetListWT(c => c.SubCategoryId == vmProduct.SubCategoryId).FirstOrDefault();
            if (subcategory != null)
                breadcrumb.Add(new KeyValuePair<string, string>(subcategory.SubCategoryName, "/Product/index?subcategoryid=" + subcategory.SubCategoryId));
            breadcrumb.Add(new KeyValuePair<string, string>(vmProduct.ProductName, ""));
            ViewBag.BreadCrumb = breadcrumb.ToList();
            return View(vmProduct);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Review(ReviewViewModel reviewViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
                    var reveiwCount = _ReviewBusiness.GetListWT(c => c.UserId == currentUser && c.ProductId == reviewViewModel.ProductId).Count();
                    if (reveiwCount <= 0)
                    {
                        Mapper.CreateMap<ReviewViewModel, Review>();
                        var review = Mapper.Map<ReviewViewModel, Review>(reviewViewModel);
                        review.CreatedDate = DateTime.Now;
                        review.UserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
                        _ReviewBusiness.Insert(review);
                        _unitOfWork.SaveChanges();
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return RedirectToAction("Index", new { productId = reviewViewModel.ProductId });
        }
    }
}
