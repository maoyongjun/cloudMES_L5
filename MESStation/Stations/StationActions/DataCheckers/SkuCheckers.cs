using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class SkuCheckers
    {
        /// <summary>
        /// 檢查輸入的料號與工單的料號是否一致
        /// 張官軍 2018/01/18
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputSkuWoSkuChecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string SkuNo = string.Empty;
            //marked by LLF 2018-02-24
            //MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (WoSession == null)
            //{
            //    MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");
            //    if (SNSession != null)
            //    {
            //        SN Sn = ((SN)SNSession.Value);
            //        WorkOrder WoTemp = new WorkOrder();
            //        WoTemp.Init(Sn.WorkorderNo, Station.SFCDB);
            //        WoSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = WoTemp };
            //        Station.StationSession.Add(WoSession);
            //    }
            //    else
            //    {
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] {Paras[0].SESSION_TYPE+Paras[0].SESSION_KEY });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //}
            //WorkOrder Wo = ((WorkOrder)WoSession.Value);

            //SkuNo = Input.Value.ToString().ToUpper().Trim();
            //if (Wo != null)
            //{
            //    if (Wo.SkuNO.Equals(SkuNo))
            //    {
            //        Station.AddMessage("MES00000111", new string[] { SkuNo }, MESReturnView.Station.StationMessageState.Pass);
            //    }
            //    else
            //    {
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000110", new string[] {SkuNo,Wo.WorkorderNo });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //}

            MESStationSession InputSKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession SNSKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            if (InputSKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if (SNSKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if (InputSKUSession.Value.ToString() != SNSKUSession.Value.ToString())
            {
                //誰把這段防呆屏蔽了？Openned by LLF 2018-03-17
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000110", new string[] { InputSKUSession.Value.ToString(), SNSKUSession.Value.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
        }
    }
}
