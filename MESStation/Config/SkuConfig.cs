using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDBHelper;
using MESDataObject.Module;
using Newtonsoft.Json;
using MESDataObject;
//using WebServer.SocketService;


namespace MESStation.Config
{
    public class SkuConfig : MesAPIBase
    {

        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();

        #region ApiInfos
        private APIInfo AllSKU = new APIInfo()
        {
            FunctionName = "GetAllSku",
            Description = "獲取所有機種",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo AllCSKU = new APIInfo()
        {
            FunctionName = "GetAllCSku",
            Description = "獲取所有機種",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _GetSkuByRouteId = new APIInfo()
        {
            FunctionName = "GetSkuByRouteId",
            Description = "根據路由 ID 獲取對應的機種",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="RouteId",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo SkuByName = new APIInfo()
        {
            FunctionName = "GetSkuByName",
            Description = "根據機種名模獲取機種",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="Sku_Name",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _UpdateSku = new APIInfo()
        {
            FunctionName = "UpdateSku",
            Description = "修改機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _AddSku = new APIInfo()
        {
            FunctionName = "AddSku",
            Description = "添加機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteSku = new APIInfo()
        {
            FunctionName = "DeleteSku",
            Description = "刪除機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteSkuById = new APIInfo()
        {
            FunctionName = "DeleteSkuById",
            Description = "刪除機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SkuID",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };
        #endregion

        public SkuConfig()
        {
            this.Apis.Add(AllSKU.FunctionName, AllSKU);
            this.Apis.Add(SkuByName.FunctionName, SkuByName);
            this.Apis.Add(_AddSku.FunctionName, _AddSku);
            this.Apis.Add(_UpdateSku.FunctionName, _UpdateSku);
            this.Apis.Add(_DeleteSku.FunctionName, _DeleteSku);
            this.Apis.Add(_DeleteSkuById.FunctionName, _DeleteSkuById);
            this.Apis.Add(_GetSkuByRouteId.FunctionName, _GetSkuByRouteId);
        }

        public void GetAllCSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_SKU> SkuList = new List<C_SKU>();
            T_C_SKU Table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuList = Table.GetAllCSku(sfcdb);
                if (SkuList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(SkuList.Count().ToString());
                    StationReturn.Data = SkuList;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        /// <summary>
        /// 只顯示最近修改的 20 條機種的信息，因為一次性返回所有數據數量太大
        /// 如果在顯示出來的列表中沒有該機種，則需要輸入機種關鍵字來進行查詢
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetAllSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<SkuObject> SkuList = new List<SkuObject>();
            T_C_SKU Table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuList = Table.GetAllSku(sfcdb);
                if (SkuList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(SkuList.Count().ToString());
                    StationReturn.Data = SkuList;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetSkuByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<SkuObject> SkuList = new List<SkuObject>();
            T_C_SKU Table = null;
            string SkuName = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuName = Data["Sku_Name"].ToString().Trim();
                if (string.IsNullOrEmpty(SkuName))
                {
                    GetAllSku(requestValue, Data, StationReturn);
                }
                else
                {
                    SkuList = Table.GetSkuByName(SkuName, sfcdb);
                    if (SkuList.Count() == 0)
                    {
                        //沒有獲取到數據
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new object();
                    }
                    else
                    {
                        //獲取成功
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(SkuList.Count().ToString());
                        StationReturn.Data = SkuList;
                        
                    }
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }

        public void GetSkuByRouteId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SKU_ROUTE table = null;
            OleExec sfcdb = null;
            string RouteId = string.Empty;
            List<C_SKU> SkuList = new List<C_SKU>();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                RouteId = Data["RouteId"].ToString();
                if (!string.IsNullOrEmpty(RouteId))
                {
                    SkuList = table.GetSkuListByMappingRouteID(RouteId, sfcdb);
                    if (SkuList.Count() == 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new object();
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(SkuList.Count().ToString());
                        StationReturn.Data = SkuList;

                    }
                }
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void UpdateSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU Table = null;
            string SkuObject = string.Empty;
            SkuObject Sku = null;
            string result = string.Empty;
            StringBuilder SkuId;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuObject = Data["SkuObject"].ToString();
                Sku = (SkuObject)JsonConvert.Deserialize(SkuObject, typeof(SkuObject));
                Sku.LastEditUser = LoginUser.EMP_NO;
                result= Table.UpdateSku(BU, Sku, "UPDATE", GetDBDateTime(),out SkuId, sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //更新成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = SkuId.ToString();
                }
                else
                {
                    //更新失敗
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                //不是最新的數據，返回字符串無法被 Int32.Parse 方法轉換成 int,所以出現異常
                if (!string.IsNullOrEmpty(result))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000032";
                    StationReturn.Data = e.Message + ":" + result;
                }
                else
                {
                    //數據庫執行異常
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }

        public void AddSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU Table = null;
            string SkuObject = string.Empty;
            SkuObject Sku = null;
            string result = string.Empty;
            StringBuilder SkuId;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuObject = Data["SkuObject"].ToString();
                Sku = (SkuObject)JsonConvert.Deserialize(SkuObject, typeof(SkuObject));
                Sku.LastEditUser = LoginUser.EMP_NO;
                result = Table.UpdateSku(BU, Sku, "ADD", GetDBDateTime(),out SkuId, sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //添加成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = SkuId.ToString();

                }
                else
                {
                    //沒有添加任何數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = e.Message + ":" + result;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(e.Message);
                    StationReturn.Data = e.Message;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DeleteSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU Table = null;
            string SkuObject = string.Empty;
            SkuObject Sku = null;
            string result = string.Empty;
            StringBuilder SkuId;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuObject = Data["SkuObject"].ToString();
                Sku = (SkuObject)JsonConvert.Deserialize(SkuObject, typeof(SkuObject));
                result = Table.UpdateSku(BU, Sku, "DELETE",GetDBDateTime(),out SkuId, sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = SkuId.ToString();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DeleteSkuById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU Table = null;
            SkuObject Sku = null;
            string result = string.Empty;
            string SkuId = string.Empty;
            StringBuilder strSkuId;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuId = Data["SkuID"].ToString();
                Sku = new SkuObject();
                Sku.SkuId = SkuId;
                result = Table.UpdateSku(BU, Sku, "DELETE",GetDBDateTime(),out strSkuId, sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //刪除成功
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.Data = strSkuId.ToString();
                }
                else
                {
                    //沒有刪除任何數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = result;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetAllSkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<string> SkuList = new List<string>();
            T_C_SKU Table = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuList = Table.GetAllSkunoList(sfcdb);
                if (SkuList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(SkuList.Count().ToString());
                    StationReturn.Data = SkuList;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    }
}
