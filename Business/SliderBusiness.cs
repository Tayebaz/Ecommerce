using Common.Cryptography;
using Common.GlobalData;
using Entities.Models;
using Filters.AuthenticationCore;
using Filters.AuthenticationModel;
using Repository.RepositoryFactoryCore;
using Repository.RepositoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace Business
{
    public class SliderBusiness
    {
        public SliderSettingRepository _sliderRepository;
        private UnitOfWork _unitOfWork;
        public SliderBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _sliderRepository = new SliderSettingRepository(dfactory);
        }

        public SliderSetting Insert(SliderSetting sliderSetting)
        {
            _sliderRepository.Insert(sliderSetting);
            return sliderSetting;
        }

        public SliderSetting Update(SliderSetting sliderSetting)
        {
            _sliderRepository.Update(sliderSetting);
            return sliderSetting;
        }

        public void Delete(long? id)
        {
            _sliderRepository.Delete(id);
        }

        public void Delete(SliderSetting sliderSetting)
        {
            _sliderRepository.Delete(sliderSetting);
        }

        public SliderSetting Find(long? id)
        {
            return _sliderRepository.Find(id);
        }
        public IEnumerable<SliderSetting> GetAllCategory()
        {
            return _sliderRepository.Query().Select();
        }
        //public IEnumerable<Brand> GetAllBrandByRegion()
        //{
        //    return _brandRepository.Query().Select().Where(u => u.RegionID == GlobalUser.getGlobalUser().RegionID).ToList();
        //}
        public SliderSetting GetCategoryById(int id)
        {
            return _sliderRepository.Query(u => u.SliderId == id).Select().FirstOrDefault();
        }
        public bool AddUpdateDeleteSlider(SliderSetting sliderSetting, string action)
        {
            bool isSuccess = true;
            try
            {

                //brand.App_User = ReadConfigData.GetDBLoginUser();
                //brand.Audit_User = GlobalUser.getGlobalUser().UserName;
                //brand.RegionID = Convert.ToInt32(GlobalUser.getGlobalUser().RegionID);
                //brand.DisplayOrder = 1;
                //brand.VersionDataID = vid;
                //brand.VersionAction = action;
                if (action == "I")
                {
                    Insert(sliderSetting);
                }
                else if (action == "U")
                {
                    Update(sliderSetting);
                }
                else if (action == "D")
                {
                    Delete(sliderSetting);
                }
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                throw ex;
            }
            return isSuccess;
        }
        public List<SliderSetting> GetListWT(Expression<Func<SliderSetting, bool>> exp = null, Func<IQueryable<SliderSetting>, IOrderedQueryable<SliderSetting>> orderby = null)
        {
            return _sliderRepository.GetListWithNoTracking(exp, orderby);
        }


        public string ValidateCategory(SliderSetting sliderSettingchk, string action)
        {
            string result = string.Empty;
            if (action == "I")
            {
                var sliderSettingList = _sliderRepository.Query(u => u.SliderImage == sliderSettingchk.SliderImage).Select();
                if (sliderSettingList.ToList().Count > 0)
                {
                    if (sliderSettingList.Where(u => u.SliderImage == sliderSettingchk.SliderImage).FirstOrDefault() != null)
                    {
                        result = "Slider Name already exists!";
                        return result;
                    }

                }
            }
            else if (action == "U")
            {
                var sliderSettingList = _sliderRepository.Query(u => u.SliderId != sliderSettingchk.SliderId && (u.SliderImage == sliderSettingchk.SliderImage)).Select();
                if (sliderSettingList.ToList().Count > 0)
                {

                    return result;

                }
            }
            return result;
        }


    }
}
