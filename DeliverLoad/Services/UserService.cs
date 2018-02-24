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
            var userDetails = dbContext.Users.Where(x => x.UserId == UserId).Select(x => new UserModel { UserId = x.UserId, UserName = x.UserName, UserType = x.UserType, FirstName = x.FirstName, LastName = x.LastName, ScreenName = x.ScreenName, ProfilePicture = x.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + x.ProfilePicture, Balance = (decimal)x.Balance, EmailID = x.EmailID, Mobile = x.Mobile, IsPhoneVerified = x.IsPhoneVerified, IsEmailVerified = x.IsEmailVerified, IsDriverLicenseVerified = x.IsDriverLicenseVerified, IsVIOVerified = x.IsVIOVerified, IsAnyIdVerified = x.IsAnyIdVerified }).FirstOrDefault();

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


        public string DocumentVerification(int UserId, string UserType, HttpPostedFileBase AnyId, HttpPostedFileBase VOI, HttpPostedFileBase DriverLicense)
        {
            try
            {
                UserDocument objUserDoc = dbContext.UserDocuments.Where(x => x.UserId == UserId).SingleOrDefault();

                if (AnyId != null)
                {
                    UserDocument objUserAnyId = dbContext.UserDocuments.Where(x => x.UserId == UserId && x.DocTypeId == 3).SingleOrDefault();

                    if (objUserAnyId != null)
                    {
                        objUserAnyId.IsActiveDocument = "N";
                        dbContext.SaveChanges();
                    }

                    string FileAnyId = Guid.NewGuid().ToString();
                    FileAnyId = FileAnyId + AnyId.FileName.Substring(AnyId.FileName.LastIndexOf('.'));
                    string physicalPath = System.IO.Path.Combine(
                                  System.Web.HttpContext.Current.Server.MapPath("~/UserDocuments/"), FileAnyId);


                    AnyId.SaveAs(physicalPath);


                    UserDocument objD = new UserDocument();
                    objD.DocumentImage = FileAnyId;
                    objD.DocTypeId = 3;
                    objD.UserId = UserId;
                    objD.IsActiveDocument = "Y";

                    dbContext.UserDocuments.Add(objD);
                    dbContext.SaveChanges();

                }

                if (VOI != null)
                {
                    UserDocument objUserVOI = dbContext.UserDocuments.Where(x => x.UserId == UserId && x.DocTypeId == 2).SingleOrDefault();

                    if (objUserVOI != null)
                    {
                        objUserVOI.IsActiveDocument = "N";
                        dbContext.SaveChanges();
                    }


                    string FileVOI = Guid.NewGuid().ToString();
                    FileVOI = FileVOI + VOI.FileName.Substring(VOI.FileName.LastIndexOf('.'));
                    string physicalPath = System.IO.Path.Combine(
                                  System.Web.HttpContext.Current.Server.MapPath("~/UserDocuments/"), FileVOI);


                    VOI.SaveAs(physicalPath);

                    UserDocument objDoc = new UserDocument();
                    objDoc.DocumentImage = FileVOI;
                    objDoc.DocTypeId = 2;
                    objDoc.UserId = UserId;
                    objDoc.IsActiveDocument = "Y";

                    dbContext.UserDocuments.Add(objDoc);
                    dbContext.SaveChanges();

                }

                if (DriverLicense != null)
                {

                    UserDocument objUserDriverLicense = dbContext.UserDocuments.Where(x => x.UserId == UserId && x.DocTypeId == 1).SingleOrDefault();

                    if (objUserDriverLicense != null)
                    {
                        objUserDriverLicense.IsActiveDocument = "N";
                        dbContext.SaveChanges();
                    }


                    string FileDriverLicense = Guid.NewGuid().ToString();
                    FileDriverLicense = FileDriverLicense + DriverLicense.FileName.Substring(DriverLicense.FileName.LastIndexOf('.'));
                    string physicalPath = System.IO.Path.Combine(
                                  System.Web.HttpContext.Current.Server.MapPath("~/UserDocuments/"), FileDriverLicense);


                    DriverLicense.SaveAs(physicalPath);


                    UserDocument objDoc1 = new UserDocument();
                    objDoc1.DocumentImage = FileDriverLicense;
                    objDoc1.DocTypeId = 1;
                    objDoc1.UserId = UserId;
                    objDoc1.IsActiveDocument = "Y";

                    dbContext.UserDocuments.Add(objDoc1);
                    dbContext.SaveChanges();
                }
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

        public void UpdateLogin(int UserId)
        {
            var userDetails = dbContext.Users.Where(x => x.UserId == UserId).FirstOrDefault();
            userDetails.LastOnline = DateTime.Now;
            dbContext.SaveChanges();


        }

        public PhoneVerificationViewModel AddPhoneVerificationAttempts(PhoneVerificationViewModel objPVA)
        {
            PhoneVerificationAttempt userPVA = dbContext.PhoneVerificationAttempts.Where(l => l.Phone == objPVA.phone && l.Otp == objPVA.otp).FirstOrDefault();
            if (userPVA != null)
            {
                userPVA.Otp = objPVA.otp;
                dbContext.Entry(userPVA).State = System.Data.EntityState.Modified;
            }
            else
            {
                PhoneVerificationAttempt pva = new PhoneVerificationAttempt();
                pva.Phone = objPVA.phone;
                pva.UserId = objPVA.user_id;
                pva.Otp = objPVA.otp;
                pva.Status = "PENDING";
                pva.AttemptedOn = DateTime.Now;
                dbContext.PhoneVerificationAttempts.Add(pva);
            }
            dbContext.SaveChanges();
            return objPVA;
        }

        public string DeductBalanceFromPaymentMonitoring(int UserId, decimal Balance)
        {
            try
            {
                PaymentMonitory objUser = dbContext.PaymentMonitories.Where(x => x.UserId == UserId).SingleOrDefault();

                objUser.TotalBalance = objUser.TotalBalance - Balance;

                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "1";

        }

        public List<UserDocument> GetUserDocuments(int UserId)
        {
            var userDocument = dbContext.UserDocuments.Where(x => x.UserId == UserId && x.IsActiveDocument == "Y").ToList();
            return userDocument;

        }
    }
}