using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GenuinaBI.Resources.Account;
namespace GenuinaBI.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [Display(Name = "Email", ResourceType = typeof(AccountResources))]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        public string Provider { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [Display(Name = "Code", ResourceType = typeof(AccountResources))]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "RememberThisBrowser", ResourceType = typeof(AccountResources))]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [Display(Name = "Email", ResourceType = typeof(AccountResources))]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [Display(Name = "Email", ResourceType = typeof(AccountResources))]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(AccountResources))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(AccountResources))]
        public bool RememberMe { get; set; }
    }

    public class LocalAccountLoginViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        public string UserId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(AccountResources))]
        public bool RememberMe { get; set; }

        public string ErrorMsg { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(AccountResources))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [StringLength(100, ErrorMessageResourceName = "PasswordMinLength", ErrorMessageResourceType = typeof(AccountResources), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(AccountResources))]
        [Compare("ConfirmPassword", ErrorMessageResourceName = "PasswordMustMatch", ErrorMessageResourceType = typeof(AccountResources))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(AccountResources))]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(AccountResources))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [StringLength(100, ErrorMessageResourceName = "PasswordMinLength", ErrorMessageResourceType = typeof(AccountResources), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(AccountResources))]
        [Compare("ConfirmPassword", ErrorMessageResourceName = "PasswordMustMatch", ErrorMessageResourceType = typeof(AccountResources))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(AccountResources))]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Code", ResourceType = typeof(AccountResources))]
        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(AccountResources))]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(AccountResources))]
        public string Email { get; set; }
    }
}