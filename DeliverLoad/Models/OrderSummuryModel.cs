using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class OrderSummuryModel
    {
        public OrderSummuryModel()
        {
            this.LoadDetail = new LoadModel();
            this.VehicleOwnerDetail = new VehicleOwnerModel();
            this.LoadOwnerDetail = new LoadOwnerModel();
            this.PaymentSummury = new PaymentSummuryModel();
        }
        public LoadModel LoadDetail { get; set; }
        public VehicleOwnerModel VehicleOwnerDetail { get; set; }
        public LoadOwnerModel LoadOwnerDetail { get; set; }
        public PaymentSummuryModel PaymentSummury { get; set; }
    }
}