using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESStation.HateEmsGetDataService;
using System.Net;
using System.Text.RegularExpressions;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckSN
    {
        /// <summary>
        /// check PanelSn ,長度是否等於8，並且以PN開頭，是否已經投入使用
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelSNInputRuleChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string PanelSNNO = Input.Value.ToString();
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            PanelSNNO = Input.Value.ToString();
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                s.Value = PanelSNNO;
                s.InputValue = PanelSNNO;
                s.ResetInput = Input;
                Station.StationSession.Add(s);
            }
            else
            {
                s.InputValue = PanelSNNO;
                s.ResetInput = Input;
                s.Value = PanelSNNO;
            }
            if (PanelSNNO.Length == 8 && ((Station.BU == "HWD" && PanelSNNO.Substring(0, 2) == "PN") || (Station.BU == "VERTIV" && PanelSNNO.Substring(0, 2) == "PV")))
            {
                T_R_PANEL_SN TR_PanelSN = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
                ////判斷是否投入使用                
                if (TR_PanelSN.CheckPanelExist(PanelSNNO, Station.SFCDB))
                {
                    //Station.AddMessage("MES00000040", new string[] { "PanelSN:" + PanelSNNO }, MESReturnView.Station.StationMessageState.Fail);
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { "PanelSN:" + PanelSNNO }));
                }
                else
                {
                    Station.AddMessage("MES00000029", new string[] { "PanelSN", PanelSNNO }, MESReturnView.Station.StationMessageState.Pass);
                }
            }
            else
            {
                //Station.AddMessage("MES00000022", new string[] { "PanelSN:" + PanelSNNO }, MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000022", new string[] { "PanelSN:" + PanelSNNO }));
            }
        }
        //SN狀態檢查
        public static void SNInputStatusChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            WorkOrder Wo;
            //int Linkqty;
            if (Paras.Count == 0)
            {
                throw new Exception("參數數量不正確!");
            }

            MESStationSession SNLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNLoadPoint == null)
            {
                SNLoadPoint = new MESStationSession() { MESDataType = "SN", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(SNLoadPoint);
            }

            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }

            //MESStationSession LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            //if (LinkQty == null)
            //{
            //    throw new Exception("RepairFlag=1 error!");
            //}
            //Linkqty = Convert.ToUInt16(LinkQty.Value.ToString());

            //SNLoadPoint.Value = "";
            //Station.StationSession.Add(SNLoadPoint);
            string snStr = Input.Value.ToString();
            SN sn = new SN(snStr, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            if (sn.RepairFailedFlag == 1.ToString())
            {
                throw new Exception("RepairFlag=1 error!");
            }
            if (sn.CompletedFlag == 1.ToString())
            {
                throw new Exception("CompleteFlag=1 error!");
            }
            if (sn.PackedFlag == 1.ToString())
            {
                throw new Exception("PackFlag=1 error!");
            }
            if (sn.ShippedFlag == 1.ToString())
            {
                throw new Exception("ShipFlag=1 error!");
            }
            if (sn.CurrentStation == "MRB")
            {
                throw new Exception("MRB error!");
            }
            //if (Wo.INPUT_QTY + Linkqty > Wo.WORKORDER_QTY)
            //{
            //    throw new Exception("Input_qty  + Linkqty > WORKORDER_QTY !");
            //}
            Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
        }
        /// <summary>
        /// 檢查投入SN是否重碼,　SN 不能存在R_SN Table中,,PanelSN不能存在r_panel_sn table中
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:"WO",SESSION_KEY:"1",VALUE:""}</param>
        public static void InputSNDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strNewSN;
            bool isUsed = false;
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            strNewSN = Input.Value.ToString();
            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = strNewSN;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = strNewSN;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                InputSN_Session.Value = strNewSN;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = strNewSN;
            }

            if (Paras[1].VALUE.ToString() == "0")//0是SN
            {
                T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
                isUsed = TR_SN.IsUsed(strNewSN, Station.SFCDB);
            }
            else if (Paras[1].VALUE.ToString() == "1")//1是PanelSN
            {
                T_R_PANEL_SN TR_PANEL_SN = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
                isUsed = TR_PANEL_SN.CheckPanelExist(strNewSN, Station.SFCDB);
            }
            else
            {
                //throw new Exception("SNType is undefined !");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000020", new string[] { "SNType", "0/1" }));
            }

            if (isUsed)
            {
                //Station.AddMessage("MES00000040", new string[] { strNewSN }, MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { strNewSN }));
            }
            else
            {
                Station.AddMessage("MES00000001", new string[] { strNewSN }, MESReturnView.Station.StationMessageState.Pass);
                //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { strNewSN }));
            }
        }

        public static void CheckDuplicateByInputSN(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strNewSN;
            bool isUsed = false;
            // int snType = 0;//0是SN，1是PanelSN
            MESStationSession NewSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (NewSnSession != null)
            {
                Station.StationSession.Remove(NewSnSession);
            }
            NewSnSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = Input.Value.ToString() };
            Station.StationSession.Add(NewSnSession);

            strNewSN = NewSnSession.Value.ToString();

            if (Paras[1].VALUE.ToString() == "0")//0是SN
            {
                T_R_SN TR_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                isUsed = TR_SN.IsUsed(strNewSN, Station.SFCDB);
            }
            else if (Paras[1].VALUE.ToString() == "1")//1是PanelSN
            {
                T_R_PANEL_SN TR_PANEL_SN = new T_R_PANEL_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                isUsed = TR_PANEL_SN.CheckPanelExist(strNewSN, Station.SFCDB);
            }
            else
            {
                throw new Exception("SNType is undefined !");
            }

            if (isUsed)
            {
                //Station.AddMessage("MES00000040", new string[] { strNewSN }, MESReturnView.Station.StationMessageState.Fail);
                string ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { strNewSN });
                throw new MESReturnMessage(ErrMessage);
            }
            else
            {
                Station.AddMessage("MES00000001", new string[] { strNewSN }, MESReturnView.Station.StationMessageState.Pass);
            }
        }
        /// <summary>
        /// 檢查投入SN是否處於工單區間,SN 要在工單的SN區間中(R_WO_Region)
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:" ",SSION_KEY:"",VALUE:"0"}</param>
        public static void WoSNRangeDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strNewSN = "";
            WorkOrder wo;
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            strNewSN = Input.Value.ToString();
            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = strNewSN;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                InputSN_Session.Value = strNewSN;
                InputSN_Session.InputValue = strNewSN;
                InputSN_Session.ResetInput = Input;
            }
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    wo = (WorkOrder)Wo_Session.Value;
                    if (wo.WorkorderNo == null || wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            T_R_WO_REGION TR_WO_REGION = new T_R_WO_REGION(Station.SFCDB, Station.DBType);
            bool isInWoRange = TR_WO_REGION.CheckSNInWoRange(strNewSN, wo.WorkorderNo, Station.SFCDB);
            if (isInWoRange)
            {
                Station.AddMessage("MES00000001", new string[] { strNewSN }, MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                Station.AddMessage("MES00000056", new string[] { strNewSN, wo.WorkorderNo }, MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000056", new string[] { strNewSN, wo.WorkorderNo }));
            }
        }

        /// <summary>
        /// 1.序號長度必須相同，且起始序號必須小于結束序號
        /// 2.序號必須在工單區間管控範圍內(R_WO_Region)
        /// 3.本次Loading的序號不能已經存在
        /// 2018/1/23 Rain
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:" ",SSION_KEY:"",VALUE:"0"}</param>
        public static void WoLotSNRangeDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strNewSN = "";
            string strStartSN = "";
            string strEndSN = "";
            WorkOrder Wo;

            if (Paras.Count != 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //工單必須存在
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_KEY + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }

            //Start SN必須加載
            MESStationSession StartSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StartSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (StartSN_Session.Value != null)
                {
                    strStartSN = StartSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            //End SN必須加載
            MESStationSession EndSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (EndSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                if (EndSN_Session.Value != null)
                {
                    strEndSN = EndSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
            }
            //1.序號長度必須相同，且起始序號必須小于結束序號
            if (strStartSN.Length != strEndSN.Length)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000117", new string[] { strStartSN, strEndSN }));
            }

            if (String.Compare(strStartSN, strEndSN) == 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000118", new string[] { strStartSN, strEndSN }));
            }
            //2.序號必須在工單區間管控範圍內(R_WO_Region)
            T_R_WO_REGION TR_WO_REGION = new T_R_WO_REGION(Station.SFCDB, Station.DBType);
            bool isInWoRange = TR_WO_REGION.CheckLotSNInWoRange(strStartSN, strEndSN, Wo.WorkorderNo, Station.SFCDB);
            if (isInWoRange == false)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000119", new string[] { strStartSN, strEndSN, Wo.WorkorderNo }));
            }
            // 3.本次Loading的序號不能已經存在
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            bool RangeIsUsed = TR_SN.SNRangeIsUsed(strStartSN, strEndSN, Station.SFCDB);
            if (RangeIsUsed == true)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000120", new string[] { strStartSN, strEndSN }));
            }

        }

        /// <summary>
        /// 1.判斷條碼后4位是否符合34進制編碼規則
        /// 2.依據條碼后4位34進制編碼規則算出區間內有多少個SN:Var_Qty
        /// 3.本次Loading數量不能超過工單未Loading的總數量
        /// 2018/1/24 Rain
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:" ",SSION_KEY:"",VALUE:"0"}</param>
        public static void SNRange34HQtyDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strStartSN = "";
            string strEndSN = "";
            WorkOrder Wo;

            if (Paras.Count != 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //工單必須存在
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }

            //Start SN必須加載
            MESStationSession StartSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StartSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (StartSN_Session.Value != null)
                {
                    strStartSN = StartSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            //End SN必須加載
            MESStationSession EndSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (EndSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                if (EndSN_Session.Value != null)
                {
                    strEndSN = EndSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
            }
            //1.判斷條碼后4位是否符合34進制編碼規則
            bool SnRuleFlag = Regex.IsMatch(strStartSN.Substring(strStartSN.Length - 4, 4), "[0-9A-HJ-MP-Z]{4}");
            if (SnRuleFlag == false)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000121", new string[] { strStartSN, "[0-9A-HJ-MP-Z]{4}" }));
            }

            SnRuleFlag = Regex.IsMatch(strEndSN.Substring(strEndSN.Length - 4, 4), "[0-9A-HJ-MP-Z]{4}");
            if (SnRuleFlag == false)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000121", new string[] { strEndSN, "[0-9A-HJ-MP-Z]{4}" }));
            }

            if (strStartSN.Substring(0, strStartSN.Length - 4) != strEndSN.Substring(0, strEndSN.Length - 4))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000126", new string[] { strStartSN, strEndSN }));
            }

            //2.依據條碼后4位34進制編碼規則算出區間內有多少個SN:Var_Qty
            T_R_WO_REGION TR_WO_REGION = new T_R_WO_REGION(Station.SFCDB, Station.DBType);
            int SnRangQty = TR_WO_REGION.GetQtyBy34HSNRange(strStartSN.Substring(strStartSN.Length - 4, 4), strEndSN.Substring(strEndSN.Length - 4, 4), Station.SFCDB);
            if (SnRangQty <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000123", new string[] { strStartSN, strEndSN }));
            }

            // 3.本次Loading數量不能超過工單未Loading的總數量
            if (Wo.WORKORDER_QTY < Wo.INPUT_QTY + SnRangQty)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000124", new string[] { SnRangQty.ToString(), (Wo.WORKORDER_QTY - Wo.INPUT_QTY).ToString() }));
            }
            //3.1 同時加載進制及數量
            MESStationSession DecimalType = Station.StationSession.Find(t => t.MESDataType == "DecimalType" && t.SessionKey == "1");
            if (DecimalType == null)
            {
                DecimalType = new MESStationSession() { MESDataType = "DecimalType", SessionKey = "1", ResetInput = Input, InputValue = "34H" };
                Station.StationSession.Add(DecimalType);
            }
            DecimalType.Value = "34H";

            MESStationSession LotQty = Station.StationSession.Find(t => t.MESDataType == "LotQty" && t.SessionKey == "1");
            if (LotQty == null)
            {
                LotQty = new MESStationSession() { MESDataType = "LotQty", SessionKey = "1", ResetInput = Input, InputValue = SnRangQty.ToString() };
                Station.StationSession.Add(LotQty);
            }
            LotQty.Value = SnRangQty.ToString();
        }

        /// <summary>
        /// 1.判斷條碼后4位是否符合10進制編碼規則
        /// 2.依據條碼后4位10進制編碼規則算出區間內有多少個SN:Var_Qty
        /// 3.本次Loading數量不能超過工單未Loading的總數量
        /// 2018/1/25 Rain
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">{SESSION_TYPE:"NEWSN",SSION_KEY:"1",VALUE:""}{SESSION_TYPE:" ",SSION_KEY:"",VALUE:"0"}</param>
        public static void SNRange10HQtyDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strStartSN = "";
            string strEndSN = "";
            WorkOrder Wo;

            if (Paras.Count != 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //工單必須存在
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value != null)
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }

            //Start SN必須加載
            MESStationSession StartSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StartSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (StartSN_Session.Value != null)
                {
                    strStartSN = StartSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            //End SN必須加載
            MESStationSession EndSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (EndSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                if (EndSN_Session.Value != null)
                {
                    strEndSN = EndSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
            }
            //1.判斷條碼后4位是否符合10進制編碼規則
            bool SnRuleFlag = Regex.IsMatch(strStartSN.Substring(strStartSN.Length - 4, 4), "[0-9]{4}");
            if (SnRuleFlag == false)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000121", new string[] { strStartSN, "[0-9]{4}" }));
            }

            SnRuleFlag = Regex.IsMatch(strEndSN.Substring(strEndSN.Length - 4, 4), "[0-9]{4}");
            if (SnRuleFlag == false)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000121", new string[] { strEndSN, "[0-9]{4}" }));
            }

            if (strStartSN.Substring(0, strStartSN.Length - 4) != strEndSN.Substring(0, strEndSN.Length - 4))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000126", new string[] { strStartSN, strEndSN }));
            }

            //2.依據條碼后4位10進制編碼規則算出區間內有多少個SN:Var_Qty
            int SnRangQty = Convert.ToInt32(strEndSN.Substring(strEndSN.Length - 4, 4)) - Convert.ToInt32(strStartSN.Substring(strStartSN.Length - 4, 4)) + 1;
            if (SnRangQty <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000123", new string[] { strStartSN, strEndSN }));
            }
            // 3.本次Loading數量不能超過工單未Loading的總數量
            if (Wo.WORKORDER_QTY < Wo.INPUT_QTY + SnRangQty)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000124", new string[] { SnRangQty.ToString(), (Wo.WORKORDER_QTY - Wo.INPUT_QTY).ToString() }));
            }
            //3.1 同時加載進制及數量
            MESStationSession DecimalType = Station.StationSession.Find(t => t.MESDataType == "DecimalType" && t.SessionKey == "1");
            if (DecimalType == null)
            {
                DecimalType = new MESStationSession() { MESDataType = "DecimalType", SessionKey = "1", ResetInput = Input, InputValue = "10H" };
                Station.StationSession.Add(DecimalType);
            }
            DecimalType.Value = "10H";

            MESStationSession LotQty = Station.StationSession.Find(t => t.MESDataType == "LotQty" && t.SessionKey == "1");
            if (LotQty == null)
            {
                LotQty = new MESStationSession() { MESDataType = "LotQty", SessionKey = "1", ResetInput = Input, InputValue = SnRangQty.ToString() };
                Station.StationSession.Add(LotQty);
            }
            LotQty.Value = SnRangQty.ToString();

        }

        /// <summary>
        /// 當前狀態不能為MRB
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNMrbchecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            if (Paras.Count != 2)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "Serial Number" });
                throw new MESReturnMessage(ErrMessage);
            }
            SN ObjSN = (SN)SessionSN.Value;

            if (ObjSN.CurrentStation == "MRB")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000066",
                                new string[] { ObjSN.SerialNo, ObjSN.CurrentStation });
                throw new MESReturnMessage(ErrMessage);
            }


            //add by 張官軍  2018-03-15
            //增加檢查邏輯
            //加载 session 中的工单
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "Work Order" }));
            }
            var wo_session = WoSession.Value;
            WorkOrder wo = new WorkOrder();
            if (wo_session is string)
            {
                wo.Init(wo_session.ToString(), Station.SFCDB);
            }
            else if (wo_session is WorkOrder)
            {
                wo = (WorkOrder)WoSession.Value;
            }


            //判斷物料對應的機種是否存在
            T_C_SKU SkuTable = new T_C_SKU(Station.SFCDB, Station.DBType);
            if (SkuTable.GetSkuBySkuno(ObjSN.SkuNo, Station.SFCDB) == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000172", new string[] { wo.SkuNO, wo.SKU_VER });
                throw new MESReturnMessage(ErrMessage);
            }

            ////判斷該工單是否有該物料的需求
            //T_R_WO_ITEM ItemTable = new T_R_WO_ITEM(Station.SFCDB, Station.DBType);
            //if (!ItemTable.CheckExist(wo.WorkorderNo, ObjSN.SkuNo, Station.SFCDB))
            //{
            //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000196", new string[] { wo.WorkorderNo, ObjSN.SkuNo });
            //    throw new MESReturnMessage(ErrMessage);
            //}

            T_R_MRB MrbTable = new T_R_MRB(Station.SFCDB, Station.DBType);
            //判斷物料SN是否有MRB過	
            if (MrbTable.HadMrbed(ObjSN.SerialNo, Station.SFCDB))
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000182", new string[] { ObjSN.SerialNo });
                throw new MESReturnMessage(ErrMessage);
            }

            //首先查看全局開關，是否要做後續檢測
            //如果有，則判斷該機種是否有設定不需要檢查 TS101 站位測試記錄
            //如果沒有設定就再判斷是否有報廢
            //如果也沒有，則判斷在 HW 測試系統中是否有測試 TS101 或者配置在 SFCCODELIKEDETAIL 中的站位的測試記錄
            if (MrbTable.GetMRBControl(Station.SFCDB))    //查看全局開關
            {
                if (!MrbTable.SkuCheckTS101(ObjSN.SkuNo, Station.SFCDB))    //判斷是否需要檢查 TS101
                {
                    if (!MrbTable.IsPreScrap(ObjSN.SerialNo, Station.SFCDB))    //判斷是否有報廢
                    {
                        if (!MrbTable.HasHWTest(ObjSN.SerialNo, Station.SFCDB))    //判斷是否有測試記錄
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000181", new string[] { ObjSN.SerialNo });
                            throw new MESReturnMessage(ErrMessage);
                        }
                    }
                }
            }

            Station.AddMessage("MES00000067", new string[] { ObjSN.SerialNo }, MESReturnView.Station.StationMessageState.Pass);


        }

        /// <summary>
        /// 檢查工單需求里是否包含SN的料號
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNMrbWoRequest(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "Serial Number" });
                throw new MESReturnMessage(ErrMessage);
            }
            SN ObjSN = (SN)SessionSN.Value;

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "Work Order" }));
            }
            var wo_session = WoSession.Value;
            WorkOrder wo = new WorkOrder();
            if (wo_session is string)
            {
                wo.Init(wo_session.ToString(), Station.SFCDB);
            }
            else if (wo_session is WorkOrder)
            {
                wo = (WorkOrder)WoSession.Value;
            }
            //判斷該工單是否有該物料的需求
            T_R_WO_ITEM ItemTable = new T_R_WO_ITEM(Station.SFCDB, Station.DBType);
            if (!ItemTable.CheckExist(wo.WorkorderNo, ObjSN.SkuNo, Station.SFCDB))
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000196", new string[] { wo.WorkorderNo, ObjSN.SkuNo });
                throw new MESReturnMessage(ErrMessage);
            }
        }





        /// <summary>
        /// 產品是Fail Check
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFailchecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string RepairFlag = string.Empty;
            T_R_REPAIR_MAIN Grepair = new T_R_REPAIR_MAIN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_MAIN> RepairMainList = new List<R_REPAIR_MAIN>();

            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                throw new Exception("请输入SN!");
            }
            else
            {
                SN ObjSN = (SN)SessionSN.Value;
                if (ObjSN.RepairFailedFlag == "1")  /// R_SN  RepairFailedFlag  欄位 1表示fail  0表示pass 
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000071",
                                    new string[] { ObjSN.SerialNo });
                    throw new MESReturnMessage(ErrMessage);
                }
                RepairMainList = Grepair.GetRepairMainBySN(Station.SFCDB, ObjSN.SerialNo);

                if (RepairMainList.Count != 0)
                {
                    if (RepairMainList[0].CLOSED_FLAG == "0")    /// R_REPAIR_MAIN  close_flag  欄位 1表示pass  0表示fail
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000071",
                                       new string[] { ObjSN.SerialNo });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }

                Station.AddMessage("MES00000067", new string[] { ObjSN.SerialNo }, MESReturnView.Station.StationMessageState.Pass);

            }
        }

        /// <summary>
        /// 產品LOT Status狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNLotStatuschecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string RepairFlag = string.Empty;
            LotNo ObjLot = new LotNo();
            Row_R_LOT_STATUS GetLotNo;
            T_R_LOT_DETAIL RowLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_LOT_STATUS RowLotStatus = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            if (Paras.Count <= 0)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession SessionNewLotFlag = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            if (SessionNewLotFlag == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if (SessionSN == null)
            {
                throw new Exception("请输入SN!");
            }
            else
            {
                SN ObjSN = (SN)SessionSN.Value;

                GetLotNo = RowLotStatus.GetLotBySN(ObjSN.SerialNo, Station.SFCDB);
                if (GetLotNo != null)
                {
                    //modify by LLF 2018-02-22 
                    //ObjLot.Init(GetLotNo.LOT_NO, Station.SFCDB);
                    ObjLot.Init(GetLotNo.LOT_NO, ObjSN.SerialNo, Station.SFCDB);

                    if (ObjLot.CLOSED_FLAG == "0") ///該批次未被關閉，系統報錯產品已入批次
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000080",
                                    new string[] { ObjSN.SerialNo, ObjLot.LOT_NO });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    //Marked by LLF 2018-02-07
                    //if (ObjSN.NextStation == "SMT_FQC")///SN待過工站不為SMT_FQC, 系統報錯產品不需要掃入批次
                    //{
                    //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000066",
                    //                new string[] { ObjSN.SerialNo, ObjSN.NextStation });
                    //    throw new MESReturnMessage(ErrMessage);
                    //}
                    if (ObjLot.CLOSED_FLAG == "1" && ObjLot.LOT_STATUS_FLAG == "0") ///該批次關閉,且LotStatusFlag 为0，系統報錯產品已入批次，处于待抽驗狀態
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000160",
                                    new string[] { ObjSN.SerialNo, ObjLot.LOT_NO });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    if (ObjLot.SAMPLING == "4")//SN是狀態為：R_LOT_DETAIL.Sampling=4,則報錯，需解鎖,
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000081",
                                    new string[] { ObjSN.SerialNo });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }

                Station.AddMessage("MES00000067", new string[] { ObjSN.SerialNo }, MESReturnView.Station.StationMessageState.Pass);
            }
        }



        /// <summary>
        /// SN產品LOT Status狀態是否為鎖定狀態
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFQCLotLockchecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string SnOrLotno = string.Empty;
            string Lotno = "";
            LotNo ObjLot = new LotNo();
            Row_R_LOT_STATUS GetLotNoBySN;
            Row_R_LOT_STATUS GetLotNoByLot;
            T_R_LOT_DETAIL RowLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_LOT_STATUS RowLotStatus = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            if (Paras.Count <= 0)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession SessionSNorLotNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSNorLotNo == null)
            {
                //throw new Exception("请输入SN !");
                SessionSNorLotNo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionSNorLotNo);
            }
            SnOrLotno = Input.Value.ToString();

            GetLotNoBySN = RowLotStatus.GetLotBySN(SnOrLotno, Station.SFCDB);
            GetLotNoByLot = RowLotStatus.GetByInput(SnOrLotno, "LOT_NO", Station.SFCDB);
            //add by LLF 2018-02-19
            if (GetLotNoBySN == null && GetLotNoByLot == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000143",
                                new string[] { SnOrLotno });
                throw new MESReturnMessage(ErrMessage);
            }

            //add by LLF 2018-02-19

            if (GetLotNoBySN != null)  ///COUNT=0 說明輸入的是LOTNO號，否則輸入的為SN
            {
                //modify by LLF 2018-02-22
                //ObjLot.Init(GetLotNo.LOT_NO, Station.SFCDB);
                ObjLot.Init(GetLotNoBySN.LOT_NO, SnOrLotno, Station.SFCDB);

                if (ObjLot.CLOSED_FLAG == "0") ///該批次未被關閉
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000080",
                                new string[] { SnOrLotno, ObjLot.LOT_NO });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (ObjLot.SAMPLING != "4")//該SN  R_LOT_DETAIL SAMPLING=4 處於鎖定狀態
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000114",
                                new string[] { SnOrLotno });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                //modify by LLF 2018-02-22
                //ObjLot.Init(SnOrLotno,"", Station.SFCDB);
                //add by LLF 2018-02-19
                if (GetLotNoByLot.CLOSED_FLAG == "0") ///該批次未被關閉，系統報錯產品已入批次
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000080",
                                new string[] { SnOrLotno, ObjLot.LOT_NO });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (!RowLotDetail.CheckLotNoDetailStatus(GetLotNoByLot.ID, Station.SFCDB))
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000114",
                                new string[] { SnOrLotno });
                    throw new MESReturnMessage(ErrMessage);
                }

            }

            SessionSNorLotNo.Value = SnOrLotno;
            Station.AddMessage("MES00000067", new string[] { SnOrLotno }, MESReturnView.Station.StationMessageState.Pass);
        }




        /// <summary>
        /// 產品掃描Rework SN Check
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// 1.	當前狀態要為MRB
        ///2.	SN 工單不能與當前重工工單一致
        public static void SNReworkchecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            //string Sn = "";
            //string Sn_Satation = "";
            //string Sn_Wo = "";
            //MESStationSession SnInput = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");
            //if (SnInput == null)
            //{
            //    //Station.AddMessage("MES00000076", new string[] { "Sn", Sn }, MESReturnView.Station.StationMessageState.Fail);
            //}
            //else
            //{
            //    SN ObjSn = new SN();
            //    ObjSn = (SN)SnInput.Value;
            //    Sn = ObjSn.SerialNo;
            //    Sn_Satation = ObjSn.CurrentStation;
            //    Sn_Wo = ObjSn.WorkorderNo;
            //}
            string Rework_WO = "";
            string Sn = Input.Value.ToString();
            T_R_SN R_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string Sn_Satation = R_SN.GetDetailBySN(Sn, Station.SFCDB).CURRENT_STATION;
            string Sn_Wo = R_SN.GetDetailBySN(Sn, Station.SFCDB).WORKORDERNO;
            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoInput == null)
            {
                Station.AddMessage("MES00000050", new string[] { }, MESReturnView.Station.StationMessageState.Fail);
                return;
            }
            else
            {
                WorkOrder ObjWo = new WorkOrder();
                ObjWo = (WorkOrder)WoInput.Value;
                Rework_WO = ObjWo.WorkorderNo;
            }
            //object next_station = (SN)Station.StationSession[1].Value;
            // add by fgg 2018.05.03 RMA 入RMA后掃重工
            if (Sn_Satation != "MRB" && Sn_Satation != "RMA")
            {
                //Station.AddMessage("MES00000076", new string[] { Sn }, MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000076", new string[] { Sn }));
            }
            else if (Sn_Wo == Rework_WO)
            {
                //  Station.AddMessage("MES00000077", new string[] { Sn }, MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000077", new string[] { Sn }));
            }
            else
            {
                //MESStationSession SNRework = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                //if (SNRework == null)
                //{
                //    SNRework = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                //    Station.StationSession.Add(SNRework);
                //}
                //SNRework.Value = Sn;
                Station.AddMessage("MES00000101", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
        }






        /// <summary>
        /// 輸入工單的版本與SN 版本對比檢查
        /// SN 工單版本與輸入工單版本比較，不相同，則報錯，相同通過；
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">SESSION_TYPE:"SN",SESSION_TYPE:"WO"</param>
        public static void InputWoVerSNVerchecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            string Sn = Input.Value.ToString();
            string WoInput_Ver = "";//取輸入工單的料號版本
            string WoFromSn_Ver = "";//取輸入SN的工單料號版本
            //MESStationSession WoFromSn = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");//配WoFromSNDataloader方法，取WorkOrder對象
            //if (WoFromSn == null)
            //{
            //    //Station.AddMessage("MES00000076", new string[] { "SnInput", SnInput }, MESReturnView.Station.StationMessageState.Fail);
            //}
            //else
            //{
            //    WorkOrder ObjWoFromSn = new WorkOrder();
            //    ObjWoFromSn = (WorkOrder)WoFromSn.Value;
            //    WoFromSn_Ver = ObjWoFromSn.SKU_VER;
            //}
            T_R_SN R_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string Sn_Wo = R_SN.GetDetailBySN(Sn, Station.SFCDB).WORKORDERNO;
            T_R_WO_BASE R_WO = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            WoFromSn_Ver = R_WO.LoadWorkorder(Sn_Wo, Station.SFCDB).SKU_VER;
            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);//配WoDataloader方法，取WorkOrder對象
            if (WoInput == null)
            {
                Station.AddMessage("MES00000050", new string[] { }, MESReturnView.Station.StationMessageState.Fail);
                return;
            }
            else
            {
                WorkOrder ObjWoInput = new WorkOrder();
                ObjWoInput = (WorkOrder)WoInput.Value;
                WoInput_Ver = ObjWoInput.SKU_VER;
            }
            if (WoFromSn_Ver == null || WoInput_Ver == null)
            {
                Station.AddMessage("MES00000006", new string[] { "SKU_VER" }, MESReturnView.Station.StationMessageState.Fail);
            }
            else if (WoFromSn_Ver != WoInput_Ver)
            {
                Station.AddMessage("MES00000084", new string[] { "Input workorder", "Input sn" }, MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                MESStationSession WoFromSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (WoFromSn == null)
                {
                    WoFromSn = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(WoFromSn);
                }
                WoFromSn.Value = Sn;
                Station.AddMessage("MES00000085", new string[] { "Input workorder", "Input sn" }, MESReturnView.Station.StationMessageState.Pass);
            }
        }





        /// <summary>
        /// 輸入SN料號與工單料號對比檢查
        /// 將SN 對應工單的料號與輸入工單的料號進行比較，不相同，則報錯，相同，則通過；
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">SESSION_TYPE:"SN",SESSION_TYPE:"WO"</param>
        public static void InputSNSkuWoSkuchecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            string Sn = Input.Value.ToString();
            string WoInput_Skuno = "";//取輸入工單的料號版本
            string WoFromSn_Skuno = "";//取輸入SN的工單料號版本
            //MESStationSession WoFromSn = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");//配WoFromSNDataloader方法，取WorkOrder對象
            //if (WoFromSn == null)
            //{
            //    //Station.AddMessage("MES00000076", new string[] { "SnInput", SnInput }, MESReturnView.Station.StationMessageState.Fail);
            //}
            //else
            //{
            //    WorkOrder ObjWoFromSn = new WorkOrder();
            //    ObjWoFromSn = (WorkOrder)WoFromSn.Value;
            //    WoFromSn_Ver = ObjWoFromSn.SKU_VER;
            //}
            T_R_SN T_R_SN = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            R_SN R_SN = T_R_SN.GetDetailBySN(Sn, Station.SFCDB);
            string Sn_Wo = R_SN.WORKORDERNO;
            //T_R_WO_BASE R_WO = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //Row_R_WO_BASE R_WO_BASE = R_WO.LoadWorkorder(Sn_Wo, Station.SFCDB);
            WoFromSn_Skuno = R_SN.SKUNO;// R_WO_BASE.SKUNO;
            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == "WO" && t.SessionKey == "1");//配WoDataloader方法，取WorkOrder對象
            if (WoInput == null)
            {
                //Station.AddMessage("MES00000050", new string[] { }, MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050", new string[] { }));

            }
            else
            {
                WorkOrder ObjWoInput = new WorkOrder();
                ObjWoInput = (WorkOrder)WoInput.Value;
                WoInput_Skuno = ObjWoInput.SkuNO;
            }
            if (WoFromSn_Skuno == null || WoInput_Skuno == null)
            {
                //Station.AddMessage("MES00000006", new string[] { "SKUNO" }, MESReturnView.Station.StationMessageState.Fail);
            }
            else if (WoFromSn_Skuno != WoInput_Skuno)
            {
                //Station.AddMessage("MES00000095", new string[] { "Input workorder", "Input sn" }, MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000095", new string[] { ((WorkOrder)WoInput.Value).WorkorderNo, R_SN.SN }));
            }
            else
            {
                //MESStationSession WoFromSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                //if (WoFromSn == null)
                //{
                //    WoFromSn = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                //    Station.StationSession.Add(WoFromSn);
                //}
                //WoFromSn.Value = Sn;
                //Station.AddMessage("MES00000096", new string[] { "Input workorder", "Input sn" }, MESReturnView.Station.StationMessageState.Pass);
            }
        }








        /// <summary>
        /// 產品掃描Rework WO Check
        /// 1.	存在workordertype 為Rework的
        /// 2.	要滿足inputqty<workorderqty 

        /// 3.	要滿足Closed=0
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReworkWOchecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            string Wo = "";
            string wo_type = "";
            double? workorderqty = 0;
            double? inputqty = 0;
            string closed_flag = "";
            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == "WO" && t.SessionKey == "1");
            if (WoInput == null)
            {
                Station.AddMessage("MES00000050", new string[] { }, MESReturnView.Station.StationMessageState.Fail);
                return;
            }
            else
            {
                WorkOrder ObjWoInput = new WorkOrder();
                ObjWoInput = (WorkOrder)WoInput.Value;
                Wo = ObjWoInput.WorkorderNo.ToUpper();
                wo_type = ObjWoInput.WO_TYPE.ToUpper();
                workorderqty = (double?)ObjWoInput.WORKORDER_QTY;
                inputqty = (double?)ObjWoInput.INPUT_QTY;
                closed_flag = ObjWoInput.CLOSED_FLAG;
            }
            if (wo_type != "REWORK")
            {
                Station.AddMessage("MES00000098", new string[] { "REWORK" }, MESReturnView.Station.StationMessageState.Fail);
            }
            else if (inputqty >= workorderqty)
            {
                Station.AddMessage("MES00000099", new string[] { }, MESReturnView.Station.StationMessageState.Fail);
            }
            else if (closed_flag != "0")
            {
                Station.AddMessage("MES00000100", new string[] { "" }, MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                MESStationSession WoChecked = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (WoChecked == null)
                {
                    WoChecked = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(WoChecked);
                }
                // WoChecked.Value = Wo;
                Station.AddMessage("MES00000101", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
        }

        public static void SNInRepairchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Re_sn = Input.Value.ToString();
            string ErrMessage = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }
            T_R_REPAIR_TRANSFER trt = new T_R_REPAIR_TRANSFER(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_TRANSFER> list = trt.GetReSNbysn(Re_sn, Station.SFCDB);
            if (list != null)
            {
                if (list[0].IN_TIME != null && list[0].OUT_TIME == null && list[0].CLOSED_FLAG == "0")
                {
                    Station.AddMessage("MES00000046", new string[] { Re_sn }, MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000007", new string[] { Re_sn }, MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                    new string[] { Re_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                Station.AddMessage("MES00000007", new string[] { Re_sn }, MESReturnView.Station.StationMessageState.Fail);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                new string[] { Re_sn });
                throw new MESReturnMessage(ErrMessage);
            }

        }

        public static void SNOutRepairchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Re_sn = Input.Value.ToString();
            string ErrMessage = string.Empty;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }
            T_R_REPAIR_TRANSFER trt = new T_R_REPAIR_TRANSFER(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_REPAIR_MAIN trm = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_TRANSFER> list = trt.GetReSNbysn(Re_sn, Station.SFCDB);
            if (list != null)
            {
                if (list[0].IN_TIME != null && list[0].OUT_TIME == null && list[0].CLOSED_FLAG == "0")
                {
                    List<R_REPAIR_MAIN> listmain = trm.GetRepairMainBySN(Station.SFCDB, Re_sn);
                    R_REPAIR_MAIN Re_main = listmain.Find(s => s.CLOSED_FLAG == "1" && s.ID == list[0].REPAIR_MAIN_ID);
                    if (Re_main != null)
                    {
                        Station.AddMessage("MES00000046", new string[] { Re_sn }, MESReturnView.Station.StationMessageState.Pass);
                    }
                    else
                    {
                        Station.AddMessage("MES00000007", new string[] { Re_sn }, MESReturnView.Station.StationMessageState.Fail);
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                        new string[] { Re_sn });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
                else
                {
                    Station.AddMessage("MES00000007", new string[] { Re_sn }, MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                    new string[] { Re_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                Station.AddMessage("MES00000007", new string[] { Re_sn }, MESReturnView.Station.StationMessageState.Fail);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000007",
                                new string[] { Re_sn });
                throw new MESReturnMessage(ErrMessage);
            }

        }

        /// <summary>
        /// 檢查 SN 所在的測試站位是否存在
        /// 張官軍 2018/01/18
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNCallHWWSChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string SerialNumber = string.Empty;
            string SettingStation = string.Empty;
            T_C_SKU_DETAIL SkuDetail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNumber = ((SN)SNSession.Value).SerialNo;

            var SettingStationObj = SkuDetail.GetSkuDetail(Station.StationName, "CHECK_TEST", ((SN)SNSession.Value).SkuNo, Station.SFCDB);
            if (SettingStationObj != null)
            {
                HateEmsData data = new HateEmsData();

                data.MesWebProxy = System.Configuration.ConfigurationManager.AppSettings["HWMesWebProxyIP"];
                data.MesWebProxyPort = System.Configuration.ConfigurationManager.AppSettings["HWMesWebProxyPort"];
                data.UserName = System.Configuration.ConfigurationManager.AppSettings["HWMesWebUserName"];
                data.Factory = System.Configuration.ConfigurationManager.AppSettings["HWMesWebFactory"];
                data.ProcStep = System.Configuration.ConfigurationManager.AppSettings["HWMesWebProcStep"];// "1";
                data.Barcode = SerialNumber;
                data.Operation = System.Configuration.ConfigurationManager.AppSettings["HWMesWebOperation"];//"111";
                data.BarcodeType = System.Configuration.ConfigurationManager.AppSettings["HWMesWebBarcodeType"];// "LOT_ID"; 
                data.Service = System.Configuration.ConfigurationManager.AppSettings["HWMesWebService"]; //"GET_PRODUCT_INFO_EMS_BY_SN";
                data.Language = Convert.ToUInt16(System.Configuration.ConfigurationManager.AppSettings["HWMesWebLanguage"].ToString());//1;

                var result = (hateEmsGetDataServiceOut)HateEmsCaller.EmsService(data);

                SettingStation = SettingStationObj.VALUE.Trim().ToString();

                if (result != null)
                {
                    if (result.operation.ToUpper().Trim().Equals(SettingStation))
                    {
                        Station.AddMessage("MES00000109", new string[] { result.emsOrderId.ToUpper().Trim() },
                            MESReturnView.Station.StationMessageState.Pass);
                    }
                    else
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000108", new string[] {
                        result.operation.ToUpper().Trim(),SettingStation,result.emsOrderId.ToUpper().Trim() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
                else
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000107", new string[] { SerialNumber });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            else
            {
                Station.AddMessage("MES00000109", new string[] { },
                            MESReturnView.Station.StationMessageState.Pass);
            }
        }

        public static void InputSNExistChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StrSN = "";
            R_SN R_Sn = null;
            //modify by LLF 2018-02-06 
            //StrSN = Station.Inputs[1].ToString();
            StrSN = Input.Value.ToString();
            SN SNObj = new SN();
            R_Sn = SNObj.LoadSN(StrSN, Station.SFCDB);

            //modify by ZGJ 2018-03-15
            //下面的判斷錯誤且多餘
            if (R_Sn != null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000138", new string[] { StrSN }));
            }

            //if (Paras.Count > 0)
            //{
            //    if (R_Sn == null)
            //    {

            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000138", new string[] { StrSN }));
            //    }
            //}
            //else
            //{
            //    if (R_Sn != null)
            //    {
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000138", new string[] { StrSN }));
            //    }
            //}
        }

        /// <summary>
        /// 檢查Panel表中是否存在該SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputSNExistPanelChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            R_PANEL_SN R_Panel_SN = null;
            string StrSN = Input.Value.ToString();
            SN SNObj = new SN();
            R_Panel_SN = SNObj.LoadPanelBySN(StrSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (R_Panel_SN != null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000138", new string[] { StrSN }));
            }
        }

        /// <summary>
        /// 檢查UNLINK工站 SN状态是否为MRB  
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void UnlinkSNStatusChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSn = (SN)SNSession.Value;

            if (ObjSn.CurrentStation != "MRB")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000076", new string[] { ObjSn.BoxSN }));
            }
        }
        public static void SNRuleChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string SNRuleName = "";
            string StrSN = "";
            bool CheckRuleFlag = false;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var obj = SNSession.Value;
            if (obj is SN)
            {
                StrSN = ((SN)SNSession.Value).SerialNo;
            }
            else if (obj is string)
            {
                StrSN = SNSession.Value.ToString();
            }

            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKUSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            SKU SkuObj = (SKU)SKUSession.Value;
            SNRuleName = SkuObj.SnRule.ToString();
            SN SNObj = new SN();
            CheckRuleFlag = SNObj.CheckSNRule(StrSN, SNRuleName, Station.SFCDB, DB_TYPE_ENUM.Oracle);
        }

        public static void HWDSNStockINChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;

            if (ObjSN.CompletedFlag == "0")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { ObjSN.SerialNo }));
            }

            if (ObjSN.NextStation.IndexOf("JOBFINISH") < 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { ObjSN.SerialNo }));
            }

            if (ObjSN.StockStatus == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000145", new string[] { ObjSN.SerialNo }));
            }
        }
        /// <summary>
        /// 產品未完工狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNNoCompleteChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;

            if (ObjSN.CompletedFlag == "0")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { ObjSN.SerialNo }));
            }
        }
        /// <summary>
        /// 最後一筆過站記錄為當前工站檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNLastStationPassChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StrSN = "";
            T_R_SN_STATION_DETAIL Table_R_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            StrSN = SNSession.Value.ToString();

            R_Sn_Station_Detail = Table_R_SN_STATION_DETAIL.GetSNLastPassStationDetail(StrSN, Station.SFCDB);

            if (R_Sn_Station_Detail != null)
            {
                if (R_Sn_Station_Detail.STATION_NAME == Station.StationName)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000188", new string[] { }));
                }
            }
        }

        public static void LotDetailSNStatusChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string StrStation = Station.StationName;
            LotNo ObjLot = new LotNo();
            bool MultiStatus = false;
            Row_R_LOT_STATUS GetLotNo;
            T_R_LOT_DETAIL RowLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_LOT_STATUS RowLotStatus = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (SessionSN == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            else
            {
                SN ObjSN = (SN)SessionSN.Value;
                if (!string.IsNullOrEmpty(Paras[0].VALUE))
                {
                    StrStation = Paras[0].VALUE.ToString();
                }
                GetLotNo = RowLotStatus.GetLotBySNForInLot(ObjSN.SerialNo, Station.SFCDB);
                if (GetLotNo != null)
                {
                    MultiStatus = RowLotDetail.CheckLotDetailSNStatus(GetLotNo.ID, StrStation, Station.SFCDB);
                    if (MultiStatus)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000202", new string[] { GetLotNo.LOT_NO }));
                    }
                }
            }
        }

        public static void SNRMAChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            R_SN r_sn = null;
            SN snObj = new SN();
            r_sn = snObj.LoadSN(SNSession.Value.ToString(), Station.SFCDB);
            if (r_sn != null && r_sn.CURRENT_STATION == "RMA")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000237", new string[] { r_sn.BOXSN }));
            }
        }

        /// <summary>
        /// 檢查測試工站的最後一次測試記錄是否PASS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNTestChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            MESDBHelper.OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            try
            {
                T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                SN snObj = new SN();
                snObj = (SN)sessionSN.Value;
                #region 檢查當前工站之前的所有測試工站的最後一筆測試記錄是否PASS By Eden 2018/05/16
                List<C_ROUTE_DETAIL> cRouteDetailList = t_c_route_detail.GetTestStationByNameBefor(sfcdb, snObj.RouteID, stationName);
                List<R_TEST_RECORD> td = t_r_test_record.GetTestDataByTimeBefor(sfcdb, snObj.ID, snObj.StartTime ?? DateTime.Now);
                foreach (var item in cRouteDetailList)
                {
                    if (td.FindAll(t => t.MESSTATION == item.STATION_NAME && t.STATE.Equals("PASS")).Count == 0)
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { snObj.SerialNo, item.STATION_NAME }));
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SNIsExistCheck(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string sn;
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            sn = Input.Value.ToString();
            MESStationSession InputSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputSN_Session == null)
            {
                InputSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                InputSN_Session.Value = sn;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = sn;
                Station.StationSession.Add(InputSN_Session);
            }
            else
            {
                InputSN_Session.Value = sn;
                InputSN_Session.ResetInput = Input;
                InputSN_Session.InputValue = sn;
            }

            T_R_SN tr_sn = new T_R_SN(Station.SFCDB, Station.DBType);

            if (tr_sn.CheckSNExists(sn, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { sn }));
            }
            else
            {
                Station.AddMessage("MES00000001", new string[] { sn }, MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 檢查當前Pack狀態(包裝里SN是否狀態一致),當前工站是否待OBA
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackSnStatus(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE).Value = "";
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_SN_PACKING T_RSnPacking = new T_R_SN_PACKING(Station.SFCDB, Station.DBType);
            if (!T_RSnPacking.CheckPackSnStatus(Station.SFCDB, Paras[1].VALUE, PackNoSession.Value.ToString()))
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180528095410", new string[] { PackNoSession.Value.ToString() }));
        }

        /// <summary>
        /// 檢查SN對象集合中是否已綁定Keypart
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSNObjectListIsLink(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionInputString = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionInputString == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            if (sessionInputString.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession snObjectList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (snObjectList == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            try
            {
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                List<R_SN> snList = new List<R_SN>();
                snList = (List<R_SN>)snObjectList.Value;
                foreach (R_SN r_sn in snList)
                {
                    if (t_r_sn_kp.CheckLinkBySNID(r_sn.ID, Station.SFCDB))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094344", new string[] { r_sn.SN }));
                    }
                }
                Station.AddMessage("MES00000001", new string[] { sessionInputString.Value.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查SN是否在該批次內
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSNInLot(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            DisplayOutPut Dis_LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO");
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_SN rRSn = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> rSnList =  rRSn.GetSnByLotNoWithOba(Dis_LotNo.Value.ToString(), Station.SFCDB);
            //{0}內Sn{1}下一站為{2};
            if (rSnList.FindAll(t=>!t.NEXT_STATION.Equals("OBA")).Count>0)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529165023", new string[] { Dis_LotNo.Value.ToString(), rSnList.Find(t => !t.NEXT_STATION.Equals("OBA")).SN, rSnList.Find(t => !t.NEXT_STATION.Equals("OBA")).NEXT_STATION }));
            //SN:{0}不在LOT:{1}內
            if (rSnList.FindAll(t => !t.SN.Equals(snSession.Value.ToString())).Count == 0)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529165225", new string[] { snSession.Value.ToString(), Dis_LotNo.Value.ToString() }));
        }


        /// <summary>
        /// 檢查當前SN狀態是否已抽檢過;
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnStatusInOba(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            DisplayOutPut Dis_LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO");
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_LOT_DETAIL tRLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            List<R_LOT_DETAIL> rLotDetail = tRLotDetail.GetLotDetailByLotNo(Dis_LotNo.Value.ToString(), Station.SFCDB);
            //Sn{0}已經抽檢過,狀態為{1}請檢查!
            if (rLotDetail.FindAll(t => t.SN.Equals(snSession.Value.ToString())).Count > 0)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529172003", new string[] { rLotDetail.Find(t => t.SN.Equals(snSession.Value.ToString())).SN, rLotDetail.Find(t => t.SN.Equals(snSession.Value.ToString())).STATUS.Equals("1")?"PASS":"FAIL" }));
        }

    }
}
