using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.Entity;
using GenuinaBI.Models;
using System.Web.UI.WebControls;
using System.Configuration;
using log4net;

namespace GenuinaBI.Service
{
    public abstract class Service<TEntity> : IService<TEntity>, IDisposable where TEntity : class
    {
        #region Private Fields
        private ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly DbContext _db;
        #endregion Private Fields

        #region Constructor
        public Service(DbContext db) 
        { 
            _db = db;
        }
        public Service()
        {
           _db = new GenuinaDBEntities((String)HttpContext.Current.Application["dbConnectionString"]);
        }
        #endregion Constructor

        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _db.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        public virtual DbContext GetContext()
        {
            return _db;
        }

        public virtual TEntity GetById(object Id) { return _db.Set<TEntity>().Find(Id); }

        public virtual IEnumerable<TEntity> GetAll() { return _db.Set<TEntity>().ToList(); }

        public virtual TEntity Find(params object[] keyValues) { return _db.Set<TEntity>().Find(keyValues); }

        public virtual IQueryable<TEntity> ExecQuery(string query, params object[] parameters) 
        {
            return _db.Set<TEntity>().SqlQuery(query, parameters).AsQueryable();
        }

        public virtual void Insert(TEntity entity) { _db.Set<TEntity>().Add(entity); }

        public virtual void InsertRange(IEnumerable<TEntity> entities) 
        {
            foreach (TEntity entity in entities)
            {
                _db.Set<TEntity>().Add(entity);
            }
        }

        public virtual void Update(TEntity entity) 
        {
            _db.Set<TEntity>().Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(object Id) 
        {
            var entity = _db.Set<TEntity>().Find(Id);
            if (entity != null)
            {
                _db.Entry(entity).State = EntityState.Deleted;
                _db.Set<TEntity>().Remove(entity); 

            }
        }

        public virtual void Delete(TEntity entity) 
        {
            _db.Set<TEntity>().Attach(entity);
            _db.Set<TEntity>().Remove(entity); 
        }

        public virtual void Delete(params object[] keys)
        {
            var entity = _db.Set<TEntity>().Find(keys);
            if (entity != null)
                _db.Set<TEntity>().Remove(entity);
        }

        public IQueryable<TEntity> Queryable() { return _db.Set<TEntity>(); }

        /* The Get method handles fetching data. It handles querying the data supporting a filtering, ordering, paging, 
         * and eager loading of child types, so that we can make one round trip and eager load the entity graph.
         * Note the method is marked “internal” because we only want the method to be accessible to objects with the same assembly, Repository.dll.
         */
        internal IQueryable<TEntity> Select(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (filter != null)
            {
                query = query.AsExpandable().Where(filter);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }
        //each service should have their own overided GetDataTableResult();
        public virtual List<TEntity> GetDataTableResultByPage(DataTableParameters param, List<TEntity> list)
        {
            return list;
        }

        public virtual int GetSearchResultCount(DataTableParameters param, List<TEntity> list)
        {
            return list.Count;
        }

        //each service should have their own overided GetSearchResult()
        public virtual IQueryable<TEntity>  GetSearchResult(DataTableParameters param, List<TEntity> list) 
        {
            return list.AsQueryable(); //default is do nothing
        }
    }
    /*
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly IUnitOfWork _uow;

        public Repository(IDbContext db, IUnitOfWork uow)
        {
            _context = db;
            _dbSet = ((DbContext)_context).Set<TEntity>();
            _uow = uow;
        }

        public virtual DbContext GetContext()
        {
            return _context;
        }

        public IRepository<T> GetRepository<T>() where T : class, IObjectState
        {
            return _uow.Repository<T>();
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual TEntity GetById(object Id)
        {
            return _dbSet.Find(Id);
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Insert(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(object Id)
        {
            var entity = _dbSet.Find(Id);
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Deleted;
                Delete(entity);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public virtual IQueryable<TEntity> ExecQuery(string query, params object[] parameters)
        {
            return _dbSet.SqlQuery(query, parameters).AsQueryable();
        }

        public IQueryable<TEntity> Queryable()
        {
            return _dbSet;
        }

        // The Get method handles fetching data. It handles querying the data supporting a filtering, ordering, paging, 
        // and eager loading of child types, so that we can make one round trip and eager load the entity graph.
        // Note the method is marked “internal” because we only want the method to be accessible to objects with the same assembly, Repository.dll.
        //
        internal IQueryable<TEntity> Select(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (filter != null)
            {
                query = query.AsExpandable().Where(filter);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }
    }
         /// <summary>
    /// The Entity Framework implementation of IUnitOfWork
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _context;
        private bool _disposed;
        private Dictionary<string, dynamic> _repositories; //Applied Generic Dictionary instead of Hashtable 
 
        public UnitOfWork()
        {
            _context = new GenuinaDBEntities();
            _repositories = new Dictionary<string, dynamic>();
        }

        public UnitOfWork(IDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<string, dynamic>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
 
        public void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
                 _context.Dispose();
 
            _disposed = true;
        }

        // We are storing all activated instances of repositories for each and every requests.
        // Once there is a request for a given repository we will first check our Hashtable to see if it has been created, 
        // Next, we’ll scan our container to see if the requested repository instance has already been created, 
        // if it has, then will return it, if it hasn’t, we will activate the requested repository instance, 
        // store it in our container, and then return it.
       // So we are only creating repository instances on demand, this allows us to create the repository instances required
        // for a given web request.
         //
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            //container to hold all of our activated repository instances
            if (_repositories == null)
                _repositories = new Dictionary<string, dynamic>();
 
            var type = typeof (TEntity).Name;
 
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof (Repository<>);
 
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof (TEntity)), _context);
                 
                _repositories.Add(type, repositoryInstance);
            }
 
            return _repositories[type];
        }
    }
     */
}
