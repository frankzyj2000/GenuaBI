using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using GenuinaBI.Models;
using System.Web.UI.WebControls;
using GenuinaBI.Configuration;

namespace GenuinaBI.Service
{
    /// <summary>
    ///     Add any custom business logic (methods) here
    /// </summary>
    public interface ITopPlayerService : IService<TopPlayerInfoST>
    {
        TopPlayerModel GetTopPlayerModel(TopPlayerParameters param);
        IEnumerable<TopPlayerInfoST> GetTopPlayerList(TopPlayerParameters param);
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class TopPlayerService : Service<TopPlayerInfoST>, ITopPlayerService
    {
        private List<TopPlayerInfoST> _topPlayList;

        public TopPlayerService(DbContext db)
            : base(db)
        {
        }

        public TopPlayerService()
            : base()
        {
        }

        public TopPlayerModel GetTopPlayerModel(TopPlayerParameters param)
        {
            TopPlayerModel model = new TopPlayerModel();
            model.TopPlayerList = this.GetTopPlayerList(param).ToList();
            return model;
        }

        public IEnumerable<TopPlayerInfoST> GetTopPlayerList(TopPlayerParameters param)
        {
            DateTime start = DateTime.ParseExact(param.Start, Config.CasinoDateTimeFormat, null);
            DateTime end = DateTime.ParseExact(param.End, Config.CasinoDateTimeFormat, null);
            this._topPlayList = ((GenuinaDBEntities)base.GetContext()).GetTopPlayerList(start, end, param.NumberOfPlayers, param.NumberOfVisits).ToList();
            return this._topPlayList;
        }

        //following 3 functions are used to Handle DataTable control for store procedure based object
        public override List<TopPlayerInfoST> GetDataTableResultByPage(DataTableParameters param, List<TopPlayerInfoST> list)
        {
            if (param.Length == -1 )
            {
                return GetSearchResult(param, list).SortBy(param.SortOrder).ToList();
            }
            else
            {
                return GetSearchResult(param, list).SortBy(param.SortOrder).Skip(param.Start).Take(param.Length).ToList();
            }
        }

        public override int GetSearchResultCount(DataTableParameters param, List<TopPlayerInfoST> list)
        {
            return GetSearchResult(param, list).ToList().Count;
        }

        //Search based on param.Search only. 
        //TODO: can add each search for individual columns
        public override IQueryable<TopPlayerInfoST> GetSearchResult(DataTableParameters param, List<TopPlayerInfoST> list) 
        {
            string search = param.Search.Value;
            return list.AsQueryable().Where(p => (search == null 
                || p.PlayerName != null && p.PlayerName.ToLower().Contains(search.ToLower()) 
                || p.Telofono != null && p.Telofono.ToLower().Contains(search.ToLower()) 
                || p.Email != null && p.Email.ToLower().Contains(search.ToLower()) 
                || p.Celular != null && p.Celular.ToLower().Contains(search.ToLower()) )); 
        }
    }
}