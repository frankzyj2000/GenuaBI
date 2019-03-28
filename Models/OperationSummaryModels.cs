using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace GenuinaBI.Models
{
    public class OperationSummaryProvider
    {
        public string Provider { get; set; }
        public decimal CashIn { get; set; }
        public decimal CashOut { get; set; }
        public decimal HandPayments { get; set; }
        public decimal D_Promotion { get; set; }
        public decimal NetWin { get; set; }
        public decimal Win { get; set; }
        public int CantPlayer { get; set; }

        public OperationSummaryProvider(string provider, decimal cashIn, decimal cashOut, 
                decimal handPayments, decimal d_promotion, decimal netWin, decimal win, int cantPlayer)
        {
            this.Provider = provider;
            this.CashIn = cashIn;
            this.CashOut = cashOut;
            this.HandPayments = handPayments;
            this.D_Promotion = d_promotion;
            this.NetWin = netWin;
            this.Win = win;
            this.CantPlayer = cantPlayer;
        }
    }

    public class OperationSummaryTrend
    {
        public string Day { get; set; }
        public decimal NetWinAfterTax { get; set; }
        public decimal WinSlots { get; set; }
        public OperationSummaryTrend(string day, decimal netwinAfterTax, decimal winSlots)
        {
            this.Day = day;
            this.NetWinAfterTax = netwinAfterTax;
            this.WinSlots = winSlots;
        }
    }

    public class OperationSummaryModel
    {
        public OperationSummaryParameters QueryParameter { get; set; }
        public decimal WinSlots { get; set; }
        public decimal NetWinCashIn { get; set; }
        public decimal NetWinCashOut { get; set; }
        public decimal Taxes { get; set; }

        public decimal PlayerAccountIn { get; set; }
        public decimal PlayerAccountOut { get; set; }
        public decimal PlayerAccountAmount { get; set; }

        public decimal SpecialPromos { get; set; }
        public decimal GrantedPromos { get; set; }
        public decimal CancelledPromos { get; set; }
        public decimal ConsumedPromos { get; set; }
        public decimal OverShortPromos { get; set; }

        public decimal TotalMoneyIn { get; set; }
        public decimal TotalMoneyOut { get; set; }
        public decimal TotalDPromotion { get; set; }
        public decimal TotalHandPayments { get; set; }
        public decimal TotalNetWin { get; set; }
        public decimal TotalWin { get; set; }
        public int TotalCantPlayer { get; set; }

        public int TotalSessions { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalSlots { get; set; }
        public int TotalSlotOccupied { get; set; }

        public List<OperationSummaryProvider> ProviderList { get; set; }
        public List<OperationSummaryTrend> OSTrendFor1Week { get; set; }
        public List<OperationSummaryTrend> OSTrendFor7Week { get; set; }
        
   }
}