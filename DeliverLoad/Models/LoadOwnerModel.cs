using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class LoadOwnerModel
    {
        public LoadOwnerModel()
        {
            this.LoadOwner = new UserMasterModel();
        }
        public UserMasterModel LoadOwner { get; set; }
        public bool? HasJoinedCategory { get; set; }
        public bool? IsBlocked { get; set; }
        public DateTime? JoinedDate { get; set; }
    }
}