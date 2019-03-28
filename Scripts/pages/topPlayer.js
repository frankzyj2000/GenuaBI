"use strict";

var topPlayer = function () {
    var paramName = 'paramTopPlayers';
    var showWatingInfoTimes = 0;
    var saveParamToLocal = function (data) {
        var start = gbService.getDateRangeStartTime(); //string
        var end = gbService.getDateRangeEndTime(); //sring
        var pageLen = gbService.getResourceMsg('pagelength');
        if (typeof data != "undefined" && data !=null && 'length' in data) pageLen = data["length"];
        //Define Parameters to be saved in local storage
        if (isParameterValid(start, end)) {
            var numPlayer = checkNumPlayer();
            var numVisit = checkNumVisit();
            gbService.setResourceMsg('pagelength', pageLen); //need to save the page length here
            var paramData = { "Start": start, "End": end, "PageLen": pageLen, "NumPlayer": numPlayer, "NumVisit": numVisit };
            gbService.setLocalObject(paramName, paramData);
        }
    }

    var loadParamFromLocal = function () {
        //get start,end parameters from local storage
        var param = gbService.setDateRangeFromLocalStorage(paramName);
        if (param != null) {
            gbService.setResourceMsg('pagelength', param['PageLen']);
            $('input[name="queryParam[numberOfPlayers]"]').val(param['NumPlayer']);
            $('input[name="queryParam[numberOfVisits]"]').val(param['NumVisit']);
        }
    }

    var isParameterValid = function (start, end) {
        if (start == null || end == null) return false; //date format is wrong    
        if (!gbService.checkDateRangeValue(start, end)) return false;
        //get other parameters
        var numPlayer = checkNumPlayer();
        var param = gbService.getLocalObject(paramName);
        if (numPlayer == 0) {
            //reload the parameter from local storage
            $('input[name="queryParam[numberOfPlayers]"]').val(param['NumPlayer']); 
            return false;
        }
        var numVisit = checkNumVisit();
        if (numVisit == -1) {
            //reload the parameter from local storage
            $('input[name="queryParam[numberOfVisits]"]').val(param['NumVisit']);
            return false;
        }
        return true;
    }

    var getTopPlayerList = function (start, end, numPlayer, numVisit, callback) {
        $.ajax({
            type: "GET",
            url: "/Dashboard/GetTopPlayers",
            dataType: "json",
            data: {
                start: start,
                end: end,
                numPlayer: numPlayer,
                numVisit: numVisit
            },
            success: function (data) {
                data = gbService.getParamFromGbiForm(data); // get parameters from local storage
                if (callback) callback(data);
            }
        });
    };

    var checkNumPlayer = function () {
        var numPlayer = $('input[name="queryParam[numberOfPlayers]"]').val();
        if (numPlayer == '' || numPlayer.indexOf(".") != -1) {
            gbService.showToastrError(gbService.getResourceMsg('playernumneeded'));
            return 0;
        }

        numPlayer = parseInt(numPlayer) || 0;
        if (numPlayer <= 0) {
            gbService.showToastrError(gbService.getResourceMsg('playernumneeded'));
            return 0;
        }
        if (numPlayer > parseInt( gbService.getResourceMsg('maxplayers') ))
        {
            var message = gbService.getResourceMsg('playeroutofmaximum');
            message = message.replace('{0}', gbService.getResourceMsg('maxplayers'));
            gbService.showToastrError(message);
            return 0;
        }
        return numPlayer;
    }

    var checkNumVisit = function () {
        var numVisit = $('input[name="queryParam[numberOfVisits]"]').val();
        if (numVisit == '' || numVisit.indexOf(".") != -1) {
            gbService.showToastrError(gbService.getResourceMsg('visitnumneeded'));
            return -1;
        }
        numVisit = parseInt(numVisit) || 0; //NumOfVisit can be zero
        if (numVisit < 0) {
            gbService.showToastrError(gbService.getResourceMsg('visitnumneeded'));
            return -1;
        }
        if (numVisit > parseInt(gbService.getResourceMsg('maxvisits'))) {
            var message = gbService.getResourceMsg('visitoutofmaximum');
            message = message.replace('{0}', gbService.getResourceMsg('maxvisits'));
            gbService.showToastrError(message);
            return -1;
        }
        return numVisit;
    }

    //only diaplay day part
    var getDayPart = function (value) {
        var str = String(value);
        return str.substring(0, str.indexOf(" "));
    }

    //Creating DataTable
    var dataTableInit = function (datalist) {
        var table;
        var lengthMenu = [[10, 25, 50, -1], [10, 25, 50, gbService.getResourceMsg('all')]];

        var ajaxCall = {
            url: "/Dashboard/TopPlayerDataTableHandler",
            type: "POST",
            data: function (data)
            {
                saveParamToLocal(data); //save parameters to local storage
                return data = gbService.getParamFromGbiForm(data);
            }
        };

        var buttons = [
                    { extend: 'colvis', text: gbService.getResourceMsg('btnchoosecolumn'), columns: ':not(:first-child)' },
                    { extend: 'excel', text: gbService.getResourceMsg('btnexcel') },
        ];

        var columnDefs = [
                {
                    targets: 4, /*column index counting from the left*/
                    className: "dt-right", /* Right align text in the header and body */
                },
                { targets: 5, className: "dt-right", render: function (data, type, row) { return gbService.toMoneyStr(data); }, },
                { targets: 6, className: "dt-right", render: function (data, type, row) { return gbService.toMoneyStr(data); }, },
                { targets: 7, className: "dt-right", },
                { targets: 8, className: "dt-right", },
        ];

        // define column content
        var columns = [
                { data: "PlayerName" },
                { data: "BirthdayDate", render: function (data, type, full) { return gbService.displayDatePart(data); } },
                { data: "Gender" },
                { data: "LastVisit", render: function (data, type, full) { return gbService.displayDatePartFromSpecialFormat(data, "YYYYMMDD"); } },
                { data: "TotalVisits" },
                { data: "WinLoss" },
                { data: "Handle" },
                { data: "Telofono" },
                { data: "Celular" },
                { data: "Email" },
        ];

        //define dom for the structure of the datatable
        var dom = "<'row'<'col-sm-3'l><'col-sm-5 text-center'B><'col-sm-4'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-5'i><'col-sm-7'p>>";

        //specified using server side processing for huge table
        if (gbService.getResourceMsg('servercache').toLowerCase() == 'true') 
        {
            table = $('#topplayerlist').DataTable({
                language: { "url": gbService.getResourceMsg('datatablelanguage') },
                destroy: true,
                processing: true,
                serverSide: true,
                ordering: true,
                info: true,
                paging: true,
                deferRender: true,
                searching: true,
                //scrollX: true, //removed for version 1.10.10
                responsive: true,
                lengthMenu: lengthMenu,
                pageLength: parseInt(gbService.getResourceMsg('pagelength')),
                ajax: ajaxCall,
                order: [[5, 'asc']],
                preDrawCallback: function () {
                    //show query waiting info
                    if ((showWatingInfoTimes % 2) == 0) {
                        gbService.showQueryWaitingInfo();
                    };
                    showWatingInfoTimes++;
                },
                drawCallback: function () {
                    gbService.clearQueryWaitingInfo(); //clear query waiting message
                },
                dom: dom,
                buttons: buttons,
                columnDefs: columnDefs,
                columns: columns,
            });
        }
        else //DataTable init with local cache
        {
            table = $('#topplayerlist').DataTable({
                language: { "url": gbService.getResourceMsg('datatablelanguage') },
                destroy: true,
                processing: true,
                ordering: true,
                data: datalist,
                info: true,
                paging: true,
                deferRender: true,
                searching: true,
                //"scrollX": true, //removed for version 1.10.10
                responsive: true,
                lengthMenu: lengthMenu,
                pageLength: parseInt(gbService.getResourceMsg('pagelength')),
                stateSave: true,
                order: [[5, 'asc']],
                preDrawCallback: function () {
                    //show query waiting info
                    if ((showWatingInfoTimes % 2) == 0) {
                        gbService.showQueryWaitingInfo();
                    };
                    showWatingInfoTimes++;
                },
                drawCallback: function () {
                    gbService.clearQueryWaitingInfo(); //clear query waiting message
                },
                dom: dom,
                buttons: buttons,
                columnDefs: columnDefs,
                columns: columns,
            });
        }
    }

    var loadAllData = function (data) {
        //Save parameters to local storage
        saveParamToLocal(data);
        //ChangeTotal(data);
        if (data != null) {
            dataTableInit(data.TopPlayerList);
        }
        else {
            dataTableInit(null);
        }       
    }

    var loadPageContent = function () {
        SessionManager.start();
        // Whenever mouse move or key in, extend the session,
        // since we know the user is interacting with the site.
        document.onmousemove = SessionManager.extend;
        document.onkeypress = SessionManager.extend;

        //get start,end parameters
        var start = gbService.getDateRangeStartTime(); //string
        var end = gbService.getDateRangeEndTime(); //sring
        if (!isParameterValid(start, end)) return;

        $('input[name="queryParam[start]"]').val(start);
        $('input[name="queryParam[end]"]').val(end);

        //Loading DataTable 
        if (gbService.getResourceMsg('servercache').toLowerCase() == 'true') {
            loadAllData(null);
        }
        else {
            var numPlayer = checkNumPlayer();
            var numVisit = checkNumVisit();
            getTopPlayerList(start, end, numPlayer, numVisit, loadAllData);
        }
    }

    var getTableColumns = function () {
        var tableColumns = [];
        tableColumns.push({ text: gbService.getResourceMsg('columnplayername'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnbirthday'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columngender'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnlastvisit'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columntotalvisit'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnwinloss'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnhandle'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columntelephone'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnmobile'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnemail'), style: 'tableheader' });     
        return tableColumns;
    }

    //var getTableTotal = function (data, fistcolumnstyle, style) {
    //    var tableTotal = [];
    //    tableTotal.push({ text: gbService.getResourceMsg('total'), style: fistcolumnstyle });
    //    tableTotal.push({ text: gbService.toMoneyStr(data.TotalMoneyIn), style: style });
    //    tableTotal.push({ text: gbService.toMoneyStr(data.TotalMoneyOut), style: style });
    //    tableTotal.push({ text: gbService.toMoneyStr(data.TotalHandPayments), style: style });
    //    tableTotal.push({ text: gbService.toMoneyStr(data.TotalDPromotion), style: style });
    //    tableTotal.push({ text: gbService.toMoneyStr(data.TotalNetWin), style: style });
    //    tableTotal.push({ text: gbService.toMoneyStr(data.TotalWin), style: style });
    //    tableTotal.push({ text: data.TotalCantPlayer.toString(), style: style });
    //    return tableTotal;
    //}

    var getTableBody = function (data) {
        var tableBody = [];
        tableBody.push(getTableColumns());

        var list = data.TopPlayerList;
        var firstcolumnstyle = 'tablefirstcolumn1';
        var valueStyle = 'tablevalue1';
        for (var i = 0; i < list.length; i++) {
            var tableRow = [];
            if (i % 2 == 0) {
                firstcolumnstyle = 'tablefirstcolumn1'
                valueStyle = 'tablevalue1';
            }
            else {
                firstcolumnstyle = 'tablefirstcolumn2'
                valueStyle = 'tablevalue2';
            };
            tableRow.push({ text: list[i]['PlayerName'], style: firstcolumnstyle });
            tableRow.push({ text: gbService.displayDatePart(list[i]['BirthdayDate']), style: valueStyle });
            tableRow.push({ text: list[i]['Gender'] == null ? "" : list[i]['Gender'], style: valueStyle });
            tableRow.push({ text: gbService.displayDatePartFromSpecialFormat(list[i]['LastVisit'], "YYYYMMDD"), style: valueStyle });
            tableRow.push({ text: list[i]['TotalVisits'].toString(), style: valueStyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['WinLoss']), style: valueStyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['Handle']), style: valueStyle });
            tableRow.push({ text: list[i]['Telofono'] == null ? "" : list[i]['Telofono'], style: valueStyle });
            tableRow.push({ text: list[i]['Celular'] == null ? "" : list[i]['Celular'], style: valueStyle });
            tableRow.push({ text: list[i]['Email'] == null ? "" : list[i]['Email'], style: valueStyle });
            tableBody.push(tableRow); //add the tableRow to tableBody
        }
        //if (list.length % 2 == 0) {
        //    firstcolumnstyle = 'tablefirstcolumn1'
        //    valueStyle = 'tablevalue1';
        //}
        //else {
        //    firstcolumnstyle = 'tablefirstcolumn2'
        //    valueStyle = 'tablevalue2';
        //}

        //tableBody.push(getTableTotal(data, firstcolumnstyle, valueStyle)); //add the tableTotal to tableBody
        return tableBody;
    }

    var getPDFTable = function (data) {
        var table = {
            headerRows: 1,
            widths: [140, 60, 40, 60, 60, 60, 60, 60, 60, '*'],
            body: getTableBody(data),
        }
        return table;
    }

    var getParameterString = function () {
        var start = gbService.getDateRangeStartTime(); //string
        var end = gbService.getDateRangeEndTime(); //sring
        return gbService.getResourceMsg('start') + ' ' + start + '   ' + gbService.getResourceMsg('end') + ' ' + end;
    }

    var getExtraParameterString = function () {
        var numPlayer = checkNumPlayer();
        var numVisit = checkNumVisit();
        return gbService.getResourceMsg('paramplayernumber') + ' : ' + numPlayer.toString() + '  ' + gbService.getResourceMsg('paramvisitnumber') + ' : ' + numVisit.toString();
    }

    var exportPDF = function () {
        var start = gbService.getDateRangeStartTime(); //string
        var end = gbService.getDateRangeEndTime(); //sring
        if (!isParameterValid(start, end)) return;

        $('input[name="queryParam[start]"]').val(start);
        $('input[name="queryParam[end]"]').val(end);

        var numPlayer = checkNumPlayer();
        var numVisit = checkNumVisit();

        //show query waiting info
        gbService.showQueryWaitingInfo();
        //Create all area charts and tables through ajax
        getTopPlayerList(start, end, numPlayer, numVisit, loadPDFData);
    }

    /*callback method called by getTopPlayerList ajax call */
    var loadPDFData = function (data) {
        //define pdf content
        var contents = [
            { text: 'Rpt #: ' + gbService.getResourceMsg('reportname'), style: 'reportname' },
            { text: gbService.getResourceMsg('title'), style: 'header' },
            { text: ' ' },
            { text: getParameterString(), style: 'parameter' },
            { text: getExtraParameterString(), style: 'parameter'},
            { text: ' ' },
            { table: getPDFTable(data), layout: gbService.getPDFTableLayout() },
        ]

        var docDefinition = {
            // a string or { width: number, height: number }
            pageSize: 'A4',
            // by default we use portrait, you can change it to landscape here
            pageOrientation: 'landscape', //can change to landscape
            // [left, top, right, bottom] or [horizontal, vertical] or just a number for equal margins
            pageMargins: gbService.getPDFMargins(),
            styles: gbService.getPDFStyles(),
            content: contents,
        };
        loadAllData(data); //refresh screen
        //clean waiting info
        gbService.clearQueryWaitingInfo();
        // open the PDF in a new window
        pdfMake.createPdf(docDefinition).open();
    }

    var pageInit = function () {
        //Load Parameters from Local Storage if exist
        loadParamFromLocal();

        //Init the Money Format
        gbService.initMoneyFormat();

        //Init the DateRange Control
        gbService.initDateRange();

        $('#buttonExecute').click(loadPageContent);
        // initiate a new Toggles class
        $('.toggle').toggles({ width: 50 });
        $('.toggle').on('toggle', function () { gbService.setAutoRefreshOnOff(loadPageContent); });

        //initiate pdf button
        if (gbService.isAcrobatInstalled) {
            $('#buttonPDF').show();
            $('#buttonPDF').click(exportPDF);
        }
        else {
            $('#buttonPDF').hide();
        }
        loadPageContent();
    }
    return {
        pageInit: pageInit,
    }
}();


$(document).ready(function () {
    topPlayer.pageInit();
});