var MesReport = function (o) {
    this.ReportClassName = o.ReportClassName ? o.ReportClassName : "MESReport.Test.TEST1";
    this.InitClassName = o.InitClassName ? o.InitClassName : "MESStation.Report.CallReport";
    this.InitFunctionName = o.InitFunctionName ? o.InitFunctionName : "GetReport";
    this.RunClassName = o.InitClassName ? o.InitClassName : "MESStation.Report.CallReport";
    this.RunFunctionName = o.InitFunctionName ? o.InitFunctionName : "RunReport";
    this.InputClassName = o.InitClassName ? o.InitClassName : "MESStation.Report.CallReport";
    this.InputFunctionName = o.InitFunctionName ? o.InitFunctionName : "RunReport";
    this.Client = o.Client;
    this.ReportJson = null;
    this.Report = null;
    this.OnInit = o.Init;
    this.IScale = o.IScale;
    this.IContainer = o.IContainer;
    this.OContainer = o.OContainer;
    MesReport.prototype.constructor = MesReport;
    MesReport.prototype.ObjList = {};
    MesReport.prototype.Init = function () {
        MesReport.prototype.StationList = {};
        var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        this.ListenStationData(MessageID);
        this.Client.CallFunction(this.InitClassName, this.InitFunctionName, { ClassName: this.ReportClassName }, this.InitCallBack, MessageID);
    };
    MesReport.prototype.ListenStationData = function (k) {
        MesReport.prototype.ObjList[k] = this;
    };
    MesReport.prototype.InitCallBack = function (d) {
        var _Mes = MesReport.prototype.ObjList[d.MessageID];
        delete MesReport.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            _Mes.ReportJson = d.Data;
            _Mes.Report = new Report(d.Data);
            if (_Mes.IContainer) {
                _Mes.ShowInputs(_Mes.IContainer);
            }
            if (_Mes.OContainer) {
                _Mes.ShowOutputs(_Mes.OContainer);
            }
            if (_Mes.OnInit != undefined) {
                _Mes.OnInit(d);
            }
        }
        else {
            swal("error", d.Message, "error");
        }
    };
    MesReport.prototype.SetInputValue = function (Name, value) {
        for (var i = 0; i < this.Report.Inputs.length; i++) {
            if (this.Report.Inputs[i].Name == Name) {
                this.Report.Inputs[i].Value = value;
                this.ReportJson.Inputs[i].Value = value;
            }
        }
    };
    MesReport.prototype.ShowInput = function (obj) {
        for (var i = 0; i < this.Inputs.length; i++) {
            if (this.Report.Inputs[i].DisplayName == obj.InputName) {
                obj.Container.find("input.form-control").unbind("keyup");
                obj.Container.find("select.form-control").unbind("change");
                this.Report.Inputs[i].Remove();
                this.Report.Inputs[i].Show({ Client: this.Client, Container: obj.Container, Scale: (obj.Scale == undefined ? (this.IScale == undefined ? "3:9" : this.IScale) : obj.Scale) });
                this.Report.Inputs[i].SetEnable();
                obj.Container.find("input.form-control:not(.datepicker)").bind("keyup", { Report: this }, function (event) {
                    event.data.Report.SetInputValue(this.name, this.value);
                    event.data.Report.InputPostBack();
                });
                obj.Container.find("input.form-control.datepicker").bind("change", { Report: this }, function (event) {
                    event.data.Report.SetInputValue(this.name, this.value);
                    event.data.Report.InputPostBack();
                });
                obj.Container.find("select.form-control").bind("change", { Report: this }, function (event) {
                    event.data.Report.SetInputValue(this.name, this.value);
                    event.data.Report.InputPostBack();
                });
                break;
            }
        }
    };
    MesReport.prototype.ShowInputs = function (Container) {
        Container.find("input.form-control").unbind("keyup");
        Container.find("select.form-control").unbind("change");
        for (var i = 0; i < this.Report.Inputs.length; i++) {
            this.Report.Inputs[i].Remove();
            this.Report.Inputs[i].Show({ Client: this.Client, Container: Container, Scale: (this.IScale == undefined ? "3:9" : this.IScale) });
            this.Report.Inputs[i].SetEnable();
        }
        Container.find("input.form-control").bind("keyup", { Report: this }, function (event) {
            event.data.Report.SetInputValue(this.name, this.value);
            event.data.Report.InputPostBack();
        });
        Container.find("input.form-control.datepicker").bind("change", { Report: this }, function (event) {
            event.data.Report.SetInputValue(this.name, this.value);
            event.data.Report.InputPostBack();
        });
        Container.find("select.form-control").bind("change", { Report: this }, function (event) {
            event.data.Report.SetInputValue(this.name, this.value);
            event.data.Report.InputPostBack();
        });
    };
    MesReport.prototype.ShowOutput = function (obj) {
        obj.Container.find(".J_menuItem").off("click");
        for (var i = 0; i < this.Report.Outputs.length; i++) {
            if (this.Report.Outputs[i].Name == obj.OutputName) {
                this.Report.Outputs[i].Remove();
                this.Report.Outputs[i].Show(obj.Container);
                break;
            }
        }
        obj.Container.find(".J_menuItem").on("click", OnLinkClick);
    };
    MesReport.prototype.ShowOutputs = function (Container) {
        Container.find(".J_menuItem").off("click");
        Container.empty();
        for (var i = 0; i < this.Report.Outputs.length; i++) {
            if (this.Report.Outputs[i].OutputType != "ReportAlart")
                this.Report.Outputs[i].Remove();
            this.Report.Outputs[i].Show(Container, (this.OScale == undefined ? "3:9" : this.OScale));
        }
        Container.find(".J_menuItem").on("click", OnLinkClick);
    };
    MesReport.prototype.InputPostBack = function () {
    };
    MesReport.prototype.InputCallBack = function (d) {
    };
    MesReport.prototype.Run = function () {
        self.parent.Loading(true);
        var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        this.ListenStationData(MessageID);
        this.Client.CallFunction(this.RunClassName, this.RunFunctionName, { ClassName: this.ReportClassName, Report: this.ReportJson }, this.CallBack, MessageID);

    };
    MesReport.prototype.CallBack = function (d) {
        var _Mes = MesReport.prototype.ObjList[d.MessageID];
        delete MesReport.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            _Mes.ReportJson = d.Data;
            _Mes.Report = new Report(d.Data);
        }
        if (_Mes.IContainer) {
            _Mes.ShowInputs(_Mes.IContainer);
        }
        if (_Mes.OContainer) {
            _Mes.ShowOutputs(_Mes.OContainer);
        }
        self.parent.Loading(false);
    };
    MesReport.prototype.initStyleSelect = function () {
        var styleValues = getCookie("messtylename");
        if (styleValues != null)
            StyleChange(styleValues);
        var c = $("#StyleSelect");
        var s1 = $("<button type=\"button\" class=\"btn btn-default\" id=\"Stylelabel\">Style</button>");
        var s2 = $(" <button type=\"button\" class=\"btn btn-default dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"> <span class=\"sr-only\">Toggle Dropdown</span></button>");
        c.append(s1, s2);
        var o = $(" <ul id=\"ss\" class=\"dropdown-menu badge-primary selected-row\">  </ul>");
        c.append(o);
        var sbtn = $("#ss");
        for (x in themes) {
            var l = $(" <li><a href=\"#\" data-theme=\"" + x + "\" class=\"theme-link\">" + x + "</a> </li>");
            sbtn.append(l);
        }
        themesheet.appendTo('head');
        $('.theme-link').click(function () {
            StyleChange($(this).attr('data-theme'));
        });
    };
    this.Init();
};
var Report = function (obj) {
    this.Inputs = [];
    this.Outputs = [];
    this.Sqls = {};
    this.RunSqls = [];
    this.LayoutRows = obj.LayoutRows;
    this.LayoutCols = obj.LayoutCols;
    this.Inputs.splice(0, this.Inputs.length);
    this.Outputs.splice(0, this.Outputs.length);
    for (var i = 0; i < obj.Inputs.length; i++) {
        var ip = new ReportInput(obj.Inputs[i]);
        this.Inputs.push(ip);
    }
    for (var i = 0; i < obj.Outputs.length; i++) {
        var op = new ReportOutput(obj.Outputs[i]);
        this.Outputs.push(op);
    }
    Report.prototype.constructor = Report;
};
var ReportInput = function (obj) {
    this.InputType = obj.InputType;
    this.Name = obj.Name;
    this.Value = obj.Value;
    this.ValueForUse = obj.ValueForUse;
    this.Enable = obj.Enable == undefined ? true : obj.Enable;
    this.SendChangeEvent = obj.SendChangeEvent == undefined ? true : obj.SendChangeEvent;
    this.RememberLastInput = obj.RememberLastInput == undefined ? false : obj.RememberLastInput;
    this.MessageID = obj.MessageID;
    ReportInput.prototype.constructor = ReportInput;
    ReportInput.prototype.Show = function (obj) {
        var E = new InputElements(obj.Client);
        var container = obj.Container;
        switch (this.InputType) {
            case "TXT":
                E.Text(container, this.Name, this.Name, this.Name, this.Value, this.RefershType, obj.Scale);
                break;
            case "Select":
                E.Select(container, this.Name, this.Name, this.Name, this.Value, this.ValueForUse, obj.Scale);
                break;
            case "DateTime":
                //E.Text(container, this.Name, this.Name, this.Name, this.Value, this.RefershType, obj.Scale);
                E.DataTimeText(container, this.Name, this.Name, this.Name, this.Value, this.RefershType, obj.Scale);
                break;
            default:

        }
    };
    ReportInput.prototype.SetFocus = function () {
        var selector = "#" + this.Name + "_" + this.ID;
        $(selector).select();
        $(selector).focus();
    };
    ReportInput.prototype.SetEnable = function (flag) {
        var selector = "#" + this.Name + "_" + this.ID;
        var f = (flag == undefined ? this.Enable : flag);
        if (f) {
            $(selector).attr("disabled", false);
        }
        else {
            $(selector).attr("disabled", true);
        }
    };
    ReportInput.prototype.Remove = function () {
        var selector = "[view-group=" + this.Name + "]";
        $(selector).find("input.form-control,select.form-control").unbind("keypress");
        $(selector).remove();
    };
};
var ReportOutput = function (obj) {
    this.OutputType = obj.OutputType;
    this.Tittle = obj.Tittle;
    this.TableRow = obj.Rows;
    this.TableColNames = obj.ColNames;
    this.ChartData = obj;
    this.ColCount = obj.ColCount;
    this.ColNum = obj.ColNum;
    this.RowNun = obj.RowNun;
    ReportOutput.prototype.constructor = ReportOutput;
    ReportOutput.prototype.Show = function (c, s) {
        var E = new OutputElements();
        switch (this.OutputType) {
            case "ReportChart":
                E.Chart(c, this.Tittle, this.ChartData, s);
                break;
            case "ReportTable":
                E.Table(c, this.Tittle.replace(" ", ""), this.TableRow, this.TableColNames);
                break;
            case "ReportAlart":
                E.Alart(c, obj.Msg, obj.AlartType);
                break;
            default:
                c.append("<span>DisplayType " + this.DisplayType + " undefined,input name " + this.Name + "</span>");
                break;

        }
    };
    ReportOutput.prototype.Remove = function () {
        this.Tittle == "" ? this.Tittle = "未定義Tittle" : this.Tittle;
        var selector = "[view-group=" + this.Tittle.replace(/\s/g, "") + "]";
        if (this.OutputType == "ReportTable") {
            $("#" + this.Tittle.replace(/\s/g, "")).bootstrapTable("destroy");
        }
        $(selector).remove();
    };
};
var InputElements = function (client) {
    this.client = client;
    InputElements.prototype.constructor = InputElements;
    InputElements.prototype.GetData = function (API, APIPara, ID, CallBack) {
        var ClassName = API.substr(0, API.lastIndexOf("."));
        var FunctionName = API.substr(API.lastIndexOf(".") + 1);
        var Params = {};
        var ParamsKey = [];
        var ParamsValeu = [];
        var PTemp1 = APIPara.split(',');
        for (var i = 0; i < PTemp1.length; i++) {
            var PTemp2 = PTemp1[i].split(':');
            ParamsKey.push(PTemp2[0]);
            ParamsValeu.push(PTemp2[1]);
        }
        for (var i = 0; i < ParamsKey.length; i++) {
            if (ParamsValeu[i].indexOf('[') >= 0) {
                var selector = "[name=" + ParamsValeu[i].substr(1, ParamsValeu[i].length - 2) + "]";
                Params[ParamsKey[i]] = $(selector).val();
            }
            else {
                Params[ParamsKey[i]] = ParamsValeu[i];
            }
        }
        this.client.CallFunction(ClassName, FunctionName, Params, CallBack, ID);
    };
    InputElements.prototype.Text = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\" " + (ScanFlag ? "data-scan=\"true\"" : "") + ">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control\" placeholder=\"" + placeholder + "\" value=\"" + value + "\">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
    InputElements.prototype.DataTimeText = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\" " + (ScanFlag ? "data-scan=\"true\"" : "") + ">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        //var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control\" placeholder=\"" + placeholder + "\" value=\"" + value + "\">");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control datepicker\" value=\"" + value + "\" data-date-format=\"yyyy-mm-dd hh:ii:ss\">");
        //var input = $("<input type=\"text\" value=\"2012-05-15 21:05\" id=\"datetimepicker\" data-date-format=\"yyyy-mm-dd hh:ii\">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
        var datatimetext = $("#"+ID);
        datatimetext.datetimepicker().on('changeDate', function (ev) {
            //$("#"+ID). ev.date.format("yyyy-MM-dd hh:mm:ss");
        });
    };
    InputElements.prototype.Select = function (c, ID, Label, placeholder, value, DataForUse, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + ":</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<select id=\"" + ID + "\" name=\"" + Label + "\" class=\"form-control\" placeholder=\"" + placeholder + "\" aria-describedby=\"basic-addon1\"></select>");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
        var select = $("#" + ID);
        select.empty();
        for (var x = 0; x < DataForUse.length; x++) {
            var op = $(" <option value=\"" + DataForUse[x] + "\"" + (value == DataForUse[x] ? "selected" : "") + ">" + DataForUse[x] + "</option>");
            select.append(op);
        }
    };
    InputElements.prototype.Checkbox = function (c, ID, Label, placeholder, value, RefershType, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"checkbox\" class=\"form-control\"" + (value ? "checked" : "") + ">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
    InputElements.prototype.Radio = function (c, ID, Label, placeholder, value, DataForUse, API, APIPara, RefershType, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var DataForUse = [];//get data 
        for (var i = 0; i < DataForUse.length; i++) {
            var label = $("<label class=\"radio-inline\"></label>");
            var radio = $("<input type=\"radio\" name=\"" + Label + "\" id=\"" + ID + "_" + i + "\"" + (value == DataForUse[i] ? "checked" : "") + ">");
            label.append(radio, DataForUse[i]);
            inputD.append(label);
        }
        div.append(label, inputD);
        c.append(div);
    };
    InputElements.prototype.Autocomplete = function (c, ID, Label, placeholder, value, DataForUse, API, APIPara, RefershType, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input class=\"form-control\" name=\"" + Label + "\" id=\"" + ID + "\" value=\"" + value + "\">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
        try {
            input.autocomplete("destroy");
        } catch (e) {

        }
        if (RefershType == "Default") {
            input.attr("PreValue", value);
            this.GetData(API, APIPara, ID, function (e) {
                if (e.Status == "Pass") {
                    $("#" + e.MessageID).autocomplete({
                        minLength: 0,
                        source: d.Data,
                        select: function (event, ui) {
                            var e = $.Event("keypress");
                            e.keyCode = 13;
                            $(this).val(ui.item.value);
                            $(this).trigger(e);
                        },
                        create: function (event, ui) {
                            $(this).bind("click", function () {
                                var active = $(this).attr("autocomplete");
                                if (active == "off") {
                                    $(this).autocomplete("search", "");
                                }
                            });
                        },
                        scroll: true,
                        scrollHeight: 180,
                        position: { my: "right top", at: "right bottom" }
                    });
                }
            });
        }
        else if (RefershType == "EveryTime") {
            $("#" + ID).autocomplete({
                minLength: 0,
                source: DataForUse,
                select: function (event, ui) {
                    var e = $.Event("keypress");
                    e.keyCode = 13;
                    $(this).val(ui.item.value);
                    $(this).trigger(e);
                },
                create: function (event, ui) {
                    $(this).bind("click", function () {
                        var active = $(this).attr("autocomplete"); //没有这一行，鼠标选择选项时，会触发输入的click事件，导致提示框不能关闭    
                        if (active == "off") {
                            $(this).autocomplete("search", "");
                        }
                    });
                },
                scroll: true,
                scrollHeight: 180,
                position: { my: "right top", at: "right bottom" }
            });
        }
    };
    InputElements.prototype.LocalChecker = function (ID, Label, placeholder, value, RefershType, MessageID) {
        swal({
            title: Label,
            text: placeholder,
            type: "input",
            showCancelButton: true,
            closeOnConfirm: false,
            showLoaderOnConfirm: true,
            animation: "slide-from-top",
            inputPlaceholder: placeholder
        },
        function (inputValue) {
            if (inputValue === false) return false;
            if (inputValue === "") {
                swal.showInputError(placeholder);
                return false;
            }
            if (inputValue !== value) {
                swal.showInputError("The value not match " + value + ",please input again!");
                return false;
            }
            if (inputValue == value) {
                this.client.CallFunction("", "", {}, function (e) { });
            }
        });
    };
};

