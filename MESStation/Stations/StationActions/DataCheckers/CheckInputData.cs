using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDataObject;
using MESDataObject.Module;
using MESStation.MESReturnView.Station;
using MESStation.LogicObject;
using System.Text.RegularExpressions;
using MESDBHelper;

namespace MESStation.Stations.StationActions.DataCheckers
{
    class CheckInputData
    {
        /// <summary>
        /// 檢查投入的SN的條碼規則,投入SN可以用L0009加載
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,SN規則檢查，SKU保存的位置</param>
        public static void SNRuleDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            SKU objsku;
            bool SnRuleFlag = false;
            string regexstr = string.Empty;
            string Getnewsn = string.Empty;
            if (Paras.Count != 2)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession Ssku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Ssku == null)
            {
                throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
            }
            else
            {
                if (Ssku.Value != null)
                {
                    objsku = (SKU)Ssku.Value;
                    if (objsku != null)
                    {
                        regexstr = objsku.SnRule;
                    }
                }
                else
                {
                    throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + "" + Paras[0].VALUE + " !");
                }
            }

            MESStationSession Snewsn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Snewsn == null)
            {
                throw new Exception("Can Not Find " + Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY + " !");
            }
            else
            {
                if (Snewsn.Value != null)
                {
                    Getnewsn = Snewsn.Value.ToString();
                }
                else
                {
                    throw new Exception("Can Not Find " + Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY + "" + Paras[1].VALUE + " !");
                }
            }

            if (string.IsNullOrEmpty(regexstr))
            {
                throw new Exception("SnRule Is Null or Empty!");
            }
            else
            {
                SnRuleFlag = Regex.IsMatch(Getnewsn, regexstr);
            }

            if (SnRuleFlag)
            {
                Station.AddMessage("MES00000059", new string[] { Getnewsn }, MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { Getnewsn, regexstr }));
                //Station.AddMessage("MES00000058", new string[] { Getnewsn, regexstr }, MESReturnView.Station.StationMessageState.Fail);
            }


        }

        public static void SNRuleStringDataChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            SKU objSku;
            bool snRuleFlag = true;
            string skuConfigRule = string.Empty;
            string inputSn = string.Empty;
            string[] ruleArray;
            string[] snArray;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSku == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE })); ;
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            try
            {
                objSku = (SKU)sessionSku.Value;
                if (objSku != null)
                {
                    skuConfigRule = objSku.SnRule;
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
                }                

                if (string.IsNullOrEmpty(skuConfigRule))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000251"));
                }
                inputSn = sessionSn.Value.ToString();
                if (inputSn.Length != skuConfigRule.Length)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000252", new string[] { skuConfigRule }));
                }
                char[] charConfigRule = skuConfigRule.ToCharArray();               
                ruleArray = new string[charConfigRule.Length];
                for (int i = 0; i < charConfigRule.Length; i++)
                {
                    ruleArray[i] = charConfigRule[i].ToString();
                }

                char[] charSn = inputSn.ToCharArray();
                snArray = new string[charSn.Length];
                for (int j = 0; j < charSn.Length; j++)
                {
                    snArray[j] = charSn[j].ToString();
                }

                for (int k = 0; k < ruleArray.Length; k++)
                {
                    if (ruleArray[k] == "*")
                    {
                        continue;
                    }
                    if (!ruleArray[k].Equals(snArray[k]))
                    {
                        snRuleFlag = false;
                        break;
                    }
                }

                if (snRuleFlag)
                {
                    Station.AddMessage("MES00000059", new string[] { inputSn }, MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { inputSn, skuConfigRule }));
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 1.檢查SN 在r_repair_failcode 表中是否已经维修完成
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void SNFailCodeReapirDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession SNFailCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNFailCodeSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (SNFailCodeSession.Value != null)
                {
                    FailCodeRow = RepairFailcode.GetByFailCodeID(SNFailCodeSession.Value.ToString(), Station.SFCDB);
                    if (FailCodeRow == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000192", new string[] {  }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].VALUE }));
                }
            }


        }

        /// <summary>
        /// 1.檢查输入的Location位置是否是在数据库中存在，不存在则报错
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void ReapirLocationDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec apdb = null;
            List<string> LocationList = new List<string>();

            if (Paras.Count != 2)
            {
                throw new Exception("參數數量不正確!");
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;

            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LocationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string StrLocation = LocationSession.Value.ToString();

            //獲取ALLPART數據
            AP_DLL APDLL = new AP_DLL();
            MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "Location");
            List<object> ret = I.DataForUse;
            ret.Clear();
            try
            {
                apdb = Station.DBS["APDB"].Borrow();
                LocationList = APDLL.CheckLocationExist(ObjSN.SkuNo, StrLocation, apdb);
                if (LocationList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { StrLocation,ObjSN.SkuNo }));
                }
                else
                {
                    foreach (object item in LocationList)
                    {
                        ret.Add(item);
                    }
                }
                Station.DBS["APDB"].Return(apdb);
                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    Station.DBS["APDB"].Return(apdb);
                }
                throw ex;
            }

        }

        /// <summary>
        /// 1.檢查工單是否可以進行SMTloading/SIloading.只有正常工單可以進行.
        /// 2.工單不能R_WO_BASE.Inputqty >= R_WO_BASE.Workorderqty
        /// 3.Check工單不能關結(R_WO_BASE.Closed != 1)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void WoLoadingStationDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            WorkOrder ObjWorkorder = new WorkOrder();
            string ErrMessage = string.Empty;            

            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
            }
            else
            {
                if (Swo.Value != null)
                {
                    ObjWorkorder = (WorkOrder)Swo.Value;
                    if (ObjWorkorder != null)
                    {
                        if (ObjWorkorder.WORKORDER_QTY <= ObjWorkorder.INPUT_QTY)
                        {
                            //Station.AddMessage("MES00000060", new string[] { ObjWorkorder.WorkorderNo.ToString(),ObjWorkorder.WORKORDER_QTY.ToString(), ObjWorkorder.INPUT_QTY.ToString() }, MESReturnView.Station.StationMessageState.Fail);
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000060",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString(),
                                        ObjWorkorder.WORKORDER_QTY.ToString(),
                                        ObjWorkorder.INPUT_QTY.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        if (ObjWorkorder.CLOSED_FLAG != "0")
                        {
                            //Station.AddMessage("MES00000041", new string[] { ObjWorkorder.WorkorderNo.ToString() }, MESReturnView.Station.StationMessageState.Fail);
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000041",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        Station.AddMessage("MES00000061", new string[] { ObjWorkorder.WorkorderNo.ToString() }, MESReturnView.Station.StationMessageState.Pass);
                    }
                }
                else
                {
                    throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + "" + Paras[0].VALUE + " !");
                }
            }


        }

        /// <summary>
        /// 1.工單不能R_WO_BASE.Inputqty >= R_WO_BASE.Workorderqty
        /// 2.Check工單狀態必須正確Closed=0,Release_date is not null
        /// 3.檢查工單是否可以進行SMTloading.只有路由起始站為SMTLoading可以進行.
        /// 4.工單不能被鎖定、工單料號不能被鎖定--?還未實現該功能
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void SMTLoadingWoStationDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            WorkOrder ObjWorkorder = new WorkOrder();
            string ErrMessage = string.Empty;
            string ParaStation = "SMTLOADING";

            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
            }
            else
            {
                if (!string.IsNullOrEmpty(Paras[0].VALUE))
                {
                    ParaStation = Paras[0].VALUE.ToString();
                }
                if (Swo.Value != null)
                {
                    ObjWorkorder = (WorkOrder)Swo.Value;
                    ObjWorkorder.Init(Swo.Value.ToString(), Station.SFCDB);
                    if (ObjWorkorder != null)
                    {
                        //1.工單不能R_WO_BASE.Inputqty >= R_WO_BASE.Workorderqty
                        if (ObjWorkorder.WORKORDER_QTY <= ObjWorkorder.INPUT_QTY)
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000060",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString(),
                                        ObjWorkorder.WORKORDER_QTY.ToString(),
                                        ObjWorkorder.INPUT_QTY.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        //2.Check工單狀態
                        if (ObjWorkorder.CLOSED_FLAG == "1")
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000041",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        if (ObjWorkorder.RELEASE_DATE == null)
                        {
                            Station.AddMessage("MES00000042", new string[] { "WO:" + ObjWorkorder.WorkorderNo }, StationMessageState.Fail);
                            return;
                        }
                        //add by LLF 2018-03-20
                        if (ObjWorkorder.ROUTE.DETAIL == null)
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000194",
                                   new string[] { ObjWorkorder.WorkorderNo.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        //Modify by LLF 2018-03-20
                        // 3.檢查工單是否可以進行SMTloading.只有路由起始站為SMTLoading可以進行.
                        if (ObjWorkorder.ROUTE.DETAIL[0].STATION_NAME != ParaStation)
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000112",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString(), ParaStation });
                            throw new MESReturnMessage(ErrMessage);

                        }
                        // 4.工單不能被鎖定、工單料號不能被鎖定--?還未實現該功能
                        Station.AddMessage("MES00000061", new string[] { ObjWorkorder.WorkorderNo.ToString() }, MESReturnView.Station.StationMessageState.Pass);
                    }
                }
                else
                {
                    throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + "" + Paras[0].VALUE + " !");
                }
            }
        }

        public static void SMTLoadingWoInputQtyDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            WorkOrder ObjWorkorder = new WorkOrder();
            string ErrMessage = string.Empty;
            int IntLinkQty = 0;
            string ParaStation = "SMTLOADING"; //add by LLF 2018-03-20

            if (Paras.Count <= 0)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (Swo == null)
            {
                throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
            }

            MESStationSession Linkqty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Linkqty != null)
            {
                IntLinkQty = Convert.ToInt16(Linkqty.Value.ToString());
            }

            if (Swo.Value != null)
            {
                //add by LLF 2018-03-28
                if (!string.IsNullOrEmpty(Paras[0].VALUE))
                {
                    ParaStation = Paras[0].VALUE.ToString();
                }
                //ObjWorkorder = (WorkOrder)Swo.Value;
                ObjWorkorder.Init(Swo.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (ObjWorkorder != null)
                {
                    //1.工單不能R_WO_BASE.Inputqty >= R_WO_BASE.Workorderqty
                    if (ObjWorkorder.WORKORDER_QTY < ObjWorkorder.INPUT_QTY + IntLinkQty)
                    {
                        Station.Inputs[3].Value = ""; //add by LLF 2018-01-26 
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000134",
                                new string[] { ObjWorkorder.WorkorderNo.ToString(),
                                    ObjWorkorder.INPUT_QTY.ToString(),
                                    IntLinkQty.ToString(),
                                    ObjWorkorder.WORKORDER_QTY.ToString()
                                     });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    //2.Check工單狀態
                    if (ObjWorkorder.CLOSED_FLAG == "1")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000041",
                                new string[] { ObjWorkorder.WorkorderNo.ToString() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    if (ObjWorkorder.RELEASE_DATE == null)
                    {
                        Station.AddMessage("MES00000042", new string[] { "WO:" + ObjWorkorder.WorkorderNo }, StationMessageState.Fail);
                        return;
                    }
                    //add by LLF 2018-03-20
                    if (ObjWorkorder.ROUTE.DETAIL == null)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000194",
                               new string[] { ObjWorkorder.WorkorderNo.ToString() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    // 3.檢查工單是否可以進行SMTloading.只有路由起始站為SMTLoading可以進行.
                    if (ObjWorkorder.ROUTE.DETAIL[0].STATION_NAME != ParaStation)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000112",
                                new string[] { ObjWorkorder.WorkorderNo.ToString(), ParaStation });
                        throw new MESReturnMessage(ErrMessage);

                    }
                    // 4.工單不能被鎖定、工單料號不能被鎖定--?還未實現該功能
                    Station.AddMessage("MES00000061", new string[] { ObjWorkorder.WorkorderNo.ToString() }, MESReturnView.Station.StationMessageState.Pass);
                }

            }
        }

        public static void RouteDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            List<R_MRB> GetMRBList = new List<R_MRB>();
            R_MRB New_R_MRB = new R_MRB();
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);

            if (Paras.Count != 3)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            } else if (SnSession.Value==null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].VALUE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SN ObjSn = (SN)SnSession.Value;

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WorkOrder ObjWo = (WorkOrder)WoSession.Value;

            MESStationSession StationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StationSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            else if (StationSession.Value == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].VALUE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            string NextStation = StationSession.Value.ToString();

            try
            {
                GetMRBList = TR_MRB.GetMrbInformationBySN(ObjSn.SerialNo, Station.SFCDB);

                if (GetMRBList==null|| GetMRBList.Count==0)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "R_MRB:" +ObjSn.SerialNo });
                    throw new MESReturnMessage(ErrMessage);
                }
                //MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "StationName");
                //List<object> snStationList = I.DataForUse;
                //snStationList.Clear();
                //snStationList.Add(""); ///BY SDL  加載頁面默認賦予空值,操作員必須點選其他有內容選項

                Route routeDetail = new Route(ObjWo.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<RouteDetail> routeDetailList = routeDetail.DETAIL;
                RouteDetail h = routeDetailList.Find(t => t.STATION_NAME == GetMRBList[0].NEXT_STATION || t.STATION_TYPE == GetMRBList[0].NEXT_STATION);
                if (h == null)   //R_MRB next_station欄位的值
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000205", new string[] { ObjSn.SerialNo, ObjWo.WorkorderNo });
                    throw new MESReturnMessage(ErrMessage);
                }

                RouteDetail g = routeDetailList.Find(t => t.STATION_NAME == NextStation);
                if (g == null)  //REWORK選擇的要打回工站
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { NextStation });
                    throw new MESReturnMessage(ErrMessage);
                }

                if (g.SEQ_NO>h.SEQ_NO)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000211", new string[] {  });
                    throw new MESReturnMessage(ErrMessage);
                }
                

                Station.AddMessage("MES00000026", new string[] {  }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 檢查輸入的Action_code是否存在
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ActionCodeDataChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {           
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string ActionCodeInput = "";
            MESStationSession sessionActionCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);           
            if (sessionActionCode == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionActionCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            try
            {
                T_C_ACTION_CODE t_c_action_code = new T_C_ACTION_CODE(Station.SFCDB, Station.DBType);
                C_ACTION_CODE c_action_code = new C_ACTION_CODE();
                c_action_code = t_c_action_code.GetByActionCode(sessionActionCode.Value.ToString(), Station.SFCDB);
                if (c_action_code==null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "ActionCode", ActionCodeInput }));
                }
                Station.AddMessage("MES00000026", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}

