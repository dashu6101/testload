using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeliverLoad.Services;
using DeliverLoad.Models;
using WebMatrix.WebData;
using Mvc.Mailer;
using DeliverLoad.Mvc.Mailers;
using System.Net.Mail;
using System.Configuration;
using DeliverLoad.Utils;
using System.IO;

namespace DeliverLoad.Controllers
{
    public class HomeController : BaseController
    {
        private IUserMailer _userMailer = new UserMailer();

        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }

        private DeliverLoadService service = new DeliverLoadService();

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }



        public ActionResult ProfileEdit()
        {

            var model = service.GetUserDetailsByUserId(sUser.UserId);
            return View(model);
        }

        [HttpPost]
        public ActionResult ProfileEdit(int UserId, string ScreenName, string FirstName, string LastName, string UserType, HttpPostedFileBase imageUpload)
        {

            string returnvalue = service.ProfileEdit(UserId, ScreenName, FirstName, LastName, UserType, imageUpload);

            // assing session b'caz user update profile then require some field like usertype,image etc..
            Session["sUser"] = DeliverLoad.Utils.Utils.GetDeliverLoadUser(WebSecurity.CurrentUserName);
            return Json(returnvalue, JsonRequestBehavior.AllowGet);
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {

            string subject = "Video encoding successful";
            string body = "";            

            body += "<p>Dear  " + "aditya" + ",</p>";
            body += "<p>Your video has been encoded and ready for viewing.</p>";
            body += "<p>Login to take a <a href=\"http://www.deliverload.com/\">look</a></p>";
            body += "<p>Sincerely,</p><p>DeliverLoad Admin.</p>";
            body += "<p>-------------------------------------------------<br/>This e-mail and any files transmitted with it may contain privileged or confidential information. It is solely for use by the individual for whom it is intended, even if addressed incorrectly. If you received this e-mail in error, please notify the sender; do not disclose, copy, distribute, or take any action in reliance on the contents of this information; and delete it from your system. Any other use of this e-mail is prohibited. Thank you for your compliance.";
            body += "--------------------------------------------------</p>";

            var msg = new MailMessage("support@deliverload.com", "aditya.murthy88@gmail.com");
            msg.Subject = subject;
            msg.Body = body;

            msg.IsBodyHtml = true;

            var client = new SmtpClient();
            client.Send(msg);

            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactModel model)
        {
            if (ModelState.IsValid)
            {

                var mail = UserMailer.ContactUs(model.UserName, model.FirstName, model.Message);
                mail.Subject = "DeliverLoad Enquiries";
                mail.To.Add(new MailAddress(ConfigurationManager.AppSettings["ContactUSEmail"].ToString()));

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
                Response.StatusCode = 500;
                return Json("-1", JsonRequestBehavior.AllowGet);
            }
            return Json("1", JsonRequestBehavior.AllowGet);
            //return View("Index");
        }

        public ActionResult Faq()
        {

            return View();
        }


        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult UploadImage(HttpPostedFileBase plupload, string CKEditorFuncNum, string CKEditor, string langCode)
        //{
        //    try
        //    {
        //        string url;
        //        var req = Request.Params;
        //        var files = Request.Files[0];
        //        var img = req["name"].ToString();

        //        for (int i = 0; i < Request.Params.Count; i++)
        //        {
        //            var a = req[i].ToString();
        //        }

        //        var filepath = "";
        //        string finalstring = "";
        //        if (img != null && img.Length > 0)
        //        {
        //            string[] tokens = img.Split('.');
        //            finalstring = Guid.NewGuid().ToString() + "." + tokens[tokens.Length - 1];
        //            filepath = Path.Combine(Server.MapPath("~/UploadedImages/"), finalstring);
        //            files.SaveAs(filepath);
        //        }

        //        string path = "/UploadedImages/" + finalstring;
        //        url = Request.Url.GetLeftPart(UriPartial.Authority) + "/" + path;
        //        return Content(url);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message);
        //    }

        //}


        public ActionResult UploadImage()
        {
            string url;
            var req = Request.Params;
            var files = Request.Files[0];
            //var img = req["name"].ToString();
            var img = files.FileName;

            //for (int i = 0; i < Request.Params.Count; i++)
            //{
            //    var a = req[i].ToString();
            //}

            var filepath = "";
            string finalstring = "";
            if (img != null && img.Length > 0)
            {
                string[] tokens = img.Split('.');
                finalstring = Guid.NewGuid().ToString() + "." + tokens[tokens.Length - 1];
                filepath = Path.Combine(Server.MapPath("~/UploadedImages/"), finalstring);
                files.SaveAs(filepath);
            }

         

            string path = "UploadedImages/" + finalstring;
            url = Request.Url.GetLeftPart(UriPartial.Authority) + "/" + path;

            return Json(new { filelink = url });
           
        }

        public JsonResult SendInvites(string emailId, string channelName, string ChannelNo)
        {

            var user = service.GetUserDetails(WebSecurity.CurrentUserName);

            foreach (var item in emailId.Split(','))
            {
                var mail = UserMailer.SendInvite(user.FirstName.ToUpper() + " " + user.LastName.ToUpper(), channelName, ChannelNo);
                mail.Subject = "DeliverLoad Invitation";
                mail.To.Add(new MailAddress(item));

                var client = new SmtpClientWrapper();
                mail.SendAsync("async send", client);
            }

            return Json("1", JsonRequestBehavior.AllowGet);
        }

      
    }
}
