using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GenuinaBI.Configuration;

namespace GenuinaBI.Controllers
{
    public class SessionController : BaseController
    {
        // This is used from JavaScript to re-establish the user's session
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Never Cache
        public virtual ActionResult Extend()
        {
            // Re-establish the session timeout
            Session.Timeout = Config.SessionTimeOut;
            return new EmptyResult();
        }

        [Authorize]
        public virtual ActionResult Expire(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return RedirectToAction("Logout","Account");
        }
    }
}