using System;
using System.Collections.Generic;
using MESStation.BaseClass;
using System.Data;
using MESDataObject;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class TRSNActions
    {
        /// <summary>
        /// 重新加载 TRSN 的数量并进行判断
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TRInputResetAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //獲取TR_SN
            string TRSN = string.Empty;

            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }


            TRSN = Paras[1].VALUE.ToString();
            //使用 TRSN 重新加載 TRSNWIP，目前不存在 DataLoader 來根據 TRSN 加載 TRSNWIP 信息
            

            MESStationSession TRSNWip = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSNWip == null)
            {
                TRSNWip = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSNWip);
            }

            double ExtQty = ((DataRow)TRSNWip.Value)["EXT_QTY"]==null?0.0d:double.Parse(((DataRow)TRSNWip.Value)["EXT_QTY"].ToString());
            TRSN = ((DataRow)TRSNWip.Value)["TR_SN"]==null?"": ((DataRow)TRSNWip.Value)["TR_SN"].ToString();
            if (ExtQty <= 0)
            {
                MESStationInput input = Station.FindInputByName("TRSN");
                input.Enable = true;
                Station.NextInput = input;
                Station.AddMessage("MES00000039", new string[] { TRSN }, MESReturnView.Station.StationMessageState.Message);
            }
            else
            {
                TRSNWip.Value = ExtQty--;
                MESStationInput input = Station.FindInputByName("NEW_SN");
            }

        }
    }
}
