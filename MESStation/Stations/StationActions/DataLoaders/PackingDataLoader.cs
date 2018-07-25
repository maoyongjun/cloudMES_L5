using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESStation.BaseClass;
using MESDataObject.Module;
namespace MESStation.Stations.StationActions.DataLoaders
{
    public class PackingDataLoader
    {
        /// <summary>
        /// 為重打印棧板而完成打印設置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadPalletFromInputForPrint(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            
            Packing.PalletBase pallet = new Packing.PalletBase(Input.Value.ToString(), Station.SFCDB);
            MESStationSession sessionPrintPL = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_PL" && t.SessionKey == "1");
            if (sessionPrintPL == null)
            {
                sessionPrintPL = new MESStationSession() { MESDataType = "ISPRINT_PL", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintPL);
            }
            sessionPrintPL.Value = "TRUE";

            MESStationSession PlPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_PL" && T.SessionKey == "1");
            if (PlPrintSession == null)
            {
                PlPrintSession = new MESStationSession() { MESDataType = "PRINT_PL", SessionKey = "1" };
                Station.StationSession.Add(PlPrintSession);
            }
            PlPrintSession.Value = pallet.DATA.PACK_NO;
            LogicObject.SKU sku = new LogicObject.SKU();
            sku.Init(pallet.DATA.SKUNO, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession SKUSession = Station.StationSession.Find(T => T.MESDataType == "SKU" && T.SessionKey == "1");
            if (SKUSession == null)
            {
                SKUSession = new MESStationSession() { MESDataType = "SKU", SessionKey = "1" };
                Station.StationSession.Add(SKUSession);
            }
            SKUSession.Value = sku;


        }

        public static void LoadPackingBySkuStation(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            
            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSKU == null)
            {
                throw new Exception("Can't load session SKU");
            }
            LogicObject.SKU SKU = (LogicObject.SKU)sessionSKU.Value;
            MESStationSession sessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCarton == null)
            {
                sessionCarton = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(sessionCarton);
            }

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallet == null)
            {
                sessionPallet = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionPallet);
            }

            T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_PACKING> PackConfigs = TCP.GetPackingBySku(SKU.SkuNo, Station.SFCDB);
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

            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_PACKING RowCartion = null;
            try
            {
                RowCartion = (Row_R_PACKING)TRP.GetObjBySelect($@"select * from R_PACKING where SKUNO='{SKU.SkuNo}' 
and PACK_TYPE='{CartionConfig.PACK_TYPE}' 
and LINE='{Station.Line}' 
and STATION='{Station.StationName}' 
and IP='{Station.IP}' and CLOSED_FLAG='0'", Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            catch
            {
                RowCartion = Packing.PackingBase.GetNewPacking(CartionConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }

            Row_R_PACKING RowPallet = null;
            try
            {
                RowPallet = (Row_R_PACKING)TRP.GetObjBySelect($@"select * from R_PACKING where SKUNO='{SKU.SkuNo}' 
and PACK_TYPE='{PalletConfig.PACK_TYPE}' 
and LINE='{Station.Line}' 
and STATION='{Station.StationName}' 
and IP='{Station.IP}' and CLOSED_FLAG='0'", Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            catch
            {
                RowPallet = Packing.PackingBase.GetNewPacking(PalletConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }
            Packing.CartionBase Cartion = new Packing.CartionBase(RowCartion);
            Packing.PalletBase Pallet = new Packing.PalletBase(RowPallet);
            if (Cartion.DATA.PARENT_PACK_ID == null || Cartion.DATA.PARENT_PACK_ID == "")
            {
                Pallet.Add(Cartion, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }

            sessionCarton.Value = Cartion;
            sessionPallet.Value = Pallet;



        }
    }
}
