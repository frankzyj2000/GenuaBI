using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Web.UI.WebControls;
using GenuinaBI.Models;
using GenuinaBI.Configuration;
namespace GenuinaBI.Service
{
    /// <summary>
    ///     Add any custom business logic (methods) here
    /// </summary>
    public interface IOperationSummaryService : IService<OperationSummaryST>
    {
        OperationSummaryModel GetOperationSummaryModel(OperationSummaryParameters param);
        IEnumerable<OperationSummaryProvider> GetOperationSummaryProviderList(DateTime start, DateTime end);
        IEnumerable<OperationSummaryTrendST> GetOperationSummaryTrendList(DateTime? day, int? numOfDay);
        IEnumerable<OperationSummaryTrend> GetOSTrendFor7Week(DateTime? day);
        IEnumerable<OperationSummaryTrend> GetOSTrendFor1Week(DateTime? day);
        decimal GetWinSlots();
        decimal GetNetWinCashIn();
        decimal GetNetWinCashOut();

        decimal GetTaxes();
        decimal GetPlayerAccountIn();
        decimal GetPlayerAccountOut();

        decimal GetPlayerAccountAmount();
        decimal GetSpecialPromos();
        decimal GetGrantedPromos();
        decimal GetCancelledPromos();
        decimal GetConsumedPromos();
        decimal GetOverShortPromos();

        decimal GetTotalMoneyIn();
        decimal GetTotalMoneyOut();
        decimal GetTotalDPromotion();
        decimal GetTotalHandPayments();
        decimal GetTotalNetWin();
        decimal GetTotalWin();

        int GetTotalCantPlayer();
        int GetTotalSessions();
        int GetTotalPlayers();

        int GetTotalSlots();
        int GetTotalSlotOccupied();

