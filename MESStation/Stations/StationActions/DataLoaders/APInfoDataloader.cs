using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESDBHelper;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class APInfoDataloader
    {
        /// <summary>
        /// 加載SKU在Allpart的配置信息從Allpart系統加載mes1.c_product_config, mes4.r_pcba_link 
        /// 將查詢到的結果存入Dictionary<string_Datarow> 中, key為表名如:" c_product_config "
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SkuAPInfoDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string StrSku = "";
            string StrVer = "";
            SKU sku = new SKU();
            OleExec apdb = null;
            Dictionary<string, List<DataRow>> APInfo = new Dictionary<string, List<DataRow>>();
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession APConfig_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (APConfig_Session == null)
            {
                APConfig_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(APConfig_Session);
            }

            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            //Modify by LLF 2017-01-25 For 獲取工單對象，從工單對象中獲取料號，版本
            //MESStationSession Sku_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            //if (Sku_Session == null)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            //}
            //else
            //{
            //    sku = (SKU)Sku_Session.Value;
            //    if (sku == null || sku.SkuNo == null || sku.SkuNo.Length <= 0)
            //    {
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            //    }
            //    if (sku.Version == null || sku.Version.Length <= 0)
            //    {
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " Version" }));
            //    }
            //}
            //獲取ALLPART數據 
            try
            {
                StrSku = ((MESStation.LogicObject.WorkOrder)Wo_Session.Value).SkuNO;
                StrVer = ((MESStation.LogicObject.WorkOrder)Wo_Session.Value).SKU_VER;
                apdb = Station.APDB;
                AP_DLL APDLL = new AP_DLL();
                List<DataRow> ConfigList = APDLL.C_Product_Config_GetBYSkuAndVerson(StrSku, StrVer, apdb);
                if (ConfigList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + StrSku + ",VER:" + StrVer, "C_PRODUCT_CONFIG" }));
                }
                APInfo.Add("C_PRODUCT_CONFIG", ConfigList);
                List<DataRow> PCBALinkList = APDLL.R_PCBA_LINK_GetBYSku(StrSku, apdb);
                if (PCBALinkList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + StrSku, "R_PCBA_LINK" }));
                }
                APInfo.Add("R_PCBA_LINK", PCBALinkList);
                APConfig_Session.Value = APInfo;
               
                Station.AddMessage("MES00000001", new string[] { StrSku }, MESReturnView.Station.StationMessageState.Pass);
                //Modify By LLF 2018-01-25 For 料號&版本從工單對象中獲取，而不是從C_SKU 中獲取
                /* List<DataRow> ConfigList = APDLL.C_Product_Config_GetBYSkuAndVerson(sku.SkuNo, sku.Version, apdb);
                if (ConfigList.Count <= 0)
                {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + sku.SkuNo, "C_PRODUCT_CONFIG" }));
                }
                APInfo.Add("C_PRODUCT_CONFIG", ConfigList);
                List<DataRow> PCBALinkList = APDLL.R_PCBA_LINK_GetBYSku(sku.SkuNo, apdb);
                if (PCBALinkList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "SKU:" + sku.SkuNo, "R_PCBA_LINK" }));
                }
                APInfo.Add("R_PCBA_LINK", PCBALinkList);
                APConfig_Session.Value = APInfo;
                Station.DBS["APDB"].Return(apdb);
                Station.AddMessage("MES00000001", new string[] { sku.SkuNo }, MESReturnView.Station.StationMessageState.Pass);
                */
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                   
                }
                throw ex;
            }
        }
        /// <summary>
        /// TR_SN數據加載，查詢R_TR_SN,R_TR_SN_WIP的數據保存到Dictionary<string_Datarow>中,key為表名 "R_TR_SN","R_TR_SN_WIP"
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TRSNDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            Dictionary<string, DataRow> APInfo = new Dictionary<string, DataRow>();
            string strTRSN = "";
            string ErrMessage = "";
            OleExec apdb = null;
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            strTRSN = Input.Value.ToString();
            MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSN_Session == null)
            {

                TRSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSN_Session);
            }
            else
            {
                TRSN_Session.ResetInput = Input;
                TRSN_Session.InputValue = strTRSN;
            }

            //獲取ALLPART數據
            APInfo = new Dictionary<string, DataRow>();
            AP_DLL APDLL = new AP_DLL();
            try
            {
                apdb = Station.APDB;
                List<DataRow> TRSNlist = APDLL.R_TR_SN_GetBYTR_SN(strTRSN, apdb);
                if (TRSNlist.Count <= 0)
                {
                    //throw new Exception("TRSN:" + "不存在ALLPART系統R_TR_SN中");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "TRSN:" + strTRSN, "R_TR_SN" }));
                }
                else
                {
                    APInfo.Add("R_TR_SN", TRSNlist[0]);
                }
                List<DataRow> TRSNWIPlist = APDLL.R_TR_SN_WIP_GetBYTR_SN(strTRSN, apdb);
                if (TRSNWIPlist.Count <= 0)
                {
                    //throw new Exception("TRSN:" + "不存在ALLPART系統R_TR_SN_WIP中");
                    //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "TRSN:" + strTRSN, "R_TR_SN_WIP" }));
                    APInfo.Add("R_TR_SN_WIP", null);
                }
                else
                {
                    APInfo.Add("R_TR_SN_WIP", TRSNWIPlist[0]);
                }
                TRSN_Session.Value = APInfo;
                
                Station.AddMessage("MES00000001", new string[] { TRSN_Session.Value.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    
                }
                throw ex;
            }
        }

        //Add by LLF 2017-01-26 Begin
        public static void TrCodeDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Process = "";
            string Message = "";
            string StrWO = "";
            string StrCode = "";
            string IP = string.Empty;
            OleExec APDB = null;

            Dictionary<string, DataRow> TrSnTable = null;
            T_R_SN Table = new T_R_SN(Station.SFCDB, Station.DBType);

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrWO = WoSession.Value.ToString();

            //獲取 TRSN 對象
            MESStationSession TrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrSnSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            TrSnTable = (Dictionary<string, DataRow>)TrSnSession.Value;

            Process = Paras[2].VALUE.ToString();

            IP = Paras[3].VALUE.ToString();

            try
            {
                APDB = Station.APDB;
                StrCode = Table.GetAPTrCode(StrWO, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, Station.LoginUser.EMP_NO, IP, APDB);
               

                MESStationSession TrCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
                if (TrCodeSession == null)
                {
                    TrCodeSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(TrCodeSession);
                }

                TrCodeSession.Value = StrCode;
                Station.AddMessage("MES00000001", new string[] { TrCodeSession.Value.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch(Exception ex)
            {
                
                throw (ex);
            }
        }
        //Add by LLF 2017-01-26 End

        /// <summary>
        /// TR_SN數據對象加載其中每個欄位 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadDataFromTRSNDataLoader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            object TRSN_SessionObject;
            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (TRSN_Session == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (TRSN_Session.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                TRSN_SessionObject = TRSN_Session.Value;

                //獲取TR_SN 欄位數據
                Dictionary<string, DataRow> dd = (Dictionary<string, DataRow>)TRSN_SessionObject;
                DataRow tr;
                dd.TryGetValue("R_TR_SN", out tr);

                MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "KP_NO");
                List<object> ret = I.DataForUse;
                ret.Clear();
                if (I != null)
                {
                    ret.Add(tr["CUST_KP_NO"].ToString());
                }

                MESStationInput I1 = Station.Inputs.Find(t => t.DisplayName == "MFR_Name");
                List<object> ret1 = I1.DataForUse;
                if (I1 != null)
                {
                    ret1.Add(tr["MFR_KP_NO"].ToString());
                }
                MESStationInput I2 = Station.Inputs.Find(t => t.DisplayName == "Date_Code");
                List<object> ret2 = I2.DataForUse;
                if (I2 != null)
                {
                    ret2.Add(tr["DATE_CODE"].ToString());
                }
                MESStationInput I3 = Station.Inputs.Find(t => t.DisplayName == "Lot_Code");
                List<object> ret3 = I3.DataForUse;
                if (I3 != null)
                {
                    ret3.Add(tr["LOT_CODE"].ToString());
                }
                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Message);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// 加載TR_SN的KP_NO,MFR_Name,Date_Code,Lot_Code到MESStationSession,以便輸出
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TRSNInfoDataLoader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            object TRSN_SessionObject;
            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (TRSN_Session == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (TRSN_Session.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }

                MESStationSession sessionKPNO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (sessionKPNO == null)
                {
                    sessionKPNO = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(sessionKPNO);
                }
                MESStationSession sessionMFRName = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (sessionMFRName == null)
                {
                    sessionMFRName = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(sessionMFRName);
                }
                MESStationSession sessionDateCode = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (sessionDateCode == null)
                {
                    sessionDateCode = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(sessionDateCode);
                }
                MESStationSession sessionLotCode = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
                if (sessionLotCode == null)
                {
                    sessionLotCode = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(sessionLotCode);
                }

                TRSN_SessionObject = TRSN_Session.Value;

                //獲取TR_SN 欄位數據
                Dictionary<string, DataRow> dd = (Dictionary<string, DataRow>)TRSN_SessionObject;
                DataRow tr;
                dd.TryGetValue("R_TR_SN", out tr);
                if (tr != null)
                {
                    sessionKPNO.Value = tr["CUST_KP_NO"].ToString();
                    sessionMFRName.Value = tr["MFR_KP_NO"].ToString();
                    sessionDateCode.Value = tr["DATE_CODE"].ToString();
                    sessionLotCode.Value = tr["Lot_Code"].ToString();
                }  
                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        /// <summary>
        /// 根據SN的SKUNO帶出維修LOCATION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LocationFromSNDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            List<string> LocationList = new List<string>();
            string ErrMessage = "";
            OleExec apdb = null;
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SN ObjSn = (SN)SNSession.Value;

            //獲取ALLPART數據
            AP_DLL APDLL = new AP_DLL();
            MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "Location");
            List<object> ret = I.DataForUse;
            ret.Clear();
            try
            {
                apdb = Station.APDB;
                
                LocationList = APDLL.GetLocationList(ObjSn.SkuNo, apdb);
                if (LocationList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { }));
                }
                else
                {
                    foreach (object item in LocationList)
                    {
                        ret.Add(item);
                    }
                }
                
                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    
                }
                throw ex;
            }
        }

        //Add by LLF 2018-02-19 Begin
        public static void PTHTrCodeDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Message = "";
            string StrWO = "";
            string StrCode = "";
            string StationName = string.Empty;
            string StrStationNum = string.Empty;
            OleExec APDB = null;

            T_R_SN Table = new T_R_SN(Station.SFCDB, Station.DBType);

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrWO = WOSession.Value.ToString();

            //獲取 TRSN 對象
            MESStationSession StationNumSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StationNumSession == null)
            {
                Message = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(Message);
            }
            StrStationNum = StationNumSession.Value.ToString();

            StationName = Station.Line + Station.StationName + StrStationNum;
            try
            {
                APDB = Station.APDB;
                StrCode = Table.GetAPPTHTrCode(StrWO, StationName, APDB);
                

                MESStationSession PTHTrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (PTHTrSnSession == null)
                {
                    PTHTrSnSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(PTHTrSnSession);
                }

                PTHTrSnSession.Value = StrCode;
                Station.AddMessage("MES00000001", new string[] { PTHTrSnSession.Value.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                
                throw (ex);
            }
        }
        //Add by LLF 2018-02-19 End
    }
}
