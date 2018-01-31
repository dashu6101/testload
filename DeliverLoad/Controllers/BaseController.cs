using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using DeliverLoad.Utils;

namespace DeliverLoad.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        protected DeliverLoadUser sUser
        {
            get
            {

                if (User.Identity.IsAuthenticated && Session["sUser"] == null)
                {
                    Session["sUser"] = Utils.Utils.GetDeliverLoadUser(WebSecurity.CurrentUserName);                                       
                }

                return (DeliverLoadUser)Session["sUser"];
                
            }

            set { sUser = value; }
                                                 
        }

    }
}
