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
    public interface ISlotOccupationService : IService<SlotOccupationST>
    {
        DateTime? getDateRangeStartTime();
        DateTime? getDateRangeEndTime();
        int GetTotalSlots();
        IEnumerable<SlotOccupationST> GetSlotOccupationList(SlotOccupationParameters param);
        decimal GetTotalWin();
        decimal GetTotalHandle();
        int GetTotalPlayers();
        int GetTotalSlotsOccupied();
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class SlotOccupationService : Service<SlotOccupationST>, ISlotOccupationService
    {
        private List<SlotOccupationST> _occupationList;

        public SlotOccupationService(DbContext db)
            : base(db)
        {
        }

        public SlotOccupationService()
            : base()
        {
        }

        public SlotOccupationModel GetSlotOccupationModel(SlotOccupationParameters param)
        {
            SlotOccupationModel model = new SlotOccupationModel();
            model.SlotOccupationList = GetSlotOccupationList(param).ToList();
            DateTime? date = getDateRangeStartTime();
            if (date == null) {
                model.StartTime = "";
            }
            else {
                model.StartTime = date.Value.ToString(Config.CasinoDateTimeFormat);
            }
            date = getDateRangeEndTime();
            if (date == null)
            {
                model.EndTime = "";
            }
            else
            {
                model.EndTime = date.Value.ToString(Config.CasinoDateTimeFormat);
            }

            model.TotalSlots = GetTotalSlots();
            model.TotalWin = GetTotalWin();
            model.TotalHandle = GetTotalHandle();
            model.TotalPlayers = GetTotalPlayers();
            model.TotalSlotsOccupied = GetTotalSlotsOccupied();
            return model;
        }

        public IEnumerable<SlotOccupationST> GetSlotOccupationList(SlotOccupationParameters param)
        {
            this._occupationList = new List<SlotOccupationST>();
            var date = DateTime.ParseExact(param.Start, Config.CasinoDateFormat, null).ToString("yyyyMMdd"); //required by the store procedure
            foreach (SlotOccupationST st in ((GenuinaDBEntities)base.GetContext()).GetSlotOccupationList(date).ToList())
            {
                this._occupationList.Add(st);
            }
            return _occupationList;
        }


        /*Get List for jQueryDataTable*/
        public override List<SlotOccupationST> GetDataTableResultByPage(DataTableParameters param, List<SlotOccupationST> list)
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

        public override int GetSearchResultCount(DataTableParameters param, List<SlotOccupationST> list)
        {
            return GetSearchResult(param, list).ToList().Count;
        }

        //Search based on param.Search only. 
        //TODO: can add each search for individual columns
        public override IQueryable<SlotOccupationST> GetSearchResult(DataTableParameters param, List<SlotOccupationST> list)
        {
            string search = param.Search.Value;
            return list.AsQueryable();
        }

        public DateTime? getDateRangeStartTime()
        { 
            if (this._occupationList.Count <= 0)
            {
                return null;
            }
            else
            {
                return this._occupationList[0].DateTimeStart;
            }   
        }

        public DateTime? getDateRangeEndTime()
        {
            if (this._occupationList.Count <= 0)
            {
                return null;
            }
            else
            {
                int size = this._occupationList.Count() - 1;
                if (size == -1) size++;
                return this._occupationList[size].DateTimeEnd;
            }   
        }

        public int GetTotalSlots()
        {
            return ((GenuinaDBEntities)base.GetContext()).GetTotalSlots(this.getDateRangeStartTime(), this.getDateRangeEndTime()).FirstOrDefault() ?? 0;
        }

        public decimal GetTotalWin()
        {
            if (this._occupationList.Count <= 0)
            {
                return 0;
            }
            else
            {
                return this._occupationList.Sum(i => i.WinLoss ??0);
            }
        }

        public decimal GetTotalHandle()
        {
            if (this._occupationList.Count <= 0)
            {
                return 0;
            }
            else
            {
                return this._occupationList.Sum(i => i.Handle ??0);
            }
        }

        public int GetTotalPlayers()
        {
            if (this._occupationList.Count <= 0)
            {
                return 0;
            }
            else
            {
                return (int) this._occupationList.FirstOrDefault().TotalPlayers;
            }
        }

        public int GetTotalSlotsOccupied()
        {
            if (this._occupationList.Count <= 0)
            {
                return 0;
            }
            else
            {
                return (int)this._occupationList.FirstOrDefault().TotalSlotsOcuped;
            }
        }

        /*
        public static decimal GetSlotOccupationOrderTotalByYear(this IRepository<SlotOccupationST> repository, string SlotOccupationId, int year)
        {
            return repository
                .Queryable()
                .Where(c => c.SlotOccupationID == SlotOccupationId)
                .SelectMany(c => c.Orders.Where(o => o.OrderDate != null && o.OrderDate.Value.Year == year))
                .SelectMany(c => c.OrderDetails)
                .Select(c => c.Quantity*c.UnitPrice)
                .Sum();
        }


        public static IEnumerable<SlotOccupationOrder> GetSlotOccupationOrder(this IRepository<SlotOccupationST> repository, string country)
        {
            var SlotOccupations = repository.GetRepository<SlotOccupationST>().Queryable();
            var orders = repository.GetRepository<Order>().Queryable();

            var query = from c in SlotOccupations
                join o in orders on new {a = c.SlotOccupationID, b = c.Country}
                    equals new {a = o.SlotOccupationID, b = country}
                select new SlotOccupationOrder
                {
                    SlotOccupationId = c.SlotOccupationID,
                    ContactName = c.ContactName,
                    OrderId = o.OrderID,
                    OrderDate = o.OrderDate
                };

            return query.AsEnumerable();
        }*/
    }
}