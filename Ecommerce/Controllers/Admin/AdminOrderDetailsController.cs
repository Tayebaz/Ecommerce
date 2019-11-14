using AutoMapper;
using Business;
using Common.Cryptography;
using Entities.Models;
using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Helper;
using Ecommerce.Models;
using Filters.ActionFilters;

namespace Ecommerce.Controllers
{
    [EcommerceAuthorize("Admin")]
    [RouteArea("Admin")]
    [RoutePrefix("Order")]
    public class AdminOrderDetailsController : Controller
    {

        //
        // GET: /OrderDetails/
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private OrderBusiness _orderBusiness;
        private OrderedItemBusiness _itemBusiness;
        private ProductBusiness _productBusiness;
        private ImageBusiness _ImageBusiness;

        public AdminOrderDetailsController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._orderBusiness = new OrderBusiness(_df, this._unitOfWork);
            this._itemBusiness = new OrderedItemBusiness(_df, this._unitOfWork);
            this._productBusiness = new ProductBusiness(_df, this._unitOfWork);
            this._ImageBusiness = new ImageBusiness(_df, this._unitOfWork);
        }
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
       
        public JsonResult GetNewOrder(string sidx, string sord, int page, int rows)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            List<OrderViewModel> vmorder = new List<OrderViewModel>();
            var productList = _productBusiness.GetListWT();
            var orderList = _orderBusiness.GetListWT(c => c.OrderStatus == "New");
            vmorder = (from c in orderList
                       select new OrderViewModel
                       {
                           OrderId = c.OrderId,
                           OrderCode = c.OrderCode,
                           OrderDate = c.OrderDate,
                           OrderStatus = c.OrderStatus,
                           OrderedItems = GetOrderedItems(c.OrderId, productList),
                           CustomerName = c.FirstName + " " + c.LastName,
                           //FullAddress = c.Address + ", " + c.Pincode + " " + c.City
                       }).OrderByDescending(col => col.OrderDate).ToList();

            var records = vmorder.AsQueryable();
            int totalRecords = records.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (!string.IsNullOrEmpty(sidx) && !string.IsNullOrEmpty(sord))
            {
                if (sord.Trim().ToLower() == "asc")
                {
                    records = SortHelper.OrderBy(records, sidx);
                }
                else
                {
                    records = SortHelper.OrderByDescending(records, sidx);
                }
            }

