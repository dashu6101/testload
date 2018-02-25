using DeliverLoad.Models;
using DeliverLoad.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeliverLoad.Controllers
{
    [Authorize]
    public class OrderSummuryController : Controller
    {
        //
        // GET: /OrderSummury/

        private DeliverLoadService service = new DeliverLoadService();

        public ActionResult OrderSummury(string CategoryId, string VehicleOwnerId, string LoadOwnerId, string LoadPrice = "")
        {
            OrderSummuryModel objOrderSummury = new OrderSummuryModel();
            try
            {
                if (CategoryId != null && Convert.ToInt32(CategoryId) > 0 && VehicleOwnerId != null && Convert.ToInt32(VehicleOwnerId) > 0 && LoadOwnerId != null && Convert.ToInt32(LoadOwnerId) > 0)
                {
                    objOrderSummury = service.getOrderSummuryByCategoryId(Convert.ToInt32(CategoryId), Convert.ToInt32(VehicleOwnerId), Convert.ToInt32(LoadOwnerId));
                }
                if (objOrderSummury != null)
                {
                    objOrderSummury.PaymentSummury.merchantId = ConfigurationManager.AppSettings["merchantId"];
                    objOrderSummury.PaymentSummury.serviceTypeId = ConfigurationManager.AppSettings["serviceTypeId"];
                    objOrderSummury.PaymentSummury.apiKey = ConfigurationManager.AppSettings["apiKey"];
                    objOrderSummury.PaymentSummury.responseurl = ConfigurationManager.AppSettings["ResponseUrl"];
                    objOrderSummury.PaymentSummury.gatewayUrl = ConfigurationManager.AppSettings["GatewayUrl"];
                    objOrderSummury.PaymentSummury.checkStatusUrl = ConfigurationManager.AppSettings["CheckSatusUrl"];

                    if (objOrderSummury.LoadOwnerDetail.LoadOwner.FirstName != null && objOrderSummury.LoadOwnerDetail.LoadOwner.FirstName != "")
                    {
                        objOrderSummury.PaymentSummury.payerName = objOrderSummury.LoadOwnerDetail.LoadOwner.FirstName;
                    }
                    else {
                        objOrderSummury.PaymentSummury.payerName = "Test";
                    }

                    if (objOrderSummury.LoadOwnerDetail.LoadOwner.EmailID != null && objOrderSummury.LoadOwnerDetail.LoadOwner.EmailID != "")
                    {
                        objOrderSummury.PaymentSummury.payerEmail = objOrderSummury.LoadOwnerDetail.LoadOwner.EmailID;
                    }
                    else
                    {
                        objOrderSummury.PaymentSummury.payerName = "test@test.com";
                    }
                    if (objOrderSummury.LoadDetail.LoadPrice > 0)
                    {
                        objOrderSummury.PaymentSummury.amt = Convert.ToString(objOrderSummury.LoadDetail.LoadPrice);
                    }
                    else {
                        objOrderSummury.PaymentSummury.amt = "1000";
                    }
                    
                    objOrderSummury.PaymentSummury.payerPhone = "123456789";
                    objOrderSummury.PaymentSummury.orderId = objOrderSummury.LoadDetail.LoadId.ToString();
                    

                    long milliseconds = DateTime.Now.Ticks;
                    string order_Id = milliseconds.ToString();
                    string hash_string = objOrderSummury.PaymentSummury.merchantId + objOrderSummury.PaymentSummury.serviceTypeId + objOrderSummury.LoadDetail.LoadId.ToString() + objOrderSummury.PaymentSummury.amt + objOrderSummury.PaymentSummury.responseurl + objOrderSummury.PaymentSummury.apiKey;
                    System.Security.Cryptography.SHA512Managed sha512 = new System.Security.Cryptography.SHA512Managed();
                    Byte[] EncryptedSHA512 = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash_string));
                    sha512.Clear();
                    objOrderSummury.PaymentSummury.hash = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();
                }
                return View("OrderSummury", objOrderSummury);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}
