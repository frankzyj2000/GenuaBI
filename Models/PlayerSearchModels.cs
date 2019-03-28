using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace GenuinaBI.Models
{
    public class PlayerSearchModel
    {
        public PlayerSearchParameters SearchParameter { get; set; }
        public PlayerDetailParameters QueryParmeter { get; set; }
        public MarketingPlayerST MKPlayer { get; set; }
        public List<MarketingPlayerST> MKPlayerList { get; set; }
        public List<MarketingPlayerTrendsST> MKPlayerTrendList { get; set; }
        public List<MarketingPlayerReferencesST> MKPlayerReferenceTodayList { get; set; }
        public List<MarketingPlayerReferencesST> MKPlayerReferenceMonthList { get; set; }
        public List<MarketingPlayerReferencesST> MKPlayerReferenceQuarterList { get; set; }
        public List<MarketingPlayerReferencesST> MKPlayerReferenceAllList { get; set; }
        public List<MarketingPlayerActivityST> MKPlayerActivityTodayList { get; set; }
        public List<MarketingPlayerActivityST> MKPlayerActivityMonthList { get; set; }
        public List<MarketingPlayerActivityST> MKPlayerActivityQuarterList { get; set; }

        public List<MarketingPlayerCardsST> MKPlayerCardList { get; set; }
        public List<MarketingPlayerGameHistoryST> MKPlayerGameHistoryList { get; set; }
        public List<MarketingPlayerPromoST> MKPlayerPromotionList { get; set; }
        public List<MarketingPlayerCashDeskST> MKPlayerCashDeskList { get; set; }
    }
}