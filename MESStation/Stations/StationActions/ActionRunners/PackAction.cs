using MESStation.LogicObject;
using MESStation.MESReturnView.Station;
using System.Collections;
using MESDataObject;
using System.Data;
using MESDBHelper;
using MESStation.Stations.StationActions.DataLoaders;
using System.Data.OleDb;
using System.Collections.Generic;
using MESDataObject.Module;
using System.Reflection;
using MESStation.Label;
using MESStation.BaseClass;
using MESStation.Packing;
using System;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class PackAction
    {
        public static void CloseCartionAndPalletAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }

            MESStationSession sessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCarton == null)
            {
                throw new System.Exception("sessionCartion miss ");
            }

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallet == null)
            {
                throw new System.Exception("sessionPallet miss ");
            }
            MESStationSession sessionPrintPL = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_PL" && t.SessionKey == "1");
            if (sessionPrintPL == null)
            {
                sessionPrintPL = new MESStationSession() { MESDataType = "ISPRINT_PL", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintPL);
            }
            sessionPrintPL.Value = "FALSE";
            MESStationSession sessionPrintCTN = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN" && t.SessionKey == "1");
            if (sessionPrintCTN == null)
            {
                sessionPrintCTN = new MESStationSession() { MESDataType = "ISPRINT_CTN", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN);
            }
            sessionPrintCTN.Value = "FALSE";
            CartionBase cartion = (CartionBase)sessionCarton.Value;
            PalletBase Pallet = (PalletBase)sessionPallet.Value;

            cartion.DATA.CLOSED_FLAG = "1";
            cartion.DATA.EDIT_TIME = DateTime.Now;
            cartion.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
            Station.SFCDB.ExecSQL(cartion.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));

            Pallet.DATA.CLOSED_FLAG = "1";
            Pallet.DATA.EDIT_TIME = DateTime.Now;
            Pallet.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
            Station.SFCDB.ExecSQL(cartion.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
            sessionPrintPL.Value = "TRUE";
            sessionPrintCTN.Value = "TRUE";

            MESStationSession CTNPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_CTN" && T.SessionKey == "1");
            if (CTNPrintSession == null)
            {
                CTNPrintSession = new MESStationSession() { MESDataType = "PRINT_CTN", SessionKey = "1" };
                Station.StationSession.Add(CTNPrintSession);
            }
            CTNPrintSession.Value = cartion.DATA.PACK_NO;

            MESStationSession PlPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_PL" && T.SessionKey == "1");
            if (PlPrintSession == null)
            {
                PlPrintSession = new MESStationSession() { MESDataType = "PRINT_PL", SessionKey = "1" };
                Station.StationSession.Add(PlPrintSession);
            }
            PlPrintSession.Value = Pallet.DATA.PACK_NO;


            Station.StationSession.Remove(sessionCarton);
            Station.StationSession.Remove(sessionPallet);




        }
        public static void CartionAndPalletAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new System.Exception("sessionSN miss ");
            }

            MESStationSession sessionCartion = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionCartion == null)
            {
                throw new System.Exception("sessionCartion miss ");
            }

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionPallet == null)
            {
                throw new System.Exception("sessionPallet miss ");
            }

            MESStationSession sessionPrintPL = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_PL" && t.SessionKey == "1");
            if (sessionPrintPL == null)
            {
                sessionPrintPL = new MESStationSession() { MESDataType = "ISPRINT_PL", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintPL);
            }
            sessionPrintPL.Value = "FALSE";
            MESStationSession sessionPrintCTN = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN" && t.SessionKey == "1");
            if (sessionPrintCTN == null)
            {
                sessionPrintCTN = new MESStationSession() { MESDataType = "ISPRINT_CTN", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN);
            }
            sessionPrintCTN.Value = "FALSE";

            SN SN = (SN)sessionSN.Value;

            if (SN.isPacked(Station.SFCDB))
            {
                throw new System.Exception($@"{SN.SerialNo} is packed!");
            }

            CartionBase cartion = (CartionBase)sessionCartion.Value;
            PalletBase Pallet = (PalletBase)sessionPallet.Value;

            

            cartion.Add(SN, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

            if (cartion.DATA.MAX_QTY <= cartion.GetCount(Station.SFCDB))
            {
                sessionPrintCTN.Value = "TRUE";
                //設置打印變量
                MESStationSession CTNPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_CTN" && T.SessionKey == "1");
                if (CTNPrintSession == null)
                {
                    CTNPrintSession = new MESStationSession() { MESDataType = "PRINT_CTN", SessionKey = "1" };
                    Station.StationSession.Add(CTNPrintSession);
                }
                CTNPrintSession.Value = cartion.DATA.PACK_NO;
                T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                List<C_PACKING> PackConfigs = TCP.GetPackingBySku(SN.SkuNo, Station.SFCDB);
                C_PACKING CartionConfig = PackConfigs.Find(T => T.PACK_TYPE == "CARTION");
                C_PACKING PalletConfig = PackConfigs.Find(T => T.PACK_TYPE == "PALLET");
                if (CartionConfig == null)
                {
                    throw new Exception("Can't find CartionConfig");
                }
                if (PalletConfig == null)
                {
                    throw new Exception("Can't find PalletConfig");
                }
                if (Pallet.DATA.MAX_QTY <= Pallet.GetCount(Station.SFCDB))
                {
                    sessionPrintPL.Value = "TRUE";
                    //設置打印變量
                    MESStationSession PlPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_PL" && T.SessionKey == "1");
                    if (PlPrintSession == null)
                    {
                        PlPrintSession = new MESStationSession() { MESDataType = "PRINT_PL", SessionKey = "1" };
                        Station.StationSession.Add(PlPrintSession);
                    }
                    PlPrintSession.Value = Pallet.DATA.PACK_NO;

                    Pallet.DATA.CLOSED_FLAG = "1";
                    Pallet.DATA.EDIT_TIME = DateTime.Now;
                    Pallet.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
                    Station.SFCDB.ExecSQL(Pallet.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));

                    Pallet.DATA = PackingBase.GetNewPacking(PalletConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

                }
                cartion.DATA.CLOSED_FLAG = "1";
                cartion.DATA.EDIT_TIME = DateTime.Now;
                cartion.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
                Station.SFCDB.ExecSQL(cartion.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
                cartion.DATA = PackingBase.GetNewPacking(CartionConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

                Pallet.Add(cartion, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }
            sessionCartion.Value = cartion;
            sessionPallet.Value = Pallet;

            cartion.DATA.AcceptChange();
            Pallet.DATA.AcceptChange();

        }
        
        public static void CloseLot(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            DisplayOutPut Dis_LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO");
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS r = tRLotStatus.GetByLotNo(Dis_LotNo.Value.ToString(), Station.SFCDB);
            if (r == null || !r.CLOSED_FLAG.Equals("0"))
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180528103627", new string[] { Dis_LotNo.Value.ToString() }));
            try
            {
                r.CLOSED_FLAG = "1";
                r.EDIT_EMP = Station.LoginUser.EMP_NO;
                Station.SFCDB.ThrowSqlExeception = true;
                r.EDIT_TIME = tRLotStatus.GetDBDateTime(Station.SFCDB);
                Station.SFCDB.ExecSQL(r.GetUpdateString(Station.DBType));
            }
            catch (Exception e)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180528105826", new string[] { Dis_LotNo.Value.ToString(), e.Message }));
            }
            finally { Station.SFCDB.ThrowSqlExeception = false; }
            #region 清空界面信息
            Station.StationSession.Clear();
            Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE).DataForUse.Clear();
            #endregion
        }


    }
}
