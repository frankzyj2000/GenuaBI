using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GenuinaBI.Models
{
    public class ChartLegend
    {
        public string BgColor { get; set; }
        public string Title { get; set; }
        public ChartLegend(string bgColor, string title)
        {
            this.BgColor = bgColor;
            this.Title = title;
        }
    }

    public class InfoBoxModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Icon { get; set; }
        public string BgColor { get; set; }
        public string ColumnAttribute { get; set; }
        public List<ChartLegend> ChartLegendList { get; set; }
        public InfoBoxModel(string name, string title, string content, string icon, string bgcolor, string columnAttr)
        {
            this.Name = name;
            this.Title = title;
            this.Content = content;
            this.Icon = icon;
            this.BgColor = bgcolor;
            this.ColumnAttribute = columnAttr;
        }

        public InfoBoxModel(string name, string title, string content, string columnAttr,  List<ChartLegend> list) //used for chart box
        {
            this.Name = name;
            this.Title = title;
            this.Content = content;
            this.Icon = "";
            this.BgColor = "";
            this.ColumnAttribute = columnAttr;
            this.ChartLegendList = list;
        }

        public InfoBoxModel(string name, string title, string content, string columnAttr) //used for chart box
        {
            this.Name = name;
            this.Title = title;
            this.Content = content;
            this.Icon = "";
            this.BgColor = "";
            this.ColumnAttribute = columnAttr;
        }

        public InfoBoxModel(string name) //used for table box
        {
            this.Name = name;
            this.Title = "";
            this.Content = "";
            this.Icon = "";
            this.BgColor = "";
            this.ColumnAttribute = "";
        }
    }

    public class DonutChartLegend
    {
        public string Legend { get; set; }
        public string LegendColor { get; set; }
        public string IdName { get; set; }
        public DonutChartLegend(string legend, string color, string idName)
        {
            this.Legend = legend;
            this.LegendColor = color;
            this.IdName = idName;
        }
    }

    public class InfoListItem
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string SmallTitle {get; set; }
        public string Content { get; set; }
        public InfoListItem(string name, string title, string smallTitle, string content)
        {
            this.Name = name;
            this.Title = title;
            this.SmallTitle = smallTitle;
            this.Content = content;
        }
    }

    public class InfoListTab
    {
        public string TabId { get; set; }
        public string TableId { get; set; }
        public string TabTitle { get; set; }
        public string ContentType { get; set; }
        public List<InfoListItem> Content { get; set; }
        public InfoListTab(List<InfoListItem> list, string tabId, string tableId, string tabTitle, string tabType)
        {
            this.TabId = tabId;
            this.TableId = tableId;
            this.TabTitle = tabTitle;
            this.ContentType = tabType;
            this.Content = list;
        }
    }

    public class InfoListTabModel
    {
        public string BoxIdName { get; set; }
        public string IdName { get; set; }
        public string Icon { get; set; }
        public string Head { get; set; }
        public string HeadSmall { get; set; }
        public string BgColor { get; set; }
        public string ColumnAttribute { get; set; }
        public List<InfoListTab> TabList { get; set; }
        public InfoListTabModel(List<InfoListTab> list, string boxidname, string idname, string head, string headsmall, string icon, string bgcolor, string columnAttr)
        {
            this.TabList = list;
            this.BoxIdName = boxidname;
            this.IdName = idname;
            this.Head = head;
            this.HeadSmall = headsmall;
            this.Icon = icon;
            this.BgColor = bgcolor;
            this.ColumnAttribute = columnAttr;
        }
    }

    public class InfoListBoxModel
    {
        public List<InfoListItem> ListItems { get; set; }
        public string BoxIdName { get; set; }
        public string IdName { get; set; }
        public string Icon { get; set; }
        public string Head { get; set; }
        public string HeadSmall { get; set; }
        public string BgColor { get; set; }
        public string ColumnAttribute { get; set; }
        public string DonutIdName {get; set;}
        public List<DonutChartLegend> DonutLegends { get; set; }
        public InfoListBoxModel(List<InfoListItem> itemlist, string boxidname, string idname, string head, string headsmall, string icon, string bgcolor, string columnAttr, string donutIdName, List<DonutChartLegend> donutLegends)
        {
            this.BoxIdName = boxidname;
            this.ListItems = itemlist;
            this.IdName = idname;
            this.Head = head;
            this.HeadSmall = headsmall;
            this.Icon = icon;
            this.BgColor = bgcolor;
            this.ColumnAttribute = columnAttr;
            this.DonutIdName = donutIdName;
            this.DonutLegends = donutLegends;
        }
    }
}