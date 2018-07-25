using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using MESDataObject;
using MESStation.LogicObject;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckKP
    {
        public static void SNStationKPDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //MESStationSession WO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;

            SN sn = (SN)SNSession.Value;
            T_R_SN_KP TRKP = new T_R_SN_KP(SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> snkp = TRKP.GetKPRecordBySnIDStation(sn.ID, Station.StationName, SFCDB);

            List<R_SN_KP> kpwait = snkp.FindAll(T => T.VALUE == "" || T.VALUE == null);
            if (kpwait.Count > 0)
            {
                Station.AddKPScan(sn.SerialNo, sn.WorkorderNo, Station.StationName);
                throw new Exception($@"{sn.SerialNo} 缺少Keypart");
            }


        }
    }
}
