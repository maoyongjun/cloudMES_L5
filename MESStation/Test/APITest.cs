using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.MESReturnView.Public;
using MESStation.BaseClass;
using MESDBHelper;
using MESDataObject;


namespace MESStation.Test
{
    public class APITest : MESStation.BaseClass.MesAPIBase
    {
        protected APIInfo FDBTEST = new APIInfo()
        {
            FunctionName = "DBTEST",
            Description = "數據庫測試",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "data1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "data2", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "data3", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public APITest()
        {
            this.Apis.Add(FDBTEST.FunctionName, FDBTEST);
        }

        public void DBTEST(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            Language = "ENGLISH";
            MESReturnMessage.Language = Language;
            MESReturnMessage.GetMESReturnMessage("MES00000002");
            //throw new Exception("sdsdsdsdsedsdsds");
            string data1 =  Data["data1"].ToString();
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            string strSql = "select 1 from dual";
            
            System.Data.DataSet res = sfcdb.ExecSelect(strSql);
            StationReturn.Data = "OK";

            MESDataObject.Module.T_C_ROUTE T = new MESDataObject.Module.T_C_ROUTE(sfcdb, DB_TYPE_ENUM.Oracle);

            //string ID = T.GetNewID(BU, sfcdb);

            //StationReturn.Data = ID;

            MESDataObject.Module.T_C_SAP_STATION_MAP TC_SAP_STATION_MAP = new MESDataObject.Module.T_C_SAP_STATION_MAP(sfcdb, DB_TYPE_ENUM.Oracle);
            string sap_station_code = TC_SAP_STATION_MAP.GetMAXSAPStationCodeBySku("A03024XNG-A",sfcdb);

            this.DBPools["SFCDB"].Return(sfcdb);
            //this.DBPools.Clear();
        }
    }
}
