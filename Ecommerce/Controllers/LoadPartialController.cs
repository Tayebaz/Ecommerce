using AutoMapper;
using Business;
using Common.GlobalData;
using Entities.Models;
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
    public class LoadPartialController : Controller
    {
        private const int DefaultPageSize = 30;
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private CategoryBusiness _categoryBusiness;
        private SubCategoryBusiness _subCategoryBusiness;
        private BrandBusiness _brandBusiness;
        private ProductBusiness _productBusiness;
        private SliderBusiness _sliderBusiness;
        private ImageBusiness _ImageBusiness;

        public LoadPartialController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._categoryBusiness = new CategoryBusiness(_df, this._unitOfWork);
            this._subCategoryBusiness = new SubCategoryBusiness(_df, this._unitOfWork);
            this._brandBusiness = new BrandBusiness(_df, this._unitOfWork);
            this._productBusiness = new ProductBusiness(_df, this._unitOfWork);
            this._sliderBusiness = new SliderBusiness(_df, this._unitOfWork);
            this._ImageBusiness = new ImageBusiness(_df, this._unitOfWork);
        }
        public ActionResult LoadLeftSideBarHome()
        {
            var categoryList = _categoryBusiness.GetListWT();

            var subCategoryList = _subCategoryBusiness.GetListWT();
            Mapper.CreateMap<SubCategory, SubCategoryViewModel>();
            var vmSubCategoryList = Mapper.Map<List<SubCategory>, List<SubCategoryViewModel>>(subCategoryList);

            var vmCategoryList = (from c in categoryList
                                  select new CategoryViewModel
                                  {
                                      CategoryId = c.CategoryId,
                                      TokenKey = c.TokenKey,
                                      CategoryName = c.CategoryName,
                                      subCategoryList = vmSubCategoryList.Where(sc => sc.CategoryId == c.CategoryId).ToList()
                                  }).ToList();

            LeftSideBarViewModel leftSideBarViewModel = new LeftSideBarViewModel();
            leftSideBarViewModel.categoryList = vmCategoryList;
            return PartialView("_LoadLeftSideBarHome", leftSideBarViewModel);
        }

        public ActionResult LoadLeftSideBar()
        {
            var categoryid = Convert.ToInt32(string.IsNullOrEmpty(Request.QueryString["categoryid"]) ? "0" : Request.QueryString["categoryid"].ToString());
            var subcategoryid = Convert.ToInt32(string.IsNullOrEmpty(Request.QueryString["subcategoryid"]) ? "0" : Request.QueryString["subcategoryid"].ToString());
            var categoryList = _categoryBusiness.GetListWT();
            
            if (categoryid == 0)
            {
                var subCategory = _subCategoryBusiness.GetListWT(c => c.SubCategoryId == subcategoryid).FirstOrDefault();
                if (subCategory != null)
                    categoryid = subCategory.CategoryId;
            }
            var category = _categoryBusiness.GetListWT(c => c.CategoryId == categoryid).FirstOrDefault();
            var subCategoryList = _subCategoryBusiness.GetListWT(c => c.CategoryId == categoryid);
            var vmCategoryList = (from c in categoryList
                                  select new CategoryViewModel
                                  {
                                      CategoryId = c.CategoryId,
                                      TokenKey = c.TokenKey,
                                      CategoryName = c.CategoryName,
                                     
                                  }).ToList();
            Mapper.CreateMap<Category, CategoryViewModel>();
            var vmCategory = Mapper.Map<Category, CategoryViewModel>(category);
          

            Mapper.CreateMap<SubCategory, SubCategoryViewModel>();
            var vmSubCategoryList = Mapper.Map<List<SubCategory>, List<SubCategoryViewModel>>(subCategoryList);

            var brandList = _brandBusiness.GetListWT(c => c.CategoryId == categoryid);
            var productList = _productBusiness.GetListWT();
            var vmbrandList = (from c in brandList
                               select new BrandViewModel
                               {
                                   BrandId = c.BrandId,
                                   TokenKey = c.TokenKey,
                                   BrandName = c.BrandName,
                                   ProductCount = productList.Where(p => p.BrandId == c.BrandId).Count()
                               }).ToList();

            LeftSideBarViewModel leftSideBarViewModel = new LeftSideBarViewModel();
            leftSideBarViewModel.category = vmCategory;
            leftSideBarViewModel.categoryList = vmCategoryList;
            leftSideBarViewModel.subCategoryList = vmSubCategoryList;
            leftSideBarViewModel.BrandList = vmbrandList;
            leftSideBarViewModel.ColorList = _productBusiness.Getcolor().Select(x => new SelectListItem
            {
                Text = x.Text.ToString(),
                Value = x.Value.ToString()
            }).ToList();

            return PartialView("_LoadLeftSideBar", leftSideBarViewModel);
        }

        public ActionResult LoadLeftSideBarSearch()
        {
            var categoryList = _categoryBusiness.GetListWT();

            var subCategoryList = _subCategoryBusiness.GetListWT();
            Mapper.CreateMap<SubCategory, SubCategoryViewModel>();
            var vmSubCategoryList = Mapper.Map<List<SubCategory>, List<SubCategoryViewModel>>(subCategoryList);

            var vmCategoryList = (from c in categoryList
                                  select new CategoryViewModel
                                  {
                                      CategoryId = c.CategoryId,
                                      TokenKey = c.TokenKey,
                                      CategoryName = c.CategoryName,
                                      subCategoryList = vmSubCategoryList.Where(sc => sc.CategoryId == c.CategoryId).ToList()
                                  }).ToList();

            LeftSideBarViewModel leftSideBarViewModel = new LeftSideBarViewModel();
            leftSideBarViewModel.categoryList = vmCategoryList;
            leftSideBarViewModel.ColorList = _productBusiness.Getcolor().Select(x => new SelectListItem
            {
                Text = x.Text.ToString(),
                Value = x.Value.ToString()
            }).ToList();

            return PartialView("_LoadLeftSideBarSearch", leftSideBarViewModel);
        }
        public ActionResult LoadSlider()
        {
            var sliderList = _sliderBusiness.GetListWT().OrderBy(c => c.ImageOrder);
            var vmSliderList = (from c in sliderList
                                select new SliderViewModel
                                {
                                    SliderId = c.SliderId,
                                    TokenKey = c.TokenKey,
                                    SliderImage =  "/SliderImage/" + c.SliderImage,
                                    Title = c.Title,
                                    Description = c.Description,
                                    ImageOrder = c.ImageOrder
                                }).ToList();
            return PartialView("_LoadSlider", vmSliderList);
        }

        public ActionResult LoadFeaturesTtems(int? page)
        {

            if (Request.QueryString["page"] != null)
            {
                page = Convert.ToInt32(Request.QueryString["page"]);
            }
            var productList = _productBusiness.GetListWT();
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
                                    
                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images =  "/ProductImage/" + il.Images
                                                  }).ToList()

                                 }).ToList();
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return PartialView("_FeaturesItems", vmProductList.ToPagedList(currentPageIndex, DefaultPageSize));
            //return PartialView("_FeaturesItems", vmProductList);
        }

        public ActionResult LoadRecommendedTtems()
        {
            var productList = _productBusiness.GetListWT();
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
                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images =  "/ProductImage/" + il.Images
                                                  }).ToList()

                                 }).ToList().Take(6);
            return PartialView("_RecommendedItems", vmProductList);
        }

        public ActionResult LoadCategory()
        {
            var subcategories = _subCategoryBusiness.GetListWT();

            var productList = _productBusiness.GetListWT();
            var imgList = _ImageBusiness.GetListWT();
            var vmProductList = new List<ProductViewModel>();
            foreach (var category in subcategories)
            {
                var products = (from c in productList.Where(cat => cat.SubCategoryId == category.SubCategoryId)
                                select new ProductViewModel
                                {
                                    ProductID = c.ProductID,
                                    ProductName = c.ProductName,
                                    TokenKey = c.TokenKey,
                                    ShortDescription = c.ShortDescription,
                                    Price = _productBusiness.GetDefaultPrice(c.ProductID),
                                    DiscountPercent = c.DiscountPercent,
                                    DiscountedPrice = Math.Round(_productBusiness.GetDefaultPrice(c.ProductID) - Decimal.Divide(c.DiscountPercent ?? 0, 100) * _productBusiness.GetDefaultPrice(c.ProductID)),
                                   
                                    SubCategoryId = category.SubCategoryId,
                                    SubCategoryName = category.SubCategoryName,
                                    ImageList = (from il in imgList
                                                 where (il.ProductId == c.ProductID)
                                                 select new ImageViewModel
                                                 {
                                                     ProductId = c.ProductID,
                                                     Images =  "/ProductImage/" + il.Images
                                                 }).ToList()

                                }).ToList().Take(4);
                foreach (var p in products)
                {
                    vmProductList.Add(p);
                }
            }
            return PartialView("_Category", vmProductList);
        }

        public ActionResult LoadHomeContents()
        {
            var categories = _categoryBusiness.GetListWT();
            var subcategories = _subCategoryBusiness.GetListWT();
            var productList = _productBusiness.GetListWT();
            var imgList = _ImageBusiness.GetListWT();

            var vmcategory = (from c in categories
                              select new CategoryViewModel
                              {
                                  CategoryId = c.CategoryId,
                                  CategoryName = c.CategoryName,
                                  subCategoryList = (from sc in subcategories
                                                     where sc.CategoryId == c.CategoryId
                                                     select new SubCategoryViewModel
                                                     {
                                                         SubCategoryId = sc.SubCategoryId,
                                                         SubCategoryName = sc.SubCategoryName,
                                                         productList = (from p in productList
                                                                        where p.SubCategoryId == sc.SubCategoryId
                                                                        select new ProductViewModel
                                                                        {
                                                                            ProductID = p.ProductID,
                                                                            ProductName = p.ProductName,
                                                                            TokenKey = p.TokenKey,
                                                                            ShortDescription = p.ShortDescription,
                                                                            Price = _productBusiness.GetDefaultPrice(p.ProductID),
                                                                            DiscountPercent = p.DiscountPercent,
                                                                            DiscountedPrice = Math.Round(_productBusiness.GetDefaultPrice(p.ProductID) - Decimal.Divide(p.DiscountPercent ?? 0, 100) * _productBusiness.GetDefaultPrice(p.ProductID)),
                                                                           
                                                                            ImageList = (from il in imgList
                                                                                         where (il.ProductId == p.ProductID)
                                                                                         select new ImageViewModel
                                                                                         {
                                                                                             ProductId = p.ProductID,
                                                                                             Images = "/ProductImage/" + il.Images
                                                                                         }).ToList()

                                                                        }).Take(10).ToList()
                                                     }).ToList(),
                                  productList = (from p in productList
                                                 where p.CategoryId == c.CategoryId
                                                 select new ProductViewModel
                                                 {
                                                     ProductID = p.ProductID,
                                                     ProductName = p.ProductName,
                                                     TokenKey = p.TokenKey,
                                                     ShortDescription = p.ShortDescription,
                                                     Price = _productBusiness.GetDefaultPrice(p.ProductID),
                                                     DiscountPercent = p.DiscountPercent,
                                                     DiscountedPrice = Math.Round(_productBusiness.GetDefaultPrice(p.ProductID) - Decimal.Divide(p.DiscountPercent ?? 0, 100) * _productBusiness.GetDefaultPrice(p.ProductID)),
                                                     
                                                     ImageList = (from il in imgList
                                                                  where (il.ProductId == p.ProductID)
                                                                  select new ImageViewModel
                                                                  {
                                                                      ProductId = p.ProductID,
                                                                      Images = "/ProductImage/" + il.Images
                                                                  }).ToList()

                                                 }).Take(10).ToList()
                              }).ToList();
            return PartialView("_HomeContents", vmcategory);
        }

        public ActionResult LoadLeftSideBarAccount()
        {
            return PartialView("_LoadLeftSideBarAccount");
        }
    }
}
