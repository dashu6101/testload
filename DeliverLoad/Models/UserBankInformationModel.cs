using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class UserBankInformationModel
    {
        public int BankInfoId { get; set; }
        public string Name { get; set; }
        public string Bank { get; set; }
        public string AccountNumber { get; set; }
        public int UserId { get; set; }
    }
}