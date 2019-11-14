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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Ecommerce.Controllers
{
    public class AdminSaleRecordController : Controller
    {
        // GET: SaleRecord
        // GET: /OrderDetails/
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private OrderBusiness _orderBusiness;
        private OrderedItemBusiness _itemBusiness;
        private ProductBusiness _productBusiness;
        private ImageBusiness _ImageBusiness;

        public AdminSaleRecordController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._orderBusiness = new OrderBusiness(_df, this._unitOfWork);
            this._itemBusiness = new OrderedItemBusiness(_df, this._unitOfWork);
            this._productBusiness = new ProductBusiness(_df, this._unitOfWork);
            this._ImageBusiness = new ImageBusiness(_df, this._unitOfWork);
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetNewOrder(string sidx, string sord, int page, int rows, string colName, string colValue)  //Gets the todo Lists.
        {
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            List<SaleRecordViewModel> vmorder = new List<SaleRecordViewModel>();
            var productList = _productBusiness.GetListWT();
            var orderList = _orderBusiness.GetListWT();
            vmorder = (from c in orderList
                       select new SaleRecordViewModel
                       {
                           OrderId = c.OrderId,
                           OrderStatus = c.OrderStatus,
                           OrderCode = c.OrderCode,
                           OrderDate = c.OrderDate,
                           Vat = Convert.ToDouble(GetVATOrderedItems(c.OrderId, productList)),
                           Sat = Convert.ToDouble(GetSATOrderedItems(c.OrderId, productList)),
                          // TotalPrice = 2000,
                           TotalPrice = Convert.ToDouble(GetOrderedItems(c.OrderId, productList)),
                           CustomerName = c.FirstName + " " + c.LastName,
                           Discount = Convert.ToDouble(GetDiscountedOrderedItems(c.OrderId, productList)),
                          TaxTotalPrice = Convert.ToDouble(GetTotal(c.OrderId, productList))
                       }).OrderByDescending(col => col.OrderDate).Where(u => u.OrderStatus == "Delivered").ToList();

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

            if (!string.IsNullOrEmpty(colName) && !string.IsNullOrEmpty(colValue))
            {
                records = records.Where(c => c.GetType().GetProperty(colName).GetValue(c, null).ToString().ToLower().Contains(colValue.ToLower()));
            }
            //applying filter

           // int totalRecords = records.Count();
          //  var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

        
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
        public string GetOrderedItems(long orderId, List<Product> productList)
        {
            double result = 0;
            var orderItems = _itemBusiness.GetListWT(c => c.OrderId == orderId);
            var vmOrderedItems = from c in orderItems
                                 join p in productList
                                 on c.ProductId equals p.ProductID
                                 select new OrderedItemViewModel
                                 {
                                     OrderId = c.OrderId,
                                     OrderQuantity = c.OrderQuantity,
                                     ProductId = c.ProductId,
                                     ProductName = p.ProductName,
                                     Price = _productBusiness.GetDefaultPrice(c.ProductId)
                                 };

            foreach (var items in vmOrderedItems)
            {
                result = result + Convert.ToDouble(items.Price);
            }
            return result.ToString();
        }
        public string GetDiscountedOrderedItems(long orderId, List<Product> productList)
        {
            double result = 0;
            var orderItems = _itemBusiness.GetListWT(c => c.OrderId == orderId);
            var vmOrderedItems = from c in orderItems
                                 join p in productList
                                 on c.ProductId equals p.ProductID
                                 select new OrderedItemViewModel
                                 {
                                     OrderId = c.OrderId,
                                     OrderQuantity = c.OrderQuantity,
                                     ProductId = c.ProductId,
                                     ProductName = p.ProductName,
                                     Price = _productBusiness.GetDefaultPrice(p.ProductID),
                                     Discount = p.DiscountPercent
                                 };

            foreach (var items in vmOrderedItems)
            {
                result = result + Convert.ToDouble((Convert.ToDouble(items.Price) * items.Discount)/100);
            }
            return result.ToString();
        }
        public string GetSATOrderedItems(long orderId, List<Product> productList)
        {
            double result = 0;
            var orderItems = _itemBusiness.GetListWT(c => c.OrderId == orderId);
            var vmOrderedItems = from c in orderItems
                                 join p in productList
                                 on c.ProductId equals p.ProductID
                                 select new OrderedItemViewModel
                                 {
                                     OrderId = c.OrderId,
                                     OrderQuantity = c.OrderQuantity,
                                     ProductId = c.ProductId,
                                     ProductName = p.ProductName,
                                     Price = _productBusiness.GetDefaultPrice(p.ProductID),
                                     Discount = p.DiscountPercent,
                                    
                                 };

            foreach (var items in vmOrderedItems)
            {
                result = result + Convert.ToDouble((Convert.ToDouble(items.Price) * items.SAT) / 100);
            }
            return result.ToString();
        }
        public string GetVATOrderedItems(long orderId, List<Product> productList)
        {
            double result = 0;
            var orderItems = _itemBusiness.GetListWT(c => c.OrderId == orderId);
            var vmOrderedItems = from c in orderItems
                                 join p in productList
                                 on c.ProductId equals p.ProductID
                                 select new OrderedItemViewModel
                                 {
                                     OrderId = c.OrderId,
                                     OrderQuantity = c.OrderQuantity,
                                     ProductId = c.ProductId,
                                     ProductName = p.ProductName,
                                     Price = _productBusiness.GetDefaultPrice(p.ProductID),
                                     Discount = p.DiscountPercent
                                 };

            foreach (var items in vmOrderedItems)
            {
                result = result + Convert.ToDouble((Convert.ToDouble(items.Price) * items.VAT) / 100);
            }
            return result.ToString();
        }
        public string GetTotal(long orderId, List<Product> productList)
        {
            double result = 0;
            var orderItems = _itemBusiness.GetListWT(c => c.OrderId == orderId);
            var vmOrderedItems = from c in orderItems
                                 join p in productList
                                 on c.ProductId equals p.ProductID
                                 select new OrderedItemViewModel
                                 {
                                     OrderId = c.OrderId,
                                     OrderQuantity = c.OrderQuantity,
                                     ProductId = c.ProductId,
                                     ProductName = p.ProductName,
                                     Price = _productBusiness.GetDefaultPrice(p.ProductID),
                                     Discount = p.DiscountPercent
                                 };

            foreach (var items in vmOrderedItems)
            {
                result = result + (Convert.ToDouble((Convert.ToDouble(items.Price) * items.SAT) / 100) + Convert.ToDouble((Convert.ToDouble(items.Price) * items.VAT) / 100) - Convert.ToDouble((Convert.ToDouble(items.Price) * items.Discount) / 100) + Convert.ToDouble(items.Price));
            }
            return result.ToString();
        }

        public ActionResult ExportToExcel()

        {

           

            List<SaleRecordViewModel> vmorder = new List<SaleRecordViewModel>();
            var productList = _productBusiness.GetListWT();
            var orderList = _orderBusiness.GetListWT();
           
            var grid = new GridView();

            grid.DataSource = (from c in orderList
                               select new SaleRecordViewModel
                               {
                                   OrderId = c.OrderId,
                                   OrderCode = c.OrderCode,
                                   OrderDate = Convert.ToDateTime(c.OrderDate),
                                   Vat = Convert.ToDouble(GetVATOrderedItems(c.OrderId, productList)),
                                   Sat = Convert.ToDouble(GetSATOrderedItems(c.OrderId, productList)),
                                   // TotalPrice = 2000,
                                   TotalPrice = Convert.ToDouble(GetOrderedItems(c.OrderId, productList)),
                                   CustomerName = c.FirstName + " " + c.LastName,
                                   Discount = Convert.ToDouble(GetDiscountedOrderedItems(c.OrderId, productList)),
                                   TaxTotalPrice = Convert.ToDouble(GetTotal(c.OrderId, productList))
                               }).OrderByDescending(col => col.OrderDate).ToList();
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");

            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();

            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);

            Response.Write(sw.ToString());
 
            Response.End();

 
            return View("Index");
        }

    }
}