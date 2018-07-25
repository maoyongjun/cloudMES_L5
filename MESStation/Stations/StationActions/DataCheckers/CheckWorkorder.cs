using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDBHelper;
using MESStation.MESReturnView.Station;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESDataObject;


namespace MESStation.Stations.StationActions.DataCheckers
{
    class CheckWorkorder
    {
        /// <summary>
        /// 檢查工單數據是否存在
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void WoDataCheck(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            if (Paras.Count != 1)
            {
                 
                throw new Exception("參數數量不正確!");
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
            //CHECK  Workorder是否存在        
            T_R_WO_BASE TRWO = new T_R_WO_BASE(Station.SFCDB,MESDataObject.DB_TYPE_ENUM.Oracle);
            Row_R_WO_BASE ROWWO;
            string WO = Input.Value.ToString();
            try
            {
                ROWWO = TRWO.GetWo(WO, Station.SFCDB);
                s.Value = WO;
                s.InputValue = Input.Value.ToString();
                s.ResetInput = Input;

                //modify by LLF 2018-02-02
                //Station.AddMessage("MES00000029", new string[] { "Workorder", WO}, MESReturnView.Station.StationMessageState.Message);
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000029", new string[] { "Workorder", WO });
                throw new MESReturnMessage(ErrMessage);
            }
            catch (Exception ex)
            {
                //modify by LLF 2018-02-02
                //ex.InnerException.Message;
                //string msgCode = ex.Message;
                //Station.AddMessage(msgCode, new string[] { "Workorder:" + WO }, StationMessageState.Fail);
                throw new MESReturnMessage(ex.Message);

            }
            
        }

        /// <summary>
        /// 檢查工單狀態必須Release&Start
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void SkuFromWODataChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }

            //marked by ZGJ 2018-03-15
            //單從這個方法的功能上（這個方法的功能定義為檢查工單的狀態，但是方法名卻像是從工單加載機種）看，
            //沒有必要使用 SKU session
            //MESStationSession SKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (SKU == null)
            //{
            //    SKU = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
            //    Station.StationSession.Add(SKU);
            //}

