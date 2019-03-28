using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using GenuinaBI.Configuration;
namespace GenuinaBI.Models
{
    public class UIChartModel
    {
        public List<GBI_CFG_DashboardCharts> UIChartList { get; set; }
        public List<CFG_Languages> LanguageList { get; set; }

        public InfoBoxModel InfoBox { get; set; }
        public InfoListBoxModel InfoListBox { get; set; }
        public InfoListTabModel InfoListTabBox { get; set; }

        public List<CFG_AppMenuesTranslation> MenuList { get; set; }
        public List<CFG_AppMenuItems> AllMenuItemList { get; set; }
        public List<CFG_AppMenuItemsTranslation> AllMenuItemTranslationList { get; set; }

        public IQueryParamaters QueryParameter { get; set; }

        public string DefaultStartDate { get; set; }
        public string DefaultEndDate { get; set; }
        public bool IsControlButtonDark { get; set; } 

        public bool OperationSummaryServerCache { get; set; }
        public bool SlotOccupationServerCache { get; set; }
        public bool TopPlayersServerCache { get; set; }
        public bool PlayerSearchServerCache { get; set; }
    }
}
