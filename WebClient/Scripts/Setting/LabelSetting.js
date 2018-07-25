var KeyPartData = [];
var Skuno = "";
var ListName = "";
var DataList = [];
var filename = "";
var UseType = "";
var Bas64File = null;
SubTable = {};
$(document).ready(function () {
    var reset = function () {
        Bas64File = null;
        $("#LabelFileUpload").val("");
    };
    var Get_data = function () {
        parent.client.CallFunction("MESStation.FileUpdate.Fileuplaod", "GetLabelList", {}, function (e) {
            if (e.Status == "Pass") {
                KeyPartData = e.Data;
                $('#Label_List_Table').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                    $("#LabelFileUpload").val("");
                });
            }
        });
    }

    var InitKPItem = function (index, row, $detail) {
        var cur_table = $detail.html('<table></table>').find('table');
        $(cur_table).bootstrapTable({
            pagination: false,
            data: [],
            columns: [{
                field: 'KPPartNo',
                title: 'KPPartNo'
            }, {
                field: 'KPName',
                title: 'KPName'
            }, {
                field: 'QTY',
                title: 'QTY'
            }, {
                field: 'SEQ',
                title: 'SEQ'
            }, {
                field: 'Station',
                title: 'Station'
            }, {
                field: 'Detail',
                title: 'Scan Detail',
                formatter: function (value, row, index) {
                    if (value.length > 0) {
                        var e = "<ol>";
                        for (var i = 0; i < value.length; i++) {
                            e += "<li>" + value[i].SCANTYPE + "</li>";
                        }
                        e += "</ol>";
                        return e;
                    }
                    return "";
                }
            }]
        });
        var MessageID = "TABLE" + parseInt(Math.random() * 99).toString() + Date.now().toString()
        SubTable[MessageID] = cur_table;
        parent.client.CallFunction("MESStation.KeyPart.KPScan", "GetKPListByListName", { ListName: row.LISTNAME }, function (e) {
            if (e.Status == "Pass") {
                var stb = SubTable[MessageID];
                delete SubTable[MessageID];
                $(stb).bootstrapTable('load', e.Data.Item);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {

                });
            }
        });
    };
    
    var ShowNewLabel = {
        id: "ShowNewLabel",
        type: 1,
        shade: 0.8,
        shadeClose: false,
        title: "New Label",
        area: ['600px', '400px'],
        content: $("#NewLabelPanel"),
        btn: ["Upload", "cancel"],
        success: function (layero, index) {
            layer.closeAll('loading');
            $("#NewLabelPanel").removeClass("hidden");
        },
        end: function () {
            $("#NewLabelPanel").addClass("hidden");
        },
        yes: function (index) {
            if ($("#txt_LabelName").val() == "") {
                layer.msg("Label Name Can't be null", {
                    icon: 2,
                    time: 2000
                }, function () {
                    $("#txt_LabelName").focus();
                });
                return false;
            }
            if ($("#txt_ArryLength").val() == "") {
                layer.msg("Label Max Length Can't be null", {
                    icon: 2,
                    time: 2000
                }, function () {
                    $("#txt_ArryLength").focus();
                });
                return false;
            } else {
                var reg = new RegExp("^[0-9]*[1-9][0-9]*$");
                if (!reg.test($("#txt_ArryLength").val())) {
                    layer.msg("Label Max Length is an invalid number", {
                        icon: 2,
                        time: 2000
                    }, function () {
                        $("#txt_ArryLength").focus();
                    });
                    return false;
                }
            }
            layer.load(3);
            parent.client.CallFunction("MESStation.FileUpdate.Fileuplaod", "UpLoadLabelFile", { Name: $("#txt_LabelName").val(), FileName: filename, MD5: "", UseType: "LABEL", LabelName: $("#txt_LabelName").val(), PrintType: $("#txt_PrintType").val(), ArryLength: $("#txt_ArryLength").val(), Bas64File: Bas64File }, function (e) {
                layer.closeAll('loading');
                if (e.Status == "Pass") {
                    layer.close(index);
                    layer.msg("Success", {
                        icon: 1,
                        time: 3000
                    }, function () {
                        $("#LabelFileUpload").val("");
                        Get_data();
                    });
                } else {
                    layer.msg(e.Message, {
                        icon: 2,
                        time: 3000
                    }, function () {
                    });
                }
            });
        },
        cancel: function (index) {
            $("#LabelFileUpload").val("");
            layer.close(index);
        }
    };

    $('#DeleteLabel').on('click', function () {
        var selRows = $('#Label_List_Table').bootstrapTable('getAllSelections');
        if (selRows.length > 0) {
            layer.open({
                title: 'Warning',
                btn: ['Delete', 'Cancel'],
                content: "Delete operation cannot be rolled back! </br> Are you sure you want to delete these records?",
                yes: function (index) {
                    layer.close(index);
                    var Names = [];
                    for (var i = 0; i < selRows.length; i++) {
                        Names.push(selRows[i].LISTNAME);
                    }
                    parent.client.CallFunction("MESStation.KeyPart.KPScan", "RemoveKPList", { ListNames: Names }, function (e) {
                        if (e.Status == "Pass") {
                            layer.msg("Success", {
                                icon: 1,
                                time: 3000
                            }, function () {
                                Get_data();
                            });
                        }
                        else {
                            layer.msg(e.Message, {
                                icon: 2,
                                time: 3000
                            }, function () {
                            });
                        }
                    });
                }
            });
        } else {
            layer.msg("no records selected", {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });

    $('#LabelFileUpload').change(function (e) {
        var w = new Worker("../../Scripts/Setting/BigFileReader.js");
        w.onmessage = function (e) {
            layer.closeAll("loading");
            if (e.data.Status == "Pass") {
                Bas64File = e.data.Bas64File;
                layer.open(ShowNewLabel);
            } else {
                $('#LabelFileUpload').val("");
                layer.msg(e.data.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        };
        w.onerror = function () {
            $('#LabelFileUpload').val("");
            layer.msg("Worker Error!", {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
        layer.load(3);
        filename = $(this).val();
        filename = filename.substring(filename.lastIndexOf("\\") + 1);
        UseType = filename.substring(filename.lastIndexOf(".") + 1).toUpperCase();
        w.postMessage({ file: e.target.files[0], filename: filename, UseType: UseType, ExtName: ".LAB,.TXT,.XLS,.XLSX" });
    });


    $('#Label_List_Table').bootstrapTable({
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: true,
        showSelectTitle: true,
        maintainSelected: true,
        clickToSelect: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#Table_Toolbar", 
        onRefresh: function (param) {
            Get_data();
        },
        columns: [{
            field: 'LABELNAME',
            title: 'Label Name'
        }, {
            field: 'R_FILE_NAME',
            title: 'File Name'
        }, {
            field: 'PRINTTYPE',
            title: 'Use Printer Index'
        }, {
            field: 'ARRYLENGTH',
            title: 'Max Length'
        }, {
            field: 'EDIT_TIME',
            title: 'Edit Time'
        }, {
            field: 'EDIT_EMP',
            title: 'Edit By'
        }],
        data: KeyPartData
    });

    Get_data();
});