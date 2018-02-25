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
            PaymentHistory objPH = new PaymentHistory();
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

                        // return "1";
                    }
                    else
                    {
                        LoadownerCategory objUC = new LoadownerCategory();

                        objUC.CategoryId = CategoryId;
                        objUC.UserId = UserId;
                        objUC.HasJoinedCategory = true;
                        objUC.JoinedDate = DateTime.Now;
                        dbContext.LoadownerCategories.Add(objUC);
                        dbContext.SaveChanges();
                    }


                    var userdetails = GetUserDetailsByUserId(UserId);

                    var PaymentHistory = dbContext.PaymentHistories.FirstOrDefault();

                    System.Random rand = new System.Random((int)System.DateTime.Now.Ticks);
                    int random = rand.Next(1, 100000000);

                    objPH.UserID = UserId;
                    objPH.Payment_Date = DateTime.Now;
                    objPH.Payment_Price = Price;
                    objPH.Email = userdetails.EmailID;
                    objPH.First_Name = userdetails.FirstName;
                    objPH.Last_Name = userdetails.LastName;
                    objPH.Is_Success = true;
                    objPH.reason_fault="Success";
                    objPH.Tracking_Code = Convert.ToString(random);
                    dbContext.PaymentHistories.Add(objPH);
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

            return objPH.Tracking_Code;
        }

        #region OrderSummury
        public OrderSummuryModel getOrderSummuryByCategoryId(int CategoryId, int VehicleOwnerId, int LoadOwnerId)
        {
            OrderSummuryModel objOrderSummury = new OrderSummuryModel();

            objOrderSummury.LoadDetail   = (from OC in dbContext.OverloadCategories
                                            join LS in dbContext.LoadSpaces on OC.LoadSpaceId equals LS.LoadSpaceId
                                            where OC.CategoryId == CategoryId
                                            select new LoadModel
                                            {
                                                LoadId = OC.CategoryId,
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
                                                LoadSpaceTitle = LS.LoadSpaceTitle
                                            }).FirstOrDefault();

            objOrderSummury.LoadOwnerDetail = (from loc in dbContext.LoadownerCategories
                                               join u in dbContext.Users on loc.UserId equals u.UserId
                                               where loc.UserId == LoadOwnerId
                                               select new LoadOwnerModel
                                               {
                                                   HasJoinedCategory = loc.HasJoinedCategory,
                                                   IsBlocked = loc.IsBlocked,
                                                   JoinedDate = loc.JoinedDate,
                                                   LoadOwner = new UserMasterModel
                                                   {
                                                     UserId = u.UserId,
                                                     FirstName = u.FirstName,
                                                     MiddleName = u.MiddleName,
                                                     LastName = u.LastName,
                                                     ScreenName = u.ScreenName,
                                                     MaritalStatus = u.MaritalStatus,
                                                     Age = u.Age,
                                                     Gender = u.Gender,
                                                     CityID= u.CityID,
                                                     StateID = u.StateID,
                                                     CountryID = u.StateID,
                                                     EthnicityID = u.StateID,
                                                     EducationID = u.StateID,
                                                     EmailID = u.EmailID,
                                                     Address = u.Address,
                                                     Phone = u.Phone,
                                                     Mobile = u.Mobile,
                                                     IsDisable = u.IsDisable,
                                                     IsChildWorking = u.IsChildWorking,
                                                     Picture = u.Picture,
                                                     DOB = u.DOB,
                                                     UserType = u.UserType,
                                                     UserStatus = u.UserStatus,
                                                     RegistrationTypeID = u.RegistrationTypeID,
                                                     CurID = u.CurID,
                                                     ExchRate = u.ExchRate,
                                                     CurEffectiveDate = u.CurEffectiveDate,
                                                     ProfilePicture = u.ProfilePicture,
                                                     GroupName = u.GroupName,
                                                     isSuspended = u.isSuspended,
                                                     RegistrationDate = u.RegistrationDate,
                                                     LastModified = u.LastModified,
                                                     LastModifiedBy = u.LastModifiedBy,
                                                     isfree = u.isfree,
                                                     Activechat = u.Activechat,
                                                     Balance = u.Balance,
                                                     PaymentAmount = u.PaymentAmount,
                                                     PaymentFrequency = u.PaymentFrequency,
                                                     MeetingAvailability = u.MeetingAvailability,
                                                     BackgroundImage = u.BackgroundImage,
                                                     Expectation = u.Expectation,
                                                     IsChannel = u.IsChannel,
                                                     ContinentID = u.ContinentID,
                                                     ChannelNo = u.ChannelNo,
                                                     IsAuthenticated = u.IsAuthenticated,
                                                     LastOnline = u.LastOnline,
                                                     IsPhoneVerified = u.IsPhoneVerified,
                                                     IsEmailVerified = u.IsEmailVerified,
                                                     IsBankVerified = u.IsBankVerified,
                                                     IsDriverLicenseVerified = u.IsDriverLicenseVerified,
                                                     IsVIOVerified = u.IsVIOVerified,
                                                     IsAnyIdVerified = u.IsAnyIdVerified
                                                   }
                                               }).FirstOrDefault();

            objOrderSummury.VehicleOwnerDetail = (from voc in dbContext.VehicleownerCategories
                                               join u in dbContext.Users on voc.UserId equals u.UserId
                                               where voc.UserId == VehicleOwnerId
                                               select new VehicleOwnerModel
                                               {
                                                   VehicleOwner = new UserMasterModel
                                                   {
                                                       UserId = u.UserId,
                                                       FirstName = u.FirstName,
                                                       MiddleName = u.MiddleName,
                                                       LastName = u.LastName,
                                                       ScreenName = u.ScreenName,
                                                       MaritalStatus = u.MaritalStatus,
                                                       Age = u.Age,
                                                       Gender = u.Gender,
                                                       CityID = u.CityID,
                                                       StateID = u.StateID,
                                                       CountryID = u.StateID,
                                                       EthnicityID = u.StateID,
                                                       EducationID = u.StateID,
                                                       EmailID = u.EmailID,
                                                       Address = u.Address,
                                                       Phone = u.Phone,
                                                       Mobile = u.Mobile,
                                                       IsDisable = u.IsDisable,
                                                       IsChildWorking = u.IsChildWorking,
                                                       Picture = u.Picture,
                                                       DOB = u.DOB,
                                                       UserType = u.UserType,
                                                       UserStatus = u.UserStatus,
                                                       RegistrationTypeID = u.RegistrationTypeID,
                                                       CurID = u.CurID,
                                                       ExchRate = u.ExchRate,
                                                       CurEffectiveDate = u.CurEffectiveDate,
                                                       ProfilePicture = u.ProfilePicture,
                                                       GroupName = u.GroupName,
                                                       isSuspended = u.isSuspended,
                                                       RegistrationDate = u.RegistrationDate,
                                                       LastModified = u.LastModified,
                                                       LastModifiedBy = u.LastModifiedBy,
                                                       isfree = u.isfree,
                                                       Activechat = u.Activechat,
                                                       Balance = u.Balance,
                                                       PaymentAmount = u.PaymentAmount,
                                                       PaymentFrequency = u.PaymentFrequency,
                                                       MeetingAvailability = u.MeetingAvailability,
                                                       BackgroundImage = u.BackgroundImage,
                                                       Expectation = u.Expectation,
                                                       IsChannel = u.IsChannel,
                                                       ContinentID = u.ContinentID,
                                                       ChannelNo = u.ChannelNo,
                                                       IsAuthenticated = u.IsAuthenticated,
                                                       LastOnline = u.LastOnline,
                                                       IsPhoneVerified = u.IsPhoneVerified,
                                                       IsEmailVerified = u.IsEmailVerified,
                                                       IsBankVerified = u.IsBankVerified,
                                                       IsDriverLicenseVerified = u.IsDriverLicenseVerified,
                                                       IsVIOVerified = u.IsVIOVerified,
                                                       IsAnyIdVerified = u.IsAnyIdVerified
                                                   }
                                               }).FirstOrDefault();
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

            if (objPaymentMonitory == null)
            {
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

        #region Vehicle Owner Load Accept And Cancel

        public int GetLoadOwnerIdFromCategoryId(int CategoryId)
        {
            var loadOwnerId = (from c in dbContext.LoadownerCategories
                               where c.CategoryId == CategoryId
                               select c.UserId).FirstOrDefault();
            if (loadOwnerId != null && Convert.ToInt32(loadOwnerId) > 0)
            {
                return Convert.ToInt32(loadOwnerId);
            }
            else
            {
                return 0;
            }
        }
        
        public bool SaveAcceptedLoadDetail(int CategoryId, int loadOwnerId, int vehicleOwnerId)
        {
            var acceptedLoadDetail = (from c in dbContext.AcceptedLoadOffers
                                      where c.LoadId == CategoryId
                                        && c.LoadOwnerId == loadOwnerId && c.VehicleOwnerId == vehicleOwnerId
                                      select c).FirstOrDefault();

            if (acceptedLoadDetail == null)
            {
                AcceptedLoadOffer objAcceptedLoadOffer = new AcceptedLoadOffer();
                objAcceptedLoadOffer.LoadId = CategoryId;
                objAcceptedLoadOffer.LoadOwnerId = loadOwnerId;
                objAcceptedLoadOffer.VehicleOwnerId = vehicleOwnerId;
                objAcceptedLoadOffer.AcceptedDate = DateTime.UtcNow;
                objAcceptedLoadOffer.IsDelete = false;
                objAcceptedLoadOffer.IsAcceptedByLoadOwner = false;
                objAcceptedLoadOffer.LoadOwnerAcceptedDate = null;
                dbContext.AcceptedLoadOffers.Add(objAcceptedLoadOffer);
            }
            else
            {
                acceptedLoadDetail.IsDelete = false;
                acceptedLoadDetail.ModifiedAcceptedDate = DateTime.UtcNow;
                dbContext.Entry(acceptedLoadDetail).State = System.Data.EntityState.Modified;
            }

            if (dbContext.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int IsOfferAcceptedByVehicleOwner(int LoadId, int LoadOwnerId, int VehicleOwnerId)
        {

            int count = dbContext.AcceptedLoadOffers.Count(x => x.LoadId == LoadId
                                                && x.LoadOwnerId == LoadOwnerId 
                                                && x.VehicleOwnerId == VehicleOwnerId && x.IsDelete != true);
            return count;
        }


        public bool CancelAcceptedLoadDetail(int CategoryId, int loadOwnerId, int vehicleOwnerId)
        {
            var acceptedLoadDetail = (from c in dbContext.AcceptedLoadOffers
                                      where c.LoadId == CategoryId
                                        && c.LoadOwnerId == loadOwnerId && c.VehicleOwnerId == vehicleOwnerId
                                      select c).FirstOrDefault();

            if (acceptedLoadDetail != null)
            {
                acceptedLoadDetail.IsDelete = true;
                acceptedLoadDetail.ModifiedAcceptedDate = DateTime.UtcNow;
                dbContext.Entry(acceptedLoadDetail).State = System.Data.EntityState.Modified;
            }
            
            if (dbContext.SaveChanges() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<AcceptedLoadOffers> GetVehicleOwnerAcceptedOffers(int vehicleOwnerId)
        {
            List<AcceptedLoadOffers> acceptedLoadOffers = new List<AcceptedLoadOffers>();
            acceptedLoadOffers = (from c in dbContext.AcceptedLoadOffers
                                  where c.VehicleOwnerId == vehicleOwnerId && c.IsDelete != true
                                  select new AcceptedLoadOffers
                                  {
                                      Id = c.Id,
                                      LoadId = c.LoadId,
                                      LoadOwnerId = c.LoadOwnerId,
                                      VehicleOwnerId = c.VehicleOwnerId,
                                      IsDelete = c.IsDelete,
                                      AcceptedDate = c.AcceptedDate,
                                      ModifiedAcceptedDate = c.ModifiedAcceptedDate,
                                      IsAcceptedByLoadOwner = c.IsAcceptedByLoadOwner,
                                      LoadOwnerAcceptedDate = c.LoadOwnerAcceptedDate
                                  }).ToList();

            return acceptedLoadOffers;
        }
        #endregion

        #region Load Owner Accept And Cancel

        public SelectList GetLoadListByLoadOwnerUserId(int LoadOwnerUserId) {

            List<OverloadCategory> objLoadlist = (from oc in dbContext.OverloadCategories
                                                    join loc in dbContext.LoadownerCategories on oc.CategoryId equals loc.CategoryId
                                                    where loc.UserId == LoadOwnerUserId
                                                    select oc).ToList();

            SelectList objmodeldata = new SelectList(objLoadlist, "CategoryId", "Name");
            /*Assign value to model*/

            return objmodeldata;
        }

        public List<AcceptedLoadOffers> GetVehicleOwnersOffersByLoadId(int LoadId, int LoadOwnerUserId)
        {
            List<AcceptedLoadOffers> acceptedLoadOffers = new List<AcceptedLoadOffers>();
            acceptedLoadOffers = (from c in dbContext.AcceptedLoadOffers
                                  where c.LoadId == LoadId && c.IsDelete != true && c.LoadOwnerId == LoadOwnerUserId
                                  select new AcceptedLoadOffers
                                  {
                                      Id = c.Id,
                                      LoadId = c.LoadId,
                                      LoadOwnerId = c.LoadOwnerId,
                                      VehicleOwnerId = c.VehicleOwnerId,
                                      IsDelete = c.IsDelete,
                                      AcceptedDate = c.AcceptedDate,
                                      OfferPrice = c.OfferPrice,
                                      ModifiedAcceptedDate = c.ModifiedAcceptedDate,
                                      IsAcceptedByLoadOwner = c.IsAcceptedByLoadOwner,
                                      LoadOwnerAcceptedDate = c.LoadOwnerAcceptedDate
                                  }).ToList();

            return acceptedLoadOffers;
        }

        public int GetVehicleOwnerIdFromCategoryId(int CategoryId)
        {
            var vehicleOwnerId = (from c in dbContext.VehicleownerCategories
                               where c.CategoryId == CategoryId
                               select c.UserId).FirstOrDefault();
            if (vehicleOwnerId != null && Convert.ToInt32(vehicleOwnerId) > 0)
            {
                return Convert.ToInt32(vehicleOwnerId);
            }
            else
            {
                return 0;
            }
        }

        public bool ConfirmVehicleOwnerOfferByLoadOwner(int LoadID, int VehicleOwnerID, int LoadOwnerID) {

            var acceptedLoadDetail = (from c in dbContext.AcceptedLoadOffers
                                      where c.LoadId == LoadID
                                      && c.LoadOwnerId == LoadOwnerID && c.VehicleOwnerId == VehicleOwnerID
                                      select c).FirstOrDefault();

            if (acceptedLoadDetail != null)
            {
                acceptedLoadDetail.IsAcceptedByLoadOwner = true;
                acceptedLoadDetail.LoadOwnerAcceptedDate = DateTime.UtcNow;
                dbContext.Entry(acceptedLoadDetail).State = System.Data.EntityState.Modified;
                if (dbContext.SaveChanges() > 0)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool CancelConfirmVehicleOwnerOffer(int LoadId, int vehicleOwnerId, int loadOwnerId)
        {
            var acceptedLoadDetail = (from c in dbContext.AcceptedLoadOffers
                                      where c.LoadId == LoadId
                                        && c.LoadOwnerId == loadOwnerId && c.VehicleOwnerId == vehicleOwnerId
                                      select c).FirstOrDefault();

            if (acceptedLoadDetail != null)
            {
                acceptedLoadDetail.IsAcceptedByLoadOwner = false;
                acceptedLoadDetail.ModifiedAcceptedDate = null;
                dbContext.Entry(acceptedLoadDetail).State = System.Data.EntityState.Modified;
                if (dbContext.SaveChanges() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}