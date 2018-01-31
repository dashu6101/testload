using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeliverLoad.Models;

namespace DeliverLoad.Services
{
    public partial class DeliverLoadService
    {
        public IEnumerable<UserModel> getParticipantsList(int CategoryId)
        {

            var ParticipantsList = (from U in dbContext.Users
                                    join PC in dbContext.ParticipantCategories on U.UserId equals PC.UserId
                                    join C in dbContext.Categories on PC.CategoryId equals C.CategoryId
                                    where C.CategoryId == CategoryId && PC.HasJoinedCategory == true
                                    select new UserModel
                                    {
                                        UserId = U.UserId,
                                        FirstName = U.FirstName,
                                        LastName = U.LastName,
                                        JoinDate = (DateTime)PC.JoinedDate,
                                        ProfilePicture = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                        UserName = U.EmailID,
                                        IsBlockedParticepant = PC.IsBlocked == null ? false : (bool)PC.IsBlocked

                                    });


            return ParticipantsList;
        }

        public IEnumerable<CategoryModel> getJoinedCategoryList(int UserId)
        {

            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.ParticipantCategories on U.UserId equals PC.UserId
                                join C in dbContext.Categories on PC.CategoryId equals C.CategoryId
                                join P in dbContext.PresenterCategories on PC.CategoryId equals P.CategoryId
                                join PU in dbContext.Users on P.UserId equals PU.UserId
                                where U.UserId == UserId && PC.HasJoinedCategory == true
                                select new CategoryModel
                                {
                                    CategoryId = C.CategoryId,
                                    Name = C.Name,
                                    Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
                                    UserId = U.UserId,
                                    ChannelNo = C.ChannelNo,
                                    Price = (decimal)C.Price,
                                    JoinedDate = (DateTime)PC.JoinedDate,
                                    IsFreeChannel = (bool)C.IsFree,
                                    FirstName = PU.FirstName,
                                    HasJoinedCategory = PC.HasJoinedCategory == null ? false : (bool)PC.HasJoinedCategory,
                                    LastName = PU.LastName,
                                    ProfileImage = PU.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + PU.ProfilePicture,
                                    IsBlockedParticepant = PC.IsBlocked == null ? false : (bool)PC.IsBlocked,
                                    IsChannelAvailable = C.IsAvailable == null ? false : (bool)C.IsAvailable
                                }).OrderByDescending(x => x.JoinedDate);

            //foreach (var item in categoryList)
            //{
            //    item.TotalParticipants = dbContext.ParticipantCategories.Where(x => x.CategoryId == item.CategoryId).Count();


            //}

            return categoryList;
        }

        public IEnumerable<CategoryModel> getAllCategoryList()
        {

            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.PresenterCategories on U.UserId equals PC.UserId
                                join C in dbContext.Categories on PC.CategoryId equals C.CategoryId
                                where U.UserType == "A"
                                select new CategoryModel
                                {
                                    CategoryId = C.CategoryId,
                                    Name = C.Name,
                                    Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
                                    //  UserId = U.UserId,
                                    ChannelNo = C.ChannelNo,
                                    Price = (decimal)C.Price,
                                    IsFreeChannel = (bool)C.IsFree,
                                    // JoinedDate = (DateTime)PC.JoinedDate

                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    ProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                    IsChannelAvailable = C.IsAvailable == null ? false : (bool)C.IsAvailable

                                }).OrderByDescending(x => x.Name);


