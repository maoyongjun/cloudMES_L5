using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
    class CSkuSampleConfig : MesAPIBase
    {
        protected APIInfo FAddCSkuSample = new APIInfo()
        {
            FunctionName = "AddCSkuSample",
            Description = "添加CSkuSample",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STATION_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AQL_TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCSkuSample = new APIInfo()
        {
            FunctionName = "DeleteCSkuSample",
            Description = "刪除CSkuSample",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectCSkuSample = new APIInfo()
        {
            FunctionName = "SelectCSkuSample",
            Description = "查詢CSkuSample",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STATION_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateCSkuSample = new APIInfo()
        {
            FunctionName = "UpdateCSkuSample",
            Description = "更新CSkuSample",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STATION_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AQL_TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };


        public CSkuSampleConfig()
        {
            this.Apis.Add(FAddCSkuSample.FunctionName, FAddCSkuSample);
            this.Apis.Add(FDeleteCSkuSample.FunctionName, FDeleteCSkuSample);
            this.Apis.Add(FSelectCSkuSample.FunctionName, FSelectCSkuSample);
            this.Apis.Add(FUpdateCSkuSample.FunctionName, FUpdateCSkuSample);
        }

        public void AddCSkuSample(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_SAMPLE SAMPLE = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SAMPLE = new T_C_SKU_SAMPLE(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SKU_SAMPLE r = (Row_C_SKU_SAMPLE)SAMPLE.NewRow();
                r.ID = SAMPLE.GetNewID(this.BU, sfcdb);
                r.SKUNO= (Data["SKUNO"].ToString()).Trim();
                r.STATION_NAME= (Data["STATION_NAME"].ToString()).Trim();
                r.AQL_TYPE = (Data["AQL_TYPE"].ToString()).Trim();
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "添加成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
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

        public void GetCAqltypeList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE AQLTYPE = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                AQLTYPE = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                List<string> list = AQLTYPE.GetAql(sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
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

        public void DeleteCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_SAMPLE SAMPLE = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SAMPLE = new T_C_SKU_SAMPLE(sfcdb, DB_TYPE_ENUM.Oracle);

                Row_C_SKU_SAMPLE r = (Row_C_SKU_SAMPLE)SAMPLE.GetObjByID((Data["ID"].ToString()).Trim(), sfcdb);
                string strRet = sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "刪除成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
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

        public void SelectCSkuSample(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_SAMPLE SAMPLE = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SAMPLE = new T_C_SKU_SAMPLE(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SKU_SAMPLE> list = SAMPLE.GetSample((Data["SKUNO"].ToString()).Trim(), (Data["STATION_NAME"].ToString()).Trim(),sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
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

        public void UpdateCSkuSample(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SKU_SAMPLE SAMPLE = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SAMPLE = new T_C_SKU_SAMPLE(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SKU_SAMPLE r = (Row_C_SKU_SAMPLE)SAMPLE.GetObjByID((Data["ID"].ToString()).Trim(), sfcdb);
                r.SKUNO = (Data["SKUNO"].ToString()).Trim();
                r.STATION_NAME = (Data["STATION_NAME"].ToString()).Trim();
                r.AQL_TYPE = (Data["AQL_TYPE"].ToString()).Trim();
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "修改成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
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


    }
}
