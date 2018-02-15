using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class PaymentMonitoryModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public decimal? TotalBalance { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? isActive { get; set; }
        public bool? isCouponCodeApplied { get; set; }
        public string CouponCode { get; set; }
    }
}