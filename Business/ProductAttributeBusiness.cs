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
  public  class ProductAttributeBusiness
    {
        public ProductRepository _productRepository;
        public ProductAttributeRepository _ProductAttributesRepository;
        private UnitOfWork _unitOfWork;
        public ProductAttributeBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _productRepository = new ProductRepository(dfactory);
            _ProductAttributesRepository = new ProductAttributeRepository(dfactory);
        }

        public ProductAttribute Insert(ProductAttribute ProductAttribute)
        {
            _ProductAttributesRepository.Insert(ProductAttribute);
            return ProductAttribute;
        }

        public ProductAttribute Update(ProductAttribute ProductAttribute)
        {
            _ProductAttributesRepository.Update(ProductAttribute);
            return ProductAttribute;
        }
        public ProductAttribute Find(long? id)
        {
            return _ProductAttributesRepository.Find(id);
        }

        public void Delete(long? id)
        {
            _ProductAttributesRepository.Delete(id);
        }
       
        public void Delete(ProductAttribute ProductAttribute)
        {
            _ProductAttributesRepository.Delete(ProductAttribute);
        }
        public IEnumerable<ProductAttribute> GetAllProductAttributesbyProduct(int n)
        {
            return _ProductAttributesRepository.Query().Select().Where(u=> u.ProductId == n);
        }
        public bool AddUpdateDeleteProductAttribute(ProductAttribute ProductAttribute, string action)
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
                    Insert(ProductAttribute);
                }
                else if (action == "U")
                {
                    Update(ProductAttribute);
                }
                else if (action == "D")
                {
                    Delete(ProductAttribute);
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

        public List<ProductAttribute> GetListWT(Expression<Func<ProductAttribute, bool>> exp = null, Func<IQueryable<ProductAttribute>, IOrderedQueryable<ProductAttribute>> orderby = null)
        {
            return _ProductAttributesRepository.GetListWithNoTracking(exp, orderby);
        }
    }
}
