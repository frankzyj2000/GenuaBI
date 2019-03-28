using System;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Security.Claims;
using GenuinaBI.Service;
using System.Linq;
namespace GenuinaBI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class PageAuthorizeAttribute : AuthorizeAttribute
    {
        public string PageName { get; set; }

        public PageAuthorizeAttribute(string pageName)
        {
            this.PageName = pageName;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext is null!");
            }

            try
            {
                HttpCookie httpCookie = HttpContext.Current.Request.Cookies[Constants.Cookies.Casino];

                //if (httpCookie == null)                                
                //{
                //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                //    {
                //        { "controller",  "Account"},
                //        { "action", "Logout" }
                //    });                    
                //}
                //else 
                if (AuthorizeCore(filterContext.HttpContext))
                {
                    // Is Allowed to Access the ScreenID?
                    var user = HttpContext.Current.User as ClaimsPrincipal;
                    var claims = user.Claims.ToList();
                    var id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    if (!(new UserService()).CanAccessPage(PageName, id.Value))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary 
                        { 
                            { "controller", "Error" }, 
                            { "action", "AccessDenied" } 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary 
                { 
                    { "controller", "Error" }, 
                    { "action", "ShowError" },
                    { "title", "Error" },
                    { "message", ex.Message }
                });                
            }
        }
    }
}