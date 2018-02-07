﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliverLoad.Models;
using DeliverLoad.Services;
using System.Configuration;

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


        [HttpGet]
        public ActionResult AddNewLoad()
        {

            //if (ConfigurationManager.AppSettings["UseSandbox"].ToString() == "true")
            //{
            //    ViewBag.URL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            //}
            //else
            //{
            //    ViewBag.URL = "https://www.paypal.com/cgi-bin/webscr";
            //}

            //if (ConfigurationManager.AppSettings["SendToReturnURL"].ToString() == "true")
            //{
            //    ViewBag.rm = "2";
            //}
            //else
            //{
            //    ViewBag.rm = "1";
            //}
            CategoryModel model = new CategoryModel();
            //model.ParticipantCategories
            //var model = service.getOwnerCategoryDetails(CategoryId, sUser.UserId);
            model.LoadSpaceList = service.getLoadSpaceList();
           // ViewBag.UserIdAuthenticate = service.UserIsAuthenticated(sUser.UserId);
           // ViewBag.Title = "Edit Channel";
            return View(model);
            //return PartialView("_CreateOrEditCategory", model);
        }


        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewPostLoad(CategoryModel model)
        {
            if (model.CategoryId == 0)
            {
                //create category
                string returnvalue = service.CreateOverLoadCategory(model, sUser.UserId, sUser.ChannelNo);

            }
            //else
            //{
            //    //update category
            //    string returnvalue = service.EditCategoryOwner(model);

            //    Session["CategoryId"] = model.CategoryId;

            //}


            return RedirectToAction("Index");
        }
    }
}