var pieChart = function (c, ID, ChartData) {
    c.highcharts({
        chart: {
            backgroundColor: 'rgba(0,0,0,0)'
        },
        title: {
            text: ChartData.Title
        },
        tooltip: {
            headerFormat: '{series.name}<br>',
            pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
        },
        credits: {
            enabled: false // 禁用版权信息
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                showInLegend: true,
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        },
        subtitle: {
            text: ChartData.SubTitle
        },
        series: ChartData.ChartDatas
    });
}
var lineChart = function (c, ID, ChartData) {
    var plot = [];
    var xAxis = [];
    var seriesdata = ChartData.ChartDatas;
    var charttypes = ChartData.ChartDatas[0].type;
    //X軸非時間類型ChartData.Plot.type == 1
    if (ChartData.Plot.type == 1) {
        xAxis = {
            labels: {
                overflow: 'justify'
            },
            title: {
                text: ChartData.XAxis.Title
            }
        };
        plot = {
            series: {
                label: {
                    connectorAllowed: false
                },
                pointStart: ChartData.Plot.pointStartIntdata
            }
        }
    }
        //X軸是時間類型
    else if (ChartData.Plot.type == 0) {
        xAxis = {
            type: 'datetime',
            labels: {
                overflow: 'justify'
            },
            title: {
                text: ChartData.XAxis.Title
            }
        };
        //給定初始值類型ChartData.Plot.pointStartDateTime != "0001-01-01 00:00:00"
        if (ChartData.Plot.pointStartDateTime != "0001-01-01 00:00:00") {
            plot = {
                charttypes: {
                    dataLabels: {
                        enabled: true          // 开启数据标签
                    },
                    lineWidth: 4,
                    states: {
                        hover: {
                            lineWidth: 5
                        }
                    },
                    marker: {
                        enabled: false
                    },
                    pointInterval: ChartData.Plot.pointInterval,
                    pointStart: new Date(ChartData.Plot.pointStartDateTime).getTime()
                }
            }
        }
            //鍵值類型;
        else {
            plot = {
                charttypes: {
                    dataLabels: {
                        enabled: true          // 开启数据标签
                    },
                    lineWidth: 4,
                    states: {
                        hover: {
                            lineWidth: 5
                        }
                    },
                    marker: {
                        enabled: false
                    }
                }
            }
            var datainfo = [];
            for (var i = 0; i < ChartData.ChartDatas[0].data.length; i++) {
                var dataitem = [new Date(ChartData.ChartDatas[0].data[i][0]).getTime(), ChartData.ChartDatas[0].data[i][1]];
                datainfo.push(dataitem);
            }
            seriesdata[0].data = datainfo;
        }
    };

    c.highcharts({
        chart: {
            backgroundColor: 'rgba(0,0,0,0)'
        },
        title: {
            text: ChartData.Title
        },
        xAxis: xAxis,
        yAxis: {
            title: {
                text: ChartData.YAxis.Title
            }
        },
        tooltip: {
            //valueSuffix: ChartData.Tooltip
            headerFormat: '<b>{series.name}</b><br>',
            pointFormat: '{point.x:%e. %b}: {point.y:.2f} m'
        },
        credits: {
            enabled: false // 禁用版权信息
        },
        plotOptions: plot,
        subtitle: {
            text: ChartData.SubTitle
        },
        series: seriesdata
    });
}
var columnChart = function (c, ID, ChartData) {
    c.highcharts({
        chart: {
            backgroundColor: 'rgba(0,0,0,0)'
        },
        title: {
            text: ChartData.Title
        },
        xAxis: {
            labels: {
                overflow: 'justify'
            },
            title: {
                text: ChartData.XAxis.Title
            }
        },
        yAxis: {
            title: {
                text: ChartData.YAxis.Title
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}' + ChartData.Tooltip + '</b> of total<br/>'
        },
        credits: {
            enabled: false // 禁用版权信息
        },
        plotOptions: {
            series: {
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                    format: '{point.y:.1f}'
                }
            }
        },
        subtitle: {
            text: ChartData.SubTitle
        },
        series: ChartData.ChartDatas
    });
}


