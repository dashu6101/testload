using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeliverLoad.Models;

namespace DeliverLoad.Services
{
    public partial class DeliverLoadService
    {

        public UserModel GetUserDetails(string LoginUserName)
        {
            var userDetails = dbContext.Users.Where(x => x.UserName == LoginUserName).Select(x => new UserModel { UserId = x.UserId, UserName = x.UserName, UserType = x.UserType, ScreenName = x.ScreenName, Password = x.Password, ChannelNo = x.ChannelNo, FirstName = x.FirstName, LastName = x.LastName, IsBloked = x.isSuspended == null ? false : (bool)x.isSuspended }).FirstOrDefault();

            return userDetails;

        }

        public UserModel GetUserDetailsByUserId(int UserId)
        {
            var userDetails = dbContext.Users.Where(x => x.UserId == UserId).Select(x => new UserModel { UserId = x.UserId, UserName = x.UserName, UserType = x.UserType, FirstName = x.FirstName, LastName = x.LastName, ScreenName = x.ScreenName, ProfilePicture = x.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + x.ProfilePicture, Balance = (decimal)x.Balance }).FirstOrDefault();

            return userDetails;

        }

        public IEnumerable<UserModel> GetUserDetailsByChanneNo(string ChannelNo)
        {
            var userDetails = dbContext.Users.Where(x => x.ChannelNo.Contains(ChannelNo) && x.UserType == "A").Select(x => new UserModel { UserId = x.UserId, UserName = x.UserName, UserType = x.UserType, FirstName = x.FirstName, LastName = x.LastName, ScreenName = x.ScreenName, ProfilePicture = x.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + x.ProfilePicture, ChannelNo = x.ChannelNo, JoinDate = (DateTime)x.RegistrationDate }).ToList();

            return userDetails;

        }


        public string ProfileEdit(int UserId, string ScreenName, string FirstName, string LastName, string UserType, HttpPostedFileBase imageUpload)
        {
            try
            {
                User objUser = dbContext.Users.Where(x => x.UserId == UserId).SingleOrDefault();
                objUser.FirstName = FirstName;
                objUser.LastName = LastName;
                objUser.ScreenName = ScreenName;
                objUser.UserType = UserType;
                objUser.LastModified = DateTime.Now;
                if (imageUpload != null)
                {

                    string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/images/profilepicture/" + imageUpload.FileName);

                    imageUpload.SaveAs(physicalPath);
                    objUser.ProfilePicture = imageUpload.FileName;

                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return UserType;
        }

        public int GetMaxChannelNo()
        {
            try
            {
                int maxNo = 0;

                var channelNo = dbContext.Users.Max(x => x.UserId);

                maxNo = Convert.ToInt32(channelNo) + 1;

                return maxNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public string DeductBalance(int UserId, decimal Balance)
        {
            try
            {
                User objUser = dbContext.Users.Where(x => x.UserId == UserId).SingleOrDefault();

                objUser.Balance = objUser.Balance - Balance;

                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "1";

        }

        public string SaveSubscriber(LoginModel model)
        {
            try
            {
                Subscription objSub = new Subscription();
                objSub.EmailId = model.SubscriptionMailId;
                dbContext.Subscriptions.Add(objSub);
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "1";
        }

    }
}