using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GenuinaBI.Models;
using GenuinaBI.Service;
using GenuinaBI.Attributes;
using GenuinaBI.Configuration;

namespace GenuinaBI.Controllers
{
    /// <summary>
    /// This is an example controller using the AdminLTE NuGet package's CSHTML templates, CSS, and JavaScript
    /// You can delete these, or use them as handy references when building your own applications
    /// </summary>
    [Authorize]
    public class MarketingController : BaseController
    {
        /// <summary>
        /// The home page of the GenuinaBI dashboard ver 2, recreated in this new system
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            using (UserService _service = new UserService())
            {
                if (_service.CanAccessPage(Constants.Pages.PlayerSearch, base.GetCurrentUserId()))
                {
                    return RedirectToAction(Constants.Pages.PlayerSearch);
                }
                else
                {
                    return RedirectToAction("AccessDenied", "Error");
                }
            }
        }

        /// <summary>
        /// The home page of the GenuinaBI dashboard, recreated in this new system
        /// </summary>
        /// <returns></returns>
        public ActionResult Marketing()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// The SlotOccupation page of the GenuinaBI
        /// </summary>
        /// <returns></returns>
        [PageAuthorize(Constants.Pages.PlayerSearch)]
        public ActionResult PlayerSearch()
        {
            using (UIChartService _service = new UIChartService())
            {
                //IQueryParamaters parameter = base.GetReportParameters(Constants.Cookies.SlotOccupationParameters); //get parameter from cookie
                IQueryParamaters parameter = new PlayerSearchParameters();
                return View(
                    _service.GetUIChartModel(Constants.Pages.PlayerSearch, base.GetCurrentUserId(), base.GetCurrentCulture(), parameter)
                );
            }
        }

        [PageAuthorize(Constants.Pages.PlayerSearch)]
        public JsonResult GetPlayerSearch(string playerId, string end)
        {
            try
            {
                if (playerId == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    PlayerDetailParameters param = new PlayerDetailParameters();
                    param.PlayerID = playerId;
                    param.End = end;
                    //base.SaveReportParameters(param); //Save parameter to cookie
                    using (PlayerSearchService _service = new PlayerSearchService())
                    {
                        PlayerSearchModel model = _service.GetPlayerSearchModel(param);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DataTableHandler was used to accept Sorting/Pagination/Filter request from client jQuery DataTable control
        /// </summary>
        /// <returns></returns>
        [PageAuthorize(Constants.Pages.PlayerSearch)]
        public JsonResult PlayerSearchDataTableHandler(DataTableParameters param,  PlayerDetailParameters queryParamters)
        {
            try
            {
                using (PlayerSearchService _service = new PlayerSearchService())
                {
                    List<MarketingPlayerST> mkList = _service.GetMKPlayerList(queryParamters.PlayerID).ToList();
                    return Json(new DataTableResult<MarketingPlayerST>
                        (
                            param.Draw,
                            _service.GetSearchResultCount(param, mkList),
                            _service.GetDataTableResultByPage(param, mkList)
                        ), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}