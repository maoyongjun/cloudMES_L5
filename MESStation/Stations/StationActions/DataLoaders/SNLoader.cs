using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using MESStation.LogicObject;
using MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;

namespace MESStation.Stations.StationActions.DataLoaders
{
    class SNLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputSNDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }
           
            string inputValue = Input.Value.ToString();
            SN SNObj = null;
            try
            {
                SNObj = new SN(inputValue, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(SNObj.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + inputValue }));
                }
                SNSession.Value = SNObj;
                SNSession.InputValue = inputValue;
                SNSession.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { "SN:", inputValue }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public static void IStaionTest(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string inputValue = Input.Value.ToString();
            int para = Paras.Count;
        }

        //加載SN待過工站
        public static void SNNextStationDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //Modify by LLF 2018-01-26 通過配置獲取
            //MESStationSession WipStationSave = new MESStationSession() {MESDataType= "WIPSTATION", InputValue=Input.Value.ToString(),SessionKey="1",ResetInput=Input };

            string StrNextStation = "";
            MESStationSession NextStationSave = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (NextStationSave == null)
            {
                NextStationSave = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(NextStationSave);
            }
            string strSn = Input.Value.ToString();
            SN sn = new SN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Route routeDetail = new Route(sn.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<string> snStationList = new List<string>();
            List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == sn.CurrentStation).FirstOrDefault();

            //Modify by LLF 2018-01-29
            //string nextStation1 = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO + 10).FirstOrDefault().STATION_NAME;

            string nextStation1 = sn.NextStation;
            if (!sn.CurrentStation.Equals("REWORK"))
            {
                if (R == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000156", new string[] { strSn, sn.CurrentStation }));
                }

                if (routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault() == null && !sn.CurrentStation.Equals("REWORK"))
                {
                    if (!string.IsNullOrEmpty(sn.CurrentStation))
                    {
                        StrNextStation = sn.NextStation;
                    }
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000200", new string[] { strSn, StrNextStation }));
                }
                nextStation1 = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            }

            snStationList.Add(nextStation1);
            if (R!=null && R.DIRECTLINKLIST != null)
            {
                foreach (var item in R.DIRECTLINKLIST)
                {
                    snStationList.Add(item.STATION_NAME);
                }
            }
            NextStationSave.Value = snStationList;
            Station.AddMessage("MES00000029", new string[] { "NextStationList", "NextStation" }, MESReturnView.Station.StationMessageState.Pass);
        }

        //add by LLF 2018-01-26
        //加載Panel待過工站
        //modify by ZGJ 2018-03-07
        public static void PanelSNNextStationDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //MESStationSession WipStationSave = new MESStationSession() { MESDataType = "WIPSTATION", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
            MESStationSession NextStationSave = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (NextStationSave == null)
            {
                NextStationSave = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(NextStationSave);
            }
            string PanelSn = Input.Value.ToString();
            T_R_PANEL_SN table = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            NextStationSave.Value = new List<string>() { table.GetPanelNextStation(PanelSn, Station.SFCDB) };
            //SN sn = new SN();

            //sn.PanelSN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            //Route routeDetail = new Route(sn.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            //List<string> snStationList = new List<string>();
            //List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            //RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == sn.CurrentStation).FirstOrDefault();
            ////Modify By LLF 2018-01-29
            ////string nextStation1 = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO + 10).FirstOrDefault().STATION_NAME;
            //string nextStation1 = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            //snStationList.Add(nextStation1);
            //if (R.DIRECTLINKLIST != null)
            //{
            //    foreach (var item in R.DIRECTLINKLIST)
            //    {
            //        snStationList.Add(item.STATION_NAME);
            //    }
            //}
            Station.AddMessage("MES00000029", new string[] { "NextStationList", "NextStation" }, MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// 加載工站Pass下一站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// add by LLF 2018-01-29
        public static void StationNextDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string NextStation = "";
            MESStationSession StationNextSave = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (StationNextSave == null)
            {
                StationNextSave = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationNextSave);
            }
            string strSn = Input.Value.ToString();
            SN sn = new SN();

            //Marked by LLF 2018-02-22 begin
            //sn.PanelSN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            try
            {
                sn.PanelSN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            catch
            {
                
                sn.Load(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            //Marked by LLF 2018-02-22 end

            Route routeDetail = new Route(sn.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<string> snStationList = new List<string>();
            List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == Station.StationName).FirstOrDefault();

            if (routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault() == null)//當前工站為最後一個工站時
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO).FirstOrDefault().STATION_TYPE;
            }
            else
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            }
            
            snStationList.Add(NextStation);
            if (R.DIRECTLINKLIST != null)
            {
                foreach (var item in R.DIRECTLINKLIST)
                {
                    snStationList.Add(item.STATION_NAME);
                }
            }
            StationNextSave.Value = snStationList;
            Station.AddMessage("MES00000029", new string[] { "StationNext", "StationNextList" }, MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// By工單Route獲取下一工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void StationNextByWODataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string NextStation = "";
            WorkOrder WoObj = new WorkOrder();

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession StationNextSave = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StationNextSave == null)
            {
                StationNextSave = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationNextSave);
            }

            WoObj = (WorkOrder)WOSession.Value;
            Route routeDetail = new Route(WoObj.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            List<string> snStationList = new List<string>();
            List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == Station.StationName).FirstOrDefault();

            if (routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault() == null)//當前工站為最後一個工站時
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO).FirstOrDefault().STATION_TYPE;
            }
            else
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            }

            snStationList.Add(NextStation);

            if (R.DIRECTLINKLIST != null)
            {
                foreach (var item in R.DIRECTLINKLIST)
                {
                    snStationList.Add(item.STATION_NAME);
                }
            }
            StationNextSave.Value = snStationList;
            Station.AddMessage("MES00000029", new string[] { "StationNext", "StationNextList" }, MESReturnView.Station.StationMessageState.Pass);
        }


        /// <summary>
        /// 從SN對象LOADER BACK STATION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetBackStationDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            //MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            //if (SnSession == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            //}
            //SN ObjSn = (SN)Input.Value;
            //try
            //{
                
            //}
            //catch (Exception ex)
            //{                
            //    throw ex;
            //}

        }

        /// <summary>
        /// 從第一個輸入框加載PanelSN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// Add by LLF 2018-02-01
        //public static void SNInputDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        //{
        //    string StrSN = Input.Value.ToString();

        //    MESStationSession SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
        //    Station.StationSession.Add(SNSession);

        //    SNSession.InputValue = Input.Value.ToString();
        //    SNSession.Value = StrSN;
        //}

        public static void SNLinkKeypartDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<Dictionary<string, string>> KPList = new List<Dictionary<string, string>>();
            int SeqNo = 0;
            KPList = null;
            if (Paras.Count != 5)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            string Sn = Input.Value.ToString();
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                WOSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WOSession);
            }
            MESStationSession SubKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SubKPSession == null)
            {
                SubKPSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SubKPSession);
            }
            MESStationSession MainKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (MainKPSession == null)
            {
                MainKPSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(MainKPSession);
            }
            MESStationSession KPListSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (KPListSession == null)
            {
                KPListSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(KPListSession);
            }

            KPListSession.Value = KPList;
            //SN sn = null;
            //WorkOrder wo = null;
            SN sn = new SN();
            WorkOrder wo = new WorkOrder();
            wo = (WorkOrder)WOSession.Value;
            T_C_KEYPART tck = new T_C_KEYPART(Station.SFCDB,DB_TYPE_ENUM.Oracle);
            //sn.Load(Sn,Station.SFCDB,DB_TYPE_ENUM.Oracle);
            //wo.Init(sn.WorkorderNo,Station.SFCDB,DB_TYPE_ENUM.Oracle);
            List<C_KEYPART> keyparts = tck.GetKeypartListByWOAndStation(Station.SFCDB,wo.WorkorderNo,Station.StationName);
            if (keyparts.Count > 0)
            {
                SeqNo = (int)((C_KEYPART)keyparts[0]).SEQ_NO;
                SubKPSession.Value = keyparts.Where(s => s.SEQ_NO == SeqNo).ToList();
                MainKPSession.Value = keyparts.Where(s => s.SEQ_NO > SeqNo).ToList();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000190");
                throw new MESReturnMessage(errMsg);
            }
        }

        public static void LinkKeypartDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            string Sn = Input.Value.ToString();
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                WOSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WOSession);
            }
            
            MESStationSession SubKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SubKPSession == null)
            {
                SubKPSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SubKPSession);
            }
            MESStationSession MainKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (MainKPSession == null)
            {
                MainKPSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(MainKPSession);
            }
            SN sn = null;
            WorkOrder wo = null;
            T_C_KEYPART tck = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            sn.Load(Sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            wo.Init(sn.WorkorderNo, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_KEYPART> keyparts = tck.GetKeypartListByWOAndStation(Station.SFCDB, wo.WorkorderNo, Station.StationName);
            SubKPSession.Value = keyparts.Where(s => s.SEQ_NO == 10).ToList();
            MainKPSession.Value = keyparts.Where(s => s.SEQ_NO != 10).ToList().OrderBy(s => s.SEQ_NO);
        }

        public static void PanelVitualSNDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSN = "";
            SN SNObj = new SN();
            R_PANEL_SN PanelObj = null;

            MESStationSession PANEL = Station.StationSession.Find(t=> t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY );
            if (PANEL == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession VirtualSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            if (VirtualSN == null)
            {
                VirtualSN = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(VirtualSN);
            }

            VirtualSN.InputValue = Input.Value.ToString();
            VirtualSN.ResetInput = Input;
            PanelSN = PANEL.InputValue.ToString();
            PanelObj = SNObj.GetPanelVirtualSN(PanelSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            VirtualSN.Value = PanelObj;
        }


        /// <summary>
        /// 通過SN獲取r_sn_keypart_detail表的LINK信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void BySnObjectGetLinkKeypartDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession LinkSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LinkSnSession == null)
            {
                LinkSnSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(LinkSnSession);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                throw new Exception("Can Not Find " + Paras[1].SESSION_TYPE + " 'Information ' !");
            }
            else
            {
                SN Objsn = (SN)SNSession.Value;
                T_R_SN_KEYPART_DETAIL stk = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_SN_KEYPART_DETAIL> KeyPartList = stk.GetKeypartBySN(Station.SFCDB, Objsn.SerialNo);

                LinkSnSession.Value = KeyPartList;

                Station.AddMessage("MES00000029", new string[] { "KeyPartList", KeyPartList.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 從輸入加載SN對象集合
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetSnObjectListDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            MESStationSession sessionInputType = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionInputType == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (sessionInputType.Value.ToString() == "")
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
            MESStationSession snObjectList = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_KEY && t.SessionKey == Paras[3].SESSION_KEY);
            if (snObjectList == null)
            {
                snObjectList = new MESStationSession(){ MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(snObjectList);
            }
            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                string inputType = sessionInputType.Value.ToString();
                string inputString = sessionInputString.Value.ToString();
                List<R_SN> snList = new List<R_SN>();
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                if (inputType.Equals("SN"))
                {
                    T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                    snList.Add(t_r_sn.LoadSN(inputString, Station.SFCDB));
                }
                else if (inputType.Equals("PANEL"))
                {
                    T_R_PANEL_SN t_r_panel_sn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
                    snList = t_r_panel_sn.GetValidSnByPanel(inputString, Station.SFCDB);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259"));                    
                }               
                snObjectList.Value = snList;
                Station.AddMessage("MES00000001", new string[] { sessionInputString.Value.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
