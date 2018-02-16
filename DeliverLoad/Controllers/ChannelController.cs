using DeliverLoad.Filters;
using DeliverLoad.Models;
using DeliverLoad.Mvc.Mailers;
using DeliverLoad.Services;
using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DeliverLoad.Controllers
{

    public class ChannelController : BaseController
    {
        private DeliverLoadService service = new DeliverLoadService();

        private IUserMailer _userMailer = new UserMailer();

        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }

        [AllowAnonymous]
        public ActionResult Content(int NodeId, string UserName, string ChannelName)
        {
            PresenterContentLinkViewModel model = null;
            model = service.GetChannelContentLinkDetail(NodeId, UserName, ChannelName);

            if (model != null)
            {
                if (model.IsChannelAvailable == true)
                {
                    ViewBag.ImagePath = model.ImageName;
                    ViewBag.PageTitle = "Channel " + model.ChannelNo.Trim() + " '" + model.Title.Trim() + "' by " + model.CategoryPresenterName.Trim();
                    ViewBag.description = Regex.Replace(model.Description, @"<[^>]*>", String.Empty);
                    ViewBag.ChapterStatus = "true";

                    return View("PresenterChannelLinkView", model);
                }
                else
                {
                    ViewBag.ChapterStatus = "false";
                    ViewBag.description = "";
                    ViewBag.PageTitle = "";
                    ViewBag.ImagePath = "";
                    PresenterContentLinkViewModel Blankmodel = new PresenterContentLinkViewModel();
                    return View("PresenterChannelLinkView", Blankmodel);
                }
            }
            else
            {
                ViewBag.ChapterStatus = "false";
                ViewBag.description = "";
                ViewBag.PageTitle = "";
                ViewBag.ImagePath = "";
                ViewBag.ChapterStatus = "false";
                return View("PresenterChannelLinkView", model = new PresenterContentLinkViewModel());

            }
        }

        [AllowAnonymous]
        public ActionResult LogOffPreview(string returnurl)
        {
            WebSecurity.Logout();
            Session["sUser"] = null;
            Session.Abandon();
            ViewBag.ReturnUrl = returnurl;
            return Redirect(returnurl);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(PresenterContentLinkViewModel model1, string returnurl)
        {

            var model = model1.LoginModel;

            if (ModelState.IsValid && model.FirstName != null && model.LastName != null && model.UserName != null && model.Password != null)
            {
                // if (ModelState.IsValid)
                //  {
                // Attempt to register the user
                try
                {
                    var UserInfo = new
                    {

                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Password = model.Password,
                        UserType = "M",
                        Balance = 2,
                        RegistrationDate = DateTime.Now,
                        ScreenName = model.FirstName,
                        LastModified = DateTime.Now,
                        MeetingAvailability = true,
                        ChannelNo = service.GetMaxChannelNo(),
                        EmailID = model.UserName

                    };

                    var confirmationToken = WebSecurity.CreateUserAndAccount(model.UserName, model.Password, UserInfo, true);

                    var mail = UserMailer.ConfirmAccount(model.FirstName, confirmationToken);
                    mail.Subject = "DeliverLoad account confirmation";
                    mail.To.Add(new MailAddress(model.UserName));

                    var client = new SmtpClientWrapper();
                    mail.SendAsync("async send", client);

                    var md = new LoginModel
                    {
                        Message = "RegisterSuccess"


                    };
                    TempData["GritterTitle"] = "success";
                    TempData["GritterMessage"] = "You are successfully registered,check your mail!";
                    return Redirect(returnurl);
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", "A user account with the same email id already exists.You need to enter a different email id");
                    TempData["GritterTitle"] = "error";
                    TempData["GritterMessage"] = "A user account with the same email id already exists.You need to enter a different email id";
                    
                    return Redirect(returnurl);
                }
                // }
            }
            else
            {
                ModelState.AddModelError("", "All fields are required.");
                 TempData["GritterTitle"] = "error";
                 TempData["GritterMessage"] = "All fields are required.";
                return Redirect(returnurl);

            }
            // If we got this far, something failed, redisplay form
            
        }
    }
}
