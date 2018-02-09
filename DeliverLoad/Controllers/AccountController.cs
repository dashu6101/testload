using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using DeliverLoad.Filters;
using DeliverLoad.Models;
using DeliverLoad.Services;
using Mvc.Mailer;
using DeliverLoad.Mvc.Mailers;
using System.Net.Mail;
using System.Configuration;
using DeliverLoad.Utils;

namespace DeliverLoad.Controllers
{
    [InitializeSimpleMembership]
    public class AccountController : BaseController
    {
        //
        // GET: /Account/Login

        private IUserMailer _userMailer = new UserMailer();

        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }

        private DeliverLoadService service = new DeliverLoadService();        

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {           
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return RedirectToAction("Index");
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && model.LoginUserName != null && model.LoginPassword != null)
            {

                if (WebSecurity.Login(model.LoginUserName, model.LoginPassword, persistCookie: model.RememberMe))
                {
                    var UserDetails = service.GetUserDetails(model.LoginUserName);
                    if (UserDetails.IsBloked==true)
                    {
                        ModelState.AddModelError("", "You account is blocked. Please contact support@chitchatchannel.com");
                        WebSecurity.Logout();
                        return View("Index", model);
                    }
                    else
                    {
                        // return RedirectToLocal(returnUrl);
                        if (UserDetails.UserType == "A")
                        {
                          return RedirectToAction("Index", "Presenter");
                           //return RedirectToAction("Index", "Vehicleowner");
                        }
                        else
                        {
                            //return RedirectToAction("Index", "Participant");

                           return RedirectToAction("Index", "Loadowner");
                        }
                    }
                    
                }

                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "User name or password provided is incorrect.");
            }
            else
            {
                ModelState.AddModelError("", "User name and password are required.");
            }

            return View("Index", model);
        }

        // Ajax Login
        [HttpPost]
        public JsonResult AjaxLogin(LoginModel model, string returnUrl)
        {
            AjaxResponse ajaxResponse = new AjaxResponse();
            ajaxResponse.Success = false;
            if (ModelState.IsValid && model.LoginUserName != null && model.LoginPassword != null)
            {

                if (WebSecurity.Login(model.LoginUserName, model.LoginPassword, persistCookie: model.RememberMe))
                {
                    var UserDetails = service.GetUserDetails(model.LoginUserName);
                    if (UserDetails.IsBloked == true)
                    {
                        ModelState.AddModelError("", "You account is blocked. Please contact support@chitchatchannel.com");
                        WebSecurity.Logout();
                        ajaxResponse.Message = "You account is blocked. Please contact support@deliverload.com";
                    }
                    else
                    {
                        ajaxResponse.Success = true;
                        // return RedirectToLocal(returnUrl);
                        if (UserDetails.UserType == "A")
                        {
                            //return RedirectToAction("Index", "Presenter");
                            ajaxResponse.Data = new { RedirectUrl = Url.Action("Index", "Vehicleowner") }; 
                        }
                        else
                        {
                            //return RedirectToAction("Index", "Participant");
                            ajaxResponse.Data = new { RedirectUrl = Url.Action("Index", "Loadowner") };
                        }
                    }

                }
                else
                {
                    ajaxResponse.Message = "User name and password are incorrect !!";
                }
            }
            else
            {
                ajaxResponse.Message = "User name and password are required.";
            }
            return this.Json(ajaxResponse);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginPreview(PresenterContentLinkViewModel model, string returnurl)
        {
            if (ModelState.IsValid && model.LoginModel.LoginUserName != null && model.LoginModel.LoginPassword != null)
            {
                if (WebSecurity.Login(model.LoginModel.LoginUserName, model.LoginModel.LoginPassword, persistCookie: model.LoginModel.RememberMe))
                {
                    var UserDetails = service.GetUserDetails(model.LoginModel.LoginUserName);
                    if (UserDetails.IsBloked == true)
                    {
                        ModelState.AddModelError("", "You are Bloked, Please contact support@chitchatchannel.com!");
                        WebSecurity.Logout();
                        return Redirect(returnurl);
                    }
                    return Redirect(returnurl);
                }
                ModelState.AddModelError("", "User name or password provided is incorrect.");
            }
            else
            {
                ModelState.AddModelError("", "User name and password are required.");
            }
            return RedirectToAction(returnurl);
        }
        //
        // POST: /Account/LogOff

       // [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            Session["sUser"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Account");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return RedirectToAction("Index");
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(LoginModel model)
        {
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
                    mail.Subject = "ChitChatChannel account confirmation";
                    mail.To.Add(new MailAddress(model.UserName));

                    var client = new SmtpClientWrapper();
                    mail.SendAsync("async send", client);

                    var md = new LoginModel
                    {
                        Message = "RegisterSuccess"
                    };

                    return View("Login", md);
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", "A user account with the same email id already exists.You need to enter a different email id");
                    //ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));

                }
                // }
            }
            else
            {
                ModelState.AddModelError("", "All fields are required.");
            }
            // If we got this far, something failed, redisplay form
            return View("Index", model);
        }

        [AllowAnonymous]
        public ActionResult Activate(string token)
        {
            //Activates a pending membership account.            
            if (WebSecurity.ConfirmAccount(token))
            {
                var model = new LoginModel
                {
                    Message = "ConfirmSuccess"
                };

                return View("Login", model);
            }
            else
            {
                return View("Login");
            }

        }

        //
        // GET: /Account/ExternalLoginFailure     

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var UserDetail = service.GetUserDetails(model.UserName);

                if (UserDetail != null)
                {

                    var mail = UserMailer.ForgotPassword(UserDetail.ScreenName, UserDetail.Password.ToString(), ConfigurationManager.AppSettings["SiteUrl"].ToString());
                    mail.Subject = "Forgot Password";
                    mail.To.Add(new MailAddress(model.UserName));

                    var client = new SmtpClientWrapper();

                    client.SendCompleted += (sender, e) =>
                    {
                        if (e.Error != null || e.Cancelled)
                        {
                            //when error comes
                            //service.CatchMe(e.Error, "Register");
                            //service.DeleteHardUser(user);
                            //errorfunction();

                        }
                    };

                    mail.SendAsync("async send", client);
                }
                else
                {
                    Response.StatusCode = 404;
                    return View(model);
                    //return Json("A user account with this email id does not exists.You need to enter a different email id", JsonRequestBehavior.AllowGet);
                }



            }
            return Json("1", JsonRequestBehavior.AllowGet);
            // return View("Index");
        }

        [HttpPost]
        [AllowAnonymous]

        public ActionResult Subscription(LoginModel model)
        {
            if (model.SubscriptionMailId == null)
            {
                Response.StatusCode = 500;
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
            else
            {
                string returnvalue = service.SaveSubscriber(model);
                if (returnvalue == "1")
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("1", JsonRequestBehavior.AllowGet);


        }


        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }


        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
