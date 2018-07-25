var MesStation = function (o) {
    this.Name = o.Name;
    this.StationName = o.StationName;
    this.InitClassName = o.InitClassName ? o.InitClassName : "MESStation.Stations.CallStation";
    this.InitFunctionName = o.InitFunctionName ? o.InitFunctionName : "InitStation";
    this.InputClassName = o.InputClassName ? o.InputClassName : "MESStation.Stations.CallStation";
    this.InputFunctionName = o.InputFunctionName ? o.InputFunctionName : "StationInput";
    this.Client = o.Client;
    this.UserInfo = o.Client.UserInfo;
    this.Line = o.Line;
    this.IScale = o.IScale;
    this.TContainer = o.TContainer;
    this.IContainer = o.IContainer;
    this.OScale = o.OScale;
    this.OContainer = o.OContainer;
    this.MContainer = o.MContainer;
    this.MessageShowType = o.MessageShowType;
    this.Inputs = [];
    this.Outputs = [];
    this.Message = [];
    this.FailInputs = [];
    this.ScanType = o.ScanType ? o.ScanType : "Pass";
    this.CurrentInputJson = null;
    this.StationJson = null;
    this.ScanKeypart = [];
    this.BeforeInit = o.BeforeInit;
    this.OnInit = o.Init;
    this.MesHelper = null;
    MesStation.prototype.InputIsRepeat = false;/*InputIsRepeat當NextInput為重複輸入是則為True，不是則為false，作為是否重新渲染所有輸入框的條件*/
    MesStation.prototype.constructor = MesStation;
    MesStation.prototype.MyName = function () {
        //这里循环查找window对象中的dog属性
        for (var name in this.global) {
            //判断是否为Dog类
            if (this.global[name] === this) {
                return name;
            }
        }
    };
    MesStation.prototype.StationList = {};
    MesStation.prototype.Init = function () {
        if (this.Line == "Line1") {
            var line = localStorage.getItem($.MES.CK_LINE_NAME);
            if (line == undefined || line == null || line == "") {
                self.parent.Loading(false);
                var htmlstr = "<select onchange=\"localStorage.setItem($.MES.CK_LINE_NAME,$(this).val());\"><option>Line1</option>";
                var linelist = JSON.parse(localStorage.getItem($.MES.CK_LINE_LIST));
                for (var i = 0; i < linelist.length; i++) {
                    htmlstr += "<option>" + linelist[i].LINE_NAME + "</option>";
                }
                htmlstr += "</select>";
                swal.Data = this;
                swal({
                    title: "Select Station Line",
                    text: htmlstr,
                    html: true,
                    showCancelButton: true,
                    closeOnConfirm: false,
                    animation: "slide-from-top"
                },
                function (isConfirm) {
                    if (isConfirm) {
                        line = localStorage.getItem($.MES.CK_LINE_NAME);
                        if (line == undefined || line == null || line == "") {
                            line = "Line1";
                            localStorage.setItem($.MES.CK_LINE_NAME, line);
                        }
                        swal.Data.Line = line;
                    }
                    else {
                        localStorage.setItem($.MES.CK_LINE_NAME, "");
                    }
                    swal.close();
                    if (swal.Data.BeforeInit != undefined) {
                        swal.Data.BeforeInit();
                    }
                    swal.Data.Init();
                });
            }
            else {
                if (this.BeforeInit != undefined) {
                    this.BeforeInit();
                }
                this.Line = line;
                MesStation.prototype.StationList = {};
                var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
                this.ListenStationData(MessageID);
                this.Client.CallFunction(this.InitClassName, this.InitFunctionName, { DisplayStationName: this.Name, Line: this.Line }, this.InitCallBack, MessageID);
            }
        }
        else {
            if (this.BeforeInit != undefined) {
                this.BeforeInit();
            }
            MesStation.prototype.StationList = {};
            var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            this.ListenStationData(MessageID);
            this.Client.CallFunction(this.InitClassName, this.InitFunctionName, { DisplayStationName: this.Name, Line: this.Line }, this.InitCallBack, MessageID);
        }
    };
    MesStation.prototype.FillLocalData = function () {
        for (var i = 0; i < this.Inputs.length; i++) {
            if (this.Inputs[i]) {
                var ck = this.Name + "_" + this.Inputs[i].Name;
                var v = $.cookie(ck);
                if (v) {
                    this.Inputs[i].value = v;
                }
            }
        }
    };
    MesStation.prototype.ListenStationData = function (k) {
        MesStation.prototype.StationList[k] = this;
    };
    MesStation.prototype.InitCallBack = function (d) {
        var station = MesStation.prototype.StationList[d.MessageID];
        delete MesStation.prototype.StationList[d.MessageID];
        if (d.Status == "Pass") {
            station.CurrentInputJson = null;
            station.StationJson = d.Data.Station;
            station.Inputs.splice(0, station.Inputs.length);
            station.Outputs.splice(0, station.Outputs.length);
            station.Message.splice(0, station.Message.length);
            station.FailInputs.splice(0, station.FailInputs.length);
            station.Name = d.Data.Station.DisplayName;
            station.StationName = d.Data.Station.StationName;
            if (d.Data.Station.FailStation) {
                for (var i = 0; i < d.Data.Station.FailStation.Inputs.length; i++) {
                    var fi = new StationInput(d.Data.Station.FailStation.Inputs[i]);
                    station.FailInputs.push(fi);
                }
                for (var i = 0; i < d.Data.Station.FailStation.StationMessages.length; i++) {
                    var m = new StationMessage(d.Data.Station.FailStation.StationMessages[i]);
                    station.Message.push(m);
                }
            }
            for (var i = 0; i < d.Data.Station.Inputs.length; i++) {
                if (i == 0) {
                    station.CurrentInputJson = d.Data.Station.Inputs[i];
                }
                var ip = new StationInput(d.Data.Station.Inputs[i]);
                station.Inputs.push(ip);
            }
            for (var i = 0; i < d.Data.Station.DisplayOutput.length; i++) {
                var op = new StationOutput(d.Data.Station.DisplayOutput[i]);
                station.Outputs.push(op);
            }
            for (var i = 0; i < d.Data.Station.StationMessages.length; i++) {
                var m = new StationMessage(d.Data.Station.StationMessages[i]);
                station.Message.push(m);
            }
            station.FillLocalData();/*加载本地保存的数据*/

            if (station.TContainer && d.Data.Station.FailStation) {
                station.TContainer.find("[name=ScanType]").unbind("change");
                station.TContainer.empty();
                var e = new InputElements(station.Client);
                e.Radio(station.TContainer, "ScanType", "ScanType", "", station.ScanType, ["Pass", "Fail"], "", "", "", station.IScale ? station.IScale : "3:9");
                station.TContainer.find("[name=ScanType]").bind("change", { Station: station }, function (e) {
                    e.data.Station.SetScanType($(this).val());
                });
            }
        }
        else {
            swal.Data = station;
            swal({
                title: "Station Init Fail",
                text: d.Message,
                type: "error"
            },
            function () {                
                swal.Data.Init();
            });
        }        
        if (station.IContainer) {
            station.ShowInputs(station.IContainer);
        }
        if (station.OContainer) {
            station.ShowOutputs(station.OContainer);
        }
        if (station.OnInit != undefined) {
            station.OnInit(d);
        }
        station.SetCurrentInput();
        try {
            self.parent.Loading(false);
        } catch (e) {
        }
        station.MesHelper = new Helper({ MContainer: station.MContainer, OnOpen: function (e) { } });
    };
    MesStation.prototype.SetLine = function (Line) {
        this.Line = Line;
        this.Init();
    };
    MesStation.prototype.SetInputValue = function (Name, value) {
        if (this.ScanType == "Pass") {
            for (var i = 0; i < this.Inputs.length; i++) {
                if (this.Inputs[i].DisplayName == Name) {
                    this.Inputs[i].Value = value;
                    this.Inputs[i].ClearValue();
                    this.Inputs[i].SetFocus();
                    this.StationJson.Inputs[i].Value = value;
                    this.CurrentInputJson = this.StationJson.Inputs[i];
                }
            }
        }
        else {
            for (var i = 0; i < this.FailInputs.length; i++) {
                if (this.FailInputs[i].DisplayName == Name) {
                    this.FailInputs[i].Value = value;
                    this.FailInputs[i].ClearValue();
                    this.FailInputs[i].SetFocus();
                    this.StationJson.FailStation.Inputs[i].Value = value;
                    this.CurrentInputJson = this.StationJson.FailStation.Inputs[i];
                }
            }
        }
    };
    MesStation.prototype.SetCurrentInput = function (DisplayName) {
        if (this.ScanType == "Pass") {
            for (var i = 0; i < this.Inputs.length; i++) {
                if (DisplayName) {
                    if (this.Inputs[i].DisplayName == DisplayName) {
                        this.Inputs[i].SetEnable();
                        this.Inputs[i].SetVisable();
                        this.Inputs[i].SetFocus();
                        break;
                    }
                }
                else {
                    if (this.Inputs[i].DisplayName == this.CurrentInputJson.DisplayName) {
                        this.Inputs[i].SetEnable();
                        this.Inputs[i].SetVisable();
                        this.Inputs[i].SetFocus();
                        break;
                    }
                }
            }
        }
        else {
            for (var i = 0; i < this.FailInputs.length; i++) {
                if (DisplayName) {
                    if (this.FailInputs[i].DisplayName == DisplayName) {
                        this.FailInputs[i].SetEnable();
                        this.FailInputs[i].SetVisable();
                        this.FailInputs[i].SetFocus();
                        break;
                    }
                }
                else {
                    if (this.FailInputs[i].DisplayName == this.CurrentInputJson.DisplayName) {
                        this.FailInputs[i].SetEnable();
                        this.FailInputs[i].SetVisable();
                        this.FailInputs[i].SetFocus();
                        break;
                    }
                }
            }
        }
    };
    MesStation.prototype.SetScanType = function (Type) {
        this.ScanType = Type ? Type : "Pass";
        if (this.ScanType == "Pass") {
            this.CurrentInputJson = this.StationJson.Inputs[0];
        }
        else if (this.StationJson.FailStation) {
            this.StationJson.FailStation.Line = this.Line;
            this.CurrentInputJson = this.StationJson.FailStation.Inputs[0];
        }
        this.IContainer.empty();
        this.ShowInputs(this.IContainer);
        this.SetCurrentInput();
    };
    MesStation.prototype.ShowInput = function (obj) {
        if (this.ScanType == "Pass") {
            for (var i = 0; i < this.Inputs.length; i++) {
                if (this.Inputs[i].DisplayName == obj.InputName) {
                    obj.Container.find("button").unbind("click");
                    obj.Container.find("input:radio").unbind("click");
                    obj.Container.find("input.form-control").unbind("keypress");
                    obj.Container.find("select.form-control").unbind("change");
                    this.Inputs[i].Remove();
                    this.Inputs[i].Show({ Client: this.Client, Container: obj.Container, Scale: (obj.Scale == undefined ? (this.IScale == undefined ? "3:9" : this.IScale) : obj.Scale) });
                    this.Inputs[i].SetEnable();
                    this.Inputs[i].SetVisable();
                    obj.Container.find("button").bind("click", { Station: this }, function (event) {
                        event.data.Station.SetInputValue(this.name, "");
                        event.data.Station.SendData();
                    });
                    obj.Container.find("input:radio").bind("click", { Station: this }, function (event) {
                        event.data.Station.SetInputValue(this.name, this.value);
                        event.data.Station.SendData();
                    });
                    obj.Container.find("input.form-control").bind("keypress", { Station: this }, function (event) {
                        if (event.keyCode == 13) {
                            event.data.Station.SetInputValue(this.name, this.value);
                            event.data.Station.SendData();
                        }
                    });
                    obj.Container.find("select.form-control").bind("change", { Station: this }, function (event) {
                        event.data.Station.SetInputValue(this.name, this.value);
                        event.data.Station.SendData();
                    });
                    break;
                }
            }
        }
        else {
            for (var i = 0; i < this.FailInputs.length; i++) {
                if (this.FailInputs[i].DisplayName == obj.InputName) {
                    obj.Container.find("button").unbind("click");
                    obj.Container.find("input:radio").unbind("click");
                    obj.Container.find("input.form-control").unbind("keypress");
                    obj.Container.find("select.form-control").unbind("change");
                    this.FailInputs[i].Remove();
                    this.FailInputs[i].Show({ Client: this.Client, Container: obj.Container, Scale: (obj.Scale == undefined ? (this.IScale == undefined ? "3:9" : this.IScale) : obj.Scale) });
                    this.FailInputs[i].SetEnable();
                    this.FailInputs[i].SetVisable();
                    obj.Container.find("button").bind("click", { Station: this }, function (event) {
                        event.data.Station.SetInputValue(this.name, "");
                        event.data.Station.SendData();
                    });
                    obj.Container.find("input:radio").bind("click", { Station: this }, function (event) {
                        event.data.Station.SetInputValue(this.name, this.value);
                        event.data.Station.SendData();
                    });
                    obj.Container.find("input.form-control").bind("keypress", { Station: this }, function (event) {
                        if (event.keyCode == 13) {
                            event.data.Station.SetInputValue(this.name, this.value);
                            event.data.Station.SendData();
                        }
                    });
                    obj.Container.find("select.form-control").bind("change", { Station: this }, function (event) {
                        event.data.Station.SetInputValue(this.name, this.value);
                        event.data.Station.SendData();
                    });
                    break;
                }
            }
        }
    };
    MesStation.prototype.ShowInputs = function (Container) {
        Container.find("button").unbind("click");
        Container.find("input:radio").unbind("click");
        Container.find("input.form-control").unbind("keypress");
        Container.find("select.form-control").unbind("change");

        for (var i = 0; i < this.Inputs.length; i++) {
            this.Inputs[i].Remove();
        }
        for (var i = 0; i < this.FailInputs.length; i++) {
            this.FailInputs[i].Remove();
        }
        if (this.ScanType == "Pass") {
            for (var i = 0; i < this.Inputs.length; i++) {
                this.Inputs[i].Remove();
                this.Inputs[i].Show({ Client: this.Client, Container: Container, Scale: (this.IScale == undefined ? "3:9" : this.IScale) });
                this.Inputs[i].SetEnable();
                this.Inputs[i].SetVisable();
            }
        }
        else {
            for (var i = 0; i < this.FailInputs.length; i++) {
                this.FailInputs[i].Remove();
                this.FailInputs[i].Show({ Client: this.Client, Container: Container, Scale: (this.IScale == undefined ? "3:9" : this.IScale) });
                this.FailInputs[i].SetEnable();
                this.FailInputs[i].SetVisable();
            }
        }
        Container.find("button").bind("click", { Station: this }, function (event) {
            event.data.Station.SetInputValue(this.name, "");
            event.data.Station.SendData();
            try {
                $.publishMoreTime(this.name, this.value);
            } catch (e) {

            }
        });
        Container.find("input:radio").bind("click", { Station: this }, function (event) {
            event.data.Station.SetInputValue(this.name, this.value);
            event.data.Station.SendData();
            try {
                $.publishMoreTime(this.name, this.value);
            } catch (e) {

            }
        });
        Container.find("input.form-control").bind("keypress", { Station: this }, function (event) {
            if (event.keyCode == 13) {
                event.data.Station.SetInputValue(this.name, this.value);
                event.data.Station.SendData();
                try {
                    $.publishMoreTime(this.name, this.value);
                } catch (e) {

                }
            }
        });
        Container.find("select.form-control").bind("change", { Station: this }, function (event) {
            event.data.Station.SetInputValue(this.name, this.value);
            event.data.Station.SendData();
            try {
                $.publishMoreTime(this.name, this.value);
            } catch (e) {

            }
        });
    };
    MesStation.prototype.ShowOutput = function (obj) {
        for (var i = 0; i < this.Outputs.length; i++) {
            if (this.Outputs[i].Name == obj.OutputName) {
                this.Outputs[i].Remove();
                this.Outputs[i].Show(obj.Container, (obj.Scale == undefined ? (this.OScale == undefined ? "3:9" : this.OScale) : obj.Scale));
                break;
            }
        }
    };
    MesStation.prototype.ShowOutputs = function (Container) {
        for (var i = 0; i < this.Outputs.length; i++) {
            this.Outputs[i].Remove();
            this.Outputs[i].Show(Container, (this.OScale == undefined ? "3:9" : this.OScale));
        }
    };
    MesStation.prototype.ShowMessage = function (Container) {
        Container = Container ? Container : this.MContainer;
        for (var i = 0; i < this.Message.length; i++) {
            if (this.MessageShowType) {
                if (Container) {
                    this.Message[i].Show("", Container);
                }
                this.Message[i].Show(this.MessageShowType, Container);
            }
            else {
                this.Message[i].Show(this.MessageShowType, Container);
            }
        }
    };
    MesStation.prototype.SendData = function () {
        var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        //for (var i = 0; i < this.Inputs.length; i++) {
        //    this.Inputs[i].SetEnable(false);
        //}
        this.ListenStationData(MessageID);
        this.Client.CallFunction(this.InputClassName, this.InputFunctionName, { Station: this.StationJson, Input: this.CurrentInputJson, ScanType: this.ScanType }, this.CallBack, MessageID);

    };
    MesStation.prototype.CallBack = function (d) {
        var station = MesStation.prototype.StationList[d.MessageID];
        delete MesStation.prototype.StationList[d.MessageID];
        if (d.Status == "Pass") {
            if (d.Data.NextInput) {
                if (station.CurrentInputJson.ID == d.Data.NextInput.ID) {
                    MesStation.prototype.InputIsRepeat = true;
                }
                else {
                    MesStation.prototype.InputIsRepeat = false;
                }
                station.CurrentInputJson = d.Data.NextInput;
            }
            else if (d.Data.NextInput == null) {
                MesStation.prototype.InputIsRepeat = true;
            }
            station.StationJson = d.Data.Station;
            station.Inputs.splice(0, station.Inputs.length);
            station.Outputs.splice(0, station.Outputs.length);
            station.Message.splice(0, station.Message.length);
            station.FailInputs.splice(0, station.FailInputs.length);
            station.Name = d.Data.Station.DisplayName;
            station.StationName = d.Data.Station.StationName;
            if (d.Data.Station.FailStation) {
                for (var i = 0; i < d.Data.Station.FailStation.Inputs.length; i++) {
                    var fi = new StationInput(d.Data.Station.FailStation.Inputs[i]);
                    station.FailInputs.push(fi);
                }
                for (var i = 0; i < d.Data.Station.FailStation.StationMessages.length; i++) {
                    var m = new StationMessage(d.Data.Station.FailStation.StationMessages[i]);
                    station.Message.push(m);
                }
            }
            for (var i = 0; i < d.Data.Station.Inputs.length; i++) {
                var ip = new StationInput(d.Data.Station.Inputs[i]);
                station.Inputs.push(ip);
            }
            for (var i = 0; i < d.Data.Station.DisplayOutput.length; i++) {
                var op = new StationOutput(d.Data.Station.DisplayOutput[i]);
                station.Outputs.push(op);
            }
            for (var i = 0; i < d.Data.Station.StationMessages.length; i++) {
                var m = new StationMessage(d.Data.Station.StationMessages[i]);
                station.Message.push(m);
            }
        }
        station.ScanKeypart = d.Data.Station.ScanKP;
        if (station.ScanKeypart.length > 0) {
            var ObjectKey = "SKEY" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            MesStation.prototype.StationList[ObjectKey] = station;
            var KPScaner = new StationKeyParts({
                Client: station.Client,
                ObjectKey: ObjectKey,
                SN: station.CurrentInputJson.Value,
                KeyName: station.CurrentInputJson.DisplayName,
                StationName: station.Name,
                WO: station.ScanKeypart[0].WO,
                Data: station.ScanKeypart[0].SN
            });
            KPScaner.Show();
        }
        if (station.TContainer && d.Data.Station.FailStation) {
            station.TContainer.find("[name=ScanType]").unbind("change");
            station.TContainer.empty();
            var e = new InputElements(station.Client);
            e.Radio(station.TContainer, "ScanType", "ScanType", "", station.ScanType, ["Pass", "Fail"], "", "", "", station.IScale ? station.IScale : "3:9");
            station.TContainer.find("[name=ScanType]").bind("change", { Station: station }, function (e) {
                e.data.Station.SetScanType($(this).val());
            });
        }
        if (station.IContainer && !MesStation.prototype.InputIsRepeat) {
            station.ShowInputs(station.IContainer);
        }
        if (station.OContainer) {
            station.ShowOutputs(station.OContainer);
        }
        for (var i = 0; i < d.Data.Station.LabelPrint.length; i++) {
            station.MesHelper.Print(d.Data.Station.LabelPrint[i], function (e) {});
        }
        station.ShowMessage(station.MContainer);
        if (station.OnInit != undefined) {
            station.OnInit(d);
        }
        station.SetCurrentInput();
    };
    this.Init();
};
var StationInput = function (obj) {
    this.ID = obj.ID;
    this.Name = obj.Name;
    this.Value = obj.Value;
    this.DataForUse = obj.DataForUse;
    this.DisplayName = obj.DisplayName;
    this.DisplayType = obj.DisplayType;
    this.DataSourceAPI = obj.DataSourceAPI == undefined ? "" : obj.DataSourceAPI;
    this.DataSourceAPIPara = obj.DataSourceAPIPara == undefined ? "" : obj.DataSourceAPIPara;
    this.RefershType = obj.RefershType == undefined ? "Default" : obj.RefershType;
    this.ScanFlag = obj.ScanFlag == 1 ? true : false;
    this.Enable = obj.Enable == undefined ? true : obj.Enable;
    this.Visable = obj.Visable == undefined ? true : obj.Visable;
    this.RememberLastInput = obj.RememberLastInput == undefined ? false : obj.RememberLastInput;
    this.MessageID = obj.MessageID;
    StationInput.prototype.constructor = StationInput;
    StationInput.prototype.Show = function (obj) {
        var E = new InputElements(obj.Client);
        var container = obj.Container;
        switch (this.DisplayType) {
            case "TXT":
                E.Text(container, this.Name + "_" + this.ID, this.DisplayName, this.Name, this.Value, this.RefershType, obj.Scale, this.ScanFlag);
                break;
            case "Button":
                E.Button(container, this.Name + "_" + this.ID, this.DisplayName, this.Value,obj.Scale);
                break;
            case "Select":
                E.Select(container, this.Name + "_" + this.ID, this.DisplayName, this.Name, this.Value, this.DataForUse, this.DataSourceAPI, this.DataSourceAPIPara, this.RefershType, obj.Scale);
                break;
            case "Checkbox":
                E.Checkbox(container, this.Name + "_" + this.ID, this.DisplayName, this.Name, this.Value, this.RefershType, obj.Scale);
                break;
            case "Radio":
                E.Radio(container, this.Name + "_" + this.ID, this.DisplayName, this.Name, this.Value, this.DataForUse, this.DataSourceAPI, this.DataSourceAPIPara, this.RefershType, obj.Scale);
                break;
            case "Autocomplete":
                E.Autocomplete(container, this.Name + "_" + this.ID, this.DisplayName, this.Name, this.Value, this.DataForUse, this.DataSourceAPI, this.DataSourceAPIPara, this.RefershType, obj.Scale);
                break;
            case "LocalChecker":
                E.LocalChecker(this.Name + "_" + this.ID, this.DisplayName, this.Name, this.Value, this.RefershType, this.MessageID);
                break;
            default:
                container.append("<span>DisplayType " + this.DisplayType + " undefined,input name " + this.Name + "</span>");
                break;
        }
    };
    StationInput.prototype.ClearValue = function () {
        var selector = "#" + this.Name + "_" + this.ID;
        var e = $(selector);
        e.val("");
    };
    StationInput.prototype.SetFocus = function () {
        var selector = "#" + this.Name + "_" + this.ID;
        var e = $(selector);
        if (!MesStation.prototype.InputIsRepeat) {
            e.select();
        }
        e.focus();
    };
    StationInput.prototype.SetEnable = function (flag) {
        var selector = "#" + this.Name + "_" + this.ID;
        var f = (flag == undefined ? this.Enable : flag);
        if (f) {
            $(selector).attr("disabled", false);
        }
        else {
            $(selector).attr("disabled", true);
        }
    };
    StationInput.prototype.SetVisable = function (flag) {
        var selector = "[view-group=" + this.Name + "_" + this.ID + "]";
        var f = (flag == undefined ? this.Visable : flag);
        if (f) {
            $(selector).show();
        }
        else {
            $(selector).hide();
        }
    };
    StationInput.prototype.Remove = function () {
        var selector = "[view-group=" + this.Name + "_" + this.ID + "]";
        $(selector).find("input.form-control,select.form-control").unbind("keypress");
        $(selector).remove();
    };
};
var StationOutput = function (obj) {
    this.Name = obj.Name;
    this.DisplayType = obj.DisplayType;
    this.Value = obj.Value;
    StationOutput.prototype.constructor = StationOutput;
    StationOutput.prototype.Show = function (c, s) {
        var E = new OutputElements();
        switch (this.DisplayType) {
            case "TXT":
                E.Text(c, this.Name, this.Value, s);
                break;
            case "Table":
                E.Table(c, this.Name, this.Value);
                break;
            default:
                c.append("<span>DisplayType " + this.DisplayType + " undefined,input name " + this.Name + "</span>");
                break;

        }
    };
    StationOutput.prototype.Remove = function () {
        var selector = "[view-group=" + this.Name + "]";
        if (this.DisplayType == "Table") {
            $("#" + this.ID).bootstrapTable("destroy");
        }
        $(selector).remove();
    };
};
var StationMessage = function (obj) {
    this.State = obj.State;
    this.Message = obj.Message;
    StationMessage.prototype.Show = function (Type, o) {
        var E = new MessageElements();
        if (this.State == 4 && !self.parent.$.MES.DEBUG) {
            return;
        }
        switch (Type) {
            case "Swal":
                E.Swal(this.Message, this.State, o);
                break;
            case "Toastr":
                E.Toastr(this.Message, this.State, o);
                break;
            default:                
                if (o) {
                    E.Default(this.Message, this.State, o);
                }
                break;
        }
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
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control\" placeholder=\"" + placeholder + "\" value=\"" + value + "\" " + (ScanFlag ? "data-scan=\"true\"" : "") + ">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
    InputElements.prototype.Button = function (c, ID, Label, value, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\"></label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var btn = $("<button class=\"btn btn-primary small\" id=\"" + ID + "\" name=\"" + Label + "\" value=\"" + value + "\">" + Label + "</button>");
        inputD.append(btn);
        div.append(label, inputD);
        c.append(div);
    };
    InputElements.prototype.Select = function (c, ID, Label, placeholder, value, DataForUse, API, APIPara, RefershType, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<select id=\"" + ID + "\" name=\"" + Label + "\" class=\"form-control\" placeholder=\"" + placeholder + "\" aria-describedby=\"basic-addon1\"></select>");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
        if (RefershType == "Default") {
            input.attr("PreValue", value);
            this.GetData(API, APIPara, ID, function (e) {
                if (e.Status == "Pass") {
                    var select = $("#" + e.MessageID);
                    select.empty();
                    var value = select.attr("PreValue");
                    for (var i = 0; i < e.Data.length; i++) {
                        var op = $(" <option value=\"" + e.Data[i] + "\" " + (value == e.Data[i] ? "selected" : "") + ">" + e.Data[i] + "</option>");
                        select.append(op);
                    }
                }
            });
        }
        else if (RefershType == "EveryTime") {
            var select = $("#" + ID);
            select.empty();
            for (var x = 0; x < DataForUse.length; x++) {
                var op = $(" <option value=\"" + DataForUse[x] + "\"" + (value == DataForUse[x] ? "selected" : "") + ">" + DataForUse[x] + "</option>");
                select.append(op);
            }
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
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + ":</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        for (var i = 0; i < DataForUse.length; i++) {
            var labelx = $("<label class=\"radio-inline\"></label>");
            var radio = $("<input type=\"radio\" name=\"" + Label + "\" id=\"" + ID + "_" + i + "\" value=\"" + DataForUse[i] + "\"" + (value == DataForUse[i] ? "checked" : "") + ">");
            labelx.append(radio, DataForUse[i]);
            inputD.append(labelx);
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
                            $(this).autocomplete("search");
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
var OutputElements = function () {
    OutputElements.prototype.constructor = OutputElements;
    OutputElements.prototype.Text = function (c, ID, value, Scale) {
        var scales = Scale.split(':');
        var d = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var l = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + ID + ":" + "</label>");
        var v = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var p = $("<u id=\"" + ID + "\" class=\"form-control-static\">" + value + "</u>");
        v.append(p);
        d.append(l, v);
        c.append(d);
    };
    OutputElements.prototype.Table = function (c, ID, value) {
        var tb = $("<table id=\"" + ID + "\" view-group=\"" + ID + "\"></table>");
        c.append(tb);
        var col = [];
        if (value.length > 0) {
            for (var item in value[0]) {
                var cell = {
                    field: item,
                    title: item,
                    align: 'center',
                    valign: 'middle',
                    sortable: false
                };
                col.push(cell);
            }
        }
        tb.bootstrapTable({
            pagination: false,
            striped: true,
            cache: false,
            columns: col,
            data: value
        });
    };
};
var MessageElements = function () {
    MessageElements.prototype.Toastr = function (Message, State, opt) {
        var type = State == 0 ? "error" : (State == 1 ? "success" : (State == 2 ? "info" : (State == 3 ? "warning" : "info")));
        toastr.options = {
            closeButton: opt.closeButton == undefined ? true : opt.closeButton,
            debug: opt.debug == undefined ? false : opt.debug,
            progressBar: opt.progressBar == undefined ? true : opt.progressBar,
            positionClass: opt.positionClass == undefined ? "toast-bottom-right" : opt.positionClass,
            onclick: opt.onclick ? opt.onclick : function () { },
            showDuration: opt.showDuration ? opt.showDuration : 400,
            hideDuration: opt.hideDuration ? opt.hideDuration : 1000,
            timeOut: opt.timeOut ? opt.timeOut : 5000,
            extendedTimeOut: opt.extendedTimeOut ? opt.extendedTimeOut : 6000,
            showEasing: "swing",
            hideEasing: "linear",
            showMethod: "fadeIn",
            hideMethod: "fadeOut"
        }
        toastr[type](Message);
    };
    MessageElements.prototype.Swal = function (Message, State, opt) {
        var type = State == 0 ? "error" : (State == 1 ? "success" : (State == 2 ? "info" : (State == 3 ? "warning" : "info")));
        swal({
            title: type,
            text: Message,
            type: type,
            timer: opt.timer
        });
    };
    MessageElements.prototype.Default = function (Message, State, Container) {
        var DTime = new Date();
        var time = DTime.format("yyyy-MM-dd hh:mm:ss");
        var type = State == 0 ? "error" : (State == 1 ? "success" : (State == 2 ? "info" : (State == 3 ? "warning" : "info")));
        if (Container.find) {
            if (Container.find("tbody").length > 0) {
                Container.find("tbody>tr:gt(200)").remove();
                var n = Container.find("tbody>tr:first>td:first").text();
                var tr = $("<tr class=\"" + type + "\"><td>" + (Number(n)+1).toString() + "</td><td>" + type + "</td><td>" + Message + "</td><td>" + time + "</td></tr>");
                Container.find("tbody").prepend(tr);
            }
            else {
                var table = $("<table class=\"table table-striped\"></table>");
                var th = $("<thead><tr><th>#</th><th>Type</th><th>Message</th><th>DataTime</th></tr></thead>");
                var tb = $("<tbody></tbody>");
                var tr = $("<tr class=\"" + type + "\"><td>1</td><td>" + type + "</td><td>" + Message + "</td><td>" + time + "</td></tr>");
                tb.append(tr);
                table.append(th);
                table.append(tb);
                Container.empty();
                Container.prepend(table);
            }
        }
    };
};
var StationKeyParts = function (obj) {
    this.ObjectKey = obj.ObjectKey;
    this.KeyName = obj.KeyName;
    this.SN = obj.SN;
    this.WO = obj.WO;
    this.Data = obj.Data;
    this.StationName = obj.StationName;
    StationKeyParts.prototype.constructor = StationKeyParts;
    StationKeyParts.prototype.List = {};
    StationKeyParts.prototype.MyName = function () {
        for (var name in this.global) {
            if (this.global[name] === this) {
                return name;
            }
        }
    };
    StationKeyParts.prototype.CallReScan = function () {
        var station = MesStation.prototype.StationList[this.ObjectKey];
        delete MesStation.prototype.StationList[this.ObjectKey];
        station.SetInputValue(this.KeyName, this.SN);
        station.SendData();
    };
    StationKeyParts.prototype.Show = function () {
        var ObjectKey = "OBJ" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        var URL = 'KeyPart.html?KeyName=' + this.KeyName + '&Data=' + this.Data + '&WO=' + this.WO + '&StationName=' + this.StationName + '&ObjectKey=' + ObjectKey;
        StationKeyParts.prototype.List = {};
        StationKeyParts.prototype.List[ObjectKey] = this;
        layer.open({
            id: this.Serialno,
            type: 2,
            title: "Key Part Scan",
            skin: "demo-class",
            closeBtn: 1,
            shadeClose: false,
            shade: 0.5,
            maxmin: true, //开启最大化最小化按钮
            area: ['90%', '90%'],
            content: [URL, 'no'], //iframe的url，no代表不显示滚动条
            cancel: function (index) {
                var Key = layer.getChildFrame("#ObjectKey", index).val();
                var isCancel = layer.getChildFrame("#Flag_IsCancel", index).val();
                if (isCancel == "1") {
                    if (confirm('确定要关闭么')) {
                        layer.close(index);
                    }
                    else {
                        return false;
                    }
                }
                else {
                    var obj = StationKeyParts.prototype.List[Key];
                    delete StationKeyParts.prototype.List[Key];
                    obj.CallReScan();
                }
            }
        });
    };
};