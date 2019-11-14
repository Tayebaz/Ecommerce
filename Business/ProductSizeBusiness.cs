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
  public  class ProductSizeBusiness
    {
        public ProductRepository _productRepository;
        public ProductSizeRepository _ProductSizesRepository;
        private UnitOfWork _unitOfWork;
        public ProductSizeBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _productRepository = new ProductRepository(dfactory);
            _ProductSizesRepository = new ProductSizeRepository(dfactory);
        }

        public ProductSize Insert(ProductSize ProductSize)
        {
            _ProductSizesRepository.Insert(ProductSize);
            return ProductSize;
        }

        public ProductSize Update(ProductSize ProductSize)
        {
            _ProductSizesRepository.Update(ProductSize);
            return ProductSize;
        }
        public ProductSize Find(long? id)
        {
            return _ProductSizesRepository.Find(id);
        }

        public void Delete(long? id)
        {
            _ProductSizesRepository.Delete(id);
        }
       
        public void Delete(ProductSize ProductSize)
        {
            _ProductSizesRepository.Delete(ProductSize);
        }
        public IEnumerable<ProductSize> GetAllProductSizesbyProduct(int n)
        {
            return _ProductSizesRepository.Query().Select().Where(u=> u.ProductId == n);
        }
        public bool AddUpdateDeleteProductSize(ProductSize ProductSize, string action)
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
                    Insert(ProductSize);
                }
                else if (action == "U")
                {
                    Update(ProductSize);
                }
                else if (action == "D")
                {
                    Delete(ProductSize);
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

        public List<ProductSize> GetListWT(Expression<Func<ProductSize, bool>> exp = null, Func<IQueryable<ProductSize>, IOrderedQueryable<ProductSize>> orderby = null)
        {
            return _ProductSizesRepository.GetListWithNoTracking(exp, orderby);
        }
    }
}
