using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using MESStation.LogicObject;
using MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckPanelData
    {

        public static void PanelStateDatachecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string P_sn = Input.Value.ToString();
            string ErrMessage = string.Empty;
            int RepairedCount = 0;
            int CompletedCount = 0;
            int ShippedCount = 0;
            int TotalCount = 0;

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (PanelSession == null)
            //{
            //    PanelSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
            //    Station.StationSession.Add(PanelSession);
            //}
            //else
            //{
            //    if (PanelSession.Value != null && PanelSession.Value.ToString().Length > 0)
            //    {
            //        P_sn = PanelSession.Value.ToString();
            //    }
            //    else
            //    {
            //        P_sn = Input.Value.ToString();
            //    }
            //}

            Panel p = new Panel();
            List<R_SN> r_sn = p.GetPanel(P_sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            //changed into follow one by zgj 2018-03-14
            if (r_sn != null && r_sn.Count > 0)
            {
                TotalCount = r_sn.Count;
                foreach (R_SN sn in r_sn)
                {
                    if (sn.REPAIR_FAILED_FLAG.Equals("1"))
                    {
                        RepairedCount++;
                    }
                    if (sn.COMPLETED_FLAG.Equals("1") || sn.PACKED_FLAG.Equals("1"))
                    {
                        CompletedCount++;
                    }
                    if (sn.SHIPPED_FLAG.Equals("1"))
                    {
                        ShippedCount++;
                    }
                }
                if (RepairedCount.Equals(TotalCount))
                {
                    Station.AddMessage("MES00000068", new string[] { P_sn }, MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000068",
                                    new string[] { P_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (CompletedCount.Equals(TotalCount))
                {
                    Station.AddMessage("MES00000069", new string[] { P_sn }, MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000069",
                                    new string[] { P_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (ShippedCount.Equals(TotalCount))
                {
                    Station.AddMessage("MES00000070", new string[] { P_sn }, MESReturnView.Station.StationMessageState.Fail);
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000070",
                                    new string[] { P_sn });
                    throw new MESReturnMessage(ErrMessage);
                }
                Station.AddMessage("MES00000067", new string[] { "PanleSN" }, MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                Station.AddMessage("MES00000007", new string[] { "PanleSN" }, StationMessageState.Fail);
            }
            //if (r_sn != null)
            //{
            //    if (r_sn[0].REPAIR_FAILED_FLAG == "1")
            //    {
            //        Station.AddMessage("MES00000068", new string[] { P_sn }, MESReturnView.Station.StationMessageState.Fail);
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000068",
            //                        new string[] { P_sn });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //    if (r_sn[0].COMPLETED_FLAG == "1" || r_sn[0].PACKED_FLAG == "1")
            //    {
            //        Station.AddMessage("MES00000069", new string[] { P_sn }, MESReturnView.Station.StationMessageState.Fail);
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000069",
            //                        new string[] { P_sn });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //    if (r_sn[0].SHIPPED_FLAG == "1")
            //    {
            //        Station.AddMessage("MES00000070", new string[] { P_sn }, MESReturnView.Station.StationMessageState.Fail);
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000070",
            //                        new string[] { P_sn });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //    Station.AddMessage("MES00000067", new string[] { "PanleSN" }, MESReturnView.Station.StationMessageState.Pass);
            //}


        }

        /// <summary>
        /// 檢查當前站是否在下一站的清單中
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// add by LLF  2018-01-27 
        public static void NextStationDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<string> ListNextStation = new List<string>();
            SN SNObj = new SN();
            string StrStation = "";

            MESStationSession NextStation_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (NextStation_Session == null)
            {
                //Station.AddMessage("MES00000135", new string[] { "ListNextStation--Function:NextStationDataChecker" }, StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000135", new string[] { "ListNextStation--Function:NextStationDataChecker" }));
            }

            ListNextStation = (List<string>)NextStation_Session.Value;
            StrStation = SNObj.StringListToString(ListNextStation);
            if (!ListNextStation.Contains(Station.StationName))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000136", new string[] { StrStation }));
            }
        }

        public static void PanelRuleDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //input test
            //string inputValue = Input.Value.ToString();
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuSession);
            }
            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PanelSession == null)
            {
                throw new MESReturnMessage("Panel加載異常");
            }
            Station.AddMessage("OK", new string[] { "ok" }, StationMessageState.Pass);
        }

        public static void PanelVirtualSNChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSN = "";
            bool CheckFlag = false;
            SN SNObj = new SN();
            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            PanelSN = PanelSession.InputValue.ToString();
            CheckFlag=SNObj.CheckPanelVirtualSNExist(PanelSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (!CheckFlag)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000139", new string[] { PanelSN }));
            }
        }

        /// <summary>
        /// 檢查該 Panel 是不是在指定站位
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPanelInSmtLoading(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSn = string.Empty;
            T_R_PANEL_SN RPanelSn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            string ErrMessage = string.Empty;
            string UniqueStation = string.Empty;

            if (Paras.Count < 1)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMessage);
            }

            PanelSn = Input.Value.ToString();
            UniqueStation = Paras[0].VALUE;
            if(UniqueStation.Length==0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000050");
                throw new MESReturnMessage(ErrMessage);
            }

            if (!RPanelSn.GetPanelUniqueStation(PanelSn, Station.SFCDB).ToUpper().Equals(UniqueStation))
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000206", new string[] { PanelSn, UniqueStation });
                throw new MESReturnMessage(ErrMessage);
            }

        }
    }
}
