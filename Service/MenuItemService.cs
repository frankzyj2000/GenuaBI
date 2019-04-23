using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using GenuinaBI.Models;

namespace GenuinaBI.Service
{
    /// <summary>
    ///     Add any custom business logic (methods) here
    /// </summary>
    public interface IMenuItemService : IService<CFG_AppMenuItems>
    {
        List<CFG_AppMenuItemsTranslation> GetAllMenuItemsTranslation(CFG_Users user, string langId);
        List<CFG_AppMenuItems> GetAllMenuItems(CFG_Users user);
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class MenuItemService : Service<CFG_AppMenuItems>, IMenuItemService
    {
        public MenuItemService(DbContext db)
            : base(db)
        {
        }

        public MenuItemService()
            : base()
        {
        }

        public List<CFG_AppMenuItemsTranslation> GetAllMenuItemsTranslation(CFG_Users user, string langId)
        {
            List<CFG_AppMenuItemsTranslation> list = new List<CFG_AppMenuItemsTranslation>();
            foreach (CFG_AppMenuItems item in this.GetAllMenuItems(user))
            {
                if ( item.CFG_AppMenues != null && item.CFG_AppMenues.CFG_Apps != null )
                {
                    if (item.CFG_AppMenues.CFG_Apps.Description == Constants.APP_NAME) //ge only dashboard menu items
                    {
                        list.Add(item.CFG_AppMenuItemsTranslation.Where(i => i.IDLanguage.Trim() == langId).FirstOrDefault());
                    }
                }
            }
            return list;
        }

        public List<CFG_AppMenuItems> GetAllMenuItems(CFG_Users user)
        {
            List<CFG_AppMenuItems> list = new List<CFG_AppMenuItems>();
            foreach (CFG_UserGroups group in user.CFG_UserGroups)
            {
                foreach (CFG_AppMenuItems menuItem in group.CFG_AppMenuItems)
                {
                    if (!list.Exists(i => i.IDMenuItem == menuItem.IDMenuItem))
                    {
                        list.Add(menuItem);
                    }
                }
            }
            return list;
        }
    }
}