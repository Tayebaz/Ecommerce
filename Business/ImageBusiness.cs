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
  public  class ImageBusiness
    {
        public ProductRepository _productRepository;
        public ImagesRepository _imagesRepository;
        private UnitOfWork _unitOfWork;
        public ImageBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _productRepository = new ProductRepository(dfactory);
            _imagesRepository = new ImagesRepository(dfactory);
        }

        public Image Insert(Image image)
        {
            _imagesRepository.Insert(image);
            return image;
        }

        public Image Update(Image image)
        {
            _imagesRepository.Update(image);
            return image;
        }
        public Image Find(long? id)
        {
            return _imagesRepository.Find(id);
        }

        public void Delete(long? id)
        {
            _imagesRepository.Delete(id);
        }
       
        public void Delete(Image image)
        {
            _imagesRepository.Delete(image);
        }
        public IEnumerable<Image> GetAllImagesbyProduct(int n)
        {
            return _imagesRepository.Query().Select().Where(u=> u.ProductId == n);
        }
        public bool AddUpdateDeleteimage(Image image, string action)
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
                    Insert(image);
                }
                else if (action == "U")
                {
                    Update(image);
                }
                else if (action == "D")
                {
                    Delete(image);
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

        public List<Image> GetListWT(Expression<Func<Image, bool>> exp = null, Func<IQueryable<Image>, IOrderedQueryable<Image>> orderby = null)
        {
            return _imagesRepository.GetListWithNoTracking(exp, orderby);
        }
    }
}
