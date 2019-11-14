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
    public class ReviewBusiness
    {
        public ReviewRepository _reviewRepository;
        private UnitOfWork _unitOfWork;
        public ReviewBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _reviewRepository = new ReviewRepository(dfactory);
        }

        public Review Insert(Review review)
        {
            _reviewRepository.Insert(review);
            return review;
        }

        public Review Update(Review review)
        {
            _reviewRepository.Update(review);
            return review;
        }

        public void Delete(long? id)
        {
            _reviewRepository.Delete(id);
        }

        public void Delete(Review review)
        {
            _reviewRepository.Delete(review);
        }

        public Review Find(long? id)
        {
            return _reviewRepository.Find(id);
        }
        public IEnumerable<Review> GetAllCategory()
        {
            return _reviewRepository.Query().Select();
        }

        public List<Review> GetListWT(Expression<Func<Review, bool>> exp = null, Func<IQueryable<Review>, IOrderedQueryable<Review>> orderby = null)
        {
            return _reviewRepository.GetListWithNoTracking(exp, orderby);
        }
    }
}
