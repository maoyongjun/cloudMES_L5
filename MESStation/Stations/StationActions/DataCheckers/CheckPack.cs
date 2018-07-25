using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESStation.HateEmsGetDataService;
using System.Net;
using System.Text.RegularExpressions;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckPack
    {
        /// <summary>
        /// 檢查Pack狀態是否可以入OBA
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackStatusInOba(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            DisplayOutPut Dis_LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO");
            DisplayOutPut Dis_SkuNo = Station.DisplayOutput.Find(t => t.Name == "SKUNO");
            DisplayOutPut Dis_Ver = Station.DisplayOutput.Find(t => t.Name == "VER");
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            #region 用於界面上顯示的批次信息
            R_LOT_STATUS rLotStatus = new R_LOT_STATUS();
            List<R_LOT_PACK> rLotPackList = new List<R_LOT_PACK>();
            #endregion
            LotNo LotNo = new LotNo(Station.DBType);
            T_R_LOT_PACK tRLotPack = new T_R_LOT_PACK(Station.SFCDB, Station.DBType);
            rLotPackList = tRLotPack.GetRLotPackWithWaitClose(Station.SFCDB, packSession.Value.ToString());
            List<R_LOT_STATUS> rLotStatusList = tRLotPack.GetRLotStatusWithWaitClose(Station.SFCDB, packSession.Value.ToString());
            rLotStatus = rLotStatusList.Find(t => t.CLOSED_FLAG == "1");
            if (rLotStatus != null)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180526181007", new string[] { packSession.Value.ToString() }));
            else
                rLotStatus = rLotStatusList.Find(t => t.CLOSED_FLAG == "0");
            if (Dis_LotNo.Value.Equals(""))
            {
                #region 當前Lot為空=>檢查當前Pack無有效LOT?新建LOT:加載LOT;
                if (rLotPackList.Count == 0)
                {
                    rLotStatus = LotNo.CreateLotByPackno(Station.LoginUser, packSession.Value.ToString(), Station.SFCDB);
                    rLotPackList.Add(new R_LOT_PACK() { LOTNO = rLotStatus.LOT_NO, PACKNO = packSession.Value.ToString() });
                }
                else
                    rLotStatus = rLotStatusList.Find(t => t.CLOSED_FLAG == "0");
                #endregion
            }
            else
            {
                #region 當前Lot不為空=>PackNo與當前頁面LOT的機種版本是否一致?ReLoad LOT信息:Throw e;
                T_R_PACKING tRPacking = new T_R_PACKING(Station.SFCDB, Station.DBType);
                Row_R_PACKING rowRPacking = tRPacking.GetRPackingByPackNo(Station.SFCDB, packSession.Value.ToString());
                if (!rowRPacking.SKUNO.Equals(Dis_SkuNo.Value))
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180526185434", new string[] { packSession.Value.ToString() }));
                if (rLotPackList.Count == 0)
                {
                    rLotStatus = LotNo.ObaInLotByPackno(Station.User, rLotStatus, packSession.Value.ToString(), Station.SFCDB);
                    rLotPackList.Add(new R_LOT_PACK() { LOTNO = rLotStatus.LOT_NO, PACKNO = packSession.Value.ToString() });
                }
                else
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180526185618", new string[] { packSession.Value.ToString(), rLotStatus.LOT_NO }));
                #endregion
            }

            #region 加載界面信息
            MESStationSession lotNoSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
            MESStationSession skuNoSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
            MESStationSession aqlSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
            MESStationSession lotQtySession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
            MESStationSession sampleQtySession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
            MESStationSession RejectQtySession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };

            Station.StationSession.Clear();
            Station.StationSession.Add(lotNoSession);
            Station.StationSession.Add(skuNoSession);
            Station.StationSession.Add(aqlSession);
            Station.StationSession.Add(lotQtySession);
            Station.StationSession.Add(sampleQtySession);
            Station.StationSession.Add(RejectQtySession);

            lotNoSession.Value = rLotStatus.LOT_NO;
            skuNoSession.Value = rLotStatus.SKUNO;
            aqlSession.Value = rLotStatus.AQL_TYPE;
            lotQtySession.Value = rLotStatus.LOT_QTY;
            sampleQtySession.Value = rLotStatus.SAMPLE_QTY;
            RejectQtySession.Value = rLotStatus.REJECT_QTY;

            MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
            s.DataForUse.Clear();
            foreach (var VARIABLE in rLotPackList)
            {
                s.DataForUse.Add(VARIABLE.PACKNO);
            }
            #endregion
        }

        /// <summary>
        /// 檢查棧板中SN是否有被鎖定
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackSnStatusIsLock(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_SN_LOCK tRSnLock = new T_R_SN_LOCK(Station.SFCDB, Station.DBType);
            List<R_SN_LOCK> rSnLockList = tRSnLock.GetLockListByPackNo(packSession.Value.ToString(),Station.SFCDB);
            string strSnList = "";
            foreach (R_SN_LOCK VARIABLE in rSnLockList)
                strSnList += VARIABLE.SN + ",";
            if(rSnLockList.Count>0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180531114237", new string[] { packSession.Value.ToString(), rSnLockList.Count().ToString(), strSnList }));
        }

    }
}
