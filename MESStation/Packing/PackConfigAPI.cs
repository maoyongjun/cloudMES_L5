using MESStation.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using Newtonsoft.Json.Linq;

namespace MESStation.Packing
{
    public class PackConfigAPI : MesAPIBase
    {
        private APIInfo _GetPackConfigBySKUNO = new APIInfo()
        {
            FunctionName = "GetPackConfigBySKUNO",
            Description = "獲取料號的包裝配置信息",
            Parameters = new List<APIInputInfo>()
            { new APIInputInfo() { InputName="SkuNo", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _AlertPackConfig = new APIInfo()
        {
            FunctionName = "AlertPackConfig",
            Description = "修改料號的包裝配置信息",
            Parameters = new List<APIInputInfo>()
            { new APIInputInfo() { InputName="PackObj", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetPackType = new APIInfo()
        {
            FunctionName = "GetPackType",
            Description = "獲取可用的PackingType",
            Parameters = new List<APIInputInfo>()
            { 
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetTransportType = new APIInfo()
        {
            FunctionName = "GetTransportType",
            Description = "獲取可用的PackingType",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };
        

        public PackConfigAPI()
        {
            Apis.Add(_GetPackConfigBySKUNO.FunctionName, _GetPackConfigBySKUNO);
            Apis.Add(_AlertPackConfig.FunctionName, _AlertPackConfig);
            Apis.Add(_GetPackType.FunctionName, _GetPackType);
            Apis.Add(_GetTransportType.FunctionName, _GetTransportType);
        }
        public void AlertPackConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                JToken OBJ = Data["PackObj"];
                T_C_PACKING TCP = new T_C_PACKING(db, DB_TYPE_ENUM.Oracle);
                //判斷ID如果為空則插入,如果不為空則更新
                if (OBJ["ID"].ToString() == "")
                {
                    Row_C_PACKING RCP = (Row_C_PACKING)TCP.NewRow();
                    RCP.SKUNO = OBJ["SKUNO"].ToString();
                    RCP.PACK_TYPE = OBJ["PACK_TYPE"].ToString();
                    RCP.TRANSPORT_TYPE = OBJ["TRANSPORT_TYPE"].ToString();
                    RCP.INSIDE_PACK_TYPE = OBJ["INSIDE_PACK_TYPE"].ToString();
                    RCP.MAX_QTY = double.Parse(OBJ["MAX_QTY"].ToString());
                    RCP.DESCRIPTION = OBJ["DESCRIPTION"].ToString();
                    RCP.SN_RULE = OBJ["SN_RULE"].ToString();
                    RCP.EDIT_EMP = LoginUser.EMP_NO;
                    RCP.EDIT_TIME = DateTime.Now;

                    RCP.ID = TCP.GetNewID(BU, db);

                    db.ExecSQL(RCP.GetInsertString(DB_TYPE_ENUM.Oracle));

                }
                else
                {
                    Row_C_PACKING RCP = (Row_C_PACKING)TCP.GetObjByID(OBJ["ID"].ToString(), db);
                    RCP.SKUNO = OBJ["SKUNO"].ToString();
                    RCP.PACK_TYPE = OBJ["PACK_TYPE"].ToString();
                    RCP.TRANSPORT_TYPE = OBJ["TRANSPORT_TYPE"].ToString();
                    RCP.INSIDE_PACK_TYPE = OBJ["INSIDE_PACK_TYPE"].ToString();
                    RCP.MAX_QTY = double.Parse(OBJ["MAX_QTY"].ToString());
                    RCP.DESCRIPTION = OBJ["DESCRIPTION"].ToString();
                    RCP.SN_RULE = OBJ["SN_RULE"].ToString();
                    RCP.EDIT_EMP = LoginUser.EMP_NO;
                    RCP.EDIT_TIME = DateTime.Now;

                    db.ExecSQL(RCP.GetUpdateString(DB_TYPE_ENUM.Oracle));
                }

                
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetPackConfigBySKUNO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                string strSku = Data["SkuNo"].ToString();
                StationReturn.Data = PackingBase.GetPackingConfigBySKU(strSku,db);
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetPackType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                T_C_PACKING_TYPE TCPT = new T_C_PACKING_TYPE(db, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TCPT.GetAllList(db);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

        public void GetTransportType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                T_C_TRANSPORT_TYPE TCTT = new T_C_TRANSPORT_TYPE(db, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TCTT.GetAllList(db);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }


    }
}
