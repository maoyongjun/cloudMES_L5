using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESStation.MESReturnView.Station;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class WorkorderLoader
    {
        /// <summary>
        /// 從輸入加載工單數據
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void LoadWoFromInput(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            
            //if (Paras.Count == 0)
            //{
            //    throw new Exception("參數數量不正確!");

            //}
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }

            //dStation.NextInput = Station.Inputs[1];
            //Station.DBS["APDB"].Borrow()
            LogicObject.WorkOrder WO = new LogicObject.WorkOrder();
            WO.Init(Input.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            s.Value = WO;
            s.InputValue = Input.Value.ToString();
            s.ResetInput = Input;
            Station.AddMessage("MES00000029", new string[] { "Workorder", WO.ToString() }, MESReturnView.Station.StationMessageState.Pass);
        }

        public static void WoInputCountDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            MESStationSession count = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (count == null)
            {
                count = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(count);
            }
            MESStationSession wo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (wo == null)
            {
                wo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(wo);
            }
            string workorder = Station.StationSession[1].InputValue.ToString();
            MESStation.LogicObject.WorkOrder worder = new LogicObject.WorkOrder();
            MESDataObject.Module.T_R_WO_BASE trwb = new MESDataObject.Module.T_R_WO_BASE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_R_WO_BASE rrwb = (MESDataObject.Module.Row_R_WO_BASE)trwb.NewRow();
            rrwb = worder.GetWoMode(workorder, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Station.AddMessage("MES00000029", new string[] { "Workorder", wo.ToString() }, MESReturnView.Station.StationMessageState.Message);

        }

        /// <summary>
        /// 工單信息加載器,加載工單信息到指定位置,主要用來給界面做輸出. 2018/1/2 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">
        /// InputQTYSave	工單已投數量保存位置	{SESSION_TYPE:"INPUTQTY",SSION_KEY:"1",VALUE:""}
        /// FinishQTYSave 工單已完成數量保存位置 { SESSION_TYPE:"FINISHQTY",SSION_KEY:"1",VALUE:""}
        /// StationQTYSave 本工站過站數量保存位置 { SESSION_TYPE:"STATIONQTY",SSION_KEY:"1",VALUE:""}
        /// WOLoadPoint 工單的保存位置 { SESSION_TYPE:"WO",SSION_KEY:"1",VALUE:""}
        /// </param>
        public static void WoInfoDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStation.LogicObject.WorkOrder wo = new MESStation.LogicObject.WorkOrder();
            string strWONO = "";
            if (Paras.Count!=5)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession InputQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);         
            if (InputQTY_Session == null)
            {
                InputQTY_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(InputQTY_Session);
            }
            MESStationSession FinishQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FinishQTY_Session == null)
            {
                FinishQTY_Session = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FinishQTY_Session);
            }
            MESStationSession StationQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StationQTY_Session == null)
            {
                StationQTY_Session = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationQTY_Session);
            }
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (Wo_Session == null)
            {   
                           
                strWONO = Input.Value.ToString();
                Wo_Session = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };             
                Station.StationSession.Add(Wo_Session);
            }
            strWONO = Wo_Session.Value.ToString();
            //add by LLF 2018-03-26 增加工單數量Sesstion
            MESStationSession WoQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (WoQty_Session == null)
            {
                WoQty_Session = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoQty_Session);
            }
            else
            {
                //wo = (WorkOrder)Wo_Session.Value;
                //if (wo != null && wo.WorkorderNo != null && wo.WorkorderNo.Length > 0)
                //{
                //    strWONO = wo.WorkorderNo;
                //   // wo.Init(wo.WorkorderNo, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                //}
                //else
                //{
                //    wo = new MESStation.LogicObject.WorkOrder();
                //    strWONO = Input.Value.ToString();
                //   // wo.Init(Input.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                //}
                //  Wo_Session.Value = wo;
                //strWONO = Wo_Session.Value.ToString();
                Wo_Session.InputValue = Input.Value.ToString();
                Wo_Session.ResetInput = Input;
            }
            try
            {               
                wo.Init(strWONO, Station.SFCDB, Station.DBType);
                Wo_Session.Value = wo;
                InputQTY_Session.Value = wo.INPUT_QTY;
                FinishQTY_Session.Value = wo.FINISHED_QTY;
                WoQty_Session.Value = wo.WORKORDER_QTY;
                T_R_SN_STATION_DETAIL TR_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
                StationQTY_Session.Value = TR_SN_STATION_DETAIL.GetCountByWOAndStation(wo.ToString(), Station.StationName, Station.SFCDB);
                Station.AddMessage("MES00000001", new string[] { Input.Value.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                Wo_Session.Value = null;
                InputQTY_Session.Value = 0;
                FinishQTY_Session.Value = 0;
                StationQTY_Session.Value = 0;
                WoQty_Session.Value = 0;
                throw ex;
            }
           
         
        }
    }
}
