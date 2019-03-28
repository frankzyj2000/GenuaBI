"use strict";

//// HtmlHelpers Module
//// Call by using HtmlHelpers.getQueryStringValue("myname");
//var HtmlHelpers = function() {
//    return {
//        // Based on http://stackoverflow.com/questions/901115/get-query-string-values-in-javascript
//        getQueryStringValue: function(name) {
//            var match = RegExp('[?&]' + name + '=([^&]*)').exec(location.search);
//            return match && decodeURIComponent(match[1].replace( /+/g , ' '));
//        }
//    };
//}();

// StringHelpers Module
// Call by using StringHelpers.padLeft("1", "000");
var StringHelpers = function() {
    return {
        // Pad string using padMask.  string '1' with padMask '000' will produce '001'.
        padLeft: function(string, padMask) {
            string = '' + string;
            return (padMask.substr(0, (padMask.length - string.length)) + string);
        }
    };
}();


var gbService = function () {

    function getPDFMargins() {
        var margins = [25, 25, 25, 25];
        return margins;
    }

    function getPDFStyles() {
        var styles = {
            header: {
                fontSize: 14,
                bold: true,
                alignment: "center",
            },
            subheader: {
                fontSize: 12,
                bold: true,
            },
            parameter: {
                fontSize: 9,
                alignment: 'center',
            },
            reportname: {
                fontSize: 8,
                bold: true,
                alignment: 'right',
            },
            createdate: {
                fontSize: 8,
                alignment: 'left'
            },
            title: {
                fontSize: 10,
                bold: true,
                alignment: 'center'
            },
            subtitle: {
                fontSize: 8,
                bold: true,
            },
            charttitle: {
                fontSize: 9,
                bold: true,
                alignment: 'center'
            },
            value: {
                fontSize: 9,
                bold: true,
                alignment: 'right'
            },
            smallvalue: {
                fontSize: 8,
                bold: true,
                alignment: 'right'
            },
            tablevalue1: {
                fontSize: 8,
                alignment: 'right'
            },
            tablevalue2: {
                fontSize: 8,
                alignment: 'right',
                fillColor: 'lightgrey'
            },

            tablefirstcolumn1: {
                fontSize: 8,
                alignment: 'left'
            },

            tablefirstcolumn2: {
                fontSize: 8,
                alignment: 'left',
                fillColor: 'lightgrey'
            },
            tableheader: {
                bold: true,
                fontSize: 9,
                alignment: 'center',
                color: 'white',
                fillColor: 'darkblue'
            },
            pagination: {
                fontSize: 10,
                alignment: 'right'
            },
            reportuser: {
                fontSize: 9,
                alignment: 'right'
            }
        };
        return styles;
    }

    function getPDFTableLayout() {
        var layout = {
            hLineWidth: function (i, node) {
                return (i === 0 || i === node.table.body.length) ? 2 : 0;
            },
            vLineWidth: function (i, node) {
                return (i === 0 || i === node.table.widths.length) ? 2 : 1;
            },
            hLineColor: function (i, node) {
                return (i === 0 || i === node.table.body.length) ? 'black' : 'gray';
            },
            vLineColor: function (i, node) {
                return (i === 0 || i === node.table.widths.length) ? 'black' : 'gray';
            },
            // paddingLeft: function(i, node) { return 4; },
            // paddingRight: function(i, node) { return 4; },
            // paddingTop: function(i, node) { return 2; },
            // paddingBottom: function(i, node) { return 2; }
        };
        return layout;
    }

    function logError(details) {
        var model = JSON.stringify({ Context: navigator.userAgent, Details: details });
        $.ajax({
            type: 'POST',
            url: '/Error/LogJavaScriptError',
            data: model,
            dataType: "json",
            contentType: "application/json"
        });
    }

    var exchangeElement = function (a, b) {
        var aparent = a.parentNode;
        var asibling = a.nextSibling === b ? a : a.nextSibling;
        b.parentNode.insertBefore(a, b);
        aparent.insertBefore(b, asibling);
    }

    function handleDragStop(event, ui) {
        var draggable = $(this);
        draggable.animate({ left: 0, top: 0 }, 500); //revert to the original position
    }

    function errorInit() {
        if (getResourceMsg('logclienterror').toLowerCase() == 'true') {
            //recording client side javascript error
            window.onerror = function (errorMsg, url, lineNumber, column, errorObj) {
                //filter out iOS safari and old android javascript error as they returned Script error
                if (errorMsg.indexOf('Script error.') > -1) {
                    return;
                }
                logError('Error: ' + errorMsg + '\n Script: ' + url + ' Line: ' + lineNumber
                + ' Column: ' + column + '\n StackTrace: ' +  errorObj);
            }
        }
    }
    //get object from local storage
    function getLocalObject(objName) {
        return JSON.parse(localStorage.getItem(objName));
    }

    //save object to local storage
    function setLocalObject(objName, obj) {
        localStorage.setItem(objName, JSON.stringify(obj)); 
    }

    function getColor(color) {
        switch (color.toLowerCase()) {
            case 'red':
                return "#ff0000";
            case 'lightred':
                return "#f56954";
            case 'aqua':
                return "#00c0ef";
            case 'lightblue':
                return "#3c8dbc";
            case 'blue':
                return "#0073B7";
            case 'yellow':
                return "#f39c12";
            case 'green':
                return "#00a65a";
            case 'grey':
                return "#d2d6de";
            case 'white':
                return "#ffffff";
            default:
                return "00c0ef"; //default is aqua
        }
    }

    function getBgColor(color, opacity) {
        if (opacity == null) opacity = 1;
        switch (color.toLowerCase()) {
            case 'red':
                return "rgba(255,0,0," + opacity + ")";
            case 'lightred':
                return "rgba(245,105,84," + opacity + ")";
            case 'aqua':
                return "rgba(0,192,239," + opacity + ")";
            case 'lightblue':
                return "rgba(60,141,188," + opacity + ")";
            case 'blue':
                return "rgba(0,115,183)," + opacity + ")";
            case 'yellow':
                return "rgba(243,156,18," + opacity + ")";
            case 'green':
                return "rgba(0,166,90," + opacity + ")";
            case 'grey':
                return "rgba(210, 214, 222," + opacity + ")";
            case 'white':
                return "rgba(255,255,255," + opacity + ")";
            default:
                return "rgba(0,192,239," + opacity + ")";
        }
    }


    //convert date format for SQL Server date column
    function showQueryWaitingInfo() {
        $('#buttonExecute').attr("disabled", true); //disable the execute button
        $('#buttonPDF').attr("disabled", true);
        var message = getResourceMsg('querywaitinginfo');
        toastr.clear();
        toastr.options = {
            "showDuration": "0",
            "hideDuration": "0",
            "timeOut": "0",
            "extendedTimeOut": "0"
        }
        toastr["info"](message);
        InitToastr();
    }

    function clearQueryWaitingInfo() {
        toastr.clear();
        $('#buttonExecute').attr("disabled", false); //enable the execute button
        $('#buttonPDF').attr("disabled", false);
    }

    // a and b are javascript Date objects
    function getDiffInDays(a, b) {
        var _MS_PER_DAY = 1000 * 60 * 60 * 24;
        // Discard the time and time-zone information.
        var utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
        var utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());

        return Math.floor((utc2 - utc1) / _MS_PER_DAY);
    }

    function displayDatePart(dateStr) {
        var date = moment(dateStr, getSysDateTimeFormat());
        if (date.isValid()) {
            return moment(date).format(getSysDateFormat());
        }
        return dateStr;
    }

    function displayDatePartFromSpecialFormat(dateStr, dateFormat) {
        var date = moment(dateStr, dateFormat);
        if (date.isValid()) {
            return moment(date).format(getSysDateFormat());
        }
        return dateStr;
    }

    //Check if the DateRange Start and End are within the limit dateFormat in configuration
    var checkDateRangeValue = function (start, end) {
        var message = getResourceMsg('daterangeerror');

        if (typeof start === "string")
            start = moment(start, getSysDateTimeFormat());
        if (typeof end === "string")
            end = moment(end, getSysDateTimeFormat());

        if (!start.isValid() || !end.isValid()) {
            showToastrError(message);
            return false;
        }

        var minimumYear = getResourceMsg('minimumparameteryear');
        message = getResourceMsg('parameteryearerror').replace('{0}', minimumYear);
        
        if (start.year() < minimumYear || end.year() < minimumYear) {
            showToastrError(message);
            return false;
        }

        var maxdays = getResourceMsg('daterangelimit');
        if (typeof (maxdays) == 'undefined' || maxdays == 0) return true; //don't do check

        message = getResourceMsg('dateranglimiterror').replace('{0}', maxdays);

        if (getDiffInDays(start.toDate(), end.toDate()) > maxdays) {
            showToastrError(message);
            return false;
        }

        return true;
    }

    //Check if the Single Date valid
    var checkSingleDateParameter = function (start) {
        var message = getResourceMsg('daterangeerror');
        if (moment(start, getSysDateFormat()).isValid() != true) {
            showToastrError(message);
            return false;
        }

        var minimumYear = getResourceMsg('minimumparameteryear');
        message = getResourceMsg('parameteryearerror').replace('{0}', minimumYear);
        if (moment(start, getSysDateFormat()).year() < minimumYear) {
            showToastrError(message);
            return false;
        }
        return true;
    }

    var autoRefresh;
    var setAutoRefreshOnOff = function (refreshAction) {
        var button = $('#buttonAutoRefresh');
        if (button == null) return; //don't do check
        if (button.data('toggles') == null) return //don't do check

        var toggle = button.data('toggles'); //check the current states of the refresh button
        if (toggle.active) {
            //if the button is active
            //if start date is larger than default start date and end date is large than current time then toggle the Refresh 
            //otherwise, display modal to override the date range parameters using default date
            if ( isDateParametersOKForAutoRefresh(getDateRangeStartTime(), getDateRangeEndTime() )) {
                var refreshRate = getResourceMsg('refreshtimeout') * 1000;
                if (refreshRate > 0) {
                    autoRefresh = setInterval(refreshAction, refreshRate); //only set one time
                }
            }
            else {
                $('#warningToDefaultDateRange')
                    .modal({backdrop: 'static'}) //disable backdrop so user need to make choice
                    .one('click', '[data-value]', function (e) {
                        e.preventDefault();
                        if ($(this).data('value')) { //Yes
                            //replace the parameter with default value
                            var startDate = getResourceMsg('defaultstartdate');
                            if (getDateRangeEndTime() == null)
                            {
                                $('input[name="daterange"]').data('daterangepicker').setStartDate(startDate);
                            }
                            else
                            {
                                var endDate = getResourceMsg('defaultenddate');
                                $('input[name="daterange"]').unbind("change"); //unbind the change func to remove 3-value change interference.
                                $('input[name="daterange"]').val(startDate + ' - ' + endDate); //should be the first
                                $('input[name="daterange"]').data('daterangepicker').setStartDate(startDate);
                                $('input[name="daterange"]').data('daterangepicker').setEndDate(endDate); 
                                updateButtonAutoRefresh(); 
                                $('input[name="daterange"]').change(updateButtonAutoRefresh); //rebind the change func
                            }
                            //start do page auto refreshing
                            var refreshRate = getResourceMsg('refreshtimeout') * 1000;
                            if (refreshRate > 0) {
                                autoRefresh = setInterval(refreshAction, refreshRate); //only set one time
                            }
                            $('#warningToDefaultDateRange').modal('hide'); //close the modal
                        }
                        else { //No
                            closeDefaultParameterWarningModal();
                        }
                    });
            }
        }
        else if (typeof (autoRefresh) != "undefined") {//disable the AutoRefresh
            $('#buttonAutoRefresh').toggles(false);
            clearInterval(autoRefresh); //clear autoRefresh
        }
    }

    var closeDefaultParameterWarningModal = function () {
        $('#buttonAutoRefresh').toggles(false);
        $('#warningToDefaultDateRange').modal('hide') //close the modal
    }

    // Check if the Start Date and End Date Range is OK for AutoRefresh
    // Note end could be null if so don't check the end 
    var isDateParametersOKForAutoRefresh = function (start, end) {
        if (typeof start === "string")
            start = moment(start, getSysDateTimeFormat()).toDate();
        if (end != null && typeof end === "string")
            end = moment(end, getSysDateTimeFormat()).toDate();

        if ( start >= moment(getResourceMsg('defaultstartdate'), getSysDateTimeFormat()).toDate() )
        {
            if (end == null || (end != null && end >= moment(new Date()).toDate()) )
            {
                return true;
            }
            else {
                return false;
            }
        }
    }

    //Enable or Disable AutoRefresh button based on start and end date range input
    var updateButtonAutoRefresh = function () {
        var startDate = getDateRangeStartTime();
        var endDate = getDateRangeEndTime();

        var button = $('#buttonAutoRefresh');
        if (button == null) return; //don't do check
        if (button.data('toggles') == null) return //don't do check

        var toggle = button.data('toggles');
        //If Date Parameters is out of default range => disable the AutoRefresh button 
        if ( ! isDateParametersOKForAutoRefresh(startDate, endDate) && toggle.active) {
            //disable and toggle back the button using Simon Tabor Toggles
            //note here toggles() was supplied instead of jQuery toggle()
            button.toggles(false);
        }
    }

    var setAutoRefreshOn = function (refreshAction) {
        if ($('#checkAutoRefresh').is(':enabled') && $('#checkAutoRefresh').is(':checked')) {
            var refreshRate = getResourceMsg('refreshtimeout') * 1000;
            if (refreshRate > 0) {
                autoRefresh = setInterval(refreshAction, refreshRate); //only set one time
            }
        }
    }

    var setAutoRefreshOff = function () {
        if (typeof (autoRefresh) != "undefined") {
            clearInterval(autoRefresh); //clear autoRefresh
        }
    }

    var enableCheckAutoRefresh = function () {
        var start = getDateRangeStartTime();
        var end = getDateRangeEndTime();
        //if start date is today and end date is large than current time set refresh button enable otherwise turn it off
        if (isDateParametersOKForAutoRefresh(start, end)) {
            $('#checkAutoRefresh').removeAttr("disabled");
            $('#labelAutoRefresh').attr("title", getResourceMsg('btnrefreshtooltip'));
        }
        else {
            $('#checkAutoRefresh').attr("checked", false);
            $('#checkAutoRefresh').attr("disabled", true);
            $('#labelAutoRefresh').attr("title", getResourceMsg('btnrefreshdisabledtooltip'));
        }
    }

    var toMoneyStr = function (value) {
        return accounting.formatNumber(Number(value));
        /*return Number(Number(value).toFixed(2)).toLocaleString("en-US", { minimumFractionDigits: 2 });*/
    }

    var strToDateTime = function (s) {
        s = s.split(/[-: ]/); //date format yyyy/MM/dd HH:mm:ss
        return new Date(s[0], s[1] - 1, s[2], s[3], s[4], s[5]);
    }

    // Return a message contained in the cshtml page container
    function getResourceMsg(msgId) {
        msgId = 'data-' + msgId;
        var container = $(".resouceLabels");
        if (container.length == 0) return;
        return container.attr(msgId);
    }

    function setResourceMsg(msgId, value) {
        msgId = 'data-' + msgId;
        var container = $(".resouceLabels");
        if (container.length == 0) return;
        container.attr(msgId, value);
    }

    // getDateRangeStartTime from DateRange
    var getDateRangeStartTime = function () {
        var inputdate = $('input[name="daterange"]').val().trim();
        var pos = inputdate.indexOf(" - ");
        var start = "";
        if (pos == -1) { //daterange is for start date only
            start = inputdate;
            return start;
        }
        else {
            start = inputdate.substring(0, pos);
            if (moment(start, getSysDateTimeFormat()).isValid() == true) {
                return start;
            }
            else {
                message = getResourceMsg('daterangeerror');
                showToastrError(message);
                return null;
            }
        }
    }

    // getDateRangeEndTime from DateRange
    var getDateRangeEndTime = function () {
        var inputdate = $('input[name="daterange"]').val().trim();
        var pos = inputdate.indexOf(" - ");
        if (pos == -1) return null; //daterange is for start date only
        var end = inputdate.substr(pos + 3, inputdate.length);

        if (moment(end, getSysDateTimeFormat()).isValid() == true) {
            return end;
        }
        else {
            message = getResourceMsg('daterangeerror');
            showToastrError(message);
            return null;
        }
    }

    //destroy old chart canvas
    var clearChartOldCanvas = function (chartName) {
        $("#"+chartName).replaceWith('<canvas id="'+ chartName +'"></canvas>'); 
    }

    var getCasinoStartTime = function (date) {
        var startTime = date.format(getResourceMsg('dateformat').toUpperCase()) + ' ' + getResourceMsg('casinostarttime');
        if (moment(startTime, getSysDateTimeFormat()).isValid())
            return moment(startTime, getSysDateTimeFormat());
        else
            return moment(datestr + '07:00:00', getSysDateTimeFormat());
    }

    var getCasinoEndTime = function (date) {
        var datestr = date.add(1, 'days').format(getResourceMsg('dateformat').toUpperCase()) + ' ';
        var endTime = datestr + getResourceMsg('casinoendtime');
        if (moment(endTime, getSysDateTimeFormat()).isValid())
            return moment(endTime, getSysDateTimeFormat());
        else
            return moment(datestr + '06:59:59', getSysDateTimeFormat());
    }

    var getParamFromGbiForm = function (data) {
        var formData = $('#gbiForm').serializeObject();
        if (formData['queryParam']) {
            data['queryParam'] = formData['queryParam'];
        }
        return data;
    }

    var getSysDateTimeFormat = function() {
        return getSysDateFormat() + " " + getResourceMsg('timeformat');
    }

    var getSysDateFormat = function() {
        return getResourceMsg('dateformat').toUpperCase();
    }

    //set date range value from local storage param object
    var setDateRangeFromLocalStorage = function (objName)
    {
        var obj = getLocalObject(objName);
        if (obj != null)
        {
            var start = obj["Start"];
            var end = obj["End"];
            if ( moment(start, getSysDateTimeFormat()).isValid() && moment(end, getSysDateTimeFormat()).isValid() ) 
            {
                $('input[name="daterange"]').val(start + " - " + end);
            }
        }
        return obj;
    }

    //set date value from local storage param object
    var setDateFromLocalStorage = function (objName) {
        var obj = getLocalObject(objName);
        if (obj != null) {
            var start = obj["Start"];
            if (moment(start, getSysDateTimeFormat()).isValid()) {
                $('input[name="daterange"]').val(start);
            }
        }
        return obj;
    }

    var initMoneyFormat = function () {
        accounting.settings = {
            currency: {
                symbol: getResourceMsg('currencysymbol'),  // default currency symbol is '$'
                format: "%s%v", // controls output: %s = symbol, %v = value/number
                decimal: getResourceMsg('currencydecimal'),     // decimal point separator
                thousand: getResourceMsg('currencythousand'),   // thousands separator
                precision: getResourceMsg('currencyprecision')  // decimal precision digits
            },
            number: {
                precision: getResourceMsg('currencyprecision'),  // default precision on numbers
                thousand: getResourceMsg('currencythousand'),
                decimal: getResourceMsg('currencydecimal'),
            }
        }
    }

    var initContextMenu = function () {
        $.contextMenu({
            selector: '.content-wrapper',
            build: function ($trigger, e) {
                // this callback is executed every time the menu is to be shown
                // its results are destroyed every time the menu is hidden
                // e is the original contextmenu event, containing e.pageX and e.pageY (amongst other data)
                return {
                    items: {
                        "add": { name: getResourceMsg('menuadd'), icon: "edit" },
                        "save": { name: getResourceMsg('menusave'), icon: "paste" },
                        "restore": { name: getResourceMsg('menurestore'), icon: "copy" },
                        "sep1": "---------",
                        "quit": { name: getResourceMsg('menuquit'), icon: function ($element, key, item) { return 'context-menu-icon context-menu-icon-quit'; } }
                    }
                };
            }
        });
    }

    var initDateRange = function () {
        //set initial value to date range
        //$('input[name="daterange"]').val(getCasinoStartTime(moment()).format('YYYY-MM-DD HH:mm:ss') + ' - ' + getCasinoEndTime(moment()).format('YYYY-MM-DD HH:mm:ss'));
        var daterangeLanguage = getResourceMsg('daterangelanguage');
        var maxdays = getResourceMsg('daterangelimit');

        var localeEs = {
            "format": getSysDateTimeFormat(),
            "applyLabel": getResourceMsg('DateRangeApply'),
            "cancelLabel": getResourceMsg('DateRangeCancel'),
            "fromLabel": getResourceMsg('DateRangeFrom'),
            "toLabel": getResourceMsg('DateRangeTo'),
            "customRangeLabel": getResourceMsg('DateRangeCustom'),
            "daysOfWeek": ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sá"],
            "monthNames": ["Enero", "Feb", "Marzo", "Abr", "Mayo", "Jun", "Jul", "Agosto", "Sept", "Oct", "Nov", "Dic"]
        }

        var localeEn = {
            "format": getSysDateTimeFormat(),
        }

        var rangeEsOptionsAll = {
            "Este Dia": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Ayer": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Los Ultimos 7 Días": [getCasinoStartTime(moment().subtract(6, 'days')), getCasinoEndTime(moment())],
            "Los Ultimos 30 Días": [getCasinoStartTime(moment().subtract(29, 'days')), getCasinoEndTime(moment())],
            "Este Mes": [getCasinoStartTime(moment().startOf('month')), getCasinoEndTime(moment().endOf('month'))],
            "El Mes Pasado": [getCasinoStartTime(moment().subtract(1, 'month').startOf('month')), getCasinoEndTime(moment().subtract(1, 'month').endOf('month'))]
        }

        var rangeEsOptions28 = {
            "Este Dia": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Ayer": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Los Ultimos 3 Días": [getCasinoStartTime(moment().subtract(2, 'days')), getCasinoEndTime(moment())],
            "Los Ultimos 7 Días": [getCasinoStartTime(moment().subtract(6, 'days')), getCasinoEndTime(moment())],
            "Los Ultimos 14 Días": [getCasinoStartTime(moment().subtract(29, 'days')), getCasinoEndTime(moment())],
        }

        var rangeEsOptions14 = {
            "Este Dia": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Ayer": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Los Ultimos 3 Días": [getCasinoStartTime(moment().subtract(2, 'days')), getCasinoEndTime(moment())],
            "Los Ultimos 7 Días": [getCasinoStartTime(moment().subtract(6, 'days')), getCasinoEndTime(moment())],
        }

        var rangeEsOptions7  = {
            "Este Dia": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Ayer": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Los Ultimos 3 Días": [getCasinoStartTime(moment().subtract(2, 'days')), getCasinoEndTime(moment())],
        }

        var rangeEsOptions3 = {
            "Este Dia": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Ayer": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Los Ultimos 2 Días": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment())],
        }
        
        var rangeEsOptions1 = {
            "Este Dia": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Ayer": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
        }

        var rangeEnOptionsAll = {
            "Today": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Yesterday": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Last 7 Days": [getCasinoStartTime(moment().subtract(6, 'days')), getCasinoEndTime(moment())],
            "Last 30 Days": [getCasinoStartTime(moment().subtract(29, 'days')), getCasinoEndTime(moment())],
            "This Month": [getCasinoStartTime(moment().startOf('month')), getCasinoEndTime(moment().endOf('month'))],
            "Last Month": [getCasinoStartTime(moment().subtract(1, 'month').startOf('month')), getCasinoEndTime(moment().subtract(1, 'month').endOf('month'))]
        }

        var rangeEnOptions28 = {
            "Today": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Yesterday": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Last 3 Days": [getCasinoStartTime(moment().subtract(2, 'days')), getCasinoEndTime(moment())],
            "Last 7 Days": [getCasinoStartTime(moment().subtract(6, 'days')), getCasinoEndTime(moment())],
            "Last 14 Days": [getCasinoStartTime(moment().subtract(29, 'days')), getCasinoEndTime(moment())],
        }

        var rangeEnOptions14 = {
            "Today": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Yesterday": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Last 3 Days": [getCasinoStartTime(moment().subtract(2, 'days')), getCasinoEndTime(moment())],
            "Last 7 Days": [getCasinoStartTime(moment().subtract(6, 'days')), getCasinoEndTime(moment())],
        }

        var rangeEnOptions7  = {
            "Today": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Yesterday": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Last 3 Days": [getCasinoStartTime(moment().subtract(2, 'days')), getCasinoEndTime(moment())],
        }

        var rangeEnOptions3 = {
            "Today": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Yesterday": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
            "Last 2 Days": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment())],
        }

        var rangeEnOptions1 = {
            "Today": [getCasinoStartTime(moment()), getCasinoEndTime(moment())],
            "Yesterday": [getCasinoStartTime(moment().subtract(1, 'days')), getCasinoEndTime(moment().subtract(1, 'days'))],
        }

        var rangeOptions;
        var localeOptions;
        if (daterangeLanguage == 'es') {
            localeOptions = localeEs;
            //if there is no date range max day limit or it is larger than 28, enable monthly date range default options
            if (typeof (maxdays) == 'undefined' || maxdays == 0 || maxdays > 29) {
                rangeOptions = rangeEsOptionsAll;
            }
            else if (maxdays > 14) {
                rangeOptions = rangeEsOptions28;
            }
            else if (maxdays > 7) {
                rangeOptions = rangeEsOptions14;
            }
            else if (maxdays > 3) {
                rangeOptions = rangeEsOptions7;
            }
            else if (maxdays > 1) {
                rangeOptions = rangeEsOptions3;
            }
            else {
                rangeOptions = rangeEsOptions1;
            }
        }
        else
        {
            localeOptions = localeEn;
            //if there is no date range max day limit or it is larger than 28, enable monthly date range default options
            if (typeof (maxdays) == 'undefined' || maxdays == 0 || maxdays > 28) {
                rangeOptions = rangeEnOptionsAll;
            }
            else if (maxdays > 14) {
                rangeOptions = rangeEnOptions28; //less than 28 days
            }
            else if (maxdays > 7) {
                rangeOptions = rangeEnOptions14; //less than 14 days
            }
            else if (maxdays > 3) {
                rangeOptions = rangeEnOptions7; //less than 7 days
            }
            else if (maxdays > 1) {
                rangeOptions = rangeEnOptions3;
            }
            else {
                rangeOptions = rangeEnOptions1;
            }
        }

        $('input[name="daterange"]').daterangepicker({
            "timePicker": true,
            "timePicker24Hour": true,
            "timePickerSeconds": true,
            "locale": localeOptions,
            "ranges": rangeOptions,
            //startDate: getCasinoStartTime(moment()),
            //endDate: getCasinoEndTime(moment())
        }, function (start, end, label) {
            end = getCasinoEndTime(moment(end).subtract(1, 'days'));
            $('input[name="daterange"]').html(start.format(getSysDateTimeFormat()) + ' - ' + end.format(getSysDateTimeFormat()));
        });

        $('input[name="daterange"]').change(updateButtonAutoRefresh); // check AutoRefresh button
        $('#btDateRange').on('click', function () { // click the image will triger the daterange picker
            $('input[name="daterange"]').focus();
        });
        if (isClientMobile()) {
            $('input[name="daterange"]').focus(hideMobileKeyboard);
        }
    }

    var initSingleDate = function () {
        //set initial value to date range
        //$('input[name="daterange"]').val(getCasinoStartTime(moment()).format('YYYY-MM-DD HH:mm:ss') + ' - ' + getCasinoEndTime(moment()).format('YYYY-MM-DD HH:mm:ss'));
        var daterangeLanguage = getResourceMsg('daterangelanguage');
        if (daterangeLanguage == 'es') {
            $('input[name="daterange"]').daterangepicker({
                    singleDatePicker: true,
                    "locale": {
                        "format": getSysDateFormat(),
                        "daysOfWeek": ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sá"],
                        "monthNames": ["Enero", "Feb", "Marzo", "Abr", "Mayo", "Jun", "Jul", "Agosto", "Sept", "Oct", "Nov", "Dic"]
                    },
                    showDropdowns: true,
                },
                function (start, end, label) {
                    $('input[name="daterange"]').html(start.format(getSysDateFormat()));
                }
            );
        } 
        else {//english
            $('input[name="daterange"]').daterangepicker({
                    singleDatePicker: true,
                    "locale": {
                        "format": getSysDateFormat(),
                    },
                    showDropdowns: true,
                },
                function (start, end, label) {
                    $('input[name="daterange"]').html(start.format(getSysDateFormat()));
                }
            );
        }

        $('input[name="daterange"]').change(updateButtonAutoRefresh); // check AutoRefresh button
        $('#btDateRange').on('click', function () { // click the image will triger the daterange picker
            $('input[name="daterange"]').focus();
        });
        if (isClientMobile()) {
            $('input[name="daterange"]').focus(hideMobileKeyboard);
        }
    }


    var hideMobileKeyboard = function () {
        $('input[name="daterange"]').blur();
    }

    var isClientMobile = function () {
        if (/Android|webOS|iPhone|iPad|iPod|pocket|psp|kindle|avantgo|blazer|midori|Tablet|Palm|maemo|plucker|phone|BlackBerry|symbian|IEMobile|mobile|ZuneWP7|Windows Phone|Opera Mini/i.test(navigator.userAgent)) {
            return true;
        };
        return false;
    }

    /* Chart Option */
    // -------
    // ChartJS - Here we will create a few options for area charts and donut chart using ChartJS
    // -------
    var getDonutOptions = function () {
        return {
            //Boolean - Whether we should show a stroke on each segment
            segmentShowStroke: true,
            //String - The colour of each segment stroke
            segmentStrokeColor: "#fff",
            //Number - The width of each segment stroke
            segmentStrokeWidth: 2,
            //Number - The percentage of the chart that we cut out of the middle
            percentageInnerCutout: 50, // This is 0 for Pie charts
            //Number - Amount of animation steps
            animationSteps: 100,
            //String - Animation easing effect
            animationEasing: "easeOutBounce",
            //Boolean - Whether we animate the rotation of the Doughnut
            animateRotate: true,
            //Boolean - Whether we animate scaling the Doughnut from the centre
            animateScale: false,
            //Boolean - whether to make the chart responsive to window resizing
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>",
            //String - A tooltip template
            tooltipTemplate: "<%=value %> <%=label%>",
            //onAnimationComplete: function () {
            //    this.showTooltip(this.segments, true);
            //    //Show tooltips in bar chart (issue: multiple datasets doesnt work http://jsfiddle.net/5gyfykka/14/)
            //    //this.showTooltip(this.datasets[0].bars, true);

            //    //Show tooltips in line chart (issue: multiple datasets doesnt work http://jsfiddle.net/5gyfykka/14/)
            //    //this.showTooltip(this.datasets[0].points, true);  
            //},
            //tooltipEvents: [],
            //showTooltips: true,
        };
    }

    var getDonutNoAnimationOptions = function () {
        return {
            //Boolean - Whether we should show a stroke on each segment
            segmentShowStroke: true,
            //String - The colour of each segment stroke
            segmentStrokeColor: "#fff",
            //Number - The width of each segment stroke
            segmentStrokeWidth: 2,
            //Number - The percentage of the chart that we cut out of the middle
            percentageInnerCutout: 50, // This is 0 for Pie charts
            //Turn off animation
            animation: false,
            //Boolean - whether to make the chart responsive to window resizing
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>",
            //String - A tooltip template
            tooltipTemplate: "<%=value %> <%=label%>",
            //onAnimationComplete: function () {
            //    this.showTooltip(this.segments, true);
            //    //Show tooltips in bar chart (issue: multiple datasets doesnt work http://jsfiddle.net/5gyfykka/14/)
            //    //this.showTooltip(this.datasets[0].bars, true);

            //    //Show tooltips in line chart (issue: multiple datasets doesnt work http://jsfiddle.net/5gyfykka/14/)
            //    //this.showTooltip(this.datasets[0].points, true);  
            //},
            //tooltipEvents: [],
            //showTooltips: true,
        };
    }

    var getBarChartOptions = function () {
        return {
            //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
            scaleBeginAtZero: true,
            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: true,
            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",
            //Number - Width of the grid lines
            scaleGridLineWidth: 1,
            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,
            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,
            //Boolean - If there is a stroke on each bar
            barShowStroke: true,
            //Number - Pixel width of the bar stroke
            barStrokeWidth: 2,
            //Number - Spacing between each of the X value sets
            barValueSpacing: 5,
            //Number - Spacing between data sets within X values
            barDatasetSpacing: 1,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
            // String - Template string for single tooltips
            tooltipTemplate: function (v) { return accounting.formatMoney(v.value); },
            // String - Template string for multiple tooltips
            multiTooltipTemplate: function (v) { return accounting.formatMoney(v.value); },
            //Boolean - whether to make the chart responsive
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
        };
    }

    var getBarChartNoAnimationOptions = function () {
        return {
            //Turn off animation
            animation: false,
            //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
            scaleBeginAtZero: true,
            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: true,
            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",
            //Number - Width of the grid lines
            scaleGridLineWidth: 1,
            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,
            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,
            //Boolean - If there is a stroke on each bar
            barShowStroke: true,
            //Number - Pixel width of the bar stroke
            barStrokeWidth: 2,
            //Number - Spacing between each of the X value sets
            barValueSpacing: 5,
            //Number - Spacing between data sets within X values
            barDatasetSpacing: 1,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
            // String - Template string for single tooltips
            tooltipTemplate: function (v) { return accounting.formatMoney(v.value); },
            // String - Template string for multiple tooltips
            multiTooltipTemplate: function (v) { return accounting.formatMoney(v.value); },
            //Boolean - whether to make the chart responsive
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
        };
    }

    var getAreaChartOptions = function () {
        return {
            //Boolean - If we should show the scale at all
            showScale: true,
            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: false,
            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",
            //Number - Width of the grid lines
            scaleGridLineWidth: 1,
            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,
            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,
            //Boolean - Whether the line is curved between points
            bezierCurve: true,
            //Number - Tension of the bezier curve between points
            bezierCurveTension: 0.3,
            //Boolean - Whether to show a dot for each point
            pointDot: false,
            //Number - Radius of each point dot in pixels
            pointDotRadius: 4,
            //Number - Pixel width of point dot stroke
            pointDotStrokeWidth: 1,
            //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
            pointHitDetectionRadius: 20,
            //Boolean - Whether to show a stroke for datasets
            datasetStroke: true,
            //Number - Pixel width of dataset stroke
            datasetStrokeWidth: 2,
            //Boolean - Whether to fill the dataset with a color
            datasetFill: true,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
            //Boolean - whether to make the chart responsive
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
            // String - Template string for single tooltips
            tooltipTemplate: function (v) { return v.label + ' : ' + accounting.formatMoney(v.value); },
            // String - Template string for multiple tooltips
            multiTooltipTemplate: function (v) { return v.label + ' : ' + accounting.formatMoney(v.value); },
        };
    }

    var getAreaChartNoAnimationOptions = function () {
        return {
            //Turn off animation
            animation: false,
            //Boolean - If we should show the scale at all
            showScale: true,
            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: false,
            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",
            //Number - Width of the grid lines
            scaleGridLineWidth: 1,
            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,
            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,
            //Boolean - Whether the line is curved between points
            bezierCurve: true,
            //Number - Tension of the bezier curve between points
            bezierCurveTension: 0.3,
            //Boolean - Whether to show a dot for each point
            pointDot: false,
            //Number - Radius of each point dot in pixels
            pointDotRadius: 4,
            //Number - Pixel width of point dot stroke
            pointDotStrokeWidth: 1,
            //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
            pointHitDetectionRadius: 20,
            //Boolean - Whether to show a stroke for datasets
            datasetStroke: true,
            //Number - Pixel width of dataset stroke
            datasetStrokeWidth: 2,
            //Boolean - Whether to fill the dataset with a color
            datasetFill: true,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
            //Boolean - whether to make the chart responsive
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
            // String - Template string for single tooltips
            tooltipTemplate: function (v) { return v.label + ' : ' + accounting.formatMoney(v.value); },
            // String - Template string for multiple tooltips
            multiTooltipTemplate: function (v) { return v.label + ' : ' + accounting.formatMoney(v.value); },
        };
    }

    var getAreaChartPercentageOptions = function () {
        return {
            //Boolean - If we should show the scale at all
            showScale: true,
            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: false,
            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",
            //Number - Width of the grid lines
            scaleGridLineWidth: 1,
            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,
            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,
            //Boolean - Whether the line is curved between points
            bezierCurve: true,
            //Number - Tension of the bezier curve between points
            bezierCurveTension: 0.3,
            //Boolean - Whether to show a dot for each point
            pointDot: false,
            //Number - Radius of each point dot in pixels
            pointDotRadius: 4,
            //Number - Pixel width of point dot stroke
            pointDotStrokeWidth: 1,
            //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
            pointHitDetectionRadius: 20,
            //Boolean - Whether to show a stroke for datasets
            datasetStroke: true,
            //Number - Pixel width of dataset stroke
            datasetStrokeWidth: 2,
            //Boolean - Whether to fill the dataset with a color
            datasetFill: true,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
            //Boolean - whether to make the chart responsive
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
            // String - Template string for single tooltips
            tooltipTemplate: function (v) { return v.label + ' : ' + v.value + ' %'; },
            // String - Template string for multiple tooltips
            multiTooltipTemplate: function (v) { return v.label + ' : ' + v.value + ' %'; },
        };
    }

    var getAreaChartPercentageNoAnimationOptions = function () {
        return {
            //Turn off animation
            animation: false,
            //Boolean - If we should show the scale at all
            showScale: true,
            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: false,
            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",
            //Number - Width of the grid lines
            scaleGridLineWidth: 1,
            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,
            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,
            //Boolean - Whether the line is curved between points
            bezierCurve: true,
            //Number - Tension of the bezier curve between points
            bezierCurveTension: 0.3,
            //Boolean - Whether to show a dot for each point
            pointDot: false,
            //Number - Radius of each point dot in pixels
            pointDotRadius: 4,
            //Number - Pixel width of point dot stroke
            pointDotStrokeWidth: 1,
            //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
            pointHitDetectionRadius: 20,
            //Boolean - Whether to show a stroke for datasets
            datasetStroke: true,
            //Number - Pixel width of dataset stroke
            datasetStrokeWidth: 2,
            //Boolean - Whether to fill the dataset with a color
            datasetFill: true,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
            //Boolean - whether to make the chart responsive
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
            // String - Template string for single tooltips
            tooltipTemplate: function (v) { return v.label + ' : ' + v.value + ' %'; },
            // String - Template string for multiple tooltips
            multiTooltipTemplate: function (v) { return v.label + ' : ' + v.value + ' %'; },
        };
    }

    var getAreaChartNumberOptions = function () {
        return {
            //Boolean - If we should show the scale at all
            showScale: true,
            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: false,
            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",
            //Number - Width of the grid lines
            scaleGridLineWidth: 1,
            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,
            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,
            //Boolean - Whether the line is curved between points
            bezierCurve: true,
            //Number - Tension of the bezier curve between points
            bezierCurveTension: 0.3,
            //Boolean - Whether to show a dot for each point
            pointDot: false,
            //Number - Radius of each point dot in pixels
            pointDotRadius: 4,
            //Number - Pixel width of point dot stroke
            pointDotStrokeWidth: 1,
            //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
            pointHitDetectionRadius: 20,
            //Boolean - Whether to show a stroke for datasets
            datasetStroke: true,
            //Number - Pixel width of dataset stroke
            datasetStrokeWidth: 2,
            //Boolean - Whether to fill the dataset with a color
            datasetFill: true,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
            //Boolean - whether to make the chart responsive
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
            // String - Template string for single tooltips
            tooltipTemplate: function (v) { return v.label + ' : ' + v.value; },
            // String - Template string for multiple tooltips
            multiTooltipTemplate: function (v) { return v.label + ' : ' + v.value; },
        };
    }

    var getAreaChartNumberNoAnimationOptions = function () {
        return {
            //Turn off animation
            animation: false,
            //Boolean - If we should show the scale at all
            showScale: true,
            //Boolean - Whether grid lines are shown across the chart
            scaleShowGridLines: false,
            //String - Colour of the grid lines
            scaleGridLineColor: "rgba(0,0,0,.05)",
            //Number - Width of the grid lines
            scaleGridLineWidth: 1,
            //Boolean - Whether to show horizontal lines (except X axis)
            scaleShowHorizontalLines: true,
            //Boolean - Whether to show vertical lines (except Y axis)
            scaleShowVerticalLines: true,
            //Boolean - Whether the line is curved between points
            bezierCurve: true,
            //Number - Tension of the bezier curve between points
            bezierCurveTension: 0.3,
            //Boolean - Whether to show a dot for each point
            pointDot: false,
            //Number - Radius of each point dot in pixels
            pointDotRadius: 4,
            //Number - Pixel width of point dot stroke
            pointDotStrokeWidth: 1,
            //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
            pointHitDetectionRadius: 20,
            //Boolean - Whether to show a stroke for datasets
            datasetStroke: true,
            //Number - Pixel width of dataset stroke
            datasetStrokeWidth: 2,
            //Boolean - Whether to fill the dataset with a color
            datasetFill: true,
            //String - A legend template
            legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
            //Boolean - whether to make the chart responsive
            responsive: true,
            // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
            maintainAspectRatio: true,
            // String - Template string for single tooltips
            tooltipTemplate: function (v) { return v.label + ' : ' + v.value; },
            // String - Template string for multiple tooltips
            multiTooltipTemplate: function (v) { return v.label + ' : ' + v.value; },
        };
    }

    var InitToastr = function () {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-right",
            "preventDuplicates": true,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        }
    }

    var showToastrError = function (message) {
        toastr.clear(); //clean previous toastr error
        toastr["error"](message);
    }
    var getBrowserName = function () {
        return getAcrobatInfo.getBrowserName();
    };

    var isAcrobatInstalled = function () {
        return getAcrobatInfo.isAcrobatInstalled();
    };

    var getAcrobatVersion = function () {
        return getAcrobatInfo.getAcrobatVersion();
    }

    return {
        errorInit: errorInit,
        handleDragStop: handleDragStop,
        setAutoRefreshOnOff: setAutoRefreshOnOff,
        enableCheckAutoRefresh: enableCheckAutoRefresh,
        updateButtonAutoRefresh: updateButtonAutoRefresh,
        setAutoRefreshOff: setAutoRefreshOff,
        setAutoRefreshOn: setAutoRefreshOn,
        closeDefaultParameterWarningModal: closeDefaultParameterWarningModal,

        checkDateRangeValue: checkDateRangeValue,
        checkSingleDateParameter: checkSingleDateParameter,
        getDiffInDays: getDiffInDays,

        strToDateTime: strToDateTime,
        initMoneyFormat: initMoneyFormat,
        toMoneyStr: toMoneyStr,
        getResourceMsg: getResourceMsg,
        setResourceMsg: setResourceMsg,
        getParamFromGbiForm: getParamFromGbiForm,

        getDonutOptions: getDonutOptions,
        getBarChartOptions: getBarChartOptions,
        getAreaChartOptions: getAreaChartOptions,
        getAreaChartNumberOptions: getAreaChartNumberOptions,
        getAreaChartPercentageOptions: getAreaChartPercentageOptions,

        getDonutNoAnimationOptions: getDonutNoAnimationOptions,
        getBarChartNoAnimationOptions: getBarChartNoAnimationOptions,
        getAreaChartNoAnimationOptions: getAreaChartNoAnimationOptions,
        getAreaChartNumberNoAnimationOptions: getAreaChartNumberNoAnimationOptions,
        getAreaChartPercentageNoAnimationOptions: getAreaChartPercentageNoAnimationOptions,

        getPDFMargins: getPDFMargins,
        getPDFStyles: getPDFStyles,
        getPDFTableLayout: getPDFTableLayout,

        getDateRangeStartTime: getDateRangeStartTime,
        getDateRangeEndTime: getDateRangeEndTime,
        getCasinoStartTime: getCasinoStartTime,
        getCasinoEndTime: getCasinoEndTime,
        initDateRange: initDateRange,
        initSingleDate: initSingleDate,
        initContextMenu: initContextMenu,
        InitToastr: InitToastr,

        getSysDateTimeFormat: getSysDateTimeFormat,
        getSysDateFormat: getSysDateFormat,
        displayDatePart: displayDatePart,
        displayDatePartFromSpecialFormat: displayDatePartFromSpecialFormat,

        showQueryWaitingInfo: showQueryWaitingInfo,
        clearQueryWaitingInfo: clearQueryWaitingInfo,
        clearChartOldCanvas: clearChartOldCanvas,
        showToastrError: showToastrError,

        getLocalObject: getLocalObject,
        setLocalObject: setLocalObject,
        setDateRangeFromLocalStorage: setDateRangeFromLocalStorage,
        setDateFromLocalStorage: setDateFromLocalStorage,

        getColor: getColor,
        getBgColor: getBgColor,
        isClientMobile: isClientMobile,
        hideMobileKeyboard: hideMobileKeyboard,
        exchangeElement: exchangeElement,

        isAcrobatInstalled: isAcrobatInstalled,
        getAcrobatVersion: getAcrobatVersion,
        getBrowserName: getBrowserName,
    };
}();

