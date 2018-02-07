using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class FindSpaceViewModel
    {
        public string from { get; set; }
        public string to { get; set; }
        public DateTime? date { get; set; }
        public bool isauthenticated { get; set; }
    }
}