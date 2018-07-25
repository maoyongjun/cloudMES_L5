using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESStation.LogicObject;
using MESStation.MESReturnView.Station;
using System.Collections;
using MESDataObject;
using System.Data;
using MESDBHelper;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class PanelActions
    {
        /// <summary>
        /// Panel 投入
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelInputAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            WorkOrder WorkOrder = null;
            T_R_SN SnTable = null;
            T_R_WO_BASE WoTable = null;
            T_R_PANEL_SN PanelTable = null;
            double LinkQty = 0d;
            string PanelSn = string.Empty;
            List<R_SN> SNs = null;
            R_SN OriginalSN = null;
            List<string> SNIds = null;
            List<R_PANEL_SN> RPanelSNs = new List<R_PANEL_SN>();
            SN SNObj = new SN();
            string NextStation = "";
            int SeqNo = 1;
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;

            if (Paras.Count != 5)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "4", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //Panel1
            MESStationSession PanelSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //Wo1
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //LinkQty1
            MESStationSession LinkCountSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (LinkCountSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }

            MESStationSession sessionWOInputQty = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionWOInputQty == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            try
            {
                PanelSn = PanelSnSession.Value.ToString();
                WorkOrder = (WorkOrder)WoSession.Value;
                LinkQty = double.Parse(LinkCountSession.Value.ToString());
                SNs = new List<R_SN>((int)LinkQty);

                OriginalSN = new R_SN();
                OriginalSN.SKUNO = WorkOrder.SkuNO;
                OriginalSN.WORKORDERNO = WorkOrder.WorkorderNo;
                OriginalSN.PLANT = WorkOrder.PLANT;
                OriginalSN.ROUTE_ID = WorkOrder.RouteID;
                OriginalSN.STARTED_FLAG = "1";
                OriginalSN.PACKED_FLAG = "0";
                OriginalSN.COMPLETED_FLAG = "0";
                OriginalSN.SHIPPED_FLAG = "0";
                OriginalSN.REPAIR_FAILED_FLAG = "0";
                OriginalSN.CURRENT_STATION = Station.StationName;
                OriginalSN.CUST_PN = WorkOrder.CUST_PN;
                OriginalSN.SCRAPED_FLAG = "0";
                OriginalSN.PRODUCT_STATUS = "FRESH";
                OriginalSN.REWORK_COUNT = 0d;
                OriginalSN.VALID_FLAG = "1";
                OriginalSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                for (int i = 1; i <= LinkQty; i++)
                {
                    SNs.Add(OriginalSN);
                }

                SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
                SNIds = SnTable.AddToRSn(SNs, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB); //批量插入到 R_SN 中
                foreach (string SNId in SNIds)
                {
                    R_PANEL_SN RPanelSN = new R_PANEL_SN();
                    RPanelSN.SN = SNId;
                    RPanelSN.PANEL = PanelSn;
                    RPanelSN.WORKORDERNO = WorkOrder.WorkorderNo;
                    RPanelSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                    RPanelSN.SEQ_NO = SeqNo++;
                    RPanelSNs.Add(RPanelSN);
                }
                PanelTable = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
                PanelTable.AddSnToPanel(RPanelSNs, Station.BU, Station.SFCDB); //批量插入到 R_PANEL_SN 中

                #region 寫入 r_sn_kp add by fgg 2018.5.23
                T_C_KP_LIST c_kp_list = new T_C_KP_LIST(Station.SFCDB, Station.DBType);
                if (WorkOrder.KP_LIST_ID != "" && c_kp_list.KpIDIsExist(WorkOrder.KP_LIST_ID, Station.SFCDB))
                {
                    SN snObject = new SN();
                    foreach (R_SN r_sn in SNs)
                    {
                        snObject.InsertR_SN_KP(WorkOrder, r_sn, Station.SFCDB, Station, Station.DBType);
                    }
                }
                #endregion

                //調用 SN 過站 插入記錄到過站記錄中 未完待續
                WoTable = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
                WoTable.AddCountToWo(WorkOrder.WorkorderNo, LinkQty, Station.SFCDB); // 更新 R_WO_BASE 中的數據

                Route routeDetail = new Route(WorkOrder.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<string> snStationList = new List<string>();
                List<RouteDetail> routeDetailList = routeDetail.DETAIL;
                RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == Station.StationName).FirstOrDefault();
                string nextStation1 = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
                snStationList.Add(nextStation1);
                if (R.DIRECTLINKLIST != null)
                {
                    foreach (var item in R.DIRECTLINKLIST)
                    {
                        snStationList.Add(item.STATION_NAME);
                    }
                }

                Row_R_WO_BASE newRowWo = WoTable.LoadWorkorder(WorkOrder.WorkorderNo, Station.SFCDB);
                sessionWOInputQty.Value = newRowWo.INPUT_QTY;

                NextStation = SNObj.StringListToString(snStationList);
                Station.AddMessage("MES00000055", new string[] { PanelSnSession.Value.ToString(), NextStation }, StationMessageState.Pass); //回饋消息到前台
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static void PanelLogAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
            try
            {
                string strSn = Input.Value.ToString();
                SN sn = new SN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_SN_STATION_DETAIL cStation = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_SN_STATION_DETAIL rStation = (Row_R_SN_STATION_DETAIL)cStation.NewRow();
                rStation.ID = sn.ID;
                rStation.NEXT_STATION = sn.NextStation;
                rStation.SKUNO = sn.SkuNo;
                rStation.WORKORDERNO = sn.WorkorderNo;
                rStation.ROUTE_ID = sn.RouteID;
                rStation.PLANT = sn.Plant;
                rStation.STARTED_FLAG = sn.StartedFlag;
                rStation.START_TIME = (DateTime)sn.StartTime;
                rStation.PACKED_FLAG = sn.PackedFlag;
                rStation.COMPLETED_FLAG = sn.CompletedFlag;
                rStation.COMPLETED_TIME = sn.CompletedTime ?? DateTime.Now;
                rStation.SHIPPED_FLAG = sn.ShippedFlag;
                rStation.SHIPDATE = sn.ShipDate ?? DateTime.Now;
                rStation.REPAIR_FAILED_FLAG = sn.RepairFailedFlag;
                rStation.CURRENT_STATION = sn.CurrentStation;
                rStation.KP_LIST_ID = sn.KeyPartList[0].ID;
                rStation.PO_NO = sn.PONO;
                rStation.CUST_ORDER_NO = sn.CustomerOrderNo;
                rStation.CUST_PN = "";
                rStation.BOXSN = sn.BoxSN;
                rStation.SCRAPED_FLAG = sn.ScrapedFlag;
                rStation.SCRAPED_TIME = sn.ScrapedTime ?? DateTime.Now;
                rStation.PRODUCT_STATUS = sn.ProductStatus;
                rStation.REWORK_COUNT = sn.ReworkCount;
                rStation.VALID_FLAG = sn.ValidFlag;
                rStation.EDIT_EMP = "";
                rStation.EDIT_TIME = DateTime.Now;
                string strRet = Station.SFCDB.ExecSQL(rStation.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    throw new Exception("ERROR!");
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void PanelSnInputAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
            MESStationInput input = null;
            string strPanel = Input.Value.ToString();
            Panel panel = new Panel();
            panel.Init(strPanel, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Hashtable tempTable = new Hashtable();
            tempTable.Add("BU", Station.BU);
            tempTable.Add("Panel", panel.PanelNo);
            tempTable.Add("WO", s.Value);
            tempTable.Add("User", Station.User.EMP_NO);
            Boolean b = panel.CreatePanel(tempTable, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            if (b == false)
            {
                Station.AddMessage("MES00000029", new string[] { "Panel", strPanel.ToString() }, MESReturnView.Station.StationMessageState.Fail);
            }
            input = Station.FindInputByName("Panel");
            Station.NextInput = input;
            Station.AddMessage("MES00000029", new string[] { "Panel", strPanel.ToString() }, MESReturnView.Station.StationMessageState.Pass);

            string strSql = "update r_sn set current_station=:CurrentStation,next_station=:nextStation where sn=:sn";
            int count = Station.SFCDB.ExecSqlNoReturn(strSql, new System.Data.OleDb.OleDbParameter[3] { new System.Data.OleDb.OleDbParameter("CurrentStation", Station.StationName), new System.Data.OleDb.OleDbParameter("nextStation", "Insp"), new System.Data.OleDb.OleDbParameter("sn", strPanel) });
        }

        /// <summary>
        /// 補 Allpart 資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddAPRecordsAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string Message = string.Empty;
            //modify by LLF 2018-01-27
            //R_SN SN = null;
            string StrSN = "";
            string StrWo = "";
            string StrStation = "";

            if (Paras.Count <= 1)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "1", Paras.Count.ToString() });
                throw new MESReturnMessage(Message);
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            //SN = (R_SN)SnSession.Value;
            StrSN = SnSession.InputValue.ToString();

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            //SN = (R_SN)SnSession.Value;
            StrWo = WoSession.Value.ToString();
            StrStation = Station.StationName;

            Message = table.AddAPRecords(StrSN, StrWo, StrStation, Station.Line, Station.SFCDB);
            if (Message.Equals("OK"))
            {
                Station.AddMessage("MES00000053", new string[] { }, StationMessageState.Pass); //回饋消息到前台
            }
            else
            {
                throw new MESReturnMessage(Message);
            }
        }


        /// <summary>
        /// 補 SMTLOADING 資料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddSMTLoadingRecordsAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string Message = string.Empty;
            R_PANEL_SN PanelSN = null;
            string StrPanel = "";
            string StrWO = "";
            string StrTrCode = "";
            Dictionary<string, DataRow> TrSnTable = null;
            string Process = string.Empty;
            double LinkQty = 0d;
            string MacAddress = string.Empty;
            OleExec apdb = null;

            if (Paras.Count != 7)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() });
                throw new MESReturnMessage(Message);
            }


            //獲取 R_PANEL_SN 對象
            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            //PanelSN = (R_PANEL_SN)PanelSession.Value;
            StrPanel = PanelSession.Value.ToString();
            //獲取 TRSN 對象
            MESStationSession TrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrSnSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            TrSnTable = (Dictionary<string, DataRow>)TrSnSession.Value;

            //獲取面別
            Process = Paras[2].VALUE.ToString();

            //獲取連板數
            MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (LinkQtySession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            LinkQty = double.Parse(LinkQtySession.Value.ToString());

            //獲取 MAC 地址
            MacAddress = Paras[4].VALUE.ToString();

            //add by LLF 2017-01-24 begin
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (WoSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrWO = WoSession.Value.ToString();

            MESStationSession TrCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);

            if (TrCodeSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }

            StrTrCode = TrCodeSession.Value.ToString();

            //Message = table.AddSMTLoadingRecords(PanelSN.WORKORDERNO, StrPanel, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, LinkQty, Station.LoginUser.EMP_NO, MacAddress, Station.SFCDB);
            try
            {

                apdb = Station.APDB;
                Message = table.AddSMTLoadingRecords(StrWO, StrPanel, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, LinkQty, Station.LoginUser.EMP_NO, MacAddress, StrTrCode, apdb);

                if (Message.Equals("OK"))
                {
                    Station.AddMessage("MES00000053", new string[] { }, StationMessageState.Pass); //回饋消息到前台
                    //Station.DBS["APDB"].Return(apdb);
                }
                else
                {
                    //Station.DBS["APDB"].Return(apdb);
                    throw new MESReturnMessage(Message);
                }
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    //Station.DBS["APDB"].Return(apdb);
                }
                throw ex;
            }
        }

        /// <summary>
        /// Panel 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelPassStationAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string PanelSn = string.Empty;
            string Status = string.Empty;
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;
            string StationNext = "";
            SN SNObj = new SN();

            if (Paras.Count <= 0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession PanelSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //Modify By LLF 2018-01-26 SN_Session.Value是對象，取InputValue
            //PanelSn = PanelSnSession.Value.ToString();
            PanelSn = PanelSnSession.InputValue.ToString();

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StatusSession == null)
            {
                //Modify by LLF 2018-01-27 Value默認Pass
                //ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                //throw new MESReturnMessage(ErrMessage);
                StatusSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input, Value = Paras[1].VALUE };
                Station.StationSession.Add(StatusSession);
            }
            Status = StatusSession.Value.ToString();

            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else//Add by LLF 2018-01-27 DeviceName 默認為工站名稱
            {
                DeviceName = Station.StationName.ToString();
            }

            MESStationSession StationNextList = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            StationNext = SNObj.StringListToString((List<string>)StationNextList.Value);

            table.PanelPassStation(PanelSn, Station.Line, Station.StationName, DeviceName, Station.BU, Status, Station.LoginUser.EMP_NO, Station.SFCDB);
            Station.AddMessage("MES00000064", new string[] { Station.StationName, StationNext }, MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// FQC Lot 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LotPassStationAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            T_R_LOT_STATUS table = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS Row_R_Lot_Status = null;
            string SerialNo = string.Empty;
            string LotNo = string.Empty;
            string Status = string.Empty;
            string[] FailInfos = new string[3];
            string DeviceName = string.Empty;

            if (Paras.Count <=0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNo = SnSession.Value.ToString();
            //SerialNo = ((R_SN)SnSession.Value).SN;

            MESStationSession LotNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LotNoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            LotNo = ((R_LOT_STATUS)LotNoSession.Value).LOT_NO.ToString();

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {
                //MODIFY by LLF 2018-02-24
                //ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                //throw new MESReturnMessage(ErrMessage);
                Status = "PASS";
            }
            //Status = StatusSession.Value.ToString();

            MESStationSession FailInfoSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (FailInfoSession != null)
            {
                Status = "FAIL";
                FailInfos = (string[])FailInfoSession.Value;
            }
            //FailInfos[0] = "TEST";
            //FailInfos[1] = "TEST_CODE";
            //FailInfos[2] = "Just for test";

            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else
            {
                DeviceName = Station.StationName;
            }

            MESStationSession SessionAQLTYPE = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            MESStationSession SessionLotQTY = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            MESStationSession SessionSAMPLEQTY = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            MESStationSession SessionREJECTQTY = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            MESStationSession SessionPassQty = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);
            MESStationSession SessionFailQty = Station.StationSession.Find(t => t.MESDataType == Paras[10].SESSION_TYPE && t.SessionKey == Paras[10].SESSION_KEY);
            if (SessionAQLTYPE == null)
            {
                SessionAQLTYPE = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionAQLTYPE);
            }

            if (SessionLotQTY == null)
            {
                SessionLotQTY = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionLotQTY);
            }

            if (SessionSAMPLEQTY == null)
            {
                SessionSAMPLEQTY = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionSAMPLEQTY);
            }

            if (SessionREJECTQTY == null)
            {
                SessionREJECTQTY = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionREJECTQTY);
            }

            if (SessionPassQty == null)
            {
                SessionPassQty = new MESStationSession() { MESDataType = Paras[9].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[9].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionPassQty);
            }

            if (SessionFailQty == null)
            {
                SessionFailQty = new MESStationSession() { MESDataType = Paras[10].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[10].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionFailQty);
            }

            table.LotPassStation(SerialNo, LotNo, Status, Station.LoginUser.EMP_NO, Station.StationName, DeviceName, Station.Line, Station.BU, Station.SFCDB, FailInfos);


            Row_R_Lot_Status = table.GetByLotNo(LotNo, Station.SFCDB);
            if (Row_R_Lot_Status != null)
            {
                SessionAQLTYPE.Value = Row_R_Lot_Status.AQL_TYPE;
                SessionLotQTY.Value = Row_R_Lot_Status.LOT_QTY;
                SessionSAMPLEQTY.Value = Row_R_Lot_Status.SAMPLE_QTY;
                SessionREJECTQTY.Value = Row_R_Lot_Status.REJECT_QTY;
                SessionPassQty.Value = Row_R_Lot_Status.PASS_QTY;
                SessionFailQty.Value = Row_R_Lot_Status.FAIL_QTY;
            }
            //table.LotPassStation(Input.Value.ToString(), "LOT_123456", "PASS", Station.LoginUser.EMP_NO, Station.StationName, Station.Line, Station.BU, Station.SFCDB, FailInfos);
            //table.LotPassStation("SN2222", "LOT_123456", "FAIL", Station.LoginUser.EMP_NO, Station.StationName, Station.Line, Station.BU, Station.SFCDB, FailInfos);
            Station.AddMessage("MES00000065", new string[] { LotNo, SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 分板過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SplitsPassStationAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string PanelSn = string.Empty;
            string SerialNo = string.Empty;
            string Status = string.Empty;
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;
            R_PANEL_SN PANELObj = null;

            if (Station.Line.Trim() == "")
            {
                throw new Exception("Line Cann't be null !");
            }
            if (Paras.Count != 5)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            PanelSn = PanelSession.InputValue.ToString();
            //PanelSn = ((R_PANEL_SN)PanelSession.Value).PANEL;

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNo = SNSession.Value.ToString();
            //SerialNo = ((R_SN)SNSession.Value).SN;

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[2].VALUE, SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StatusSession);
                if (StatusSession.Value.ToString() == "")
                {
                    StatusSession.Value = "PASS";
                }
            }
            Status = StatusSession.Value.ToString();

            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else
            {
                DeviceName = Station.StationName;
            }

            //add by LLF 2018-03-28
            MESStationSession PanelVitualSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (PanelVitualSNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            PANELObj = (R_PANEL_SN)PanelVitualSNSession.Value;

            table.SplitsPassStation(PanelSn, Station.Line, Station.StationName, DeviceName, Station.BU, SerialNo, Status, Station.LoginUser.EMP_NO, Station.SFCDB, Station.APDB, PANELObj);
            //table.SplitsPassStation("P147852", Input.Value.ToString(), "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);

            //检查是否BIP完成
            string strsql = $@"select count(1) from r_sn where sn in (
select sn from r_panel_sn where panel = '{PanelSn}' ) 
and id = sn";
            int C = int.Parse(Station.SFCDB.ExecSelectOneValue(strsql).ToString());
            if (C > 0)
            {
                Station.NextInput = Input;
                Station.StationMessages.Add(new StationMessage { Message = $@"还有{C}PCS未分版", State = StationMessageState.Message });
            }
            else
            {
                Station.NextInput = Station.Inputs[0];
                Input.Value = "";
                //add by LLF 2018-04-04
                //Station.StationMessages.Add(new StationMessage { Message = $@"请输入新Panel", State = StationMessageState.Message });
                Station.AddMessage("MES00000222", new string[] { PanelSn }, StationMessageState.Alert); //回饋消息到前台
            }


            //Modify by LLF 2018-04-04
            //Station.AddMessage("MES00000148", new string[] { PanelSn, SerialNo }, StationMessageState.Pass); //回饋消息到前台
           // Station.AddMessage("MES00000148", new string[] { PanelSn, SerialNo }, StationMessageState.Alert); //回饋消息到前台

        }

        public static void InLotPassAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            T_R_LOT_STATUS Table_Lot_Status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS Row_R_Lot_Status = null;
            string SerialNo = string.Empty;
            string LotNo = string.Empty;
            string LotID = string.Empty;
            string AQL_TYPE = string.Empty;
            string NewLotFlag = "0";
            SN SNObj = new SN();
            R_SN RSNObj = new R_SN();
            LotNo LotObj = new LotNo();

            if (Paras.Count <= 0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SNObj = (SN)SnSession.Value;

            MESStationSession LotNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LotNoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession NewLotFlagSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (NewLotFlagSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            NewLotFlag = NewLotFlagSession.Value.ToString();

            MESStationSession AQLTYPESession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (AQLTYPESession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            AQL_TYPE = AQLTYPESession.Value.ToString();

            //
            MESStationSession SessionLotQTY = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            MESStationSession SessionSAMPLEQTY = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            MESStationSession SessionREJECTQTY = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            MESStationSession SessionPassQty = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            MESStationSession SessionFailQty = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
           
            if (SessionLotQTY == null)
            {
                SessionLotQTY = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionLotQTY);
            }

            if (SessionSAMPLEQTY == null)
            {
                SessionSAMPLEQTY = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionSAMPLEQTY);
            }

            if (SessionREJECTQTY == null)
            {
                SessionREJECTQTY = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionREJECTQTY);
            }

            if (SessionPassQty == null)
            {
                SessionPassQty = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionPassQty);
            }

            if (SessionFailQty == null)
            {
                SessionFailQty = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionFailQty);
            }

            //

            if (NewLotFlag == "1")
            {
                LotNo = LotNoSession.Value.ToString();
            }
            else
            {
                LotObj = (LotNo)LotNoSession.Value;
                LotID = LotObj.ID;
                LotNo = LotObj.LOT_NO;
            }

            RSNObj = SNObj.LoadSN(SNObj.SerialNo, Station.SFCDB);
            Table_Lot_Status.InLotPassStation(NewLotFlag, RSNObj, LotNo, LotID, Station.StationName, Station.LoginUser.EMP_NO, AQL_TYPE, Station.Line, Station.BU, Station.SFCDB);

            Row_R_Lot_Status = Table_Lot_Status.GetByLotNo(LotNo, Station.SFCDB);
            if (Row_R_Lot_Status != null)
            {
                AQLTYPESession.Value = Row_R_Lot_Status.AQL_TYPE;
                SessionLotQTY.Value = Row_R_Lot_Status.LOT_QTY;
                SessionSAMPLEQTY.Value = Row_R_Lot_Status.SAMPLE_QTY;
                SessionREJECTQTY.Value = Row_R_Lot_Status.REJECT_QTY;
                SessionPassQty.Value = Row_R_Lot_Status.PASS_QTY;
                SessionFailQty.Value = Row_R_Lot_Status.FAIL_QTY;
            }

            Station.AddMessage("MES00000157", new string[] { SNObj.SerialNo, LotNo }, StationMessageState.Pass); //回饋消息到前台
        }

        public static void LotCloseAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string StrLotNo = "";
            LotNo LotObj = new LotNo();
            T_R_LOT_STATUS Table_Lot_Status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS Row_Lot_Status = null;

            if (Paras.Count <= 0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "2", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession LotNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LotNoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession NewLotFlagSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (NewLotFlagSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if (NewLotFlagSession.Value.ToString() == "0")
            {
                LotObj = (LotNo)LotNoSession.Value;
                StrLotNo = LotObj.LOT_NO;
            }
            else
            {
                StrLotNo = LotNoSession.Value.ToString();
            }

            Row_Lot_Status=Table_Lot_Status.GetByInput(StrLotNo, "LOT_NO", Station.SFCDB);

            if (Row_Lot_Status == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000161", new string[] { });
                throw new MESReturnMessage(ErrMessage);
            }

            if (Row_Lot_Status.CLOSED_FLAG == "1")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000092", new string[] { Row_Lot_Status.LOT_NO });
                throw new MESReturnMessage(ErrMessage);
            }

            Row_R_LOT_STATUS Rows = (Row_R_LOT_STATUS)Table_Lot_Status.GetObjByID(Row_Lot_Status.ID, Station.SFCDB);
            Rows.CLOSED_FLAG = "1";
            Rows.EDIT_TIME = Station.GetDBDateTime();
            Rows.EDIT_EMP = Station.LoginUser.EMP_NO;
            Station.SFCDB.ExecSQL(Rows.GetUpdateString(Station.DBType));

            Station.AddMessage("MES00000155", new string[] { Row_Lot_Status.LOT_NO }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 刪條碼
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void DeletePanel(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSN = string.Empty;
            T_R_PANEL_SN RPanelSn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            int result = 0;
            string ErrMessage = string.Empty;
            T_R_WO_BASE Wo = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_R_PANEL_SN Panel = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            List<R_PANEL_SN> Panels = new List<R_PANEL_SN>();
            
            PanelSN = Input.Value.ToString();
            Panels = Panel.GetPanel(PanelSN, Station.SFCDB);
            
            if (Panels.Count != 0)
            {
                RPanelSn.RecordPanelStationDetail(PanelSN, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB, Station.DBType);
                result = RPanelSn.SetPanelInValid(PanelSN, Station.SFCDB, Station.DBType);
                if (result == 0)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "Panel#Valid_Flag" });
                    throw new MESReturnMessage(ErrMessage);
                }

                Wo.AddCountToWo(Panels[0].WORKORDERNO, Convert.ToDouble("-" + result), Station.SFCDB);
                Station.AddMessage("MES00000207", new string[] { PanelSN}, StationMessageState.Pass);
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000038", new string[] { PanelSN });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        /// <summary>
        /// Panel 退 SMTLOADING
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReturnSMTLoadingAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSN = string.Empty;
            string WO = string.Empty;
            OleExec apdb = null;

            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanel == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            //Panel pa=(Panel)sessionPanel.Value;
            WO = sessionWO.Value.ToString();
            PanelSN = ((Panel)sessionPanel.Value).PanelNo;

            if (string.IsNullOrEmpty(PanelSN) || string.IsNullOrEmpty(WO))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            T_R_PANEL_SN RPanelSn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            if (!RPanelSn.CheckPanelExist(PanelSN,Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            if (!RPanelSn.GetPanelCurrentStation(PanelSN,Station.SFCDB).Equals("SMTLOADING"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000261", new string[] { Paras[0].SESSION_TYPE }));
            }

            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> snList = t_r_sn.GetRSNbyPsn(PanelSN, Station.SFCDB);
            foreach(R_SN r_sn in snList)
            {
                Row_R_SN rowSN = (Row_R_SN)t_r_sn.GetObjByID(r_sn.ID, Station.SFCDB);
                rowSN.SN = "#" + rowSN.SN;
                rowSN.SKUNO = "#" + rowSN.SKUNO;
                rowSN.WORKORDERNO = "#" + rowSN.WORKORDERNO;
                rowSN.VALID_FLAG = "0";
                rowSN.EDIT_TIME = t_r_sn.GetDBDateTime(Station.SFCDB);
                rowSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                Station.SFCDB.ExecSQL(rowSN.GetUpdateString(Station.DBType));
            }

            T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            List<R_SN_STATION_DETAIL> snDetailList = t_r_sn_station_detail.GetSNStationDetailByPanel(PanelSN, Station.SFCDB);
            foreach (R_SN_STATION_DETAIL r_detail_sn in snDetailList)
            {
                Row_R_SN_STATION_DETAIL rowDetailSN = (Row_R_SN_STATION_DETAIL)t_r_sn_station_detail.GetObjByID(r_detail_sn.ID, Station.SFCDB);
                rowDetailSN.R_SN_ID = "#" + rowDetailSN.R_SN_ID;
                rowDetailSN.SN = "#" + rowDetailSN.SN;
                rowDetailSN.SKUNO = "#" + rowDetailSN.SKUNO;
                rowDetailSN.WORKORDERNO = "#" + rowDetailSN.WORKORDERNO;
                Station.SFCDB.ExecSQL(rowDetailSN.GetUpdateString(Station.DBType));
            }

            T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            t_r_wo_base.AddCountToWo(WO, - snList.Count, Station.SFCDB);

            T_R_PANEL_SN t_r_panel_sn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            List<R_PANEL_SN> panelSnList = t_r_panel_sn.GetPanel(PanelSN, Station.SFCDB);
            foreach(R_PANEL_SN r_panel_sn in panelSnList)
            {
                Row_R_PANEL_SN rowPanelSN = (Row_R_PANEL_SN)t_r_panel_sn.GetObjByID(r_panel_sn.ID, Station.SFCDB);
                rowPanelSN.SN = "#" + rowPanelSN.SN;
                rowPanelSN.PANEL = "#" + rowPanelSN.PANEL;
                rowPanelSN.WORKORDERNO = "#" + rowPanelSN.WORKORDERNO;
                Station.SFCDB.ExecSQL(rowPanelSN.GetUpdateString(Station.DBType));
            }

            AP_DLL APDLL = new AP_DLL();
            apdb = Station.APDB;
            APDLL.APUpdateUndoSmtloading(PanelSN, apdb);

        }
    }
}
