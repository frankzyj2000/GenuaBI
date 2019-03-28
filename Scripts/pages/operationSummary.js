"use strict";
 
var operationSummary = function () {
    var paramName = 'paramOperationSummary';
    var donutChartOccupation = null;
    var donutChartPromotion = null;
    var barChart1Week = null;
    var barChart7Week = null;

    var saveParamToLocal = function (data) {
        var start = gbService.getDateRangeStartTime(); //string
        var end = gbService.getDateRangeEndTime(); //sring
        var pageLen = gbService.getResourceMsg('pagelength');
        if (typeof data != "undefined" && data != null && 'length' in data) pageLen = data["length"];
        //Define Parameters to be saved in local storage
        if (isParameterValid(start, end)) {
            var paramData = { "Start": start, "End": end, "PageLen": pageLen };
            gbService.setResourceMsg('pagelength', pageLen); //need to save the page length here
            gbService.setLocalObject(paramName, paramData);
        }
    }

    var loadParamFromLocal = function () {
        //get start,end parameters from local storage for only starting the page
        var param = gbService.setDateRangeFromLocalStorage(paramName);
        if (param != null) {
            gbService.setResourceMsg('pagelength', param['PageLen']);
        }
    }

    var isParameterValid = function (start, end) {
        if (start == null || end == null) return false; //date format is wrong       
        if (!gbService.checkDateRangeValue(start, end)) return false; //date range is out of limit
        return true;
    }

    var dragInit = function () {
        var dragObjects = '#boxWinSlots, #boxPlayerAccountAmount, #boxNetWinAfterTax, #boxConsumeredPromos, #boxSlotOccupationRate, #OperationSummaryTrend1WeekChart, #OperationSummaryTrend7WeekChart, #boxProviderList';
        $(dragObjects).draggable({ containment: '#gbiContent', cursor: 'move', stack: dragObjects, revert: true, stop: gbService.handleDragStop });
        $(dragObjects).droppable({ accept: dragObjects, hoverClass: 'hovered', drop: handleDrop });
    }

    var handleDrop = function (event, ui) {
        var draggable = ui.draggable;
        $(this).css({ left: '', top: '' });
        gbService.exchangeElement(draggable[0], $(this)[0]);
        draggable.css({ left: '', top: '' });
    }

    var getOperationSummaryList = function (start, end, isAnimate, callback) {
        $.ajax({
            type: "GET",
            url: "/Dashboard/GetOperationSummary",
            dataType: "json",
            data: {
                start: start,
                end: end
            },
            success: function (data) {
                data = gbService.getParamFromGbiForm(data); // get parameters from local storage
                if (callback) callback(data, isAnimate);
            }
        });
    }

    var changeTotal = function (data) {
        $("#totalMoneyIn"     ).text(gbService.toMoneyStr(data.TotalMoneyIn));
        $("#totalMoneyOut"    ).text(gbService.toMoneyStr(data.TotalMoneyOut));
        $("#totalHandPayments").text(gbService.toMoneyStr(data.TotalHandPayments));
        $("#totalDPromotion"  ).text(gbService.toMoneyStr(data.TotalDPromotion));
        $("#totalNetWin"      ).text(gbService.toMoneyStr(data.TotalNetWin));
        $("#totalWin"         ).text(gbService.toMoneyStr(data.TotalWin));
        $("#totalCantPlayer"  ).text(data.TotalCantPlayer);
    }

    var getNetWin = function(data) {
        return data.NetWinCashIn - data.NetWinCashOut;
    }

    var getNetWinAfterTax = function (data) {
        return data.NetWinCashIn - data.NetWinCashOut - data.Taxes;
    }

    var getConsumedPromoRateStr = function (data) {
        var value = data.ConsumedPromos * 100.0 / (data.CancelledPromos + data.ConsumedPromos)
        return value.toFixed(2) + "%";
    }

    var getCancelledPromoRateStr = function (data) {
        var value = data.CancelledPromos * 100.0 / (data.CancelledPromos + data.ConsumedPromos);
        return value.toFixed(2) + "%";
    }

    var getSlotOccupationRateStr = function (data) {
        var value = data.TotalSlotOccupied * 100.0 / data.TotalSlots;
        return value.toFixed(2) + "%";
    }

    var getEmptySlotRateStr = function (data) {
        var value = (data.TotalSlots - data.TotalSlotOccupied) * 100.0 / data.TotalSlots;
        return value.toFixed(2) + "%";
    }

    var changeNumbericText = function (data) {
        $("#NetWin").text(gbService.toMoneyStr(getNetWin(data)));
        if (data.NetWinCashIn < data.NetWinCashOut) {
            $("#NetWin").removeClass("bg-blue");
            $("#NetWin").addClass("bg-red");
        }
        else {
            $("#NetWin").removeClass("bg-red");
        }
        $("#NetWinCashIn" ).text(gbService.toMoneyStr(data.NetWinCashIn));
        $("#NetWinCashOut").text(gbService.toMoneyStr(data.NetWinCashOut));

        $("#Taxes").text(gbService.toMoneyStr(data.Taxes));

        var value = getNetWinAfterTax(data);
        $("#NetWinAfterTax").text(gbService.toMoneyStr(value));
        if (value < 0) {
            $("#NetWinAfterTax").removeClass("bg-blue");
            $("#NetWinAfterTax").addClass("bg-red");
        }
        else {
            $("#NetWinAfterTax").removeClass("bg-red");
        }

        $("#PlayerAccountAmount").text(gbService.toMoneyStr(data.PlayerAccountAmount));
        if (data.PlayerAccountAmount < 0) {
            $("#PlayerAccountAmount").removeClass("bg-blue");
            $("#PlayerAccountAmount").addClass("bg-red");
        }
        else {
            $("#PlayerAccountAmount").removeClass("bg-red");
        }
        $("#PlayerCashIn" ).text(gbService.toMoneyStr(data.PlayerAccountIn));
        $("#PlayerCashOut").text(gbService.toMoneyStr(data.PlayerAccountOut));

        $("#WinSlots"     ).text(gbService.toMoneyStr(data.WinSlots));
        $("#HandPayments" ).text(gbService.toMoneyStr(data.TotalHandPayments));

        $("#SpecialPromos"   ).text(gbService.toMoneyStr(data.SpecialPromos));
        $("#GrantedPromos"   ).text(gbService.toMoneyStr(data.GrantedPromos));
        $("#CancelledPromos" ).text(gbService.toMoneyStr(data.CancelledPromos));
        $("#ConsumeredPromos").text(gbService.toMoneyStr(data.ConsumedPromos));
        $("#OverShortPromos" ).text(gbService.toMoneyStr(data.OverShortPromos));
        if (data.OverShortPromos < 0) {
            $("#OverShortPromos").removeClass("bg-blue");
            $("#OverShortPromos").addClass("bg-red");
        }
        else {
            $("#OverShortPromos").removeClass("bg-red");
        }
        $("#DataCancelled").text( getCancelledPromoRateStr(data) );
        $("#DataConsumed").text( getConsumedPromoRateStr(data) );

        $("#TotalMoneyIn"   ).text(gbService.toMoneyStr(data.TotalMoneyIn));
        $("#TotalMoneyOut"  ).text(gbService.toMoneyStr(data.TotalMoneyOut));
        $("#TotalDPromotion").text(gbService.toMoneyStr(data.TotalDPromotion));

        $("#TotalSessions"  ).text(data.TotalSessions);
        $("#TotalPlayers"   ).text(data.TotalPlayers);
        $("#TotalSlotOccupied").text(data.TotalSlotOccupied);
        $("#TotalSlots"     ).text(data.TotalSlots);
        
        $("#SlotOccupationRate").text(getSlotOccupationRateStr(data));
        $("#DataSlotOccupied").text(getSlotOccupationRateStr(data));
        $("#DataSlotEmpty").text(getEmptySlotRateStr(data));
    }

    var getAxisFor1Week = function (data) {
        var result = [];
        if (data.OSTrendFor1Week) {
            for (var i = 0; i < data.OSTrendFor1Week.length; i++) {
                if (data.OSTrendFor1Week[i]) {
                    result.push(data.OSTrendFor1Week[i].Day);
                }
            }
        }
        return result;
    }

    var getAxisFor7Week = function (data) {
        var result = [];
        if (data.OSTrendFor7Week) {
            for (var i = 0; i < data.OSTrendFor7Week.length; i++) {
                if (data.OSTrendFor1Week[i]) {
                    result.push(data.OSTrendFor7Week[i].Day);
                }
            }
        }
        return result;
    }

    var getTrendFor1WeekWinSlots = function (data) {
        var result = [];
        if (data.OSTrendFor1Week) {
            for (var i = 0; i < data.OSTrendFor1Week.length; i++) {
                if (data.OSTrendFor1Week[i]) {
                    result.push(data.OSTrendFor1Week[i].WinSlots);
                }
            }
        }
        return result;
    }

    var getTrendFor7WeekWinSlots = function (data) {
        var result = [];
        if (data.OSTrendFor7Week) {
            for (var i = 0; i < data.OSTrendFor7Week.length; i++) {
                if (data.OSTrendFor7Week[i]) {
                    result.push(data.OSTrendFor7Week[i].WinSlots);
                }
            }
        }
        return result;
    }

    var getTrendFor1WeekNetWin = function (data) {
        var result = [];
        if (data.OSTrendFor1Week) {
            for (var i = 0; i < data.OSTrendFor1Week.length; i++) {
                if (data.OSTrendFor1Week[i]) {
                    result.push(data.OSTrendFor1Week[i].NetWinAfterTax);
                }
            }
        }
        return result;
    }

    var getTrendFor7WeekNetWin = function (data) {
        var result = [];
        if (data.OSTrendFor7Week) {
            for (var i = 0; i < data.OSTrendFor7Week.length; i++) {
                if (data.OSTrendFor7Week[i]) {
                    result.push(data.OSTrendFor7Week[i].NetWinAfterTax);
                }
            }
        }
        return result;
    }

    var changeSummaryRow = function (api) {
        // Remove the formatting to get integer data for summation
        var intVal = function (i) {
            return typeof i === 'string' ?
                i.replace(/[\$,\%]/g, '') * 1 :
                typeof i === 'number' ?
                i : 0;
        };
        for (var ii = 1; ii <= 7; ii++) {
            // Total over this page
            if (api.column(ii).data().length) {
                var pageTotal = api
                    .column(ii, { page: 'current' })
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    })
            }
            else { pageTotal = 0 };

            // Update footer
            if (ii <= 6)
                $(api.column(ii).footer()).html(gbService.toMoneyStr(pageTotal));
            else
                $(api.column(ii).footer()).html(pageTotal);
        }
    }

    //Creating DataTable
    var dataTableInit = function (datalist) {
        var table;
        var lengthMenu = [[10, 25, 50, -1], [10, 25, 50, gbService.getResourceMsg('all')]];

        var ajaxCall = {
            url: "/Dashboard/OperationSummaryDataTableHandler",
            type: "POST",
            data: function (data) {
                gbService.showQueryWaitingInfo(); //show query waiting info
                saveParamToLocal(data); //save parameters to local storage
                return data = gbService.getParamFromGbiForm(data);
            }
        };

        var buttons = [
                    { extend: 'colvis', text: gbService.getResourceMsg('btnchoosecolumn'), columns: ':not(:first-child)' },
                    { extend: 'excel',  text: gbService.getResourceMsg('btnexcel') },
        ];

        var columnDefs = [
                    {
                        targets: 1, /*column index counting from the left*/
                        className: "dt-right", /* Right align text in the header and body */
                        render: function (data, type, row) { return gbService.toMoneyStr(data); },
                    },
                    { targets: 2, className: "dt-right", render: function (data, type, row) { return gbService.toMoneyStr(data); }, },
                    { targets: 3, className: "dt-right", render: function (data, type, row) { return gbService.toMoneyStr(data); }, },
                    { targets: 4, className: "dt-right", render: function (data, type, row) { return gbService.toMoneyStr(data); }, },
                    { targets: 5, className: "dt-right", render: function (data, type, row) { return gbService.toMoneyStr(data); }, },
                    { targets: 6, className: "dt-right", render: function (data, type, row) { return gbService.toMoneyStr(data); }, },
                    { targets: 7, className: "dt-right", },
        ];

        // define column content
        var columns = [
                    { data: "Provider", className: "all" },
                    { data: "CashIn" },
                    { data: "CashOut" },
                    { data: "HandPayments" },
                    { data: "D_Promotion" },
                    { data: "NetWin" },
                    { data: "Win" },
                    { data: "CantPlayer" }
        ];

        //define dom for the structure of the datatable
        var dom = "<'row'<'col-sm-3'l><'col-sm-5 text-center'B><'col-sm-4'f>>" +
                    "<'row'<'col-sm-12'tr>>" +
                    "<'row'<'col-sm-5'i><'col-sm-7'p>>";

        //specified using server side processing for huge table
        if (gbService.getResourceMsg('servercache').toLowerCase() == 'true') {
            table = $('#providerlist').DataTable({
                language: { "url": gbService.getResourceMsg('datatablelanguage') },
                destroy: true,
                processing: true,
                ordering: true,
                serverSide: true,
                paging: true,
                searching: true,
                responsive: true,
                lengthMenu: lengthMenu,
                pageLength: parseInt(gbService.getResourceMsg('pagelength')),
                ajax: ajaxCall,
                drawCallback: function () {
                    gbService.clearQueryWaitingInfo(); //clean up query waiting info
                },
                dom: dom, 
                buttons: buttons,
                columnDefs: columnDefs,
                columns: columns,
                footerCallback: function (row, data, start, end, display) {
                    changeSummaryRow(this.api());
                }
            });
        }
        else //DataTable init with local cache
        {
            table = $('#providerlist').DataTable({
                language: { "url": gbService.getResourceMsg('datatablelanguage') },
                destroy: true,
                processing: true,
                ordering: true,
                data: datalist,
                spaging: true,
                searching: true,
                responsive: true,
                lengthMenu: lengthMenu,
                stateSave: true,
                pageLength: parseInt(gbService.getResourceMsg('pagelength')),
                drawCallback: function () {
                    gbService.clearQueryWaitingInfo(); //clean up query waiting info
                },
                dom: dom,
                buttons: buttons,
                columnDefs: columnDefs,
                columns: columns,
                footerCallback: function (row, data, start, end, display) {
                    changeSummaryRow(this.api());
                }
            });
        }
    }

    var redrawCharts = function () {
        barChart1Week.resize(barChart1Week.render, true);
        barChart7Week.resize(barChart7Week.render, true);
    }

    var drawOccupationDonutChart = function (data, option) {
        if (donutChartOccupation != null) {
            //donutChartOccupation.clear(); //clean old canvas
            donutChartOccupation.destroy(); //destroy old chart
        }
        // Get context with jQuery - using jQuery's .get() method.
        var donutChartCanvas = $("#occupationDonutChart").get(0).getContext("2d");
        var donutData = [
            {
                value: data.TotalSlotOccupied,
                color: gbService.getColor('green'),
                highlight: gbService.getColor('green'),
                label: gbService.getResourceMsg("slot-occupied")
            },
            {
                value: data.TotalSlots - data.TotalSlotOccupied,
                color: gbService.getColor('grey'),
                highlight: gbService.getColor('grey'),
                label: gbService.getResourceMsg("slot-empty")
            }
        ];
        //Create douhnut chart, You can switch between pie and douhnut using the method below.
        donutChartOccupation = new Chart(donutChartCanvas).Doughnut(donutData, option);
    }

    var drawPromotionDonutChart = function (data, option) {
        if (donutChartPromotion != null) {
            //donutChartPromotion.clear(); //clean old canvas
            donutChartPromotion.destroy(); //destroy old chart
        }
        var donutChartCanvas = $("#promotionDonutChart").get(0).getContext("2d");
        var donutData = [
            {
                value: data.CancelledPromos,
                color: gbService.getColor('grey'),
                highlight: gbService.getColor('grey'),
                label: "$"
            },
            {
                value: data.ConsumedPromos,
                color: gbService.getColor('aqua'), 
                highlight: gbService.getColor('aqua'),
                label: "$"
            },
        ];
        //Create douhnut chart, You can switch between pie and douhnut using the method below.
        donutChartPromotion = new Chart(donutChartCanvas).Doughnut(donutData, option);
    }

    var drawBarChart1Week = function (data, option) {
        if (barChart1Week != null) {
            barChart1Week.destroy();
        }
        var barChartCanvas = $("#chartTrend1Week").get(0).getContext("2d");
        var barChartData = {
            labels: getAxisFor1Week(data),
            datasets: [
                {
                    label: "Win Slots",
                    fillColor: gbService.getBgColor('green', 0.5),
                    strokeColor: gbService.getBgColor('green', 0.8),
                    highlightFill: gbService.getBgColor('green', 0.75),
                    highlightStroke: gbService.getBgColor('green', 1),
                    data: getTrendFor1WeekWinSlots(data)
                },
                {
                    label: "Net Win After Tax",
                    fillColor: gbService.getBgColor('yellow', 0.5),
                    strokeColor: gbService.getBgColor('yellow', 0.8),
                    highlightFill: gbService.getBgColor('yellow', 0.75),
                    highlightStroke: gbService.getBgColor('yellow', 1),
                    data: getTrendFor1WeekNetWin(data)
                },

            ]
        };
        //gbService.setLocalObject('chartTrend1Week', barChartData);
        barChart1Week = new Chart(barChartCanvas).Bar(barChartData, option);
        ////Adding legend
        //var legendHolder = document.createElement('div');
        //legendHolder.innerHTML = barChart1Week.generateLegend();
        //$("#chartLengend1Week").append(legendHolder.firstChild);
    }

    var drawBarChart7Week = function (data, option) {
        if (barChart7Week != null) {
            barChart7Week.destroy();
        }
        var barChartCanvas = $("#chartTrend7Week").get(0).getContext("2d");
        var barChartData = {
            labels: getAxisFor7Week(data),
            datasets: [
                {
                    label: "Win Slots",
                    fillColor: gbService.getBgColor('green', 0.5),
                    strokeColor: gbService.getBgColor('green', 0.8),
                    highlightFill: gbService.getBgColor('green', 0.75),
                    highlightStroke: gbService.getBgColor('green', 1),
                    data: getTrendFor7WeekWinSlots(data)
                },
                {
                    label: "Net Win After Tax",
                    fillColor: gbService.getBgColor('yellow', 0.5),
                    strokeColor: gbService.getBgColor('yellow', 0.8),
                    highlightFill: gbService.getBgColor('yellow', 0.75),
                    highlightStroke: gbService.getBgColor('yellow', 1),
                    data: getTrendFor7WeekNetWin(data)
                },
            ]
        };
        //save the chart1Week data to local which will be used by redraw
        //gbService.setLocalObject('chartTrend7Week', barChartData);
        barChart7Week = new Chart(barChartCanvas).Bar(barChartData, option);

        //Adding legend
        //var legendHolder = document.createElement('div');
        //legendHolder.innerHTML = barChart7Week.generateLegend();
        //$("#chartLengend1Week").append(legendHolder.firstChild);
    }

    var loadAllData = function (data, isAnimate) {
        // Save parameters to local storage
        saveParamToLocal(data);

        // Fill all numberic fields
        changeNumbericText(data);
        changeTotal(data);

        if (isAnimate) {
            // Donut CHART
            drawOccupationDonutChart(data, gbService.getDonutOptions());
            drawPromotionDonutChart(data, gbService.getDonutOptions());
            // BAR CHART
            drawBarChart1Week(data, gbService.getBarChartOptions());
            drawBarChart7Week(data, gbService.getBarChartOptions());
        }
        else {
            // Donut CHART
            drawOccupationDonutChart(data, gbService.getDonutNoAnimationOptions());
            drawPromotionDonutChart(data, gbService.getDonutNoAnimationOptions());
            // BAR CHART
            drawBarChart1Week(data, gbService.getBarChartNoAnimationOptions());
            drawBarChart7Week(data, gbService.getBarChartNoAnimationOptions());
        }

        //Loading DataTable  
        dataTableInit(data.ProviderList);
    }

    var getPDFTableColumns = function () {
        var tableColumns = [];
        tableColumns.push({ text: gbService.getResourceMsg('columnprovider'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnmoneyin'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnmoneyout'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnhandpayments'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columndpromotion'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnnetwin'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnwin'), style: 'tableheader' });
        tableColumns.push({ text: gbService.getResourceMsg('columnplayers'), style: 'tableheader' });
        return tableColumns;
    }

    var getPDFTableTotal = function (data, fistcolumnstyle, style) {
        var tableTotal = [];
        tableTotal.push({ text: gbService.getResourceMsg('total'), style: fistcolumnstyle });
        tableTotal.push({ text: gbService.toMoneyStr(data.TotalMoneyIn), style: style });
        tableTotal.push({ text: gbService.toMoneyStr(data.TotalMoneyOut), style: style });
        tableTotal.push({ text: gbService.toMoneyStr(data.TotalHandPayments), style: style });
        tableTotal.push({ text: gbService.toMoneyStr(data.TotalDPromotion), style: style });
        tableTotal.push({ text: gbService.toMoneyStr(data.TotalNetWin), style: style });
        tableTotal.push({ text: gbService.toMoneyStr(data.TotalWin), style: style });
        tableTotal.push({ text: data.TotalCantPlayer.toString(), style: style });
        return tableTotal;
    }

    var getPDFTableBody = function (data) {
        var tableBody = [];
        tableBody.push(getPDFTableColumns());

        var list = data.ProviderList;
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
            tableRow.push({ text: list[i]['Provider'], style: firstcolumnstyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['CashIn']), style: valueStyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['CashOut']), style: valueStyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['HandPayments']), style: valueStyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['D_Promotion']), style: valueStyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['NetWin']), style: valueStyle });
            tableRow.push({ text: gbService.toMoneyStr(list[i]['Win']), style: valueStyle });
            tableRow.push({ text: list[i]['CantPlayer'].toString(), style: valueStyle });
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

        tableBody.push(getPDFTableTotal(data, firstcolumnstyle, valueStyle)); //add the tableTotal to tableBody
        return tableBody;
    }

    var getPDFTable = function (data) {
        var table = {
            headerRows: 1,
            widths: [80, 60, 60, 70, 60, 50, 50, '*'] ,
            body: getPDFTableBody(data),
        }
        return table;
    }
     
    var getParameterString = function () {
        var start = gbService.getDateRangeStartTime(); //string
        var end = gbService.getDateRangeEndTime(); //sring
        return gbService.getResourceMsg('start') + ' ' + start + '   ' + gbService.getResourceMsg('end') + ' ' + end;
    }

    var exportPDF = function () {
        var start = gbService.getDateRangeStartTime(); //string
        var end = gbService.getDateRangeEndTime(); //sring
        if (!isParameterValid(start, end)) return;

        $('input[name="queryParam[start]"]').val(start);
        $('input[name="queryParam[end]"]').val(end);

        //show query waiting info
        gbService.showQueryWaitingInfo();
        //Create all area charts and tables through ajax
        getOperationSummaryList(start, end, false, loadPDFData);
    }

    var loadPDFData = function (data, isAnimate) {
        //create image for every chart used by pdf export
        drawOccupationDonutChart(data, gbService.getDonutNoAnimationOptions());
        drawPromotionDonutChart(data, gbService.getDonutNoAnimationOptions());
        drawBarChart1Week(data, gbService.getBarChartNoAnimationOptions());
        drawBarChart7Week(data, gbService.getBarChartNoAnimationOptions());

        var imgTrend1Week = $("#chartTrend1Week")[0].toDataURL();
        var imgTrend7Week = $("#chartTrend7Week")[0].toDataURL();

        var imgPromotion = $("#promotionDonutChart")[0].toDataURL();
        var imgOccupation = $("#occupationDonutChart")[0].toDataURL();

        var imgWhiteCircle = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAeAB4AAD/4QAiRXhpZgAATU0AKgAAAAgAAQESAAMAAAABAAEAAAAAAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAATABQDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9yvFvi3UrvxMuh6GsK3YQST3Egytup6cc+3Y1R1S58UeBIP7QuL6HWLGLm4j8gRvEO5B74/yo602/v1+H3xSub29Vl0/VoVVZgpIicY4P5VZ+IPxB0+XQZrGxuIdQvtQQwwxwuJPvcZJHA/nWnVJIk63TNWj1bTobqBt0NwgkU+xoqn4K0h9A8KWNnId0kEQDE+p5P86KnQo0LvT4NTtnhuIY54WPKSKGU/gap6L4W03SJzNa2Fnby/d3xxBWA+tFFLoBpr3+tFFFID//2Q==";
        var imgAquaCircle  = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAATCAIAAAAf7rriAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAGuSURBVDhPpZO/S8NAFMeLKCiC4OAmjoKTiIogOIoIDi7i4OI/IC4uLrq5iCDuDm7Oint/KEVtlbZWBG2LtmKV1qSXtEmTNvWbvjNt0qYOfrnlvvc+7929u/PU/qFOcFYz0qph8FkbOWGEXuT1mTDr8Qkerzm6vMLYTeH0S9Nb0tjgb91YjEqIJswx5u6l97ItQQP+1IzpMEMQ4MkQO8mWX5QqxllOm71jlHEixBDGAQuGsfFcwjJ2u/eqODaI6VFG7fWb/GpctlY5HJErQ1ci1pDCQVraSSmoPxAQAqJODof331SQI0ExqVTJaRU2jM4hbCtRIofDK3EZ7nxEcitLorCFiERTDmPe7Lpps96XqTArVMwph5ditpRuWn8qtqm8m1Lgdj5zXjfGb827RH1yOIwGoo1oJlpKTqsOM2q3T+jzC+c5jRwOo0+4QGTt9wvHH2Uym4WnguwIwAHV365yGMJN4AFhGfWXH+SoXKEYHGTtsUgvZDgoxuTGuRowlFCqxLcdo9cFZOShddlgCF/nIK3Sa7PG4KW4nVSYDTTlhC3hM9PH6PClXeG/Vav9AIFq8fetO/2sAAAAAElFTkSuQmCC";
        var imgGreenCircle = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABMAAAARCAIAAACw+gCQAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAHUSURBVDhPhVM9SEJRGI0agoaioiCCoqYaGhoboz2iMSOCsqIogwJpcYgagqSppakWrbQ/UYzIoSAxSLICBTMxMU3UXmr+9vR1nvf5er2MDt/wvvOdc9+93/1uGfMHMrlPf5JK0hku/wWx8z2bWH44bD6aKVMNkajXTsisO6F0jFMU8cN56r9rPJjiPcKo0YxpvJY8k+ekQqfaY67aG4WoTitV3GvsUZ8vGXmKB5UOAxjwlbsjKs8Vp+adrvhr24kM5Z4zxUvyjZA8wul47/kKqtiRjfIQknPKb9UowIwlCCNCIEV16hehGb/eIntmnZFMvNu4BHbdoS/ISmPbfVGukmB19Bwp67ynvA0Hk9X7Y+aQs6ApDSJDqyzhR6SsE1/IeeovoGEtx7MVaonOZ0XKOm8i7lqN9F+nMxZoOpz+8c9gKtqhX/j3nLhPnLP1eM6bCCNlnegVOgYnuoceFmRiUNkPXBg0g5cb2RwNhnUCuCUyPf0X6xARkkeCTg+bN1EVnohzApgPTAnK7SfzmCcy65h7o9/WZZCDx1Yx0vwAfjtBbblMZAB/BxZds+vofI5TC50Eno/QwKWS/JwErqHPtHpHPXOKIsROAvI4cYGI0k+UYb4AAJkN9xFUGToAAAAASUVORK5CYII=";

        //define pdf style dictionar
        var contents = [
            { text: 'Rpt #: ' + gbService.getResourceMsg('reportname'), style: 'reportname' },
            { text: gbService.getResourceMsg('title'), style: 'header' },
            { text:' '},
            { text: getParameterString(), style: 'parameter' },
            { text: ' '},
            {
                table:
                {
                    widths: [180, 180, '*'],
                    body:
                    [
                        [
                            { text: gbService.getResourceMsg('titlewinslots'), style: 'title' },
                            { text: gbService.getResourceMsg('titleplayeraccount'), style: 'title' },
                            { text: gbService.getResourceMsg('titlenetwinaftertax'), style: 'title' },
                        ],
                        [
                            { text: '$ ' + gbService.toMoneyStr(data.WinSlots), style: 'title' },
                            { text: '$ ' + gbService.toMoneyStr(data.PlayerAccountAmount), style: 'title' },
                            { text: '$ ' + gbService.toMoneyStr(getNetWinAfterTax(data)), style: 'title' },
                        ],
                        [
                            {
                                table: {
                                    widths: [80, 80],
                                    body: [
                                        [{ text: gbService.getResourceMsg('numtotalmoneyin'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.TotalMoneyIn), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numtotalmoneyout'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.TotalMoneyOut), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numhandpayment'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.TotalHandPayments), style: 'smallvalue' }],
                                    ],
                                },
                                layout: 'noBorders'
                            },
                            {
                                table: {
                                    widths: [80, 80],
                                    body: [
                                        [{ text: gbService.getResourceMsg('numplayercashin'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.PlayerAccountIn), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numplayercashout'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.PlayerAccountOut), style: 'smallvalue' }],
                                    ],
                                },
                                layout: 'noBorders'
                            },
                            {
                                table: {
                                    widths: [80, 80],
                                    body: [
                                        [{ text: gbService.getResourceMsg('numnetwincashin'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.NetWinCashIn), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numnetwincashout'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.NetWinCashOut), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numtaxreservation'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.Taxes), style: 'smallvalue' }], 
                                    ],
                                },
                                layout: 'noBorders'
                            },
                        ],
                    ],
                },
                layout: 'noBorders',
            },
            { text: ' ' },
            { text: gbService.getResourceMsg('titlepromotion') + ' - $ ' + gbService.toMoneyStr(data.ConsumedPromos), style: 'title' },
            {
                table:
                {
                    widths: [40, 230, '*'],
                    body:
                    [
                        [
                            { text: ' '},
                            {
                                table: {
                                    widths: [140, '*'],
                                    body: [
                                        [{ text: gbService.getResourceMsg('numpromotiongranted'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.GrantedPromos), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numpromotioncancelled'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.CancelledPromos), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numpromotionovershot'), style: 'subtitle' }, { text: gbService.toMoneyStr(data.OverShortPromos), style: 'smallvalue' }],
                                    ],
                                },
                                layout: 'noBorders'
                            },
                            {
                                table: {
                                    widths: [100, '*'],
                                    body:
                                    [
                                        [
                                            { image: imgPromotion, width: 100 },
                                            {
                                                table: {
                                                    widths: [10, '*'],
                                                    body: [
                                                        [{ image: imgWhiteCircle, width: 10 }, { text: getCancelledPromoRateStr(data) + ' ' + gbService.getResourceMsg('promotioncancelled'), style: 'tablefirstcolumn1' }],
                                                        [{ image: imgAquaCircle, width: 10 }, { text: getConsumedPromoRateStr(data) + ' ' + gbService.getResourceMsg('promotionconsumed'), style: 'tablefirstcolumn1' }],
                                                    ]
                                                },
                                                layout: 'noBorders'
                                            }
                                        ],
                                    ]
                                },
                                layout: 'noBorders'
                            },
                        ]
                    ],
                },
                layout: 'noBorders',
            },
            { text: ' ' },
            { text: gbService.getResourceMsg('titleslotoccupation') + ' - ' + getSlotOccupationRateStr(data), style: 'title' },
            {
                table:
                {
                    widths: [40, 230, '*'],
                    body:
                    [
                        [
                            { text: ' '},
                            {
                                table: {
                                    widths: [140, '*'],
                                    body: [
                                        [{ text: gbService.getResourceMsg('numsessions'), style: 'subtitle' }, { text: data.TotalSessions.toString(), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numtotalplayers'), style: 'subtitle' }, { text: data.TotalPlayers.toString(), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numtotaloccupied'), style: 'subtitle' }, { text: data.TotalSlotOccupied.toString(), style: 'smallvalue' }],
                                        [{ text: gbService.getResourceMsg('numtotalslots'), style: 'subtitle' }, { text: data.TotalSlots.toString(), style: 'smallvalue' }],
                                    ],
                                },
                                layout: 'noBorders'
                            },
                            {
                                table: {
                                    widths: [100, '*'],
                                    body: [
                                        [
                                            { image: imgOccupation, width: 100 },
                                            {
                                                table: {
                                                    widths: [10, '*'],
                                                    body: [
                                                        [{ image: imgWhiteCircle, width: 10 }, { text: getEmptySlotRateStr(data) + ' ' + gbService.getResourceMsg('slot-empty'), style: 'tablefirstcolumn1' }],
                                                        [{ image: imgGreenCircle, width: 10 }, { text: getSlotOccupationRateStr(data) + ' ' + gbService.getResourceMsg('slot-occupied'), style: 'tablefirstcolumn1' }],
                                                    ]
                                                },
                                                layout: 'noBorders'
                                            }
                                        ],
                                    ]
                                },
                                layout: 'noBorders'
                            },
                        ]
                    ],
                },
                layout: 'noBorders',
            },

            { text: ' ' },
            {
                table:
                {
                    widths: [270, '*'],
                    body:
                    [
                        [{ text: gbService.getResourceMsg('titletrends7days'), style: 'charttitle' }, { text: gbService.getResourceMsg('titletrends7weeks'), style: 'charttitle' }],
                        [{ image: imgTrend1Week, width: 270 }, { image: imgTrend7Week, width: 270 }],
                    ]
                },
                layout: 'noBorders'
            },
            { text: ' ' },
            { table: getPDFTable(data), layout: gbService.getPDFTableLayout() },
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

        var start = gbService.getDateRangeStartTime(); //string
        var end = gbService.getDateRangeEndTime(); //sring
        if (!isParameterValid(start, end)) return;

        $('input[name="queryParam[start]"]').val(start);
        $('input[name="queryParam[end]"]').val(end);

        //show query waiting info
        gbService.showQueryWaitingInfo();
        //Create all area charts and tables through ajax
        getOperationSummaryList(start, end, true, loadAllData);
    }

    var pageInit = function () {
        //Load Parameters from Local Storage if exist
        loadParamFromLocal();

        //Init the Right Click buttons
        //gbService.initContextMenu();

        //Init the Money Format
        gbService.initMoneyFormat();

        //Init the DateRange Control
        gbService.initDateRange();

        dragInit();
        //iCheck for checkbox and radio inputs
        $('input[type="checkbox"].minimal, input[type="radio"].minimal').iCheck({
            checkboxClass: 'icheckbox_minimal-blue',
            radioClass: 'iradio_minimal-blue'
        });

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
        redrawCharts: redrawCharts,
    }
}();

$(document).ready(function () {
    operationSummary.pageInit();
});