using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace GenuinaBI.Models
{
    public class TopPlayerModel
    {
        public TopPlayerParameters QueryParameter { get; set; }
        public List<TopPlayerInfoST> TopPlayerList { get; set; }
   }
}