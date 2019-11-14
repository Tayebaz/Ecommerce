using AutoMapper;
using Business;
using Commom.GlobalMethods;
using Common.Cryptography;
using Entities.Models;
using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Helper;
using Ecommerce.Models;
using Filters.ActionFilters;

namespace Ecommerce.Controllers
{
    [EcommerceAuthorize("Admin")]
    [RouteArea("Admin")]
    [RoutePrefix("Brand")]
    public class AdminBrandController : Controller
    {
        // GET: /Brand/
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private BrandBusiness _brandBusiness;
        private CategoryBusiness _categoryBusiness;
      
        public AdminBrandController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._brandBusiness = new BrandBusiness(_df, this._unitOfWork);
            this._categoryBusiness = new CategoryBusiness(_df, this._unitOfWork);
        }
        //
        //
        // GET: /Brand/
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
        
        public JsonResult GetBrands(string sidx, string sord, int page, int rows, string colName, string colValue)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            var brandList = _brandBusiness.GetListWT();
            var categoryList = _categoryBusiness.GetListWT();
            List<BrandViewModel> brandViewModelList = new List<BrandViewModel>();

            var records = (from p in brandList
                           join c in categoryList on p.CategoryId equals c.CategoryId
                           select new BrandViewModel
                           {
                               TokenKey = p.TokenKey,
                               BrandName = p.BrandName,                             
                               CategoryName = c.CategoryName                              
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
            BrandViewModel brandViewModel = new BrandViewModel();
            brandViewModel.CategoryList = _categoryBusiness.GetListWT().Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            return View(brandViewModel);
        }

        //
        // POST: /Brand/Create
        [HttpPost]
        [Route("Create")]
        public ActionResult Create(BrandViewModel brandViewModel)
        {
            brandViewModel.CategoryList = _categoryBusiness.GetListWT().Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<BrandViewModel, Brand>();
                Brand brand = Mapper.Map<BrandViewModel, Brand>(brandViewModel);
                var result = _brandBusiness.ValidateBrand(brand, "I");
                if (!string.IsNullOrEmpty(result))
                {
                    ModelState.AddModelError("", result);
                    return View(brandViewModel);
                }
                else
                {
                    brand.TokenKey = GlobalMethods.GetToken();
                    bool isSuccess = _brandBusiness.AddUpdateDeleteBrand(brand, "I");
                    if (isSuccess)
                    {
                        TempData["Success"] = "Brand Created Successfully!!";
                        TempData["isSuccess"] = "true";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Success"] = "Failed to create Brand!!";
                        TempData["isSuccess"] = "false";
                    }
                }
            }
            return View(brandViewModel);

        }

        [Route("Edit/{tkn}")]
        public ActionResult Edit(string tkn)
        {
            var brand = _brandBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<Brand, BrandViewModel>();
            BrandViewModel brandViewModel = Mapper.Map<Brand, BrandViewModel>(brand);
            brandViewModel.CategoryList = _categoryBusiness.GetListWT().Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            return View(brandViewModel);
        }

        //
        // POST: /Event/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit(BrandViewModel brandViewModel)
        {
            brandViewModel.CategoryList = _categoryBusiness.GetListWT().Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();

            if (ModelState.IsValid)
            {
                var brand = _brandBusiness.GetListWT(c => c.TokenKey == brandViewModel.TokenKey).FirstOrDefault();              
                brand.BrandName = brandViewModel.BrandName;
                brand.CategoryId = brandViewModel.CategoryId;

                var result = _brandBusiness.ValidateBrand(brand, "U");
                if (!string.IsNullOrEmpty(result))
                {
                    ModelState.AddModelError("", result);
                    return View(brandViewModel);
                }
                else
                {
                    bool isSuccess = _brandBusiness.AddUpdateDeleteBrand(brand, "U");
                    if (isSuccess)
                    {
                        TempData["Success"] = "Brand Updated Successfully!!";
                        TempData["isSuccess"] = "true";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Success"] = "Failed to update Brand!!";
                        TempData["isSuccess"] = "false";
                    }
                }
            }
            return View(brandViewModel);
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
                    List<Brand> brandList = _brandBusiness.GetListWT(x => x.TokenKey == tokenkey).ToList();
                    foreach (Brand brand in brandList)
                    {
                        int deleteId = Convert.ToInt32(brand.BrandId);
                        var br = _brandBusiness.Find(deleteId);
                        _brandBusiness.Delete(br);
                        _unitOfWork.SaveChanges();
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
    }
}
