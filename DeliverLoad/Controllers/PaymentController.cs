using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using DeliverLoad.Models;
using System.Configuration;
using System.Web.UI;
using System.Globalization;
using System.Text;
using System.Net;
using System.IO;
using System.Data;
using DeliverLoad.Filters;
using DeliverLoad.Services;
//using Mvc.Mailer;

using System.Net.Mail;
using DeliverLoad.Mvc.Mailers;
using Mvc.Mailer;

namespace DeliverLoad.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class PaymentController : BaseController
    {

        //
        // GET: /Payment/

        // GET: /Account/Login

        private IUserMailer _userMailer = new UserMailer();

        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }

        private DeliverLoadService service = new DeliverLoadService();

        private string business = ConfigurationManager.AppSettings["BusinessEmail"];
        private string currency_code = ConfigurationManager.AppSettings["CurrencyCode"];
        static DataSet requests = new DataSet();
        static DataSet responses = new DataSet();

        public ActionResult Index()
        {
            if (ConfigurationManager.AppSettings["UseSandbox"].ToString() == "true")
            {
                ViewBag.URL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
            else
            {
                ViewBag.URL = "https://www.paypal.com/cgi-bin/webscr";
            }

            //MasterPage pg = this.Master;
            //var form = (System.Web.UI.HtmlControls.HtmlForm)pg.FindControl("form1");
            //form.Action = ViewBag["URL"];

            if (ConfigurationManager.AppSettings["SendToReturnURL"].ToString() == "true")
            {
                ViewBag.rm = "2";
            }
            else
            {
                ViewBag.rm = "1";
            }

            //if (Convert.ToBoolean(Request.QueryString["IsSubcription"]) == true && Request.QueryString["option"].ToString() != "")
            //{
            //    item_name = "Channel Subscription";
            //    amount = Request.QueryString["option"].ToString();
            //    Session["IsSubcription"] = "1";
            //}
            //else
            //{

            //    item_name = "Reports";

            //    if (Request.QueryString["option"].ToString() == "1")
            //    {
            //        amount = "20";
            //    }
            //    else if (Request.QueryString["option"].ToString() == "2")
            //    {
            //        amount = "50";
            //    }
            //}

            //Session["paypal-post-url"] = ViewBag["URL"];


            List<PaymentHistoryModel> pay = service.getPaymentHistory(sUser.UserId);

            return View(pay);
        }

        #region
        public ActionResult PaymentResponse()
        {
            CultureInfo ci = new CultureInfo("en-us");
            string strFormValues = Encoding.ASCII.GetString(Request.BinaryRead(Request.ContentLength));
            dynamic strNewValue = null;

            // getting the URL to work with

            Session["paypal-post-url"] = null;

            string URL = null;
            if (ConfigurationManager.AppSettings["UseSandbox"].ToString() == "true")
            {
                URL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
            else
            {
                URL = "https://www.paypal.com/cgi-bin/webscr";
            }

            // Session["paypal-post-url"] = URL;

            // Create the request back
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);


            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            strNewValue = strFormValues + "&cmd=_notify-validate";
            req.ContentLength = strNewValue.Length;

            // Write the request back IPN strings
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            stOut.Write(strNewValue);
            stOut.Close();
            //send the request, read the response
            HttpWebResponse strResponse = (HttpWebResponse)req.GetResponse();
            Stream IPNResponseStream = strResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(IPNResponseStream, encode);

            Char[] read = new Char[257];
            // Reads 256 characters at a time.
            int count = readStream.Read(read, 0, 256);

            while (count > 0)
            {
                // Dumps the 256 characters to a string
                String IPNResponse = new String(read, 0, count);
                count = readStream.Read(read, 0, 256);
                //string amount = null;
                //try
                //{
                //    // getting the total cost of goods in the cart for a request identified stored in the "custom" variable
                //    amount = GetRequestPrice(Request["custom"].ToString());
                //    if (string.IsNullOrEmpty(amount))
                //    {
                //        //("Error in payment_success: amount = """)
                //        readStream.Close();
                //        strResponse.Close();
                //        return;
                //    }

                //}
                //catch (Exception ex)
                //{
                //    //("Error in payment_success: " + ex.Message)
                //    readStream.Close();
                //    strResponse.Close();
                //    return;
                //}


                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NumberGroupSeparator = ",";
                provider.NumberGroupSizes = new int[] { 3 };


                string txn_id = "", payment_id = "", email = "", first_name = "", last_name = "", street = "", city = "", state = "", zip = "", country = "", reason_fault = "";
                string request_id = "";
                decimal payment_price = 0;
                bool is_success = false;


                // if the request is verified
                if (IPNResponse == "VERIFIED")
                {
                    // check the receiver's e-mail (login is user's identifier in PayPal) and the transaction type
                    if (Request["receiver_email"] != business | Request["txn_type"] != "web_accept")
                    {
                        try
                        {
                            // parameters are not correct. Write a response from PayPal and create a record in the Log file.
                            service.CreatePaymentResponses(Request["txn_id"], Convert.ToDecimal(Request["mc_gross"], provider), Request["payer_email"], Request["first_name"], Request["last_name"], Request["address_street"], Request["address_city"], Request["address_state"], Request["address_zip"], Request["address_country"],
                            Convert.ToString(Request["custom"]), false, "INVALID paymetn's parameters (receiver_email or txn_type)", sUser.UserId, false);
                            //("Error in payment_success: INVALID paymetn's parameters (receiver_email or txn_type)")

                            //int userid = sUser.UserId;
                            //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();
                            return RedirectToAction("Index");

                        }
                        catch (Exception ex)
                        {
                            //("Error in payment_success: " + ex.Message)
                            readStream.Close();
                            strResponse.Close();
                            //int userid = sUser.UserId;
                            //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();
                            return RedirectToAction("Index");

                        }

                    }
                    // check whether this request was received earlier for its identifier 
                    if (service.IsDuplicateID(Request["txn_id"]))
                    {
                        // the current request has been already processed. Write a response from PayPal and create a record in the Log file.
                        service.CreatePaymentResponses(Request["txn_id"], Convert.ToDecimal(Request["mc_gross"], provider), Request["payer_email"], Request["first_name"], Request["last_name"], Request["address_street"], Request["address_city"], Request["address_state"], Request["address_zip"], Request["address_country"],
                       Convert.ToString(Request["custom"]), false, "Duplicate txn_id found", sUser.UserId, false);
                        //("Error in payment_success: Duplicate txn_id found")
                        readStream.Close();
                        strResponse.Close();
                        //int userid = sUser.UserId;
                        //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();
                        return RedirectToAction("Index");

                    }
                    // the amount of payment, the status of the payment, amd a possible reason of delay
                    // The fact that Getting txn_type=web_accept or txn_type=subscr_payment are got odes not mean that seller will receive the payment.
                    // That's why we check payment_status=completed. The single exception is when the seller's account in not American and pending_reason=intl
                    //if (Request["mc_gross"].ToString(ci) != amount | Request["mc_currency"] != currency_code | (Request["payment_status"] != "Completed" & Request["pending_reason"] != "intl"))
                    //{
                    //    // parameters are incorrect or the payment was delayed. A response from PayPal should not be written to DB of an XML file
                    //    // because it may lead to a failure of uniqueness check of the request identifier.
                    //    // Create a record in the Log file with information about the request.
                    //    //("Error in payment_success: INVALID paymetn's parameters. Request: " + strFormValues)
                    //    readStream.Close();
                    //    strResponse.Close();
                    //    return;
                    //}

                    try
                    {
                        // write a response from PayPal

                        txn_id = Request["txn_id"].ToString();
                        payment_id = "";
                        payment_price = Convert.ToDecimal(Request["mc_gross"], provider);
                        email = Request["payer_email"].ToString();


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["first_name"])))
                        {
                            first_name = Request["first_name"].ToString();

                        }
                        else
                        {
                            first_name = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["last_name"])))
                        {
                            last_name = Request["last_name"].ToString();
                        }
                        else
                        {
                            last_name = "";

                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_street"])))
                        {
                            street = Request["address_street"].ToString();

                        }
                        else
                        {
                            street = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_city"])))
                        {
                            city = Request["address_city"].ToString();

                        }
                        else
                        {
                            city = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_state"])))
                        {
                            state = Request["address_state"].ToString();

                        }
                        else
                        {
                            state = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_zip"])))
                        {

                            zip = Request["address_zip"].ToString();
                        }
                        else
                        {
                            zip = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_country"])))
                        {

                            country = Request["address_country"].ToString();
                        }
                        else
                        {
                            country = "";
                        }

                        request_id = Convert.ToString(Request["custom"]);
                        // request_id = 0;
                        is_success = true;
                        reason_fault = "Success";


                        service.CreatePaymentResponses(txn_id, payment_price, email, first_name, last_name, street, city, state, zip, country,
                   request_id, is_success, reason_fault, sUser.UserId, true);

                        // CreatePaymentResponses(Request["txn_id"].ToString(), Convert.ToDecimal(Request["mc_gross"], provider), Request["payer_email"].ToString(), Request["first_name"].ToString(), Request["last_name"].ToString(), Request["address_street"].ToString(), Request["address_city"].ToString(), Request["address_state"].ToString(), Request["address_zip"].ToString(), Request["address_country"].ToString(),
                        //Convert.ToInt32(Request["custom"]), true, "");
                        //("Success in payment_success: PaymentResponses created")
                        ///''''''''''
                        // Here we notify the person responsible for goods delivery that 
                        // the payment was performed and providing him with all needed information about
                        // the payment. Some flags informing that user paid for a services can be also set here.
                        // For example, if user paid for registartion on the site, then the flag should be set 
                        // allowing the user who paid to access the site
                        ///''''''''''
                        ///

                        //int userid = sUser.UserId;
                        //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();



                        //var mail = UserMailer.PaymentSucess(email, first_name, txn_id);
                        //mail.Subject = "Payment success";
                        //mail.To.Add(new MailAddress(email));

                        //var client = new SmtpClientWrapper();

                        //client.SendCompleted += (sender, e) =>
                        //{
                        //    if (e.Error != null || e.Cancelled)
                        //    {
                        //        //when error comes
                        //        //service.CatchMe(e.Error, "Register");
                        //        //service.DeleteHardUser(user);
                        //        errorfunction();

                        //    }
                        //};

                        // mail.SendAsync("async send", client);
                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Index");
                        // return null;
                        //("Error in payment_success: " + ex.Message)
                    }

                }
                else
                {

                    //   CreatePaymentResponses("1111", 15, "krunal.bihola@gmail.com", "dixit","Pratik", "street", "city", "state", "zip", "country","request_id is_success", false,"falut");

                    // parameters are not correct. Write a response from PayPal and create a record in the Log file.
                    service.CreatePaymentResponses(Request["txn_id"], Convert.ToDecimal(Request["mc_gross"], provider), Request["payer_email"], Request["first_name"], Request["last_name"], Request["address_street"], Request["address_city"], Request["address_state"], Request["address_zip"], Request["address_country"],
                    Convert.ToString(Request["custom"]), false, "INVALID paymetn's parameters (receiver_email or txn_type)", sUser.UserId, false);

                    //int userid = sUser.UserId;
                    //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();
                    return RedirectToAction("Index");

                }
            }

            return RedirectToAction("Index");

        }

        public ActionResult AccountConfirmation()
        {
            CultureInfo ci = new CultureInfo("en-us");
            string strFormValues = Encoding.ASCII.GetString(Request.BinaryRead(Request.ContentLength));
            dynamic strNewValue = null;

            // getting the URL to work with

            Session["paypal-post-url"] = null;

            string URL = null;
            if (ConfigurationManager.AppSettings["UseSandbox"].ToString() == "true")
            {
                URL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
            else
            {
                URL = "https://www.paypal.com/cgi-bin/webscr";
            }

            // Session["paypal-post-url"] = URL;

            // Create the request back
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);


            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            strNewValue = strFormValues + "&cmd=_notify-validate";
            req.ContentLength = strNewValue.Length;

            // Write the request back IPN strings
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            stOut.Write(strNewValue);
            stOut.Close();
            //send the request, read the response
            HttpWebResponse strResponse = (HttpWebResponse)req.GetResponse();
            Stream IPNResponseStream = strResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(IPNResponseStream, encode);

            Char[] read = new Char[257];
            // Reads 256 characters at a time.
            int count = readStream.Read(read, 0, 256);

            while (count > 0)
            {
                // Dumps the 256 characters to a string
                String IPNResponse = new String(read, 0, count);
                count = readStream.Read(read, 0, 256);
                //string amount = null;
                //try
                //{
                //    // getting the total cost of goods in the cart for a request identified stored in the "custom" variable
                //    amount = GetRequestPrice(Request["custom"].ToString());
                //    if (string.IsNullOrEmpty(amount))
                //    {
                //        //("Error in payment_success: amount = """)
                //        readStream.Close();
                //        strResponse.Close();
                //        return;
                //    }

                //}
                //catch (Exception ex)
                //{
                //    //("Error in payment_success: " + ex.Message)
                //    readStream.Close();
                //    strResponse.Close();
                //    return;
                //}


                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NumberGroupSeparator = ",";
                provider.NumberGroupSizes = new int[] { 3 };


                string txn_id = "", payment_id = "", email = "", first_name = "", last_name = "", street = "", city = "", state = "", zip = "", country = "", reason_fault = "";
                string request_id = "";
                decimal payment_price = 0;
                bool is_success = false;


                // if the request is verified
                if (IPNResponse == "VERIFIED")
                {
                    // check the receiver's e-mail (login is user's identifier in PayPal) and the transaction type
                    if (Request["receiver_email"] != business | Request["txn_type"] != "web_accept")
                    {
                        try
                        {
                            // parameters are not correct. Write a response from PayPal and create a record in the Log file.
                            service.CreatePaymentResponses(Request["txn_id"], Convert.ToDecimal(Request["mc_gross"], provider), Request["payer_email"], Request["first_name"], Request["last_name"], Request["address_street"], Request["address_city"], Request["address_state"], Request["address_zip"], Request["address_country"],
                            Convert.ToString(Request["custom"]), false, "INVALID paymetn's parameters (receiver_email or txn_type)", sUser.UserId, false);
                            //("Error in payment_success: INVALID paymetn's parameters (receiver_email or txn_type)")

                            //int userid = sUser.UserId;
                            //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();

                            return RedirectToAction("Create", "Presenter");

                        }
                        catch (Exception ex)
                        {
                            //("Error in payment_success: " + ex.Message)
                            readStream.Close();
                            strResponse.Close();
                            //int userid = sUser.UserId;
                            //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();
                            return RedirectToAction("Create", "Presenter");

                        }

                    }
                    // check whether this request was received earlier for its identifier 
                    if (service.IsDuplicateID(Request["txn_id"]))
                    {
                        // the current request has been already processed. Write a response from PayPal and create a record in the Log file.
                        service.CreatePaymentResponses(Request["txn_id"], Convert.ToDecimal(Request["mc_gross"], provider), Request["payer_email"], Request["first_name"], Request["last_name"], Request["address_street"], Request["address_city"], Request["address_state"], Request["address_zip"], Request["address_country"],
                       Convert.ToString(Request["custom"]), false, "Duplicate txn_id found", sUser.UserId, false);
                        //("Error in payment_success: Duplicate txn_id found")
                        readStream.Close();
                        strResponse.Close();
                        //int userid = sUser.UserId;
                        //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();

                        return RedirectToAction("Create", "Presenter");


                    }
                    // the amount of payment, the status of the payment, amd a possible reason of delay
                    // The fact that Getting txn_type=web_accept or txn_type=subscr_payment are got odes not mean that seller will receive the payment.
                    // That's why we check payment_status=completed. The single exception is when the seller's account in not American and pending_reason=intl
                    //if (Request["mc_gross"].ToString(ci) != amount | Request["mc_currency"] != currency_code | (Request["payment_status"] != "Completed" & Request["pending_reason"] != "intl"))
                    //{
                    //    // parameters are incorrect or the payment was delayed. A response from PayPal should not be written to DB of an XML file
                    //    // because it may lead to a failure of uniqueness check of the request identifier.
                    //    // Create a record in the Log file with information about the request.
                    //    //("Error in payment_success: INVALID paymetn's parameters. Request: " + strFormValues)
                    //    readStream.Close();
                    //    strResponse.Close();
                    //    return;
                    //}

                    try
                    {
                        // write a response from PayPal

                        txn_id = Request["txn_id"].ToString();
                        payment_id = "";
                        payment_price = Convert.ToDecimal(Request["mc_gross"], provider);
                        email = Request["payer_email"].ToString();


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["first_name"])))
                        {
                            first_name = Request["first_name"].ToString();

                        }
                        else
                        {
                            first_name = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["last_name"])))
                        {
                            last_name = Request["last_name"].ToString();
                        }
                        else
                        {
                            last_name = "";

                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_street"])))
                        {
                            street = Request["address_street"].ToString();

                        }
                        else
                        {
                            street = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_city"])))
                        {
                            city = Request["address_city"].ToString();

                        }
                        else
                        {
                            city = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_state"])))
                        {
                            state = Request["address_state"].ToString();

                        }
                        else
                        {
                            state = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_zip"])))
                        {

                            zip = Request["address_zip"].ToString();
                        }
                        else
                        {
                            zip = "";
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(Request["address_country"])))
                        {

                            country = Request["address_country"].ToString();
                        }
                        else
                        {
                            country = "";
                        }

                        request_id = Convert.ToString(Request["custom"]);
                        // request_id = 0;
                        is_success = true;
                        reason_fault = "Success";


                        service.CreatePaymentResponses(txn_id, payment_price, email, first_name, last_name, street, city, state, zip, country,
                   request_id, is_success, reason_fault, sUser.UserId, true);

                        // CreatePaymentResponses(Request["txn_id"].ToString(), Convert.ToDecimal(Request["mc_gross"], provider), Request["payer_email"].ToString(), Request["first_name"].ToString(), Request["last_name"].ToString(), Request["address_street"].ToString(), Request["address_city"].ToString(), Request["address_state"].ToString(), Request["address_zip"].ToString(), Request["address_country"].ToString(),
                        //Convert.ToInt32(Request["custom"]), true, "");
                        //("Success in payment_success: PaymentResponses created")
                        ///''''''''''
                        // Here we notify the person responsible for goods delivery that 
                        // the payment was performed and providing him with all needed information about
                        // the payment. Some flags informing that user paid for a services can be also set here.
                        // For example, if user paid for registartion on the site, then the flag should be set 
                        // allowing the user who paid to access the site
                        ///''''''''''
                        ///

                        //int userid = sUser.UserId;
                        //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();



                        //var mail = UserMailer.PaymentSucess(email, first_name, txn_id);
                        //mail.Subject = "Payment success";
                        //mail.To.Add(new MailAddress(email));

                        //var client = new SmtpClientWrapper();

                        //client.SendCompleted += (sender, e) =>
                        //{
                        //    if (e.Error != null || e.Cancelled)
                        //    {
                        //        //when error comes
                        //        //service.CatchMe(e.Error, "Register");
                        //        //service.DeleteHardUser(user);
                        //        errorfunction();

                        //    }
                        //};

                        // mail.SendAsync("async send", client);
                        return RedirectToAction("Create", "Presenter");

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Create", "Presenter");
                        // return null;
                        //("Error in payment_success: " + ex.Message)
                    }

                }
                else
                {

                    //   CreatePaymentResponses("1111", 15, "krunal.bihola@gmail.com", "dixit","Pratik", "street", "city", "state", "zip", "country","request_id is_success", false,"falut");

                    // parameters are not correct. Write a response from PayPal and create a record in the Log file.
                    service.CreatePaymentResponses(Request["txn_id"], Convert.ToDecimal(Request["mc_gross"], provider), Request["payer_email"], Request["first_name"], Request["last_name"], Request["address_street"], Request["address_city"], Request["address_state"], Request["address_zip"], Request["address_country"],
                    Convert.ToString(Request["custom"]), false, "INVALID paymetn's parameters (receiver_email or txn_type)", sUser.UserId, false);

                    //int userid = sUser.UserId;
                    //List<PaymentHistory> pay = DB.PaymentHistories.Where(x => x.UserID == userid).ToList();
                    return RedirectToAction("Create", "Presenter");

                }
            }

            return RedirectToAction("Create", "Presenter");

        }



        #endregion


        public ActionResult SubscriptionDetails()
        {
            var model = service.getJoinedCategoryList(sUser.UserId);
            return View(model);
        }

        public ActionResult PaymentSummury(string CategoryId)
        {
            PaymentMonitoryModel objPaymentMonitoryModel = new PaymentMonitoryModel();
            try
            {
                if (CategoryId != null && Convert.ToInt32(CategoryId) > 0)
                {
                    //commented because change in order summury
                    //OrderSummuryModel objOrderSummury = service.getOrderSummuryByCategoryId(Convert.ToInt32(CategoryId));

                    OrderSummuryModel objOrderSummury = new OrderSummuryModel();
                    ViewBag.Price = objOrderSummury.LoadDetail.LoadPrice;

                    if (objOrderSummury != null)
                    {
                        ViewBag.OrderSummury = objOrderSummury;
                        objPaymentMonitoryModel = service.getPaymentMonitoryByUserId(sUser.UserId);
                    }
                    else
                    {
                        ViewBag.OrderSummury = null;
                        objPaymentMonitoryModel = new PaymentMonitoryModel();
                    }
                }
                else
                {
                    ViewBag.OrderSummury = null;
                }
                ViewBag.CategoryId = CategoryId;

                return View("PaymentSummury", objPaymentMonitoryModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpPost]
        public ActionResult ProceedCategory(int CategoryId, decimal Price)
        {
            string ReturnValue = "";

            try
            {
                ReturnValue = service.ProceedCategory(CategoryId, sUser.UserId, Price);
                if (ReturnValue != "-1")
                {
                    //commented because change in order summury
                    //OrderSummuryModel objOrderSummury = service.getOrderSummuryByCategoryId(CategoryId);

                    OrderSummuryModel objOrderSummury = new OrderSummuryModel();
                    if (objOrderSummury != null)
                    {
                        if (objOrderSummury.LoadOwnerDetail.LoadOwner.UserId > 0 && objOrderSummury.LoadOwnerDetail.LoadOwner.EmailID != null)
                        {
                            SendPaymentSuccessEmailToLoadOwner(objOrderSummury, ReturnValue);
                        }
                        if (objOrderSummury.VehicleOwnerDetail.VehicleOwner.UserId > 0 && objOrderSummury.VehicleOwnerDetail.VehicleOwner.EmailID != null)
                        {
                            SendNewOrderEmailToVehicleOwner(objOrderSummury);
                        }
                    }
                }
                return Json(ReturnValue, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public void SendPaymentSuccessEmailToLoadOwner(OrderSummuryModel objOrderSummury, string code)
        {

            //For local test
            string url = "http://localhost:2018/Payment/PaymentSummury?CategoryId=197";
            decimal extraCharge = 5;

            var mail = UserMailer.PaymentSuccess(objOrderSummury.LoadOwnerDetail.LoadOwner.FirstName + " " +objOrderSummury.LoadOwnerDetail.LoadOwner.LastName, objOrderSummury.LoadDetail.LoadName, Convert.ToInt64(objOrderSummury.LoadDetail.LoadPrice), extraCharge, DateTime.UtcNow.ToString(), code, url);
            mail.Subject = "Thank you for your payment for " + objOrderSummury.LoadDetail.LoadName + " of $" + objOrderSummury.LoadDetail.LoadPrice + extraCharge;
            mail.To.Add(new MailAddress(objOrderSummury.LoadOwnerDetail.LoadOwner.EmailID));

            var client = new SmtpClientWrapper();

            client.SendCompleted += (sender, e) =>
            {
                if (e.Error != null || e.Cancelled)
                {
                     
                }
            };

            mail.SendAsync("async send", client);
        }

        public void SendNewOrderEmailToVehicleOwner(OrderSummuryModel objOrderSummury) {

            //For local test
            string url = "http://localhost:2018/Payment/PaymentSummury?CategoryId=197";
            decimal extraCharge = 5;

            var mail = UserMailer.NewOrderNotification(objOrderSummury.VehicleOwnerDetail.VehicleOwner.FirstName +" " + objOrderSummury.VehicleOwnerDetail.VehicleOwner.LastName, objOrderSummury.LoadOwnerDetail.LoadOwner.FirstName + " "+ objOrderSummury.LoadOwnerDetail.LoadOwner.LastName, objOrderSummury.LoadDetail.LoadName, Convert.ToInt64(objOrderSummury.LoadDetail.LoadPrice) + extraCharge, DateTime.UtcNow.ToString(), url);
            mail.Subject = "New order has been placed for " + objOrderSummury.LoadDetail.LoadName + " of $" + objOrderSummury.LoadDetail.LoadPrice + extraCharge;
            mail.To.Add(new MailAddress(objOrderSummury.VehicleOwnerDetail.VehicleOwner.EmailID));

            var client = new SmtpClientWrapper();

            client.SendCompleted += (sender, e) =>
            {
                if (e.Error != null || e.Cancelled)
                {

                }
            };

            mail.SendAsync("async send", client);
        }

        public ActionResult PaymentConfirmation(string OrderId)
        {
            ViewBag.OrderId = OrderId;
            return View();

        }
    }
}