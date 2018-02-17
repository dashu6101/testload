using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeliverLoad.Models;
using System.Web.Mvc;

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



        //New Requirement 
        public CategoryModel getOwnerCategoryDetails(int CategoryId, int UserId)
        {
            var Loadownerdetails = dbContext.LoadownerCategories.Where(x => x.CategoryId == CategoryId && x.UserId == UserId).FirstOrDefault();
            bool HasJoinedCategory = false;
            if (Loadownerdetails != null)
            {
                HasJoinedCategory = (bool)Loadownerdetails.HasJoinedCategory;
            }

            var categoryDetails = (from C in dbContext.OverloadCategories
                                   join PC in dbContext.VehicleownerCategories on C.CategoryId equals PC.CategoryId
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
                                       IsAuthenticated = U.IsAuthenticated == null ? false : (bool)U.IsAuthenticated,
                                       PickupDate = C.PickupDate,
                                       PickupLocation = C.PickupLocation,
                                       DropOffDate = C.DropOffDate,
                                       DropOffLocation = C.DropOffLocation,
                                       LoadSpaceId = C.LoadSpaceId,
                                   }).FirstOrDefault();


            return categoryDetails;
        }

        public IEnumerable<CategoryModel> getAllLoadownerCategoryList(int UserId)
        {
            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.VehicleownerCategories on U.UserId equals PC.UserId
                                join OC in dbContext.OverloadCategories on PC.CategoryId equals OC.CategoryId
                                join LS in dbContext.LoadSpaces on OC.LoadSpaceId equals LS.LoadSpaceId
                                //join LC in dbContext.LoadownerCategories on U.UserId equals LC.UserId into ucuc
                                //from y in ucuc.DefaultIfEmpty()
                                //join UU in dbContext.Users on PC.UserId equals UU.UserId into ucuc1
                                //from x in ucuc1.DefaultIfEmpty()
                                where U.UserType == "A"
                                //&& y.UserId == UserId
                                select new CategoryModel
                                {
                                    CategoryId = OC.CategoryId,
                                    Description = OC.Description,
                                    PickupLocation = OC.PickupLocation,
                                    PickupDate = (DateTime)OC.PickupDate,
                                    DropOffLocation = OC.DropOffLocation,
                                    DropOffDate = (DateTime)OC.DropOffDate,
                                    Name = OC.Name,
                                    Image = OC.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + OC.Image,
                                    //  UserId = U.UserId,
                                    ChannelNo = OC.ChannelNo,
                                    Price = (decimal)OC.Price,
                                    IsFreeChannel = (bool)OC.IsFree,
                                    // JoinedDate = (DateTime)PC.JoinedDate

                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    ProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                    IsChannelAvailable = OC.IsAvailable == null ? false : (bool)OC.IsAvailable,
                                    LoadSpaceTitle = LS.LoadSpaceTitle,
                                    //HasJoinedCategory = y.HasJoinedCategory == null ? false : (bool)y.HasJoinedCategory,
                                    //UserId = ucuc1.UserId
                                }).OrderByDescending(x => x.CategoryId);



            return categoryList;
        }

        public IEnumerable<CategoryModel> getLoadownerCategoryListBySearch(FindSpaceViewModel searchVM)
        {
            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.VehicleownerCategories on U.UserId equals PC.UserId
                                join OC in dbContext.OverloadCategories on PC.CategoryId equals OC.CategoryId
                                join LS in dbContext.LoadSpaces on OC.LoadSpaceId equals LS.LoadSpaceId
                                where U.UserType == "M"
                                select new CategoryModel
                                {
                                    CategoryId = OC.CategoryId,
                                    Description = OC.Description,
                                    PickupLocation = OC.PickupLocation,
                                    PickupDate = (DateTime)OC.PickupDate,
                                    DropOffLocation = OC.DropOffLocation,
                                    DropOffDate = (DateTime)OC.DropOffDate,
                                    Name = OC.Name,
                                    Image = OC.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + OC.Image,
                                    //  UserId = U.UserId,
                                    ChannelNo = OC.ChannelNo,
                                    Price = (decimal)OC.Price,
                                    IsFreeChannel = (bool)OC.IsFree,
                                    // JoinedDate = (DateTime)PC.JoinedDate

                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    ProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                    IsChannelAvailable = OC.IsAvailable == null ? false : (bool)OC.IsAvailable,
                                    LoadSpaceTitle = LS.LoadSpaceTitle
                                }).OrderByDescending(x => x.Name).ToList();


            if (searchVM.from != null)
            {
                categoryList = categoryList.Where(c => c.PickupLocation.Contains(searchVM.from)).ToList();
            }
            if (searchVM.to != null)
            {
                categoryList = categoryList.Where(c => c.DropOffLocation.Contains(searchVM.to)).ToList();
            }
            if (searchVM.date != null)
            {
                categoryList = categoryList.Where(c => c.PickupDate == searchVM.date).ToList();
            }

            return categoryList;
        }


        public SelectList getLoadSpaceList()
        {


            List<LoadSpace> objLoadSpacelist = (from data in dbContext.LoadSpaces
                                                select data).ToList();

            SelectList objmodeldata = new SelectList(objLoadSpacelist, "LoadSpaceId", "LoadSpaceTitle");
            /*Assign value to model*/

            return objmodeldata;
        }

        public CategoryModel getDeliveryLoadCategoryDetails(int CategoryId, int UserId, string Type)
        {
            var Loadownerdetails = dbContext.LoadownerCategories.Where(x => x.CategoryId == CategoryId && x.UserId == UserId).FirstOrDefault();
            bool HasJoinedCategory = false;
            //if (Loadownerdetails != null)
            //{
            //    HasJoinedCategory = (bool)Loadownerdetails.HasJoinedCategory;
            //}
            var categoryDetails = new CategoryModel();
            if (Type == "Load")
            {
                categoryDetails = (from C in dbContext.OverloadCategories
                                   join PC in dbContext.LoadownerCategories on C.CategoryId equals PC.CategoryId
                                   join U in dbContext.Users on PC.UserId equals U.UserId
                                   join LS in dbContext.LoadSpaces on C.LoadSpaceId equals LS.LoadSpaceId
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
                                       IsAuthenticated = U.IsAuthenticated == null ? false : (bool)U.IsAuthenticated,
                                       PickupLocation = C.PickupLocation,
                                       PickupDate = (DateTime)C.PickupDate,
                                       DropOffLocation = C.DropOffLocation,
                                       DropOffDate = (DateTime)C.DropOffDate,
                                       LoadSpaceTitle = LS.LoadSpaceTitle,
                                       RegistrationDate = U.RegistrationDate,
                                       LastOnline = U.LastOnline
                                   }).FirstOrDefault();


            }
            else
            {
                categoryDetails = (from C in dbContext.OverloadCategories
                                   join PC in dbContext.VehicleownerCategories on C.CategoryId equals PC.CategoryId
                                   join U in dbContext.Users on PC.UserId equals U.UserId
                                   join LS in dbContext.LoadSpaces on C.LoadSpaceId equals LS.LoadSpaceId
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
                                       IsAuthenticated = U.IsAuthenticated == null ? false : (bool)U.IsAuthenticated,
                                       PickupLocation = C.PickupLocation,

                                       PickupDate = (DateTime)C.PickupDate,
                                       DropOffLocation = C.DropOffLocation,
                                       DropOffDate = (DateTime)C.DropOffDate,
                                       LoadSpaceTitle = LS.LoadSpaceTitle,
                                       RegistrationDate = U.RegistrationDate,
                                       LastOnline = U.LastOnline
                                   }).FirstOrDefault();

            }

            categoryDetails.from = categoryDetails.PickupLocation;
            categoryDetails.to = categoryDetails.DropOffLocation;
            return categoryDetails;
        }

        public IEnumerable<CategoryModel> getJoinedOverLoadCategoryList(int UserId)
        {

            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.LoadownerCategories on U.UserId equals PC.UserId
                                join C in dbContext.OverloadCategories on PC.CategoryId equals C.CategoryId
                                join P in dbContext.VehicleownerCategories on PC.CategoryId equals P.CategoryId
                                join PU in dbContext.Users on P.UserId equals PU.UserId
                                join LS in dbContext.LoadSpaces on C.LoadSpaceId equals LS.LoadSpaceId
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
                                    IsChannelAvailable = C.IsAvailable == null ? false : (bool)C.IsAvailable,
                                    PickupLocation = C.PickupLocation,
                                    PickupDate = (DateTime)C.PickupDate,
                                    DropOffLocation = C.DropOffLocation,
                                    DropOffDate = (DateTime)C.DropOffDate,
                                    LoadSpaceTitle = LS.LoadSpaceTitle,
                                    LastOnline = U.LastOnline
                                }).OrderByDescending(x => x.JoinedDate);

            //foreach (var item in categoryList)
            //{
            //    item.TotalParticipants = dbContext.ParticipantCategories.Where(x => x.CategoryId == item.CategoryId).Count();


            //}

            return categoryList;
        }

        public IEnumerable<CategoryModel> getMyVehicleCategoryList(int UserId)
        {
            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.VehicleownerCategories on U.UserId equals PC.UserId
                                join OC in dbContext.OverloadCategories on PC.CategoryId equals OC.CategoryId
                                join LS in dbContext.LoadSpaces on OC.LoadSpaceId equals LS.LoadSpaceId
                                where U.UserType == "A" && U.UserId == UserId

                                select new CategoryModel
                                {
                                    CategoryId = OC.CategoryId,
                                    Description = OC.Description,
                                    PickupLocation = OC.PickupLocation,
                                    PickupDate = (DateTime)OC.PickupDate,
                                    DropOffLocation = OC.DropOffLocation,
                                    DropOffDate = (DateTime)OC.DropOffDate,
                                    Name = OC.Name,
                                    Image = OC.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + OC.Image,
                                    //  UserId = U.UserId,
                                    ChannelNo = OC.ChannelNo,
                                    Price = (decimal)OC.Price,
                                    IsFreeChannel = (bool)OC.IsFree,
                                    // JoinedDate = (DateTime)PC.JoinedDate

                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    ProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                    IsChannelAvailable = OC.IsAvailable == null ? false : (bool)OC.IsAvailable,
                                    LoadSpaceTitle = LS.LoadSpaceTitle
                                }).OrderByDescending(x => x.CategoryId);



            return categoryList;
        }

        public string JoinOverLoadCategory(int CategoryId, int UserId, Decimal Price)
        {
            try
            {

                var Loadownerdetails = dbContext.LoadownerCategories.Where(x => x.CategoryId == CategoryId && x.UserId == UserId).FirstOrDefault();

                //check balance
                var userdetails = GetUserDetailsByUserId(UserId);
                Decimal Balance = userdetails.Balance;

                if (Balance >= Price)
                {
                    //deduct balance
                    DeductBalance(UserId, Price);

                    if (Loadownerdetails != null)
                    {
                        Loadownerdetails.HasJoinedCategory = true;

                        dbContext.SaveChanges();

                        return "1";
                    }

                    LoadownerCategory objUC = new LoadownerCategory();

                    objUC.CategoryId = CategoryId;
                    objUC.UserId = UserId;
                    objUC.HasJoinedCategory = true;
                    objUC.JoinedDate = DateTime.Now;
                    dbContext.LoadownerCategories.Add(objUC);
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

        public string ProceedCategory(int CategoryId, int UserId, Decimal Price)
        {
            try
            {

                var Loadownerdetails = dbContext.LoadownerCategories.Where(x => x.CategoryId == CategoryId && x.UserId == UserId).FirstOrDefault();

                //check balance
                var Paymentdetails = GetPaymentDetailsByUserId(UserId);
                //Decimal Balance = Paymentdetails.TotalBalance;

                if (Paymentdetails.TotalBalance >= Price)
                {
                    //deduct balance
                    DeductBalanceFromPaymentMonitoring(UserId, Price);

                    if (Loadownerdetails != null)
                    {
                        Loadownerdetails.HasJoinedCategory = true;

                        dbContext.SaveChanges();

                        return "1";
                    }

                    LoadownerCategory objUC = new LoadownerCategory();

                    objUC.CategoryId = CategoryId;
                    objUC.UserId = UserId;
                    objUC.HasJoinedCategory = true;
                    objUC.JoinedDate = DateTime.Now;
                    dbContext.LoadownerCategories.Add(objUC);
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

        #region OrderSummury
        public OrderSummuryModel getOrderSummuryByCategoryId(int CategoryId)
        {
            OrderSummuryModel objOrderSummury = new OrderSummuryModel();
            objOrderSummury = (from OC in dbContext.OverloadCategories
                                join LOC in dbContext.VehicleownerCategories on OC.CategoryId equals LOC.CategoryId
                                join U in dbContext.Users on LOC.UserId equals U.UserId
                                join LS in dbContext.LoadSpaces on OC.LoadSpaceId equals LS.LoadSpaceId
                                where OC.CategoryId == CategoryId
                                select new OrderSummuryModel
                                {
                                    CategoryId = OC.CategoryId,
                                    
                                    LoadName = OC.Name,
                                    LoadDesc = OC.Description,
                                    LoadCreatedDate = OC.CreatedDate, 
                                    LoadImage = OC.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + OC.Image,
                                    LoadPrice = OC.Price,
                                    LoadPickupDate = OC.PickupDate,
                                    LoadDropOffDate = OC.DropOffDate,
                                    LoadPickupLocation = OC.PickupLocation,
                                    LoadDropOffLocation = OC.DropOffLocation,
                                    LoadSpaceId = OC.LoadSpaceId,
                                    LoadIsFree = OC.IsFree == null ? false : OC.IsFree,
                                    LoadIsAvailable = OC.IsAvailable == null ? false : OC.IsAvailable,
                                    LoadChannelNo = OC.ChannelNo,
                                    LoadSpaceTitle = LS.LoadSpaceTitle,

                                    LoadownerCategoryId = LOC.Id,
                                    LoadOwnerId = LOC.UserId,
                                    //LoadOwnerHasJoinedCategory = LOC.HasJoinedCategory == null ? false : LOC.HasJoinedCategory,
                                    //LoadOwnerIsBlocked = LOC.IsBlocked == null ? false : LOC.IsBlocked,
                                    //LoadOwnerJoinedDate = LOC.JoinedDate,
                                    LoadOwnerFirstName = U.FirstName,
                                    LoadOwnerMiddleName = U.MiddleName,
                                    LoadOwnerLastName = U.LastName,
                                    LoadOwnerEmail = U.EmailID,
                                    LoadOwnerAge = U.Age,
                                    LoadOwnerGender = U.Gender,
                                    LoadOwnerCityId = U.CityID,
                                    LoadOwnerStateId = U.StateID,
                                    LoadOwnerCountryId = U.CountryID,
                                    LoadOwnerAddress = U.Address,
                                    LoadOwnerDOB = U.DOB,
                                    LoadOwnerProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                    LoadOwnerLastOnlineDate = U.LastOnline
                                }).ToList().FirstOrDefault();

            if (objOrderSummury != null)
            {
                var VehicalOwnerSummury = (from OC in dbContext.OverloadCategories
                                           join VOC in dbContext.LoadownerCategories on OC.CategoryId equals VOC.CategoryId
                                           join U in dbContext.Users on VOC.UserId equals U.UserId
                                           join LS in dbContext.LoadSpaces on OC.LoadSpaceId equals LS.LoadSpaceId
                                           where OC.CategoryId == CategoryId
                                           select new OrderSummuryModel
                                           {
                                               VehicleOwnerCategoryId = VOC.Id,
                                               VehicleOwnerId = VOC.UserId,
                                               VehicleOwnerFirstName = U.FirstName,
                                               VehicleOwnerMiddleName = U.MiddleName,
                                               VehicleOwnerLastName = U.LastName,
                                               VehicleOwnerEmail = U.EmailID,
                                               VehicleOwnerAge = U.Age,
                                               VehicleOwnerGender = U.Gender,
                                               VehicleOwnerCityId = U.CityID,
                                               VehicleOwnerStateId = U.StateID,
                                               VehicleOwnerCountryId = U.CountryID,
                                               VehicleOwnerAddress = U.Address,
                                               VehicleOwnerDOB = U.DOB,
                                               VehicleOwnerProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                               VehicleOwnerLastOnlineDate = U.LastOnline
                                           }).ToList().FirstOrDefault();

                if (VehicalOwnerSummury != null)
                {
                    objOrderSummury.VehicleOwnerCategoryId = VehicalOwnerSummury.VehicleOwnerCategoryId;
                    objOrderSummury.VehicleOwnerId = VehicalOwnerSummury.VehicleOwnerId;
                    objOrderSummury.VehicleOwnerFirstName = VehicalOwnerSummury.VehicleOwnerFirstName;
                    objOrderSummury.VehicleOwnerMiddleName = VehicalOwnerSummury.VehicleOwnerMiddleName;
                    objOrderSummury.VehicleOwnerLastName = VehicalOwnerSummury.VehicleOwnerLastName;
                    objOrderSummury.VehicleOwnerEmail = VehicalOwnerSummury.VehicleOwnerEmail;
                    objOrderSummury.VehicleOwnerAge = VehicalOwnerSummury.VehicleOwnerAge;
                    objOrderSummury.VehicleOwnerGender = VehicalOwnerSummury.VehicleOwnerGender;
                    objOrderSummury.VehicleOwnerCityId = VehicalOwnerSummury.VehicleOwnerCityId;
                    objOrderSummury.VehicleOwnerStateId = VehicalOwnerSummury.VehicleOwnerStateId;
                    objOrderSummury.VehicleOwnerCountryId = VehicalOwnerSummury.VehicleOwnerCountryId;
                    objOrderSummury.VehicleOwnerAddress = VehicalOwnerSummury.VehicleOwnerAddress;
                    objOrderSummury.VehicleOwnerDOB = VehicalOwnerSummury.VehicleOwnerDOB;
                    objOrderSummury.VehicleOwnerProfileImage = VehicalOwnerSummury.VehicleOwnerProfileImage;
                    objOrderSummury.VehicleOwnerLastOnlineDate = VehicalOwnerSummury.VehicleOwnerLastOnlineDate;
                }
            }
            return objOrderSummury;
        }
        #endregion

        #region PaymentSummury
        public PaymentMonitoryModel getPaymentMonitoryByUserId(int UserId)
        {
            PaymentMonitoryModel objPaymentMonitory = new PaymentMonitoryModel();
            objPaymentMonitory = (from pm in dbContext.PaymentMonitories
                                  where pm.UserId == UserId
                                  select new PaymentMonitoryModel
                                  {
                                      Id = pm.Id,
                                      UserId = pm.UserId,
                                      TotalBalance = pm.TotalBalance,
                                      CreateDate = pm.CreateDate,
                                      ModifiedDate = pm.ModifiedDate,
                                      isActive = pm.isActive,
                                      isCouponCodeApplied = pm.isCouponCodeApplied,
                                      CouponCode = pm.CouponCode
                                  }).ToList().FirstOrDefault();

            if (objPaymentMonitory == null) {
                objPaymentMonitory = new PaymentMonitoryModel();
            }
            return objPaymentMonitory;
        }

        public PaymentMonitory GetPaymentDetailsByUserId(int UserId)
        {
            var PaymentMonitory = dbContext.PaymentMonitories.Where(x => x.UserId == UserId).FirstOrDefault();

            return PaymentMonitory;

        }
        #endregion
    }
}