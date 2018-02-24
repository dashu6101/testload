using System;
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
            //var model = service.getJoinedOverLoadCategoryList(sUser.UserId);

            ////display selected channel list.
            //if (model.Any())
            //{
            //    ViewBag.IsChannelSelected = "1";
            //    return View(model);

            //}

            //display all channels if no channel selected.
            var allcategoryList = service.getAllLoadownerCategoryList(sUser.UserId);
            ViewBag.IsChannelSelected = "0";
            return View(allcategoryList);

        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(FindSpaceViewModel searchVM)
        {
            try
            {
                if (sUser.UserType == "A")
                {
                    return RedirectToAction("Index", "Vehicleowner", searchVM);
                }
                if (searchVM == null)
                {
                    return RedirectToAction("Index", "FindSpace");
                }
                var model = service.getLoadownerCategoryListBySearch(searchVM);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "FindSpace");
            }
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
                string returnvalue = service.CreateOverLoadCategory(model, sUser.UserId, sUser.ChannelNo, "Load");

            }
            //else
            //{
            //    //update category
            //    string returnvalue = service.EditCategoryOwner(model);

            //    Session["CategoryId"] = model.CategoryId;

            //}


            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult JoinCategory(int CategoryId, decimal Price)
        {
            string ReturnValue = "";

            try
            {
                ReturnValue = service.JoinOverLoadCategory(CategoryId, sUser.UserId, Price);
                return Json(ReturnValue, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult MyWatchList()
        {
            var model = service.getJoinedOverLoadCategoryList(sUser.UserId);

            //display selected channel list.

            ViewBag.IsChannelSelected = "1";
            return View(model);
        }

        public ActionResult MyLoads() {
            try
            {
                ViewBag.LoadDetail = service.GetLoadListByLoadOwnerUserId(sUser.UserId);
                return View();
            }
            catch(Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetVehicleOwnersOffersByLoadId(string CategoryId) {
            try
            {
                List<AcceptedLoadOffers> acceptedLoadOfferList = service.GetVehicleOwnersOffersByLoadId(Convert.ToInt32(CategoryId), sUser.UserId);
                return Json(acceptedLoadOfferList);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            
        }

        [HttpPost]
        public ActionResult MyLoadForLoadOwner(List<AcceptedLoadOffers> acceptedLoadOfferList)
        {
            try
            {
                if (acceptedLoadOfferList == null)
                {
                    acceptedLoadOfferList = new List<AcceptedLoadOffers>();
                }
                else
                {
                    foreach (var item in acceptedLoadOfferList)
                    {
                        if (item.LoadId > 0)
                        {
                            OrderSummuryModel objOrderSummury = service.getOrderSummuryByCategoryId(item.LoadId);
                            if (objOrderSummury.CategoryId > 0 && objOrderSummury.LoadOwnerId > 0)
                            {
                                item.orderSummuryModel.LoadName = objOrderSummury.LoadName;
                                item.orderSummuryModel.LoadDesc = objOrderSummury.LoadDesc;
                                item.orderSummuryModel.LoadCreatedDate = objOrderSummury.LoadCreatedDate;
                                item.orderSummuryModel.LoadImage = objOrderSummury.LoadImage;
                                item.orderSummuryModel.LoadPrice = objOrderSummury.LoadPrice;
                                item.orderSummuryModel.LoadPickupDate = objOrderSummury.LoadPickupDate;
                                item.orderSummuryModel.LoadDropOffDate = objOrderSummury.LoadDropOffDate;
                                item.orderSummuryModel.LoadPickupLocation = objOrderSummury.LoadPickupLocation;
                                item.orderSummuryModel.LoadDropOffLocation = objOrderSummury.LoadDropOffLocation;
                                item.orderSummuryModel.VehicleOwnerFirstName = objOrderSummury.VehicleOwnerFirstName;
                                item.orderSummuryModel.VehicleOwnerMiddleName = objOrderSummury.VehicleOwnerMiddleName;
                                item.orderSummuryModel.VehicleOwnerEmail = objOrderSummury.VehicleOwnerEmail;
                                item.orderSummuryModel.VehicleOwnerGender = objOrderSummury.VehicleOwnerGender;
                                item.orderSummuryModel.VehicleOwnerProfileImage = objOrderSummury.VehicleOwnerProfileImage;
                            }
                        }

                    }
                }
                return PartialView("_MyLoadForLoadOwner", acceptedLoadOfferList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPost]
        public JsonResult ConfirmVehicleOwnerOfferByLoadOwner(string categoryId, string vehicleOwnerId)
        {
            try
            {
                bool isSuccess = false;
                if (Convert.ToInt32(vehicleOwnerId) > 0)
                {
                    isSuccess = service.ConfirmVehicleOwnerOfferByLoadOwner(Convert.ToInt32(categoryId), Convert.ToInt32(vehicleOwnerId), sUser.UserId);
                }
                return Json(isSuccess);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult CancelConfirmVehicleOwnerOffer(string categoryId, string vehicleOwnerId)
        {
            try
            {
                bool isSuccess = false;
                if (Convert.ToInt32(vehicleOwnerId) > 0)
                {
                    isSuccess = service.CancelConfirmVehicleOwnerOffer(Convert.ToInt32(categoryId), Convert.ToInt32(vehicleOwnerId), sUser.UserId);
                }
                return Json(isSuccess);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
