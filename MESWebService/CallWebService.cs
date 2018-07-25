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

namespace MESWebService
{
    public class CallWebService : MESStation.BaseClass.MesAPIBase
    {
        public CallWebService(OleExecPool SfcDb, OleExecPool ApDb)
        {
            if (!DBPools.ContainsKey("SFCDB"))
            {
                DBPools.Add("SFCDB", SfcDb);
            }
            if (!DBPools.ContainsKey("APDB"))
            {
                DBPools.Add("APDB", ApDb);
            }
        }
        public MESStationReturn InitStation(MESStation.BaseClass.MESStationReturn StationReturn, StationPara sp)
        {
            MESReturnMessage.SetSFCDBPool(this.DBPools["SFCDB"]);
            //string Token = requestValue["Token"]?.ToString();
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            //OleExec SFCDB = new OleExec("VERTIVTESTDB", true);
            try
            {
                MESStationBase retStation = retStation = new MESStationBase();

                retStation.StationOutputs.Clear();
                retStation.StationMessages.Clear();
                retStation.StationSession.Clear();
                retStation.DisplayOutput.Clear();
                retStation.Inputs.Clear();
                retStation.IP = this.IP;
                
                //add by 張官軍 2018-1-4 不添加的話，後面獲取該信息的時候回傳空
                MESStation.LogicObject.User User = new MESStation.LogicObject.User();
                User.EMP_NO = "Webservice";
                User.EMP_NAME = "Webservice";
                retStation.LoginUser = User;
                //給工站對象賦公共值               
                retStation.Init(sp.Station, sp.Line, BU, SFCDB);
                MESStation.MESReturnView.Station.CallStationReturn ret = new MESStation.MESReturnView.Station.CallStationReturn();
                ret.Station = retStation;
                //用以執行InitInput.Run()  2018/01/30 SDL
                retStation.SFCDB = SFCDB;
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
                StationReturn.Message = "Station '" + sp.Station + "'Init successfull.";                
            }
            catch (Exception ee)
            {
                StationReturn.Data = null;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "Station '" + sp.Station + "'Init Fail! "+ee.Message;
                throw ee;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            return StationReturn;
        }

        public void StationInput(MESStationReturn StationReturn,string CurrScanType,string InputName)
        {
            MESStationInput CurrInput = null;
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            OleExec APDB = this.DBPools["APDB"].Borrow();
            MESStation.MESReturnView.Station.CallStationReturn ret = (MESStation.MESReturnView.Station.CallStationReturn)StationReturn.Data;
            MESStationBase Station = ret.Station;
            Station.StationMessages.Clear();
            Station.NextInput = null;
            Station.SFCDB = SFCDB;
            Station.APDB = APDB;           
            Station.ScanKP.Clear();
            try
            {
                CurrInput = Station.Inputs.Find(t=>t.DisplayName== InputName);

                //if (Station.FailStation != null)
                //{
                //    //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 Begin
                //    Station.FailStation.DBS = Station.DBS;
                //    Station.FailStation.SFCDB = SFCDB;
                //    Station.FailStation.APDB = APDB;
                //    //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 End
                //    for (int i = 0; i < Data["Station"]["FailStation"]["Inputs"].Count(); i++)
                //    {

                //        JToken rinput = Data["Station"]["FailStation"]["Inputs"][i];
                //        MESStationInput input = Station.FailStation.Inputs.Find(t => t.DisplayName == rinput["DisplayName"].ToString());
                //        if (input == null)
                //        {
                //            continue;
                //        }
                //        input.Value = rinput["Value"].ToString();
                //        if (Data["ScanType"].ToString() == "Fail" && input.DisplayName == RCurrInput["DisplayName"].ToString())
                //        {
                //            CurrInput = input;
                //        }
                //    }
                //}

                ret = new MESStation.MESReturnView.Station.CallStationReturn();
                ret.ScanType = CurrScanType;
                //add by ZGJ 2018-03-19 清空之前的輸入動作執行後輸出到前台的消息
                CurrInput.Station.StationMessages.Clear();
                //調用處理邏輯
                CurrInput.Run();

                Station.MakeOutput();

                if (ret.ScanType.ToUpper() == "PASS")
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
                else if (Station.FailStation != null)
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

                Station.SFCDB = null;
                Station.APDB = null;

                ret.Station = Station;
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + Station.DisplayName + "' Input successfull.";
            }
            catch (Exception ee)
            {
                Station.MakeOutput();
                Station.SFCDB = null;
                Station.APDB = null;
                ret = new MESStation.MESReturnView.Station.CallStationReturn();
                ret.Station = Station;
                Station.StationMessages.Add(new MESStation.MESReturnView.Station.StationMessage() { Message = ee.Message, State = MESStation.MESReturnView.Station.StationMessageState.Fail });
                Station.NextInput = CurrInput;
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + Station.DisplayName + "' Input not successfull.";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["SFCDB"].Return(APDB);
            }
        }
    }
    public class StationPara
    {
        public string Station;
        public string Line;
    }
}