var OutputElements = function () {
    OutputElements.prototype.constructor = OutputElements;
    OutputElements.prototype.Chart = function (c, ID, ChartData, Scale) {
        var scales = Scale.split(':');
        var d = $("<div class=\"form-group panel panel-default\" view-group=\"" + ID + "\"></div>");
        //var l = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + ID + ":" + "</label>");
        var ctb_title = $("<div class=\"panel-heading\"><h3 class=\"panel-title\"><b>" + ID + "</b></h3></div>");
        var ctb_body = $("<div class=\"panel-body\"></div>");
        var p = $("<u id=\"" + ID + "\" class=\"form-control-static\"></u>");
        ctb_body.append(p);
        d.append(ctb_title, ctb_body);
        c.append(d);
        switch (ChartData.Type) {
            case "pieChart":
                pieChart(ctb_body, ID, ChartData);
                break;
            case "lineChart":
                lineChart(ctb_body, ID, ChartData);
                break;
            case "columnChart":
                columnChart(ctb_body, ID, ChartData);
                break;
            default: break;
        }
    };
    OutputElements.prototype.Alart = function (c, Msg , ObjType) {
        swal({
            title: "",
            text: Msg,
            type: ObjType
            }, function () { $(".SearchBox").toggle(); }
         );
    };
    OutputElements.prototype.Table = function (c, ID, RowsData, ColData) {
        var ctb = $("<div class=\"panel panel-default\"></div>");
        var ctb_title = $("<div class=\"panel-heading\"><h3 class=\"panel-title\"><b>查询结果 :</b></h3></div>");
        var ctb_body = $("<div class=\"panel-body\"></div>");
        var tb = $("<table id=\"" + ID + "\" view-group=\"" + ID + "\" class=\"table table-condensed\"  data-search=\"true\" data-show-toggle=\"true\" data-show-columns=\"true\" ></table>");
        ctb_body.append(tb);
        ctb.append(ctb_title);
        ctb.append(ctb_body);
        c.append(ctb);
        var col = [];
        if (RowsData.length > 0) {
            var row = RowsData[0];
            for (var item in row) {
                if (ColData.Contain(item)) {
                    var cell = {
                        field: item,
                        title: item,
                        align: 'center',
                        valign: 'middle',
                        sortable: false,
                        formatter: function (value, row, index) {
                            var res = "";
                            switch (value.LinkType) {
                                case "Report":
                                    res = '<a class="J_menuItem" data-index="' + value.Value + '" href="javascript:;" url="/FunctionPage/Report/Report.html?ClassName=' + value.LinkData + '&Data=' + value.Value + '&RunFlag=1">' + value.Value + '</a>';
                                    break;
                                case "Link":
                                    res = '<a class="J_menuItem" data-index="' + value.Value + '" href="javascript:;" url="' + value.LinkData + '">' + value.Value + '</a>';
                                    break;
                                default:
                                    res = value.Value;
                            }
                            return res;
                        },
                        CalcRowspan: function (value, row, index) {
                            return 'rowspan="' + value.RowSpan + '"';
                        },
                        CalcColspan: function (value, row, index) {
                            return 'colspan="' + value.ColSpan + '"';
                        },
                        cellStyle: function (value, row, index, field) {
                            if (value.RowSpan == 0 || value.ColSpan == 0) {
                                return {
                                    css: {
                                        display: "none"
                                    }
                                }
                            } else {
                                return {
                                    css: {
                                        backgroundColor: "",
                                        color: ""
                                    }
                                };
                            }
                        }
                    };
                    col.push(cell);
                }
            };
        };
        tb.bootstrapTable('destroy').bootstrapTable({
            pagination: true,
            striped: true,
            cache: false,
            columns: col,
            data: RowsData,
            pageSize: 10,
            pageList: [10, 25, 50, 100],
            sidePagination: "client",
            onPageChange: function (number, size) {
                c.find(".J_menuItem").on("click", OnLinkClick);
            },
            onToggle: function (cardView) {
                c.find(".J_menuItem").on("click", OnLinkClick);
            },
            showExport: true,
            exportDataType: "all"
        });
        //tb.bootstrapTable('destroy').bootstrapTable({
        //    columns: col,
        //    data: RowsData
        //});
        //tb.find("thead").find("tr").addClass("intro");
        //tb.find("tbody").find("tr").addClass("introa");
        ////tb.find("th").addClass("introa");
        //tb.find("td").addClass("introa");
        //tb.addClass("table table-condensed");
    };
   
};
var OnLinkClick = function (e) {
    // 获取标识数据
    var dataUrl = $(this).attr('url'),
        dataIndex = $(this).data('index'),
        menuName = $.trim($(this).text() + "_Report"),
        flag = true;
    if (dataUrl == undefined || $.trim(dataUrl).length == 0) return false;

    // 选项卡菜单已存在
    $(self.parent.document).find(".J_menuTab").each(function () {
        if ($(this).data('id') == dataUrl) {
            if (!$(this).hasClass('active')) {
                $(this).addClass('active').siblings('.J_menuTab').removeClass('active');
                self.parent.window.scrollToTab(this);
                // 显示tab对应的内容区
                $(self.parent.document).find('.J_mainContent .J_iframe').each(function () {
                    if ($(this).data('id') == dataUrl) {
                        $(this).show().siblings('.J_iframe').hide();
                        return false;
                    }
                });
            }
            flag = false;
            return false;
        }
    });

    // 选项卡菜单不存在
    if (flag) {
        $(self.parent.document).find('.J_iframe').hide();
        var str = '<a href="javascript:;" class="active J_menuTab" data-id="' + dataUrl + '">' + menuName + ' <i class="fa fa-times-circle"></i></a>';
        $(self.parent.document).find('.J_menuTab').removeClass('active');
        // 添加选项卡对应的iframe
        var str1 = '<iframe class="J_iframe" name="iframe' + dataIndex + '" width="100%" height="100%" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
        $(self.parent.document).find('.J_mainContent').find('iframe.J_iframe').hide().parents('.J_mainContent').append(str1);
        // 添加选项卡
        $(self.parent.document).find('.J_menuTabs .page-tabs-content').append(str);
        self.parent.window.scrollToTab($(self.parent.document).find('.J_menuTab.active'));
    }
    return false;
};