            //applying paging
            records = records.Skip(pageIndex * pageSize).Take(pageSize);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = records
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Route("GetOrderedItems")]
        public string GetOrderedItems(long orderId, List<Product> productList)
        {
            string result = string.Empty;
            var orderItems = _itemBusiness.GetListWT(c => c.OrderId == orderId);
            var vmOrderedItems = (from c in orderItems
                                 join p in productList
                                 on c.ProductId equals p.ProductID
                                 select new OrderedItemViewModel
                                 {
                                     OrderId = c.OrderId,
                                     OrderQuantity = c.OrderQuantity,
                                     ProductId = c.ProductId,
                                     ProductName = p.ProductName,
                                     Price = _productBusiness.GetDefaultPrice(c.ProductId)
                                 }).ToList();

            foreach (var items in vmOrderedItems)
            {
                result = result + ", " + items.ProductName;
            }
            if (result.Length > 0)
                result = result.Substring(1);
            return result;
        }

        [Route("PendingOrder")]
        public ActionResult PendingOrder()
        {
            return View();
        }

      
        public JsonResult GetPendingOrder(string sidx, string sord, int page, int rows, string colName, string colValue)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            List<OrderViewModel> vmorder = new List<OrderViewModel>();
            var productList = _productBusiness.GetListWT();
            var orderList = _orderBusiness.GetListWT(c => c.OrderStatus == "Pending");
            vmorder = (from c in orderList
                       select new OrderViewModel
                       {
                           OrderId = c.OrderId,
                           OrderCode = c.OrderCode,
                           OrderDate = c.OrderDate,
                           OrderStatus = c.OrderStatus,
                           OrderedItems = GetOrderedItems(c.OrderId, productList),
                           CustomerName = c.FirstName + " " + c.LastName,
                           //FullAddress = c.Address + ", " + c.Pincode + " " + c.City
                       }).OrderByDescending(col => col.OrderDate).ToList();

            var records = vmorder.AsQueryable();

            if (!string.IsNullOrEmpty(colName) && !string.IsNullOrEmpty(colValue))
            {
                records = records.Where(c => c.GetType().GetProperty(colName).GetValue(c, null).ToString().ToLower().Contains(colValue.ToLower()));
            }

            int totalRecords = records.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (!string.IsNullOrEmpty(sidx) && !string.IsNullOrEmpty(sord))
            {
                if (sord.Trim().ToLower() == "asc")
                {
                    records = SortHelper.OrderBy(records, sidx);
                }
                else
                {
                    records = SortHelper.OrderByDescending(records, sidx);
                }
            }

            //applying paging
            records = records.Skip(pageIndex * pageSize).Take(pageSize);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = records
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Route("CancelledOrder")]
        public ActionResult CancelledOrder()
        {
            return View();
        }

       
        public JsonResult GetCancelledOrder(string sidx, string sord, int page, int rows)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            List<OrderViewModel> vmorder = new List<OrderViewModel>();
            var productList = _productBusiness.GetListWT();
            var orderList = _orderBusiness.GetListWT(c => c.OrderStatus == "Cancelled");
            vmorder = (from c in orderList
                       select new OrderViewModel
                       {
                           OrderId = c.OrderId,
                           OrderCode = c.OrderCode,
                           OrderDate = c.OrderDate,
                           OrderStatus = c.OrderStatus,
                           OrderedItems = GetOrderedItems(c.OrderId, productList),
                           CustomerName = c.FirstName + " " + c.LastName,
                           //FullAddress = c.Address + ", " + c.Pincode + " " + c.City
                       }).OrderByDescending(col => col.OrderDate).ToList();

            var records = vmorder.AsQueryable();


            int totalRecords = records.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (!string.IsNullOrEmpty(sidx) && !string.IsNullOrEmpty(sord))
            {
                if (sord.Trim().ToLower() == "asc")
                {
                    records = SortHelper.OrderBy(records, sidx);
                }
                else
                {
                    records = SortHelper.OrderByDescending(records, sidx);
                }
            }

            //applying paging
            records = records.Skip(pageIndex * pageSize).Take(pageSize);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = records
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Route("DeliveredOrder")]
        public ActionResult DeliveredOrder()
        {
            return View();
        }

       
        public JsonResult GetDeliveredOrder(string sidx, string sord, int page, int rows)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            List<OrderViewModel> vmorder = new List<OrderViewModel>();
            var productList = _productBusiness.GetListWT();
            var orderList = _orderBusiness.GetListWT(c => c.OrderStatus == "Delivered");
            vmorder = (from c in orderList
                       select new OrderViewModel
                       {
                           OrderId = c.OrderId,
                           OrderCode = c.OrderCode,
                           OrderDate = c.OrderDate,
                           OrderStatus = c.OrderStatus,
                           OrderedItems = GetOrderedItems(c.OrderId, productList),
                           CustomerName = c.FirstName + " " + c.LastName,
                           //FullAddress = c.Address + ", " + c.Pincode + " " + c.City
                       }).OrderByDescending(col => col.OrderDate).ToList();

            var records = vmorder.AsQueryable();


            int totalRecords = records.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (!string.IsNullOrEmpty(sidx) && !string.IsNullOrEmpty(sord))
            {
                if (sord.Trim().ToLower() == "asc")
                {
                    records = SortHelper.OrderBy(records, sidx);
                }
                else
                {
                    records = SortHelper.OrderByDescending(records, sidx);
                }
            }

            //applying paging
            records = records.Skip(pageIndex * pageSize).Take(pageSize);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = records
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Route("AllOrders")]
        public ActionResult AllOrders()
        {
            return View();
        }

       
        public JsonResult GetallOrderStatus(string sidx, string sord, int page, int rows)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            List<OrderViewModel> vmorder = new List<OrderViewModel>();
            var productList = _productBusiness.GetListWT();
            var orderList = _orderBusiness.GetListWT();
            vmorder = (from c in orderList
                       select new OrderViewModel
                       {
                           OrderId = c.OrderId,
                           OrderCode = c.OrderCode,
                           OrderDate = c.OrderDate,
                           OrderStatus = c.OrderStatus,
                           OrderedItems = GetOrderedItems(c.OrderId, productList),
                           CustomerName = c.FirstName + " " + c.LastName,
                           //FullAddress = c.Address + ", " + c.Pincode + " " + c.City
                       }).OrderByDescending(col => col.OrderDate).ToList();

            var records = vmorder.AsQueryable();


            int totalRecords = records.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            if (!string.IsNullOrEmpty(sidx) && !string.IsNullOrEmpty(sord))
            {
                if (sord.Trim().ToLower() == "asc")
                {
                    records = SortHelper.OrderBy(records, sidx);
                }
                else
                {
                    records = SortHelper.OrderByDescending(records, sidx);
                }
            }

            //applying paging
            records = records.Skip(pageIndex * pageSize).Take(pageSize);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = records
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Route("orderitemdetails")]
        public ActionResult orderitemdetails(int orderId)
        {
            OrderViewModel vmorder = new OrderViewModel();
            var order = _orderBusiness.GetListWT(c => c.OrderId == orderId).FirstOrDefault();
            var orderedItem = _itemBusiness.GetListWT(c => c.OrderId == orderId);
            var productList = _productBusiness.GetListWT();
            Mapper.CreateMap<OrderDetail, OrderViewModel>();
            vmorder = Mapper.Map<OrderDetail, OrderViewModel>(order);
            //showing cartdata to checkout
            var imgList = _ImageBusiness.GetListWT();
            var vmProductList = (from c in productList
                                 join oi in orderedItem
                                 on c.ProductID equals oi.ProductId
                                 select new ItemListViewModel
                                 {
                                     ProductID = c.ProductID,
                                     ProductCode = c.ProductCode,
                                     ProductName = oi.ProductName,
                                     TokenKey = c.TokenKey,
                                     ShortDescription = c.ShortDescription,
                                     Price = oi.Price ?? 0,
                                     quantity = oi.OrderQuantity,
                                     DiscountPercent = oi.DiscountPercent,
                                     DiscountedPrice = Math.Round(oi.Price ?? 0 - Decimal.Divide(oi.DiscountPercent ?? 0, 100) * oi.Price ?? 0),
                                     SizeId = oi.Size.Value,
                                     Size = _productBusiness.GetSizeName(c.ProductID, oi.Size.Value),
                                     AttributeId = oi.Attributes,
                                     Attributes = _productBusiness.GetAttributes(c.ProductID, oi.Attributes),
                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images = "/ProductImage/" + il.Images
                                                  }).ToList()

                                 }).ToList();
            vmorder.orderItems = vmProductList;

            return View(vmorder);
        }

        [Route("ChangeStatusCancel")]
        public JsonResult ChangeStatusCancel(string stkns)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Status Changed!";
            _unitOfWork.BeginTransaction();
            try
            {
                foreach (string tokenkey in stkns.Substring(1).Split('~'))
                {

                    int id = Convert.ToInt32(tokenkey);

                    List<OrderDetail> orderList = _orderBusiness.GetListWT(x => x.OrderId == id).ToList();
                    foreach (OrderDetail order in orderList)
                    {
                        int deleteId = Convert.ToInt32(order.OrderId);
                        // var br = _orderBusiness.Find(deleteId);

                        order.OrderStatus = "Cancelled";
                        _orderBusiness.AddUpdateDeleteOrder(order, "U");
                        _unitOfWork.SaveChanges();
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                message = "Change Unsuccessful!";
                throw ex;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }

        [Route("ChangeStatusDelivered")]
        public JsonResult ChangeStatusDelivered(string stkns)
        {
            string JsonStr = "";
            bool isSuccess = true;
            string message = "Status Changed!";
            _unitOfWork.BeginTransaction();
            try
            {
                foreach (string tokenkey in stkns.Substring(1).Split('~'))
                {

                    int id = Convert.ToInt32(tokenkey);

                    List<OrderDetail> orderList = _orderBusiness.GetListWT(x => x.OrderId == id).ToList();
                    foreach (OrderDetail order in orderList)
                    {
                        int deleteId = Convert.ToInt32(order.OrderId);
                        // var br = _orderBusiness.Find(deleteId);

                        order.OrderStatus = "Delivered";
                        _orderBusiness.AddUpdateDeleteOrder(order, "U");
                        _unitOfWork.SaveChanges();
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                message = "Change Unsuccessful!";
                throw ex;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            TempData["Success"] = message;
            TempData["isSuccess"] = isSuccess.ToString();

            JsonStr = "{\"message\":\"" + message + "\",\"isSuccess\":\"" + isSuccess + "\"}";
            return Json(JsonStr, JsonRequestBehavior.AllowGet);
        }
    }
}
