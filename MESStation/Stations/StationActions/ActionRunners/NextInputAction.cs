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
    public class NextInputAction
    {
        public static void SMTLoadingPassNextInputAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            Station.NextInput = Input;
            Station.Inputs[3].Value = "";
        }

        public static void SetNextInputAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //参数1，获取一个内存变量，检查它的值是否为配置值，
            //如是，设置next为配置2的
            //
            R_Station_Action_Para P1 = Paras[0];
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == P1.SESSION_TYPE && t.SessionKey == P1.SESSION_KEY);
            if (s == null)
            {
                return;
            }
            if (s.Value.ToString() == P1.VALUE.ToString())
            {
                R_Station_Action_Para P2 = Paras[1];
                MESStationInput i = Station.Inputs.Find(t => t.DisplayName == P2.VALUE.ToString());
                if (i != null)
                {
                    Station.NextInput = i;
                }
            }
            else
            {
                R_Station_Action_Para P2 = Paras[2];
                MESStationInput i = Station.Inputs.Find(t => t.DisplayName == P2.VALUE.ToString());
                if (i != null)
                {
                    Station.NextInput = i;
                }
            }



        }

    }
}
