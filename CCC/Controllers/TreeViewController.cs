using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CCC.Services;
using CCC.Models;
using CCC.Mvc.Mailers;
using System.Net.Mail;
using System.Configuration;
using Mvc.Mailer;
using System.IO;

namespace CCC.Controllers
{
    [Authorize]
    public class TreeViewController : BaseController
    {
        //
        // GET: /TreeView/
        private CCCService service = new CCCService();

        private IUserMailer _userMailer = new UserMailer();

        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }

        [HttpGet]
        public ActionResult ContentDetails(string id)
        {
            var treeView = service.GetTreeVeiwList(Convert.ToInt32(id));
            ViewBag.CategoryId = id;
            return View(treeView);
        }

        //[HttpGet]
        //public ActionResult RefreshTreeView(int CategoryId)
        //{
        //    var treeView = service.GetTreeVeiwList(CategoryId);

        //    return PartialView("_ChildNode",treeView);
        //}

        public ActionResult GetCategoryContentDetails(int CategoryId)
        {
            var treeView = service.GetTreeVeiwList(CategoryId);
            return PartialView("_TreeView", treeView);
        }

        public ActionResult ContentDetails(int NodeId)
        {
            string returnvalue = service.ContentDetails(sUser.UserId, NodeId);
            return Json(returnvalue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContentDescription(int NodeId)
        {
            var model = service.GetCategoryDescription(sUser.UserId, NodeId);
            ViewBag.CurrentUser = sUser.UserId;
            return View("_ContentComment", model);
        }

        [ValidateInput(false)]
        //  [ValidateAntiForgeryToken]
        public ActionResult CreateOrEditContent(int NodeId, string Title, string Description, HttpPostedFileBase File, string VideoName,string VideoFrom)
        {

            string[] tokens;
            var finalstring = "";

            if (File != null)
            {
                tokens = File.FileName.Split('.');
                finalstring = Guid.NewGuid().ToString() + "." + tokens[tokens.Length - 1];
            }

            string returnvalue = service.EditTreeNode(NodeId, Title, Description, finalstring, VideoName,VideoFrom);

            if (returnvalue == "2")
            {

                if (!string.IsNullOrEmpty(finalstring))
                {
                    var filepath = Path.Combine(Server.MapPath("~/UploadedImages/"), finalstring);
                    File.SaveAs(filepath);
                }
            }

            return Json(returnvalue, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult DeleteChannel(int ChannelId)
        {
            int returnvalue = service.DeleteChannel(ChannelId);
            return Json(returnvalue.ToString(), JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult SaveTreeNode(int CategoryId, int ParentId, string Name, string Description, HttpPostedFileBase File, string VedioLink,string VideoFrom,bool IsTreeView)
        {

            string[] tokens;
            var finalstring = "";

            if (File != null)
            {
                tokens = File.FileName.Split('.');
                finalstring = Guid.NewGuid().ToString() + "." + tokens[tokens.Length - 1];
            }

            string returnvalue = service.CreateChildNode(CategoryId, ParentId, Name, sUser.UserId, Description, finalstring, VedioLink,VideoFrom);
            //string returnvalue = "325";
            int numChannel;
            if (int.TryParse(returnvalue, out numChannel))
            {
                if (!string.IsNullOrEmpty(finalstring))
                {
                    var filepath = Path.Combine(Server.MapPath("~/UploadedImages/"), finalstring);
                    File.SaveAs(filepath);
                }

                var channel = service.getCategoryDetails(CategoryId, sUser.UserId);
                var participants = service.getParticipantsList(CategoryId);

                // send mail to all subscribers

                foreach (var item in participants)
                {
                    string channelNumber = service.GetChannelNumberByCategoryId(CategoryId);
                    var mail = UserMailer.NewContentNotification(item.FirstName, channel.Name, sUser.UserName, Name, channelNumber);
                    mail.Subject = "New Content from ChitChatChannel";
                    mail.To.Add(new MailAddress(item.UserName));

                    var client = new SmtpClientWrapper();
                    mail.SendAsync("async send", client);

                }

                if(IsTreeView == false)
                {
                    return Json(returnvalue, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(returnvalue, JsonRequestBehavior.AllowGet);
            }

            
        }

        public ActionResult EditTreeNode(int CategoryId, int ParentId, string Name, string imageName, string VideoName, string VideoFrom ,bool IsTreeView)
        {
            string Description = "";
            string returnvalue = service.EditTreeNode(ParentId, Name, Description, imageName, VideoName, VideoFrom);

            return Json(returnvalue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTreeNode(int NodeId)
        {

            string returnvalue = service.DeleteTreeNode(NodeId, sUser.UserId);

            return Json(returnvalue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveComment(int NodeId, string Comment)
        {
            TreeViewNodeVM model = null;
            string returnvalue = service.SaveComments(NodeId, Comment, sUser.UserId, sUser.UserType);

            if (returnvalue == "1")
            {
                List<CategoryCommentDetailUsers> objCategory = new List<CategoryCommentDetailUsers>();
                objCategory = service.GetCategoryCommentDetailUsers(NodeId, sUser.UserId);
                if (objCategory.Count > 0)
                {
                    var mail = UserMailer.NewCommentNotification(sUser.UserName.ToUpper(), objCategory.Select(x => x.ChannelNo).Distinct().FirstOrDefault(), objCategory.Select(x => x.ContentName).Distinct().FirstOrDefault());
                    mail.Subject = "New Comment from ChitChatChannel";

                    foreach (var item in objCategory.Select(x => x.UserEmail).Distinct())
                    {
                        mail.Bcc.Add(new MailAddress(item));

                    }
                    var client = new SmtpClientWrapper();
                    mail.SendAsync("async send", client);
                }
                model = service.GetCategoryDescription(sUser.UserId, NodeId);
            }

            return View("_ContentComment", model);
        }

        public ActionResult SaveCommentPresenter(int NodeId, string Comment)
        {

            IEnumerable<ContentCommentModel> model = new List<ContentCommentModel>();
            string returnvalue = service.SaveComments(NodeId, Comment, sUser.UserId, sUser.UserType);

            if (returnvalue == "1")
            {
                List<CategoryCommentDetailUsers> objCategory = new List<CategoryCommentDetailUsers>();
                objCategory = service.GetCategoryCommentDetailPresenter(NodeId, sUser.UserId);
                if (objCategory.Count > 0)
                {
                    var mail = UserMailer.NewCommentNotification(sUser.UserName.ToUpper(), objCategory.Select(x => x.ChannelNo).Distinct().FirstOrDefault(), objCategory.Select(x => x.ContentName).Distinct().FirstOrDefault());
                    mail.Subject = "New Comment from ChitChatChannel";

                    foreach (var item in objCategory.Select(x => x.UserEmail).Distinct())
                    {
                        mail.Bcc.Add(new MailAddress(item));
                    }
                    var client = new SmtpClientWrapper();
                    mail.SendAsync("async send", client);
                }
                model = service.getCommentList(NodeId);

            }
            return View("_PresenterContentComments", model);
        }

        public ActionResult TreeNodeComments(int NodeId)
        {
            return View("_PresenterContentComments", service.getCommentList(NodeId));
        }
    }
}
