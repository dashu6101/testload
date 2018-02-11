using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class OrderSummury
    {
        public int CategoryId { get; set; }
        public string LoadName { get; set; }
        public string LoadDesc { get; set; }
        public DateTime? LoadCreatedDate { get; set; }
        public string LoadImage { get; set; }
        public decimal? LoadPrice { get; set; }
        public DateTime? LoadPickupDate { get; set; }
        public DateTime? LoadDropOffDate { get; set; }
        public string LoadPickupLocation { get; set; }
        public string LoadDropOffLocation { get; set; }
        public int? LoadSpaceId { get; set; }
        public bool? LoadIsFree { get; set; }
        public bool? LoadIsAvailable { get; set; }
        public string LoadChannelNo { get; set; }
        public string LoadSpaceTitle { get; set; }

        public int? LoadownerCategoryId { get; set; }
        public int? LoadOwnerId { get; set; }
        public bool? LoadOwnerHasJoinedCategory { get; set; }
        public bool? LoadOwnerIsBlocked { get; set; }
        public DateTime? LoadOwnerJoinedDate { get; set; }


        public string LoadOwnerFirstName { get; set; }
        public string LoadOwnerMiddleName { get; set; }
        public string LoadOwnerLastName { get; set; }
        public string LoadOwnerEmail { get; set; }
        public decimal? LoadOwnerAge { get; set; }
        public string LoadOwnerGender { get; set; }
        public decimal? LoadOwnerCityId { get; set; }
        public decimal? LoadOwnerStateId { get; set; }
        public decimal? LoadOwnerCountryId { get; set; }
        public string LoadOwnerAddress { get; set; }
        public DateTime? LoadOwnerDOB { get; set; }
        public string LoadOwnerProfileImage { get; set; }
        public DateTime? LoadOwnerLastOnlineDate { get; set; }

        public int? VehicleOwnerCategoryId { get; set; }
        public int? VehicleOwnerId { get; set; }
        public string VehicleOwnerFirstName { get; set; }
        public string VehicleOwnerMiddleName { get; set; }
        public string VehicleOwnerLastName { get; set; }
        public string VehicleOwnerEmail { get; set; }
        public decimal? VehicleOwnerAge { get; set; }
        public string VehicleOwnerGender { get; set; }
        public decimal? VehicleOwnerCityId { get; set; }
        public decimal? VehicleOwnerStateId { get; set; }
        public decimal? VehicleOwnerCountryId { get; set; }
        public string VehicleOwnerAddress { get; set; }
        public DateTime? VehicleOwnerDOB { get; set; }
        public string VehicleOwnerProfileImage { get; set; }
        public DateTime? VehicleOwnerLastOnlineDate { get; set; }
    }
}