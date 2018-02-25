using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class UserMasterModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ScreenName { get; set; }
        public string MaritalStatus { get; set; }
        public decimal? Age { get; set; }
        public string Gender { get; set; }
        public decimal? CityID { get; set; }
        public decimal? StateID { get; set; }
        public decimal? CountryID { get; set; }
        public decimal? EthnicityID { get; set; }
        public decimal? EducationID { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public Boolean? IsDisable { get; set; }
        public Boolean? IsChildWorking { get; set; }
        public string Picture { get; set; }
        public DateTime? DOB { get; set; }
        public string UserType { get; set; }
        public Boolean? UserStatus { get; set; }
        public int? RegistrationTypeID { get; set; }
        public decimal? CurID { get; set; }
        public decimal? ExchRate { get; set; }
        public DateTime? CurEffectiveDate { get; set; }
        public string ProfilePicture { get; set; }
        public string GroupName { get; set; }
        public Boolean? isSuspended { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastModified { get; set; }
        public decimal? LastModifiedBy { get; set; }
        public Boolean? isfree { get; set; }
        public string Activechat { get; set; }
        public decimal? Balance { get; set; }
        public decimal? PaymentAmount { get; set; }
        public int? PaymentFrequency { get; set; }
        public Boolean? MeetingAvailability { get; set; }
        public string BackgroundImage { get; set; }
        public string Expectation { get; set; }
        public Boolean? IsChannel { get; set; }
        public decimal? ContinentID { get; set; }
        public string ChannelNo { get; set; }
        public Boolean? IsAuthenticated { get; set; }
        public DateTime? LastOnline { get; set; }
        public Boolean? IsPhoneVerified { get; set; }
        public Boolean? IsEmailVerified { get; set; }
        public Boolean? IsBankVerified { get; set; }
        public Boolean? IsDriverLicenseVerified { get; set; }
        public Boolean? IsVIOVerified { get; set; }
        public Boolean? IsAnyIdVerified { get; set; }
    }
}