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
    public interface ICasinoService : IService<CFG_Casinos>
    {
        CFG_Casinos GetCasinoByName(string CasinoName);
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class CasinoService : Service<CFG_Casinos>, ICasinoService
    {
        public CasinoService(DbContext db)
            : base(db)
        {
        }

        public CasinoService()
            : base()
        {
        }

        public CFG_Casinos GetCasinoByName(string casinoName)
        {
            return this.GetAll().Where(i => i.Description.Contains(casinoName)).FirstOrDefault();
        }

        public CFG_Casinos GetDefaultCasino()
        {
            return this.GetAll().FirstOrDefault();
        }
    }
}