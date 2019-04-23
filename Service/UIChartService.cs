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
    public interface IUIChartService : IService<GBI_CFG_DashboardCharts>
    {
        IEnumerable<GBI_CFG_DashboardCharts> GetUIChartByGroupName(string groupname);
        UIChartModel GetUIChartModel(string groupname, string userId, string langId, IQueryParamaters parameter);
    }

    /// <summary>
    ///     All methods that are exposed from Repository in Service are overridable to add business logic,
    ///     business logic should be in the Service layer and not in repository for separation of concerns.
    /// </summary>
    public class UIChartService : Service<GBI_CFG_DashboardCharts>, IUIChartService
    {
        public UIChartService(DbContext db)
            : base(db)
        {
        }

        public UIChartService()
            : base()
        {
        }

        public IEnumerable<GBI_CFG_DashboardCharts> GetUIChartByGroupName(string groupname)
        {
            return this.GetAll().Where(i => i.ChartGroupName == groupname);
        }

        private string GetDefaultStartDate(IQueryParamaters parameter)
        {
            string start = String.Empty;
            if (parameter is TopPlayerParameters)
            {
                start = (new TopPlayerParameters()).Start;
            }
            else if (parameter is OperationSummaryParameters)
            {
                start = (new OperationSummaryParameters()).Start;
            }
            else if (parameter is SlotOccupationParameters)
            {
                start = (new SlotOccupationParameters()).Start;
            }
            return start;
        }

        private string GetDefaultEndDate(IQueryParamaters parameter)
        {
            string end = String.Empty;
            if (parameter is TopPlayerParameters)
            {
                end = (new TopPlayerParameters()).End;
            }
            else if (parameter is OperationSummaryParameters)
            {
                end = (new OperationSummaryParameters()).End;
            }
            return end;
        }

        public UIChartModel GetUIChartModel(string groupname, string userId, string langId, IQueryParamaters parameter)
        {
            UIChartModel model = new UIChartModel();
            model.UIChartList = this.GetUIChartByGroupName(groupname).ToList();
            model.QueryParameter = parameter;

            using (LanguageService _service = new LanguageService())
            {
                model.LanguageList = _service.GetAll().ToList();
            }
            using (UserService _service = new UserService())
            {
                model.MenuList = _service.GetAppMenusTranslation(userId, langId);
                model.AllMenuItemList = _service.GetAllMenuItems(userId);
                model.AllMenuItemTranslationList = _service.GetAllMenuItemsTranslation(userId, langId);
            }

            model.DefaultStartDate = GetDefaultStartDate(parameter);
            model.DefaultEndDate = GetDefaultEndDate(parameter);

            model.IsControlButtonDark = false; //control button disiplayed with normal color
            model.OperationSummaryServerCache = Config.OperationSummaryServerCache;
            model.SlotOccupationServerCache = Config.SlotOccupationServerCache;
            model.TopPlayersServerCache = Config.TopPlayersServerCache;
            model.PlayerSearchServerCache = Config.PlayerSearchServerCache;
            return model;
        }
        
    }
}