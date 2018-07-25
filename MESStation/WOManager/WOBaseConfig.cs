using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MESStation.WOManager
{
    public class WOBaseConfig : MESStation.BaseClass.MesAPIBase
    {
        public WOBaseConfig()
        {
            this.Apis.Add(FAddWO.FunctionName, FAddWO);
            this.Apis.Add(FMarkWO.FunctionName, FMarkWO);
            this.Apis.Add(FShowAllData.FunctionName, FShowAllData);
            this.Apis.Add(FModifyWO.FunctionName, FModifyWO);
            this.Apis.Add(FCheckFlag.FunctionName, FCheckFlag);
            this.Apis.Add(FGetWorkorderno.FunctionName, FGetWorkorderno);
            this.Apis.Add(FGetSkunoByWo.FunctionName, FGetSkunoByWo);
            this.Apis.Add(FGetQty.FunctionName, FGetQty);
            this.Apis.Add(FCheckToUpload.FunctionName, FCheckToUpload);
        }

        protected APIInfo FShowAllData = new APIInfo()
        {
            FunctionName = "ShowAllData",
            Description = "Show data of all WorkOrder",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        private APIInfo FGetWObyWONO = new APIInfo()
        {
            FunctionName = "GetWObyWONO",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo FCheckFlag = new APIInfo()
        {
            FunctionName = "CheckFlag",
            Description = "Check condition to delete",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>()
            { }

        };

        protected APIInfo FModifyWO = new APIInfo()
        {
            FunctionName = "ModifyWO",
            Description = "Edit data of WorkOrder selected",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PLANT", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RELEASE_DATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DOWNLOAD_DATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRODUCTION_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WO_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKU_VER", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKU_SERIES", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKU_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKU_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUST_PN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUST_PN_VER", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUSTOMER_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROUTE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "START_STATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "KP_LIST_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLOSED_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLOSE_DATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WORKORDER_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "INPUT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FINISHED_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SCRAPED_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STOCK_LOCATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PO_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUST_ORDER_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROHS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddWO = new APIInfo()
        {
            FunctionName = "AddWO",
            Description = "Add new WorkOrder, max_sn will be auto enter",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PLANT", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RELEASE_DATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DOWNLOAD_DATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRODUCTION_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WO_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKU_VER", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKU_SERIES", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKU_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKU_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUST_PN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUST_PN_VER", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUSTOMER_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROUTE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "START_STATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "KP_LIST_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLOSED_FLAG", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLOSE_DATE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WORKORDER_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "INPUT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FINISHED_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SCRAPED_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STOCK_LOCATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PO_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUST_ORDER_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ROHS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FMarkWO = new APIInfo()
        {
            FunctionName = "MarkWO",
            Description = "Mark WorkOrder what was pending",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetWorkorderno = new APIInfo()
        {
            FunctionName = "GetWoNo",
            Description = "SelectAllW/O",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetSkunoByWo = new APIInfo()
        {
            FunctionName = "GetSkunoByWo",
            Description = "SelectSkunoByWO",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetQty = new APIInfo()
        {
            FunctionName = "GetQty",
            Description = "SelectQTY",
            Parameters = new List<APIInputInfo>()
            {
            new APIInputInfo() { InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" },
            new APIInputInfo() { InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCheckToUpload = new APIInfo()
        {
            FunctionName = "CheckToUpload",
            Description = "Check exist to upload",
            Parameters = new List<APIInputInfo>()
            {
            new APIInputInfo() { InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" },
            new APIInputInfo() { InputName = "SKUNO", InputType = "string", DefaultValue = "" },
            new APIInputInfo() { InputName = "QTY", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public void ShowAllData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_R_WO_BASE rWo = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                List<R_WO_BASE> list = new List<R_WO_BASE>();
                list = rWo.ShowAllData(sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        public void CheckFlag(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_BASE sec = null;
            string WORKORDERNO = string.Empty;
            List<R_WO_BASE> list = new List<R_WO_BASE>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                sec = new T_R_WO_BASE(oleDB, DBTYPE);
                WORKORDERNO = Data["WO"].ToString().Trim();
                list = sec.CheckFlag(WORKORDERNO, oleDB);
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = exception.Message;
                StationReturn.Data = "";
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }
        public void AddWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

        }
        public void ModifyWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

        }
        public void GetWObyWONO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

        }
        public void MarkWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

        }
        public void GetWoNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_BASE sec = null;
            string WORKORDERNO = string.Empty;
            List<R_WO_BASE> list = new List<R_WO_BASE>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                sec = new T_R_WO_BASE(oleDB, DBTYPE);
                WORKORDERNO = Data["WORKORDERNO"].ToString().Trim();
                list = sec.GetAllWO(WORKORDERNO, oleDB);
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000043";
                    StationReturn.Data = new object();
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = exception.Message;
                StationReturn.Data = "";
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }
        public void GetSkunoByWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_BASE sec = null;
            string WORKORDERNO = string.Empty;
            string SKUNO = string.Empty;
            List<R_WO_BASE> list = new List<R_WO_BASE>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                sec = new T_R_WO_BASE(oleDB, DBTYPE);
                WORKORDERNO = Data["WORKORDERNO"].ToString().Trim();
                list = sec.GetSkunoByWO(WORKORDERNO, SKUNO, oleDB);
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = exception.Message;
                StationReturn.Data = "";
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }
        public void GetQty(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_BASE sec = null;
            string WORKORDERNO = string.Empty;
            string SKUNO = string.Empty;
            int QTY = 0;
            List<R_WO_BASE> list = new List<R_WO_BASE>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                sec = new T_R_WO_BASE(oleDB, DBTYPE);
                WORKORDERNO = Data["WORKORDERNO"].ToString().Trim();
                SKUNO = Data["SKUNO"].ToString().Trim();
                list = sec.GetQtyByWOSkuno(WORKORDERNO, SKUNO, QTY, oleDB);
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000043";
                    StationReturn.Data = new object();
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = exception.Message;
                StationReturn.Data = "";
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }

        public void CheckToUpload(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_BASE sec = null;
            string WORKORDERNO = string.Empty;
            string SKUNO = string.Empty;
            int QTY = 0;
            List<R_WO_BASE> list = new List<R_WO_BASE>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                sec = new T_R_WO_BASE(oleDB, DBTYPE);
                WORKORDERNO = Data["WORKORDERNO"].ToString().Trim();
                SKUNO = Data["SKUNO"].ToString().Trim();
                QTY = int.Parse(Data["QTY"].ToString());
                list = sec.GetQtyByWOSkuno(WORKORDERNO, SKUNO, QTY, oleDB);
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000043";
                    StationReturn.Data = new object();
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = exception.Message;
                StationReturn.Data = "";
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }

    }
}
//RuRun

