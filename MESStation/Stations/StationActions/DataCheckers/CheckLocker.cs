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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckLocker
    {
        /// <summary>
        /// 檢查SN/PanelSN是否被鎖
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNLockedChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //input test
            //string inputValue = Input.Value.ToString();
            //MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (snSession == null)
            //{
            //    throw new MESReturnMessage("SN加載異常");
            //}
            //SN sn = (SN) snSession.Value;
            OleExec sfcdb = Station.SFCDB;
            //R_SN_LOCK r_sn_lock = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailBySN(sfcdb, sn.SerialNo, Station.StationName);//sn.SerialNo,sn.CurrentStation
            R_SN_LOCK r_sn_lock = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailBySN(sfcdb, Input.Value.ToString(), Station.StationName);//sn.SerialNo,sn.CurrentStation
            if (r_sn_lock != null)
            {
                Station.AddMessage("MES00000044", new string[] { "SN", r_sn_lock.SN, r_sn_lock.LOCK_EMP }, StationMessageState.Fail);
                //return;
                throw new MESReturnMessage("SN被鎖定");
            }
            
        }

        /// <summary>
        /// 檢查線體是否被鎖
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LineLockedChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //GET LINE
            string LineName = Station.Line;
            string CurrentStation = Station.StationName;

            if (string.IsNullOrEmpty(LineName))
            {
                throw new MESReturnMessage("LINE線體加載異常");
            }
            OleExec sfcdb = Station.SFCDB;
            R_SN_LOCK r_sn_lock = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailBySN(sfcdb, LineName, CurrentStation);//sn.SerialNo,sn.CurrentStation
            if (r_sn_lock != null)
            {
                Station.AddMessage("MES00000044", new string[] { "LINE", r_sn_lock.SN, r_sn_lock.LOCK_EMP }, StationMessageState.Fail);
                //return;
                throw new MESReturnMessage("線體被鎖定");
            }
            
        }

        /// <summary>
        /// 檢查當前工單是否上料齊套
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTAPMaterialChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //return;
            //input test
            //string inputValue = Input.Value.ToString();
            MESStationSession WO_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WO_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "WO" }));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }

            string wo = WO_Session.Value.ToString();
            //Modify By LLF 2018-01-26 SN_Session.Value是對象，取InputValue
            //string sn = SN_Session.Value.ToString();
            string sn = SN_Session.InputValue.ToString();
            string line = Station.Line;
            string station = Station.StationName;

            OleExecPool apdbPool = Station.DBS["APDB"];
            OleExec apdb = apdbPool.Borrow();
            OleDbParameter[] paras = new OleDbParameter[] {
                new OleDbParameter("G_PSN", sn),
                new OleDbParameter("G_WO", wo),
                new OleDbParameter("G_STATION", line),
                new OleDbParameter("G_EVENT", station),
                new OleDbParameter(":RES", OleDbType.VarChar, 200)
            };
            paras[4].Direction = ParameterDirection.Output;
            string msg = apdb.ExecProcedureNoReturn("MES1.CMC_INSERTDATA_SP", paras);

            if (apdb != null)
            {
                apdbPool.Return(apdb);
            }
            if ("OK".Equals(msg.ToUpper()))
            {
                Station.AddMessage("MES00000047", new string[] { "wo" }, StationMessageState.Pass);//wo
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000046", new string[] { msg }));
            }
        }

    }
}
