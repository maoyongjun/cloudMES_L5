using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;

namespace MESStation.Config
{
    public class SkuKeyPartConfig : MesAPIBase
    {

        protected APIInfo FQueryMpnBySku = new APIInfo()
        {
            FunctionName = "QueryMpnBySku",
            Description = "Query Mpn By Sku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "AddReplaceKpWithSku",
            Description = "AddReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REPLACEPARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
            {
            }
        };

        protected APIInfo FUpdateReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "UpdateReplaceKpWithSku",
            Description = "UpdateReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REPLACEPARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>(){}
        };

        protected APIInfo FQueryReplaceKpBySku = new APIInfo()
        {
            FunctionName = "QueryReplaceKpBySku",
            Description = "QueryReplaceKpBySku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddMpnWithSku = new APIInfo()
        {
            FunctionName = "AddMpnWithSku",
            Description = "Query Mpn With Sku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MPN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteMpnWithSku = new APIInfo()
        {
            FunctionName = "DeleteMpnWithSku",
            Description = "DeleteMpnWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CSKUMPNIDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateMpnBySku = new APIInfo()
        {
            FunctionName = "UpdateMpnWithSku",
            Description = "Update Mpn With Sku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MPN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSetWoReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "SetWoReplaceKpWithSku",
            Description = "SetWoReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetWoReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "GetWoReplaceKpWithSku",
            Description = "GetWoReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "REPLACEPARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteWoReplaceKpWithSku = new APIInfo()
        {
            FunctionName = "DeleteWoReplaceKpWithSku",
            Description = "DeleteWoReplaceKpWithSku",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "RWOKPREPLACEIDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddCKpCheck = new APIInfo()
        {
            FunctionName = "AddCKpCheck",
            Description = "AddCKpCheck",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TYPENAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DLL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLASS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FUNCTION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FQueryCKpCheck = new APIInfo()
        {
            FunctionName = "QueryCKpCheck",
            Description = "QueryCKpCheck",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TYPENAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateCKpCheck = new APIInfo()
        {
            FunctionName = "UpdateCKpCheck",
            Description = "UpdateCKpCheck",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPENAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DLL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLASS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FUNCTION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCKpCheck = new APIInfo()
        {
            FunctionName = "DeleteCKpCheck",
            Description = "DeleteCKpCheck",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "IDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FQueryCKpRule = new APIInfo()
        {
            FunctionName = "QueryCKpRule",
            Description = "QueryCKpRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddCKpRule = new APIInfo()
        {
            FunctionName = "AddCKpRule",
            Description = "AddCKpRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MPN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SCANTYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REGEX", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FModifyCKpRule = new APIInfo()
        {
            FunctionName = "ModifyCKpRule",
            Description = "ModifyCKpRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REGEX", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCKpRule = new APIInfo()
        {
            FunctionName = "DeleteCKpRule",
            Description = "DeleteCKpRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "IDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public SkuKeyPartConfig()
        {
            this.Apis.Add(FQueryMpnBySku.FunctionName, FQueryMpnBySku);
            this.Apis.Add(FAddMpnWithSku.FunctionName, FAddMpnWithSku);
            this.Apis.Add(FDeleteMpnWithSku.FunctionName, FDeleteMpnWithSku);
            this.Apis.Add(FUpdateMpnBySku.FunctionName, FUpdateMpnBySku);
            this.Apis.Add(FQueryReplaceKpBySku.FunctionName, FQueryReplaceKpBySku);
            this.Apis.Add(FAddReplaceKpWithSku.FunctionName, FAddReplaceKpWithSku);
            this.Apis.Add(FSetWoReplaceKpWithSku.FunctionName, FSetWoReplaceKpWithSku);
            this.Apis.Add(FGetWoReplaceKpWithSku.FunctionName, FGetWoReplaceKpWithSku);
            this.Apis.Add(FAddCKpCheck.FunctionName, FAddCKpCheck);
            this.Apis.Add(FQueryCKpCheck.FunctionName, FQueryCKpCheck);
            this.Apis.Add(FUpdateCKpCheck.FunctionName, FUpdateCKpCheck);

            this.Apis.Add(FQueryCKpRule.FunctionName, FQueryCKpRule);
            this.Apis.Add(FAddCKpRule.FunctionName, FAddCKpRule);
            this.Apis.Add(FModifyCKpRule.FunctionName, FModifyCKpRule);
            this.Apis.Add(FDeleteCKpRule.FunctionName, FDeleteCKpRule);
        }

        public void QueryMpnBySku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Sku = Data["SKUNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_SKU_MPN cSkuMpn = null;
            List<C_SKU_MPN> cSkuMpnList = new List<C_SKU_MPN>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuMpn = new T_C_SKU_MPN(oleDB, DBTYPE);
                cSkuMpnList = cSkuMpn.GetMpnBySku(oleDB, Sku);
                if(cSkuMpnList.Count>0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(cSkuMpnList.Count);
                    StationReturn.Data = cSkuMpnList;
                }else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void QueryReplaceKpBySku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Sku = Data["SKUNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Replace cKpReplace = null;
            List<C_KP_Replace> cKpReplaceList = new List<C_KP_Replace>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpReplace = new T_C_KP_Replace(oleDB, DBTYPE);
                cKpReplaceList = cKpReplace.GetReplaceKpBySku(oleDB, Sku);
                if (cKpReplaceList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(cKpReplaceList.Count);
                    StationReturn.Data = cKpReplaceList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void AddMpnWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Sku = Data["SKUNO"].ToString().Trim(), PartNo = Data["PARTNO"].ToString().Trim(), Mpn = Data["MPN"].ToString().Trim();
            OleExec oleDB = null;
            T_C_SKU_MPN cSkuMpn = null;
            T_C_SKU cSku = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuMpn = new T_C_SKU_MPN(oleDB, DBTYPE);
                cSku = new T_C_SKU(oleDB, DBTYPE);
                if (!cSku.SkuIsExist(Sku,oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000244";
                    StationReturn.MessagePara = new List<object>() { Sku} ;
                }
                else if (cSkuMpn.IsExists(oleDB,Sku, PartNo, Mpn))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_C_SKU_MPN rowCSkuMpn = (Row_C_SKU_MPN)cSkuMpn.NewRow();
                    rowCSkuMpn.ID = cSkuMpn.GetNewID(this.BU, oleDB, DBTYPE);
                    rowCSkuMpn.SKUNO = Sku;
                    rowCSkuMpn.PARTNO = PartNo;
                    rowCSkuMpn.MPN = Mpn;
                    rowCSkuMpn.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCSkuMpn.EDIT_TIME = GetDBDateTime(); ;
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCSkuMpn.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void AddReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Sku = Data["SKUNO"].ToString().Trim(), PartNo = Data["PARTNO"].ToString().Trim(), ReplacePartno = Data["REPLACEPARTNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Replace cKpReplace = null;
            T_C_SKU cSku = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpReplace = new T_C_KP_Replace(oleDB, DBTYPE);
                cSku = new T_C_SKU(oleDB, DBTYPE);
                if (!cSku.SkuIsExist(Sku, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000244";
                    StationReturn.MessagePara = new List<object>() { Sku };
                }
                else if (cKpReplace.IsExists(oleDB, Sku, PartNo, ReplacePartno))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_C_KP_Replace rowCKpReplace = (Row_C_KP_Replace)cKpReplace.NewRow();
                    rowCKpReplace.ID = cKpReplace.GetNewID(this.BU, oleDB, DBTYPE);
                    rowCKpReplace.SKUNO = Sku;
                    rowCKpReplace.PARTNO = PartNo;
                    rowCKpReplace.REPLACEPARTNO = ReplacePartno;
                    rowCKpReplace.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCKpReplace.EDIT_TIME = GetDBDateTime(); ;
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCKpReplace.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void SetWoReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Wo = Data["WO"].ToString().Trim(), PartNo = Data["PARTNO"].ToString().Trim(), ReplacePartno = Data["REPLACEPARTNO"].ToString().Trim(),Sku = Data["SKUNO"].ToString().Trim();
            OleExec oleDB = null;
            T_R_WO_KP_Repalce rWoKpReplace = null;
            T_C_SKU cSku = null;
            T_R_WO_BASE rWoBase = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                rWoKpReplace = new T_R_WO_KP_Repalce(oleDB, DBTYPE);
                cSku = new T_C_SKU(oleDB, DBTYPE);
                rWoBase = new T_R_WO_BASE(oleDB, DBTYPE);
                if (!cSku.SkuIsExist(Sku, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000245";
                    StationReturn.MessagePara = new List<object>() { Sku };
                }
                else if (rWoBase.CheckDataExist(Wo,Sku, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000245";
                    StationReturn.Data = "";
                }
                else if (rWoKpReplace.CheckDataExist(Wo, PartNo, ReplacePartno, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_R_WO_KP_Repalce rowRWoKpReplace = (Row_R_WO_KP_Repalce)rWoKpReplace.NewRow();
                    rowRWoKpReplace.ID = rWoKpReplace.GetNewID(this.BU, oleDB, DBTYPE);
                    rowRWoKpReplace.WO = Wo;
                    rowRWoKpReplace.PARTNO = PartNo;
                    rowRWoKpReplace.REPALCEPARTNO = ReplacePartno;
                    rowRWoKpReplace.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowRWoKpReplace.EDIT_TIME = GetDBDateTime(); ;
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowRWoKpReplace.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetWoReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string PartNo = Data["PARTNO"].ToString().Trim(), ReplacePartno = Data["REPLACEPARTNO"].ToString().Trim(), Sku = Data["SKUNO"].ToString().Trim();
            OleExec oleDB = oleDB = this.DBPools["SFCDB"].Borrow(); ;
            T_R_WO_KP_Repalce rWoKpReplace = null;
            T_R_WO_BASE rWoBase = null;
            try
            {
                rWoKpReplace = new T_R_WO_KP_Repalce(oleDB, DBTYPE);
                List<R_WO_KP_Repalce> rWoKpRepalceList = rWoKpReplace.GetWoRepalceKpBySkuPartno(Sku, PartNo, ReplacePartno, oleDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000002";
                StationReturn.Data = rWoKpRepalceList;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void DeleteMpnWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string CSKUMPNIDS = Data["CSKUMPNIDS"].ToString().Trim();
            ArrayList DelIds = new ArrayList(CSKUMPNIDS.Split(','));
            DelIds.Remove("");
            OleExec oleDB = null;
            T_C_SKU_MPN cSkuMpn = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuMpn = new T_C_SKU_MPN(oleDB, DBTYPE);
                oleDB.BeginTrain();
                foreach (var VARIABLE in (string[])DelIds.ToArray(typeof(string)))
                {
                    oleDB.ThrowSqlExeception = true;
                    Row_C_SKU_MPN rowCSkuMpn = (Row_C_SKU_MPN)cSkuMpn.GetObjByID(VARIABLE, oleDB, DBTYPE);
                    oleDB.ExecSQL(rowCSkuMpn.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
            }
            catch (Exception e)
            {
                oleDB.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void UpdateMpnWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString().Trim(),
                   sku = Data["SKUNO"].ToString().Trim(),
                   partNo = Data["PARTNO"].ToString().Trim(),
                   mpn = Data["MPN"].ToString().Trim();
            OleExec oleDB = null;
            T_C_SKU_MPN cSkuMpn = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cSkuMpn = new T_C_SKU_MPN(oleDB, DBTYPE);
                Row_C_SKU_MPN rowCSkuMpn = (Row_C_SKU_MPN)cSkuMpn.GetObjByID(id, oleDB, DBTYPE);
                if (cSkuMpn.IsExists(oleDB, sku, partNo, mpn))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else if (rowCSkuMpn!=null)
                {
                    rowCSkuMpn.SKUNO = sku;
                    rowCSkuMpn.PARTNO = partNo;
                    rowCSkuMpn.MPN = mpn;
                    rowCSkuMpn.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCSkuMpn.EDIT_TIME = GetDBDateTime();
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCSkuMpn.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000004";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void UpdateReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString().Trim(),
                   sku = Data["SKUNO"].ToString().Trim(),
                   partNo = Data["PARTNO"].ToString().Trim(),
                   replacePartno = Data["REPLACEPARTNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Replace cKpReplace = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpReplace = new T_C_KP_Replace(oleDB, DBTYPE);
                Row_C_KP_Replace rowCKpReplace = (Row_C_KP_Replace)cKpReplace.GetObjByID(id, oleDB, DBTYPE);
                if (cKpReplace.IsExists(oleDB, sku, partNo, replacePartno))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else if (rowCKpReplace != null)
                {
                    rowCKpReplace.SKUNO = sku;
                    rowCKpReplace.PARTNO = partNo;
                    rowCKpReplace.REPLACEPARTNO = replacePartno;
                    rowCKpReplace.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCKpReplace.EDIT_TIME = GetDBDateTime();
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCKpReplace.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000004";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void DeleteWoReplaceKpWithSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string RWOKPREPLACEIDS = Data["RWOKPREPLACEIDS"].ToString().Trim();
            ArrayList DelIds = new ArrayList(RWOKPREPLACEIDS.Split(','));
            DelIds.Remove("");
            OleExec oleDB = null;
            T_R_WO_KP_Repalce rWoKpRepalce = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                rWoKpRepalce = new T_R_WO_KP_Repalce(oleDB, DBTYPE);
                oleDB.BeginTrain();
                foreach (var VARIABLE in (string[])DelIds.ToArray(typeof(string)))
                {
                    oleDB.ThrowSqlExeception = true;
                    Row_R_WO_KP_Repalce rowCSkuMpn = (Row_R_WO_KP_Repalce)rWoKpRepalce.GetObjByID(VARIABLE, oleDB, DBTYPE);
                    oleDB.ExecSQL(rowCSkuMpn.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
            }
            catch (Exception e)
            {
                oleDB.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void AddCKpCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string TypeName = Data["TYPENAME"].ToString().Trim(), Dll = Data["DLL"].ToString().Trim(), Class = Data["CLASS"].ToString().Trim(), Function = Data["FUNCTION"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Check cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Check(oleDB, DBTYPE);
                if (cKpCheck.IsExists(TypeName, Dll, Class, Function, oleDB))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_C_KP_Check rowCKpCheck = (Row_C_KP_Check)cKpCheck.NewRow();
                    rowCKpCheck.ID = cKpCheck.GetNewID(this.BU, oleDB, DBTYPE);
                    rowCKpCheck.TYPENAME = TypeName;
                    rowCKpCheck.DLL = Dll;
                    rowCKpCheck.CLASS = Class;
                    rowCKpCheck.FUNCTION = Function;
                    rowCKpCheck.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCKpCheck.EDIT_TIME = GetDBDateTime(); ;
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCKpCheck.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void QueryCKpCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string TypeName = Data["TYPENAME"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Check cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Check(oleDB, DBTYPE);
                List<C_KP_Check> cKpCheckList = new List<C_KP_Check>();
                cKpCheckList = cKpCheck.GetCKpCheckByType(TypeName, oleDB);
                if (cKpCheckList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(cKpCheckList.Count);
                    StationReturn.Data = cKpCheckList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void UpdateCKpCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString().Trim(),
                   Dll = Data["DLL"].ToString().Trim(),
                   Class = Data["CLASS"].ToString().Trim(),
                   Function = Data["FUNCTION"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Check cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Check(oleDB, DBTYPE);
                Row_C_KP_Check rowCKpCheck = (Row_C_KP_Check)cKpCheck.GetObjByID(id, oleDB, DBTYPE);
                if (rowCKpCheck == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else 
                {
                    rowCKpCheck.DLL = Dll;
                    rowCKpCheck.CLASS = Class;
                    rowCKpCheck.FUNCTION = Function;
                    rowCKpCheck.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowCKpCheck.EDIT_TIME = GetDBDateTime();
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(rowCKpCheck.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000004";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void DeleteCKpCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string RWOKPREPLACEIDS = Data["IDS"].ToString().Trim();
            ArrayList DelIds = new ArrayList(RWOKPREPLACEIDS.Split(','));
            DelIds.Remove("");
            OleExec oleDB = null;
            T_C_KP_Check cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Check(oleDB, DBTYPE);
                oleDB.BeginTrain();
                foreach (var VARIABLE in (string[])DelIds.ToArray(typeof(string)))
                {
                    oleDB.ThrowSqlExeception = true;
                    Row_C_KP_Check row = (Row_C_KP_Check)cKpCheck.GetObjByID(VARIABLE, oleDB, DBTYPE);
                    oleDB.ExecSQL(row.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
            }
            catch (Exception e)
            {
                oleDB.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void QueryCKpRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Partno = Data["PARTNO"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Rule T = null;
            List<C_KP_Rule> CList = new List<C_KP_Rule>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                T = new T_C_KP_Rule(oleDB, DBTYPE);
                CList = T.GetCKpRule(oleDB, Partno);
                if (CList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(CList.Count);
                    StationReturn.Data = CList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void AddCKpRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Partno = Data["PARTNO"].ToString().Trim(), Mpn = Data["MPN"].ToString().Trim(), ScanType = Data["SCANTYPE"].ToString().Trim(), REGEX = Data["REGEX"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Rule cKpRule = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpRule = new T_C_KP_Rule(oleDB, DBTYPE);
                if (cKpRule.IsExists(oleDB, Partno, Mpn, ScanType))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    Row_C_KP_Rule row = (Row_C_KP_Rule)cKpRule.NewRow();
                    row.ID = cKpRule.GetNewID(this.BU, oleDB, DBTYPE);
                    row.PARTNO = Partno;
                    row.MPN = Mpn;
                    row.SCANTYPE = ScanType;
                    row.REGEX = REGEX;
                    row.EDIT_EMP = this.LoginUser.EMP_NO;
                    row.EDIT_TIME = GetDBDateTime(); ;
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(row.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void ModifyCKpRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString().Trim(),
                   REGEX = Data["REGEX"].ToString().Trim();
            OleExec oleDB = null;
            T_C_KP_Rule cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_KP_Rule(oleDB, DBTYPE);
                Row_C_KP_Rule row = (Row_C_KP_Rule)cKpCheck.GetObjByID(id, oleDB, DBTYPE);
                if (row == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    row.REGEX = REGEX;
                    row.EDIT_EMP = this.LoginUser.EMP_NO;
                    row.EDIT_TIME = GetDBDateTime();
                    oleDB.ThrowSqlExeception = true;
                    oleDB.ExecSQL(row.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000003";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void DeleteCKpRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string RWOKPREPLACEIDS = Data["IDS"].ToString().Trim();
            ArrayList DelIds = new ArrayList(RWOKPREPLACEIDS.Split(','));
            DelIds.Remove("");
            OleExec oleDB = null;
            T_C_KP_Rule cKpRule = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpRule = new T_C_KP_Rule(oleDB, DBTYPE);
                oleDB.BeginTrain();
                foreach (var VARIABLE in (string[])DelIds.ToArray(typeof(string)))
                {
                    oleDB.ThrowSqlExeception = true;
                    Row_C_KP_Rule row = (Row_C_KP_Rule)cKpRule.GetObjByID(VARIABLE, oleDB, DBTYPE);
                    oleDB.ExecSQL(row.GetDeleteString(DBTYPE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
            }
            catch (Exception e)
            {
                oleDB.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
    }
}
