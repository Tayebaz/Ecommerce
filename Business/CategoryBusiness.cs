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
  public  class CategoryBusiness
    {
           public CategoryRepository _categoryRepository;
        private UnitOfWork _unitOfWork;
        public CategoryBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _categoryRepository = new CategoryRepository(dfactory);
        }

        public Category Insert(Category Category)
        {
            _categoryRepository.Insert(Category);
            return Category;
        }

        public Category Update(Category Category)
        {
            _categoryRepository.Update(Category);
            return Category;
        }

        public void Delete(long? id)
        {
            _categoryRepository.Delete(id);
        }

        public void Delete(Category Category)
        {
            _categoryRepository.Delete(Category);
        }

        public Category Find(long? id)
        {
            return _categoryRepository.Find(id);
        }
        public IEnumerable<Category> GetAllCategory()
        {
            return _categoryRepository.Query().Select();
        }
        //public IEnumerable<Brand> GetAllBrandByRegion()
        //{
        //    return _brandRepository.Query().Select().Where(u => u.RegionID == GlobalUser.getGlobalUser().RegionID).ToList();
        //}
        public Category GetCategoryById(int id)
        {
            var category = _categoryRepository.Query(u => u.CategoryId == id).Select().FirstOrDefault();
            if (category == null)
            {
                category = new Category();
            }
            return category;
        }
        public bool AddUpdateDeleteCategory(Category category, string action)
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
                    Insert(category);
                }
                else if (action == "U")
                {
                    Update(category);
                }
                else if (action == "D")
                {
                    Delete(category);
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
        public List<Category> GetListWT(Expression<Func<Category, bool>> exp = null, Func<IQueryable<Category>, IOrderedQueryable<Category>> orderby = null)
        {
            return _categoryRepository.GetListWithNoTracking(exp, orderby);
        }


        public string ValidateCategory(Category categorychk, string action)
        {
            string result = string.Empty;
            if (action == "I")
            {
                var categoryList = _categoryRepository.Query(u => u.CategoryName == categorychk.CategoryName).Select();
                if (categoryList.ToList().Count > 0)
                {
                    if (categoryList.Where(u => u.CategoryName == categorychk.CategoryName).FirstOrDefault() != null)
                    {
                        result = "Category Name already exists!";
                        return result;
                    }

                }
            }
            else if (action == "U")
            {
                var categoryList = _categoryRepository.Query(u => u.CategoryId != categorychk.CategoryId && (u.CategoryName == categorychk.CategoryName)).Select();
                if (categoryList.ToList().Count > 0)
                {

                    return result;

                }
            }
            return result;
        }


    }
}
