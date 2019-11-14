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
   public class AddToCartBusiness
    {  
        public AddToCartRepository _cartRepository;      
        private UnitOfWork _unitOfWork;
        public AddToCartBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _cartRepository = new AddToCartRepository(dfactory);           
        }
        public AddToCart Insert(AddToCart Cart)
        {
            _cartRepository.Insert(Cart);
            return Cart;
        }

        public AddToCart Update(AddToCart Cart)
        {
            _cartRepository.Update(Cart);
            return Cart;
        }

        public void Delete(long? id)
        {
            _cartRepository.Delete(id);
        }

        public void Delete(AddToCart Cart)
        {
            _cartRepository.Delete(Cart);
        }

        public AddToCart Find(long? id)
        {
            return _cartRepository.Find(id);
        }
        public IEnumerable<AddToCart> GetAllCart()
        {
            return _cartRepository.Query().Select();
        }
        //public IEnumerable<Brand> GetAllBrandByRegion()
        //{
        //    return _brandRepository.Query().Select().Where(u => u.RegionID == GlobalUser.getGlobalUser().RegionID).ToList();
        //}
        public AddToCart GetCartById(int id)
        {
            return _cartRepository.Query(u => u.ID == id).Select().FirstOrDefault();
        }

        public List<AddToCart> GetListWT(Expression<Func<AddToCart, bool>> exp = null, Func<IQueryable<AddToCart>, IOrderedQueryable<AddToCart>> orderby = null)
        {
            return _cartRepository.GetListWithNoTracking(exp, orderby);
        }

        public List<AddToCart> GetList(Expression<Func<AddToCart, bool>> exp = null, Func<IQueryable<AddToCart>, IOrderedQueryable<AddToCart>> orderby = null)
        {
            return _cartRepository.GetList(exp, orderby);
        }
     
    }
}
