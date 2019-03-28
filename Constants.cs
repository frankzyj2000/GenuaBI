using System.Collections.ObjectModel;

namespace GenuinaBI
{
    public class Constants
    {
        public static double SCREEN_REFRESH_TIME = 15;

        public const string CONNECTION_STRING = "DefaultConnection";

        public const int DEFAULT_VALUE_REFRESH_DASHBOARD = 30;

        public const string APP_NAME = "Genuina_Dashboard";

        public struct AppKey
        {
            public const string DefaultNoComm = "DefaultDelayNoCommunication";

            public const string SenderEmail = "SenderEmail";

            public const string UsernameEmail = "UsernameEmail";

            public const string PasswordEmail = "PasswordEmail";

            public const string SupportEmail = "SupportEmail";

            public const string SendMailError = "SendMailError";

            public const string RecipientsError = "RecipientsError";
        }

        public struct IconsName
        {
            public const string Dashboard = "fa fa-dashboard";
            public const string Circle    = "fa fa-circle-o";
            public const string Widgets   = "fa fa-th";
            public const string Forms     = "fa fa-edit";
            public const string Laptop    = "fa fa-laptop";
            public const string Tables    = "fa fa-table";
            public const string Calendar  = "fa fa-calendar";
            public const string Mailbox   = "fa fa-envelope";
            public const string Folder    = "fa fa-folder";
            public const string Document  = "fa fa-book";
        }
        // this 
        public static ReadOnlyCollection<string> Icons = new ReadOnlyCollection<string>(
            new[]{
                IconsName.Document, //should be the first for the icon of each menuitem
                IconsName.Dashboard,
                IconsName.Widgets,
                IconsName.Forms,
                IconsName.Laptop,
                IconsName.Tables,
                IconsName.Calendar,
                IconsName.Mailbox,
                IconsName.Folder,
            }
        );

        public struct Pages //used for page authorization
        {
            public const string SlotOccupation = "SlotOccupation";
            public const string TopPlayers = "TopPlayers";
            public const string OperationSummary = "OperationSummary";
            public const string PlayerSearch = "PlayerSearch";
        }

        public struct Cookies
        {
            public const string Casino = "CasinoCookie";
            public const string SlotOccupationParameters = "SlotOccupationParameters";
            public const string OperationSummaryParameters = "OperationSummaryParameters";
            public const string TopPlayerParameters = "TopPlayerParameters";
        }

        public struct ReportName
        {
            public const string SlotOccupation = "MK 30";
            public const string TopPlayers = "MK 15";
            public const string OperationSummary = "CF 51 op C";
            public const string PlayerSearch = "GBI 01";
        }

        public struct CasinoCookie
        {
            public const string Id = "Id";
            public const string Name = "Name";
            public const string Culture = "Culture";
        }

        public struct ReportParameterCookie
        {
            public const string Start = "Start";
            public const string End = "End";
            public const string NumberOfPlayers = "NumberOfPlayers";
            public const string NumberOfVisits = "NumberOfVisits";
            public const string Type = "Type";
            public const string PageLength = "PageLength";
            public const string SlotMachine = "SlotMachine";
            public const string PlayerName = "PlayerName";
            public const string CardNumber = "CardNumber";
            public const string MaxLoadingRecords = "MaxLoadingRecords";
            public const string PlayerID = "PlayerID";
        }

        public struct Configuration
        {
            public const string LogClientError = "LogClientError";
            public const string DBConfigSource = "DBConfigSource";
            public const string MinimumDBVersion = "MinimumDBVersion";
            public const string RefreshTimeOut = "RefreshTimeOut";
            public const string SessionTimeOut = "SessionTimeOut";
            public const string WarningBeforeSessionTimeOut = "WarningBeforeSessionTimeOut";

            public const string OperationSummaryDateRangeLimit = "OperationSummaryDateRangeLimit";

            public const string TopPlayersMaxVisits = "TopPlayersMaxVisits";
            public const string TopPlayersMaxPlayers = "TopPlayersMaxPlayers";
            public const string TopPlayersDefaultPlayers = "TopPlayersDefaultPlayers";
            public const string TopPlayersDefaultVisits = "TopPlayersDefaultVisits";
            public const string TopPlayersDefaultDateRange ="TopPlayersDefaultDateRange";

            public const string DataTableDefaultPageLength = "DataTableDefaultPageLength";
            public const string CasinoStartTime = "CasinoStartTime";
            public const string CasinoEndTime = "CasinoEndTime";
            public const string CasinoDateFormat = "CasinoDateFormat";
            public const string CasinoTimeFormat = "CasinoTimeFormat";
            public const string CasinoCurrencyThousand = "CasinoCurrencyThousand";
            public const string CasinoCurrencyDecimal = "CasinoCurrencyDecimal";
            public const string CasinoCurrencyPrecision = "CasinoCurrencyPrecision";
            public const string CasinoCurrencySymbol = "CasinoCurrencySymbol";
            public const string GCSHost = "GCSHost";
            public const string GCSPort = "GCSPort";
            public const string MinimumParameterYear = "MinimumParameterYear";
        }

        public const string SuperAdminUser = "GenuinaBI";

        public const string DefaultConnection = "DefaultConnection";

        public struct Roles
        {
            public const string SuperAdmin = "SuperAdmin";

            public const string Admin = "Admin";
        }

        public const string SuperAdminRole = "SuperAdmin";
    }
}