            MESStationSession TWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TWO == null)
            {
                TWO = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TWO);
            }

            //CHECK  Workorder是否Release&Start 
            WorkOrder WorkorderInfo = new WorkOrder();
            //string WoNum = TWO.InputValue;
            var obj_wo = TWO.Value;
            
            //string WoNum = TWO.Value.ToString();
            try
            {
                //add by ZGJ 2018-03-15
                //檢查工單時，之前的步驟中可能就已經把工單實例放在 WO1 中，所以這裡判斷，如果已經是工單實例，
                //那麼就直接賦值，否則進行加載
                if (obj_wo is string)
                {
                    WorkorderInfo.Init(obj_wo.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                }
                else if (obj_wo is WorkOrder)
                {
                    WorkorderInfo =(WorkOrder) obj_wo;
                }
                //WorkorderInfo.Init(WoNum, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(WorkorderInfo.CLOSED_FLAG)||WorkorderInfo.CLOSED_FLAG=="1")   //null or 1代表工單已經關閉，0代表工單開啟
                {
                    //Modify by LLF 2018-02-02 
                    //Station.AddMessage("MES00000041", new string[] { "WO:" + WorkorderInfo.WorkorderNo }, StationMessageState.Fail);
                    //return;
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000041", new string[] { WorkorderInfo.WorkorderNo });
                    throw new MESReturnMessage(ErrMessage);
                }

                if(WorkorderInfo.RELEASE_DATE==null)
                {
                    //Modify by LLF 2018-02-02 
                    //Station.AddMessage("MES00000042", new string[] { "WO:" + WorkorderInfo.WorkorderNo }, StationMessageState.Fail);
                    //return;
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000042", new string[] { WorkorderInfo.WorkorderNo });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            catch (Exception ex)
            {
                //Modify by LLF 2018-02-02 
                //ex.InnerException.Message;
                //string msgCode = ex.Message;
                //Station.AddMessage(msgCode, new string[] { "Workorder:" + WorkorderInfo.WorkorderNo }, StationMessageState.Fail);
                throw new MESReturnMessage(ex.Message);

            }
        }

        //工單狀態檢查
        public static void WoStateDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            ////ADD BY  SDL  20180316
            if (Paras.Count != 1)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession WoLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoLoadPoint == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            WorkOrder ObjWO = (WorkOrder)WoLoadPoint.Value;

            ////ADD BY  SDL  20180316
            //CHECK  Workorder是否存在 
            string ErrMessage = "";

            T_R_WO_BASE TRWO = new T_R_WO_BASE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Row_R_WO_BASE ROWWO;
            T_R_SN rSn = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            // string WO = Input.Value.ToString();   by sdl  20180316
            string WO = ObjWO.WorkorderNo;
            try
            {
                //List<R_SN> snList = rSn.GetRSNbyWo(WO, Station.SFCDB);
                ROWWO = TRWO.GetWo(WO, Station.SFCDB);
                R_WO_BASE woBase= ROWWO.GetDataObject();
                WorkOrder ObjWorkorder = new WorkOrder();
                //if (snList!=null)
                //{
                //    foreach (var item in snList)
                //    {
                //        if (woBase.ROUTE_ID != item.ROUTE_ID)
                //        {
                //            //throw new Exception("SN RouteID不唯一!");
                //            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000167", new string[] { item.SN });
                //            throw new MESReturnMessage(ErrMessage);
                //        }
                //    }
                //}
                
                if (woBase.CLOSED_FLAG==1.ToString())
                {
                    // throw new Exception("ClosedFlag=1!");
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000168", new string[] { woBase.WORKORDERNO });
                    throw new MESReturnMessage(ErrMessage);
                }

                if ((woBase.START_STATION ==null|| woBase.START_STATION=="N/A")&& woBase.WO_TYPE=="REWORK")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000203", new string[] { woBase.WORKORDERNO });
                    throw new MESReturnMessage(ErrMessage);
                }

                if (woBase.FINISHED_QTY>woBase.WORKORDER_QTY)
                {
                  //  throw new Exception("FinishQty>WorkOrderQty!");
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000169", new string[] { woBase.WORKORDERNO });
                    throw new MESReturnMessage(ErrMessage);
                }
                Station.StationSession.Add(WoLoadPoint);
                ObjWorkorder.Init(WO, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                WoLoadPoint.Value = ObjWorkorder;
                WoLoadPoint.InputValue = Input.Value.ToString();
                WoLoadPoint.ResetInput = Input;
                WoLoadPoint.SessionKey = "1";
                WoLoadPoint.MESDataType = "WO";
                Station.AddMessage("MES00000029", new string[] { "Workorder", WO }, MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// add by fgg 2018.05.12
        /// 工單投入檢查 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WOInputDataChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string stationName = Station.StationName;
            OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 4)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionWOQty = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_TYPE);
            if (sessionWOQty == null)
            {
                sessionWOQty = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionWOQty);
            }

            MESStationSession sessionInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_TYPE);
            if (sessionInputQty == null)
            {
                sessionInputQty = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionInputQty);
            }

            MESStationSession sessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_TYPE);
            if (sessionExtQty == null)
            {
                sessionExtQty = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionExtQty);
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                //投錯工站
                if (!objWorkorder.START_STATION.Equals(stationName))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000246",new string[] { stationName, objWorkorder.START_STATION }));
                }
                //工單關節
                if (objWorkorder.CLOSED_FLAG.Equals("1"))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000100"));
                }
                //已經投滿
                if (objWorkorder.INPUT_QTY >= objWorkorder.WORKORDER_QTY)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000247",new string[] { objWorkorder.WorkorderNo }));
                }
                sessionWOQty.Value = objWorkorder.WORKORDER_QTY;
                sessionInputQty.Value = objWorkorder.INPUT_QTY;
                sessionExtQty.Value = objWorkorder.WORKORDER_QTY - objWorkorder.INPUT_QTY;

                Station.AddMessage("MES00000029", new string[] { "Workorder", objWorkorder.WorkorderNo }, MESReturnView.Station.StationMessageState.Message);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// add by fgg 2018.05.12
        /// 工單類型檢查 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WOTypeDataChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec sfcdb = Station.SFCDB;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;                
                if (objWorkorder.WO_TYPE.Equals("REWORK"))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000249",new string[] {objWorkorder.WorkorderNo, objWorkorder.WO_TYPE }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
