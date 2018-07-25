using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using System.Data;
using MESDataObject.Module;
using MESStation.BaseClass;
using MESStation.LogicObject;
using MESStation.MESReturnView.Station;
using MESDBHelper;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class SNActions
    {
        /// <summary>
        /// SN 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNInputAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            R_SN SN = null;
            WorkOrder WorkOrder = null;
            T_R_SN SnTable = null;
            T_R_WO_BASE WoTable = null;
            MESStationSession SnSession = null;
            List<R_SN> SNs = new List<R_SN>();
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;

            if (Paras.Count != 3)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession NewSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (NewSn == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession Wo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WorkOrder = (WorkOrder)Wo.Value;

            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }

            SN = new R_SN();
            SN.SN = NewSn.Value.ToString();
            SN.SKUNO = WorkOrder.SkuNO;
            SN.WORKORDERNO = WorkOrder.WorkorderNo;
            SN.PLANT = WorkOrder.PLANT;
            SN.ROUTE_ID = WorkOrder.RouteID;
            SN.STARTED_FLAG = "1";
            SN.PACKED_FLAG = "0";
            SN.COMPLETED_FLAG = "0";
            SN.SHIPPED_FLAG = "0";
            SN.REPAIR_FAILED_FLAG = "0";
            SN.CURRENT_STATION = Station.StationName;
            SN.CUST_PN = WorkOrder.CUST_PN;
            SN.SCRAPED_FLAG = "0";
            SN.PRODUCT_STATUS = "FRESH";
            SN.REWORK_COUNT = 0d;
            SN.VALID_FLAG = "1";
            SN.EDIT_EMP = Station.LoginUser.EMP_NO;
            SNs.Add(SN);

            SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
            SnTable.AddToRSn(SNs, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB); // 插入到 R_SN 表中

            WoTable = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            WoTable.AddCountToWo(WorkOrder.WorkorderNo, 1, Station.SFCDB); //更新工單投入數量

            Station.AddMessage("MES00000054", new string[] { NewSn.Value.ToString() }, StationMessageState.Pass); //回饋消息到前台

        }

        /// <summary>
        /// SMTLoading工站，SN區間批次過站
        /// 2018/1/25 Rain
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LotSNInputAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strStartSN = "";
            string strEndSN = "";
            string strTempSN = "";
            string strDecmialType = "";
            int intLotQty;
            WorkOrder Wo;

            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            //工單必須存在
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
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
                if (StartSN_Session.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                else
                {
                    strStartSN = StartSN_Session.Value.ToString();
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
            //DecimalType必須加載
            MESStationSession DecimalType_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (DecimalType_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000127", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            else
            {
                if (DecimalType_Session.Value != null)
                {
                    strDecmialType = DecimalType_Session.Value.ToString();
                    if (strDecmialType != "10H" && strDecmialType != "34H")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000129", new string[] { Paras[3].SESSION_TYPE + strDecmialType }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000127", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
            }
            //LotQty必須加載
            MESStationSession LotQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (LotQty_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000128", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }
            else
            {
                if (LotQty_Session.Value != null)
                {
                    intLotQty = Convert.ToInt32(LotQty_Session.Value.ToString());
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000128", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
            }

            //批量投入SN，處理R_SN及過站記錄表
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> R_SNs = new List<R_SN>(intLotQty);
            List<string> SNIds = null;


            //循環取得下一個序號
            T_R_WO_BASE TR_Wo_Base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            string strSubSN = "";
            for (int i = 1; i <= intLotQty; i++)
            {
                R_SN OriginalSN = new R_SN();
                //初始化OriginalSN
                OriginalSN.SKUNO = Wo.SkuNO;
                OriginalSN.WORKORDERNO = Wo.WorkorderNo;
                OriginalSN.PLANT = Wo.PLANT;
                OriginalSN.ROUTE_ID = Wo.RouteID;
                OriginalSN.STARTED_FLAG = "1";
                OriginalSN.PACKED_FLAG = "0";
                //OriginalSN.PACKDATE
                OriginalSN.COMPLETED_FLAG = "0";
                //OriginalSN.COMPLETED_TIME
                OriginalSN.SHIPPED_FLAG = "0";
                //OriginalSN.SHIPDATE
                OriginalSN.REPAIR_FAILED_FLAG = "0";
                OriginalSN.CURRENT_STATION = Station.StationName;
                OriginalSN.CUST_PN = Wo.CUST_PN;
                OriginalSN.SCRAPED_FLAG = "0";
                //OriginalSN.SCRAPED_TIME
                OriginalSN.PRODUCT_STATUS = "FRESH";
                OriginalSN.REWORK_COUNT = 0d;
                OriginalSN.VALID_FLAG = "1";
                OriginalSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                if (i == 1)
                {
                    strTempSN = strStartSN;
                }
                else
                {
                    strSubSN = strTempSN.Substring(strTempSN.Length - 4, 4);
                    strSubSN = TR_Wo_Base.Get_NextSN(strSubSN, strDecmialType);
                    strSubSN = strSubSN.PadLeft(4, '0');
                    strTempSN = strTempSN.Substring(0, strTempSN.Length - 4) + strSubSN;
                }
                OriginalSN.SN = strTempSN;
                R_SNs.Add(OriginalSN);
            }
            //寫R_SN及R_SN_Station_Detail
            SNIds = TR_SN.AddToRSn(R_SNs, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
            // 更新 R_WO_BASE 中的投入數量
            TR_Wo_Base.AddCountToWo(Wo.WorkorderNo, intLotQty, Station.SFCDB);
            ////回饋消息到前台
            Station.AddMessage("MES00000130", new string[] { intLotQty.ToString() }, StationMessageState.Pass);

        }

        /// <summary>
        /// 記錄良率
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RecordYieldRateAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StationName = string.Empty;
            string WorkOrderNo = string.Empty;
            string Sn = string.Empty;
            string Status = string.Empty;
            T_R_SN SnTable = null;
            SN SNObj = null;
            double LinkQty = 0d;
            string ErrMessage = string.Empty;

            if (Paras.Count != 4)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "4", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //NEWSN
            MESStationSession SNSession = Station.StationSession.Find(
                                                    t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //Modify By LLF 2018-01-27 
            //Sn = SNSession.Value.ToString();
            Sn = SNSession.InputValue.ToString();

            ////WO
            MESStationSession WoSession = Station.StationSession.Find(
                                                    t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            ////STATUS
            MESStationSession StatusSession = Station.StationSession.Find(
                                                    t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {
                //StatusSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = "Pass" };
                //Station.StationSession.Add(StatusSession);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);

            }
            Status = StatusSession.Value.ToString();


            //Modify by LLF 2018-01-28,LinqQty 取實際的連板數量，而不是配置的連板數量
            ////LINKQTY
            //MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            //if (LinkQtySession == null)
            //{
            //    //LinkQtySession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = "1" };
            //    //Station.StationSession.Add(LinkQtySession);
            //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
            //    throw new MESReturnMessage(ErrMessage);
            //}
            SNObj = new SN();
            LinkQty = SNObj.GetLinkQty(Sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            StationName = Station.StationName;
            WorkOrderNo = ((WorkOrder)WoSession.Value).WorkorderNo;

            SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
            SnTable.RecordYieldRate(WorkOrderNo, LinkQty, Sn, Status, Station.Line, StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
            Station.AddMessage("MES00000150", new string[] { Sn, "Yield Rate" }, StationMessageState.Pass);
        }

        /// <summary>
        /// 記錄 UPH
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RecordUPHAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StationName = string.Empty;
            string WorkOrderNo = string.Empty;
            string Sn = string.Empty;
            string Status = string.Empty;
            T_R_SN SnTable = null;
            SN SNObj = null;
            double LinkQty = 0d;
            string ErrMessage = string.Empty;

            if (Paras.Count != 4)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "4", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //NEWSN
            MESStationSession SNSession = Station.StationSession.Find(
                                                    t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            Sn = SNSession.InputValue.ToString();

            //WO
            MESStationSession WoSession = Station.StationSession.Find(
                                                    t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            //STATUS
            MESStationSession StatusSession = Station.StationSession.Find(
                                                    t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {

                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
                //StatusSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = "Pass" };
                //Station.StationSession.Add(StatusSession);
            }
            Status = StatusSession.Value.ToString();

            //Modify by LLF 2018-01-27,LinqQty 取實際的連板數量，而不是配置的連板數量
            //LINKQTY
            //MESStationSession LinkQtySession = Station.StationSession.Find(
            //                                        t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            //if (LinkQtySession == null)
            //{
            //    LinkQtySession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = "1" };
            //    Station.StationSession.Add(LinkQtySession);
            //    //ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
            //    //throw new MESReturnMessage(ErrMessage);
            //}
            //LinkQty = double.Parse(LinkQtySession.Value.ToString());
            SNObj = new SN();
            LinkQty = SNObj.GetLinkQty(Sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            StationName = Station.StationName;
            WorkOrderNo = ((WorkOrder)WoSession.Value).WorkorderNo;

            SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
            SnTable.RecordUPH(WorkOrderNo, LinkQty, Sn, Status, Station.Line, StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
            Station.AddMessage("MES00000150", new string[] { Sn, "UPH" }, StationMessageState.Pass);
        }

        /// <summary>
        /// SN 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNPassStationAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;

            if (Paras.Count != 3)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }



            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SnObject = (SN)SNSession.Value;

            //STATUS,方便寫良率和UPH使用
            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == "STATUS" && t.SessionKey == "1");
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[1].VALUE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StatusSession);
                if (StatusSession.Value == null ||
                    (StatusSession.Value != null && StatusSession.Value.ToString() == ""))
                {
                    StatusSession.Value = "PASS";
                }
            }

            //Device
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else //Add by LLF 2018-02-05
            {
                DeviceName = Station.StationName;
            }

            table.PassStation(SnObject.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, StatusSession.Value.ToString(), Station.LoginUser.EMP_NO, Station.SFCDB);
            //table.PassStation("6131351adfdf", "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);
            Station.AddMessage("MES00000063", new string[] { SnObject.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 寫過站記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RecordPassStationDetailAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string SerialNo = string.Empty;
            string DeviceName = Station.StationName;
            string ErrMessage = string.Empty;


            if (Paras.Count != 2)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "2", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
            //獲取 SN1
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNo = SnSession.Value.ToString();
            //SerialNo = ((R_SN)SnSession.Value).SN;

            //獲取 DEVICE1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }


            table.RecordPassStationDetail(SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);
            //table.RecordPassStationDetail(Input.Value.ToString(), Station.Line, Station.StationName, DeviceName,Station.BU, Station.SFCDB);

        }

        /// <summary>
        /// 產品掃描MRB過站Action,2018/01/10 肖倫      
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNMrbPassAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int result = 0;
            string ZCPP_FLAG = "";//add by fgg 2018.04.10 用於標誌是組裝退料還是從工單入MRB 
            SN SnObject = null;
            bool isSame = false;
            string UserEMP = Station.LoginUser.EMP_NO;
            string To_Storage = "";
            string From_Storage = "";
            string Confirmed_Flag = "";
            string DeviceName = "";
            R_SN NewSN = new R_SN();
            R_MRB New_R_MRB = new R_MRB();
            //Modify by LLF
            //H_MRB_GT HMRB_GT = new H_MRB_GT();
            R_MRB_GT HMRB_GT = new R_MRB_GT();
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);
            //Modify by LLF 
            //T_H_MRB_GT TH_MRB_GT = new T_H_MRB_GT(Station.SFCDB, Station.DBType);
            T_R_MRB_GT TH_MRB_GT = new T_R_MRB_GT(Station.SFCDB, Station.DBType);
            T_R_WO_BASE TR_WO_BASE = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_C_SAP_STATION_MAP TC_SAP_STATION_MAP = new T_C_SAP_STATION_MAP(Station.SFCDB, Station.DBType);
            if (Paras.Count < 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SnObject = (SN)SNSession.Value;
            //SNID必須存在
            if (SnObject.ID == null || SnObject.ID.Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //SN如果已經完工，Confirmed_Flag=1，否則Confirmed_Flag=0
            if (SnObject.CompletedFlag != null && SnObject.CompletedFlag == "1")
            {
                Confirmed_Flag = "1";
            }
            else
            {
                Confirmed_Flag = "0";
            }
            //判斷MRBType，0是單板入MRB，1是退料
            //0則From_Storage放空
            //1則From_Storage則取前台傳的工單
            if (Paras[1].VALUE == "0")//0是單板入MRB
            {
                if (Paras.Count != 3)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }
                From_Storage = "";
                ZCPP_FLAG = "0";
            }
            else if (Paras[1].VALUE == "1")//1是退料
            {
                if (Paras.Count != 4)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }
                MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (WoSession == null || WoSession.Value == null || WoSession.Value.ToString().Length <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else
                {
                    From_Storage = WoSession.Value.ToString();
                    ZCPP_FLAG = "1";
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000020", new string[] { "MRBType", "0/1" }));
            }
            //獲取To_Storage
            MESStationSession ToStorageSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (ToStorageSession == null || ToStorageSession.Value == null || ToStorageSession.Value.ToString().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                To_Storage = ToStorageSession.Value.ToString();
            }
            //更新SN當前站，下一站，如果SN的compled=1了也就是Confirmed_Flag==1,則修改當前站和下一站即可
            //如果如果SN的compled!=1,則還需要修改sn的compled=1和SN對應工單的finishedQTY要加一
            if (Confirmed_Flag != "1")
            {
                result = TR_SN.SN_Mrb_Pass_action(SnObject.ID, UserEMP, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
                }
                else
                {
                    if (SnObject.WorkorderNo == null || SnObject.WorkorderNo.Trim().Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000125", new string[] { SnObject.SerialNo }));
                    }
                    else
                    {
                        TR_WO_BASE.GetWo(SnObject.WorkorderNo, Station.SFCDB);//這裡如果工單不存在GetWo會報錯
                        result = TR_WO_BASE.UpdateFINISHEDQTYAddOne(SnObject.WorkorderNo, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + SnObject.SerialNo, "UPDATE" }));
                        }
                    }
                }
            }
            else
            {
                result = TR_SN.SN_Mrb_Pass_actionNotUpdateCompleted(SnObject.ID, UserEMP, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
                }
            }

            //添加一筆MRB記錄
            //給new_r_mrb賦值
            New_R_MRB.ID = TR_MRB.GetNewID(Station.BU, Station.SFCDB, Station.DBType);
            New_R_MRB.SN = SnObject.SerialNo;
            New_R_MRB.WORKORDERNO = SnObject.WorkorderNo;
            New_R_MRB.NEXT_STATION = SnObject.NextStation;
            New_R_MRB.SKUNO = SnObject.SkuNo;
            New_R_MRB.FROM_STORAGE = From_Storage;
            New_R_MRB.TO_STORAGE = To_Storage;
            New_R_MRB.REWORK_WO = "";//空
            New_R_MRB.CREATE_EMP = UserEMP;
            New_R_MRB.CREATE_TIME = Station.GetDBDateTime();
            New_R_MRB.MRB_FLAG = "0";
            New_R_MRB.SAP_FLAG = "0";
            New_R_MRB.EDIT_EMP = UserEMP;
            New_R_MRB.EDIT_TIME = New_R_MRB.CREATE_TIME;
            result = TR_MRB.Add(New_R_MRB, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + SnObject.SerialNo, "ADD" }));
            }

            //存在R_MRB_GT WO =? And SAP_FLAG = 0,則檢查FROM_STORAGE，TO_STORAGE，CONFIRMED_FLAG是否一樣，一樣則累加1
            if (Paras[1].VALUE == "0")//0是單板入MRB
            {               
                HMRB_GT = TH_MRB_GT.GetByWOAndSAPFlageIsZero(SnObject.WorkorderNo, Station.SFCDB);
            }
            else if (Paras[1].VALUE == "1")//1是退料
            {
                HMRB_GT = TH_MRB_GT.GetByWOAndSAPFlageIsZero(From_Storage, Station.SFCDB);
            }
                
            isSame = false;
            if (HMRB_GT != null)
            {
                HMRB_GT.FROM_STORAGE = (HMRB_GT.FROM_STORAGE == null || HMRB_GT.FROM_STORAGE.Trim().Length <= 0) ? "" : HMRB_GT.FROM_STORAGE;
                HMRB_GT.TO_STORAGE = (HMRB_GT.TO_STORAGE == null || HMRB_GT.TO_STORAGE.Trim().Length <= 0) ? "" : HMRB_GT.TO_STORAGE;
                HMRB_GT.CONFIRMED_FLAG = (HMRB_GT.CONFIRMED_FLAG == null || HMRB_GT.CONFIRMED_FLAG.Trim().Length <= 0) ? "" : HMRB_GT.CONFIRMED_FLAG;
                if (HMRB_GT.FROM_STORAGE == New_R_MRB.FROM_STORAGE && HMRB_GT.TO_STORAGE == New_R_MRB.TO_STORAGE && HMRB_GT.CONFIRMED_FLAG == Confirmed_Flag)
                {
                    isSame = true;
                    if (Paras[1].VALUE == "0")//0是單板入MRB
                    {
                        result = TH_MRB_GT.updateTotalQTYAddOne(SnObject.WorkorderNo, UserEMP, Station.SFCDB);
                    }
                    else if (Paras[1].VALUE == "1")//1是退料
                    {
                        result = TH_MRB_GT.updateTotalQTYAddOne(From_Storage, UserEMP, Station.SFCDB);
                    }
                    
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "H_MRB_GT:" + SnObject.SerialNo, "UPDATE" }));
                    }
                }
            }
            if (!isSame)
            {
                //Modify by LLF
                //H_MRB_GT New_HMRB_GT = new H_MRB_GT();
                R_MRB_GT New_HMRB_GT = new R_MRB_GT();
                //賦值
                New_HMRB_GT.ID = TH_MRB_GT.GetNewID(Station.BU, Station.SFCDB);
                if (Paras[1].VALUE == "0")//0是單板入MRB
                {
                    New_HMRB_GT.WORKORDERNO = SnObject.WorkorderNo;
                }
                else if (Paras[1].VALUE == "1")//1是退料
                {
                    New_HMRB_GT.WORKORDERNO = From_Storage;
                }                
                Row_R_WO_BASE RowWO_BASE = TR_WO_BASE.GetWo(SnObject.WorkorderNo, Station.SFCDB);
                //string sapStationCode = TC_SAP_STATION_MAP.GetMAXSAPStationCodeBySkuAndWorkorderType(SnObject.SkuNo, RowWO_BASE.WO_TYPE, Station.SFCDB);               
                string sapStationCode = TC_SAP_STATION_MAP.GetMAXSAPStationCodeBySku(SnObject.SkuNo, Station.SFCDB);
                if (sapStationCode == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000224", new string[] { SnObject.SkuNo }));
                }
                New_HMRB_GT.SAP_STATION_CODE = sapStationCode;
                New_HMRB_GT.FROM_STORAGE = From_Storage;
                New_HMRB_GT.TO_STORAGE = To_Storage;
                New_HMRB_GT.TOTAL_QTY = 1;
                New_HMRB_GT.CONFIRMED_FLAG = Confirmed_Flag;
                New_HMRB_GT.ZCPP_FLAG = ZCPP_FLAG;//暫時預留
                New_HMRB_GT.SAP_FLAG = "0";//0待拋,1已拋,2待重拋
                New_HMRB_GT.SKUNO = SnObject.SkuNo;
                New_HMRB_GT.SAP_MESSAGE = "";
                New_HMRB_GT.EDIT_EMP = UserEMP;
                New_HMRB_GT.EDIT_TIME = Station.GetDBDateTime();
                result = TH_MRB_GT.Add(New_HMRB_GT, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "H_MRB_GT:" + SnObject.SerialNo, "ADD" }));
                }
            }
            //添加過站記錄           
            result = Convert.ToInt32(TR_SN.RecordPassStationDetail(SnObject.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB));
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + SnObject.SerialNo, "ADD" }));
            }
            Station.AddMessage("MES00000063", new string[] { SnObject.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 產品Rework過站Action,2018/01/10 肖倫 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReworkPassAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int result = 0;
            SN SnObject = null;
            R_SN NewSN = new R_SN();
            string UserEMP = Station.LoginUser.EMP_NO;
            string SNValidFlag = "0";
            string DeviceName = "";
            string ReworkStation = "";
            WorkOrder WoObject = null;
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);
            T_R_WO_BASE TR_WO_BASE = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_H_MRB_GT TH_MRB_GT = new T_H_MRB_GT(Station.SFCDB, Station.DBType);
            if (Paras.Count != 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SnObject = (SN)SNSession.Value;
            //SNID必須要存在
            if (SnObject.ID == null || SnObject.ID.Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //獲取工單對象
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else if (WOSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            WoObject = (WorkOrder)WOSession.Value;
            if (WoObject.WorkorderNo == null || WoObject.WorkorderNo.Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession NextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (NextStationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else if (NextStationSession.Value == null || NextStationSession.Value.ToString().Trim().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                ReworkStation = NextStationSession.Value.ToString().Trim();
            }
            //更新SN的Valid_Flag=0
            result = TR_SN.updateValid_Flag(SnObject.ID, SNValidFlag, UserEMP, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
            }
            //新增一筆SN 與重工工單記錄
            NewSN = TR_SN.GetById(SnObject.ID, Station.SFCDB);
            NewSN.ID = TR_SN.GetNewID(Station.BU, Station.SFCDB);
            NewSN.WORKORDERNO = WoObject.WorkorderNo;
            NewSN.CURRENT_STATION = "REWORK";
            NewSN.NEXT_STATION = ReworkStation;
            NewSN.ROUTE_ID = WoObject.RouteID;
            NewSN.VALID_FLAG = "1";
            NewSN.COMPLETED_FLAG = "0";
            NewSN.PRODUCT_STATUS = "REWORK";
            if (NewSN.REWORK_COUNT == null)
            {
                NewSN.REWORK_COUNT = 1;
            }
            else
            {
                NewSN.REWORK_COUNT = NewSN.REWORK_COUNT + 1;
            }
            NewSN.EDIT_EMP = UserEMP;
            NewSN.EDIT_TIME = Station.GetDBDateTime();
            result = TR_SN.AddNewSN(NewSN, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "ADD" }));
            }
            //更新r_MRB
            result = TR_MRB.OutMrbUpdate(WoObject.WorkorderNo, UserEMP, SnObject.SerialNo, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + SnObject.SerialNo, "UPDATE" }));
            }
            //更新工單投入數量
            result = Convert.ToInt32(TR_WO_BASE.AddCountToWo(WoObject.WorkorderNo, 1, Station.SFCDB));
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + WoObject.WorkorderNo, "UPDATE" }));
            }
            //添加TR_SN_STATION_DETAIL  
            result = Convert.ToInt32(TR_SN.RecordPassStationDetail(NewSN.SN, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB));
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + SnObject.SerialNo, "ADD" }));
            }
            Station.AddMessage("MES00000063", new string[] { SnObject.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        ///產品單個維修動作完成Action
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNSaveRepairAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            Object ReplaceIDObject;
            string UpdateSql = null;
            string VRepairer = Station.LoginUser.EMP_NO;
            string VDuty = "";
            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            T_r_repair_action RepairAction = new T_r_repair_action(Station.SFCDB, Station.DBType);
            Row_r_repair_action RepairRow = (Row_r_repair_action)RepairAction.NewRow();
            T_C_REPAIR_ITEMS TTRepairItems = new T_C_REPAIR_ITEMS(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS RepairItemsRow;
            T_C_REPAIR_ITEMS_SON HHRepairItemSon = new T_C_REPAIR_ITEMS_SON(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS_SON RepairItemSonRow;

            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                //獲取到 SN 對象
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (SNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                SnObject = (SN)SNSession.Value;

                MESStationInput RepairedBySession = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
                if (RepairedBySession.Value.ToString() != "")
                {
                    VRepairer = RepairedBySession.Value.ToString();
                }

                MESStationInput ActionCodeSession = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE);
                if (ActionCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                else if (ActionCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                string VActionCode = ActionCodeSession.Value.ToString();

                MESStationInput RootCauseSession = Station.Inputs.Find(t => t.DisplayName == Paras[3].SESSION_TYPE);
                if (RootCauseSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else if (RootCauseSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                string VRootCause = RootCauseSession.Value.ToString();

                MESStationInput DutySession = Station.Inputs.Find(t => t.DisplayName == Paras[4].SESSION_TYPE);
                if (DutySession.Value.ToString() != "")
                {
                    VDuty = DutySession.Value.ToString();
                }

                MESStationSession TR_SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
                if (TR_SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                else if (TR_SNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                string VTR_SN = TR_SNSession.Value.ToString();

                MESStationSession PartNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
                if (PartNoSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                }
                else if (PartNoSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                }
                string VPartNo = PartNoSession.Value.ToString();

                MESStationSession MFRNameSession = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
                if (MFRNameSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[7].SESSION_TYPE + Paras[7].SESSION_KEY }));
                }
                else if (MFRNameSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[7].SESSION_TYPE + Paras[7].SESSION_KEY }));
                }
                string VMFRName = MFRNameSession.Value.ToString();

                MESStationSession DateCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
                if (DateCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                else if (DateCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                string VDateCode = DateCodeSession.Value.ToString();

                MESStationSession LotCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);
                if (LotCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[9].SESSION_TYPE + Paras[9].SESSION_KEY }));
                }
                else if (LotCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[9].SESSION_TYPE + Paras[9].SESSION_KEY }));
                }
                string VLotCode = LotCodeSession.Value.ToString();

                MESStationInput DescriptionSession = Station.Inputs.Find(t => t.DisplayName == Paras[10].SESSION_TYPE);
                if (DescriptionSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                }
                else if (DescriptionSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                }
                string VDescription = DescriptionSession.Value.ToString();

                MESStationInput FailCodeIDSession = Station.Inputs.Find(t => t.DisplayName == Paras[11].SESSION_TYPE);
                if (FailCodeIDSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                }
                else if (FailCodeIDSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                }
                string VFailCodeID = FailCodeIDSession.Value.ToString();

                if (VFailCodeID != null)
                {
                    FailCodeRow = RepairFailcode.GetByFailCodeID(VFailCodeID, Station.SFCDB);
                    if (FailCodeRow == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { SnObject.SerialNo, VFailCodeID }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                }

                MESStationInput RepairItemSession = Station.Inputs.Find(t => t.DisplayName == Paras[12].SESSION_TYPE);

                string RepairItem = RepairItemSession.Value.ToString();

                if (string.IsNullOrEmpty(RepairItem))
                {
                    RepairItem = "";
                }
                else
                {
                    RepairItemsRow = TTRepairItems.GetIDByItemName(RepairItem, Station.SFCDB);
                    RepairItem = RepairItemsRow.ID;
                }

                MESStationInput RepairItemSonSession = Station.Inputs.Find(t => t.DisplayName == Paras[13].SESSION_TYPE);

                string RepairItemSon = RepairItemSonSession.Value.ToString();

                if (string.IsNullOrEmpty(RepairItemSon))
                {
                    RepairItemSon = "";
                }
                else
                {
                    RepairItemSonRow = HHRepairItemSon.GetIDByItemsSon(RepairItemSon, Station.SFCDB);
                    RepairItemSon = RepairItemSonRow.ID;
                }

                MESStationInput LocationSession = Station.Inputs.Find(t => t.DisplayName == Paras[14].SESSION_TYPE);
                if (LocationSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[14].SESSION_TYPE + Paras[14].SESSION_KEY }));
                }
                else if (LocationSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[14].SESSION_TYPE + Paras[14].SESSION_KEY }));
                }
                string Location = LocationSession.Value.ToString();

                MESStationInput ProcessSession = Station.Inputs.Find(t => t.DisplayName == Paras[15].SESSION_TYPE);
                if (ProcessSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                else if (ProcessSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                string Process = ProcessSession.Value.ToString();

                Station.SFCDB.BeginTrain();
                RepairRow.ID = RepairAction.GetNewID(Station.BU, Station.SFCDB);
                RepairRow.REPAIR_FAILCODE_ID = VFailCodeID;
                RepairRow.SN = SnObject.SerialNo;
                RepairRow.ACTION_CODE = VActionCode;
                RepairRow.SECTION_ID = VDuty;
                RepairRow.PROCESS = Process;//FailCodeRow.FAIL_PROCESS;
                RepairRow.ITEMS_ID = RepairItem;
                RepairRow.ITEMS_SON_ID = RepairItemSon;
                RepairRow.REASON_CODE = VRootCause;
                RepairRow.DESCRIPTION = VDescription;
                RepairRow.FAIL_LOCATION = Location;//FailCodeRow.FAIL_LOCATION;
                RepairRow.FAIL_CODE = FailCodeRow.FAIL_CODE;
                RepairRow.KEYPART_SN = VPartNo;
                RepairRow.NEW_KEYPART_SN = "";
                RepairRow.TR_SN = VTR_SN;
                RepairRow.MFR_NAME = VMFRName;
                RepairRow.DATE_CODE = VDateCode;
                RepairRow.LOT_CODE = VLotCode;
                RepairRow.REPAIR_EMP = VRepairer;
                RepairRow.REPAIR_TIME = Station.GetDBDateTime();
                RepairRow.EDIT_EMP = Station.LoginUser.EMP_NO;
                RepairRow.EDIT_TIME = Station.GetDBDateTime();

                string StrRes = Station.SFCDB.ExecSQL(RepairRow.GetInsertString(Station.DBType));
                if (StrRes == "1")
                {
                    Row_R_REPAIR_FAILCODE FRow = (Row_R_REPAIR_FAILCODE)RepairFailcode.GetObjByID(VFailCodeID, Station.SFCDB);
                    FRow.REPAIR_FLAG = "1";  //執行完維修動作後更新R_REPAIR_FAILCODE   FLAG=1 
                    FRow.EDIT_TIME = Station.GetDBDateTime();
                    UpdateSql = FRow.GetUpdateString(Station.DBType);
                    Station.SFCDB.ExecSQL(UpdateSql);
                    Station.SFCDB.CommitTrain();
                    Station.AddMessage("MES00000105", new string[] { SnObject.SerialNo, VFailCodeID }, StationMessageState.Pass);
                }
                else
                {
                    Station.SFCDB.RollbackTrain();
                    Station.AddMessage("MES00000083", new string[] { SnObject.SerialNo, VFailCodeID }, StationMessageState.Fail);
                }
            }
            catch (Exception ex)
            {
                Station.SFCDB.RollbackTrain();
                throw ex;
            }
        }

        /// <summary>
        /// add by fgg 2018.5.31
        ///產品單個維修動作完成Action New 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNSaveRepairActionNew(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;           
            string UpdateSql = null;
            string VRepairer = Station.LoginUser.EMP_NO;

            string VFailCodeID = "";
            string VActionCode = "";
            string RepairReseaonCode = "";
            string Process = "";
            string Location = "";
            string VSection = "";
            string RepairItem = "";
            string RepairItemSon = "";
            string RepairKeypartSN = "";

            string VTR_SN = "";
            string VKPNO = "";
            string VMFRName = "";
            string VDateCode = "";
            string VLotCode = "";
                       
            string VDescription = "";            

            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            T_r_repair_action RepairAction = new T_r_repair_action(Station.SFCDB, Station.DBType);
            Row_r_repair_action RepairRow = (Row_r_repair_action)RepairAction.NewRow();
            T_C_REPAIR_ITEMS TTRepairItems = new T_C_REPAIR_ITEMS(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS RepairItemsRow;
            T_C_REPAIR_ITEMS_SON HHRepairItemSon = new T_C_REPAIR_ITEMS_SON(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS_SON RepairItemSonRow;

            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                //獲取到 SN 對象
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (SNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                SnObject = (SN)SNSession.Value;

                MESStationInput FailCodeIDSession = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
                if (FailCodeIDSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                else if (FailCodeIDSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                VFailCodeID = FailCodeIDSession.Value.ToString();

                if (VFailCodeID != null)
                {
                    FailCodeRow = RepairFailcode.GetByFailCodeID(VFailCodeID, Station.SFCDB);
                    if (FailCodeRow == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { SnObject.SerialNo, VFailCodeID }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }

                MESStationInput ActionCodeSession = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE);
                if (ActionCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                else if (ActionCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                VActionCode = ActionCodeSession.Value.ToString();

                MESStationInput ReseaonCodeSession = Station.Inputs.Find(t => t.DisplayName == Paras[3].SESSION_TYPE);
                if (ReseaonCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else if (ReseaonCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                RepairReseaonCode = ReseaonCodeSession.Value.ToString();

                MESStationInput ProcessSession = Station.Inputs.Find(t => t.DisplayName == Paras[4].SESSION_TYPE);
                if (ProcessSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                else if (ProcessSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                Process = ProcessSession.Value.ToString();

                MESStationInput LocationSession = Station.Inputs.Find(t => t.DisplayName == Paras[5].SESSION_TYPE);
                if (LocationSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                else if (LocationSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                Location = LocationSession.Value.ToString();

                MESStationInput SectionSession = Station.Inputs.Find(t => t.DisplayName == Paras[6].SESSION_TYPE);
                if (SectionSession != null && SectionSession.Value != null)
                {
                    VSection = SectionSession.Value.ToString();
                }

                MESStationInput RepairItemSession = Station.Inputs.Find(t => t.DisplayName == Paras[7].SESSION_TYPE);
                if (RepairItemSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                RepairItem = RepairItemSession.Value.ToString();
                if (string.IsNullOrEmpty(RepairItem))
                {
                    RepairItem = "";
                }
                else
                {
                    RepairItemsRow = TTRepairItems.GetIDByItemName(RepairItem, Station.SFCDB);
                    RepairItem = RepairItemsRow.ID;
                }

                MESStationInput RepairItemSonSession = Station.Inputs.Find(t => t.DisplayName == Paras[8].SESSION_TYPE);
                if (RepairItemSonSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                RepairItemSon = RepairItemSonSession.Value.ToString();
                if (string.IsNullOrEmpty(RepairItemSon))
                {
                    RepairItemSon = "";
                }
                else
                {
                    RepairItemSonRow = HHRepairItemSon.GetIDByItemsSon(RepairItemSon, Station.SFCDB);
                    RepairItemSon = RepairItemSonRow.ID;
                }

                MESStationInput KeypartSNSession = Station.Inputs.Find(t => t.DisplayName == Paras[9].SESSION_TYPE);
                if (KeypartSNSession != null && KeypartSNSession.Value != null)
                {
                    RepairKeypartSN = KeypartSNSession.Value.ToString();
                }

                MESStationSession TR_SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[10].SESSION_TYPE && t.SessionKey == Paras[10].SESSION_KEY);
                MESStationSession KPNOSession = Station.StationSession.Find(t => t.MESDataType == Paras[11].SESSION_TYPE && t.SessionKey == Paras[11].SESSION_KEY);
                MESStationSession MFRNameSession = Station.StationSession.Find(t => t.MESDataType == Paras[12].SESSION_TYPE && t.SessionKey == Paras[12].SESSION_KEY);
                MESStationSession DateCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[13].SESSION_TYPE && t.SessionKey == Paras[13].SESSION_KEY);
                MESStationSession LotCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[14].SESSION_TYPE && t.SessionKey == Paras[14].SESSION_KEY);

                //如果有輸入ALLPART條碼,則ALLPART條碼對應的料號、廠商、DateCode、LotCode必須要有
                if (TR_SNSession != null)
                {
                    VTR_SN = TR_SNSession.Value.ToString();

                    if (KPNOSession == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                    }
                    else if (KPNOSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                    }
                    VKPNO = KPNOSession.Value.ToString();


                    if (MFRNameSession == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[12].SESSION_TYPE + Paras[12].SESSION_KEY }));
                    }
                    else if (MFRNameSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[12].SESSION_TYPE + Paras[12].SESSION_KEY }));
                    }
                    VMFRName = MFRNameSession.Value.ToString();


                    if (DateCodeSession == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[13].SESSION_TYPE + Paras[13].SESSION_KEY }));
                    }
                    else if (DateCodeSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[13].SESSION_TYPE + Paras[13].SESSION_KEY }));
                    }
                    VDateCode = DateCodeSession.Value.ToString();

                    if (LotCodeSession == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[14].SESSION_TYPE + Paras[14].SESSION_KEY }));
                    }
                    else if (LotCodeSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[14].SESSION_TYPE + Paras[14].SESSION_KEY }));
                    }
                    VLotCode = LotCodeSession.Value.ToString();
                }
                else
                {
                    if (KPNOSession != null && KPNOSession.Value != null)
                    {
                        VKPNO = KPNOSession.Value.ToString();
                    }

                    if (MFRNameSession != null && MFRNameSession.Value != null)
                    {
                        VMFRName = MFRNameSession.Value.ToString();
                    }

                    if (DateCodeSession != null && DateCodeSession.Value != null)
                    {
                        VDateCode = DateCodeSession.Value.ToString();
                    } 

                    if (LotCodeSession != null && LotCodeSession.Value != null)
                    {
                        VLotCode = LotCodeSession.Value.ToString();
                    }
                }                

                MESStationInput DescriptionSession = Station.Inputs.Find(t => t.DisplayName == Paras[15].SESSION_TYPE);
                if (DescriptionSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                else if (DescriptionSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                VDescription = DescriptionSession.Value.ToString();
               
                Station.SFCDB.BeginTrain();
                RepairRow.ID = RepairAction.GetNewID(Station.BU, Station.SFCDB);
                RepairRow.REPAIR_FAILCODE_ID = VFailCodeID;
                RepairRow.SN = SnObject.SerialNo;
                RepairRow.ACTION_CODE = VActionCode;
                RepairRow.SECTION_ID = VSection;
                RepairRow.PROCESS = Process;
                RepairRow.ITEMS_ID = RepairItem;
                RepairRow.ITEMS_SON_ID = RepairItemSon;
                RepairRow.REASON_CODE = RepairReseaonCode;
                RepairRow.DESCRIPTION = VDescription;
                RepairRow.FAIL_LOCATION = Location;
                RepairRow.FAIL_CODE = FailCodeRow.FAIL_CODE;
                RepairRow.KEYPART_SN = "";
                RepairRow.NEW_KEYPART_SN = RepairKeypartSN;
                RepairRow.TR_SN = VTR_SN;
                RepairRow.KP_NO = VKPNO;
                RepairRow.MFR_NAME = VMFRName;
                RepairRow.DATE_CODE = VDateCode;
                RepairRow.LOT_CODE = VLotCode;
                RepairRow.REPAIR_EMP = VRepairer;
                RepairRow.REPAIR_TIME = Station.GetDBDateTime();
                RepairRow.EDIT_EMP = Station.LoginUser.EMP_NO;
                RepairRow.EDIT_TIME = Station.GetDBDateTime();

                string StrRes = Station.SFCDB.ExecSQL(RepairRow.GetInsertString(Station.DBType));
                if (StrRes == "1")
                {
                    Row_R_REPAIR_FAILCODE FRow = (Row_R_REPAIR_FAILCODE)RepairFailcode.GetObjByID(VFailCodeID, Station.SFCDB);
                    FRow.REPAIR_FLAG = "1";  //執行完維修動作後更新R_REPAIR_FAILCODE   FLAG=1 
                    FRow.EDIT_TIME = Station.GetDBDateTime();
                    UpdateSql = FRow.GetUpdateString(Station.DBType);
                    Station.SFCDB.ExecSQL(UpdateSql);
                    Station.SFCDB.CommitTrain();
                    Station.AddMessage("MES00000105", new string[] { SnObject.SerialNo, VFailCodeID }, StationMessageState.Pass);
                }
                else
                {
                    Station.SFCDB.RollbackTrain();
                    Station.AddMessage("MES00000083", new string[] { SnObject.SerialNo, VFailCodeID }, StationMessageState.Fail);
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }


        /// <summary>
        ///產品所有維修動作完成Action
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairFinishAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            string UpdateSql = "";
            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            T_r_repair_action RepairAction = new T_r_repair_action(Station.SFCDB, Station.DBType);
            Row_r_repair_action RepairRow = (Row_r_repair_action)RepairAction.NewRow();
            T_R_REPAIR_MAIN RMain = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            List<R_REPAIR_MAIN> RepairMainInfo = new List<R_REPAIR_MAIN>();
            List<R_REPAIR_FAILCODE> FailCodeInfo = new List<R_REPAIR_FAILCODE>();
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string DeviceName = Station.StationName;

            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SnObject = (SN)SNSession.Value;

            //獲取 DEVICE1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }

            try
            {
                RepairMainInfo = RMain.GetRepairMainBySN(Station.SFCDB, SnObject.SerialNo);
                if (RepairMainInfo != null && RepairMainInfo[0].CLOSED_FLAG == "0")
                {
                    FailCodeInfo = RepairFailcode.CheckSNRepairFinishAction(Station.SFCDB, SnObject.SerialNo, RepairMainInfo[0].ID);
                    if (FailCodeInfo.Count != 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000106", new string[] { SnObject.SerialNo, FailCodeInfo[0].ID })); ///未维修完成的无法update repair_main 表信息
                    }
                    else
                    {
                        Station.SFCDB.BeginTrain();
                        table.RecordPassStationDetail(SnObject.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);   //添加过站记录

                        //執行完所有的維修動作後才能更新R_REPAIR_MAIN  FLAG=1 
                        Row_R_REPAIR_MAIN FRow = (Row_R_REPAIR_MAIN)RMain.GetObjByID(RepairMainInfo[0].ID, Station.SFCDB);
                        FRow.CLOSED_FLAG = "1";
                        FRow.EDIT_TIME = Station.GetDBDateTime();
                        UpdateSql = FRow.GetUpdateString(Station.DBType);
                        Station.SFCDB.ExecSQL(UpdateSql);

                        //執行完所有的維修動作後 更新R_SN  FLAG=0
                        Row_R_SN SnRow = (Row_R_SN)table.GetObjByID(SnObject.ID, Station.SFCDB);
                        SnRow.REPAIR_FAILED_FLAG = "0";
                        SnRow.EDIT_TIME = Station.GetDBDateTime();
                        UpdateSql = SnRow.GetUpdateString(Station.DBType);
                        Station.SFCDB.ExecSQL(UpdateSql);

                        Station.SFCDB.CommitTrain();

                    }
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }


        /// <summary>
        ///SN批次解鎖ACTION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFQCLotUnlockAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_LOT_DETAIL Ulotdetail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);

            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SessionSNorLotNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSNorLotNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SessionSNorLotNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string SnOrLotno = SessionSNorLotNo.Value.ToString();

            try
            {
                //Station.SFCDB.BeginTrain();
                Ulotdetail.UnLockLotBySnOrLotNo(SnOrLotno, Station.LoginUser.EMP_NO, Station.SFCDB);
                Station.AddMessage("MES00000173", new string[] { SnOrLotno }, StationMessageState.Pass); //回饋消息到前台
                //Station.SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                //Station.SFCDB.RollbackTrain();
                throw ex;
            }
        }

        public static void SNStockInPassAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string DeviceName = "";
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession SessionDevice = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SessionDevice == null)
            {
                DeviceName = Station.StationName;
            }
            SN SNObj = (SN)SessionSN.Value;

            T_R_SN T_R_Sn = new T_R_SN(Station.SFCDB, Station.DBType);
            Row_R_SN Rows = (Row_R_SN)T_R_Sn.GetObjByID(SNObj.ID, Station.SFCDB);
            Rows.STOCK_STATUS = "1";
            Rows.STOCK_IN_TIME = Station.GetDBDateTime();
            Rows.EDIT_TIME = Station.GetDBDateTime();
            Station.SFCDB.ExecSQL(Rows.GetUpdateString(Station.DBType));

            T_R_Sn.RecordPassStationDetail(SNObj.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);

            //R_SN R_Sn = T_R_Sn.LoadSN(SNObj.SerialNo, Station.SFCDB);
            Station.AddMessage("MES00000063", new string[] { SNObj.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        public static void SNPreSCRAPPassAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string DeviceName = "";
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession SessionDevice = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SessionDevice == null)
            {
                DeviceName = Station.StationName;
            }
            SN SNObj = (SN)SessionSN.Value;

            T_R_SN T_R_Sn = new T_R_SN(Station.SFCDB, Station.DBType);
            Row_R_SN Rows = (Row_R_SN)T_R_Sn.GetObjByID(SNObj.ID, Station.SFCDB);
            Rows.CURRENT_STATION = Station.StationName;
            Rows.NEXT_STATION = "MRB";
            Rows.SCRAPED_FLAG = "1";
            Rows.SCRAPED_TIME = Station.GetDBDateTime();
            Rows.EDIT_TIME = Station.GetDBDateTime();
            Station.SFCDB.ExecSQL(Rows.GetUpdateString(Station.DBType));

            T_R_Sn.RecordPassStationDetail(SNObj.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);

            Station.AddMessage("MES00000063", new string[] { SNObj.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        public static void CounterAddAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession CounterSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (CounterSession == null)
            {
                CounterSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(CounterSession);
                CounterSession.Value = 1;
            }
            else
            {
                CounterSession.Value = (int)CounterSession.Value + 1;
            }
        }


        /// <summary>
        /// 解除SN的keyparts綁定關係,把R_SN_KEYPART_DETAIL表的VAIID改為0   0表示無效，1代表有效
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNUnlinkAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string SerialSn = SNSession.Value.ToString();

            List<R_SN_KEYPART_DETAIL> LinkSN = new List<R_SN_KEYPART_DETAIL>();
            T_R_SN_KEYPART_DETAIL stk = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            string UpdateSql = "";
            string KeyPartSnID = null;
            MESStationSession LinksnSesssion = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LinksnSesssion == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else if (LinksnSesssion.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (LinksnSesssion != null)
            {
                LinkSN = (List<R_SN_KEYPART_DETAIL>)LinksnSesssion.Value;
                try
                {
                    Station.SFCDB.BeginTrain();
                    for (int i = 0; i < LinkSN.Count; i++)
                    {
                        KeyPartSnID = LinkSN[i].ID;
                        Row_R_SN_KEYPART_DETAIL RowKeypartSNID = (Row_R_SN_KEYPART_DETAIL)stk.GetObjByID(KeyPartSnID, Station.SFCDB);
                        RowKeypartSNID.VALID = "0";
                        UpdateSql = RowKeypartSNID.GetUpdateString(Station.DBType);
                        Station.SFCDB.ExecSQL(UpdateSql);
                    }

                    Station.SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    Station.SFCDB.RollbackTrain();
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 解除SN的keyparts綁定關係後，把R_SN最新的VALID_FLAG=0的數據恢復為VALID_FLAG=1,並把和該SN綁定的板子打入MRB
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNUnlinkFinishallAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int result = 0;
            R_MRB New_R_MRB = new R_MRB();
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);

            List<R_SN_KEYPART_DETAIL> LinkSN = new List<R_SN_KEYPART_DETAIL>();
            T_R_SN_KEYPART_DETAIL stk = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN shk = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN> GetRsnList = new List<R_SN>();
            List<R_MRB> GetMRBList = new List<R_MRB>();

            string UpdateSql = "";
            string RSNID = null;

            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN SerialSn = (SN)SNSession.Value;
            try
            {
                //註釋掉R_SN  VALID_FLAG=1的數據，恢復之前VALID_FLAG=0 時間最晚的數據
                Station.SFCDB.BeginTrain();
                Row_R_SN RowSN = (Row_R_SN)shk.GetObjByID(SerialSn.ID, Station.SFCDB);
                RowSN.VALID_FLAG = "0";
                UpdateSql = RowSN.GetUpdateString(Station.DBType);
                Station.SFCDB.ExecSQL(UpdateSql);

                GetRsnList = shk.GetINVaildSN(SerialSn.SerialNo, SerialSn.ID, Station.SFCDB);

                if (GetRsnList != null && GetRsnList.Count > 0)
                {
                    Row_R_SN RowInvailSN = (Row_R_SN)shk.GetObjByID(GetRsnList[0].ID, Station.SFCDB);
                    RowInvailSN.VALID_FLAG = "1";
                    UpdateSql = RowInvailSN.GetUpdateString(Station.DBType);
                    Station.SFCDB.ExecSQL(UpdateSql);
                }

                Station.SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                Station.SFCDB.RollbackTrain();
                throw ex;
            }

            string FROM_STORAGE = "";
            string TO_STORAGE = "";

            GetMRBList = TR_MRB.GetMrbBySN(SerialSn.SerialNo, Station.SFCDB);
            if (GetMRBList != null && GetMRBList.Count > 0)
            {
                FROM_STORAGE = GetMRBList[0].FROM_STORAGE;
                TO_STORAGE = GetMRBList[0].TO_STORAGE;
            }
            //處理被LINK的數據 
            MESStationSession LinksnSesssion = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LinksnSesssion != null)
            {
                LinkSN = (List<R_SN_KEYPART_DETAIL>)LinksnSesssion.Value;
                try
                {
                    Station.SFCDB.BeginTrain();

                    for (int i = 0; i < LinkSN.Count; i++)
                    {
                        RSNID = LinkSN[i].R_SN_ID;
                        Row_R_SN RowInvailSN = (Row_R_SN)shk.GetObjByID(RSNID, Station.SFCDB);

                        //添加一筆MRB記錄
                        //給new_r_mrb賦值
                        New_R_MRB.ID = TR_MRB.GetNewID(Station.BU, Station.SFCDB, Station.DBType);
                        New_R_MRB.SN = RowInvailSN.SN;
                        New_R_MRB.WORKORDERNO = RowInvailSN.WORKORDERNO;
                        New_R_MRB.NEXT_STATION = RowInvailSN.NEXT_STATION;
                        New_R_MRB.SKUNO = RowInvailSN.SKUNO;
                        New_R_MRB.FROM_STORAGE = FROM_STORAGE;
                        New_R_MRB.TO_STORAGE = TO_STORAGE;
                        New_R_MRB.REWORK_WO = "";//空
                        New_R_MRB.CREATE_EMP = Station.LoginUser.EMP_NO;
                        New_R_MRB.CREATE_TIME = Station.GetDBDateTime();
                        New_R_MRB.MRB_FLAG = "0";
                        New_R_MRB.SAP_FLAG = "0";
                        New_R_MRB.EDIT_EMP = Station.LoginUser.EMP_NO;
                        New_R_MRB.EDIT_TIME = New_R_MRB.CREATE_TIME;
                        result = TR_MRB.Add(New_R_MRB, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + RowInvailSN.SN, "ADD" }));
                        }
                    }

                    Station.SFCDB.CommitTrain();

                    Station.AddMessage("MES00000063", new string[] { SerialSn.SerialNo }, StationMessageState.Pass);
                }
                catch (Exception ex)
                {
                    Station.SFCDB.RollbackTrain();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// add by ZGJ
        /// 替換 SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReplaceSnAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string NewSn = string.Empty;
            string OldSn = string.Empty;
            SN SnObj = null;
            SN OldSnObj = null;
            string sql = string.Empty;
            OleExecPool APDBPool = Station.DBS["APDB"];
            OleExec APDB = null;
            int result = 0;
            T_R_REPAIR_MAIN RepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            T_R_REPAIR_FAILCODE RepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            T_R_SN RSn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN_STATION_DETAIL RSnStationDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN_KEYPART_DETAIL RSnKeypartDetail = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, Station.DBType);
            T_R_REPLACE_SN ReplaceSn = new T_R_REPLACE_SN(Station.SFCDB, Station.DBType);
            R_REPLACE_SN ReplaceSnObj = new R_REPLACE_SN();
            T_R_PANEL_SN RPanelSn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            T_R_MRB RMrb = new T_R_MRB(Station.SFCDB, Station.DBType);

            MESStationSession OldSnSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (OldSnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            var OldObj = OldSnSession.Value;
            if (OldObj is string)
            {
                OldSn = OldSnSession.Value.ToString();
                OldSnObj = new SN(OldSn, Station.SFCDB, Station.DBType);
            }
            else
            {
                OldSnObj = (SN)OldSnSession.Value;
                OldSn = OldSnObj.SerialNo;

            }


            MESStationSession NewSnSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[1].SESSION_TYPE) && t.SessionKey.Equals(Paras[1].SESSION_KEY));
            if (NewSnSession == null)
            {
                NewSn = Input.Value.ToString();
                SnObj = new SN(NewSn, Station.SFCDB, Station.DBType);
                NewSnSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = SnObj };
            }
            var NewObj = NewSnSession.Value;
            if (NewObj is string)
            {
                NewSn = NewSnSession.Value.ToString();
            }
            else
            {
                NewSn = ((SN)NewSnSession.Value).SerialNo;
            }

            try
            {
                //Station.SFCDB.BeginTrain();
                APDB = Station.APDB;


                RSn.ReplaceSn(NewSn, OldSn, Station.SFCDB, Station.DBType);
                RSn.RecordPassStationDetail(NewSn, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
                RSnStationDetail.ReplaceSnStationDetail(NewSn, OldSn, Station.SFCDB, Station.DBType);
                RepairMain.ReplaceSnRepairFailMain(NewSn, OldSn, Station.SFCDB, Station.DBType);
                RepairFailCode.ReplaceSnRepairFailCode(NewSn, OldSn, Station.SFCDB, Station.DBType);
                RSnKeypartDetail.ReplaceSnKeypartDetail(NewSn, OldSn, Station.SFCDB, Station.DBType);
                //add update RPanelSn ReplaceRMrb by wuq 20180416
                RPanelSn.ReplaceRPanelSn(NewSn, OldSn, Station.SFCDB, Station.DBType);
                RMrb.ReplaceRMrb(NewSn, OldSn, Station.SFCDB, Station.DBType);

                ReplaceSnObj.OLD_SN_ID = OldSnObj.ID;
                ReplaceSnObj.OLD_SN = OldSnObj.SerialNo;
                ReplaceSnObj.NEW_SN = NewSn;
                ReplaceSnObj.EDIT_TIME = Station.GetDBDateTime();
                ReplaceSnObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                ReplaceSn.AddReplaceSNRecord(ReplaceSnObj, Station.BU, Station.SFCDB, Station.DBType);


                sql = $@"UPDATE MES4.R_SN_LINK R SET R.P_SN='{NewSn}' WHERE R.P_SN='{OldSn}'";
                result = APDB.ExecSqlNoReturn(sql, null);
                if (result == 0)
                {
                    //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000197",new string[] { OldSn, "MES4.R_SN_LINK" });
                    //    throw new MESReturnMessage(ErrMessage);
                }

                sql = $@"UPDATE MES4.R_TR_PRODUCT_DETAIL R SET R.P_SN='{NewSn}' WHERE R.P_SN='{OldSn}'";
                result = APDB.ExecSqlNoReturn(sql, null);
                //if (result == 0)
                //{
                //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000197",new string[] { OldSn, "MES4.R_TR_PRODUCT_DETAIL" });
                //    throw new MESReturnMessage(ErrMessage);
                //}


                if (Paras.Count == 3)
                {
                    MESStationSession ClearFlagSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = "true" };
                    Station.StationSession.Add(ClearFlagSession);
                }
                Station.AddMessage("MES00000198", new string[] { NewSn, OldSn }, StationMessageState.Pass);


            }
            catch (MESReturnMessage ex)
            {

                throw ex;
            }
            finally
            {
                //APDBPool.Return(APDB);
            }
        }

        /// <summary>
        /// HWD RMA過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RMAPassAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN Table_RSn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_C_ROUTE Table_CRoute = new T_C_ROUTE(Station.SFCDB, Station.DBType);
            string StrSN = "";
            string nextStation = "";
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (SKUSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SKU SkuObj = (SKU)SKUSession.Value;
            //OldSn = OldSnObj.SerialNo;

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[1].SESSION_TYPE) && t.SessionKey.Equals(Paras[1].SESSION_KEY));
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            //SN SnObj = (SN)SNSession.Value;
            StrSN = SNSession.Value.ToString();
            try
            {
                R_SN r_sn = null;
                SN snObj = new SN();
                r_sn = snObj.LoadSN(StrSN, Station.SFCDB);
                if (r_sn != null)
                {
                    Table_RSn.updateValid_Flag(r_sn.ID, "0", Station.LoginUser.EMP_NO, Station.SFCDB);
                    nextStation = r_sn.NEXT_STATION;
                }
                else
                {
                    nextStation = "SMT_FQC";
                }
                // add by fgg 2018.05.03 R_MRB 表加一筆記錄，以便可以掃REWORK
                T_R_MRB t_r_mrb = new T_R_MRB(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_MRB rowMrb = (Row_R_MRB)t_r_mrb.NewRow();
                rowMrb.ID = t_r_mrb.GetNewID(Station.BU, Station.SFCDB);
                rowMrb.SN = StrSN;
                rowMrb.WORKORDERNO = "RMA";
                rowMrb.NEXT_STATION = nextStation;//跟PE杜軍協商掃入RMA的條碼在MRB表的下一站默認為SMT_FQC
                rowMrb.SKUNO = SkuObj.SkuNo;
                rowMrb.FROM_STORAGE = "";
                rowMrb.TO_STORAGE = "";
                rowMrb.REWORK_WO = "";//空
                rowMrb.CREATE_EMP = Station.LoginUser.EMP_NO;
                rowMrb.CREATE_TIME = Station.GetDBDateTime();
                rowMrb.MRB_FLAG = "0";
                rowMrb.SAP_FLAG = "0";
                rowMrb.EDIT_EMP = Station.LoginUser.EMP_NO;
                rowMrb.EDIT_TIME = Station.GetDBDateTime();
                Station.SFCDB.ExecSQL(rowMrb.GetInsertString(DB_TYPE_ENUM.Oracle));

                Row_C_ROUTE Row_C_Route = (Row_C_ROUTE)Table_CRoute.GetRouteBySkuno(SkuObj.SkuId, Station.SFCDB, Station.DBType);
                Table_RSn.InsertRMASN(StrSN, "RMA", SkuObj.SkuNo, Row_C_Route.ID, "", "RMA", "REWORK", Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB, "WDN1", "FRESH");
                Table_RSn.RecordPassStationDetail(StrSN, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
                Station.AddMessage("MES00000063", new string[] { StrSN }, StationMessageState.Pass); //回饋消息到前台
            }
            catch (Exception ex)
            {
                Station.AddMessage("MES00000233" + ";" + ex.Message, new string[] { StrSN }, StationMessageState.Fail);
            }
        }

        /// <summary>
        /// add by fgg 2018.5.14
        /// SILOADING 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNSILoadingPassAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionInputQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            MESStationSession sessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionExtQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                OleExec sfcdb = Station.SFCDB;
                DB_TYPE_ENUM sfcdbType = Station.DBType;
                Dictionary<string, object> dicNextStation;
                string nextStation = "";
                int result;
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, sfcdbType);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, sfcdbType);
                objWorkorder = (WorkOrder)sessionWO.Value;
                dicNextStation = t_c_route_detail.GetNextStations(objWorkorder.RouteID, Station.StationName, sfcdb);
                nextStation = ((List<string>)dicNextStation["NextStations"])[0].ToString();

                //工單關節
                if (objWorkorder.CLOSED_FLAG.Equals("1"))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000100"));
                }
                //已經投滿
                if (objWorkorder.INPUT_QTY >= objWorkorder.WORKORDER_QTY)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000247", new string[] { objWorkorder.WorkorderNo }));
                }

                #region  寫入r_sn,r_sn_station_detail
                T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
                R_SN r_sn = new R_SN();
                r_sn.ID = t_r_sn.GetNewID(Station.BU, sfcdb, sfcdbType);
                r_sn.SN = sessionSn.Value.ToString();
                r_sn.SKUNO = objWorkorder.SkuNO;
                r_sn.WORKORDERNO = objWorkorder.WorkorderNo;
                r_sn.PLANT = objWorkorder.PLANT;
                r_sn.ROUTE_ID = objWorkorder.RouteID;
                r_sn.STARTED_FLAG = "1";
                r_sn.START_TIME = Station.GetDBDateTime();
                r_sn.PACKED_FLAG = "0";
                r_sn.COMPLETED_FLAG = "0";
                r_sn.SHIPPED_FLAG = "0";
                r_sn.REPAIR_FAILED_FLAG = "0";
                r_sn.CURRENT_STATION = Station.StationName;
                r_sn.NEXT_STATION = nextStation;
                r_sn.KP_LIST_ID = objWorkorder.KP_LIST_ID;
                r_sn.CUST_PN = objWorkorder.CUST_PN;
                r_sn.VALID_FLAG = "1";
                r_sn.STOCK_STATUS = "0";
                r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                r_sn.EDIT_TIME = Station.GetDBDateTime();
                result = t_r_sn.AddNewSN(r_sn, sfcdb);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + sessionSn.Value.ToString(), "ADD" }));
                }
                t_r_sn.RecordPassStationDetail(sessionSn.Value.ToString(), Station.Line, Station.StationName, Station.StationName, Station.BU, sfcdb);
                #endregion

                #region  寫入 r_sn_kp 
                T_C_KP_LIST c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
                if (objWorkorder.KP_LIST_ID != "" && c_kp_list.KpIDIsExist(objWorkorder.KP_LIST_ID, sfcdb))
                {
                    SN snObject = new SN();
                    snObject.InsertR_SN_KP(objWorkorder, r_sn, sfcdb, Station, sfcdbType);
                }
                #endregion

                //更新工單投入數量

                result = Convert.ToInt32(t_r_wo_base.AddCountToWo(objWorkorder.WorkorderNo, 1, Station.SFCDB));
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + objWorkorder.WorkorderNo, "UPDATE" }));
                }

                Row_R_WO_BASE newRowWo = t_r_wo_base.LoadWorkorder(objWorkorder.WorkorderNo, sfcdb);
                sessionInputQty.Value = newRowWo.INPUT_QTY;
                sessionExtQty.Value = newRowWo.WORKORDER_QTY - newRowWo.INPUT_QTY;

                if (newRowWo.INPUT_QTY >= newRowWo.WORKORDER_QTY)
                {
                    Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
                    Station.AddMessage("MES00000247", new string[] { newRowWo.WORKORDERNO }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 更改未綁定的SN的Keypart信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void UpdateRSNKPAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession snObjectList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (snObjectList == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession sessionInputString = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionInputString == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            if (sessionInputString.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                List<R_SN> snList = new List<R_SN>();
                snList = (List<R_SN>)snObjectList.Value;
                SN snObject = new SN();
                snObject.UpdateSNKP(objWorkorder, snList, Station);
                Station.AddMessage("MES00000063", new string[] { sessionInputString.Value.ToString() }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 記錄SN抽檢信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RecordSnObaSampleInfo(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO").Value.ToString();
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL tRLotDetail = new T_R_LOT_DETAIL(Station.SFCDB,Station.DBType);
            T_R_SN tRSn = new T_R_SN(Station.SFCDB, Station.DBType);
            Row_R_LOT_DETAIL rowRLotDetail = (Row_R_LOT_DETAIL)tRLotDetail.NewRow();
            R_SN rSn = tRSn.GetDetailBySN(snSession.Value.ToString(),Station.SFCDB);
            Row_R_LOT_STATUS rowRLotStatus = tRLotStatus.GetByLotNo(LotNo, Station.SFCDB);
            //Lot{0}不處於待抽檢狀態,請檢查!
            if(!rowRLotStatus.CLOSED_FLAG.Equals("1"))
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180530114417", new string[] { LotNo }));
            rowRLotDetail.ID = tRLotDetail.GetNewID(Station.BU, Station.SFCDB);
            rowRLotDetail.LOT_ID = rowRLotStatus.ID;
            rowRLotDetail.SN = snSession.Value.ToString();
            rowRLotDetail.WORKORDERNO = rSn.WORKORDERNO;
            rowRLotDetail.CREATE_DATE = tRLotDetail.GetDBDateTime(Station.SFCDB);
            rowRLotDetail.STATUS = Paras[1].VALUE.Equals("PASS")?"1":"2";
            rowRLotDetail.EDIT_EMP = Station.LoginUser.EMP_NO;
            rowRLotDetail.EDIT_TIME = rowRLotDetail.CREATE_DATE;

            if (Paras[1].VALUE.Equals("PASS"))
                rowRLotStatus.PASS_QTY++;
            else
                rowRLotStatus.FAIL_QTY++;

            rowRLotStatus.EDIT_EMP = Station.LoginUser.EMP_NO;
            rowRLotStatus.EDIT_TIME = rowRLotDetail.CREATE_DATE;
            if (rowRLotStatus.REJECT_QTY <= rowRLotStatus.FAIL_QTY)
            {
                rowRLotStatus.CLOSED_FLAG = "2";
                rowRLotStatus.LOT_STATUS_FLAG = "2";
                //鎖定LOT所有SN
                T_R_SN_LOCK tRSnLock = new T_R_SN_LOCK(Station.SFCDB, Station.DBType);
                tRSnLock.LockSnInOba(LotNo, Station.SFCDB);
            }
            else if(rowRLotStatus.LOT_QTY<= rowRLotStatus.PASS_QTY+ rowRLotStatus.FAIL_QTY)
            {
                rowRLotStatus.CLOSED_FLAG = "2";
                rowRLotStatus.LOT_STATUS_FLAG = "1";
                //批量過站;
                List<R_SN> rSnList = new List<R_SN>();
                rSnList = tRSn.GetObaSnListByLotNo(LotNo, Station.SFCDB);
                tRSn.LotsPassStation(rSnList, Station.Line, rSn.NEXT_STATION, rSn.NEXT_STATION, Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB); // 過站
                //記錄通過數 ,UPH
                foreach (var snobj in rSnList)
                {
                    tRSn.RecordYieldRate(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                    tRSn.RecordUPH(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                }
            }
            Station.SFCDB.ExecSQL(rowRLotDetail.GetInsertString(Station.DBType));
            Station.SFCDB.ExecSQL(rowRLotStatus.GetUpdateString(Station.DBType));
            #region 加載界面信息
            if (rowRLotStatus.CLOSED_FLAG == "2")//抽檢完清空界面信息
            {
                MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[11].SESSION_TYPE);
                s.DataForUse.Clear();
                Station.StationSession.Clear();
                MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == "SN");
                MESStationInput packInput = Station.Inputs.Find(t => t.DisplayName == "PACKNO");
                MESStationInput failSnInput = Station.Inputs.Find(t => t.DisplayName == "FailSn");
                MESStationInput scanTypeInput = Station.Inputs.Find(t => t.DisplayName == "ScanType");
                MESStationInput failCodeInput = Station.Inputs.Find(t => t.DisplayName == "FailCode");
                MESStationInput locationInput = Station.Inputs.Find(t => t.DisplayName == "Location");
                MESStationInput failDescInput = Station.Inputs.Find(t => t.DisplayName == "FailDesc");
                packInput.Visable = true;
                snInput.Visable = false;
                scanTypeInput.Visable = false;
                failCodeInput.Visable = false;
                locationInput.Visable = false;
                failDescInput.Visable = false;
                failSnInput.Visable = false;
            }
            else//未抽檢完=>更新界面信息,設置NextInput
            {
                Station.NextInput = Station.Inputs.Find(t => t.DisplayName.Equals(Paras[0].SESSION_TYPE));
                MESStationSession lotNoSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                MESStationSession skuNoSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                MESStationSession aqlSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                MESStationSession lotQtySession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                MESStationSession sampleQtySession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                MESStationSession rejectQtySession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                MESStationSession sampleQtyWithAqlSession = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                MESStationSession passQtySession = new MESStationSession() { MESDataType = Paras[9].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[9].SESSION_KEY, ResetInput = Input };
                MESStationSession failQtySession = new MESStationSession() { MESDataType = Paras[10].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[10].SESSION_KEY, ResetInput = Input };

                Station.StationSession.Clear();
                Station.StationSession.Add(lotNoSession);
                Station.StationSession.Add(skuNoSession);
                Station.StationSession.Add(aqlSession);
                Station.StationSession.Add(lotQtySession);
                Station.StationSession.Add(sampleQtySession);
                Station.StationSession.Add(rejectQtySession);
                Station.StationSession.Add(sampleQtyWithAqlSession);
                Station.StationSession.Add(passQtySession);
                Station.StationSession.Add(failQtySession);

                lotNoSession.Value = rowRLotStatus.LOT_NO;
                skuNoSession.Value = rowRLotStatus.SKUNO;
                aqlSession.Value = rowRLotStatus.AQL_TYPE;
                lotQtySession.Value = rowRLotStatus.LOT_QTY;
                sampleQtySession.Value = rowRLotStatus.SAMPLE_QTY;
                rejectQtySession.Value = rowRLotStatus.REJECT_QTY;
                sampleQtyWithAqlSession.Value = rowRLotStatus.PASS_QTY + rowRLotStatus.FAIL_QTY;
                passQtySession.Value = rowRLotStatus.PASS_QTY;
                failQtySession.Value = rowRLotStatus.FAIL_QTY;
            }
            #endregion

        }
    }
}