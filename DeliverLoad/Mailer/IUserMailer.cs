using Mvc.Mailer;

namespace DeliverLoad.Mvc.Mailers
{
    public interface IUserMailer
    {
        MvcMailMessage ForgotPassword(string name, string password, string SiteUrl);

        MvcMailMessage ContactUs(string email,string name,string message);
        
        MvcMailMessage ResetPassword(string email,string token, string verifyUrl);

        MvcMailMessage NewContentNotification(string userName, string channelName, string PresenterName, string Title, string ChannelNumber);

        MvcMailMessage ConfirmAccount(string userName,string token);

        MvcMailMessage SendInvite(string inviterName, string channelName, string ChannelNo);

        MvcMailMessage NewCommentNotification(string Name, string ChannelNo, string ChannelName);

        MvcMailMessage AzureVideoUploadNotification(string Name, int VideoStatus, string ChannelName);

        MvcMailMessage PaymentSuccess(string userName, string orderName, decimal orderPrice, decimal extraCharge, string payementDate, string code, string additionalInfoUrl);

        MvcMailMessage NewOrderNotification(string userName, string loadOwnerName, string loadName, decimal price, string payementDate, string sendReminderUrl);
    }
}