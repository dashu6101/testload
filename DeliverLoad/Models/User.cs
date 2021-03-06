//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeliverLoad.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.CategoryContentComments = new HashSet<CategoryContentComment>();
            this.ChannelRequestMasters = new HashSet<ChannelRequestMaster>();
            this.ContentVisitors = new HashSet<ContentVisitor>();
            this.LoadownerCategories = new HashSet<LoadownerCategory>();
            this.ParticipantCategories = new HashSet<ParticipantCategory>();
            this.PhoneVerificationAttempts = new HashSet<PhoneVerificationAttempt>();
            this.PresenterCategories = new HashSet<PresenterCategory>();
            this.VehicleownerCategories = new HashSet<VehicleownerCategory>();
            this.webpages_Roles = new HashSet<webpages_Roles>();
            this.TrackingInformations = new HashSet<TrackingInformation>();
        }
    
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ScreenName { get; set; }
        public string MaritalStatus { get; set; }
        public Nullable<decimal> Age { get; set; }
        public string Gender { get; set; }
        public Nullable<decimal> CityID { get; set; }
        public Nullable<decimal> StateID { get; set; }
        public Nullable<decimal> CountryID { get; set; }
        public Nullable<decimal> EthnicityID { get; set; }
        public Nullable<decimal> EducationID { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public Nullable<bool> IsDisable { get; set; }
        public Nullable<bool> IsChildWorking { get; set; }
        public string Picture { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string UserType { get; set; }
        public Nullable<bool> UserStatus { get; set; }
        public Nullable<int> RegistrationTypeID { get; set; }
        public Nullable<decimal> CurID { get; set; }
        public Nullable<decimal> ExchRate { get; set; }
        public Nullable<System.DateTime> CurEffectiveDate { get; set; }
        public string ProfilePicture { get; set; }
        public string GroupName { get; set; }
        public Nullable<bool> isSuspended { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<System.DateTime> LastModified { get; set; }
        public Nullable<decimal> LastModifiedBy { get; set; }
        public Nullable<bool> isfree { get; set; }
        public string Activechat { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public Nullable<int> PaymentFrequency { get; set; }
        public Nullable<bool> MeetingAvailability { get; set; }
        public string BackgroundImage { get; set; }
        public string Expectation { get; set; }
        public Nullable<bool> IsChannel { get; set; }
        public Nullable<decimal> ContinentID { get; set; }
        public string ChannelNo { get; set; }
        public Nullable<bool> IsAuthenticated { get; set; }
        public Nullable<System.DateTime> LastOnline { get; set; }
        public Nullable<bool> IsPhoneVerified { get; set; }
        public Nullable<bool> IsEmailVerified { get; set; }
        public Nullable<bool> IsBankVerified { get; set; }
        public Nullable<bool> IsDriverLicenseVerified { get; set; }
        public Nullable<bool> IsVIOVerified { get; set; }
        public Nullable<bool> IsAnyIdVerified { get; set; }
    
        public virtual ICollection<CategoryContentComment> CategoryContentComments { get; set; }
        public virtual ICollection<ChannelRequestMaster> ChannelRequestMasters { get; set; }
        public virtual ICollection<ContentVisitor> ContentVisitors { get; set; }
        public virtual ICollection<LoadownerCategory> LoadownerCategories { get; set; }
        public virtual ICollection<ParticipantCategory> ParticipantCategories { get; set; }
        public virtual ICollection<PhoneVerificationAttempt> PhoneVerificationAttempts { get; set; }
        public virtual ICollection<PresenterCategory> PresenterCategories { get; set; }
        public virtual ICollection<VehicleownerCategory> VehicleownerCategories { get; set; }
        public virtual ICollection<webpages_Roles> webpages_Roles { get; set; }
        public virtual ICollection<TrackingInformation> TrackingInformations { get; set; }
    }
}
