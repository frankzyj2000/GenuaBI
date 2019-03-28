"use strict";
 
var slotOccupation = function () {
    var varTotalSlots = 0;
    var paramName = 'paramSlotOccupation';

    var areaChartPlayers = null;
    var areaChartSlots = null;
    var areaChartVisitors = null;
    var areaChartRates = null;
    var areaChartHandles = null;
    var areaChartWins = null;
    var donutChartTotalSlots = null;

    var redrawCharts = function () {
        areaChartPlayers.resize(areaChartPlayers.render, true);
        areaChartSlots.resize(areaChartSlots.render, true);
        areaChartVisitors.resize(areaChartVisitors.render, true);
        areaChartRates.resize(areaChartRates.render, true);
        areaChartHandles.resize(areaChartHandles.render, true);
        areaChartSlots.resize(areaChartSlots.render, true);
        areaChartWins.resize(areaChartWins.render, true);
        //donutChartTotalSlots.resize(donutChartTotalSlots.render, true);
    }

    var saveParamToLocal = function (data) {
        var start = gbService.getDateRangeStartTime(); //string
        //Define Parameters to be saved in local storage
        var paramData = { "Start": start };
        gbService.setLocalObject(paramName, paramData);
    }

    var loadParamFromLocal = function () {
        //get start,end parameters from local storage for only starting the page
        var param = gbService.setDateFromLocalStorage(paramName);
    }

    var dragInit = function () {
        var dragObjects = "#boxTotalHandle, #boxTotalWin, #boxTotalVisitors, #boxTotalSlotOccupied, #boxTotalSlots,";
        dragObjects += "#boxSlotsOccupationRate, #boxSlotOccupationChart, #VisitorsPerHourChart, #SlotsPerHourChart,";
        dragObjects += "#SessionsPerHourChart, #OccupationRatePerHourChart, #HandlesPerHourChart, #WinLossPerHourChart, #boxOccupationList";
        $(dragObjects).draggable({ containment: '#gbiContent', cursor: 'move', stack: dragObjects, revert: true, stop: gbService.handleDragStop });
        $(dragObjects).droppable({ accept: dragObjects, hoverClass: 'hovered', drop: handleDrop });
    }

    var handleDrop = function (event, ui) {
        var dropId = $(this).attr("id");
        var draggable = ui.draggable;
        var dragId = draggable.attr("id");
        var groupObject = ['boxTotalHandle', 'boxTotalWin', 'boxTotalVisitors', 'boxTotalSlotOccupied', 'boxTotalSlots', 'boxSlotsOccupationRate'];
        if (dropId != null) {            
            if (groupObject.indexOf(dropId) >= 0 && groupObject.indexOf(dragId) < 0) {
                gbService.exchangeElement(draggable[0], $('#boxIndicator')[0]);
            }
            else if (groupObject.indexOf(dropId) < 0 && groupObject.indexOf(dragId) >= 0) {
                gbService.exchangeElement($('#boxIndicator')[0], $(this)[0]);
            }
            else {
                gbService.exchangeElement(draggable[0], $(this)[0]);
            }
        }
    }

    var getOccupationList = function (start, isAnimate, callback) {
        $.ajax({
            type: "GET",
            url: "/Dashboard/GetSlotOccupation",
            dataType: "json",
            data: {
                start: start
            },
            success: function (data) {
                data = gbService.getParamFromGbiForm(data); // get parameters from local storage
                if (callback) callback(data, isAnimate);
            }
        });
    }

    var getTimeAxis = function (data) {
        var result = [];
        if (data.SlotOccupationList) {
            for (var i = 0; i < data.SlotOccupationList.length; i++) {
                if (data.SlotOccupationList[i]) {
                    result.push(data.SlotOccupationList[i].Hora.toString());
                }
            }
        }
        return result;
    }

    var getSlotOccupationRate = function (data) {
        return (data.TotalSlots == 0 ? 0 : (data.TotalSlotsOccupied * 100 / data.TotalSlots)).toFixed(2) + " %";
    }

    var changeNumbericText = function (data) {
        $("#TotalHandle").text(gbService.toMoneyStr(data.TotalHandle));
        $("#TotalWin").text(gbService.toMoneyStr(data.TotalWin));
        if (data.TotalWin < 0) {
            $("#TotalWin").addClass("info-box-number-red");
        }
        else {
            $("#TotalWin").removeClass("info-box-number-red");
        }
        $("#DataSlotOccupied").text(data.TotalSlotsOccupied);
        $("#DataSlotEmpty").text(data.TotalSlots - data.TotalSlotsOccupied);

        $("#TotalVisitors").text(data.TotalPlayers);
        $("#TotalSlotOccupied").text(data.TotalSlotsOccupied);
        $("#TotalSlots").text(data.TotalSlots);
        $("#SlotsOccupationRate").text(getSlotOccupationRate(data));
        $("#occupationlist_title").text(gbService.getResourceMsg("data-start") + ' ' + data.StartTime + ' ' + gbService.getResourceMsg("data-end") + ' ' + data.EndTime);
    }

    var getWinData = function (data) {
        var result = [];
        if (data.SlotOccupationList) {
            for (var i = 0; i < data.SlotOccupationList.length; i++) {
                if (data.SlotOccupationList[i]) {
                    result.push(parseFloat(data.SlotOccupationList[i].WinLoss));
                }
            }
        }
        return result;
    }

    var getTotalSlotsOccupied = function (data) {
        return data.TotalSlotsOccupied;
    }

    var getEmptySlots = function (data) {
        return data.TotalSlots - data.TotalSlotsOccupied;
    }

    var getPlayersData = function (data) {
        var result = [];
        if (data.SlotOccupationList) {
            for (var i = 0; i < data.SlotOccupationList.length; i++) {
                if (data.SlotOccupationList[i]) {
                    result.push(parseFloat(data.SlotOccupationList[i].Players));
                }
            }
        }
        return result;
    }

    var getSlotsData = function (data) {
        var result = [];
        if (data.SlotOccupationList) {
            for (var i = 0; i < data.SlotOccupationList.length; i++) {
                if (data.SlotOccupationList[i]) {
                    result.push(data.SlotOccupationList[i].Slots);
                }
            }
        }
        return result;
    }

    var getTotalSessionData = function (data) {
        var result = [];
        if (data.SlotOccupationList) {
            for (var i = 0; i < data.SlotOccupationList.length; i++) {
                if (data.SlotOccupationList[i]) {
                    result.push(data.SlotOccupationList[i].TotalSession);
                }
            }
        }
        return result;
    }

    var getHandleData = function (data) {
        var result = [];
        if (data.SlotOccupationList) {
            for (var i = 0; i < data.SlotOccupationList.length; i++) {
                if (data.SlotOccupationList[i]) {
                    result.push(parseFloat(data.SlotOccupationList[i].Handle));
                }
            }
        }
        return result;
    }

    var getRateData = function (data) {
        var result = [];
        if (data.SlotOccupationList) {
            for (var i = 0; i < data.SlotOccupationList.length; i++) {
                if (data.SlotOccupationList[i]) {
                    result.push(parseFloat(data.SlotOccupationList[i].Slots * 100 / data.TotalSlots).toFixed(2));
                }
            }
        }
        return result;
    }

    var getTimeStr = function (value) {
        var start, end;
        if (value <= 9) {
            start = "0" + value;
        }
        else {
            start = value;
        }

        if (value <= 8) {
            end = "0" + (value + 1);
        }
        else {
            end = (value + 1)
        }

        return gbService.getResourceMsg("from") + ' ' + start + ' ' + gbService.getResourceMsg("to") + ' ' + end + " Hs";
    }

    var getOccupationRate = function (data) {
        if (typeof data !== 'undefined' && data != null) { // the variable is defined
            if (varTotalSlots == 0) return 0;
            else
                return parseFloat((data * 100) / varTotalSlots).toFixed(2);
        }
        else
            return "";
    }

    //creating dataTable
    var dataTableInit = function (datalist) {
        var table;
        var lengthMenu = [[10, 25, 50, -1], [10, 25, 50, gbService.getResourceMsg('all')]];
        var ajaxCall = {
            url: "/Dashboard/SlotOccupationDataTableHandler",
            type: "POST",
            data: function (data) {
                gbService.showQueryWaitingInfo(); //show query waiting info
                saveParamToLocal(data); //save parameters to local storage
                return data = gbService.getParamFromGbiForm(data);
            }
        };

        //define dom for the structure of the datatable
        var dom = "<'row'<'col-sm-3'l><'col-sm-5 text-center'B><'col-sm-4'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-5'i><'col-sm-7'p>>"; 

        var buttons = [
                    { extend: 'colvis', text: gbService.getResourceMsg('btnchoosecolumn'), columns: ':not(:first-child):not(:last-child)' },
                    { extend: 'excel',  text: gbService.getResourceMsg('btnexcel') },
        ];

        var columnDefs = [
                    {
                        targets: 0, /*column index counting from the left*/
                        render: function (data, type, row) { return getTimeStr(data); },
                        "orderData": [7]
                    },
                    { targets: 1, className: "dt-right", },
                    { targets: 2, className: "dt-right", },
                    { targets: 3, className: "dt-right", },
                    { targets: 4, className: "dt-right", },
                    { targets: 5, className: "dt-right", render: function (data, type, row) { return gbService.toMoneyStr(data); } },
                    { targets: 6, className: "dt-right", render: function (data, type, row) { return gbService.toMoneyStr(data); } },
                    { targets: 7, className: "dt-right", type: "string", visible: false } //this column should never display
        ];

        var columns = [
                    { data: "Hora" },
                    { data: "Players" },
                    { data: "Slots" },
                    { data: "TotalSession" },
                    { data: "Slots", render: function (data, type, full) { return getOccupationRate(data); } },
                    { data: "Handle" },
                    { data: "WinLoss" },
                    { data: "HoraInicio", visible: false }
        ];

        if (gbService.getResourceMsg('servercache').toLowerCase() == 'true') //specified using server side processing for huge table
        {
            table = $('#occupationlist').DataTable({
                language: { "url": gbService.getResourceMsg('datatablelanguage') },
                destroy: true,
                processing: true,
                serverSide: true,
                ordering: true,
                //data: datalist,
                info: true,
                paging: false,
                searching: false,
                //"scrollX": true, //removed for version 1.10.10
                responsive: true,
                ajax: ajaxCall,
                drawCallback: function () {
                    gbService.clearQueryWaitingInfo(); //clean up query waiting info
                },
                dom: dom,
                buttons: buttons,
                columnDefs: columnDefs,
                columns: columns,
            });
        }
        else //DataTable init with local cache
        {
            table = $('#occupationlist').DataTable({
                language: { "url": gbService.getResourceMsg('datatablelanguage') },
                destroy: true,
                processing: true,
                serverSide: false,
                ordering: true,
                data: datalist,
                info: true,
                paging: false,
                searching: false,
                //scrollX: true, //removed for version 1.10.10
                responsive: true,
                //ajax: ajaxCall,
                stateSave: true,
                drawCallback: function () {
                    gbService.clearQueryWaitingInfo(); //clean up query waiting info
                },
                dom: dom,
                buttons: buttons,
                columnDefs: columnDefs,
                columns: columns,
            });
        }
        return table;
    }

    var drawPlayerChart = function (data, option) {
        // Get context with jQuery - using jQuery's .get() method.
        var areaChartCanvas = $("#chartPlayers").get(0).getContext("2d");
        var areaChartData = {
            labels: getTimeAxis(data),
            datasets: [
                {
                    label: "Visitas",
                    fillColor: gbService.getBgColor('lightblue', 0.9),
                    strokeColor: gbService.getBgColor('blue', 0.8),
                    pointColor: gbService.getColor('blue'),
                    pointStrokeColor: gbService.getBgColor('blue', 1),
                    pointHighlightFill: gbService.getColor('white'),
                    pointHighlightStroke: gbService.getBgColor('blue', 1),
                    data: getPlayersData(data)
                }
            ]
        };
        if (areaChartPlayers != null) {
            areaChartPlayers.destroy(); //remove old charts
        }
        //Draw the area chart
        areaChartPlayers = new Chart(areaChartCanvas).Line(areaChartData, option);
    }

    var drawSlotChart = function (data, option) {
        // Get context with jQuery - using jQuery's .get() method.
        var areaChartCanvas = $("#chartSlots").get(0).getContext("2d");
        var areaChartData = {
            labels: getTimeAxis(data),
            datasets: [
                {
                    label: "Visitas",
                    fillColor: gbService.getBgColor('lightblue', 0.9),
                    strokeColor: gbService.getBgColor('blue', 0.8),
                    pointColor: gbService.getColor('blue'),
                    pointStrokeColor: gbService.getBgColor('blue', 1),
                    pointHighlightFill: gbService.getColor('white'),
                    pointHighlightStroke: gbService.getBgColor('blue', 1),
                    data: getSlotsData(data)
                }
            ]
        };
        if (areaChartSlots != null) {
            areaChartSlots.destroy(); //remove old charts
        }
        //Draw the area chart
        areaChartSlots = new Chart(areaChartCanvas).Line(areaChartData, option);
    }

    var drawSessionChart = function (data, option) {
        // Get context with jQuery - using jQuery's .get() method.
        var areaChartCanvas = $("#chartSessions").get(0).getContext("2d");
        var areaChartData = {
            labels: getTimeAxis(data),
            datasets: [
                {
                    label: "Visitas",
                    fillColor: gbService.getBgColor('lightblue', 0.9),
                    strokeColor: gbService.getBgColor('blue', 0.8),
                    pointColor: gbService.getColor('blue'),
                    pointStrokeColor: gbService.getBgColor('blue', 1),
                    pointHighlightFill: gbService.getColor('white'),
                    pointHighlightStroke: gbService.getBgColor('blue', 1),
                    data: getTotalSessionData(data)
                }
            ]
        };
        if (areaChartVisitors != null) {
            areaChartVisitors.destroy(); //remove old charts
        }
        //Draw the area chart
        areaChartVisitors = new Chart(areaChartCanvas).Line(areaChartData, option);
    }

    var drawRateChart = function (data, option) {
        // Get context with jQuery - using jQuery's .get() method.
        var areaChartCanvas = $("#chartRates").get(0).getContext("2d");
        var areaChartData = {
            labels: getTimeAxis(data),
            datasets: [
                {
                    label: "Visitas",
                    fillColor: gbService.getBgColor('lightblue', 0.9),
                    strokeColor: gbService.getBgColor('blue', 0.8),
                    pointColor: gbService.getColor('blue'),
                    pointStrokeColor: gbService.getBgColor('blue', 1),
                    pointHighlightFill: gbService.getColor('white'),
                    pointHighlightStroke: gbService.getBgColor('blue', 1),
                    data: getRateData(data)
                }
            ]
        };
        if (areaChartRates != null) {
            areaChartRates.destroy(); //remove old charts
        }
        //Draw the area chart
        areaChartRates = new Chart(areaChartCanvas).Line(areaChartData, option);
    }

    var drawHandleChart = function (data, option) {
        // Get context with jQuery - using jQuery's .get() method.
        var areaChartCanvas = $("#chartHandles").get(0).getContext("2d");
        var areaChartData = {
            labels: getTimeAxis(data),
            datasets: [
                {
                    label: "Visitas",
                    fillColor: gbService.getBgColor('lightblue', 0.9),
                    strokeColor: gbService.getBgColor('blue', 0.8),
                    pointColor: gbService.getColor('blue'),
                    pointStrokeColor: gbService.getBgColor('blue', 1),
                    pointHighlightFill: gbService.getColor('white'),
                    pointHighlightStroke: gbService.getBgColor('blue', 1),
                    data: getHandleData(data)
                }
            ]
        };
        if (areaChartHandles != null) {
            areaChartHandles.destroy(); //remove old charts
        }
        //Draw the area chart
        areaChartHandles = new Chart(areaChartCanvas).Line(areaChartData, option);
    }

    var drawWinChart = function (data, option) {
        // Get context with jQuery - using jQuery's .get() method.
        var areaChartCanvas = $("#chartWins").get(0).getContext("2d");
        var areaChartData = {
            labels: getTimeAxis(data),
            datasets: [
                {
                    label: "Visitas",
                    fillColor: gbService.getBgColor('lightblue', 0.9),
                    strokeColor: gbService.getBgColor('blue', 0.8),
                    pointColor: gbService.getColor('blue'),
                    pointStrokeColor: gbService.getBgColor('blue', 1),
                    pointHighlightFill: gbService.getColor('white'),
                    pointHighlightStroke: gbService.getBgColor('blue', 1),
                    data: getWinData(data)
                }
            ]
        };
        if (areaChartWins != null) {
            areaChartWins.destroy(); //remove old charts
        }
        //Draw the area chart
        areaChartWins = new Chart(areaChartCanvas).Line(areaChartData, option);
    }

    var drawTotalSlotDonutChart = function (data, option) {
        var donutChartCanvas = $("#slotTotalDonutChart").get(0).getContext("2d");
        var donutData = [
            {
                value: getTotalSlotsOccupied(data),
                color: gbService.getColor('blue'),
                highlight: gbService.getColor('blue'),
                label: gbService.getResourceMsg("data-slot-occupied")
            },
            {
                value: getEmptySlots(data),
                color: gbService.getColor('grey'),
                highlight: gbService.getColor('grey'),
                label: gbService.getResourceMsg("data-slot-empty")
            }
        ];
        if (donutChartTotalSlots != null) {
            donutChartTotalSlots.destroy(); //remove old charts
        }
        //Create douhnut chart, You can switch between pie and douhnut using the method below.
        donutChartTotalSlots = new Chart(donutChartCanvas).Doughnut(donutData, option);
    }

    var loadAllData = function (data, isAnimate) {
        //Save parameters to local storage
        saveParamToLocal(data);

        // set total slots for later reference
        varTotalSlots = data.TotalSlots;
        // fill all numberic fields
        changeNumbericText(data);

        if (isAnimate) {
            drawPlayerChart(data, gbService.getAreaChartNumberOptions());
            drawSlotChart(data, gbService.getAreaChartNumberOptions());
            drawSessionChart(data, gbService.getAreaChartNumberOptions());

            drawRateChart(data, gbService.getAreaChartPercentageOptions());
            drawHandleChart(data, gbService.getAreaChartOptions());
            drawWinChart(data, gbService.getAreaChartOptions());

            drawTotalSlotDonutChart(data, gbService.getDonutOptions());
        }
        else {
            drawPlayerChart(data, gbService.getAreaChartNumberNoAnimationOptions());
            drawSlotChart(data, gbService.getAreaChartNumberNoAnimationOptions());
            drawSessionChart(data, gbService.getAreaChartNumberNoAnimationOptions());

            drawRateChart(data, gbService.getAreaChartPercentageNoAnimationOptions());
            drawHandleChart(data, gbService.getAreaChartNoAnimationOptions());
            drawWinChart(data, gbService.getAreaChartNoAnimationOptions());

            drawTotalSlotDonutChart(data, gbService.getDonutNoAnimationOptions());
        }
        //Loading DataTable
        dataTableInit(data.SlotOccupationList);
    }

    var getPDFTableColumns = function () {
        var tableColumns = [];
        tableColumns.push({ text: gbService.getResourceMsg('columntime'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnplayers'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnslots'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnsessions'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnrate'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnhandle'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnwin'), style: 'tableheader' });
        return tableColumns;
    }

    //var getPDFTableTotal = function (data, fistcolumnstyle, style) {
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

    var getPDFTableBody = function (data) {
        var tableBody = [];
        tableBody.push(getPDFTableColumns());

        var list = data.SlotOccupationList;
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

            var slotRate = parseFloat((list[i]['Slots'] * 100) / data.TotalSlots).toFixed(2);
            tableRow.push({ text: getTimeStr(list[i]['Hora']), style: firstcolumnstyle });
            tableRow.push({ text: list[i]['Players'].toString(), style: valueStyle });
            tableRow.push({ text: list[i]['Slots'].toString(), style: valueStyle });
            tableRow.push({ text: list[i]['TotalSession'].toString(), style: valueStyle });
            tableRow.push({ text: slotRate.toString() + '%', style: valueStyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['Handle']), style: valueStyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['WinLoss']), style: valueStyle });
            tableBody.push(tableRow); //add the tableRow to tableBody
        }
        if (list.length % 2 == 0) {
            firstcolumnstyle = 'tablefirstcolumn1'
            valueStyle = 'tablevalue1';
        }
        else {
            firstcolumnstyle = 'tablefirstcolumn2'
            valueStyle = 'tablevalue2';
        }

        //tableBody.push(getPDFTableTotal(data, firstcolumnstyle, valueStyle)); //add the tableTotal to tableBody
        return tableBody;
    }

    var getPDFTable = function (data) {
        var table = {
            headerRows: 1,
            widths: [90, 60, 60, 80, 60, 70, '*'],
            body: getPDFTableBody(data),
        }
        return table;
    }

    var getParameterString = function () {
        var start = gbService.getDateRangeStartTime() + ' ' + gbService.getResourceMsg('casinostarttime');
        var end = gbService.getCasinoEndTime(moment(gbService.getDateRangeStartTime(), gbService.getSysDateFormat())); //moment datetime
        return gbService.getResourceMsg('start') + ' ' + start + '   ' + gbService.getResourceMsg('end') + ' ' + end.format(gbService.getSysDateTimeFormat());
    }

    var exportPDF = function () {
        //Create all area charts through ajax
        var inputdate = $('input[name="daterange"]').val().trim();
        if (!gbService.checkSingleDateParameter(inputdate))
            return; //do nothing
        $('input[name="queryParam[start]"]').val(inputdate);

        //show query waiting info
        gbService.showQueryWaitingInfo();
        //Create all area charts and tables through ajax
        getOccupationList(inputdate, false, loadPDFData); //no animation
    }

    var loadPDFData = function (data, isAnimate) {
        //create image for every chart used by pdf export
        drawPlayerChart(data, gbService.getAreaChartNumberNoAnimationOptions());
        drawSlotChart(data, gbService.getAreaChartNumberNoAnimationOptions());
        drawSessionChart(data, gbService.getAreaChartNumberNoAnimationOptions());
        drawRateChart(data, gbService.getAreaChartPercentageNoAnimationOptions());
        drawHandleChart(data, gbService.getAreaChartNoAnimationOptions());
        drawWinChart(data, gbService.getAreaChartNoAnimationOptions());
        drawTotalSlotDonutChart(data, gbService.getDonutNoAnimationOptions());

        var imgPlayers = $("#chartPlayers")[0].toDataURL();
        var imgSlots = $("#chartSlots")[0].toDataURL();
        var imgSessions = $("#chartSessions")[0].toDataURL();
        var imgRates = $("#chartRates")[0].toDataURL();
        var imgHandles = $("#chartHandles")[0].toDataURL();
        var imgWins = $("#chartWins")[0].toDataURL();
        var imgTotolSlot = $("#slotTotalDonutChart")[0].toDataURL();

        var emptySlots = data.TotalSlots - data.TotalSlotsOccupied;
        var imgWhiteCircle = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAeAB4AAD/4QAiRXhpZgAATU0AKgAAAAgAAQESAAMAAAABAAEAAAAAAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAATABQDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9yvFvi3UrvxMuh6GsK3YQST3Egytup6cc+3Y1R1S58UeBIP7QuL6HWLGLm4j8gRvEO5B74/yo602/v1+H3xSub29Vl0/VoVVZgpIicY4P5VZ+IPxB0+XQZrGxuIdQvtQQwwxwuJPvcZJHA/nWnVJIk63TNWj1bTobqBt0NwgkU+xoqn4K0h9A8KWNnId0kEQDE+p5P86KnQo0LvT4NTtnhuIY54WPKSKGU/gap6L4W03SJzNa2Fnby/d3xxBWA+tFFLoBpr3+tFFFID//2Q==";
        var imgLightBlueCircle = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAeAB4AAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAATABQDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9ef2hfj/431740wfCj4UQ6ZF4m+xDUdZ1zUUMltoVuxwmEwQ0rZBAII5HBySvJePdV+P37IekHxjrPi/S/i34SsMPrdgdGh0y9s4M/NNAYuG2jk7jgD+HGSG+L/Glt+x3+3nr3irxaJLXwV8UtOtbWHWtjPBp15bqF8mUjO0MoJyR3B6BiNr9r39tLwPd/BzVfC/hDW9K8deLfGVrJpGl6Xolyl+zvMpjLOYywQKGJwxBOOB1I+so0Zp0aVGip05pXbje9/i97ePLqtGrWuz4uvXhKNetXryhVg5cqUmrJfB7m0uZWeqd72R9C+DfFtj4+8JabremTfaNO1a1ju7aTGN8bqGU47HBornP2bvhtcfB/wCAfhDwxeOJLzRNKgtbhlbcvmKg34PoGyB7CivmK8YRqSjTd4pu3ofXYeU5UoyqK0mlddnbU6jxH4a07xhos+m6tp9lqmnXS7ZrW7gWeGYdcMjAqR9RWB4E+Afgf4Xam974b8HeGNBvJE8trjT9Mht5WX+7uRQce2aKKUatSMXCLdn0HKjTlNVJRTa2dtTraKKKzNT/2Q==";
        //define pdf style dictionar
        var contents = [
            { text: 'Rpt #: ' + gbService.getResourceMsg('reportname'), style: 'reportname' },
            { text: gbService.getResourceMsg('title'), style: 'header' },
            { text:' ' },
            { text: getParameterString(), style: 'parameter' },
            { table: getPDFTable(data), layout: gbService.getPDFTableLayout() },
            { table:
                {
                    widths: [270, '*'],
                    body:
                    [
                        [
                            { text: gbService.getResourceMsg('titlegrandtotalslots'), style: 'charttitle' },
                            { text: ' ' }
                        ],
                        [
                            {
                                table: {
                                    widths: [150, '*'],
                                    body: [
                                        [
                                            { image: imgTotolSlot, width: 150 },
                                            {
                                                table: {
                                                    widths: [10, '*'],
                                                    body: [
                                                        [{ image: imgWhiteCircle, width: 10 }, { text: emptySlots.toString() + ' ' + gbService.getResourceMsg('slot-empty'), style: 'tablefirstcolumn1' }],
                                                        [{ image: imgLightBlueCircle, width: 10 }, { text: data.TotalSlotsOccupied.toString() + ' ' + gbService.getResourceMsg('slot-occupied'), style: 'tablefirstcolumn1' }],
                                                    ]
                                                },
                                                layout: 'noBorders'
                                            }
                                        ],
                                    ]
        
                                },
                                layout: 'noBorders'
                            },
                            {
                                table: {
                                    widths: [120, '*'],
                                    body: [
                                        [{ text: gbService.getResourceMsg('numbertotalhandle'),  style: 'subtitle' }, { text: gbService.toMoneyStr(data.TotalHandle), style: 'value' }],
                                        [{ text: gbService.getResourceMsg('numbertotalwin'),     style: 'subtitle' }, { text: gbService.toMoneyStr(data.TotalWin),    style: 'value' }],
                                        [{ text: gbService.getResourceMsg('numbertotalvisitor'), style: 'subtitle' }, { text: data.TotalPlayers.toString(),           style: 'value' }],
                                        [{ text: gbService.getResourceMsg('numbertotalslots'),   style: 'subtitle' }, { text: data.TotalSlots.toString(),             style: 'value' }],
                                        [{ text: gbService.getResourceMsg('numbertotalslotsoccupied'), style: 'subtitle' }, { text: data.TotalSlotsOccupied.toString(), style: 'value' }],
                                        [{ text: gbService.getResourceMsg('numberrateslots'),    style: 'subtitle' }, { text: getSlotOccupationRate(data),            style: 'value' }],
                                    ],
                                },
                                layout: 'noBorders'
                            }
                        ],
                        //[{ text: ' ' }, { text: ' ' }],
                        [{ text: gbService.getResourceMsg('titlevisitorsperhour'), style: 'charttitle' }, { text: gbService.getResourceMsg('titleslotsperhour'), style: 'charttitle' }],
                        [{ image: imgPlayers, width: 260, height: 100 }, { image: imgSlots, width: 260, height: 100 }],
                        [{ text: gbService.getResourceMsg('titlesessionsperhour'), style: 'charttitle' }, { text: gbService.getResourceMsg('titleratesperhour'), style: 'charttitle' }],
                        [{ image: imgSessions, width: 260, height: 100 }, { image: imgRates, width: 260, height: 100 }],
                        [{ text: gbService.getResourceMsg('titlehandlesperhour'), style: 'charttitle' }, { text: gbService.getResourceMsg('titlewinlossperhour'), style: 'charttitle' }],
                        [{ image: imgHandles, width: 260, height: 100 }, { image: imgWins, width: 260, height: 100 }],
                    ]
                },
                layout: 'noBorders'
            },
        ]

        var docDefinition = {
            // a string or { width: number, height: number }
            pageSize: 'A4',
            // by default we use portrait, you can change it to landscape here
            pageOrientation: 'portrait', //can change to landscape
            // [left, top, right, bottom] or [horizontal, vertical] or just a number for equal margins
            pageMargins: gbService.getPDFMargins(),
            styles: gbService.getPDFStyles(),
            content: contents,
        };
        loadAllData(data, isAnimate); //refresh screen
        //clean waiting info
        gbService.clearQueryWaitingInfo();
        // open the PDF in a new window
        pdfMake.createPdf(docDefinition).open();
    }

    var loadPageContent = function () {
        SessionManager.start();
        // Whenever mouse move or key in, extend the session,
        // since we know the user is interacting with the site.
        document.onmousemove = SessionManager.extend;
        document.onkeypress = SessionManager.extend;

        //Create all area charts through ajax
        var inputdate = $('input[name="daterange"]').val().trim();
        if (!gbService.checkSingleDateParameter(inputdate))
            return; //do nothing
        $('input[name="queryParam[start]"]').val(inputdate);

        //show query waiting info
        gbService.showQueryWaitingInfo();
        getOccupationList(inputdate, true, loadAllData); //have animation
    }

    var pageInit = function () {
        //Load Parameters from Local Storage if exist
        loadParamFromLocal();

        //Init the Money Format
        gbService.initMoneyFormat();

        //Init the DateRange Control
        gbService.initSingleDate();

        dragInit();
        $('#buttonExecute').click(loadPageContent);

        $('.toggle').toggles({ width: 50 });
        $('.toggle').on('toggle', function () { gbService.setAutoRefreshOnOff(loadPageContent); });

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
        redrawCharts: redrawCharts,
    }
}();


$(document).ready(function () {
    slotOccupation.pageInit();
});