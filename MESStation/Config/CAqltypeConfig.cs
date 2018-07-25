using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
    class CAqltypeConfig : MesAPIBase
    {
        protected APIInfo FAddCAqltype = new APIInfo()
        {
            FunctionName = "AddCAqltype",
            Description = "添加CAqltype",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "AQL_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "GL_LEVEL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SAMPLE_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ACCEPT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REJECT_QTY", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCAqltype = new APIInfo()
        {
            FunctionName = "DeleteCAqltype",
            Description = "刪除CAqltype",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateCAqltype = new APIInfo()
        {
            FunctionName = "UpdateCAqltype",
            Description = "更新CAqltype",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "AQL_TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "GL_LEVEL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SAMPLE_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ACCEPT_QTY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REJECT_QTY", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectCAqltype = new APIInfo()
        {
            FunctionName = "SelectCAqltype",
            Description = "查询CAqltype",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "AQL_TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CAqltypeConfig()
        {
            this.Apis.Add(FAddCAqltype.FunctionName, FAddCAqltype);
            this.Apis.Add(FDeleteCAqltype.FunctionName, FDeleteCAqltype);
            this.Apis.Add(FUpdateCAqltype.FunctionName, FUpdateCAqltype);
            this.Apis.Add(FSelectCAqltype.FunctionName, FSelectCAqltype);
        }

        public void AddCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cAqultype = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cAqultype = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_AQLTYPE r = (Row_C_AQLTYPE)cAqultype.NewRow();
                r.ID = cAqultype.GetNewID(this.BU, sfcdb);
                r.AQL_TYPE = (Data["AQL_TYPE"].ToString()).Trim();
                r.LOT_QTY = Convert.ToDouble((Data["LOT_QTY"].ToString()).Trim());
                r.GL_LEVEL = (Data["GL_LEVEL"].ToString()).Trim();
                r.SAMPLE_QTY = Convert.ToDouble((Data["SAMPLE_QTY"].ToString()).Trim());
                r.ACCEPT_QTY = Convert.ToDouble((Data["ACCEPT_QTY"].ToString()).Trim());
                r.REJECT_QTY = Convert.ToDouble((Data["REJECT_QTY"].ToString()).Trim());
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

        public void DeleteCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cAqultype = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cAqultype = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);

                Row_C_AQLTYPE r = (Row_C_AQLTYPE)cAqultype.GetObjByID((Data["ID"].ToString()).Trim(), sfcdb);
                string strRet= sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
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

        public void UpdateCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cAqultype = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cAqultype = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_AQLTYPE r = (Row_C_AQLTYPE)cAqultype.GetObjByID((Data["ID"].ToString()).Trim(), sfcdb);
                r.AQL_TYPE = (Data["AQL_TYPE"].ToString()).Trim();
                r.LOT_QTY = Convert.ToDouble((Data["LOT_QTY"].ToString()).Trim());
                r.GL_LEVEL = (Data["GL_LEVEL"].ToString()).Trim();
                r.SAMPLE_QTY = Convert.ToDouble((Data["SAMPLE_QTY"].ToString()).Trim());
                r.ACCEPT_QTY = Convert.ToDouble((Data["ACCEPT_QTY"].ToString()).Trim());
                r.REJECT_QTY = Convert.ToDouble((Data["REJECT_QTY"].ToString()).Trim());
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "更新成功！！";
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

        public void SelectCAqltype(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_AQLTYPE cSkuDetail = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                cSkuDetail = new T_C_AQLTYPE(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_AQLTYPE> list = cSkuDetail.GetAqlBySkuno((Data["AQL_TYPE"].ToString()).Trim(), sfcdb);

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
    }
}
