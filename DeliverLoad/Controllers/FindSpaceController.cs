using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliverLoad.Models;

namespace DeliverLoad.Controllers
{
    public class FindSpaceController : Controller
    {
        //
        // GET: /FindSpace/

        public ActionResult Index()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult Index(FindSpaceViewModel searchVM)
        //{
        //    return RedirectToAction("Index")
        //}
    }
}
