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
    
    public partial class PhoneVerificationAttempt
    {
        public int PVA_Id { get; set; }
        public int UserId { get; set; }
        public string Phone { get; set; }
        public int Otp { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> AttemptedOn { get; set; }
    
        public virtual User User { get; set; }
    }
}