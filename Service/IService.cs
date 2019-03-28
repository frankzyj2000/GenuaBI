using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.Entity;
using GenuinaBI.Models;

namespace GenuinaBI.Service
{
    public interface IService<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        TEntity GetById(object Id);
        TEntity Find(params object[] keyValues);
        IQueryable<TEntity> ExecQuery(string query, params object[] parameters);
        void Update(TEntity entity);
        void Delete(object Id);
        void Delete(TEntity entity);
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        List<TEntity> GetDataTableResultByPage(DataTableParameters param, List<TEntity> List);
        int GetSearchResultCount(DataTableParameters param, List<TEntity> list);
        IQueryable<TEntity> GetSearchResult(DataTableParameters param, List<TEntity> List);
        IQueryable<TEntity> Queryable();
        DbContext GetContext();
    }
}