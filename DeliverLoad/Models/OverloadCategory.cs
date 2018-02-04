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
    
    public partial class OverloadCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string ChannelNo { get; set; }
        public Nullable<bool> IsFree { get; set; }
        public Nullable<bool> IsAvailable { get; set; }
        public Nullable<bool> IsPresenterAvailability { get; set; }
        public string PickupLocation { get; set; }
        public Nullable<System.DateTime> PickupDate { get; set; }
        public string DropOffLocation { get; set; }
        public Nullable<System.DateTime> DropOffDate { get; set; }
        public Nullable<int> LoadSpaceId { get; set; }
    }
}