using AutoMapper;
using Business;
using Commom;
using Commom.GlobalMethods;
using Common.GlobalData;
using Entities.Models;
using Filters.AuthenticationModel;
using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;
using Common.GlobalMethods;
using Neodynamic.SDK.Printing;

namespace GrihastiWebsite.Controllers
{
    public class CheckOutController : Controller
    {
        private DatabaseFactory _df = new DatabaseFactory();
        private UnitOfWork _unitOfWork;
        private UserBusiness _userBusiness;
        private AddToCartBusiness _AddToCartBusiness;
        private ProductBusiness _productBusiness;
        private ImageBusiness _ImageBusiness;
        private OrderBusiness _OrderBusiness;
        private OrderedItemBusiness _OrderedItemBusiness;
        EcommerceContext db = new EcommerceContext();

        public CheckOutController()
        {
            this._unitOfWork = new UnitOfWork(_df);
            this._userBusiness = new UserBusiness(_df, this._unitOfWork);
            this._AddToCartBusiness = new AddToCartBusiness(_df, this._unitOfWork);
            this._productBusiness = new ProductBusiness(_df, this._unitOfWork);
            this._ImageBusiness = new ImageBusiness(_df, this._unitOfWork);
            this._OrderBusiness = new OrderBusiness(_df, this._unitOfWork);
            this._OrderedItemBusiness = new OrderedItemBusiness(_df, this._unitOfWork);
        }
        //
        // GET: /CheckOut/

        public ActionResult Index()
        {
          

            EcommerceContext db = new EcommerceContext();
            string[] WorkingHour=null;


            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                WorkingHour = db.Settings.AsNoTracking().ToList()[0].Sat.ToString().Split('-');
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                WorkingHour = db.Settings.AsNoTracking().ToList()[0].Sun.ToString().Split('-');
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                WorkingHour = db.Settings.AsNoTracking().ToList()[0].Mon.ToString().Split('-');
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
                WorkingHour = db.Settings.AsNoTracking().ToList()[0].Tue.ToString().Split('-');
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
                WorkingHour = db.Settings.AsNoTracking().ToList()[0].Wed.ToString().Split('-');
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                WorkingHour = db.Settings.AsNoTracking().ToList()[0].Thur.ToString().Split('-');
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                WorkingHour = db.Settings.AsNoTracking().ToList()[0].Fri.ToString().Split('-');




            DateTime  FromDateTime  = DateTime.Parse( DateTime.Now.ToString("MM/dd/yyyy") + " " + WorkingHour[0]);
            DateTime ToDateTime = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy") + " " + WorkingHour[1]);

            if (FromDateTime.ToString("tt").ToLower() == "pm" && ToDateTime.ToString("tt").ToLower() == "am")
                ToDateTime = ToDateTime.AddDays(1);


            double iWorkingHour = (  ToDateTime - FromDateTime).TotalHours;
            double CurrentTotalHour =  (DateTime.Now - FromDateTime  ).TotalHours;

            bool IsClosed = false;

            if (CurrentTotalHour >= iWorkingHour)
                IsClosed = true;
                                            CheckOutViewModel chkOut = new CheckOutViewModel();
            var assignedProductList = new List<AddToCart>();

            var productList = _productBusiness.GetListWT();
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);

            CookieStore mycookie = new CookieStore();
            var products = mycookie.GetCookie(Enumerator.CustomerAction.AddToCart.ToString());
            if (!string.IsNullOrEmpty(products))
            {
                assignedProductList = (from p in products.Split(',')
                                       select new AddToCart
                                       {
                                           ProductId = Convert.ToInt32(p.Split('~')[0]),
                                           Quantity = Convert.ToInt32(p.Split('~')[1]),
                                           Size = Convert.ToInt32(p.Split('~')[2]),
                                           Attributes = p.Split('~')[3]
                                       }).ToList();
            }
            else
            {
                //Can't place order if cart empty
                // display message here 

            }

            //assignedProductList = _AddToCartBusiness.GetListWT(c => c.UserId == currentUserId);


