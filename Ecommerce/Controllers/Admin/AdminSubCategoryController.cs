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
    [RoutePrefix("SubCategory")]
    public class AdminSubCategoryController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private SubCategoryBusiness _subcategoryBusiness;
        private CategoryBusiness _categoryBusiness;
        public AdminSubCategoryController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._subcategoryBusiness = new SubCategoryBusiness(_df, this._unitOfWork);
            this._categoryBusiness = new CategoryBusiness(_df, this._unitOfWork);
        }
        //
        // GET: /Category/
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCategory(string sidx, string sord, int page, int rows, string colName, string colValue)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var categoryList = _categoryBusiness.GetListWT();
            var subcategoryList = _subcategoryBusiness.GetListWT();
            var records = (from p in subcategoryList
                           join c in categoryList on p.CategoryId equals c.CategoryId
                           select new SubCategoryViewModel
                           {
                               TokenKey = p.TokenKey,
                               SubCategoryName = p.SubCategoryName,
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
            SubCategoryViewModel subcategoryViewModel = new SubCategoryViewModel();
            subcategoryViewModel.CategoryList = _categoryBusiness.GetListWT().Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            return View(subcategoryViewModel);
        }

        //
        // POST: /Brand/Create
        [HttpPost]
        [Route("Create")]
        public ActionResult Create(SubCategoryViewModel subcategoryViewModel)
        {
            subcategoryViewModel.CategoryList = _categoryBusiness.GetListWT().Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<SubCategoryViewModel, SubCategory>();
                SubCategory category = Mapper.Map<SubCategoryViewModel, SubCategory>(subcategoryViewModel);

                var result = _subcategoryBusiness.ValidateCategory(category, "I");

                if (!string.IsNullOrEmpty(result))
                {
                    ModelState.AddModelError("", result);
                    return View(subcategoryViewModel);
                }
                else
                {
                    category.TokenKey = GlobalMethods.GetToken();
                    bool isSuccess = _subcategoryBusiness.AddUpdateDeleteCategory(category, "I");
                    if (isSuccess)
                    {
                        TempData["Success"] = "SubCategory Created Successfully!!";
                        TempData["isSuccess"] = "true";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Success"] = "Failed to create subcategory!!";
                        TempData["isSuccess"] = "false";
                    }
                }
            }
            return View(subcategoryViewModel);

        }
        [Route("Edit/{tkn}")]
        public ActionResult Edit(string tkn)
        {
            var category = _subcategoryBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<SubCategory, SubCategoryViewModel>();
            SubCategoryViewModel subcategoryViewModel = Mapper.Map<SubCategory, SubCategoryViewModel>(category); 
            subcategoryViewModel.CategoryList = _categoryBusiness.GetListWT().Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            return View(subcategoryViewModel);
            //  return View();
        }

        //
        // POST: /Brand/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit(SubCategoryViewModel categoryViewModel)
        {
            categoryViewModel.CategoryList = _categoryBusiness.GetListWT().Select(x => new SelectListItem
            {
                Text = x.CategoryName.ToString(),
                Value = x.CategoryId.ToString()
            }).ToList();
            if (ModelState.IsValid)
            {
                var category = _subcategoryBusiness.GetListWT(c => c.TokenKey == categoryViewModel.TokenKey).FirstOrDefault();
                category.SubCategoryName = categoryViewModel.SubCategoryName;

                var result = _subcategoryBusiness.ValidateCategory(category, "U");
                if (!string.IsNullOrEmpty(result))
                {
                    ModelState.AddModelError("", result);
                    return View(categoryViewModel);
                }
                else
                {
                    bool isSuccess = _subcategoryBusiness.AddUpdateDeleteCategory(category, "U");
                    if (isSuccess)
                    {
                        TempData["Success"] = "SubCategory Updated Successfully!!";
                        TempData["isSuccess"] = "true";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Success"] = "Failed to update SubCategory!!";
                        TempData["isSuccess"] = "false";
                    }
                }
            }
            return View(categoryViewModel);
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
                    List<SubCategory> categoryList = _subcategoryBusiness.GetListWT(x => x.TokenKey == tokenkey).ToList();
                    foreach (SubCategory cat in categoryList)
                    {
                        int deleteId = Convert.ToInt32(cat.SubCategoryId);
                        var br = _subcategoryBusiness.Find(deleteId);
                        _subcategoryBusiness.Delete(br);
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
