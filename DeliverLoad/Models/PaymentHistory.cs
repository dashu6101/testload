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
    
    public partial class PaymentHistory
    {
        public int ID { get; set; }
        public string Payment_Code { get; set; }
        public string Tracking_Code { get; set; }
        public Nullable<System.DateTime> Payment_Date { get; set; }
        public Nullable<decimal> Payment_Price { get; set; }
        public string Email { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Request_Id { get; set; }
        public Nullable<bool> Is_Success { get; set; }
        public string reason_fault { get; set; }
        public decimal UserID { get; set; }
        public Nullable<bool> IsActivePaymentCode { get; set; }
        public Nullable<bool> IsActiveTrackingCode { get; set; }
        public Nullable<byte> Status { get; set; }
    }
}
