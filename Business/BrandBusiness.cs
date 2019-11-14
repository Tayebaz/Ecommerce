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
    public class BrandBusiness
    {
        public BrandRepository _brandRepository;       
        private UnitOfWork _unitOfWork;
        public BrandBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _brandRepository = new BrandRepository(dfactory);
        }

        public Brand Insert(Brand Brand)
        {
            _brandRepository.Insert(Brand);
            return Brand;
        }

        public Brand Update(Brand Brand)
        {
            _brandRepository.Update(Brand);
            return Brand;
        }

        public void Delete(long? id)
        {
            _brandRepository.Delete(id);
        }

        public void Delete(Brand Brand)
        {
            _brandRepository.Delete(Brand);
        }

        public Brand Find(long? id)
        {
            return _brandRepository.Find(id);
        }
        public IEnumerable<Brand> GetAllBrand()
        {
            return _brandRepository.Query().Select();
        }
        //public IEnumerable<Brand> GetAllBrandByRegion()
        //{
        //    return _brandRepository.Query().Select().Where(u => u.RegionID == GlobalUser.getGlobalUser().RegionID).ToList();
        //}
        public List<Brand> GetListWT(Expression<Func<Brand, bool>> exp = null, Func<IQueryable<Brand>, IOrderedQueryable<Brand>> orderby = null)
        {
            return _brandRepository.GetListWithNoTracking(exp, orderby);
        }

        public Brand GetBrandById(int id)
        {
            var brand = _brandRepository.Query(u => u.BrandId == id).Select().FirstOrDefault();
            if (brand == null)
            {
                brand = new Brand();
            }
            return brand;
        }
        public bool AddUpdateDeleteBrand(Brand brand, string action)
        {
            bool isSuccess = true;
            try
            {

                //brand.App_User = ReadConfigData.GetDBLoginUser();
                //brand.Audit_User = GlobalUser.getGlobalUser().UserName;
                //brand.RegionID = Convert.ToInt32(GlobalUser.getGlobalUser().RegionID);
                //brand.DisplayOrder = 1;
                //brand.VersionDataID = vid;
                //brand.VersionAction = action;nbkhikihikhihioiho
                if (action == "I")
                {
                    Insert(brand);
                }
                else if (action == "U")
                {
                    Update(brand);
                }
                else if (action == "D")
                {
                    Delete(brand);
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

        public string ValidateBrand(Brand brandchk, string action)
        {
            string result = string.Empty;
            if (action == "I")
            {
                var brandList = _brandRepository.Query(u => u.BrandName == brandchk.BrandName).Select();
                if (brandList.ToList().Count > 0)
                {
                    if (brandList.Where(u => u.BrandName == brandchk.BrandName).FirstOrDefault() != null)
                    {
                        result = "Brand Name already exists!";
                        return result;
                    }

                }
            }
            else if (action == "U")
            {
                var brandList = _brandRepository.Query(u => u.BrandId != brandchk.BrandId && (u.BrandName == brandchk.BrandName)).Select();
                if (brandList.ToList().Count > 0)
                {

                    return result;

                }
            }
            return result;
        }


    }
}