            //var categoryList = (from U in dbContext.Users
            //                    join PC in dbContext.ParticipantCategories on U.UserId equals PC.UserId
            //                    join C in dbContext.Categories on PC.CategoryId equals C.CategoryId
            //                    join P in dbContext.PresenterCategories on PC.CategoryId equals P.CategoryId
            //                    join PU in dbContext.Users on P.UserId equals PU.UserId
            //                    where PC.HasJoinedCategory == true
            //                    select new CategoryModel
            //                    {
            //                        CategoryId = C.CategoryId,
            //                        Name = C.Name,
            //                        Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
            //                        UserId = U.UserId,
            //                        ChannelNo = C.ChannelNo,
            //                        Price = (decimal)C.Price,
            //                        JoinedDate = (DateTime)PC.JoinedDate,
            //                        IsFreeChannel = (bool)C.IsFree,
            //                        FirstName = PU.FirstName,
            //                        HasJoinedCategory = PC.HasJoinedCategory == null ? false : (bool)PC.HasJoinedCategory,
            //                        LastName = PU.LastName,
            //                        ProfileImage = PU.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + PU.ProfilePicture,
            //                        IsBlockedParticepant = PC.IsBlocked == null ? false : (bool)PC.IsBlocked,
            //                        IsChannelAvailable = C.IsAvailable == null ? false : (bool)C.IsAvailable
            //                    }).OrderByDescending(x => x.Name).Distinct();

            //foreach (var item in categoryList)
            //{
            //    item.TotalParticipants = dbContext.ParticipantCategories.Where(x => x.CategoryId == item.CategoryId).Count();


            //}

            return categoryList;
        }
        public string JoinCategory(int CategoryId, int UserId, Decimal Price)
        {
            try
            {

                var Partcipantsdetails = dbContext.ParticipantCategories.Where(x => x.CategoryId == CategoryId && x.UserId == UserId).FirstOrDefault();

                //check balance
                var userdetails = GetUserDetailsByUserId(UserId);
                Decimal Balance = userdetails.Balance;

                if (Balance >= Price)
                {
                    //deduct balance
                    DeductBalance(UserId, Price);

                    if (Partcipantsdetails != null)
                    {
                        Partcipantsdetails.HasJoinedCategory = true;

                        dbContext.SaveChanges();

                        return "1";
                    }

                    ParticipantCategory objUC = new ParticipantCategory();

                    objUC.CategoryId = CategoryId;
                    objUC.UserId = UserId;
                    objUC.HasJoinedCategory = true;
                    objUC.JoinedDate = DateTime.Now;
                    dbContext.ParticipantCategories.Add(objUC);
                    dbContext.SaveChanges();


                }
                else
                {
                    return "-1";
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "1";

        }

        public string RemoveCategory(int CategoryId, int UserId)
        {
            try
            {
                var objUC = dbContext.ParticipantCategories.Where(x => x.CategoryId == CategoryId && x.UserId == UserId).FirstOrDefault();

                objUC.HasJoinedCategory = false;
                objUC.JoinedDate = DateTime.Now;

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "1";

        }

        public string GetCategorySummary(int CategoryId, int UserId)
        {
            try
            {
                var _obj = dbContext.Categories.Where(x => x.CategoryId == CategoryId).Select(x => x.Description).FirstOrDefault();
                return _obj;
            }
            catch (Exception ex)
            {

                return "-1";
            }
        }

        public CategoryModel getCategoryDetails(int CategoryId, int UserId)
        {
            var Partcipantsdetails = dbContext.ParticipantCategories.Where(x => x.CategoryId == CategoryId && x.UserId == UserId).FirstOrDefault();
            bool HasJoinedCategory = false;
            if (Partcipantsdetails != null)
            {
                HasJoinedCategory = (bool)Partcipantsdetails.HasJoinedCategory;
            }

            var categoryDetails = (from C in dbContext.Categories
                                   join PC in dbContext.PresenterCategories on C.CategoryId equals PC.CategoryId
                                   join U in dbContext.Users on PC.UserId equals U.UserId
                                   where C.CategoryId == CategoryId
                                   select new CategoryModel
                                   {
                                       CategoryId = C.CategoryId,
                                       Name = C.Name,
                                       Description = C.Description,
                                       Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
                                       UserId = (int)PC.UserId,
                                       HasJoinedCategory = HasJoinedCategory,
                                       Price = (decimal)C.Price,
                                       IsFreeChannel = (bool)C.IsFree,
                                       FirstName = U.FirstName,
                                       LastName = U.LastName,
                                       ProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                       ChannelNo = "Channel no:" + C.ChannelNo,
                                       IsAuthenticated = U.IsAuthenticated == null ? false : (bool)U.IsAuthenticated
                                   }).FirstOrDefault();


            return categoryDetails;
        }


    }
}