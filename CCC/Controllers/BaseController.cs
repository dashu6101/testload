using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using CCC.Utils;

namespace CCC.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        protected CCCUser sUser
        {
            get
            {

                if (User.Identity.IsAuthenticated && Session["sUser"] == null)
                {
                    Session["sUser"] = Utils.Utils.GetCCCUser(WebSecurity.CurrentUserName);                                       
                }

                return (CCCUser)Session["sUser"];
                
            }

            set { sUser = value; }
                                                 
        }

    }
}
