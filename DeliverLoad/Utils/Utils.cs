using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeliverLoad.Models;
using WebMatrix.WebData;

namespace DeliverLoad.Utils
{

    public class DeliverLoadUser
    {

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string ProfilePicture { get; set; }
        public string ChannelNo { get; set; }
    }

    public static class Utils
    {

        public static DeliverLoadUser GetDeliverLoadUser(string userName)
        {
            if (userName == "" || userName == null)
            {

                DeliverLoadEntities dbContext = new DeliverLoadEntities();
                return new DeliverLoadUser
                {
                    UserId = 0,
                    UserName = "",
                    UserType = "",
                    ProfilePicture = "/Images/nopic.png",
                    ChannelNo = ""
                };

            }
            else
            {
                DeliverLoadEntities dbContext = new DeliverLoadEntities();
                var curUser = dbContext.Users.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();

                return new DeliverLoadUser
                {
                    UserId = curUser.UserId,
                    UserName = curUser.FirstName,
                    UserType = curUser.UserType,
                    ProfilePicture = curUser.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + curUser.ProfilePicture,
                    ChannelNo = curUser.ChannelNo
                };
            }


        }

        public static string GetUserName()
        {

            DeliverLoadEntities DB = new DeliverLoadEntities();

            if (HttpContext.Current.Session["UserName"] == null)
            {
                var c = HttpContext.Current.User.Identity.Name;
                string name = (from p in DB.Users
                               where p.UserName == c
                               select p.FirstName).FirstOrDefault();
                HttpContext.Current.Session["UserName"] = name;
                return name;
            }
            else
            {
                return HttpContext.Current.Session["UserName"].ToString();
            }
        }

        // function which returns image url or video url
        public static string GetCategoryPicURL(string pic)
        {


            if (string.IsNullOrEmpty(pic) || pic.ToLower() == "/images/nopic.png")
            {
                return "/Images/nopic.png";
            }
            else
            {
                return "/Images/Category" + pic;
            }

        }

        public static string ToRelativetime(DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now - dateTime;

            if (dateTime.Ticks == 0)
            {
                return " just now";
            }

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                return timeSpan.Seconds + " Seconds ago";
            }
            if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                return timeSpan.Minutes > 1 ? timeSpan.Minutes + " minutes ago" : "a minute ago";
            }
            if (timeSpan <= TimeSpan.FromHours(24))
            {
                return timeSpan.Hours > 1 ? timeSpan.Hours + " hours ago" : "an hour ago";
            }

            // span is less than or equal to 30 days (1 month), measure in days.

            if (timeSpan <= TimeSpan.FromDays(30))
            {
                return timeSpan.Days > 1 ? timeSpan.Days + " days ago" : "a day ago";
            }

            // span is less than or equal to 365 days (1 year), measure in months.

            if (timeSpan <= TimeSpan.FromDays(365))
            {
                return timeSpan.Days > 30 ? timeSpan.Days / 30 + " months ago" : "a month ago";
            }


            // span is greater than 365 days (1 year), measure in years.

            return timeSpan.Days > 365 ? timeSpan.Days / 365 + " years ago" : "a year ago";
        }
    }
}