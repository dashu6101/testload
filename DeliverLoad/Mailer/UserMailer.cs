using Mvc.Mailer;

namespace DeliverLoad.Mvc.Mailers
{


    public class UserMailer : MailerBase, IUserMailer
    {
        public UserMailer()
        {
            MasterName = "_Layout";
        }

        public virtual MvcMailMessage ForgotPassword(string name, string password, string SiteUrl)
        {

            ViewBag.Name = name;

            ViewBag.Password = password;


            ViewBag.SiteUrl = SiteUrl;

            return Populate(x =>
                {
                    x.ViewName = "ForgotPassword";
                    x.From = new System.Net.Mail.MailAddress("Deliverload Admin<no-reply@deliverload.com>");
                });
        }

        public virtual MvcMailMessage ContactUs(string email, string name, string message)
        {

            ViewBag.Name = name;

            ViewBag.Email = email;

            ViewBag.Message = message;
            //ViewBag.VerifyUrl = verifyUrl;

            return Populate(x =>
            {
                x.ViewName = "ContactUs";
                x.From = new System.Net.Mail.MailAddress("DeliverLoad Admin<no-reply@deliverload.com>");
            });
        }

        public virtual MvcMailMessage ResetPassword(string email, string token, string verifyUrl)
        {
            ViewBag.Title = "Steps to reset your account...";

            ViewBag.Name = email;
            ViewBag.VerifyUrl = verifyUrl;
            ViewBag.Token = token;

            return Populate(x =>
            {
                x.ViewName = "ResetPassword";
                x.From = new System.Net.Mail.MailAddress("Scisis Team <no-reply@Scisis.com>");
            });
        }

        public virtual MvcMailMessage NewContentNotification(string userName, string channelName, string PresenterName, string Title, string ChannelNumber)
        {
            ViewBag.Name = userName;
            ViewBag.ChannelName = channelName;
            ViewBag.PresenterName = PresenterName;
            ViewBag.Title = Title;
            ViewBag.ChannelNumber = ChannelNumber;

            return Populate(x =>
            {
                x.ViewName = "NewContentNotification";
                x.From = new System.Net.Mail.MailAddress("Deliverload Admin<no-reply@deliverload.com>");
            });
        }

        public virtual MvcMailMessage ConfirmAccount(string userName, string token)
        {
            ViewBag.Name = userName;
            ViewBag.ConfirmLink = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString() + "/account/activate?token=" + token;

            return Populate(x =>
            {
                x.ViewName = "ConfirmAccount";
                x.From = new System.Net.Mail.MailAddress("Deliverload Admin<no-reply@deliverload.com>");
            });
        }

        public virtual MvcMailMessage SendInvite(string inviterName, string channelName,string ChannelNo)
        {
            ViewBag.Name = inviterName;
            ViewBag.ChannelNo = ChannelNo;
            ViewBag.ChannelName = channelName;

            ViewBag.Link = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();

            return Populate(x =>
            {
                x.ViewName = "SendInvite";
                x.From = new System.Net.Mail.MailAddress("Deliverload Admin<no-reply@deliverload.com>");
            });
        }

        public virtual MvcMailMessage NewCommentNotification(string Name, string ChannelNo, string ChannelName)
        {
            ViewBag.Name = Name;
            ViewBag.ChannelNumber = ChannelNo;
            ViewBag.ChannelName = ChannelName;

            return Populate(x =>
            {
                x.ViewName = "NewCommentNotification";
                x.From = new System.Net.Mail.MailAddress("Deliverload Admin<no-reply@deliverload.com>");
            });
        }

        public virtual MvcMailMessage AzureVideoUploadNotification(string Name, int VideoStatus, string ChannelName)
        {
            //ViewBag.Name = Name;
            //ViewBag.VideoStatus = VideoStatus;
            //ViewBag.ChannelName = ChannelName;


         


            ViewBag.Name = "chirag";

            ViewBag.Password = "admin@123";


            ViewBag.SiteUrl = "www.google.com";


            return Populate(x =>
            {
                x.ViewName = "ForgotPassword";
                x.From = new System.Net.Mail.MailAddress("Deliverload Admin<no-reply@deliverload.com>");
            });

            //return Populate(x =>
            //{
            //    x.ViewName = "AzureVideoUploadNotifiction";
            //    x.From = new System.Net.Mail.MailAddress("deliverload Admin<no-reply@deliverload.com>");
            //});

        }

    }
}