//Cookie-function-start
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
};

function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}

function setCookie(name, value, time) {
    var strsec = getsec(time);
    var exp = new Date();
    exp.setTime(exp.getTime() + strsec * 1);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
};

function getsec(str) {
    var str1 = str.substring(1, str.length) * 1;
    var str2 = str.substring(0, 1);
    if (str2 == "s") {
        return str1 * 1000;
    }
    else if (str2 == "h") {
        return str1 * 60 * 60 * 1000;
    }
    else if (str2 == "d") {
        return str1 * 24 * 60 * 60 * 1000;
    }
};
//Cookie-function-end
var themes = {
    //"default": "../../css/plugins/bootstrapTable/bootstrap-table.min.css",
    //"cerulean": "../../css/plugins/bootswatch/bootstrap.min.cerulean.css",
    "cosmo": "../../css/plugins/bootswatch/bootstrap.min.cosmo.css",
    "cyborg": "../../css/plugins/bootswatch/bootstrap.min.cyborg.css",
    "Darkly": "../../css/plugins/bootswatch/bootstrap.min.Darkly.css",
    "Journal": "../../css/plugins/bootswatch/bootstrap.min.Journal.css",
    //"Litera": "../../css/plugins/bootswatch/bootstrap.min.Litera.css",
    "Lumen": "../../css/plugins/bootswatch/bootstrap.min.Lumen.css",
    "LUX": "../../css/plugins/bootswatch/bootstrap.min.LUX.css",
    //"Minty": "../../css/plugins/bootswatch/bootstrap.min.Minty.css",
    "Pulse": "../../css/plugins/bootswatch/bootstrap.min.Pulse.css",
    //"Sandsto": "../../css/plugins/bootswatch/bootstrap.min.Sandstone.css",
    "Simplex": "../../css/plugins/bootswatch/bootstrap.min.Simplex.css",
    "Sketchy": "../../css/plugins/bootswatch/bootstrap.min.Sketchy.css",
    "Slate": "../../css/plugins/bootswatch/bootstrap.min.Slate.css",
    //"Solar": "../../css/plugins/bootswatch/bootstrap.min.Solar.css",
    //"Spacela": "../../css/plugins/bootswatch/bootstrap.min.Spacelab.css",
    //"Superhe": "../../css/plugins/bootswatch/bootstrap.min.Superhero.css",
    "United": "../../css/plugins/bootswatch/bootstrap.min.United.css",
    "Yeti": "../../css/plugins/bootswatch/bootstrap.min.Yeti.css",
    "Flatly": "../../css/plugins/bootswatch/bootstrap.min.Flatly.css"
};
var themesheet = $('<link href="' + themes['default'] + '" rel="stylesheet" />');

