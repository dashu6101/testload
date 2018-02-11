using DeliverLoad.Models;
using DeliverLoad.Services;
using System;
using System.Collections.Generic;
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

        public ActionResult OrderSummury(string CategoryId)
        {
            OrderSummury objOrderSummury = new OrderSummury();
            try
            {
                if (CategoryId != null && Convert.ToInt32(CategoryId) > 0)
                {
                    objOrderSummury = service.getOrderSummuryByCategoryId(Convert.ToInt32(CategoryId));
                }
                return View("OrderSummury", objOrderSummury);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}