// SessionManager Module
var SessionManager = function() {
    var sessionTimeoutSeconds = (gbService.getResourceMsg('sessiontimeout') * 60),
        countdownSeconds = gbService.getResourceMsg('warningbeforesessiontimeout') * 60,
        secondsBeforePrompt = sessionTimeoutSeconds - countdownSeconds,
        displayCountdownIntervalId,
        promptToExtendSessionTimeoutId,
        originalTitle = document.title,
        count = countdownSeconds < 0? 0 : countdownSeconds,
        extendSessionUrl = '/Session/Extend',
        expireSessionUrl = '/Session/Expire?returnUrl=' + location.pathname;

    var endSession = function() {
        location.href = expireSessionUrl;
    };

    var displayCountdown = function() {
        var countdown = function() {
            var hourStr = gbService.getResourceMsg('sessionhour');
            var minuteStr = gbService.getResourceMsg('sessionminute');
            var secondStr = gbService.getResourceMsg('sessionsecond')
            var cd = new Date(count * 1000),
                hours = cd.getUTCHours(),
                minutes = cd.getUTCMinutes(),
                seconds = cd.getUTCSeconds(),
                hoursDisplay = hours == 1 ? ' 1 ' + hourStr : hours === 0 ? '' : ' ' + hours + ' '+ hourStr +'s',
                minutesDisplay = minutes === 1 ? ' 1 ' + minuteStr : minutes === 0 ? '' : ' ' + minutes + ' '+ minuteStr +'s',
                secondsDisplay = seconds === 1 ? ' 1 ' + secondStr : ' ' + seconds + ' ' + secondStr +'s',
                cdDisplay = hoursDisplay + minutesDisplay + secondsDisplay;

            document.title = gbService.getResourceMsg('sessionexpirein') + ' ' +
                StringHelpers.padLeft(minutes, '00') + ':' + StringHelpers.padLeft(seconds, '00');
            $('#sm-countdowntimer').text( cdDisplay );
            if (count === 0) {
                document.title = gbService.getResourceMsg('sessionexpired');
                endSession();
            }
            count--;
        };
        countdown();
        displayCountdownIntervalId = setInterval(countdown, 1000);
    };

    var promptToExtendSession = function() {
        $('#sm-countdown-dialog')
            .modal({backdrop: 'static'}) //disable backdrop so user need to make choice
            .one('click', '[data-value]', function (e) {
                e.preventDefault();
                if ($(this).data('value')) { //Continue
                    refreshSession();
                    document.title = originalTitle;
                    $('#sm-countdown-dialog').modal('hide'); //close the modal
                }
                else { //Logout
                    endSession(false);
                }
            });
        count = countdownSeconds;
        displayCountdown();
    };

    var refreshSession = function() {
        if ( !$('#sm-countdown-dialog').hasClass('in') ) //check if the popup modal is open
        {
            clearInterval(displayCountdownIntervalId);
            var img = new Image(1, 1);
            img.src = extendSessionUrl;
            clearTimeout(promptToExtendSessionTimeoutId);
            startSessionManager();
        }
    };

    var startSessionManager = function() {
        clearTimeout(promptToExtendSessionTimeoutId);
        promptToExtendSessionTimeoutId =
            setTimeout(promptToExtendSession, secondsBeforePrompt * 1000);
    };

    // Public Functions
    return {
        start: function() {
            startSessionManager();
        },
        extend: function() {
            refreshSession();
        }
    };
}();