        decimal GetNetWinAfterTaxForTrend(OperationSummaryTrendST model);
        decimal GetWinSlotsForTrend(OperationSummaryTrendST model);
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class OperationSummaryService : Service<OperationSummaryST>, IOperationSummaryService
    {
        private IEnumerable<OperationSummaryProviderST> _providerList;
        private OperationSummaryST _operationSummary;

        public OperationSummaryService(DbContext db)
            : base(db)
        {
        }

        public OperationSummaryService()
            : base()
        {
        }

        public OperationSummaryModel GetOperationSummaryModel(OperationSummaryParameters param)
        {
            DateTime start = DateTime.ParseExact(param.Start, Config.CasinoDateTimeFormat, null);
            DateTime end = DateTime.ParseExact(param.End, Config.CasinoDateTimeFormat, null);

            OperationSummaryModel model = new OperationSummaryModel();
            model.ProviderList = this.GetOperationSummaryProviderList(start, end).ToList();
            model.WinSlots = GetWinSlots();
            model.NetWinCashIn = GetNetWinCashIn();
            model.NetWinCashOut = GetNetWinCashOut();
            model.Taxes = GetTaxes();
            model.PlayerAccountIn = GetPlayerAccountIn();
            model.PlayerAccountOut = GetPlayerAccountOut();

            model.PlayerAccountAmount = GetPlayerAccountAmount();
            model.SpecialPromos = GetSpecialPromos();
            model.GrantedPromos = GetGrantedPromos();
            model.CancelledPromos = GetCancelledPromos();

            model.ConsumedPromos = GetConsumedPromos();
            model.OverShortPromos = GetOverShortPromos();

            model.TotalMoneyIn = GetTotalMoneyIn();
            model.TotalMoneyOut = GetTotalMoneyOut();
            model.TotalHandPayments = GetTotalHandPayments();
            model.TotalDPromotion = GetTotalDPromotion();
            model.TotalNetWin = GetTotalNetWin();
            model.TotalWin = GetTotalWin();
            model.TotalCantPlayer = GetTotalCantPlayer();

            model.TotalSessions = GetTotalSessions();
            model.TotalPlayers = GetTotalPlayers();
            model.TotalSlots = GetTotalSlots();
            model.TotalSlotOccupied = GetTotalSlotOccupied();

            model.OSTrendFor1Week = GetOSTrendFor1Week(end).ToList();
            model.OSTrendFor7Week = GetOSTrendFor7Week(end).ToList();
            return model;
        }

        public IEnumerable<OperationSummaryProvider> GetOperationSummaryProviderList(DateTime start, DateTime end)
        {
            this._providerList = ((GenuinaDBEntities)base.GetContext()).GetOperationSummaryProviderList(start, end).ToList();
            this._operationSummary = ((GenuinaDBEntities)base.GetContext()).GetOperationSummaryList(start, end).FirstOrDefault();

            List<OperationSummaryProvider> list = new List<OperationSummaryProvider>();
            foreach (OperationSummaryProviderST provider in this._providerList)
            {
                list.Add(
                    new OperationSummaryProvider(
                        provider.Description,
                        provider.CashIN ??0,
                        provider.CashOut ??0,
                        provider.HandPayments ??0,
                        (provider.PROMO_IN ??0) - (provider.PROMO_OUT ??0),
                        (provider.CashIN ??0) + (provider.PROMO_IN ??0) - (provider.CashOut ??0) 
                            - (provider.PROMO_OUT ??0) - (provider.HandPayments ??0),
                        (provider.CashIN ??0 - provider.CashOut ??0) - (provider.HandPayments ??0),
                        (int)provider.CantPlayer
                    )
                );
            }
            return list;
        }

        /*Get List for jQueryDataTable*/
        public virtual List<OperationSummaryProvider> GetDataTableResultByPage(DataTableParameters param, List<OperationSummaryProvider> list)
        {
            if (param.Length == -1)
            {
                return GetSearchResult(param, list).SortBy(param.SortOrder).ToList();
            }
            else
            {
                return GetSearchResult(param, list).SortBy(param.SortOrder).Skip(param.Start).Take(param.Length).ToList();
            }
        }

        public virtual int GetSearchResultCount(DataTableParameters param, List<OperationSummaryProvider> list)
        {
            return GetSearchResult(param, list).ToList().Count;
        }

        //Search based on param.Search only. 
        //TODO: can add each search for individual columns
        public virtual IQueryable<OperationSummaryProvider> GetSearchResult(DataTableParameters param, List<OperationSummaryProvider> list)
        {
            string search = param.Search.Value;
            return list.AsQueryable().Where(p => (search == null
                || p.Provider != null && p.Provider.ToLower().Contains(search.ToLower())));
        }

        public decimal GetWinSlots()
        {
            if (this._providerList == null)
            {
                return 0;
            }
            else
            {
                return this._providerList.Sum(i => i.CashIN ??0) - this._providerList.Sum(i => i.CashOut ??0) 
                    - this._providerList.Sum(i => i.HandPayments ??0);
            }        
        }

        public decimal GetNetWinCashIn()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.AmountIn ??0);
            }
        }

        public decimal GetNetWinCashOut()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.AmountOut ??0);
            }
        }

        public decimal GetTaxes()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.Taxes ??0);
            }
        }

        public decimal GetPlayerAccountIn()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.PlayerAccountIN ??0);
            }
        }

        public decimal GetPlayerAccountOut()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.PlayerAccountOUT ??0);
            }
        }

        public decimal GetPlayerAccountAmount()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.AmountPlayerAccountVariation ??0);
            }
        }

        public decimal GetSpecialPromos()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.EspecialPromo ??0);
            }
        }

        public decimal GetGrantedPromos()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.Promociones ??0);
            }
        }

        public decimal GetCancelledPromos()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.CanceledPromo ??0);
            }
        }

        public decimal GetConsumedPromos()
        {
            if (this._providerList == null)
            {
                return 0;
            }
            else
            {
                return (this._providerList.Sum(i => i.PROMO_IN ??0) - this._providerList.Sum(i => i.PROMO_OUT ??0));
            }
        }

        public decimal GetOverShortPromos()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                return (this._operationSummary.Promociones ??0) 
                    - (this._operationSummary.CanceledPromo ??0) - this.GetConsumedPromos();
            }
        }

        public decimal GetTotalMoneyIn()
        {
            if (this._providerList == null)
            {
                return 0;
            }
            else
            {
                return this._providerList.Sum(i => i.CashIN ??0);
            }
        }

        public decimal GetTotalMoneyOut()
        {
            if (this._providerList == null)
            {
                return 0;
            }
            else
            {
                return this._providerList.Sum(i => i.CashOut ??0);
            }
        }

        public decimal GetTotalDPromotion()
        {
            return this.GetConsumedPromos();
        }

        public decimal GetTotalHandPayments()
        {
            if (this._providerList == null)
            {
                return 0;
            }
            else
            {
                return (this._providerList.Sum(i => i.HandPayments ?? 0));
            }
        }

        public int GetTotalCantPlayer()
        {
            if (this._providerList == null)
            {
                return 0;
            }
            else
            {
                return (int) this._providerList.Sum(i => i.CantPlayer ?? 0);
            }       
        }

        public decimal GetTotalNetWin() 
        {
            if (this._providerList == null)
            {
                return 0;
            }
            else
            {
                return this._providerList.Sum( i => (i.CashIN ??0) + (i.PROMO_IN ??0) - (i.CashOut ??0) - (i.PROMO_OUT ??0) - (i.HandPayments ??0) );
            }
        }

        public decimal GetTotalWin()
        {
            if (this._providerList == null)
            {
                return 0;
            }
            else
            {
                return this._providerList.Sum( i => (i.CashIN ??0 - i.CashOut ??0) - (i.HandPayments ??0) );
            }
        }

        public int GetTotalSessions()
        {
            if (this._providerList == null)
            {
                return 0;
            }
            else
            {
                return (int) this._providerList.Sum(i => i.Sesiones ??0);
            }
        }

        public int GetTotalPlayers()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                if (this._operationSummary.OcupacionPeriodo == null)
                    return 0;
                else
                    return (int) this._operationSummary.OcupacionPeriodo;
            }
        }

        public int GetTotalSlots()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                if (this._operationSummary.TotalSlots == null)
                    return 0;
                else
                    return (int)this._operationSummary.TotalSlots;
            }
        }

        public int GetTotalSlotOccupied()
        {
            if (this._operationSummary == null)
            {
                return 0;
            }
            else
            {
                if (this._operationSummary.TotalSlotsOcuped == null)
                    return 0;
                else
                    return (int)this._operationSummary.TotalSlotsOcuped;
            }
        }

        public IEnumerable<OperationSummaryTrendST> GetOperationSummaryTrendList(DateTime? day, int? numOfDay)
        {
            return ((GenuinaDBEntities)base.GetContext()).GetOperationSummaryTrendList(day, Config.CasinoStartTime, Config.CasinoEndTime, numOfDay);
        }

        public IEnumerable<OperationSummaryTrend> GetOSTrendFor1Week(DateTime? day)
        {
            List<OperationSummaryTrend> list = new List<OperationSummaryTrend>();
            foreach (OperationSummaryTrendST model in this.GetOperationSummaryTrendList(day, 7))
            {
                list.Add(new OperationSummaryTrend
                    (
                        (model.StartDateTime ?? DateTime.MinValue).Date.ToString(Config.CasinoDateFormat),
                        this.GetNetWinAfterTaxForTrend(model),
                        this.GetWinSlotsForTrend(model)
                    )
                );
            }
            return list;
        }

        public IEnumerable<OperationSummaryTrend> GetOSTrendFor7Week(DateTime? day)
        {
            //only get the 7th, 14th, 28th, ... days before the day
            List<OperationSummaryTrend> list = new List<OperationSummaryTrend>();
            foreach (OperationSummaryTrendST model in this.GetOperationSummaryTrendList(day, 49).Where(c => c.DayDifference % 7 == 0))
            {
                list.Add(new OperationSummaryTrend
                    (
                        (model.StartDateTime ?? DateTime.MinValue).Date.ToString(Config.CasinoDateFormat), 
                        this.GetNetWinAfterTaxForTrend(model), 
                        this.GetWinSlotsForTrend(model)
                    )
                );
            }
            return list;
        }

        public decimal GetNetWinAfterTaxForTrend(OperationSummaryTrendST model)
        {
            return (model.HardSoftCountBill ??0) + (model.AmountIn ??0) - (model.AmountOut ??0) - (model.Taxes ??0);
        }

        public decimal GetWinSlotsForTrend(OperationSummaryTrendST model)
        {
            return (model.CashIn ??0) - (model.CashOut ??0) - (model.HandPayments ??0) - (model.Jackpots ??0);
        }
        /*
        public static decimal GetOperationSummaryOrderTotalByYear(this IRepository<OperationSummaryST> repository, string OperationSummaryId, int year)
        {
            return repository
                .Queryable()
                .Where(c => c.OperationSummaryID == OperationSummaryId)
                .SelectMany(c => c.Orders.Where(o => o.OrderDate != null && o.OrderDate.Value.Year == year))
                .SelectMany(c => c.OrderDetails)
                .Select(c => c.Quantity*c.UnitPrice)
                .Sum();
        }


        public static IEnumerable<OperationSummaryOrder> GetOperationSummaryOrder(this IRepository<OperationSummaryST> repository, string country)
        {
            var OperationSummarys = repository.GetRepository<OperationSummaryST>().Queryable();
            var orders = repository.GetRepository<Order>().Queryable();

            var query = from c in OperationSummarys
                join o in orders on new {a = c.OperationSummaryID, b = c.Country}
                    equals new {a = o.OperationSummaryID, b = country}
                select new OperationSummaryOrder
                {
                    OperationSummaryId = c.OperationSummaryID,
                    ContactName = c.ContactName,
                    OrderId = o.OrderID,
                    OrderDate = o.OrderDate
                };

            return query.AsEnumerable();
        }*/
    }
}