using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCC.Models;
using WebMatrix.WebData;


namespace CCC
{
    public class CCCUser
    {
        private CCCEntities dbContext = new CCCEntities();

        public int UserId { get; set; }
        public string  UserName { get; set; }
        public string UserType { get; set; }
        public string ProfilePicture { get; set; }
        public string  ChannelNo { get; set; }

        public CCCUser(string userName)
        {

            try
            {
                var curUser = dbContext.Users.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();

                UserId = curUser.UserId;
                UserName = curUser.FirstName;
                UserType = curUser.UserType;
                ProfilePicture = curUser.ProfilePicture == null ? "/Images/nopic.png" : "/Images/ProfilePicture/" + curUser.ProfilePicture;
                ChannelNo = curUser.ChannelNo;


            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
    }
}