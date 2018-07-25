using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;

namespace MESStation.Stations
{
    public class CallStation : MESStation.BaseClass.MesAPIBase
    {
        static Dictionary<string, MESStationBase> StationPool = new Dictionary<string, MESStationBase>();

        public static void logout(string Token)
        {
            List<string> keys = StationPool.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].StartsWith(Token))
                {
                    MESStationBase s = StationPool[keys[i]];
                    s.DBS = null;
                    s.SFCDB = null;
                    StationPool.Remove(keys[i]);
                }
            }
        }

        protected APIInfo FInitStation = new APIInfo()
        {
            FunctionName = "InitStation",
            Description = "初始化工站",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DisplayStationName", InputType = "string", DefaultValue = "SFC_SMT_LOADING" },
                new APIInputInfo() {InputName = "Line", InputType = "string", DefaultValue = "Line1" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FStationInput = new APIInfo()
        {
            FunctionName = "StationInput",
            Description = "工站Input",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Station", InputType = "object", DefaultValue = "" },
                new APIInputInfo() {InputName = "Input", InputType = "object", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo initcreatestation = new APIInfo()
        {
            FunctionName = "InitCreateStation",
            Description = "加載工站數據",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DisplayStationName", InputType = "string", DefaultValue = "Sample" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo savestation = new APIInfo()
        {
            FunctionName = "SaveStation",
            Description = "保存工站",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DisplayStationName", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        public CallStation()
        {
            this.Apis.Add("Init", FInitStation);
            this.Apis.Add("StationInput", FStationInput);
            this.Apis.Add("InitCreateStation", initcreatestation);
            this.Apis.Add("savestation", savestation);
        }

        public void InitStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            
            string DisplayName = Data["DisplayStationName"]?.ToString();
            string Token = requestValue["Token"]?.ToString();
            string Line = Data["Line"]?.ToString();
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            OleExec APDB = this.DBPools["APDB"].Borrow();
            try
            {
                //從對象池中取工站,如不存在則新建一個
                MESStationBase retStation = null;
                if (StationPool.ContainsKey(Token + DisplayName))
                {
                    retStation = StationPool[Token + DisplayName];
                }
                else
                {
                    retStation = new MESStationBase();
                    StationPool.Add(Token + DisplayName, retStation);
                }
                retStation.StationOutputs.Clear();
                retStation.StationMessages.Clear();
                retStation.StationSession.Clear();
                retStation.DisplayOutput.Clear();
                retStation.Inputs.Clear();
                retStation.IP = this.IP;
                //add by 張官軍 2018-1-4 不添加的話，後面獲取該信息的時候回傳空
                retStation.LoginUser = LoginUser;
                //給工站對象賦公共值               
                retStation.Init(DisplayName, Line, BU, DBPools);
                MESStation.MESReturnView.Station.CallStationReturn ret = new MESReturnView.Station.CallStationReturn();
                ret.Station = retStation;
                //用以執行InitInput.Run()  2018/01/30 SDL
                retStation.SFCDB = SFCDB;
                retStation.APDB = APDB;
                //調用工站初始配置
                MESStationInput InitInput = retStation.Inputs.Find(t => t.Name == "StationINIT");
                if (InitInput != null)
                {
                    InitInput.Run();
                    retStation.Inputs.Remove(InitInput);
                }
                if (retStation.FailStation != null)
                {
                    InitInput = null;
                    InitInput = retStation.FailStation.Inputs.Find(t => t.Name == "StationINIT");
                    if (InitInput != null)
                    {
                        InitInput.Run();
                        retStation.FailStation.Inputs.Remove(InitInput);
                    }
                }

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "'Init successfull.";
            } catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                this.DBPools["APDB"].Return(APDB);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void InitCreateStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            string DisplayName = Data["DisplayStationName"]?.ToString();
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                //新建一個工站對象
                //   MESStationBase retStation = new MESStationBase();

                MESStationModel stationmodel = new MESStationModel();

                //給工站對象賦公共值
                stationmodel.Init(DisplayName, SFCDB);

            //    MESStation.BaseClass.test test = new MESStation.BaseClass.test();
                //    test.ccc();
                //    MESStation.MESReturnView.Station.CallStationReturn ret = new MESReturnView.Station.CallStationReturn();
                //   ret.Station = retStation;
                StationReturn.Data = stationmodel;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "'Init successfull.";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                throw ee;
            }
        }
        public void StationInput(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string DisplayName = Data["Station"]["DisplayName"]?.ToString();
            string Token = requestValue["Token"]?.ToString();
            JToken RCurrInput = Data["Input"];
            MESStationInput CurrInput = null;
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            OleExec APDB = this.DBPools["APDB"].Borrow();
            //將工站返回的值加載入工站模型中
            MESStationBase Station = null;
            if (StationPool.ContainsKey(Token + DisplayName))
            {
                Station = StationPool[Token + DisplayName];
            }
            Station.StationMessages.Clear();
            Station.NextInput = null;
            Station.SFCDB = SFCDB;
            Station.APDB = APDB;
            Station.IP = requestValue["IP"]["Value"].ToString();

            Station.LabelPrint.Clear();
            Station.ScanKP.Clear();
            try
            {

                for (int i = 0; i < Data["Station"]["Inputs"].Count(); i++)
                {

                    JToken rinput = Data["Station"]["Inputs"][i];
                    MESStationInput input = Station.Inputs.Find(t => t.DisplayName == rinput["DisplayName"].ToString());
                    if (input == null)
                    {
                        continue;
                    }
                    input.Value = rinput["Value"].ToString();
                    if (Data["ScanType"].ToString() == "Pass" && input.DisplayName == RCurrInput["DisplayName"].ToString())
                    {
                        CurrInput = input;
                    }
                }
                if (Station.FailStation != null)
                {
                    //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 Begin
                    Station.FailStation.DBS = Station.DBS;
                    Station.FailStation.SFCDB = SFCDB;
                    Station.FailStation.APDB = APDB;
                    //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 End
                    for (int i = 0; i < Data["Station"]["FailStation"]["Inputs"].Count(); i++)
                    {

                        JToken rinput = Data["Station"]["FailStation"]["Inputs"][i];
                        MESStationInput input = Station.FailStation.Inputs.Find(t => t.DisplayName == rinput["DisplayName"].ToString());
                        if (input == null)
                        {
                            continue;
                        }
                        input.Value = rinput["Value"].ToString();
                        if (Data["ScanType"].ToString() == "Fail" && input.DisplayName == RCurrInput["DisplayName"].ToString())
                        {
                            CurrInput = input;
                        }
                    }
                }

                MESStation.MESReturnView.Station.CallStationReturn ret = new MESReturnView.Station.CallStationReturn();
                ret.ScanType = Data["ScanType"].ToString();
                //add by ZGJ 2018-03-19 清空之前的輸入動作執行後輸出到前台的消息
                CurrInput.Station.StationMessages.Clear();
                //調用處理邏輯
                CurrInput.Run();

                Station.MakeOutput();

                if (Data["ScanType"].ToString() == "Pass")
                {
                    if (Station.NextInput == null)
                    {
                        for (int i = 0; i < Station.Inputs.Count; i++)
                        {
                            if (Station.Inputs[i] == CurrInput)
                            {
                                if (i != Station.Inputs.Count - 1)
                                {
                                    ret.NextInput = Station.Inputs[i + 1];
                                }
                                else
                                {

                                    ret.NextInput = Station.Inputs[0];
                                }

                            }
                        }
                    }
                    else
                    {
                        ret.NextInput = Station.NextInput;
                    }
                }
                else if(Station.FailStation!=null)
                {
                    if (Station.FailStation.NextInput == null)
                    {
                        for (int i = 0; i < Station.FailStation.Inputs.Count; i++)
                        {
                            if (Station.FailStation.Inputs[i] == CurrInput)
                            {
                                if (i != Station.FailStation.Inputs.Count - 1)
                                {
                                    ret.NextInput = Station.FailStation.Inputs[i + 1];
                                }
                                else
                                {

                                    ret.NextInput = Station.FailStation.Inputs[0];
                                }

                            }
                        }
                    }
                    else
                    {
                        ret.NextInput = Station.FailStation.NextInput;
                    }
                }


                //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 Begin
                if (Station.FailStation != null)
                {
                    Station.FailStation.DBS = null;
                    Station.FailStation.SFCDB = null;
                }
                //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 end
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);

                Station.SFCDB = null;
                Station.APDB = null;

                ret.Station = Station;
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "' Input successfull.";
            }
            catch (Exception ee)
            {
                Station.MakeOutput();
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);

                Station.SFCDB = null;
                Station.APDB = null;
                MESStation.MESReturnView.Station.CallStationReturn ret = new MESReturnView.Station.CallStationReturn();
                ret.Station = Station;
                Station.StationMessages.Add(new MESReturnView.Station.StationMessage() { Message = ee.Message, State = MESReturnView.Station.StationMessageState.Fail });
                Station.NextInput = CurrInput;
                
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "' Input not successfull.";
            }

        }

//        public void SaveStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
//        {
//            string DisplayName = Data["Station"]["DISPLAY_STATION_NAME"]?.ToString();
//            string StationName = Data["Station"]["STATION_NAME"]?.ToString();
//            string FailStationID = Data["Station"]["FAIL_STATION_ID"]?.ToString();
//            double FailStationFlag =Convert.ToDouble( Data["Station"]["FAIL_STATION_FLAG"]);
//            string StationID= Data["Station"]["ID"]?.ToString();

//            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
//            try
//            {
//                sfcdb.BeginTrain();
//                MesAPIBase api = new MesAPIBase();
//                api.LoginUser = LoginUser;
//                //插入R_STATION表
//                StationConfig.StationConfig stationconfig = new StationConfig.StationConfig();
              
//                stationconfig.AddStation(DisplayName, StationName, FailStationID, FailStationFlag, StationID, sfcdb);
//                //插入R_Station_Output表
//                for (int i = 0; i < Data["Station"]["OutputList"].Count(); i++)
//                {
//                    JToken output = Data["Station"]["OutputList"][i];
//                    StationConfig.StationOutputConfig outputconfig = new StationConfig.StationOutputConfig();
//                    outputconfig.LoginUser = LoginUser;
//                    outputconfig.AddStationOutput(output, sfcdb);
//                }
//                //   //插入 R_Station_Input 表
//                for (int i = 0; i < Data["Station"]["InputList"].Count(); i++)
//                {
//                    //插入 R_Station_Input 表
//                    JToken input = Data["Station"]["InputList"][i];
//                    StationConfig.StationInputConfig stationinput = new StationConfig.StationInputConfig();
//                    stationinput.LoginUser = LoginUser;
//                    stationinput.AddInput(input, sfcdb);
//                    //插入 R_Input_Action 表
//                    for (int j = 0; j < Data["input"]["InputActionList"].Count(); j++)
//                    {
//                        JToken iaction = Data["input"]["InputActionList"][i];
//                        StationConfig.InputActionConfig inputaction = new StationConfig.InputActionConfig();
//                        inputaction.LoginUser = LoginUser;
//                        inputaction.AddInputActionS(iaction, sfcdb);
//                        for (int k = 0; k < iaction["ParaSA"].Count(); k++) //插入R_Station_Action_Para表
//                        {
//                            StationConfig.StationActionParaConfig stationactionpara = new StationConfig.StationActionParaConfig();
//                            stationactionpara.LoginUser = LoginUser;
//                            stationactionpara.AddStationActionPara(iaction["ParaSA"][i], sfcdb);
//                        }
//                    }
//                    //插入 R_Station_Action 表
//                    for (int j = 0; j < Data["input"]["StationActionList"].Count(); j++)
//                    {
//                        JToken saction = Data["input"]["StationActionList"][i];
//                        StationConfig.RStationActionConfig stationaction = new StationConfig.RStationActionConfig();
//                        stationaction.LoginUser = LoginUser;
//                        stationaction.AddStationAction(saction, sfcdb);
//                        for (int k=0;k< saction["ParaSA"].Count();k++) //插入R_Station_Action_Para表
//                        {
//                            StationConfig.StationActionParaConfig stationactionpara = new StationConfig.StationActionParaConfig();
//                            stationactionpara.LoginUser = LoginUser;
//                            stationactionpara.AddStationActionPara(saction["ParaSA"][i], sfcdb);
//                        }
//                    }

//                }
//                sfcdb.CommitTrain();
//                this.DBPools["SFCDB"].Return(sfcdb);

//            }
//            catch (Exception ee)
//            {
//                this.DBPools["SFCDB"].Return(sfcdb);
//                throw ee;
//            }

//}
        void RunAction( )
        {

        }

        void sortAction(List<R_Station_Action> Actions, Dictionary<string,List< R_Station_Action>> dir)
        {
            List<R_Station_Action> Action = Actions.FindAll(t => t.CONFIG_TYPE == "Default");
            Action.Sort();
            dir.Add("Default", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "Customer");
            Action.Sort();
            dir.Add("Customer", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "Series");
            Action.Sort();
            dir.Add("Series", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "Sku");
            Action.Sort();
            dir.Add("Sku", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "WorkerOrder");
            Action.Sort();
            dir.Add("WorkerOrder", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "Line");
            Action.Sort();
            dir.Add("Line", Action);
        }


        //void InitMESStationBase

    }
}
