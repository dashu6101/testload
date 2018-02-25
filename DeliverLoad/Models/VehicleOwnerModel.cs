using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class VehicleOwnerModel
    {
        public VehicleOwnerModel()
        {
            this.VehicleOwner = new UserMasterModel();
        }
        public UserMasterModel VehicleOwner { get; set; }
    }
}