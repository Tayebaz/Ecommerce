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
   public class OrderBusiness
    {
       
        public OrderDetailsRepository _orderRepository;
        private UnitOfWork _unitOfWork;
        public OrderBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _orderRepository = new OrderDetailsRepository(dfactory);
        }

        public OrderDetail Insert(OrderDetail Order)
        {
            _orderRepository.Insert(Order);
            return Order;
        }

        public OrderDetail Update(OrderDetail Order)
        {
            _orderRepository.Update(Order);
            return Order;
        }

        public void Delete(long? id)
        {
            _orderRepository.Delete(id);
        }

        public void Delete(OrderDetail Order)
        {
            _orderRepository.Delete(Order);
        }

        public OrderDetail Find(long? id)
        {
            return _orderRepository.Find(id);
        }
        public IEnumerable<OrderDetail> GetAllorder()
        {
            return _orderRepository.Query().Select();
        }
        //public IEnumerable<Brand> GetAllBrandByRegion()
        //{
        //    return _brandRepository.Query().Select().Where(u => u.RegionID == GlobalUser.getGlobalUser().RegionID).ToList();
        //}
        public OrderDetail GetOrderById(int id)
        {
            return _orderRepository.Query(u => u.OrderId == id).Select().FirstOrDefault();
        }
        public List<OrderDetail> GetListWT(Expression<Func<OrderDetail, bool>> exp = null, Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>> orderby = null)
        {
            return _orderRepository.GetListWithNoTracking(exp, orderby);
        }
        public bool AddUpdateDeleteOrder(OrderDetail Order, string action)
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
                    Insert(Order);
                }
                else if (action == "U")
                {
                    Update(Order);
                }
                else if (action == "D")
                {
                    Delete(Order);
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

        public string ValidateOrder(OrderDetail Orderchk, string action)
        {
            string result = string.Empty;
            //if (action == "I")
            //{
            //    var OrderList = _orderRepository.Query(u => u.CustomerName == Orderchk.CustomerName).Select();
            //    if (OrderList.ToList().Count > 0)
            //    {
            //        if (OrderList.Where(u => u.CustomerName == Orderchk.CustomerName).FirstOrDefault() != null)
            //        {
            //            result = "Order Name already exists!";
            //            return result;
            //        }

            //    }
            //}
            //else if (action == "U")
            //{
            //    var OrderList = _orderRepository.Query(u => u.OrderId != Orderchk.OrderId && (u.CustomerName == Orderchk.CustomerName)).Select();
            //    if (OrderList.ToList().Count > 0)
            //    {

            //        return result;

            //    }
            //}
            return result;
        }
    }
}
