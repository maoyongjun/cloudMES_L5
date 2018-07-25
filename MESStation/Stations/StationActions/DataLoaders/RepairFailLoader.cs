using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Common;
using System.Data;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class RepairFailLoader
    {
        /// <summary>
        /// 從SN對象中加載RepairFail訊息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairFailDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            //Repair Session
            MESStationSession RepairMain = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (RepairMain == null)
            {
                RepairMain = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(RepairMain);
            }
            MESStationSession RepairFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (RepairFailCode == null)
            {
                RepairFailCode = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(RepairFailCode);
            }
            string sn = SN_Session.Value.ToString();
            OleExec sfcdb = Station.SFCDB;
            //存入R_REPAIR_MAIN信息
            Dictionary<string, List<R_REPAIR_MAIN>> repairMainInfo = new Dictionary<string, List<R_REPAIR_MAIN>>();
            List<R_REPAIR_MAIN> repairMains = new T_R_REPAIR_MAIN(sfcdb, DB_TYPE_ENUM.Oracle).GetRepairMainBySN(sfcdb, sn);
            repairMainInfo.Add("R_REPAIR_MAIN", repairMains);
            RepairMain.Value = repairMainInfo;
            //存入R_REPAIR_FAILCODE信息
            Dictionary<string, List<R_REPAIR_FAILCODE>> repairFailCodeInfo = new Dictionary<string, List<R_REPAIR_FAILCODE>>();
            List<R_REPAIR_FAILCODE> failCodes = new T_R_REPAIR_FAILCODE(sfcdb, DB_TYPE_ENUM.Oracle).GetFailCodeBySN(sfcdb, sn);
            repairFailCodeInfo.Add("R_REPAIR_FAILCODE", failCodes);
            RepairFailCode.Value = repairFailCodeInfo;
        }


        /// <summary>
        /// 從SN對象中加載RepairFailCode訊息   // 同上一个方法一样，只是传出数据格式变化
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairFailCodeDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            //Repair Action Session
            MESStationSession RepairAction = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (RepairAction == null)
            {
                RepairAction = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(RepairAction);
            }
            MESStationSession RepairFailCode = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (RepairFailCode == null)
            {
                RepairFailCode = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(RepairFailCode);
            }
            string sn = SN_Session.Value.ToString();

            DataTable FailCodeInfo = new DataTable();
            T_R_REPAIR_FAILCODE TFailcode=new T_R_REPAIR_FAILCODE(Station.SFCDB,Station.DBType);
            FailCodeInfo = TFailcode.SelectFailCodeBySN(sn, Station.SFCDB, Station.DBType);
            RepairFailCode.Value = ConvertToJson.DataTableToJson(FailCodeInfo);

            DataTable RepairActionInfo = new DataTable();
            T_r_repair_action TAction = new T_r_repair_action(Station.SFCDB, Station.DBType);
            RepairActionInfo = TAction.SelectRepairActionBySN(sn, Station.SFCDB, Station.DBType);
            RepairAction.Value = ConvertToJson.DataTableToJson(RepairActionInfo);
        }

        /// <summary>
        /// 從SN對象加載該SN已經維修的次數
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairCountDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            if (sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            //Repair Count Session
            MESStationSession sessionRepairCount = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionRepairCount == null)
            {
                sessionRepairCount = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionRepairCount);
            }
            try
            {
                LogicObject.SN snObject = (LogicObject.SN)sessionSN.Value;
                T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
                sessionRepairCount.Value = t_r_repair_main.GetRepairedCount(snObject.SerialNo, Station.SFCDB, Station.DBType);
                sessionRepairCount.InputValue = t_r_repair_main.GetRepairedCount(snObject.SerialNo, Station.SFCDB, Station.DBType).ToString();
                sessionRepairCount.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { Paras[1].SESSION_TYPE, sessionRepairCount.Value.ToString() }, MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }  
    }
}
