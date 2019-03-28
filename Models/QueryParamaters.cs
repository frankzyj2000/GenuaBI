using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GenuinaBI.Configuration;

namespace GenuinaBI.Models
{
    /*Note this param is for store procedure in most cases*/
    public class IQueryParamaters
    {
    }

    public class QueryParamters : IQueryParamaters
    {
        protected int startHours = 7; //default casino start time
        protected int startMinutes = 0;
        protected int startSeconds = 0;

        protected int endHours = 6; //default casino end time
        protected int endMinutes = 59;
        protected int endSeconds = 59;

        protected int defaultPageLength = 10;

        protected bool IsCurrentTimeLargerThanDefaultEndTime()
        {
            string timeFormat = Config.CasinoTimeFormat;
            timeFormat = timeFormat.Replace("HH", "hh");
            timeFormat = timeFormat.Replace(":", "\\:"); //custom format need to be 
            if ( DateTime.Now.TimeOfDay >= System.TimeSpan.ParseExact(Config.CasinoEndTime, timeFormat, null) )
                return true;
            else
                return false;
        }

        public QueryParamters()
        {
            if (Configuration.Config.CasinoStartTime.Trim().Length == 8) //07:00:00
            {
                startHours = Int32.Parse(Configuration.Config.CasinoStartTime.Substring(0, 2));
                startMinutes = Int32.Parse(Configuration.Config.CasinoStartTime.Substring(3, 2));
                startSeconds = Int32.Parse(Configuration.Config.CasinoStartTime.Substring(6, 2));
            }

            if (Configuration.Config.CasinoEndTime.Trim().Length == 8) //06:59:59
            {
                endHours = Int32.Parse(Configuration.Config.CasinoEndTime.Substring(0, 2));
                endMinutes = Int32.Parse(Configuration.Config.CasinoEndTime.Substring(3, 2));
                endSeconds = Int32.Parse(Configuration.Config.CasinoEndTime.Substring(6, 2));
            }
            defaultPageLength = Config.DataTableDefaultPageLength;
        }
    }

    public class SlotOccupationParameters : QueryParamters
    {
        public string Start { get; set; }
        public int PageLength { get; set; }
        public SlotOccupationParameters()
            : base() //call base constructor first
        {
            Start = DateTime.Today.ToString(Config.CasinoDateFormat);
            PageLength = defaultPageLength;
        }
    }

    public class OperationSummaryParameters : QueryParamters
    {
        public string Start { get; set; }
        public string End { get; set; }
        public int PageLength { get; set; }
        public OperationSummaryParameters()
            : base() //call base constructor first
        {
            DateTime start = DateTime.Today.AddDays(-1).AddHours(startHours).AddMinutes(startMinutes).AddSeconds(startSeconds);
            DateTime end = DateTime.Today.AddHours(endHours).AddMinutes(endMinutes).AddSeconds(endSeconds);

            if (IsCurrentTimeLargerThanDefaultEndTime())
            {
                start = start.AddDays(1);
                end = end.AddDays(1);
            }

            Start = start.ToString(Config.CasinoDateTimeFormat);
            End = end.ToString(Config.CasinoDateTimeFormat);
            PageLength = defaultPageLength;
        }
    }

    public class PlayerSearchParameters : QueryParamters
    {
        //must be on the same order in playerSearch.js
        public string SlotMachine { get; set; }
        public string PlayerName { get; set; }
        public string CardNumber { get; set; }
        public int MaxLoadingRecords {get; set;}
        public PlayerSearchParameters()
            : base() //call base constructor first
        {
        }
    }

    public class PlayerDetailParameters : QueryParamters
    {
        //must be on the same order in playerSearch.js
        public string PlayerID { get; set; }
        public string End {get; set;} //define as DateTime because it doesn't change manually
        public int PageLength { get; set; }
        public PlayerDetailParameters()
            : base() //call base constructor first
        {
            PageLength = defaultPageLength;
        }
    }

    public class TopPlayerParameters : QueryParamters
    {
        //must be on the same order in topplayer.js
        public string Start { get; set; }
        public string End { get; set; }
        public int NumberOfPlayers { get; set; }
        public int NumberOfVisits { get; set; }
        public int PageLength { get; set; }
        public int MaxPlayers { get; set; }
        public int MaxVisits { get; set; }
        public TopPlayerParameters()
            : base() //call base constructor first
        {
            NumberOfPlayers = Config.TopPlayersDefaultPlayers;
            NumberOfVisits = Config.TopPlayersDefaultVisits;
            MaxPlayers = Config.TopPlayersMaxPlayers;
            MaxVisits = Config.TopPlayersMaxVisits;
            DateTime start = DateTime.Today.AddDays(-Config.TopPlayersDefaultDateRange).AddHours(startHours).AddMinutes(startMinutes).AddSeconds(startSeconds);
            DateTime end = DateTime.Today.AddHours(endHours).AddMinutes(endMinutes).AddSeconds(endSeconds);
            if (IsCurrentTimeLargerThanDefaultEndTime())
            {
                start = start.AddDays(1);
                end = end.AddDays(1);
            }

            Start = start.ToString(Config.CasinoDateTimeFormat);
            End = end.ToString(Config.CasinoDateTimeFormat);
            PageLength = defaultPageLength;
        }
    }
}