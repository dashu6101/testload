using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeliverLoad.Models;

namespace DeliverLoad.Services
{
    public partial class DeliverLoadService
    {
        private DeliverLoadEntities dbContext = new DeliverLoadEntities();

        public IEnumerable<CategoryModel> getAllLoadownerCategoryList()
        {
     
            var categoryList = (from U in dbContext.Users
                                join PC in dbContext.PresenterCategories on U.UserId equals PC.UserId
                                join OC in dbContext.OverloadCategories on PC.CategoryId equals OC.CategoryId
                                where U.UserType == "A"
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
                                    IsChannelAvailable = OC.IsAvailable == null ? false : (bool)OC.IsAvailable

                                }).OrderByDescending(x => x.Name);



            return categoryList;
        }
    }
}