            //showing cartdata to checkout
            var imgList = _ImageBusiness.GetListWT();
            var vmProductList = (from c in productList
                                 join ap in assignedProductList
                                 on c.ProductID equals ap.ProductId
                                 select new CartWishlistViewModel
                                 {
                                     ProductID = c.ProductID,
                                     ProductCode = c.ProductCode,
                                     ProductName = c.ProductName,
                                     TokenKey = c.TokenKey,
                                     ShortDescription = c.ShortDescription,
                                     Price = _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.GetValueOrDefault(), ap.Attributes),
                                     DiscountPercent = c.DiscountPercent,
                                     DiscountedPrice = 0,//Math.Round(_productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes) - Decimal.Divide(c.DiscountPercent ?? 0, 100) * _productBusiness.GetSelectedPrice(c.ProductID, ap.Size.Value, ap.Attributes)),
                                     SizeId = ap.Size.Value,
                                     Size = _productBusiness.GetSizeName(c.ProductID, ap.Size.GetValueOrDefault()),
                                     AttributeId = ap.Attributes,
                                     Attributes = _productBusiness.GetAttributes(c.ProductID, ap.Attributes),
                                     quantity = ap.Quantity,
                                     ImageList = (from il in imgList
                                                  where (il.ProductId == c.ProductID)
                                                  select new ImageViewModel
                                                  {
                                                      ProductId = c.ProductID,
                                                      Images = "/ProductImage/" + il.Images
                                                  }).ToList()

                                 }).ToList();




            chkOut.OrderList = vmProductList;
            _userBusiness = new UserBusiness();
            var currentUser = _userBusiness.GetUserByemail(Session["CurrentUserEmail"].ToString());
            chkOut.FirstNameShopper = currentUser.FirstName;
            chkOut.LastNameShopper = currentUser.LastName;
            chkOut.EmailShopper = currentUser.Email;
            chkOut.IsBlocked = currentUser.IsBlocked;
            chkOut.IsStoreClosed = IsClosed;

