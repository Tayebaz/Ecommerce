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
    public class OrderedItemBusiness
    {
        public OrderedItemRepository _OrderedItemRepository;
        private UnitOfWork _unitOfWork;
        public OrderedItemBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _OrderedItemRepository = new OrderedItemRepository(dfactory);
        }

        public OrderedItem Insert(OrderedItem orderedItem)
        {
            _OrderedItemRepository.Insert(orderedItem);
            return orderedItem;
        }

        public OrderedItem Update(OrderedItem orderedItem)
        {
            _OrderedItemRepository.Update(orderedItem);
            return orderedItem;
        }

        public void Delete(long? id)
        {
            _OrderedItemRepository.Delete(id);
        }

        public void Delete(OrderedItem orderedItem)
        {
            _OrderedItemRepository.Delete(orderedItem);
        }

        public OrderedItem Find(long? id)
        {
            return _OrderedItemRepository.Find(id);
        }
        public IEnumerable<OrderedItem> GetAllCategory()
        {
            return _OrderedItemRepository.Query().Select();
        }

        public List<OrderedItem> GetListWT(Expression<Func<OrderedItem, bool>> exp = null, Func<IQueryable<OrderedItem>, IOrderedQueryable<OrderedItem>> orderby = null)
        {
            return _OrderedItemRepository.GetListWithNoTracking(exp, orderby);
        }

        public List<OrderedItem> GetList(Expression<Func<OrderedItem, bool>> exp = null, Func<IQueryable<OrderedItem>, IOrderedQueryable<OrderedItem>> orderby = null)
        {
            return _OrderedItemRepository.GetList(exp, orderby);
        }
    }
}
