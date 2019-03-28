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
    public class DashboardController : BaseController
    {
        /// <summary>
        /// The home page of the GenuinaBI dashboard ver 2, recreated in this new system
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            using (UserService _service = new UserService())
            {
                if (_service.CanAccessPage(Constants.Pages.OperationSummary, base.GetCurrentUserId()))
                {
                    return RedirectToAction(Constants.Pages.OperationSummary);
                }
                else if (_service.CanAccessPage(Constants.Pages.SlotOccupation, base.GetCurrentUserId()))
                {
                    return RedirectToAction(Constants.Pages.SlotOccupation);
                }
                else if (_service.CanAccessPage(Constants.Pages.TopPlayers, base.GetCurrentUserId()))
                {
                    return RedirectToAction(Constants.Pages.TopPlayers);
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
        public ActionResult Dashboard()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// The SlotOccupation page of the GenuinaBI
        /// </summary>
        /// <returns></returns>
        [PageAuthorize(Constants.Pages.SlotOccupation)]
        public ActionResult SlotOccupation()
        {
            using (UIChartService _service = new UIChartService())
            {
                //IQueryParamaters parameter = base.GetReportParameters(Constants.Cookies.SlotOccupationParameters); //get parameter from cookie
                IQueryParamaters parameter = new SlotOccupationParameters();
                return View(
                    _service.GetUIChartModel(Constants.Pages.SlotOccupation, base.GetCurrentUserId(), base.GetCurrentCulture(), parameter )
                );
            }
        }

        /// <summary>
        /// The OperationSummary page of the GenuinaBI
        /// </summary>
        /// <returns></returns>
        //[RequireHttps]
        [PageAuthorize(Constants.Pages.OperationSummary)]
        public ActionResult OperationSummary()
        {
            using (UIChartService _service = new UIChartService())
            {
                //IQueryParamaters parameter = base.GetReportParameters(Constants.Cookies.OperationSummaryParameters); //get parameter from cookie
                IQueryParamaters parameter = new OperationSummaryParameters();
                return View(
                    _service.GetUIChartModel(Constants.Pages.SlotOccupation, base.GetCurrentUserId(), base.GetCurrentCulture(), parameter )
                );
            }
        }

        /// <summary>
        /// The OperationSummary page of the GenuinaBI
        /// </summary>
        /// <returns></returns>
        [PageAuthorize(Constants.Pages.TopPlayers)]
        public ActionResult TopPlayers()
        {
            using (UIChartService _service = new UIChartService())
            {
                //IQueryParamaters parameter = base.GetReportParameters(Constants.Cookies.TopPlayerParameters); //get parameter from cookie
                IQueryParamaters parameter = new TopPlayerParameters();
                return View(
                    _service.GetUIChartModel(Constants.Pages.TopPlayers, base.GetCurrentUserId(), base.GetCurrentCulture(), parameter )
                );
            }
        }

        [PageAuthorize(Constants.Pages.SlotOccupation)]
        public JsonResult GetSlotOccupation(string start)
        {
            try
            {
                if (start == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    SlotOccupationParameters param = new SlotOccupationParameters();
                    param.Start = start;
                    //base.SaveReportParameters(param); //Save parameter to cookie
                    using (SlotOccupationService _service = new SlotOccupationService())
                    {
                        SlotOccupationModel model = _service.GetSlotOccupationModel(param);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [PageAuthorize(Constants.Pages.OperationSummary)]
        public JsonResult GetOperationSummary(string start, string end)
        {
            try
            {
                if (start == null || end == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    OperationSummaryParameters param = new OperationSummaryParameters();
                    param.Start = start;
                    param.End = end;
                    //base.SaveReportParameters(param); //Save parameter to cookie
                    using (OperationSummaryService _service = new OperationSummaryService())
                    {
                        OperationSummaryModel model = _service.GetOperationSummaryModel(param);
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
        [PageAuthorize(Constants.Pages.TopPlayers)]
        public JsonResult GetTopPlayers(string start, string end, int numPlayer, int numVisit)
        {
            try
            {
                if (start == null || end == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TopPlayerParameters param = new TopPlayerParameters();
                    param.Start = start;
                    param.End = end;
                    param.NumberOfPlayers = numPlayer;
                    param.NumberOfVisits = numVisit;
                    //base.SaveReportParameters(param); //Save parameter to cookie
                    using (TopPlayerService _service = new TopPlayerService())
                    {
                        TopPlayerModel model = _service.GetTopPlayerModel(param);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// DataTableHandler was used to accept Sorting/Pagination/Filter request from client jQuery DataTable control
        /// </summary>
        /// <returns></returns>
        [PageAuthorize(Constants.Pages.TopPlayers)]
        public JsonResult TopPlayerDataTableHandler(DataTableParameters param, TopPlayerParameters queryParam)
        {
            try
            {
                //base.SaveReportParameters(queryParam); //Save parameter to cookie
                using (TopPlayerService _service = new TopPlayerService())
                {
                    List<TopPlayerInfoST> playerList = _service.GetTopPlayerList(queryParam).ToList();
                    return Json(new DataTableResult<TopPlayerInfoST>
                        (
                            param.Draw,
                            _service.GetSearchResultCount(param, playerList),
                            _service.GetDataTableResultByPage(param, playerList)
                        ), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// DataTableHandler was used to accept Sorting/Pagination/Filter request from client jQuery DataTable control
        /// </summary>
        /// <returns></returns>
        [PageAuthorize(Constants.Pages.SlotOccupation)]
        public JsonResult SlotOccupationDataTableHandler(DataTableParameters param, SlotOccupationParameters queryParam)
        {
            try
            {
                using (SlotOccupationService _service = new SlotOccupationService())
                {
                    List<SlotOccupationST> slotList = _service.GetSlotOccupationList(queryParam).ToList();
                    return Json(new DataTableResult<SlotOccupationST>
                        (
                            param.Draw,
                            _service.GetSearchResultCount(param, slotList),
                            _service.GetDataTableResultByPage(param, slotList)
                        ), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// DataTableHandler was used to accept Sorting/Pagination/Filter request from client jQuery DataTable control
        /// </summary>
        /// <returns></returns>
        [PageAuthorize(Constants.Pages.OperationSummary)]
        public JsonResult OperationSummaryDataTableHandler(DataTableParameters param, OperationSummaryParameters queryParam)
        {
            try
            {
                using (OperationSummaryService _service = new OperationSummaryService())
                {
                    DateTime start = DateTime.ParseExact(queryParam.Start, Config.CasinoDateTimeFormat, null);
                    DateTime end = DateTime.ParseExact(queryParam.End, Config.CasinoDateTimeFormat, null);

                    List<OperationSummaryProvider> providerList = _service.GetOperationSummaryProviderList(start, end).ToList();
                    return Json(new DataTableResult<OperationSummaryProvider>
                        (
                            param.Draw,
                            _service.GetSearchResultCount(param, providerList),
                            _service.GetDataTableResultByPage(param, providerList)
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