using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.MESReturnView.Station
{
    public class CallStationReturn
    {
        public MESStation.BaseClass.MESStationBase Station;
        public MESStation.BaseClass.MESStationInput NextInput = null;
        //public string[] StationMessage = null;
        public string[] StationLabel = null;
        /// <summary>
        /// Pass,Fail
        /// </summary>
        public string ScanType;
        //public string[] Ctrl = null;
        //public string[] printLabels = null;
    }

    public class StationMessage
    {
        public string Message;
        public StationMessageState State = StationMessageState.Message;
    }

    public enum StationMessageState
    {
        Fail = 0,
        Pass = 1,
        Message = 2,
        Alert= 3,
        Debug = 4
    }
}
