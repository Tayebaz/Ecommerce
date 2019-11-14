using AutoMapper;
using Common.Cryptography;
using Entities.Models;
using Filters.ActionFilters;
using Ecommerce.Models;
using Repository.RepositoryFactoryCore;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Filters.AuthenticationModel;
using System.Threading.Tasks;
using Ecommerce.Helper;
using Commom.GlobalMethods;
using System.IO;

namespace Ecommerce.Controllers
{
    [EcommerceAuthorize("Admin")]
    [RouteArea("Admin")]
    [RoutePrefix("Slider")]
    public class AdminSliderSettingController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private SliderBusiness _sliderBusiness;

        public AdminSliderSettingController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._sliderBusiness = new SliderBusiness(_df, this._unitOfWork);

        }
       
        [Route("")]
        public ActionResult Index()
        {
            var CartList = _sliderBusiness.GetAllCategory().ToList();
            List<SliderViewModel> brandViewModelList = new List<SliderViewModel>();
            Mapper.CreateMap<SliderSetting, SliderViewModel>()
                 .ForMember(dest => dest.TokenKey, opt => opt.MapFrom(src => (src.TokenKey.ToString())));
            brandViewModelList = Mapper.Map<List<SliderSetting>, List<SliderViewModel>>(CartList);
            return View(CartList);    
        }

        public JsonResult GetSlider(string sidx, string sord, int page, int rows, string colName, string colValue)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var sliderList = _sliderBusiness.GetListWT();
            var records = (from p in sliderList
                           select new SliderViewModel
                           {
                               TokenKey = p.TokenKey,
                               SliderImage = p.SliderImage,
                               ImageOrder = p.ImageOrder,
                               Title = p.Title,
                               Description = p.Description
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
            SliderViewModel sliderViewModel = new SliderViewModel();

            return View(sliderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create(SliderViewModel sliderViewModel)
        {            
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<SliderViewModel, SliderSetting>();
                SliderSetting slider = Mapper.Map<SliderViewModel, SliderSetting>(sliderViewModel);
              
             
                slider.TokenKey = GlobalMethods.GetToken();
                FileOperations.CreateDirectory(Server.MapPath("~/SliderImage"));
                if (sliderViewModel.SliderImageUpload != null)
                {
                    string ext = Path.GetExtension(sliderViewModel.SliderImageUpload.FileName).ToLower();
                    string filename = slider.TokenKey + ext;

                    string filePath = Server.MapPath("~/SliderImage/") + filename;
                    sliderViewModel.SliderImageUpload.SaveAs(filePath);
                    slider.SliderImage = filename;
                }

                bool isSuccess = _sliderBusiness.AddUpdateDeleteSlider(slider, "I");
                if (isSuccess)
                {
                    TempData["Success"] = "Slider Added Successfully!!";
                    TempData["isSuccess"] = "true";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Success"] = "Failed to add slider!!";
                    TempData["isSuccess"] = "false";
                }
            }
            else
            {
                TempData["Success"] = ModelState.Values.SelectMany(m => m.Errors).FirstOrDefault().ErrorMessage;
                TempData["isSuccess"] = "false";
            }

            return View(sliderViewModel);
        }

        public JsonResult Delete(string token)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Delete Successful!";

            try
            {
                var slider = _sliderBusiness.GetListWT(x => x.TokenKey == token).FirstOrDefault();
                var filename = slider.SliderImage;
                int deleteId = Convert.ToInt32(slider.SliderId);
                var br = _sliderBusiness.Find(deleteId);
                _sliderBusiness.Delete(br);
                _unitOfWork.SaveChanges();

                var sliderImagepath = Server.MapPath("~/SliderImage/" + filename);
                FileOperations.DeleteFileFromDirectory(sliderImagepath);
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