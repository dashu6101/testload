using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeliverLoad.Models;


namespace DeliverLoad.Services
{
    public partial class DeliverLoadService
    {
        public string CreateCategory(CategoryModel model, int UserId, string ChannelNo)
        {
            try
            {

                Category objCategory = new Category();
                objCategory.Name = model.Name;
                objCategory.Description = model.Description;
                objCategory.CreatedDate = DateTime.Now;
                objCategory.Price = model.Price;
                objCategory.IsFree = model.IsFreeChannel;
                objCategory.IsAvailable = false;

                var channelNo = GetMaxChannelNo(UserId, ChannelNo);

                objCategory.ChannelNo = channelNo;


                if (model.ImageUpload != null)
                {
                    string image = Guid.NewGuid().ToString();
                    image = image + model.ImageUpload.FileName.Substring(model.ImageUpload.FileName.LastIndexOf('.'));
                    string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/images/category/" + image);

                    model.ImageUpload.SaveAs(physicalPath);
                    objCategory.Image = image;

                }

                dbContext.Categories.Add(objCategory);


                PresenterCategory objUC = new PresenterCategory();

                objUC.CategoryId = objCategory.CategoryId;
                objUC.UserId = UserId;
                dbContext.PresenterCategories.Add(objUC);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "1";

        }


        public string EditCategory(CategoryModel model)
        {
            try
            {
                var objCategory = dbContext.Categories.Where(x => x.CategoryId == model.CategoryId).FirstOrDefault();
                objCategory.Name = model.Name;
                objCategory.Description = model.Description;
                objCategory.Price = model.Price;
                objCategory.IsFree = model.IsFreeChannel;

                if (model.ImageUpload != null)
                {
                    string image = Guid.NewGuid().ToString();
                    image = image + model.ImageUpload.FileName.Substring(model.ImageUpload.FileName.LastIndexOf('.'));
                    string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/images/category/" + image);

                    model.ImageUpload.SaveAs(physicalPath);
                    objCategory.Image = image;


                }

                dbContext.SaveChanges();



            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "1";

        }

        public IEnumerable<CategoryModel> getPresenterCategoryList(int UserId)
        {

            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.PresenterCategories on U.UserId equals PC.UserId
                                join C in dbContext.Categories on PC.CategoryId equals C.CategoryId
                                where U.UserId == UserId && U.UserType == "A"
                                select new CategoryModel
                                {
                                    CategoryId = C.CategoryId,
                                    Name = C.Name,
                                    Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
                                    UserId = U.UserId,
                                    CreatedDate = (DateTime)C.CreatedDate,
                                    ChannelNo = C.ChannelNo,
                                    Price = (decimal)C.Price,
                                    IsFreeChannel = (bool)C.IsFree,
                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    ProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                    IsChannelAvailable = C.IsAvailable == null ? true : (bool)C.IsAvailable,

                                }).OrderByDescending(x => x.CreatedDate);

            //foreach (var item in categoryList)
            //{
            //    item.TotalParticipants = dbContext.ParticipantCategories.Where(x => x.CategoryId == item.CategoryId).Count();


            //}


            return categoryList;
        }

        public IEnumerable<CategoryModel> getCategoryListByChannelNo(string ChannelNo)
        {

            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.PresenterCategories on U.UserId equals PC.UserId
                                join C in dbContext.Categories on PC.CategoryId equals C.CategoryId
                                where U.ChannelNo == ChannelNo && U.UserType == "A"
                                select new CategoryModel
                                {
                                    CategoryId = C.CategoryId,
                                    Name = C.Name,
                                    Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
                                    UserId = U.UserId,
                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    Price = (decimal)C.Price,
                                    ChannelNo = C.ChannelNo,
                                    IsFreeChannel = (bool)C.IsFree,
                                    IsChannelAvailable = C.IsAvailable == null ? true : (bool)C.IsAvailable,

                                });



            return categoryList;
        }

        public IEnumerable<CategoryModel> getChannelDetailsByChannelNo(string ChannelNo)
        {

            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.PresenterCategories on U.UserId equals PC.UserId
                                join C in dbContext.Categories on PC.CategoryId equals C.CategoryId
                                where C.ChannelNo == ChannelNo && U.UserType == "A"
                                select new CategoryModel
                                {
                                    CategoryId = C.CategoryId,
                                    Name = C.Name,
                                    Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
                                    UserId = U.UserId,
                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    Price = (decimal)C.Price,
                                    ChannelNo = C.ChannelNo,
                                    IsFreeChannel = (bool)C.IsFree,
                                    IsChannelAvailable = C.IsAvailable == null ? true : (bool)C.IsAvailable,

                                });



