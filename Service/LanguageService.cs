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
    public interface ILanguageervice : IService<CFG_Languages>
    {
        CFG_Languages GetLanguageByName(string LanguageName);
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class LanguageService : Service<CFG_Languages>, ILanguageervice
    {
        public LanguageService(DbContext db)
            : base(db)
        {
        }

        public LanguageService()
            : base()
        {
        }

        public override IEnumerable<CFG_Languages> GetAll() 
        {
            return base.GetAll().Where(i => i.IDLanguage.Trim() != "fr"); 
        }

        public CFG_Languages GetLanguageByName(string LanguageName)
        {
            return this.GetAll().Where(i => i.Description.Contains(LanguageName)).FirstOrDefault();
        }

        public CFG_Languages GetDefaultLanguage()
        {
            return this.GetAll().FirstOrDefault();
        }
    }
}