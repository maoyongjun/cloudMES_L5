using MESDataObject;
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

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckRepairFail
    {
        /// <summary>
        /// 維修輸入SN Fail狀態檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairFailChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec sfcdb = Station.SFCDB;
            //input test
            /*string inputValue = Input.Value.ToString();
            if (string.IsNullOrEmpty(inputValue))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN輸入值" }));
            }
            SN sn = new SN(inputValue, sfcdb, DB_TYPE_ENUM.Oracle);*/

            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE
                           && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            SN sn = (SN) SN_Session.Value;
            
            if (sn.RepairFailedFlag == "0")
            {
                //正常品
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000078", new string[] { sn.SerialNo }));
            }
            List<R_REPAIR_MAIN> repairMains = new T_R_REPAIR_MAIN(sfcdb, DB_TYPE_ENUM.Oracle).GetRepairMainBySN(sfcdb, sn.SerialNo);
            if (repairMains == null || repairMains.Count == 0)
            {
                //無維修主檔信息
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { "SN", sn.SerialNo }));
            }
            foreach (R_REPAIR_MAIN rm in repairMains)
            {
                //存在closed_flag=0
                if (rm.CLOSED_FLAG != "0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000097", new string[] {"SN", rm.SN }));
                }
            }
            Station.AddMessage("MES00000046", new string[] { "OK" }, StationMessageState.Pass);
        }

        /// <summary>
        /// 維修輸入SN Fail次數管控
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairCountChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //string inputValue = Input.Value.ToString();

            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SN" }));
            }
            SN sn = (SN) SN_Session.Value;
            string skuno = null;
            //Paras: SESSION_TYPE='SKU'  SESSION_KEY='1'  VALUE='0,1'
            switch (Paras[1].VALUE)
            {
                case "0":
                    skuno = sn.SkuNo;
                    break;
                default:
                    skuno = "ALL";
                    break;
            }

            OleExec sfcdb = Station.SFCDB;
            C_REPAIR_DAY repairDay = new T_C_REPAIR_DAY(sfcdb, DB_TYPE_ENUM.Oracle).GetDetailBySkuno(sfcdb, skuno);
            if (repairDay != null)
            {
                //repair_count
                if (repairDay.REPAIR_COUNT == 3)
                {
                    Station.AddMessage("MES00000087", new string[] { repairDay.REPAIR_COUNT.ToString(), "請注意" }, StationMessageState.Message);
                }
                if (repairDay.REPAIR_COUNT > 3)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000087", new string[] { repairDay.REPAIR_COUNT.ToString(), "已鎖定" }));
                }
            }
        }

    }
}
