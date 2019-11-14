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
    public class WishListBusiness
    {
        public WishListRepository _wishListRepository;
        private UnitOfWork _unitOfWork;
        public WishListBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _wishListRepository = new WishListRepository(dfactory);
        }

        public WishList Insert(WishList wishList)
        {
            _wishListRepository.Insert(wishList);
            return wishList;
        }

        public WishList Update(WishList wishList)
        {
            _wishListRepository.Update(wishList);
            return wishList;
        }

        public void Delete(long? id)
        {
            _wishListRepository.Delete(id);
        }

        public void Delete(WishList wishList)
        {
            _wishListRepository.Delete(wishList);
        }

        public WishList Find(long? id)
        {
            return _wishListRepository.Find(id);
        }
        public IEnumerable<WishList> GetAllCategory()
        {
            return _wishListRepository.Query().Select();
        }

        public List<WishList> GetListWT(Expression<Func<WishList, bool>> exp = null, Func<IQueryable<WishList>, IOrderedQueryable<WishList>> orderby = null)
        {
            return _wishListRepository.GetListWithNoTracking(exp, orderby);
        }

        public List<WishList> GetList(Expression<Func<WishList, bool>> exp = null, Func<IQueryable<WishList>, IOrderedQueryable<WishList>> orderby = null)
        {
            return _wishListRepository.GetList(exp, orderby);
        }
    }
}