//
// http://thecodeabode.blogspot.com
// @author: Ben Kitzelman
// @license:  FreeBSD: (http://opensource.org/licenses/BSD-2-Clause) Do whatever you like with it
// @updated: 03-03-2013
var getAcrobatInfo = function () {

    var getBrowserName = function () {
        return this.name = this.name || function () {
            var userAgent = navigator ? navigator.userAgent.toLowerCase() : "other";

            if (userAgent.indexOf("chrome") > -1) return "chrome";
            else if (userAgent.indexOf("safari") > -1) return "safari";
            else if (userAgent.indexOf("msie") > -1) return "ie";
            else if (userAgent.indexOf("firefox") > -1) return "firefox";
            return userAgent;
        }();
    };

    var getActiveXObject = function (name) {
        try { return new ActiveXObject(name); } catch (e) { }
    };

    var getNavigatorPlugin = function (name) {
        for (key in navigator.plugins) {
            var plugin = navigator.plugins[key];
            if (plugin.name == name) return plugin;
        }
    };

    var getPDFPlugin = function () {
        return this.plugin = this.plugin || function () {
            if (getBrowserName() == 'ie') {
                //
                // load the activeX control
                // AcroPDF.PDF is used by version 7 and later
                // PDF.PdfCtrl is used by version 6 and earlier
                return getActiveXObject('AcroPDF.PDF') || getActiveXObject('PDF.PdfCtrl');
            }
            else {
                return getNavigatorPlugin('Adobe Acrobat') || getNavigatorPlugin('Chrome PDF Viewer') || getNavigatorPlugin('WebKit built-in PDF');
            }
        }();
    };

    var isAcrobatInstalled = function () {
        return !!getPDFPlugin();
    };

    var getAcrobatVersion = function () {
        try {
            var plugin = getPDFPlugin();

            if (getBrowserName() == 'ie') {
                var versions = plugin.GetVersions().split(',');
                var latest = versions[0].split('=');
                return parseFloat(latest[1]);
            }

            if (plugin.version) return parseInt(plugin.version);
            return plugin.name

        }
        catch (e) {
            return null;
        }
    }

    //
    // The returned object
    // 
    return {
        browser: getBrowserName(),
        acrobat: isAcrobatInstalled() ? 'installed' : false,
        acrobatVersion: getAcrobatVersion()
    };
};