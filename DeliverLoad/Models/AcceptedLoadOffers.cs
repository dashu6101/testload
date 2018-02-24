using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class AcceptedLoadOffers
    {
        public AcceptedLoadOffers()
        {
            orderSummuryModel = new OrderSummuryModel();
        }

        public long Id { get; set; }
        public int LoadId { get; set; }
        public int LoadOwnerId { get; set; }
        public int VehicleOwnerId { get; set; }
        public DateTime AcceptedDate { get; set; }
        public Decimal? OfferPrice { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? ModifiedAcceptedDate { get; set; }
        public bool? IsAcceptedByLoadOwner { get; set; }
        public DateTime? LoadOwnerAcceptedDate { get; set; }
        public OrderSummuryModel orderSummuryModel { get; set; }
    }
}