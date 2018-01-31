using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class PaymentHistoryModel
    {
      
            public int ID { get; set; }
            public string Payment_Id { get; set; }
            public string Txn_Id { get; set; }
            public System.DateTime Payment_Date { get; set; }
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
            //public Nullable<decimal> AdminID { get; set; }
            //public Nullable<bool> IsSubscription { get; set; }
       
    }
}