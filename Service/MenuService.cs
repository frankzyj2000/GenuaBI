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
    public interface IMenuService : IService<CFG_AppMenues>
    {
        CFG_AppMenuesTranslation GetMenuNameByMenuId(int menuId, string langId);
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class MenuService : Service<CFG_AppMenues>, IMenuService
    {
        public MenuService(DbContext db)
            : base(db)
        {
        }

        public MenuService()
            : base()
        {
        }

        public CFG_AppMenuesTranslation GetMenuNameByMenuId(int menuId, string langId)
        {
            CFG_AppMenues menuItem = this.GetAll().Where(i => i.IDMenuHeader == menuId).FirstOrDefault();
            if (menuItem == null) return null;
            List<CFG_AppMenuesTranslation> results = new List<CFG_AppMenuesTranslation>();

            CFG_AppMenuesTranslation item = menuItem.CFG_AppMenuesTranslation.Where(i => i.IDLanguage.Trim() == langId).FirstOrDefault();
            return item;
        }
    }
}