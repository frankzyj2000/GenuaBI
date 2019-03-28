using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using GenuinaBI.Models;
using GenuinaBI.Service;
using GenuinaBI.Configuration;
using GenuinaBI.Attributes;
using GenuinaBI.Extensions;
using FormCollection = System.Web.Mvc.FormCollection;

namespace GenuinaBI.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        #region Indivdual User Cookie NewLogins
        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.
        [AllowAnonymous]
        public ActionResult NewLogin()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return NewLogin();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult NewLogin(LocalAccountLoginViewModel model, string returnUrl)
        {
            using (UserService _service = new UserService())
            {
                if (model.UserId == null)
                {
                    model.ErrorMsg = Resources.Account.LoginResources.UserIdIsEmpty;
                    return View(model);
                }
                if (model.Password == null)
                {
                    model.ErrorMsg = Resources.Account.LoginResources.PasswordIsEmpty;
                    return View(model);
                }
                CFG_Users user = _service.GetUserById(model.UserId);
                if (user == null)
                {
                    model.ErrorMsg = Resources.Account.LoginResources.UserNotExist;
                    return View(model);
                }
                if (!user.Enabled)
                {
                    model.ErrorMsg = Resources.Account.LoginResources.UserNotActive;
                    return View(model);
                }

                if (!_service.ValidateUser(model.UserId, model.Password))
                {
                    model.ErrorMsg = Resources.Account.LoginResources.InvalidUserNamePassword;
                    return View(model);
                }

                using (DBVersionService dbVersionService = new DBVersionService() )
                {
                    if (!dbVersionService.IsDBVersionOK())
                    {
                        model.ErrorMsg = Resources.Shared.GeneralResources.DBVersionError.Replace("{0}", Config.MinimumDBVersion) ;
                        model.ErrorMsg = model.ErrorMsg.Replace("{1}", dbVersionService.GetDBVersion());

                        return View(model);
                    }
                }

                var claims = new List<Claim>();

                // create *required* claims
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.IDUser));
                claims.Add(new Claim(ClaimTypes.Name, _service.GetUserName(user)));
                claims.Add(new Claim("IsAdmin", _service.IsUserAdmin(user).ToString()));
                claims.Add(new Claim("Culture", user.IDLanguage.Trim()));

                IdentitySignin(claims, model.UserId, model.RememberMe);

                if (!string.IsNullOrEmpty(returnUrl)) 
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Dashboard", null);
            }
        }

        public ActionResult Logout()
        {
            IdentitySignout();
            return RedirectToAction("NewLogin");
        }
        #endregion

        #region New Account Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                DisplayErrorPage("Error", Resources.Account.AccountResources.EmptyUserId);
                return RedirectToAction("NewLogin");
            }

            using (UserService _service = new UserService())
            {
                CFG_Users user = _service.GetUserById(userId);

                if (user != null) //The UserId exists 
                {
                    try
                    {
                        _service.Delete(user);
                    }
                    catch
                    {
                        DisplayErrorPage("Error", Resources.Account.AccountResources.AccountDeleteFailed);
                    }
                }
                else
                {
                    IdentitySignout();
                }
            }
            return RedirectToAction("NewLogin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection formVars)
        {
            string id = formVars["Id"];
            using (UserService _service = new UserService())
            {
                CFG_Users user = _service.GetUserById(id);
                if (user != null) //The UserId exists 
                {
                    DisplayErrorPage("Error", Resources.Account.AccountResources.NewUserIdExist);
                    return View("Register");
                }
                string password = formVars["password"];
                string confirmPassword = formVars["confirmPassword"];
                if (confirmPassword != password)
                {
                    DisplayErrorPage("Error", Resources.Account.AccountResources.TwoPasswordNotSame);
                    return View("Register");
                }

                user = new CFG_Users();
                user.IDUser = id;
                user.Password = confirmPassword;
                user.IDLanguage = formVars["languageId"];
                user.FirstName = formVars["firstName"];
                user.LastName = formVars["lastName"];
                user.Enabled = Convert.ToBoolean(formVars["enabled"]);
                _service.Insert(user);

                var claims = new List<Claim>();

                // create *required* claims
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.IDUser));
                claims.Add(new Claim(ClaimTypes.Name, _service.GetUserName(user)));
                claims.Add(new Claim("IsAdmin", _service.IsUserAdmin(user).ToString()));
                claims.Add(new Claim("Culture", user.IDLanguage.Trim()));

                IdentitySignin(claims);
            }
            return RedirectToAction("Index", "Dashboard", null);
        }

        #endregion

        #region SignIn and Signout
        /// <summary>
        /// Helper method that adds the Identity cookie to the request output
        /// headers. Assigns the userState to the claims for holding user
        /// data without having to reload the data from disk on each request.
        /// 
        /// AppUserState is read in as part of the baseController class.
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="providerKey"></param>
        /// <param name="isPersistent"></param>
        public void IdentitySignin(List<Claim> claims, string providerKey = null, bool isPersistent = false)
        {
            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            // add to user here!
            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddDays(1)
            }, identity);

            var userCulture = identity.GetCulture();
            // set lang to the user's language 
            if (RouteData.Values["lang"].ToString() != userCulture)
            {
                RouteData.Values["lang"] = userCulture;
                SetThreadCulture(userCulture);
            }
        }

        public void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                DefaultAuthenticationTypes.ExternalCookie);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        #endregion

        #region Reset Password
        [AllowAnonymous]
        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordFailed()
        {
            return View();
        }
        #endregion
    }
}