            return categoryList;
        }
        public string GetChannelNumberByCategoryId(int CategoryID)
        {
            string ChannelNumber = (from C in dbContext.Categories
                                    where C.CategoryId == CategoryID
                                    select C.ChannelNo).FirstOrDefault();
            return ChannelNumber;
        }

        public PresenterContentLinkViewModel GetChannelContentLinkDetail(int NodeId, string UserName, string ChannelName)
        {
            var Result = (from CC in dbContext.CategoryContents
                          join C in dbContext.Categories on CC.CategoryId equals C.CategoryId
                          join U in dbContext.Users on CC.UserId equals U.UserId
                          where CC.Id == NodeId
                          select new PresenterContentLinkViewModel
                          {
                              CategoryId = C.CategoryId,
                              CategoryName = C.Name,
                              ChannelName = CC.Name,
                              Description = CC.Description,
                              ImageName = CC.ImageName,
                              VideoName = CC.VideoName,
                              VideoFrom = CC.VideoFrom,
                              Title = CC.Name,
                              ChannelNo = C.ChannelNo,
                              UserId = CC.UserId,
                              CategoryPresenterName = U.FirstName + " " + U.LastName,
                              IsChannelAvailable = C.IsAvailable == null ? true : (bool)C.IsAvailable,

                          }).FirstOrDefault();
            return Result;
        }


        public IEnumerable<CategoryModel> getChannelDetailsByName(string Name)
        {

            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.PresenterCategories on U.UserId equals PC.UserId
                                join C in dbContext.Categories on PC.CategoryId equals C.CategoryId
                                where (C.Name.Contains(Name) || C.Description.Contains(Name)) && U.UserType == "A"
                                select new CategoryModel
                                {
                                    CategoryId = C.CategoryId,
                                    Name = C.Name,
                                    Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
                                    UserId = U.UserId,
                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    Price = (decimal)C.Price,
                                    ChannelNo = C.ChannelNo,
                                    IsFreeChannel = (bool)C.IsFree,
                                    IsChannelAvailable = C.IsAvailable == null ? true : (bool)C.IsAvailable,
                                });



