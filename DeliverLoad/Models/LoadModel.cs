using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class LoadModel
    {
        public int LoadId { get; set; }
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
    }
}