function initStyleSelect() {
    var styleValues = getCookie("messtylename");
    var c = $("#StyleSelect");
    var s1 = $("<button type=\"button\" class=\"btn btn-default\" id=\"StyleLabel\">Style</button>");
    var s2 = $(" <button type=\"button\" class=\"btn btn-default dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"> <span class=\"sr-only\">Toggle Dropdown</span></button>");
    c.append(s1, s2);
    var o = $(" <ul id=\"ss\" class=\"dropdown-menu badge-primary selected-row\">  </ul>");
    c.append(o);
    var sbtn = $("#ss");
    for (x in themes) {
        var l = $(" <li><a href=\"#\" data-theme=\"" + x + "\" class=\"theme-link\">" + x + "</a> </li>");
        sbtn.append(l);
    }
    themesheet.appendTo('head');
    $('.theme-link').click(function () {
        StyleChange($(this).attr('data-theme'));
        //var themeurl = themes[$(this).attr('data-theme')];
        //themesheet.attr('href', themeurl);
    });
    if (styleValues != null)
        StyleChange(styleValues);
    else
        StyleChange("Flatly");
}

function StyleChange(obj) {
    var themeurl = themes[obj];
    themesheet.attr('href', themeurl);
    var stylevalue = getCookie("messtylename");
    if (stylevalue != obj)
        setCookie("messtylename", obj, "d30");
    var stylelabel = $("#StyleLabel");
    stylelabel[0].innerHTML = obj;
    //switch (obj) {
    //case "锐利":
    //    var ca = $("#" + obj);
    //    removeTheme(obj);
    //    //addTheme("../../css/testcss/TableStyleTest_TableA.css", "锐利");
    //    addTheme("../../css/plugins/bootswatch/bootstrap.min.Darkly.css", "锐利");
    //    break;
    //case "柔和":
    //    removeTheme(obj);
    //    //addTheme("../../css/testcss/TableStyleTest_TableB.css", "柔和");
    //    addTheme("../../css/plugins/bootswatch/bootstrap.min.cosmo.css", "锐利");
    //    break;
    //default: break;
    //}
}

// 添加主题 --换另种方法
function addTheme(cssurl, themeid) {
    var link = document.createElement('link');
    link.type = 'text/css';
    link.id = themeid;  // 加上id方便后面好查找到进行删除
    link.rel = 'stylesheet';
    link.href = cssurl;
    document.getElementsByTagName("head")[0].appendChild(link);
    //save;
    var stylevalue = getCookie("messtylename");
    if (stylevalue != themeid)
        setCookie("messtylename", themeid, "d30");
}
// 删主题--换另种方法
function removeTheme(themeid) {
    $('#' + themeid).remove();
}