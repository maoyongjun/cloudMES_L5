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

namespace MESStation.Config
{
    public class SNRangeConfig : MESStation.BaseClass.MesAPIBase
    {
        public SNRangeConfig()
        {
            this.Apis.Add(FAddWO.FunctionName, FAddWO);
            this.Apis.Add(FMarkWO.FunctionName, FMarkWO);
            this.Apis.Add(FShowAllData.FunctionName, FShowAllData);
            //this.Apis.Add(MarkColumnName.FunctionName, MarkColumnName);
            this.Apis.Add(FModifyWO.FunctionName, FModifyWO);
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

        protected APIInfo FModifyWO = new APIInfo()
        {
            FunctionName = "ModifyWO",
            Description = "Edit data of WorkOrder selected",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MIN_SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAX_SN", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddWO = new APIInfo()
        {
            FunctionName = "AddWO",
            Description = "Add new WorkOrder, max_sn will be auto enter",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "QTY", InputType = "int", DefaultValue = "" },
                new APIInputInfo() {InputName = "MIN_SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAX_SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        //protected APIInfo CalculatorMax = new APIInfo()
        //{
        //    FunctionName = "FCalculatorMax",
        //    Description = "Calculator MaxSN , apply with hexa too",
        //    Parameters = new List<APIInputInfo>()
        //    {
        //        new APIInputInfo() {InputName = "QTY", InputType = "int", DefaultValue = "" },
        //        new APIInputInfo() {InputName = "MIN_SYSSERIALNO", InputType = "string", DefaultValue = "" },
        //        new APIInputInfo() {InputName = "STRING", InputType = "int", DefaultValue = "Input string which appear in SN, should by CAPSLOCK" },

        //    },
        //    Permissions = new List<MESPermission>() { }
        //};

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

        public void ModifyWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_REGION rWo = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rWo = new T_R_WO_REGION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_WO_REGION r = (Row_R_WO_REGION)rWo.NewRow();
                r = (Row_R_WO_REGION)rWo.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                r.WORKORDERNO = (Data["WO"].ToString()).Trim();
                r.SKUNO = (Data["SKUNO"].ToString()).Trim();
                r.QTY = int.Parse(Data["QTY"].ToString());
                r.MIN_SYSSERIALNO = (Data["MIN_SN"].ToString()).Trim();
                //r.MAX_SYSSERIALNO = (Data["MAX_SYSSERIALNO" + "QTY"].ToString()).Trim();
                r.MAX_SYSSERIALNO = (Data["MAX_SN"].ToString()).Trim();
                r.EDIT_EMP = LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.MessageCode = "MES00000003";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "UpdateNoData";
                    StationReturn.Status = StationReturnStatusValue.Fail;
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
        public void AddWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_WO_REGION wo = new T_R_WO_REGION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_WO_REGION r = (Row_R_WO_REGION)wo.NewRow();
                r.ID = wo.GetNewID(BU, sfcdb, DBTYPE);
                r.WORKORDERNO = (Data["WO"].ToString()).Trim();
                r.SKUNO = (Data["SKUNO"].ToString()).Trim();
                r.QTY = int.Parse(Data["QTY"].ToString());
                r.MIN_SYSSERIALNO = (Data["MIN_SN"].ToString()).Trim();
                //r.MAX_SYSSERIALNO = r.MIN_SYSSERIALNO;
                r.MAX_SYSSERIALNO = (Data["MAX_SN"].ToString()).Trim();
                r.EDIT_EMP = LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = strRet;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "UpdateNoData";
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
        public void ShowAllData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_R_WO_REGION rWo = new T_R_WO_REGION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<R_WO_REGION> list = new List<R_WO_REGION>();
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
                    StationReturn.MessageCode = "QueryNoData";
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
        public void GetWObyWONO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<R_WO_REGION> list = new List<R_WO_REGION>();
            T_R_WO_REGION rwo;
            string WO = Data["WORKORDERNO"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rwo = new T_R_WO_REGION(sfcdb, DBTYPE);
                list = rwo.GetWObyWONO(WO, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES000000016";
                StationReturn.Data = list;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
        public void MarkWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_WO_REGION wo = new T_R_WO_REGION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_WO_REGION r = (Row_R_WO_REGION)wo.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                string strRet = sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000004";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "NotLatestData";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        //public void CalculatorMax(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    T_R_WO_REGION rWo = null;
        //    OleExec sfcdb = null;
        //    try
        //    {

        //    }
        //    catch
        //    {

        //    }
        //}

    }
}


