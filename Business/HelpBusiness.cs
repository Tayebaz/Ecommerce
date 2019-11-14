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
    public class HelpBusiness
    {
        public HelpRepository _helpRepository;
        private UnitOfWork _unitOfWork;
        public HelpBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _helpRepository = new HelpRepository(dfactory);
        }

        public Help Insert(Help help)
        {
            _helpRepository.Insert(help);
            return help;
        }

        public Help Update(Help help)
        {
            _helpRepository.Update(help);
            return help;
        }

        public void Delete(long? id)
        {
            _helpRepository.Delete(id);
        }

        public void Delete(Help help)
        {
            _helpRepository.Delete(help);
        }

        public Help Find(long? id)
        {
            return _helpRepository.Find(id);
        }
        public IEnumerable<Help> GetAllCategory()
        {
            return _helpRepository.Query().Select();
        }

        public List<Help> GetListWT(Expression<Func<Help, bool>> exp = null, Func<IQueryable<Help>, IOrderedQueryable<Help>> orderby = null)
        {
            return _helpRepository.GetListWithNoTracking(exp, orderby);
        }
    }
}
