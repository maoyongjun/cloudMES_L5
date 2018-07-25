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
    public class CStationConfig : MESStation.BaseClass.MesAPIBase
    {
        public CStationConfig()
        {
            this.Apis.Add(FSelectByColumnName.FunctionName, FSelectByColumnName);
            this.Apis.Add(FStationInsert.FunctionName, FStationInsert);
            this.Apis.Add(FStationDelete.FunctionName, FStationDelete);
            this.Apis.Add(FShowAllData.FunctionName, FShowAllData);
            this.Apis.Add(FUpdateStationByID.FunctionName, FUpdateStationByID);
            this.Apis.Add(FGetStationSelectInputType.FunctionName, FGetStationSelectInputType);
        }

        protected APIInfo FUpdateStationByID = new APIInfo()
        {
            FunctionName = "UpdateStationByID",
            Description = "根据ID值更新STATION",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "StationName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FShowAllData = new APIInfo()
        {
            FunctionName = "ShowAllData",
            Description = "查询STATION所有数据",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectByColumnName = new APIInfo()
        {
            FunctionName = "SelectByColumnName",
            Description = "根据传入的栏位及值进行查询操作",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ColumnName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ColumnValue", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FStationInsert = new APIInfo()
        {
            FunctionName = "StationInsert",
            Description = "执行插入操作",
            Parameters = new List<APIInputInfo>()
            {
                //new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "StationName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FStationDelete = new APIInfo()
        {
            FunctionName = "DeleteStationByID",
            Description = "根据ID删除信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetStationSelectInputType = new APIInfo()
        {
            FunctionName = "GetStationSelectInputType",
            Description = "獲取工站可以選擇的輸入類型",
            Parameters = new List<APIInputInfo>()
            {

            },
            Permissions = new List<MESPermission>() { }
        };

        public void UpdateStationByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STATION cSection = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_STATION r = (Row_C_STATION)cSection.NewRow();
                r = (Row_C_STATION)cSection.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                r.STATION_NAME = (Data["StationName"].ToString()).Trim();
                r.TYPE = (Data["TYPE"].ToString()).Trim();
                string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet)>0)
                {
                    StationReturn.MessageCode = "MES00000003";
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

        public void ShowAllData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_STATION_DETAIL> list = new List<C_STATION_DETAIL>();
                list = cStation.ShowAllData(sfcdb);
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

        public void StationInsert(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_STATION rStation = (Row_C_STATION)cStation.NewRow();
                rStation.ID = cStation.GetNewID(BU, sfcdb);
                rStation.STATION_NAME = (Data["StationName"].ToString()).Trim();
                rStation.TYPE = (Data["TYPE"].ToString()).Trim();
                string strRet = sfcdb.ExecSQL(rStation.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = strRet;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000036";
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

        public void SelectByColumnName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string data = (Data["ColumnValue"].ToString()).Trim();
                string column = (Data["ColumnName"].ToString()).Trim();
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_STATION_DETAIL> list = new List<C_STATION_DETAIL>();
                list = cStation.GetDataByColumn(column, data, sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
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

        public void DeleteStationByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_STATION r = (Row_C_STATION)cStation.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
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

        /// <summary>
        ///獲取PTH StationNumber
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetStationNumber(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //List<int> StationNum = new List<int>();
            //for (int i = 1; i < 18; i++)
            //{
            //    StationNum.Add(i);
            //}
            //StationNum.Sort();
            List<string> StationNum = new List<string>();
            StationNum.Add("");
            for (int i = 1; i < 18; i++)
            {
                StationNum.Add(i.ToString());
            }
            StationReturn.Data = StationNum;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }

        /// <summary>
        /// 獲取工站可以選擇的輸入類型
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetStationSelectInputType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {   
            List<string> listInputType = new List<string>();
            listInputType.Add("");
            listInputType.Add("SN");
            listInputType.Add("PANEL");
            StationReturn.Data = listInputType;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
    }
}

