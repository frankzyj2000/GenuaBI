using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using System.Linq;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using GenuinaBI.Service;
using GenuinaBI.Cookies;
using GenuinaBI.Models;
using GenuinaBI.Extensions;
using GenuinaBI.Configuration;

namespace GenuinaBI.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected BaseController()
        { 
        }

        /// <summary>
        /// Allow external initialization of this controller by explicitly
        /// passing in a request context
        /// </summary>
        /// <param name="requestContext"></param>
        public void InitializeForced(RequestContext requestContext)
        {
            Initialize(requestContext);
        }

        /// <summary>
        /// Displays a self contained error page without redirecting.
        /// Depends on ErrorController.ShowError() to exist
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="redirectTo"></param>
        /// <returns></returns>
        protected internal ActionResult DisplayErrorPage(string title, string message, string detailErrorMessage = "", string redirectTo = null)
        {
            ErrorController controller = new ErrorController();
            controller.InitializeForced(ControllerContext.RequestContext);
            return controller.ShowError(title, message, detailErrorMessage, redirectTo);
        }

        # region Save/Remove File
        protected ActionResult SaveFile(HttpPostedFileBase file)
        {
            string physicalPath = string.Empty;

            if (file != null)
            {
                Guid guid = this.FormatFilename(file);

                var filename = Path.GetFileName(file.FileName);
                physicalPath = Path.Combine(Server.MapPath("~/App_Data/"), string.Format("{0}{1}", guid, filename));
                file.SaveAs(physicalPath);
            }

            // Return an empty content to signify success
            return Json(new{data=physicalPath},"text/plain");
        }

        private Guid FormatFilename(HttpPostedFileBase file)
        {
            var filename = Path.GetFileName(file.FileName);
            string formattedName = string.Empty;            

            while (true)
            {
                Guid newGuid = Guid.NewGuid();

                formattedName =  Path.Combine(Server.MapPath("~/App_Data/"),string.Format("{0}{1}", newGuid.ToString(), filename));

                if (!System.IO.File.Exists(formattedName))
                    return newGuid;
            }
        }

        protected ActionResult RemoveFile(string filename)
        {
            if (filename != null)
            {
                var fileName = Path.GetFileName(filename);
                var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);
                }
            }

            // Return an empty string to signify success
            return Content("");
        }
        # endregion 

        /*Need to override this method so that the following ExecuteCore() can be fired always*/
        protected override bool DisableAsyncSupport
        {
            get
            {
                return true;
            }
        }

        /*Set Culture for current thread*/
        protected override void ExecuteCore()
        {
            if (RouteData.Values["lang"] != null && !string.IsNullOrWhiteSpace(RouteData.Values["lang"].ToString()))
            {
                // set the culture from the route data (url)
                SetThreadCulture(RouteData.Values["lang"].ToString());
            }
            else
            {
                var langHeader = string.Empty;
                var user = User.Identity as ClaimsIdentity;
                if (user.IsAuthenticated)
                {
                    langHeader = user.GetCulture();
                }
                else
                {
                    //check the request's accept language first, if support, use it, if not get from casino default
                    if (HttpContext.Request.UserLanguages != null)
                    {
                        langHeader = HttpContext.Request.UserLanguages[0].Substring(0, 2);
                        if (langHeader != "es" && langHeader != "en")
                        {
                            //get from casino cookie culture
                            langHeader = this.BaseCasinoCookie.Culture.Trim();
                        }
                    }
                }

                SetThreadCulture(langHeader);
                // set the lang value into route data
                RouteData.Values["lang"] = langHeader;
            }
            base.ExecuteCore();
        }

        //customize Json to display special Microsoft Date Type
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new CustomJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        public void SetThreadCulture(string culture)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }

        public string GetCurrentUserId()
        {
            var user = User.Identity as ClaimsIdentity;
            return user.GetUserId();
        }

        public string GetCurrentCulture()
        {
            return RouteData.Values["lang"].ToString();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                HandleError(filterContext);
                filterContext.Result = RedirectToAction("ShowError", "Error", new { title = "Error", message = filterContext.Exception.Message } );
                filterContext.ExceptionHandled = true;
            };
        }

        protected void HandleError(ExceptionContext filterContext)
        {
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            Exception ex = filterContext.Exception;

            /*
            try
            {
                var configurationManagerService = Container.Resolve<IConfigurationManagerService>();
                bool sendMail = configurationManagerService.SendMailError;

                if (sendMail)
                {
                    var sb = new StringBuilder();

                    sb.AppendLine(string.Format("Error Call Center : {0}", this.GetCurrentCasinoId));
                    sb.AppendLine();                
                    sb.AppendLine();
                    sb.AppendLine(string.Format("Error : {0} : Stacktrace : {1}", ex.Message, ex.StackTrace));

                    var mailService = Container.Resolve<IMailService>();

                    mailService.Initialize(configurationManagerService.SenderEmail,
                                           configurationManagerService.UsernameEmail,
                                           configurationManagerService.PasswordEmail);
                    
                    mailService.SendMail("Error Call Center",sb.ToString(),configurationManagerService.RecipientsError); 
                } 
            }
            catch // If the exception cannot be send by email the application won't crash
            {

            };*/
            logger.Error("Controller:" + controller + " Action:" + action + "\n" + GetErrMsg(ex));
        }

        //Wrap all error message from Exception
        private string GetErrMsg(Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append(ex.Message);

            if (ex.InnerException != null)
            {
                msg.Append("\n");
                msg.Append(ex.InnerException.Message);
            }
            msg.Append("\n");
            msg.Append(ex.StackTrace);
            return msg.ToString();
        }

        public string GetCasinoCulture()
        {
            return this.BaseCasinoCookie.Culture;
        }

        public Cookie BaseCasinoCookie
        {
            get
            {
                var casino = new Cookie();
                HttpCookie cookie = HttpContext.Request.Cookies.Get(Constants.Cookies.Casino);
                if (cookie == null)
                {
                    //always get cookie from DB
                    using (CasinoService _service = new CasinoService())
                    {
                        var casino_data = _service.GetDefaultCasino();
                        if (casino_data != null)
                        {
                            //don't save cookie, just use it anyway
                            //SaveCasinoCookie(casino_data.IDCasino, casino_data.Description, casino_data.IDLanguageByDefault.Trim());
                            casino = new Cookie
                            {
                                Id = casino_data.IDCasino,
                                Name = casino_data.Description,
                                Culture = casino_data.IDLanguageByDefault.Trim()
                            };
                        }
                    }
                }
                else
                {
                    casino = new Cookie
                    {
                        Id = int.Parse(cookie.Values[Constants.CasinoCookie.Id]),
                        Name = cookie.Values[Constants.CasinoCookie.Name],
                        Culture = cookie.Values[Constants.CasinoCookie.Culture]
                    };
                }
                return casino;
            }
        }

        protected void SaveCasinoCookie(int id, string name, string culture)
        {
            HttpCookie casino = new HttpCookie(Constants.Cookies.Casino);
            casino.Values[Constants.CasinoCookie.Id] = id.ToString();
            casino.Values[Constants.CasinoCookie.Name] = name;
            casino.Values[Constants.CasinoCookie.Culture] = culture;
            casino.Expires = DateTime.Now.AddDays(1); //Casino Cookie will be refreshed from DB for every day

            HttpContext.Response.Cookies.Remove(Constants.Cookies.Casino);
            HttpContext.Response.SetCookie(casino);
        }

        //protected void SaveReportParameters(IQueryParamaters parameter)
        //{
        //    HttpCookie reportParameters = null;
        //    if (parameter is OperationSummaryParameters)
        //    {
        //        reportParameters = new HttpCookie(Constants.Cookies.OperationSummaryParameters);
        //        reportParameters.Values[Constants.ReportParameterCookie.Start] = ((OperationSummaryParameters)parameter).Start; ;
        //        reportParameters.Values[Constants.ReportParameterCookie.End] = ((OperationSummaryParameters)parameter).End;
        //        reportParameters.Values[Constants.ReportParameterCookie.PageLength] = ((OperationSummaryParameters)parameter).PageLength.ToString();
        //        HttpContext.Response.Cookies.Remove(Constants.Cookies.OperationSummaryParameters); //remove existing cookie
        //    }
        //    else if (parameter is TopPlayerParameters)
        //    {
        //        reportParameters = new HttpCookie(Constants.Cookies.TopPlayerParameters);
        //        reportParameters.Values[Constants.ReportParameterCookie.Start] = ((TopPlayerParameters)parameter).Start;
        //        reportParameters.Values[Constants.ReportParameterCookie.End] = ((TopPlayerParameters)parameter).End;
        //        reportParameters.Values[Constants.ReportParameterCookie.PageLength] = ((TopPlayerParameters)parameter).PageLength.ToString();
        //        reportParameters.Values[Constants.ReportParameterCookie.NumberOfPlayers] = ((TopPlayerParameters)parameter).NumberOfPlayers.ToString();
        //        reportParameters.Values[Constants.ReportParameterCookie.NumberOfVisits] = ((TopPlayerParameters)parameter).NumberOfVisits.ToString();
        //        HttpContext.Response.Cookies.Remove(Constants.Cookies.TopPlayerParameters); //remove existing cookie
        //    }
        //    else if (parameter is SlotOccupationParameters)
        //    {
        //        reportParameters = new HttpCookie(Constants.Cookies.SlotOccupationParameters);
        //        reportParameters.Values[Constants.ReportParameterCookie.Start] = ((SlotOccupationParameters)parameter).Start;
        //        reportParameters.Values[Constants.ReportParameterCookie.PageLength] = ((SlotOccupationParameters)parameter).PageLength.ToString();
        //        HttpContext.Response.Cookies.Remove(Constants.Cookies.SlotOccupationParameters); //remove existing cookie
        //    }

        //    reportParameters.Expires = DateTime.Now.AddDays(1);    
        //    HttpContext.Response.SetCookie(reportParameters);
        //}

        //protected IQueryParamaters GetReportParameters(string cookieName)
        //{
        //    HttpCookie cookie = null;
        //    cookie = HttpContext.Request.Cookies.Get(cookieName);
        //    if (cookie == null) return null; //cookie doesn't exist
        //    if (cookieName == Constants.Cookies.SlotOccupationParameters)
        //    {
        //        SlotOccupationParameters parameter = new SlotOccupationParameters();
        //        try
        //        { //trying to convert date from cookie to verify the date format is correct
        //            string startDate = cookie.Values[Constants.ReportParameterCookie.Start];
        //            DateTime start = DateTime.ParseExact(startDate, Config.CasinoDateFormat, null );
        //            parameter.Start = start.ToString(Config.CasinoDateFormat);
        //            parameter.PageLength = int.Parse(cookie.Values[Constants.ReportParameterCookie.PageLength]);
        //            return parameter;
        //        } //If reading cookie failed, remove the cookie, this is useful when date format is changed
        //        catch
        //        {
        //            //If can't convert the date from cookie, then delete the cookie
        //            HttpContext.Response.Cookies.Remove(Constants.Cookies.SlotOccupationParameters); //remove existing cookie
        //        }
        //        return parameter;
        //    }

        //    if (cookieName == Constants.Cookies.OperationSummaryParameters)
        //    {
        //        OperationSummaryParameters parameter = new OperationSummaryParameters();
        //        try
        //        {
        //            string startDate = cookie.Values[Constants.ReportParameterCookie.Start];
        //            string endDate = cookie.Values[Constants.ReportParameterCookie.End];
        //            DateTime start = DateTime.ParseExact(startDate, Config.CasinoDateTimeFormat, null);
        //            DateTime end = DateTime.ParseExact(endDate, Config.CasinoDateTimeFormat, null);
        //            parameter.Start = start.ToString(Config.CasinoDateTimeFormat);
        //            parameter.End = end.ToString(Config.CasinoDateTimeFormat);
        //            parameter.PageLength = int.Parse(cookie.Values[Constants.ReportParameterCookie.PageLength]);
        //        }
        //        catch
        //        {
        //            //If can't convert the date from cookie, then delete the cookie
        //            HttpContext.Response.Cookies.Remove(Constants.Cookies.OperationSummaryParameters); //remove existing cookie
        //        }
        //        return parameter;
        //    }

        //    if (cookieName == Constants.Cookies.TopPlayerParameters)
        //    {
        //        TopPlayerParameters parameter = new TopPlayerParameters();
        //        try
        //        {
        //            string startDate = cookie.Values[Constants.ReportParameterCookie.Start];
        //            string endDate = cookie.Values[Constants.ReportParameterCookie.End];
        //            DateTime start = DateTime.ParseExact(startDate, Config.CasinoDateTimeFormat, null);
        //            DateTime end = DateTime.ParseExact(endDate, Config.CasinoDateTimeFormat, null);
        //            parameter.Start = start.ToString(Config.CasinoDateTimeFormat);
        //            parameter.End = end.ToString(Config.CasinoDateTimeFormat);
        //            parameter.NumberOfPlayers = int.Parse(cookie.Values[Constants.ReportParameterCookie.NumberOfPlayers]);
        //            parameter.NumberOfVisits = int.Parse(cookie.Values[Constants.ReportParameterCookie.NumberOfVisits]);
        //            parameter.PageLength = int.Parse(cookie.Values[Constants.ReportParameterCookie.PageLength]);
        //        }
        //        catch
        //        {
        //            //If can't convert the date from cookie, then delete the cookie
        //            HttpContext.Response.Cookies.Remove(Constants.Cookies.TopPlayerParameters); //remove existing cookie
        //        }
        //        return parameter;
        //    }
        //    return null;
        //}
    }
}