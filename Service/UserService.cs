using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using GenuinaBI.Models;
using Encryption.Security;
namespace GenuinaBI.Service
{
    /// <summary>
    ///     Add any custom business logic (methods) here
    /// </summary>
    public interface IUserService : IService<CFG_Users>
    {
        CFG_Users GetUserById(string userId);
        List<CFG_UserGroups> GetUserGroups(string userId);

        List<CFG_AppMenues> GetAppMenus(string userId);
        List<CFG_AppMenuesTranslation> GetAppMenusTranslation(string userId, string langId);

        List<CFG_AppMenuItems> GetAllMenuItems(string userId);
        List<CFG_AppMenuItemsTranslation> GetAllMenuItemsTranslation(string userId, string langId);

        bool CanAccessPage(string pageName, string userId);
        string GetUserName(CFG_Users user);
        bool IsUserAdmin(CFG_Users user);
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class UserService : Service<CFG_Users>, IUserService
    {
        public UserService(DbContext db)
            : base(db)
        {
        }

        public UserService()
            : base()
        {
        }

        public CFG_Users GetUserById(string userId)
        {
            return this.GetAll().Where(i => i.IDUser.ToUpper() == userId.ToUpper()).FirstOrDefault();
        }

        public List<CFG_UserGroups> GetUserGroups(string userId)
        {
            CFG_Users user = this.GetUserById(userId);
            if (user != null && user.CFG_UserGroups != null)
            {
                return user.CFG_UserGroups.ToList();
            }
            return null;
        }

        public bool IsUserAdmin(CFG_Users user)
        {
            foreach (CFG_UserGroups group in user.CFG_UserGroups)
            {
                if (group.Description.IndexOf("Admin") != -1) /*Administrator Group*/
                {
                    return true;
                }
            }
            return false;
        }


        public List<CFG_AppMenues> GetAppMenus(string userId)
        {
            CFG_Users user = this.GetUserById(userId);
            if (user != null && user.CFG_UserGroups != null)
            {
                List<CFG_AppMenues> list = new List<CFG_AppMenues>();
                foreach (CFG_UserGroups group in user.CFG_UserGroups)
                {
                    foreach (CFG_AppMenues item in group.CFG_AppMenues)
                    {
                        if (!list.Exists(i => i.IDMenuHeader == item.IDMenuHeader) && item.CFG_Apps.Description == Constants.APP_NAME)
                            list.Add(item);
                    }
                }
                return list;
            }
            return null;
        }

        public List<CFG_AppMenuesTranslation> GetAppMenusTranslation(string userId, string langId)
        {
            List<CFG_AppMenuesTranslation> list = new List<CFG_AppMenuesTranslation>();
            foreach (CFG_AppMenues menu in this.GetAppMenus(userId))
            {
                list.Add(menu.CFG_AppMenuesTranslation.First(i => i.IDLanguage.Trim() == langId));
            }
            return list;
        }

        public List<CFG_AppMenuItems> GetAllMenuItems(string userId)
        {
            CFG_Users user = this.GetUserById(userId);
            using (MenuItemService _service = new MenuItemService())
            {
                return _service.GetAllMenuItems(user);
            }
        }

        public List<CFG_AppMenuItemsTranslation> GetAllMenuItemsTranslation(string userId, string langId)
        {
            CFG_Users user = this.GetUserById(userId);
            using (MenuItemService _service = new MenuItemService())
            {
                return _service.GetAllMenuItemsTranslation(user, langId);
            }
        }

        public bool CanAccessPage(string pageName, string userId)
        {
            CFG_Users user = this.GetUserById(userId);
            List<CFG_AppMenues> list = this.GetAppMenus(userId);
            if (list == null) return false;
            foreach (CFG_AppMenuItems item in this.GetAllMenuItems(userId))
            {
                if (item.AssemblyPath == pageName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the name fo the the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserName(CFG_Users user)
        {
            return user.FirstName + ' ' + user.LastName;
        }

        /*
        public void SetUserForEmailValidation(User user = null)
        {
            if (user == null)
                user = Entity;
            if (user == null)
                return;

            user.InActive = true;
            user.Validator = DataUtils.GenerateUniqueId(8);
        }

        protected override bool OnBeforeSave(User entity)
        {
            if (entity.Email == null)
                entity.Email = string.Empty;

            if (!entity.Password.EndsWith(App.PasswordEncodingPostfix))
                entity.Password = App.EncodePassword(entity.Password, entity.Id);

            entity.Updated = DateTime.UtcNow;

            return base.OnBeforeSave(entity);
        }


        protected override void OnValidate(User entity)
        {
            base.OnValidate(entity);

            bool isNew = entity.tstamp == null;

            if (string.IsNullOrEmpty(entity.Name))
                ValidationErrors.Add("Name must be provided.", "Name");
            if (string.IsNullOrEmpty(entity.Email))
                ValidationErrors.Add("An email address must be provided.", "Email");

            if (string.IsNullOrEmpty(entity.OpenId))
            {
                if (string.IsNullOrEmpty(entity.Password) || entity.Password.Length < 4)
                    ValidationErrors.Add("Password length must be at least 4 characters.", "Password");

                if (string.IsNullOrEmpty(entity.Email))
                    ValidationErrors.Add("Email address must be provided ", "Email");
            }

            // trying to add new email that already exists?
            if (Entity.tstamp == null && !string.IsNullOrEmpty(entity.Email))
            {
                if (Context.Users.Where(usr => usr.Email == Entity.Email).Count() > 0)
                    ValidationErrors.Add("Email address/username is already in use.", "Email");
            }
        }
                /// <summary>
        /// Users open ID account. NOTE should only be called if returned from
        /// a successful OpenId validation
        /// </summary>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        public User ValidateUserWithExternalLogin(string providerKey)
        {
            SetError();
            if (string.IsNullOrEmpty(providerKey))
            {
                SetError("Invalid login.");
                return null;
            }

            User user = LoadBase(usr => usr.OpenId == providerKey || usr.OpenIdClaim == providerKey);
            if (user == null)
            {
                SetError("OpenId Login is not associated with an account.");
                return null;
            }

            Entity = user;
            return user;
        }

        public User ValidateEmailAddress(string validator)
        {
            var user = LoadBase(usr => usr.Validator == validator);
            if (user == null)
                throw new ApplicationException("Invalid email validator id.");

            user.InActive = false;
            user.Validator = null;

            if (!Save())
                throw new ApplicationException("Unable to validate email address at this time.");

            return user;
        }*/

        /// <summary>
        /// Validates a user by username
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public bool ValidateUser(string username, string password)
        {
            CFG_Users user = this.GetUserById(username);

            if (user == null)
            {
                return false;
            }

            string encodedPassword = (new Encrypt()).encryptUserPasswd(password);

            //string originPassword = (new Encrypt()).decryptPassword(user.Password, "T.04ynYKhj8W");
            if (encodedPassword == user.Password)
            {
                return true; //set to true now
            }
            return false;
        }
    }
}