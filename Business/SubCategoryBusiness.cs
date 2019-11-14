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
    public class SubCategoryBusiness
    {
        public SubCategoryRepository _subcategoryRepository;
        private UnitOfWork _unitOfWork;
        public SubCategoryBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _subcategoryRepository = new SubCategoryRepository(dfactory);
        }

        public SubCategory Insert(SubCategory subCategory)
        {
            _subcategoryRepository.Insert(subCategory);
            return subCategory;
        }

        public SubCategory Update(SubCategory subCategory)
        {
            _subcategoryRepository.Update(subCategory);
            return subCategory;
        }

        public void Delete(long? id)
        {
            _subcategoryRepository.Delete(id);
        }

        public void Delete(SubCategory subCategory)
        {
            _subcategoryRepository.Delete(subCategory);
        }

        public SubCategory Find(long? id)
        {
            return _subcategoryRepository.Find(id);
        }
        public IEnumerable<SubCategory> GetAllCategory()
        {
            return _subcategoryRepository.Query().Select();
        }
        //public IEnumerable<Brand> GetAllBrandByRegion()
        //{
        //    return _brandRepository.Query().Select().Where(u => u.RegionID == GlobalUser.getGlobalUser().RegionID).ToList();
        //}
        public SubCategory GetCategoryById(int id)
        {
            return _subcategoryRepository.Query(u => u.SubCategoryId == id).Select().FirstOrDefault();
        }
        public bool AddUpdateDeleteCategory(SubCategory subcategory, string action)
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
                    Insert(subcategory);
                }
                else if (action == "U")
                {
                    Update(subcategory);
                }
                else if (action == "D")
                {
                    Delete(subcategory);
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
        public List<SubCategory> GetListWT(Expression<Func<SubCategory, bool>> exp = null, Func<IQueryable<SubCategory>, IOrderedQueryable<SubCategory>> orderby = null)
        {
            return _subcategoryRepository.GetListWithNoTracking(exp, orderby);
        }


        public string ValidateCategory(SubCategory subcategorychk, string action)
        {
            string result = string.Empty;
            if (action == "I")
            {
                var categoryList = _subcategoryRepository.Query(u => u.SubCategoryName == subcategorychk.SubCategoryName).Select();
                if (categoryList.ToList().Count > 0)
                {
                    if (categoryList.Where(u => u.SubCategoryName == subcategorychk.SubCategoryName).FirstOrDefault() != null)
                    {
                        result = "SubCategory Name already exists!";
                        return result;
                    }

                }
            }
            else if (action == "U")
            {
                var categoryList = _subcategoryRepository.Query(u => u.SubCategoryId != subcategorychk.SubCategoryId && (u.SubCategoryName == subcategorychk.SubCategoryName)).Select();
                if (categoryList.ToList().Count > 0)
                {

                    return result;

                }
            }
            return result;
        }


    }
}
