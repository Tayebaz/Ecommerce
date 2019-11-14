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
    public class ProductBusiness
    {
        public ProductRepository _productRepository;
        public ProductSizeRepository _productSizeRepository;
        public ProductAttributeRepository _productAttributeRepository;
        private UnitOfWork _unitOfWork;
        public ProductBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _productRepository = new ProductRepository(dfactory);
            _productSizeRepository = new ProductSizeRepository(dfactory);
            _productAttributeRepository = new ProductAttributeRepository(dfactory);
        }

        public Product Insert(Product Product)
        {
            _productRepository.Insert(Product);
            return Product;
        }

        public Product Update(Product Product)
        {
            _productRepository.Update(Product);
            return Product;
        }

        public void Delete(long? id)
        {
            _productRepository.Delete(id);
        }

        public void Delete(Product Product)
        {
            _productRepository.Delete(Product);
        }

        public Product Find(long? id)
        {
            return _productRepository.Find(id);
        }
        public List<SelectListItem> Getcolor()
        {
            List<SelectListItem> colorType = new List<SelectListItem>();
            colorType.Add(new SelectListItem { Text = "Black", Value = "Black" });
            colorType.Add(new SelectListItem { Text = "Red", Value = "Red" });
            colorType.Add(new SelectListItem { Text = "Green", Value = "Green" });
            colorType.Add(new SelectListItem { Text = "Yellow", Value = "Yellow" });
            colorType.Add(new SelectListItem { Text = "Blue", Value = "Blue" });
            colorType.Add(new SelectListItem { Text = "White", Value = "White" });
            colorType.Add(new SelectListItem { Text = "Silver", Value = "Silver" });
            colorType.Add(new SelectListItem { Text = "Golden", Value = "Golden" });
            //   questionType.Add(new SelectListItem { Text = "Boolean", Value = "Booleanvalue" });         
            return colorType;
        }

        public List<SelectListItem> GetCondition()
        {
            List<SelectListItem> colorType = new List<SelectListItem>();
            colorType.Add(new SelectListItem { Text = "New", Value = "New" });
            colorType.Add(new SelectListItem { Text = "Sale", Value = "Sale" });
            return colorType;
        }
        public IEnumerable<Product> GetAllProduct()
        {
            return _productRepository.Query().Select();
        }
        //public IEnumerable<Brand> GetAllBrandByRegion()
        //{
        //    return _brandRepository.Query().Select().Where(u => u.RegionID == GlobalUser.getGlobalUser().RegionID).ToList();
        //}
        public Product GetProductById(int id)
        {
            return _productRepository.Query(u => u.ProductID == id).Select().FirstOrDefault();
        }

        public bool AddUpdateDeleteProduct(Product product, string action)
        {
            bool isSuccess = true;
            try
            {
                if (action == "I")
                {
                    Insert(product);
                }
                else if (action == "U")
                {
                    Update(product);
                }
                else if (action == "D")
                {
                    Delete(product);
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

        public List<Product> GetListWT(Expression<Func<Product, bool>> exp = null, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderby = null)
        {
            return _productRepository.GetListWithNoTracking(exp, orderby);
        }

        public decimal GetDefaultPrice(int productId)
        {
            decimal result = 0;
            var defaultSize = _productSizeRepository.GetListWithNoTracking(c => c.ProductId == productId).FirstOrDefault();
            var defaultPrice = _productRepository.Find(productId);
            var price = defaultPrice.Price;
            if (defaultSize != null)
            {
                result = defaultSize.Price;
            }
            return price;
        }

        public decimal GetSelectedPrice(int productId, int size, string attributes)
        {
            decimal result = 0;
            var defaultPrice = _productRepository.Find(productId);
            if (defaultPrice != null)
            {
                result = defaultPrice.Price;
            }
            var defaultSize = _productSizeRepository.GetListWithNoTracking(c => c.ProductId == productId && c.Id == size).FirstOrDefault();
            if (defaultSize != null)
            {
                result += defaultSize.Price;
            }

            if(attributes != null && attributes != "")
            {
                var attrList = attributes.Split('`');
                foreach(var attr in attrList)
                {
                    var attrId = Convert.ToInt32(attr);
                    var dbattr = _productAttributeRepository.Find(attrId);
                    result += dbattr.Price;
                }
            }
            return result;
        }

        public string GetSizeName(int productId, int size)
        {
            string result = string.Empty;
            var defaultSize = _productSizeRepository.GetListWithNoTracking(c => c.ProductId == productId && c.Id == size).FirstOrDefault();
            if (defaultSize != null)
            {
                result += defaultSize.Size;
            }
            return result;
        }

        public string GetAttributes(int productId, string attributes)
        {
            string result = string.Empty;
            if (attributes != "")
            {
                var attrList = attributes.Split('`');
                foreach (var attr in attrList)
                {
                    var attrId = Convert.ToInt32(attr);
                    var dbattr = _productAttributeRepository.Find(attrId);
                    result += "," + dbattr.Attributes;
                }
            }
            return result == "" ? "" : result.Substring(1);
        }
    }
}
 