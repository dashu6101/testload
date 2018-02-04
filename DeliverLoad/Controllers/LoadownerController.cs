using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliverLoad.Models;
using DeliverLoad.Services;

namespace DeliverLoad.Controllers
{
    [Authorize]
    public class LoadownerController : BaseController
    {
        //
        // GET: /Participant/

        private DeliverLoadService service = new DeliverLoadService();

        public ActionResult Index()
        {
            var model = service.getJoinedCategoryList(sUser.UserId);

            //display selected channel list.
            if (model.Any())
            {
                ViewBag.IsChannelSelected = "1";
                return View(model);
               
            }

            //display all channels if no channel selected.
            var allcategoryList = service.getAllLoadownerCategoryList();
            ViewBag.IsChannelSelected = "0";
            return View(allcategoryList);

        }

     
    }
}
