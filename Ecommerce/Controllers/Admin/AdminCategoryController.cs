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
namespace Ecommerce.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("Category")]
    public class AdminCategoryController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private CategoryBusiness _categoryBusiness;

        public AdminCategoryController()
        {
            this._unitOfWork = new UnitOfWork(_df);
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
            var records = (from p in categoryList
                           select new CategoryViewModel
                           {
                               TokenKey = p.TokenKey,
                               CategoryName = p.CategoryName
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
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            return View(categoryViewModel);
        }

        //
        // POST: /Brand/Create
        [HttpPost]
        [Route("Create")]
        public ActionResult Create(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<CategoryViewModel, Category>();
                Category category = Mapper.Map<CategoryViewModel, Category>(categoryViewModel);

                var result = _categoryBusiness.ValidateCategory(category, "I");

                if (!string.IsNullOrEmpty(result))
                {
                    ModelState.AddModelError("", result);
                    return View(categoryViewModel);
                }
                else
                {
                    category.TokenKey = GlobalMethods.GetToken();
                    bool isSuccess = _categoryBusiness.AddUpdateDeleteCategory(category, "I");
                    if (isSuccess)
                    {
                        TempData["Success"] = "Category Created Successfully!!";
                        TempData["isSuccess"] = "true";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Success"] = "Failed to create category!!";
                        TempData["isSuccess"] = "false";
                    }
                }
            }
            return View(categoryViewModel);

        }
        [Route("Edit/{tkn}")]
        public ActionResult Edit(string tkn)
        {
            var category = _categoryBusiness.GetListWT(c => c.TokenKey == tkn).FirstOrDefault();
            Mapper.CreateMap<Category, CategoryViewModel>();
            CategoryViewModel categoryViewModel = Mapper.Map<Category, CategoryViewModel>(category);
            return View(categoryViewModel);
            //  return View();
        }

        //
        // POST: /Brand/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public ActionResult Edit(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var category = _categoryBusiness.GetListWT(c => c.TokenKey == categoryViewModel.TokenKey).FirstOrDefault();
                category.CategoryName = categoryViewModel.CategoryName;

                var result = _categoryBusiness.ValidateCategory(category, "U");
                if (!string.IsNullOrEmpty(result))
                {
                    ModelState.AddModelError("", result);
                    return View(categoryViewModel);
                }
                else
                {
                    bool isSuccess = _categoryBusiness.AddUpdateDeleteCategory(category, "U");
                    if (isSuccess)
                    {
                        TempData["Success"] = "Category Updated Successfully!!";
                        TempData["isSuccess"] = "true";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Success"] = "Failed to update Category!!";
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
                    List<Category> categoryList = _categoryBusiness.GetListWT(x => x.TokenKey == tokenkey).ToList();
                    foreach (Category cat in categoryList)
                    {
                        int deleteId = Convert.ToInt32(cat.CategoryId);
                        var br = _categoryBusiness.Find(deleteId);
                        _categoryBusiness.Delete(br);
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