            return categoryList;
        }

        //public IEnumerable<CategoryModel> getCategoryListByName(string Name)
        //{

        //    var categoryList = (from U in dbContext.Users
        //                        join PC in dbContext.PresenterCategories on U.UserId equals PC.UserId
        //                        join C in dbContext.Categories on PC.CategoryId equals C.CategoryId
        //                        where U.FirstName == Name
        //                        select new CategoryModel
        //                        {
        //                            CategoryId = C.CategoryId,
        //                            Name = C.Name,
        //                            Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
        //                            UserId = U.UserId


        //                        });


        //    return categoryList;
        //}


        public string GetMaxChannelNo(int UserId, string PresenterChannelNo)
        {
            try
            {
                double MaxChannelNo = 0;
                string ChannelNo = ""; ;
                ChannelNo = getPresenterCategoryList(UserId).Select(x => x.ChannelNo).FirstOrDefault();

                if (ChannelNo != null)
                {
                    var CNo = ChannelNo.Split('.');

                    double No = Convert.ToDouble(CNo[1]);
                    if (No >= 9)
                    {
                        ChannelNo = CNo[0] + "." + Convert.ToString(No + 1);
                    }
                    else
                    {
                        MaxChannelNo = Convert.ToDouble(ChannelNo);
                        ChannelNo = Convert.ToString(MaxChannelNo + 0.1);
                    }


                }
                else
                {
                    ChannelNo = PresenterChannelNo.ToString() + ".1";

                }

                return ChannelNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public CategoryModel getCategoryDetails(int CategoryId)
        //{


        //    var categoryDetails = dbContext.Categories.Where(x => x.CategoryId == CategoryId).Select(x => new CategoryModel { CategoryId = x.CategoryId, Name = x.Name, Description = x.Description, Image = x.Image == null ? "/Images/nopic.png" : "/Images/Category/" + x.Image }).FirstOrDefault();

        //    return categoryDetails;
        //}

        public IEnumerable<Subscription> getSubscriberList()
        {
            var result = dbContext.Database.SqlQuery<Subscription>("SELECT * FROM [Subscription]").ToList();
            return result;

        }
        public IEnumerable<User> GetUsersList()
        {
            var result = dbContext.Users.ToList();
            return result;
        }

        public Boolean UserIsAuthenticated(int UserID)
        {
            var result = dbContext.Users.Where(x => x.UserId == UserID).Select(x => x.IsAuthenticated).FirstOrDefault();
            return result == null ? false : (bool)result;
        }
        public string DeleteCategoryParticepentById(int CategoryID, int UserID)
        {
            try
            {
                var channel = dbContext.ParticipantCategories.Where(x => x.CategoryId == CategoryID && x.UserId == UserID).FirstOrDefault();

                if (channel != null)
                {
                    dbContext.ParticipantCategories.Remove(channel);
                    dbContext.SaveChanges();
                    return "1";
                }

                return "-1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string BlockCategoryParticepentById(int CategoryID, int UserID, bool status)
        {
            try
            {
                var channel = dbContext.ParticipantCategories.Where(x => x.CategoryId == CategoryID && x.UserId == UserID).FirstOrDefault();
                if (channel != null)
                {

                    channel.IsBlocked = status;
                    dbContext.SaveChanges();
                    return "1";
                }
                return "-1";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CategoryAvailability(int CategoryID, bool Status)
        {
            try
            {
                var Category = dbContext.Categories.Where(x => x.CategoryId == CategoryID).FirstOrDefault();
                if (Category != null)
                {
                    Category.IsAvailable = Status;
                    dbContext.SaveChanges();
                    return "1";
                }
                return "-1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string DeleteCommnet(int UserId, int CommentId)
        {
            try
            {
                var CommentDetail = dbContext.CategoryContentComments.Where(x => x.Id == CommentId).FirstOrDefault();
                if (CommentDetail != null)
                {
                    dbContext.CategoryContentComments.Remove(CommentDetail);
                    dbContext.SaveChanges();
                    return "1";
                }

                return "-1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteUser(int UserId)
        {
            try
            {
                var Users = dbContext.Users.Where(x => x.UserId == UserId).FirstOrDefault();
                if (Users != null)
                {
                    dbContext.Users.Remove(Users);
                    dbContext.SaveChanges();
                    return "1";

                }
                return "-1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string BlockUser(int UserId, bool Status)
        {
            try
            {
                var users = dbContext.Users.Where(x => x.UserId == UserId).FirstOrDefault();
                if (users != null)
                {
                    users.isSuspended = Status == false ? true : false;
                    dbContext.SaveChanges();
                    return "1";
                }
                return "-1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //New requirement 
        public string EditCategoryOwner(CategoryModel model)
        {
            try
            {
                var objCategory = dbContext.OverloadCategories.Where(x => x.CategoryId == model.CategoryId).FirstOrDefault();
                objCategory.Name = model.Name;
                objCategory.Description = model.Description;
                objCategory.Price = model.Price;
                objCategory.IsFree = model.IsFreeChannel;
                objCategory.DropOffLocation = model.DropOffLocation;
                objCategory.DropOffDate = model.DropOffDate;
                objCategory.PickupLocation = model.PickupLocation;
                objCategory.PickupDate = model.PickupDate;
                objCategory.LoadSpaceId = model.LoadSpaceId;

                if (model.ImageUpload != null)
                {
                    string image = Guid.NewGuid().ToString();
                    image = image + model.ImageUpload.FileName.Substring(model.ImageUpload.FileName.LastIndexOf('.'));
                    string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/images/category/" + image);

                    model.ImageUpload.SaveAs(physicalPath);
                    objCategory.Image = image;


                }

                dbContext.SaveChanges();



            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "1";

        }

        public IEnumerable<CategoryModel> getVehicleLoadCategoryList(int UserId, FindSpaceViewModel searchVM)
        {

            var categoryList = (from U in dbContext.Users
                                join VC in dbContext.LoadownerCategories on U.UserId equals VC.UserId
                                join OC in dbContext.OverloadCategories on VC.CategoryId equals OC.CategoryId
                                join LS in dbContext.LoadSpaces on OC.LoadSpaceId equals LS.LoadSpaceId
                                //where U.UserId == UserId 
                                where U.UserType == "M"
                               
                                select new CategoryModel
                                {
                                    CategoryId = OC.CategoryId,
                                    Name = OC.Name,
                                    Image = OC.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + OC.Image,
                                    UserId = U.UserId,
                                    CreatedDate = (DateTime)OC.CreatedDate,
                                    ChannelNo = OC.ChannelNo,
                                    Price = (decimal)OC.Price,
                                    IsFreeChannel = (bool)OC.IsFree,
                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    ProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                    IsChannelAvailable = OC.IsAvailable == null ? true : (bool)OC.IsAvailable,



                                    Description = OC.Description,
                                    PickupLocation = OC.PickupLocation,
                                    PickupDate = (DateTime)OC.PickupDate,
                                    DropOffLocation = OC.DropOffLocation,
                                    DropOffDate = (DateTime)OC.DropOffDate,

                                    LoadSpaceTitle = LS.LoadSpaceTitle

                                }).OrderByDescending(x => x.CategoryId).ToList();

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


        public string CreateOverLoadCategory(CategoryModel model, int UserId, string ChannelNo, string PostType)
        {
            try
            {

                OverloadCategory objCategory = new OverloadCategory();
                objCategory.Name = model.Name;
                objCategory.Description = model.Description;
                objCategory.CreatedDate = DateTime.Now;
                objCategory.Price = model.Price;
                objCategory.IsFree = model.IsFreeChannel;
                objCategory.PickupDate = model.PickupDate;
                objCategory.PickupLocation = model.PickupLocation;
                objCategory.DropOffDate = model.DropOffDate;
                objCategory.DropOffLocation = model.DropOffLocation;
                objCategory.LoadSpaceId = model.LoadSpaceId;
                objCategory.IsAvailable = false;

                var channelNo = GetMaxChannelNoVehicleOwner(UserId, ChannelNo);

                objCategory.ChannelNo = channelNo;


                if (model.ImageUpload != null)
                {
                    string image = Guid.NewGuid().ToString();
                    image = image + model.ImageUpload.FileName.Substring(model.ImageUpload.FileName.LastIndexOf('.'));
                    string physicalPath = System.IO.Path.Combine(
                                  System.Web.HttpContext.Current.Server.MapPath("~/images/category/"), image);
                    //string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/images/category/" + image);

                    model.ImageUpload.SaveAs(physicalPath);
                    objCategory.Image = image;

                }

                dbContext.OverloadCategories.Add(objCategory);

                if (PostType == "Vehicles")
                {
                    VehicleownerCategory objVC = new VehicleownerCategory();

                    objVC.CategoryId = objCategory.CategoryId;
                    objVC.UserId = UserId;
                    dbContext.VehicleownerCategories.Add(objVC);
                    dbContext.SaveChanges();
                }
                else
                {
                    LoadownerCategory objUC = new LoadownerCategory();

                    objUC.CategoryId = objCategory.CategoryId;
                    objUC.UserId = UserId;
                    dbContext.LoadownerCategories.Add(objUC);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return "1";
            
        }


        public string GetMaxChannelNoVehicleOwner(int UserId, string VehicleOwnerChannelNo)
        {
            try
            {
                double MaxChannelNo = 0;
                string ChannelNo = ""; ;
                ChannelNo = getVehicleOwnerCategoryList(UserId).Select(x => x.ChannelNo).FirstOrDefault();

                if (ChannelNo != null)
                {
                    var CNo = ChannelNo.Split('.');

                    double No = Convert.ToDouble(CNo[1]);
                    if (No >= 9)
                    {
                        ChannelNo = CNo[0] + "." + Convert.ToString(No + 1);
                    }
                    else
                    {
                        MaxChannelNo = Convert.ToDouble(ChannelNo);
                        ChannelNo = Convert.ToString(MaxChannelNo + 0.1);
                    }


                }
                else
                {
                    ChannelNo = VehicleOwnerChannelNo.ToString() + ".1";

                }

                return ChannelNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IEnumerable<CategoryModel> getVehicleOwnerCategoryList(int UserId)
        {

            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.VehicleownerCategories on U.UserId equals PC.UserId
                                join C in dbContext.OverloadCategories on PC.CategoryId equals C.CategoryId
                                where U.UserId == UserId && U.UserType == "A"
                                select new CategoryModel
                                {
                                    CategoryId = C.CategoryId,
                                    Name = C.Name,
                                    Image = C.Image == null ? "/Images/CategoryImage.jpg" : "/Images/Category/" + C.Image,
                                    UserId = U.UserId,
                                    CreatedDate = (DateTime)C.CreatedDate,
                                    ChannelNo = C.ChannelNo,
                                    Price = (decimal)C.Price,
                                    IsFreeChannel = (bool)C.IsFree,
                                    FirstName = U.FirstName,
                                    LastName = U.LastName,
                                    ProfileImage = U.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + U.ProfilePicture,
                                    IsChannelAvailable = C.IsAvailable == null ? true : (bool)C.IsAvailable,

                                }).OrderByDescending(x => x.CreatedDate);

        

            return categoryList;
        }
    }
}