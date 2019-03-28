using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
namespace GenuinaBI.Models
{
    public class SlotOccupationModel
    {
        public SlotOccupationParameters QueryParameter { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int TotalSlots { get; set; }
        public List<SlotOccupationST> SlotOccupationList { get; set; }
        public decimal TotalWin { get; set; }
        public decimal TotalHandle { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalSlotsOccupied { get; set; }

    }
}