            return View(chkOut);
        }
      
       

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(CheckOutViewModel checkOutViewModel)
        {
            var ordercodetoken = string.Empty;
            string JsonStr = "";
            bool isSuccess = true;

            if (ModelState.IsValid)
            {
                Session["CheckOutViewModel"] = checkOutViewModel;
                return RedirectToAction("OrderSuccess", "CheckOut", new { paymentid = "" });
            }
            else
            {
                var error = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).FirstOrDefault();
                ModelState.AddModelError("", error);
            }

            


            return View(checkOutViewModel);
        }


        public ActionResult OrderSuccess(string paymentid)
        {
            var ordercodetoken = SaveOrder(paymentid);
            OrderViewModel vmorder = new OrderViewModel();

            var order = _OrderBusiness.GetListWT(c => c.TokenKey == ordercodetoken).FirstOrDefault();
            var orderedItem = _OrderedItemBusiness.GetListWT(c => c.OrderId == order.OrderId);
            var productList = _productBusiness.GetListWT();
            Mapper.CreateMap<OrderDetail, OrderViewModel>();
            vmorder = Mapper.Map<OrderDetail, OrderViewModel>(order);
            //showing cartdata to checkout
            var imgList = _ImageBusiness.GetListWT();
            var vmProductList = (from c in productList
                                 join oi in orderedItem
                                 on c.ProductID equals oi.ProductId
                                 select new CartWishlistViewModel
                                 {
                                     ProductID = c.ProductID,
                                     ProductCode = c.ProductCode,
                                     ProductName = oi.ProductName,
                                     TokenKey = c.TokenKey,
                                     ShortDescription = c.ShortDescription,
                                     Price = oi.Price ?? 0,
                                     quantity = oi.OrderQuantity,
                                     DiscountPercent = oi.DiscountPercent,
                                     DiscountedPrice = (Math.Round((oi.Price ?? 0) - (Decimal.Divide(oi.DiscountPercent ?? 0, 100) * oi.Price ?? 0)) * oi.OrderQuantity),
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
            vmorder.cartwishlist = vmProductList;

            #region print invoice
            //Define a ThermalLabel object and set unit to inch and label size
            ThermalLabel tLabel = new ThermalLabel(UnitType.Inch, 3, 2);
            tLabel.GapLength = 0.2;

            //Define a TextItem object 
            TextItem txt = new TextItem(0.1, 0.1, 2.8, 0.5, "Decreasing 50");
            //set counter step for decreasing by 1
            //txt.CounterStep = -1;
            //set font
           // txt.Font.Name = Neodynamic.SDK.Printing.Font.NativePrinterFontS;
            txt.Font.Unit = FontUnit.Point;

            //Define a BarcodeItem object
            // bc = new BarcodeItem(0.1, 0.57, 2.8, 1.3, BarcodeSymbology.Code128, "ABC01");
            //set counter step for increasing by 1
            //.CounterStep = 1;
            //set barcode size
            //bc.BarWidth = 0.02;
            //bc.BarHeight = 0.75;
            //set barcode alignment
            //bc.BarcodeAlignment = BarcodeAlignment.MiddleCenter;
            //set font
            //bc.Font.Name = Neodynamic.SDK.Printing.Font.NativePrinterFontA;
            //bc.Font.Unit = FontUnit.Point;
            //bc.Font.Size = 10;

            //Add items to ThermalLabel object...
            tLabel.Items.Add(txt);
            //tLabel.Items.Add(bc);

            //Create a WindowsPrintJob object
            using (WindowsPrintJob pj = new WindowsPrintJob())
            {
                //Create PrinterSettings object
                PrinterSettings myPrinter = new PrinterSettings();
                myPrinter.Communication.CommunicationType = CommunicationType.USB;
                myPrinter.Dpi = 203;
                myPrinter.ProgrammingLanguage = ProgrammingLanguage.ESCPOS;
                myPrinter.PrinterName = "POS-80";
              

                //Set PrinterSettings to PrintJob
                pj.PrinterSettings = myPrinter;
                //Set num of labels to generate
                pj.Copies = 1;
                //Print ThermalLabel object...
                pj.Print(tLabel);
            }
            #endregion
            return View(vmorder);
        }

        [NonAction]
        public string SaveOrder(string paymentid)
        {
            var ordertoken = string.Empty;
            var checkOutViewModel = (CheckOutViewModel)Session["CheckOutViewModel"];
            _unitOfWork.BeginTransaction();
            try
            {
                var productList = _productBusiness.GetListWT();
                OrderDetail order = new OrderDetail();
                order.TokenKey = GlobalMethods.GetToken();
                order.FirstName = checkOutViewModel.FirstNameShopper;
                order.LastName = checkOutViewModel.LastNameShopper;
                order.Email = checkOutViewModel.EmailShopper;
                order.Phone = checkOutViewModel.PhoneShopper;
                //order.MobilePhone = checkOutViewModel.MobilePhoneShopper;
                //order.Address = checkOutViewModel.AddressShopper;
                //order.Pincode = checkOutViewModel.PincodeShopper;
                //order.City = checkOutViewModel.CityShopper;
                order.PaymentType = checkOutViewModel.PaymentType;
                order.PaymentId = paymentid;
                //order.ShippingOrder = checkOutViewModel.ShippingOrder;
                order.OrderedBy = "Guest";
                order.OrderStatus = "New";
                order.OrderDate = DateTime.Now;
                order.OrderCode = "";
                _OrderBusiness.Insert(order);
                _unitOfWork.SaveChanges();

                order.OrderCode = "OC" + order.OrderId;
                _OrderBusiness.Update(order);
                _unitOfWork.SaveChanges();

                ordertoken = order.TokenKey;

                //generating message
                var smsmessage = "Thank you for your purchase. " + Environment.NewLine + "Your order code is:" + order.OrderCode + Environment.NewLine;
                var mailmessage = "Thank you for placing an order.<br>Your Order detail:<br>";
                mailmessage = mailmessage + "Order Code:" + order.OrderCode + "<br>";
                mailmessage = mailmessage + "Order Date:" + order.OrderDate.ToShortDateString() + "<br>";

                mailmessage = mailmessage + "Payment:" + order.PaymentType + "<br>";

                var itemdetailmessage = string.Empty;

                //Adding Order
                Decimal totalAmount = 0;
                OrderedItem ordItem = new OrderedItem();
                if (!string.IsNullOrEmpty(checkOutViewModel.OrderListJson))
                {
                    dynamic Obj = Newtonsoft.Json.JsonConvert.DeserializeObject(checkOutViewModel.OrderListJson);
                    foreach (var product in Obj)
                    {
                        var productId = Convert.ToInt32(product.ProductID.ToString());
                        var sizeId = Convert.ToInt32(product.SizeId.ToString());
                        var attributes = product.AttributeId.ToString();
                        var prdt = productList.Where(c => c.ProductID == productId).FirstOrDefault();
                        ordItem.OrderId = Convert.ToInt32(order.OrderId);
                        ordItem.ProductId = productId;
                        ordItem.OrderQuantity = product.Quantity;
                        ordItem.ProductName = prdt.ProductName;
                        ordItem.Price = _productBusiness.GetSelectedPrice(productId, sizeId, attributes);
                        ordItem.DiscountPercent = prdt.DiscountPercent;
                        ordItem.Size = Convert.ToInt32(product.SizeId);
                        ordItem.Attributes = product.AttributeId;
                        _OrderedItemBusiness.Insert(ordItem);
                        _unitOfWork.SaveChanges();
                        //formatting message
                        totalAmount = totalAmount + (Math.Round((ordItem.Price ?? 0) - (Decimal.Divide(ordItem.DiscountPercent ?? 0, 100) * ordItem.Price ?? 0)) * ordItem.OrderQuantity);
                        itemdetailmessage = itemdetailmessage + @"<tr><td>" + prdt.ProductCode + "</td><td>" + prdt.ProductName + "</td><td>NRs " + (Math.Round((ordItem.Price ?? 0) - (Decimal.Divide(ordItem.DiscountPercent ?? 0, 100) * ordItem.Price ?? 0))) + "</td><td>" + ordItem.OrderQuantity + "</td><td>NRs " + (Math.Round((ordItem.Price ?? 0) - (Decimal.Divide(ordItem.DiscountPercent ?? 0, 100) * ordItem.Price ?? 0)) * ordItem.OrderQuantity) + "</td></tr>";
                    }
                }

                smsmessage = smsmessage + "And total amount is:$ " + totalAmount + Environment.NewLine + Environment.NewLine;
                smsmessage = smsmessage + "For further information please check your email." + Environment.NewLine + Environment.NewLine + "Ecommerce";

                Setting contactUs = db.Settings.AsNoTracking().FirstOrDefault();

                CookieStore mycookie = new CookieStore();
                var expireCookieTimeHr = Convert.ToInt32(ReadConfigData.GetAppSettingsValue("ExpireCookieTimeHr"));
                mycookie.SetCookie(Enumerator.CustomerAction.AddToCart.ToString(), "", expireCookieTimeHr);

                _unitOfWork.Commit();

                Session.Remove("CheckOutViewModel");

                //sending order mail to customer                  
                string messageBody = @"<table style='border-collapse: collapse;' border='0' width='600' cellspacing='0' cellpadding='0' align='center'>
                                            <tbody>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>      
                                                <tr>
                                                    <td>
                                                        <table style='border-collapse: collapse;' border='0' width='560' cellspacing='0' cellpadding='0' align='center'>
                                                            <tbody>
                                                                <tr>
                                                                    <td style='font-family: Arial, Helvetica, sans-serif; font-size: 16px; line-height: 22px; color: #777777;'>Dear <strong>" + order.FirstName + @"</strong>,</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>&nbsp;</td>
                                                                </tr>
                                                                <tr>
                                                                    <td style='font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 22px; color: #777777;'>" + mailmessage + @"</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>&nbsp;</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Items Ordered</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <table>
                                                                            <thead>
                                                                                <tr>
                                                                                    <th>Product Code</th>
                                                                                    <th>Product Name</th>
                                                                                    <th>Price</th>
                                                                                    <th>Quantity</th>
                                                                                    <th>Total</th>
                                                                                </tr>
                                                                            </thead>
                                                                            <tbody>" + itemdetailmessage + @"</tbody>
                                                                            <tfoot>
                                                                                <tr>
                                                                                    <td colspan='4'>Subtotal</td>
                                                                                    <td>$ " + totalAmount + @"</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan='4'>Grand Total</td>
                                                                                    <td>$ " + totalAmount + @"</td>
                                                                                </tr>
                                                                            </tfoot>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>&nbsp;</td>
                                                                </tr>
                                                                <tr>
                                                                    <td style='font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 22px; color: #a1004b; font-weight: bold;'>Regards,<br />Call Us On: " + contactUs.Phone + @" </td> </tr> <tr> <td>&nbsp;</td></tr>  </tbody>     </table>      </td>   </tr>   </tbody>   </table>";
                string subj = "Order Product";
                string tomail = order.Email;

                //sending order mail to customer service                  
                var messageBodycustomerservice = @"<table style='border-collapse: collapse;' border='0' width='600' cellspacing='0' cellpadding='0' align='center'>
                                            <tbody>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>      
                                                <tr>
                                                    <td>
                                                        <table style='border-collapse: collapse;' border='0' width='560' cellspacing='0' cellpadding='0' align='center'>
                                                            <tbody>
                                                                <tr>
                                                                    <td style='font-family: Arial, Helvetica, sans-serif; font-size: 16px; line-height: 22px; color: #777777;'>Dear <strong>Customer Service</strong>,</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>&nbsp;</td>
                                                                </tr>
                                                                <tr>
                                                                    <td style='font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 22px; color: #777777;'>" + order.FirstName + " has placed order. His/Her order code is: <strong>" + order.OrderCode + @"</strong></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>&nbsp;</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Items Ordered</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <table>
                                                                            <thead>
                                                                                <tr>
                                                                                    <th>Product Code</th>
                                                                                    <th>Product Name</th>
                                                                                    <th>Price</th>
                                                                                    <th>Quantity</th>
                                                                                    <th>Total</th>
                                                                                </tr>
                                                                            </thead>
                                                                            <tbody>" + itemdetailmessage + @"</tbody>
                                                                            <tfoot>
                                                                                <tr>
                                                                                    <td colspan='4'>Subtotal</td>
                                                                                    <td>$ " + totalAmount + @"</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan='4'>Grand Total</td>
                                                                                    <td >$ " + totalAmount + @"</td>
                                                                                </tr>
                                                                            </tfoot>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style='font-family: Arial, Helvetica, sans-serif; font-size: 14px; line-height: 22px; color: #a1004b; font-weight: bold;'>With Regards,<br /></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>&nbsp;</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>";
                var subjcustomerservice = "Order Product";
                var tomailcustomerservice = ReadConfigData.GetAppSettingsValue("CustomerServiceEmail");

                //sending sms
                var mobileno = order.Phone;


                Task SendMailCustomer = Task.Factory.StartNew(() =>
                {
                    //sending mail to customer
                    MailManagement.SendEmail(subj, tomail, messageBody);
                    //sending message to customer service
                    MailManagement.SendEmail(subjcustomerservice, tomailcustomerservice, messageBodycustomerservice);
                    //send sms
                    //SendSMS.Send(mobileno, smsmessage);
                });
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
            }

            clearCart();
            return ordertoken;
        }


        public void clearCart()
        {
            var currentUserId = Convert.ToInt32(GlobalUser.getGlobalUser().UserId);
            if (currentUserId > 0)
            {
                var cartData = _AddToCartBusiness.GetList(c => c.UserId == currentUserId);
                foreach (var product in cartData)
                {
                    _AddToCartBusiness.Delete(product);
                    _unitOfWork.SaveChanges();
                }
            }
            else
            {
                CookieStore mycookie = new CookieStore();
                var expireCookieTimeHr = Convert.ToInt32(ReadConfigData.GetAppSettingsValue("ExpireCookieTimeHr"));
                mycookie.SetCookie(Enumerator.CustomerAction.AddToCart.ToString(), "", expireCookieTimeHr);
            }
        }


        public JsonResult UserIsBlocked()
        {
            return Json(new { msg = "true" });

        }



       

    }
}
