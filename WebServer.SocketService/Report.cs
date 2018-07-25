using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using MESStation;
using MESStation.MESReturnView.Public;
using MESStation.BaseClass;
using System.Xml.Linq;
using System.Reflection;
using MESDBHelper;
using MESDataObject;

namespace WebServer.SocketService
{
    public class Report : ServiceBase
    {
        public static MESDBHelper.OleExecPool SFCDBPool = new OleExecPool("TESTDB", true);
        public static MESDBHelper.OleExecPool APDBPool = new OleExecPool("APDB", true);
        public static Dictionary<string, MESStation.LogicObject.User> LoginUsers = new Dictionary<string, MESStation.LogicObject.User>();
        public string Token = null;
        

        protected override void OnMessage(MessageEventArgs e)
        {
            
            MESStationReturn StationReturn = null;// new MESStationReturn();
            string[] Para = null; //add by LLF 2017-1-4
            try
            {
                //處理JSON
                //Newtonsoft.Json.Linq.JObject Request = (Newtonsoft.Json.Linq.JObject) Newtonsoft.Json.JsonConvert.DeserializeObject(
                //"{ TOKEN:null, CLASS: \"MESStation.ApiHelper\", FUNCTION:\"GetApiClassList\", DATA:{ } }");

                //Request = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject("{ TOKEN:null, CLASS: \"MESStation.ApiHelper\", FUNCTION:\"GetApiFunctionsList\", DATA:{ CLASSNAME:\"MESStation.ApiHelper\" } }");
                //Request = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(
                //    "{ TOKEN:null, CLASS: \"MESStation.ApiHelper\", FUNCTION:\"GetApiFunctionsList\", DATA:{ CLASSNAME:\"MESStation.UserManager\" } }");

                Newtonsoft.Json.Linq.JObject Request = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(e.Data);
                string CLASS = Request["Class"].ToString();
                string FUNCTION = Request["Function"].ToString();
                string TOKEN = Request["Token"].ToString();
                string MsgID = Request["MessageID"]?.ToString();
                string ClientID = Request["ClientID"]?.ToString();
                Request.Add("IP", Newtonsoft.Json.Linq.JToken.Parse("{Value:\""+this.ClientIP +"\"}"));
                

                StationReturn = new MESStationReturn(MsgID, ClientID);
                //反射加載

                //ApiHelper api = new ApiHelper();
                Type APIType;
                //加載類庫
                Assembly assembly = Assembly.Load("MESStation");
                APIType = assembly.GetType(CLASS);
                object API_CLASS = assembly.CreateInstance(CLASS);
                MesAPIBase API = (MesAPIBase)API_CLASS;
                if (!API.DBPools.ContainsKey("SFCDB"))
                {
                    API.DBPools.Add("SFCDB", SFCDBPool);
                }
                if (!API.DBPools.ContainsKey("APDB"))
                {
                    API.DBPools.Add("APDB", APDBPool);
                }
                //API.BU = "HWD";
                //API.BU = "VERTIV";
                ((MesAPIBase)API_CLASS).IP = this.ClientIP;

                API.Language = "CHINESE";  //CHINESE,CHINESE_TW,ENGLISH;

                //初始化異常類型的數據庫連接池
                MESReturnMessage.SetSFCDBPool(SFCDBPool);
                //獲取調用函數
                MethodInfo Function = APIType.GetMethod(FUNCTION);
                //
                bool CheckLogin = false;
                if (LoginUsers.ContainsKey(TOKEN))
                {
                    MESStation.LogicObject.User lu = LoginUsers[TOKEN];
                    ((MesAPIBase)API_CLASS).LoginUser = lu;
                    CheckLogin = true;
                    API.BU = lu.BU;
                }
                else
                {
                    if (FUNCTION != "Login" && ((MesAPIBase)API_CLASS).MastLogin)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = "No Login !";

                    }
                    else
                    {
                        if (FUNCTION == "Login")
                        {
                            CheckLogin = true;
                        }
                    }
                }
                if (CheckLogin)
                {

                    Function.Invoke(API_CLASS, new object[] { Request, Request["Data"], StationReturn });
                    if (FUNCTION == "Login")
                    {
                        if (StationReturn.Status == "Pass")
                        {
                            LoginReturn r = (LoginReturn)StationReturn.Data;
                            MESStation.LogicObject.User lu = ((MesAPIBase)API_CLASS).LoginUser;
                            if (this.Token != null)
                            {
                                Report.LoginUsers.Remove(Token);
                                MESStation.Stations.CallStation.logout(Token);
                            }
                            string NewToken = r.Token;
                            Token = r.Token;
                            if (LoginUsers.ContainsKey(NewToken))
                            {
                                LoginUsers[NewToken] = lu;
                            }
                            else
                            {
                                LoginUsers.Add(NewToken, lu);
                            }
                        }

                    }
                }//函數不要求登錄
                else if (!((MesAPIBase)API_CLASS).MastLogin)
                {
                    Function.Invoke(API_CLASS, new object[] { Request, Request["Data"], StationReturn });
                    if (FUNCTION == "Login")
                    {
                        if (StationReturn.Status == "Pass")
                        {
                            LoginReturn r = (LoginReturn)StationReturn.Data;
                            MESStation.LogicObject.User lu = ((MesAPIBase)API_CLASS).LoginUser;
                            string NewToken = r.Token;
                            if (LoginUsers.ContainsKey(NewToken))
                            {
                                LoginUsers[NewToken] = lu;
                            }
                            else
                            {
                                LoginUsers.Add(NewToken, lu);
                            }
                        }
                    }
                }

                //add by LLF 2017-12-27
                if (StationReturn.MessageCode!=null)
                {
                    if (StationReturn.MessageCode.Length > 0)
                    {
                        if (StationReturn.MessagePara != null)
                        {
                            if (StationReturn.MessagePara.Count > 0)
                            {
                                Para = new string[StationReturn.MessagePara.Count];
                                for (int i = 0; i < StationReturn.MessagePara.Count; i++)
                                {
                                    Para[i] = StationReturn.MessagePara[i].ToString();
                                }
                            }
                        }
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage(StationReturn.MessageCode, Para);
                    }
                }
            }
            catch (MESReturnMessage ee)
            {
                StationReturn.Status = MESStation.BaseClass.StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
                if (ee.InnerException != null)
                {
                    StationReturn.Data = ee.InnerException.Message;
                }
            }
            catch (Exception ee)
            {
                StationReturn.Status = MESStation.BaseClass.StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
                if (ee.InnerException != null)
                {
                    StationReturn.Data = ee.InnerException.Message;
                }
            }


            
            System.Web.Script.Serialization.JavaScriptSerializer JsonMaker = new System.Web.Script.Serialization.JavaScriptSerializer();
            JsonMaker.MaxJsonLength = int.MaxValue;

            string json = JsonMaker.Serialize(StationReturn);
            //JavaScriptSerializer 實例在序列化對象的時候，遇到 DateTime 類型會序列化出不可讀的數據，
            //因此改用 Newtonsoft 的 JsonConvert 來進行序列化，序列化出來的 DateTime 形如 2017-12-06T11:14:37
            //另外如果遇到無法將 System.DBNull 類型轉換成 string 類型的，可以手動檢測下值的類型，
            //如果是 System.DBNull，直接將值改為 null 即可。
            //實在無法實現你所需要的功能，可將下面這句註釋掉。
            //
            // modify by 張官軍 2017/12/06

            //變更時間格式  modify by Wuq 2018/01/25
            json = Newtonsoft.Json.JsonConvert.SerializeObject(StationReturn, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            //json = Newtonsoft.Json.JsonConvert.SerializeObject(StationReturn);

            Send(json);

        }

    }
}
