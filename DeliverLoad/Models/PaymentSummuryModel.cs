using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class PaymentSummuryModel
    {
        public string payerName { get; set; }
        public string payerEmail { get; set; }
        public string payerPhone { get; set; }
        public string orderId { get; set; }
        public string merchantId { get; set; }
        public string serviceTypeId { get; set; }
        public string responseurl { get; set; }
        public string amt { get; set; }
        public string hash { get; set; }
        public string paymenttype { get; set; }
        public string apiKey { get; set; }
        public string gatewayUrl { get; set; }
        public string checkStatusUrl { get; set; }
    }
}