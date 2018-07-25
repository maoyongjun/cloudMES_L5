using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESStation.Config
{
    public class ConvertWorkorder : MesAPIBase
    {
        #region
        protected APIInfo _GetWoConvertList = new APIInfo()
        {
            FunctionName = "GetWoConvertList",
            Description = "Get Wo Convert List",
            Parameters = new List<APIInputInfo>()
            {
                //new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetWoInfoByWo = new APIInfo()
        {
            FunctionName = "GetWoInfoByWo",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "WO", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetWoInfoById = new APIInfo()
        {
            FunctionName = "GetWoInfoById",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "ID", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetWoType = new APIInfo()
        {
            FunctionName = "GetWoType",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                //new APIInputInfo()
                //{
                //    InputName = "ID", InputType = "string", DefaultValue = ""
                //}
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetStationBySkuno = new APIInfo()
        {
            FunctionName = "GetStationBySkuno",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "skuno", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetRouteBySkuno = new APIInfo()
        {
            FunctionName = "GetRouteBySkuno",
            Description = "GetWoInfoByWo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "skuno", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetKeyPartBySkuno = new APIInfo()
        {
            FunctionName = "GetKeyPartBySkuno",
            Description = "_GetKeyPartBySkuno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo()
                {
                    InputName = "skuno", InputType = "string", DefaultValue = ""
                }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _SubmitWoInfo = new APIInfo()
        {
            FunctionName = "SubmitWoInfo",
            Description = "_SubmitWoInfo",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "wo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "factory", InputType = "string" },
                new APIInputInfo() { InputName = "date", InputType = "string" },
                new APIInputInfo() { InputName = "qty", InputType = "string" },
                new APIInputInfo() { InputName = "wo_type", InputType = "string" },
                new APIInputInfo() { InputName = "station", InputType = "string" },
                new APIInputInfo() { InputName = "skuno", InputType = "string" },
                new APIInputInfo() { InputName = "sku_ver", InputType = "string" },
                new APIInputInfo() { InputName = "route_name", InputType = "string" },
                new APIInputInfo() { InputName = "kp_list_id", InputType = "string" }
            },
            Permissions = new List<MESPermission>() { }
        };
        #endregion

        public ConvertWorkorder()
        {
            Apis.Add(_GetWoConvertList.FunctionName, _GetWoConvertList);
            Apis.Add(_GetWoInfoByWo.FunctionName, _GetWoInfoByWo);
            Apis.Add(_GetWoInfoById.FunctionName, _GetWoInfoById);
            Apis.Add(_GetWoType.FunctionName, _GetWoType);
            Apis.Add(_GetStationBySkuno.FunctionName, _GetStationBySkuno);
            Apis.Add(_GetRouteBySkuno.FunctionName, _GetRouteBySkuno);
            Apis.Add(_GetKeyPartBySkuno.FunctionName, _GetKeyPartBySkuno);
            Apis.Add(_SubmitWoInfo.FunctionName, _SubmitWoInfo);
        }

        public void GetWoConvertList(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_HEADER t_header = null;
            DataTable dt = null;
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_R_WO_HEADER(oleDB, DB_TYPE_ENUM.Oracle);
                //dt = t_header.GetConvertWoList(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetWoSpecialVar(oleDB, new string[0]);
                if (dt == null || dt.Rows.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    //StationReturn.Data = dt;
                    StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception ex)
            {
                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
                throw ex;
            }
        }

        public void GetWoInfoByWo(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_HEADER t_header = null;
            DataTable dt = null;
            string wo = Data["WO"].ToString();
            if (string.IsNullOrEmpty(wo))
            {
                //throw new Exception();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Workorder" };
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_R_WO_HEADER(oleDB, DB_TYPE_ENUM.Oracle);
                //dt = t_header.GetConvertWoList(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetConvertWoTableByWO(oleDB, DB_TYPE_ENUM.Oracle, wo);
                if (dt == null || dt.Rows.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception ex)
            {
                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
                throw ex;
            }
        }

        public void GetWoInfoById(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_HEADER t_header = null;
            DataTable dt = null;
            string wo = Data["ID"].ToString();
            if (string.IsNullOrEmpty(wo))
            {
                //throw new Exception();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "ID" };
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_R_WO_HEADER(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetConvertWoTableById(oleDB, wo);
                if (dt == null || dt.Rows.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception ex)
            {
                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
                throw ex;
            }
        }

        public void GetWoType(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_TYPE t_header = null;
            List<string> dt = null;
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_R_WO_TYPE(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetAllType(oleDB);
                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                    //StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception ex)
            {
                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
                throw ex;
            }
        }

        public void GetStationBySkuno(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_SKU t_header = null;
            List<string> dt = null;
            string skuno = Data["skuno"].ToString();
            if (string.IsNullOrEmpty(skuno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Skuno" };
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_C_SKU(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetStationBySku(oleDB, skuno);
                
                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                    //StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception ex)
            {
                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
                throw ex;
            }
        }

        public void GetRouteBySkuno(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_ROUTE t_header = null;
            List<string> dt = null;
            string skuno = Data["skuno"].ToString();
            if (string.IsNullOrEmpty(skuno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Skuno" };
                StationReturn.Data = "";
                return;
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_C_ROUTE(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetRouteBySkuno(oleDB, skuno);
                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                    //StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception ex)
            {
                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
                throw ex;
            }
        }
        
        public void GetKeyPartBySkuno(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_KEYPART t_header = null;
            List<string> dt = null;
            string skuno = Data["skuno"].ToString();
            if (string.IsNullOrEmpty(skuno))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<object>() { "Skuno" };
                StationReturn.Data = "";
                return;
                //throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "skuno" }));
            }
            try
            {
                oleDB = DBPools["SFCDB"].Borrow();
                t_header = new T_C_KEYPART(oleDB, DB_TYPE_ENUM.Oracle);
                dt = t_header.GetListBySkuno(oleDB, skuno);
                if (dt == null || dt.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = dt;
                    //StationReturn.Data = ConvertToJson.DataTableToJson(dt);
                }

                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception ex)
            {
                if (oleDB != null) DBPools["SFCDB"].Return(oleDB);
                throw ex;
            }
        }

        public void SubmitWoInfo(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            //wo
            string wo = Data["wo"].ToString();
            if (string.IsNullOrEmpty(wo))
            {
                //StationReturn.Status = StationReturnStatusValue.Fail;
                //StationReturn.MessageCode = "MES00000006";
                //StationReturn.MessagePara = new List<object>() { "Skuno" };
                //StationReturn.Data = "";
                //return;
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "WO" }));
            }

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            T_R_WO_BASE t_wo = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
            
            if (!t_wo.CheckDataExist(wo, sfcdb))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { wo }));
            }

            R_WO_HEADER wo_header = new T_R_WO_HEADER(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailByWo(sfcdb, wo);
            
            //sku info
            string skuno = Data["skuno"].ToString();//wo_header.MATNR
            if (string.IsNullOrEmpty(skuno))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SKUNO" }));
            }
            string skuver = Data["sku_ver"].ToString();//wo_header.REVLV

            C_SKU c_sku = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle).GetSku(skuno, sfcdb, DB_TYPE_ENUM.Oracle).GetDataObject();
            if (c_sku == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { skuno }));
            }


            //route exchange from name
            string route_name = Data["route_name"].ToString();
            if (string.IsNullOrEmpty(route_name))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "ROUTE" }));
            }
            C_ROUTE c_route = new T_C_ROUTE(sfcdb, DB_TYPE_ENUM.Oracle).GetByRouteName(route_name, sfcdb);
            if (c_route == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { route_name }));
            }

            //station route check
            string station_name = Data["station"].ToString();
            if (string.IsNullOrEmpty(station_name))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "STATION" }));
            }
            List<C_ROUTE_DETAIL> c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle).GetByRouteIdOrderBySEQASC(c_route.ID, sfcdb);
            if (c_route_detail != null && c_route_detail.Count > 0)
            {
                C_ROUTE_DETAIL check = c_route_detail.Find(t => t.STATION_NAME == station_name);
                if (check == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { station_name }));
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { route_name }));
            }

            //data record
            Row_R_WO_BASE row_wobase = (Row_R_WO_BASE) t_wo.NewRow();
            row_wobase.ID = t_wo.GetNewID(this.BU, sfcdb);
            row_wobase.WORKORDERNO = wo;
            row_wobase.PLANT = Data["factory"].ToString();
            row_wobase.RELEASE_DATE = DateTime.Now;
            row_wobase.DOWNLOAD_DATE = Convert.ToDateTime(Data["date"].ToString());
            row_wobase.PRODUCTION_TYPE = "BTO";
            row_wobase.WO_TYPE = Data["wo_type"].ToString();
            row_wobase.SKUNO = skuno;
            row_wobase.SKU_VER = skuver;
            row_wobase.SKU_NAME = c_sku.SKU_NAME;
            //row_wobase.SKU_SERIES = null;
            //row_wobase.SKU_DESC = null;
            row_wobase.CUST_PN = c_sku.CUST_PARTNO;
            row_wobase.ROUTE_ID = c_route.ID;
            row_wobase.START_STATION = station_name;
            row_wobase.KP_LIST_ID = Data["kp_list_id"].ToString();
            row_wobase.CLOSED_FLAG = "0";
            row_wobase.WORKORDER_QTY = Convert.ToDouble(Data["qty"].ToString());
            row_wobase.STOCK_LOCATION = wo_header.LGORT;
            row_wobase.CUST_ORDER_NO = wo_header.ABLAD;
            row_wobase.EDIT_EMP = this.LoginUser.EMP_NO;
            row_wobase.EDIT_TIME = DateTime.Now;

            string sql = row_wobase.GetInsertString(DB_TYPE_ENUM.Oracle);
            
            try
            {
                int res = sfcdb.ExecSqlNoReturn(sql, null);
                if (res == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { wo }));
                }
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception ex)
            {
                if (sfcdb != null) DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }

        }

    }
}
