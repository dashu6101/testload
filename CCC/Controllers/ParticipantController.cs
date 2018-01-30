using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCC.Models;
using CCC.Services;

namespace CCC.Controllers
{
    [Authorize]
    public class ParticipantController : BaseController
    {
        //
        // GET: /Participant/

        private CCCService service = new CCCService();

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
            var allcategoryList = service.getAllCategoryList();
            ViewBag.IsChannelSelected = "0";
            return View(allcategoryList);

        }

        public ActionResult ChannelList()
        {
           
            //display all channels if no channel selected.
            var allcategoryList = service.getAllCategoryList();
            ViewBag.IsChannelSelected = "0";
            return View(allcategoryList);

        }

        [HttpGet]
        public ActionResult ChannelDetails(string id)
        {
            if (id != null)
            {
                ViewBag.CategoryId = id;
            }

            var model = service.getCategoryDetails(Convert.ToInt32(id), sUser.UserId);
            return View(model);
        }

        [HttpPost]
        public ActionResult JoinCategory(int CategoryId, bool HasJoinedCategory, Decimal Price)
        {
            string returnvalue = "";
            if (HasJoinedCategory)
            {
                returnvalue = service.RemoveCategory(CategoryId, sUser.UserId);
            }
            else
            {
                returnvalue = service.JoinCategory(CategoryId, sUser.UserId, Price);

            }
            return Json(returnvalue, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult ViewSummary(int CategoryId)
        {
             string returnvalue = "";

             returnvalue = service.GetCategorySummary(CategoryId, sUser.UserId);

            if (returnvalue != "-1")
            {
                return Json(returnvalue,JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = 500;
                return Json(returnvalue, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}
