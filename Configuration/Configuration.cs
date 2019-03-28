using System;
using System.Configuration;
namespace GenuinaBI.Configuration
{
    public static class Config
    {
        public static bool LogClientError 
        { 
            get 
            {
                return bool.Parse(ConfigurationManager.AppSettings[Constants.Configuration.LogClientError] ?? "false");
            } 
            set
            {
                Save(Constants.Configuration.LogClientError, value.ToString());
            }
        }

        public static string DBConfigSource 
        { 
            get 
            { 
                return ConfigurationManager.AppSettings[Constants.Configuration.DBConfigSource]; 
            } 
            set
            {
                Save(Constants.Configuration.DBConfigSource, value);
            }
        }

        public static string MinimumDBVersion
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.Configuration.MinimumDBVersion] ?? "63001";
            }
            set
            {
                Save(Constants.Configuration.MinimumDBVersion, value);
            }
        }

        public static int MinimumParameterYear
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.MinimumParameterYear] ?? "2000");
            }
            set
            {
                Save(Constants.Configuration.MinimumParameterYear, value.ToString());
            }
        }

        public static int RefreshTimeOut 
        { 
            get 
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.RefreshTimeOut] ?? "15"); 
            } 
            set
            {
                Save(Constants.Configuration.RefreshTimeOut, value.ToString());
            }
        }

        public static int SessionTimeOut
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.SessionTimeOut] ?? "60");
            }
            set
            {
                Save(Constants.Configuration.SessionTimeOut, value.ToString());
            }
        }

        public static int WarningBeforeSessionTimeOut 
        { 
            get 
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.WarningBeforeSessionTimeOut] ?? "5"); 
            } 
            set
            {
                Save(Constants.Configuration.WarningBeforeSessionTimeOut, value.ToString());
            }
        }

        public static int OperationSummaryDateRangeLimit
        {
            get 
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.OperationSummaryDateRangeLimit] ?? "30"); 
            } 
            set
            {
                Save(Constants.Configuration.OperationSummaryDateRangeLimit, value.ToString());
            }
        }

        public static bool TopPlayersServerCache
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings[Constants.Pages.TopPlayers + "ServerCache"] ?? "false");
            }
            set
            {
                Save(Constants.Pages.TopPlayers + "ServerCache", value.ToString());
            }
        }

        public static bool OperationSummaryServerCache
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings[Constants.Pages.OperationSummary + "ServerCache"] ?? "false");
            }
            set
            {
                Save(Constants.Pages.OperationSummary + "ServerCache", value.ToString());
            }
        }

        public static bool SlotOccupationServerCache
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings[Constants.Pages.SlotOccupation + "ServerCache"] ?? "false");
            }
            set
            {
                Save(Constants.Pages.SlotOccupation + "ServerCache", value.ToString());
            }
        }

        public static bool PlayerSearchServerCache
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings[Constants.Pages.PlayerSearch + "ServerCache"] ?? "false");
            }
            set
            {
                Save(Constants.Pages.PlayerSearch + "ServerCache", value.ToString());
            }
        }
        
        public static int TopPlayersMaxPlayers
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.TopPlayersMaxPlayers] ?? "250");
            }
            set
            {
                Save(Constants.Configuration.TopPlayersMaxPlayers, value.ToString());
            }
        }

        public static int TopPlayersMaxVisits
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.TopPlayersMaxVisits] ?? "250");
            }
            set
            {
                Save(Constants.Configuration.TopPlayersMaxVisits, value.ToString());
            }
        }

        public static int TopPlayersDefaultDateRange
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.TopPlayersDefaultDateRange] ?? "21");
            }
            set
            {
                Save(Constants.Configuration.TopPlayersDefaultDateRange, value.ToString());
            }
        }

        public static int TopPlayersDefaultPlayers
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.TopPlayersDefaultPlayers] ?? "20");
            }
            set
            {
                Save(Constants.Configuration.TopPlayersDefaultPlayers, value.ToString());
            }
        }

        public static int TopPlayersDefaultVisits
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.TopPlayersDefaultVisits] ?? "10");
            }
            set
            {
                Save(Constants.Configuration.TopPlayersDefaultVisits, value.ToString());
            }
        }

        public static int DataTableDefaultPageLength
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.DataTableDefaultPageLength] ?? "25");
            }
            set
            {
                Save(Constants.Configuration.DataTableDefaultPageLength, value.ToString());
            }
        }

        public static string CasinoStartTime
        {
            get
            {   
                string startTime = ConfigurationManager.AppSettings[Constants.Configuration.CasinoStartTime];
                //verify the configuration value
                if (IsValidTime(startTime))
                    return startTime;
                else
                    return "07:00:00"; // if configuration value has error use this default
            }
            set
            {
                Save(Constants.Configuration.CasinoStartTime, value.ToString());
            }
        }

        public static string CasinoEndTime
        {
            get
            {
                string endTime = ConfigurationManager.AppSettings[Constants.Configuration.CasinoEndTime];
                if (IsValidTime(endTime))
                    return endTime;
                else
                    return "06:59:59"; // if configuration value has error use this default
            }
            set
            {
                Save(Constants.Configuration.CasinoEndTime, value.ToString());
            }
        }

        public static string CasinoDateFormat
        {
            get
            {
                string dateFormat = ConfigurationManager.AppSettings[Constants.Configuration.CasinoDateFormat];
                if (dateFormat == null || dateFormat == string.Empty)
                {
                    dateFormat = "dd/MM/yyyy"; //TODO: Get it from GenuinaDB
                }
                return dateFormat;
            }
            set
            {
                Save(Constants.Configuration.CasinoDateFormat, value.ToString());
            }
        }

        public static string CasinoDateTimeFormat
        {
            get
            {
                return Config.CasinoDateFormat + " " + Config.CasinoTimeFormat;
            }
        }

        public static string CasinoTimeFormat
        {
            get
            {
                string timeFormat = ConfigurationManager.AppSettings[Constants.Configuration.CasinoTimeFormat];
                if (timeFormat == null || timeFormat == string.Empty)
                {
                    timeFormat = "HH:mm:ss"; //TODO: Get it from GenuinaDB
                }
                return timeFormat;
            }
            set
            {
                Save(Constants.Configuration.CasinoTimeFormat, value.ToString());
            }
        }

        //Currency Format - Thousand Separator
        public static string CasinoCurrencyThousand
        {
            get
            {
                string currencyThousand = ConfigurationManager.AppSettings[Constants.Configuration.CasinoCurrencyThousand];
                if (currencyThousand == null || currencyThousand == string.Empty)
                {
                    currencyThousand = ","; //TODO: Get it from GenuinaDB
                }
                return currencyThousand;
            }
            set
            {
                Save(Constants.Configuration.CasinoCurrencyThousand, value.ToString());
            }
        }

        //Currency Format - Decimal Separator
        public static string CasinoCurrencyDecimal
        {
            get
            {
                string currencyDecimal = ConfigurationManager.AppSettings[Constants.Configuration.CasinoCurrencyDecimal];
                if (currencyDecimal == null || currencyDecimal == string.Empty)
                {
                    currencyDecimal = "."; //TODO: Get it from GenuinaDB
                }
                return currencyDecimal;
            }
            set
            {
                Save(Constants.Configuration.CasinoCurrencyDecimal, value.ToString());
            }
        }

        //Currency Format - Currency Precision
        public static int CasinoCurrencyPrecision
        {
            get
            {
                //TODO: Get it from GenuinaDB
                int currencyPrecision = int.Parse(ConfigurationManager.AppSettings[Constants.Configuration.CasinoCurrencyPrecision] ?? "2") ;
                return currencyPrecision;
            }
            set
            {
                Save(Constants.Configuration.CasinoCurrencyPrecision, value.ToString());
            }
        }

        public static string CasinoCurrencySymbol
        {
            get
            {
                string currencySymbol = ConfigurationManager.AppSettings[Constants.Configuration.CasinoCurrencySymbol];
                if (currencySymbol == null || currencySymbol == string.Empty)
                {
                    currencySymbol = "$"; //TODO: Get it from GenuinaDB
                }
                return currencySymbol;
            }
            set
            {
                Save(Constants.Configuration.CasinoCurrencySymbol, value.ToString());
            }
        }

        public static class GCS
        {
            public static string Host 
            {
                get 
                {
                    return ConfigurationManager.AppSettings[Constants.Configuration.GCSHost]; 
                }
                set
                {
                    Save(Constants.Configuration.GCSHost, value);
                }
            }
            public static string Port
            {
                get
                {
                    return ConfigurationManager.AppSettings[Constants.Configuration.GCSPort];
                }
                set
                {
                    Save(Constants.Configuration.GCSPort, value);
                }
            }
        }

        private static void Save(string key, string value )
        {
            //Get current configuration file
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationManager.AppSettings[key] != null)
            {
                config.AppSettings.Settings.Remove(key);
            }
            config.AppSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        private static bool IsValidTime(string time)
        {
            try
            {
                int hour = Int32.Parse(time.Substring(0,2));
                int min  = Int32.Parse(time.Substring(3,2));
                int sec  = Int32.Parse(time.Substring(6,2));
                if (time.Length == 8 && hour<24 && min <60 && sec < 60 && time.Substring(2,1) == ":" && time.Substring(5,1) == ":")
                    return true;
                else
                    return false;
            }
            catch //has error
            {
                return false;
            }

        }
    }
}