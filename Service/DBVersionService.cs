using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using GenuinaBI.Models;
using GenuinaBI.Configuration;

namespace GenuinaBI.Service
{
    /// <summary>
    ///     Add any custom business logic (methods) here
    /// </summary>
    public interface IDBVersionService : IService<DBVersionGnSy>
    {
        string GetDBVersion();
        bool IsDBVersionOK();
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class DBVersionService : Service<DBVersionGnSy>, IDBVersionService
    {
        public DBVersionService(DbContext db)
            : base(db)
        {
        }

        public DBVersionService()
            : base()
        {
        }

        private int GetMaxID()
        {
            return this.GetAll().OrderByDescending(i => i.ID).First().ID;
        }

        public string GetDBVersion()
        {
            return this.GetAll().Where(i => i.ID == GetMaxID()).FirstOrDefault().TextVersion;
        }

        public bool IsDBVersionOK()
        {
            string dbVersion = GetDBVersion();
            if (dbVersion.Replace(".", "").CompareTo(Config.MinimumDBVersion.Replace(".", "")) >= 0)
                return true;
            else
                return false;
        }
    }
}