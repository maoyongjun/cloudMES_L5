using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using System.Data;
using MESDataObject.Module;
using MESStation.BaseClass;
using MESStation.LogicObject;
using MESStation.MESReturnView.Station;
using MESDBHelper;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class TestAction
    {
        public static void TEST1(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string SN = SNMaker.SNmaker.GetNextSN("TEST",Station.SFCDB);

            Station.StationMessages.Add(new StationMessage() { Message = SN, State = StationMessageState.Message });
        }
    }
}
