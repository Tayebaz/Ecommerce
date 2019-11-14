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
    public class CompareListBusiness
    {
        public CompareListRepository _CompareListRepository;
        private UnitOfWork _unitOfWork;
        public CompareListBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _CompareListRepository = new CompareListRepository(dfactory);
        }

        public CompareList Insert(CompareList CompareList)
        {
            _CompareListRepository.Insert(CompareList);
            return CompareList;
        }

        public CompareList Update(CompareList CompareList)
        {
            _CompareListRepository.Update(CompareList);
            return CompareList;
        }

        public void Delete(long? id)
        {
            _CompareListRepository.Delete(id);
        }

        public void Delete(CompareList CompareList)
        {
            _CompareListRepository.Delete(CompareList);
        }

        public CompareList Find(long? id)
        {
            return _CompareListRepository.Find(id);
        }
        public IEnumerable<CompareList> GetAllCategory()
        {
            return _CompareListRepository.Query().Select();
        }

        public List<CompareList> GetListWT(Expression<Func<CompareList, bool>> exp = null, Func<IQueryable<CompareList>, IOrderedQueryable<CompareList>> orderby = null)
        {
            return _CompareListRepository.GetListWithNoTracking(exp, orderby);
        }

        public List<CompareList> GetList(Expression<Func<CompareList, bool>> exp = null, Func<IQueryable<CompareList>, IOrderedQueryable<CompareList>> orderby = null)
        {
            return _CompareListRepository.GetList(exp, orderby);
        }
    }
}
