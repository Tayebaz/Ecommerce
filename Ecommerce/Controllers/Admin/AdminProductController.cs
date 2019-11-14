using AutoMapper;
using Business;
using Commom.GlobalMethods;
using Common.Cryptography;
using Common.GlobalData;
using Entities.Models;
using Newtonsoft.Json;
using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Helper;
using Ecommerce.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Filters.ActionFilters;

namespace Ecommerce.Controllers
{
    [EcommerceAuthorize("Admin")]
    [RouteArea("Admin")]
    [RoutePrefix("Product")]
    public class AdminProductController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private ProductBusiness _productBusiness;
        private ProductSizeBusiness _productSizeBusiness;
        private ProductAttributeBusiness _productAttributeBusiness;
        private BrandBusiness _brandBusiness;
        private CategoryBusiness _categoryBusiness;
        private SubCategoryBusiness _subCategoryBusiness;
        private ImageBusiness _imageBusiness;
        private SliderBusiness _sliderBusiness;
        //
        // GET: /Product/

        public AdminProductController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._productBusiness = new ProductBusiness(_df, this._unitOfWork);
            this._productSizeBusiness = new ProductSizeBusiness(_df, this._unitOfWork);
            this._productAttributeBusiness = new ProductAttributeBusiness(_df, this._unitOfWork);
            this._brandBusiness = new BrandBusiness(_df, this._unitOfWork);
            this._categoryBusiness = new CategoryBusiness(_df, this._unitOfWork);
            this._imageBusiness = new ImageBusiness(_df, this._unitOfWork);
            this._sliderBusiness = new SliderBusiness(_df, this._unitOfWork);
            this._subCategoryBusiness = new SubCategoryBusiness(_df, this._unitOfWork);
        }
        //
        // GET: /Category/
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProducts(string sidx, string sord, int page, int rows, string colName, string colValue)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var brandList = _productBusiness.GetListWT();

            List<ProductViewModel> brandViewModelList = new List<ProductViewModel>();

            var records = (from p in brandList
                           select new ProductViewModel
                           {
                               TokenKey = p.TokenKey,
                               Brand = _brandBusiness.GetBrandById(Convert.ToInt32(p.BrandId)).BrandName,
                               Category = _categoryBusiness.GetCategoryById(Convert.ToInt32(p.CategoryId)).CategoryName,
                               ProductName = p.ProductName,
                               ProductCode = p.ProductCode
                           }).AsQueryable();

            //applying filter
            if (!string.IsNullOrEmpty(colName) && !string.IsNullOrEmpty(colValue))
            {
                records = records.Where(c => c.GetType().GetProperty(colName).GetValue(c, null).ToString().ToLower().Contains(colValue.ToLower()));
            }

            int totalRecords = records.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (!string.IsNullOrEmpty(sidx) && !string.IsNullOrEmpty(sord))
            {
                if (sord.Trim().ToLower() == "asc")
                {
                    records = SortHelper.OrderBy(records, sidx);
                }
                else
                {
                    records = SortHelper.OrderByDescending(records, sidx);
                }
            }

            //applying paging
            records = records.Skip(pageIndex * pageSize).Take(pageSize);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = records
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [Route("Create")]
        public ActionResult Create()
        {

            ProductViewModel productViewModel = new ProductViewModel();
            var categoryList = _categoryBusiness.GetListWT();
            var subcategoryList = _subCategoryBusiness.GetListWT();

            productViewModel.CategoryList = categoryList.Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            productViewModel.AvailabilityList = Commom.GlobalMethods.GlobalMethods.GetAvailabilityList();
            return View(productViewModel);
        }

        //
        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create(ProductViewModel productViewModel, HttpPostedFileBase[] files)
        {
            var categoryList = _categoryBusiness.GetListWT();
            var subcategoryList = _subCategoryBusiness.GetListWT();
            productViewModel.CategoryList = categoryList.Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            productViewModel.AvailabilityList = Commom.GlobalMethods.GlobalMethods.GetAvailabilityList(); 
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<ProductViewModel, Product>();
                Product product = Mapper.Map<ProductViewModel, Product>(productViewModel);

                product.TokenKey = GlobalMethods.GetToken();
                FileOperations.CreateDirectory(Server.MapPath("~/ProductImage"));
                FileOperations.CreateDirectory(Server.MapPath("~/ProductImage/Thumbnails/"));

                bool isSuccess = _productBusiness.AddUpdateDeleteProduct(product, "I");
                foreach (HttpPostedFileBase file in files)
                {
                    var image = new Entities.Models.Image();

                    string extension = System.IO.Path.GetExtension(file.FileName);
                    string filename = Guid.NewGuid().ToString() + extension;

                    image.Images = filename;
                    image.ProductId = product.ProductID;
                    _imageBusiness.AddUpdateDeleteimage(image, "I");

                    var filePathThumbnail = Server.MapPath("~/ProductImage/Thumbnails");
                    file.SaveAs(Server.MapPath("~/ProductImage/" + filename));

                    var thumbnail = FileOperations.CreateThumbnail(Server.MapPath("~/ProductImage/" + filename), 72, 72);
                    FileOperations.SaveBitmapImageToDirectory(thumbnail, Server.MapPath("~/ProductImage/Thumbnails/" + filename));
                }


                if (isSuccess)
                {
                    TempData["Success"] = "Product Created Successfully!!";
                    TempData["isSuccess"] = "true";
                    return RedirectToAction("Index");

                }
                else
                {
                    TempData["Success"] = "Failed to create product!!";
                    TempData["isSuccess"] = "false";
                }

            }
            return View(productViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("uploadimage")]
        public ActionResult uploadimage(ProductViewModel productViewModel, HttpPostedFileBase[] files)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Image Added Successfully!!";
            try
            {
                var product = _productBusiness.GetListWT(c => c.TokenKey == productViewModel.TokenKey).FirstOrDefault();
                FileOperations.CreateDirectory(Server.MapPath("~/ProductImage"));
                FileOperations.CreateDirectory(Server.MapPath("~/ProductImage/Thumbnails/"));
                foreach (HttpPostedFileBase file in files)
                {
                    var image = new Entities.Models.Image();

                    string extension = System.IO.Path.GetExtension(file.FileName);
                    string filename = Guid.NewGuid().ToString() + extension;

                    image.Images = filename;
                    image.ProductId = product.ProductID;
                    _imageBusiness.AddUpdateDeleteimage(image, "I");

                    var filePathThumbnail = Server.MapPath("~/ProductImage/Thumbnails");
                    file.SaveAs(Server.MapPath("~/ProductImage/" + filename));

                    var thumbnail = FileOperations.CreateThumbnail(Server.MapPath("~/ProductImage/" + filename), 72, 72);
                    FileOperations.SaveBitmapImageToDirectory(thumbnail, Server.MapPath("~/ProductImage/Thumbnails/" + filename));
                }
                TempData["Success"] = "Image Uploaded Successfully!!";
                TempData["isSuccess"] = "true";
            }
            catch (Exception)
            {
                TempData["Success"] = "Failed to upload Image!!";
                TempData["isSuccess"] = "false";
                message = "Failed to upload Image!!";
                isSuccess = false;
            }

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [Route("Detail/{tkn}")]
        public ActionResult Detail(string tkn)
        {
            if (string.IsNullOrEmpty(tkn))
            {
                return RedirectToAction("Index");
            }

            var product = _productBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<Product, ProductViewModel>();
            var vmprod = Mapper.Map<Product, ProductViewModel>(product);
            return View(vmprod);
        }

        [Route("Overview/{tkn}")]
        public ActionResult Overview(string tkn)
        {

            var prodList = _productBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            int pid = prodList.ProductID;

            var imagelist = _imageBusiness.GetAllImagesbyProduct(pid);
            var brandList = _brandBusiness.GetListWT().Where(c => c.BrandId == prodList.BrandId);
            var categoryList = _categoryBusiness.GetListWT().Where(c => c.CategoryId == prodList.CategoryId);

            Mapper.CreateMap<Product, ProductViewModel>();
            var vmUser = Mapper.Map<Product, ProductViewModel>(prodList);
            vmUser.Brand = brandList.Select(c => c.BrandName).FirstOrDefault();
            vmUser.Category = categoryList.Select(c => c.CategoryName).FirstOrDefault();
            vmUser.Image = imagelist.Select(c => c.Images).FirstOrDefault();
            

            return PartialView("_Overview", vmUser);
        }


        [Route("Size/{tkn}")]
        public ActionResult Size(string tkn)
        {
            var product = _productBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            int productId = product.ProductID;
            var sizeList = _productSizeBusiness.GetListWT(c => c.ProductId == productId);

            Mapper.CreateMap<Product, ProductViewModel>();
            var vmProduct = Mapper.Map<Product, ProductViewModel>(product);
            vmProduct.ProductSizeList = (from il in sizeList
                                   where (il.ProductId == product.ProductID)
                                   select new ProductSizeViewModel
                                   {
                                       Id = il.Id,
                                       ProductId = product.ProductID,
                                       Size = il.Size,
                                       Price = il.Price
                                   }).ToList();
            return PartialView("_Size", vmProduct);
        }

        [Route("Attribute/{tkn}")]
        public ActionResult Attribute(string tkn)
        {
            var product = _productBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            int productId = product.ProductID;
            var attributeList = _productAttributeBusiness.GetListWT(c => c.ProductId == productId);

            Mapper.CreateMap<Product, ProductViewModel>();
            var vmProduct = Mapper.Map<Product, ProductViewModel>(product);
            vmProduct.ProductAttributeList = (from il in attributeList
                                   where (il.ProductId == product.ProductID)
                                   select new ProductAttributeViewModel
                                   {
                                       Id = il.Id,
                                       ProductId = product.ProductID,
                                       Attributes = il.Attributes,
                                       Price = il.Price
                                   }).ToList();
            return PartialView("_Attribute", vmProduct);
        }

        [Route("DisplayImage/{tkn}")]
        public ActionResult DisplayImage(string tkn)
        {
            var product = _productBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            int productId = product.ProductID;
            var imgList = _imageBusiness.GetListWT(c => c.ProductId == productId);

            Mapper.CreateMap<Product, ProductViewModel>();
            var vmProduct = Mapper.Map<Product, ProductViewModel>(product);
            vmProduct.ImageList = (from il in imgList
                                   where (il.ProductId == product.ProductID)
                                   select new ImageViewModel
                                   {
                                       ImageId = il.ImageId,
                                       ProductId = product.ProductID,
                                       Images = "/ProductImage/Thumbnails/" + il.Images
                                   }).ToList();
            return PartialView("_DisplayImage", vmProduct);
        }
        // POST: /Event/Edit/5

        [Route("Edit/{tkn}")]
        public ActionResult Edit(string tkn)
        {
            var user = _productBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<Product, ProductViewModel>();
            ProductViewModel prodViewModel = Mapper.Map<Product, ProductViewModel>(user);
            var categoryList = _categoryBusiness.GetListWT();
            prodViewModel.CategoryList = categoryList.Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            prodViewModel.AvailabilityList = Commom.GlobalMethods.GlobalMethods.GetAvailabilityList();
            return PartialView("_Edit", prodViewModel);
        }

        //
        // POST: /Event/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit(ProductViewModel prodViewModel)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Product Updated Successfully!!";
            Entities.Models.Image images = new Entities.Models.Image();
            if (ModelState.IsValid)
            {
                try
                {
                    var product = _productBusiness.GetListWT(c => c.TokenKey == prodViewModel.TokenKey).FirstOrDefault();
                    product.ProductName = prodViewModel.ProductName;
                    product.ProductCode = prodViewModel.ProductCode;
                    product.BrandId = prodViewModel.BrandId;
                    product.CategoryId = prodViewModel.CategoryId;
                    product.SubCategoryId = prodViewModel.SubCategoryId;
                    product.DiscountPercent = prodViewModel.DiscountPercent;
                    product.ShortDescription = prodViewModel.ShortDescription;
                    product.Description = prodViewModel.Description;
                    product.Availability = prodViewModel.Availability;
                    _productBusiness.Update(product);
                    _unitOfWork.SaveChanges();
                }
                catch
                {
                    message = "Failed to update product!!";
                    isSuccess = false;
                    _unitOfWork.Dispose();
                }
            }
            else
            {
                message = ModelState.Values.SelectMany(m => m.Errors).FirstOrDefault().ErrorMessage;
                isSuccess = false;
            }
            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult Delete(string stkns)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Delete Successful!";
            _unitOfWork.BeginTransaction();
            try
            {
                foreach (string tokenkey in stkns.Substring(1).Split('~'))
                {
                    List<Product> prodList = _productBusiness.GetListWT(x => x.TokenKey == tokenkey).ToList();
                    foreach (Product prod in prodList)
                    {
                        List<string> productimages = new List<string>();
                        productimages = _imageBusiness.GetListWT(x => x.ProductId == prod.ProductID).Select(c => c.Images).ToList();

                        int deleteId = Convert.ToInt32(prod.ProductID);
                        var br = _productBusiness.Find(deleteId);
                        _productBusiness.Delete(br);
                        _unitOfWork.SaveChanges();

                        foreach (var image in productimages)
                        {
                            var productImagePath = Server.MapPath("~/ProductImage/" + image);
                            var productImageThumbnailPath = Server.MapPath("~/ProductImage/Thumbnails/" + image);
                            FileOperations.DeleteFileFromDirectory(productImagePath);
                            FileOperations.DeleteFileFromDirectory(productImageThumbnailPath);
                        }
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                message = "Delete Unsuccessful!";
                throw ex;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [Route("DeleteProductImage")]
        public JsonResult DeleteProductImage(string token)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Delete Successful!";

            try
            {
                var id = Convert.ToInt32(token);
                var image = _imageBusiness.GetListWT(x => x.ImageId == id).FirstOrDefault();
                var filename = image.Images;
                var br = _imageBusiness.Find(id);
                _imageBusiness.Delete(br);
                _unitOfWork.SaveChanges();

                var productImagePath = Server.MapPath("~/ProductImage/" + image.Images);
                var productImageThumbnailPath = Server.MapPath("~/ProductImage/Thumbnails/" + image.Images);
                FileOperations.DeleteFileFromDirectory(productImagePath);
                FileOperations.DeleteFileFromDirectory(productImageThumbnailPath);


            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                message = "Delete Unsuccessful!";
                throw ex;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [Route("GetSubCategory/{category}")]
        public ActionResult GetSubCategory(int category)
        {
            return Json(
                _subCategoryBusiness.GetListWT(c => c.CategoryId == category).Select(x => new { value = x.SubCategoryId, text = x.SubCategoryName }),
                JsonRequestBehavior.AllowGet
            );
        }

        [Route("GetBrand/{category}")]
        public ActionResult GetBrand(int category)
        {

            return Json(
                _brandBusiness.GetListWT(c => c.CategoryId == category).Select(x => new { value = x.BrandId, text = x.BrandName }),
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SaveSize")]
        public ActionResult SaveSize(ProductViewModel productViewModel)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Size Added Successfully!!";
            try
            {
                var product = _productBusiness.GetListWT(c => c.TokenKey == productViewModel.TokenKey).FirstOrDefault();
                var size = new Entities.Models.ProductSize();
                size.Size = productViewModel.Size;
                size.Price = productViewModel.Price;
                size.ProductId = product.ProductID;
                _productSizeBusiness.AddUpdateDeleteProductSize(size, "I");
                TempData["Success"] = message;
                TempData["isSuccess"] = "true";
            }
            catch (Exception)
            {
                message = "Failed to add size!!";
                isSuccess = false;

                TempData["Success"] = message;
                TempData["isSuccess"] = "false";
                
            }

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SaveAttribute")]
        public ActionResult SaveAttribute(ProductViewModel productViewModel)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Attribute Added Successfully!!";
            try
            {
                var product = _productBusiness.GetListWT(c => c.TokenKey == productViewModel.TokenKey).FirstOrDefault();
                var attribute = new Entities.Models.ProductAttribute();
                attribute.Attributes = productViewModel.Attributes;
                attribute.Price = productViewModel.Price;
                attribute.ProductId = product.ProductID;
                _productAttributeBusiness.AddUpdateDeleteProductAttribute(attribute, "I");
                TempData["Success"] = message;
                TempData["isSuccess"] = "true";
            }
            catch (Exception)
            {
                message = "Failed to add Attribute!!";
                isSuccess = false;

                TempData["Success"] = message;
                TempData["isSuccess"] = "false";

            }

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [Route("DeleteProductSize")]
        public JsonResult DeleteProductSize(string token)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Delete Successful!";

            try
            {
                var id = Convert.ToInt32(token);               
                var br = _productSizeBusiness.Find(id);
                _productSizeBusiness.Delete(br);
                _unitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                message = "Delete Unsuccessful!";
                throw ex;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [Route("DeleteProductAttribute")]
        public JsonResult DeleteProductAttribute(string token)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Delete Successful!";

            try
            {
                var id = Convert.ToInt32(token);
                var br = _productAttributeBusiness.Find(id);
                _productAttributeBusiness.Delete(br);
                _unitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                message = "Delete Unsuccessful!";
                throw ex;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }
    }
}
