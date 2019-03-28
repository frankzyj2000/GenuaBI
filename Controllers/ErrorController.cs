using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using GenuinaBI.Models;
using GenuinaBI.Extensions;

namespace GenuinaBI.Controllers
{
    public class ErrorController : BaseController
    {
        /// <summary>
        /// Displays a generic error message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="redirectTo"></param>
        /// <returns></returns>
        public ActionResult ShowError(string title, string message, string detailMessage="", string redirectTo=null)
        {
            if (redirectTo == null)
                redirectTo = Request.Url.OriginalString;

            if (string.IsNullOrEmpty(message))
                message = Resources.Shared.ErrorResources.UnspecifiedError;

            ErrorViewModel model = new ErrorViewModel
            {
                Message = message,
                Title = title,
                RedirectTo = redirectTo != null ? Url.Content(redirectTo) : "",
                MessageIsHtml = true
            };

            // Explicitly point at Error View regardless of action
            return View("Error", model);
        }

        /// <summary>
        /// Displays a generic error message but allows passing a view model directly for 
        /// additional flexibility
        /// </summary>
        /// <param name="errorModel"></param>
        /// <returns></returns>
        public ActionResult ShowErrorFromModel(ErrorViewModel errorModel)
        {
            return View("Error", errorModel);
        }

        public ActionResult ShowMessage(string title, string message, string detailMessage, string redirectTo)
        {
            return this.ShowError(title, message, detailMessage, redirectTo);
        }

        [HttpPost]
        public ActionResult LogJavaScriptError(JavascriptErrorModel model)
        {
            if (model != null)
            {
                string errorMsg = System.Environment.NewLine +"Client Context:" + model.Context + System.Environment.NewLine + "Details:" + model.Details;
                logger.Error(new JavaScriptErrorException(errorMsg));
                return Json("Success");
            }
            else
                return Json("Can not log the error!");
        }

        // GET: /Error/HttpError404
        public ActionResult HttpError404(string message)
        {
            return ShowError("Error 404", message);
        }

        // GET: /Error/HttpError404
        public ActionResult HttpError500(string message)
        {
            return ShowError("Error 500", message);
        }

        public ActionResult AccessDenied()
        {
            return ShowError("Error", Resources.Shared.ErrorResources.AccessDenied);
        }

        public ActionResult CauseError()
        {
            ErrorController controller = null;
            controller.CauseError();  // cause exception for debug
            // never called
            return View("Error");
        }


        // *** The following are STATIC controller methods that allow 
        // *** triggering the error display outside of a controller context
        // *** (from a module or global.asax handler for example)

        /// <summary>
        /// Static method that can be called from outside of MVC requests
        /// (like in Application_Error) to display an error View.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="redirectTo"></param>
        /// <param name="messageIsHtml"></param>
        public static void ShowErrorPage(string title, string message, string redirectTo)
        {
            ErrorController controller = new ErrorController();

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "ShowError");
            routeData.Values.Add("title", title);
            routeData.Values.Add("message", message);
            routeData.Values.Add("redirectTo", redirectTo);

            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(System.Web.HttpContext.Current), routeData));
        }

        /// <summary>
        /// Static method that can be called from outside of MVC requests
        /// (like in Application_Error) to display an error View.
        /// </summary>
        public static void ShowErrorPage(ErrorViewModel errorModel)
        {
            ErrorController controller = new ErrorController();

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "ShowErrorFromModel");
            routeData.Values.Add("errorModel", errorModel);

            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(System.Web.HttpContext.Current), routeData));
        }

        /// <summary>
        /// Static method that can be called from outside of MVC requests
        /// (like in Application_Error) to display an error View.
        /// </summary>
        public static void ShowErrorPage(string title, string message)
        {
            ShowErrorPage(title, message, null);
        }
    }
}