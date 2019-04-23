using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.SqlTypes;
using GenuinaBI.Models;
using System.Web.UI.WebControls;
using GenuinaBI.Configuration;

namespace GenuinaBI.Service
{
    /// <summary>
    ///     Add any custom business logic (methods) here
    /// </summary>
    public interface IPlayerSearchService : IService<MarketingPlayerST>
    {
        PlayerSearchModel GetPlayerSearchModel(PlayerDetailParameters param);    
        IEnumerable<MarketingPlayerST> GetMKPlayerList(string playerID);
        IEnumerable<MarketingPlayerTrendsST> GetMKPlayerTrendList(string playerID, DateTime start, DateTime end);
        IEnumerable<MarketingPlayerReferencesST> GetMKPlayerReferenceList(string playerID, DateTime start, DateTime end);
        IEnumerable<MarketingPlayerActivityST> GetMKPlayerActivityList(string playerID, DateTime start, DateTime end);
        IEnumerable<MarketingPlayerCardsST> GetMKPlayerCardList(string playerID);
        IEnumerable<MarketingPlayerGameHistoryST> GetMKPlayerGameHistoryList(string playerID);
        IEnumerable<MarketingPlayerPromoST> GetMKPlayerPromotionList(string playerID);
        IEnumerable<MarketingPlayerCashDeskST> GetMKPlayerCashDeskList(string playerID);
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class PlayerSearchService : Service<MarketingPlayerST>, IPlayerSearchService
    {
        public PlayerSearchService(DbContext db)
            : base(db)
        {
        }

        public PlayerSearchService()
            : base()
        {
        }

        public PlayerSearchModel GetPlayerSearchModel(PlayerDetailParameters param)
        {
            PlayerSearchModel model = new PlayerSearchModel();
            DateTime end = new DateTime();
            if (param.End == null)
            {
                end = DateTime.Now;
            }
            else
            {
                end = DateTime.ParseExact(param.End, Config.CasinoDateTimeFormat, null);
            }

            model.MKPlayer = this.GetMKPlayerList(param.PlayerID).FirstOrDefault();
            DateTime start = end.AddDays(-1);
            model.MKPlayerReferenceTodayList = this.GetMKPlayerReferenceList(param.PlayerID, start, end).ToList();
            model.MKPlayerActivityTodayList = this.GetMKPlayerActivityList(param.PlayerID, start, end).ToList();
            start = end.AddMonths(-1);
            model.MKPlayerReferenceMonthList = this.GetMKPlayerReferenceList(param.PlayerID, start, end).ToList();
            model.MKPlayerActivityMonthList = this.GetMKPlayerActivityList(param.PlayerID, start, end).ToList();
            start = end.AddMonths(-3);
            model.MKPlayerTrendList = this.GetMKPlayerTrendList(param.PlayerID, start, end).ToList();
            model.MKPlayerReferenceQuarterList = this.GetMKPlayerReferenceList(param.PlayerID, start, end).ToList();
            model.MKPlayerActivityQuarterList = this.GetMKPlayerActivityList(param.PlayerID, start, end).ToList();
            start = (DateTime) SqlDateTime.MinValue; //must use SqlDateTime to get valid Datetime
            end = (DateTime) SqlDateTime.MaxValue;
            model.MKPlayerReferenceAllList = this.GetMKPlayerReferenceList(param.PlayerID, start, end).ToList();

            model.MKPlayerCardList = this.GetMKPlayerCardList(param.PlayerID).ToList();
            model.MKPlayerGameHistoryList = this.GetMKPlayerGameHistoryList(param.PlayerID).ToList();
            model.MKPlayerPromotionList = this.GetMKPlayerPromotionList(param.PlayerID).ToList();
            model.MKPlayerCashDeskList = this.GetMKPlayerCashDeskList(param.PlayerID).ToList();
            return model;
        }

        public IEnumerable<MarketingPlayerST> GetMKPlayerList(string playerID)
        {
            return ((GenuinaDBEntities)base.GetContext()).GetMarketingPlayer(playerID).ToList();
        }

        public IEnumerable<MarketingPlayerTrendsST> GetMKPlayerTrendList(string playerID, DateTime start, DateTime end)
        {  
            return ((GenuinaDBEntities)base.GetContext()).GetMarketingPlayerTrends(playerID, start, end).ToList();
        }

        public IEnumerable<MarketingPlayerReferencesST> GetMKPlayerReferenceList(string playerID, DateTime start, DateTime end)
        {
            return ((GenuinaDBEntities)base.GetContext()).GetMarketingPlayerReferences(playerID, start, end).ToList();
        }

        public IEnumerable<MarketingPlayerActivityST> GetMKPlayerActivityList(string playerID, DateTime start, DateTime end)
        {
            return ((GenuinaDBEntities)base.GetContext()).GetMarketingPlayerActivity(playerID, start, end).ToList();
        }

        public IEnumerable<MarketingPlayerCardsST> GetMKPlayerCardList(string playerID)
        {
            return ((GenuinaDBEntities)base.GetContext()).GetMarketingPlayerCards(playerID).ToList();
        }

        public IEnumerable<MarketingPlayerGameHistoryST> GetMKPlayerGameHistoryList(string playerID)
        {
            return ((GenuinaDBEntities)base.GetContext()).GetMarketingPlayerGameHistory(playerID).ToList();
        }

        public IEnumerable<MarketingPlayerPromoST> GetMKPlayerPromotionList(string playerID)
        {
            return ((GenuinaDBEntities)base.GetContext()).GetMarketingPlayerPromo(playerID).ToList();
        }

        public IEnumerable<MarketingPlayerCashDeskST> GetMKPlayerCashDeskList(string playerID)
        {
            return ((GenuinaDBEntities)base.GetContext()).GetMarketingPlayerCashDesk(playerID).ToList();
        }

        public string GetMarketingPlayerID(PlayerSearchParameters param)
        {
            string playerID = "";
            if (param.SlotMachine != null && param.SlotMachine.Length > 0)
            {
                playerID = this.GetPlayerIDBySlotMachine(param.SlotMachine);
            }
            else if (param.PlayerName != null && param.PlayerName.Length > 0)
            {
                playerID = this.GetPlayerIDByPlayerName(param.PlayerName);
            }
            else if (param.CardNumber != null && param.CardNumber.Length > 0)
            {
                playerID = this.GetPlayerIDByCardNumber(param.CardNumber);
            };
            return playerID;
        }

        private string GetPlayerIDBySlotMachine(string slotMachine)
        {
            return slotMachine;
        }

        private string GetPlayerIDByPlayerName(string playerName)
        {
            return playerName;
        }

        private string GetPlayerIDByCardNumber(string cardNumber)
        {
            return cardNumber;
        }

        //following 3 functions are used to Handle DataTable control for store procedure based object
        public override List<MarketingPlayerST> GetDataTableResultByPage(DataTableParameters param, List<MarketingPlayerST> list)
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

        public override int GetSearchResultCount(DataTableParameters param, List<MarketingPlayerST> list)
        {
            return GetSearchResult(param, list).ToList().Count;
        }

        //Search based on param.Search only. 
        //TODO: can add each search for individual columns
        //public override IQueryable<MarketingPlayerST> GetSearchResult(DataTableParameters param, List<MarketingPlayerST> list) 
        //{
        //    string search = param.Search.Value;
        //    return list.AsQueryable().Where(p => (search == null 
        //        || p.PlayerName != null && p.PlayerName.ToLower().Contains(search.ToLower()) 
        //        || p.Telofono != null && p.Telofono.ToLower().Contains(search.ToLower()) 
        //        || p.Email != null && p.Email.ToLower().Contains(search.ToLower()) 
        //        || p.Celular != null && p.Celular.ToLower().Contains(search.ToLower()) )); 
        //}
    }
}