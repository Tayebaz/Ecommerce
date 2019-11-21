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

using System.Text;
using PrinterUtility;
using System.Net.Mail;
using System.Net;

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

        PrinterUtility.EscPosEpsonCommands.EscPosEpson obj = new PrinterUtility.EscPosEpsonCommands.EscPosEpson();

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
            string[] WorkingHour = null;


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




            DateTime FromDateTime = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy") + " " + WorkingHour[0]);
            DateTime ToDateTime = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy") + " " + WorkingHour[1]);

            if (FromDateTime.ToString("tt").ToLower() == "pm" && ToDateTime.ToString("tt").ToLower() == "am")
                ToDateTime = ToDateTime.AddDays(1);


            double iWorkingHour = (ToDateTime - FromDateTime).TotalHours;
            double CurrentTotalHour = (DateTime.Now - FromDateTime).TotalHours;

            bool IsClosed = false;

            if( (CurrentTotalHour >= iWorkingHour) || (DateTime.Now < FromDateTime))
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
            Entities.Models.User CurrentUser = (Entities.Models.User)Session["CurrentUserInfo"];
            var currentUser = _userBusiness.GetUserByemail(CurrentUser.Email);
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


            PrintInvoice(vmorder);
            SendEmail(vmorder);

            return View(vmorder);
        }



        private void PrintInvoice(OrderViewModel  MyOrderModel)
        {
           
            var BytesValue = Encoding.ASCII.GetBytes(string.Empty);
            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Separator());
            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.CharSize.Nomarl());
            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.FontSelect.FontA());
            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Center());
            BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes(MyOrderModel.FirstName+ " " + MyOrderModel.LastName+"\n"));
            BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes(MyOrderModel.Email  + "\n"));
            BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes(MyOrderModel.Phone + "\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.CharSize.DoubleWidth4());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes(MyOrderModel.Phone));
            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.CharSize.Nomarl());
            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Separator());
            BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("Invoice\n"));
            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Left());
            BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("Order No. : "+ MyOrderModel.OrderId.ToString()+"\n"));
            BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("Date      : "+DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss")+"\n"));
            BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("Itm                                      Qty      Size   Attr\n"));
            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Separator());

            for (int index=0; index< MyOrderModel.cartwishlist.Count; index++)
            {
                
                BytesValue = PrintExtensions.AddBytes(BytesValue, string.Format("{0,-40}{1,6}{2,9}{3,9}\n", MyOrderModel.cartwishlist[index].ProductName, MyOrderModel.cartwishlist[index].quantity, MyOrderModel.cartwishlist[index].Size, MyOrderModel.cartwishlist[index].Attributes));
            }


            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Right());
            BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Separator());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("Total\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("288.00\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Separator());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Lf());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Center());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.CharSize.DoubleHeight6());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.BarCode.Code128("12345"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.QrCode.Print("12345", PrinterUtility.Enums.QrCodeSize.Grande));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, "-------------------Thank you for coming------------------------\n");
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Left());
            BytesValue = PrintExtensions.AddBytes(BytesValue, CutPage());
            PrinterUtility.PrintExtensions.Print(BytesValue, System.Configuration.ConfigurationManager.AppSettings["PrinterPath"].ToString());



        }

      
            private bool SendEmail(OrderViewModel MyOrderModel)
            {
                bool result = false;
                try
                {
                    if (ModelState.IsValid)
                    {
                    string FromEmail = System.Configuration.ConfigurationManager.AppSettings["FromEmail"];
                    string EmailPass = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"];
                    string DisplayName = System.Configuration.ConfigurationManager.AppSettings["FromEmailDisplayName"];

                        var senderEmail = new MailAddress(FromEmail, DisplayName);
                        var receiverEmail = new MailAddress(MyOrderModel.Email, MyOrderModel.FirstName);
                        var password = EmailPass;
                        var sub = "Your Order " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt");
                        var body = GetEmailBody(MyOrderModel);
                       
                       

                        var smtp = new SmtpClient
                        {
                            Host = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"],
                            Port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]),
                            EnableSsl = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpEnableSsl"]),
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = sub,
                            Body = body,
                            IsBodyHtml = true
                        })
                        {
                            smtp.Send(mess);
                        result= true;
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Some Error";
                result= false;
                }
            return result;

            }

        private string GetEmailBody(OrderViewModel MyOrderModel)
        {
           string  body = System.IO.File.ReadAllText(Server.MapPath("~/files/EmailTemplate.html"));
            body = body.Replace("#0#", MyOrderModel.OrderCode).Replace("#1#", MyOrderModel.OrderDate.ToString("MM-dd-yyyy hh:mm:ss tt")).Replace("#2#", MyOrderModel.OrderStatus).Replace("#3#", MyOrderModel.Email).Replace("#4#", MyOrderModel.FirstName + " " + MyOrderModel.LastName).Replace("#5#", MyOrderModel.Phone);


            string RowItm = "";
            double Total = 0;
            for (int i = 0; i< MyOrderModel.cartwishlist.Count; i++ )
            {
                RowItm += "<tr data-productid='9'>";
                RowItm += "<td data-title='Product Name'><b>" + MyOrderModel.cartwishlist[i].ProductName + "</b>";
                RowItm += "<ul>";
                RowItm += "<li>" + MyOrderModel.cartwishlist[i].ProductCode + "</li>";
                RowItm += "<li>Size: " + MyOrderModel.cartwishlist[i].Size + "</li>";
                RowItm += "<li>Attr: " + MyOrderModel.cartwishlist[i].Attributes + "</li>";
                RowItm += "</ul>";
                RowItm += "</td>";
                RowItm += " <td data-title='Price' >$ <span >" + MyOrderModel.cartwishlist[i].Price.ToString() + "</span></td>";
                RowItm += "<td data-title='Quantity' >"+ MyOrderModel.cartwishlist[i].quantity.ToString()+ "</td>";
                RowItm += "<td data-title='Total' >$ "+ (MyOrderModel.cartwishlist[i].Price  * MyOrderModel.cartwishlist[i].quantity).ToString() + "</td>";
                RowItm += "</tr>";
                
                Total += Convert.ToDouble(MyOrderModel.cartwishlist[i].Price) *  Convert.ToDouble(MyOrderModel.cartwishlist[i].quantity);

            }

            body = body.Replace("#6#", RowItm).Replace("#7#", Total.ToString()).Replace("#8#", Total.ToString());

            return body;
        }

         

        

        private byte[] CutPage()
        {
            List<byte> oby = new List<byte>();
            oby.Add(Convert.ToByte(Convert.ToChar(0x1D)));
            oby.Add(Convert.ToByte('V'));
            oby.Add((byte)66);
            oby.Add((byte)3);
            return oby.ToArray();
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
