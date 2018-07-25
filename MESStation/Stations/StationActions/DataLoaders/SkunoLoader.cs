using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESDataObject;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class SkunoLoader
    {
        public static void SkuDataLoader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
            SKU sku = new SKU();
            try
            {
                MESDBHelper.OleExec ole = Station.SFCDB;
                sku = sku.Init(Input.Value.ToString(), ole, MESDataObject.DB_TYPE_ENUM.Oracle);
                s.Value = sku;
                Station.AddMessage("MES00000029", new string[] { "Skuno", sku.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch
            {

                //Station.AddMessage("MES00000052", new string[] { "Skuno"+ ":"+ Input.Value.ToString() }, MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "Skuno" + ":" + Input.Value.ToString() }));
            }


        }

        public static void SmtSkuFromWODataCheck(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載SKUSavePoint位置
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuSession);
            }
            //加載WoLoadPoint
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
                // throw new Exception("WO參數不存在!");
            }

            WorkOrder WOObject = (WorkOrder)WOSession.Value;

            //WOObject.SkuNO

            SKU sku = new SKU();
            try
            {
                MESDBHelper.OleExec ole = Station.SFCDB;
                sku = sku.Init(WOObject.SkuNO, ole, MESDataObject.DB_TYPE_ENUM.Oracle);

                MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                DisplayOutPut CurrentSkuno = Station.DisplayOutput.Find(t => t.Name == "SKUNO");
                //SN機種與當前操作機種不一致卡住
                if (CurrentSkuno.Value != null && CurrentSkuno.Value.ToString().Trim().Length > 0 && !sku.SkuNo.Equals(CurrentSkuno.Value.ToString()))
                {
                    string ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000235", new string[] { SessionSN.Value.ToString(), sku.SkuNo, CurrentSkuno.Value.ToString() });
                    throw new MESReturnMessage(ErrMessage);
                }

                SkuSession.Value = sku;
                Station.AddMessage("MES00000029", new string[] { "Skuno", sku.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception e)
            {
                Station.AddMessage("MES00000007", new string[] { "Skuno" }, MESReturnView.Station.StationMessageState.Fail);
                throw e;
            }
        }

        public static void SkuFromWODataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載SKUSavePoint位置
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuSession);
            }
            //加載WoLoadPoint
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
                // throw new Exception("WO參數不存在!");
            }

            WorkOrder WOObject = (WorkOrder)WOSession.Value;

            //WOObject.SkuNO

            SKU sku = new SKU();
            try
            {
                MESDBHelper.OleExec ole = Station.SFCDB;
                sku = sku.Init(WOObject.SkuNO, ole, MESDataObject.DB_TYPE_ENUM.Oracle);

                //MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                //DisplayOutPut CurrentSkuno = Station.DisplayOutput.Find(t => t.Name == "SKUNO");
                ////SN機種與當前操作機種不一致卡住
                //if (CurrentSkuno.Value!=null&&CurrentSkuno.Value.ToString().Trim().Length > 0 && !sku.SkuNo.Equals(CurrentSkuno.Value.ToString()))
                //{
                //    string ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000235", new string[] { SessionSN.Value.ToString(), sku.SkuNo, CurrentSkuno.Value.ToString() });
                //    throw new MESReturnMessage(ErrMessage);
                //}

                SkuSession.Value = sku;
                Station.AddMessage("MES00000029", new string[] { "Skuno", sku.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch(Exception e)
            {
                Station.AddMessage("MES00000007", new string[] { "Skuno" }, MESReturnView.Station.StationMessageState.Fail);
                throw e;
            }


        }

        public static void SnSKUDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載SnPoint位置
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                //modify by 張官軍 2018-03-15
                //根據該方法的定義：從 SN 對象加載機種對象，SnSession 不能為空
                //SnSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                //Station.StationSession.Add(SnSession);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000170"));
            }
            //加載SkunoPoint
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(SkuSession);
                //throw new MESReturnMessage("Skuno參數不存在");
            }

            // BY SDL 20180320  SN SnObject = (SN)SkuSession.Value;.

            SN SnObject = (SN)SnSession.Value;

            //SnObject.SerialNo

            SKU sku = new SKU();

            try
            {
                MESDBHelper.OleExec ole = Station.SFCDB;
                sku = sku.InitBySn(SnObject.SerialNo, ole, MESDataObject.DB_TYPE_ENUM.Oracle);
                SkuSession.Value = sku;
                Station.AddMessage("MES00000029", new string[] { "SN", sku.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch
            {
                //Station.AddMessage("MES00000007", new string[] { "SN" }, MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN" }));

            }


        }

        /// <summary>
        /// 通過SKU對象，獲取對應批次對象,如果批次對象為空，新增批次對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">2個參數,SKU,LOTNO保存的位置</param>
        public static void GetLotDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ColoumName = "skuno";
            Row_R_LOT_STATUS RLotSku;
            T_R_LOT_STATUS TR = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            LotNo LOT = new LotNo();
            if (Paras.Count<=0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession Ssku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession Slot = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            MESStationSession SLotNewFlag= Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);

            if (Ssku == null)
            {
                //throw new Exception("请输入SKU!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            else
            {
                SKU ObjSku = (SKU)Ssku.Value;
                //Marked by LLF 20018-02-22 
                //RLotSku = TR.GetByInput(ObjSku.SkuNo, ColoumName, Station.SFCDB);
                RLotSku = TR.GetLotBySku(ObjSku.SkuNo, ColoumName, Station.SFCDB);
            }

            if (Slot == null)
            {
                Slot = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Slot);
            }

            if (SLotNewFlag == null)
            {
                SLotNewFlag = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE,SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SLotNewFlag);
            }

            try
            {
                //Modify by LLF 2018-02-07 SLotNewFlag 為新生成Lot的標誌位
                //if (LOT == null)//LOT 為空需產生新的LOTNO
                if (RLotSku== null)
                {
                    SLotNewFlag.Value = "1";
                    Slot.Value = LOT.GetNewLotNo("HWD_FQCLOT", Station.SFCDB);
                }
                else
                {
                    SLotNewFlag.Value = "0";
                    LOT.Init(RLotSku.LOT_NO,"", Station.SFCDB);
                    Slot.Value = LOT;
                    Station.AddMessage("MES00000029", new string[] { "LotNo", LOT.ToString() }, MESReturnView.Station.StationMessageState.Message);
                }

            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }
        }

        /// <summary>
        /// 獲取批次抽樣數量
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,SKU 保存的位置</param>
        public static void GetSampleQtyDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            Row_C_SKU_SAMPLE RLotSku;
            Row_C_AQLTYPE RAqltype;
            T_C_AQLTYPE QR=new T_C_AQLTYPE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_SKU_SAMPLE TR = new T_C_SKU_SAMPLE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            if (Paras.Count != 10)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession SessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            MESStationSession SessionAQLTYPE = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            MESStationSession SessionLotQTY = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            MESStationSession SessionSAMPLEQTY = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            MESStationSession SessionREJECTQTY = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            MESStationSession SessionPassQty = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            MESStationSession SessionFailQty = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            MESStationSession SessionLotNewFlag = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            MESStationSession SessionLotNo = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);

            if (SessionSN == null)
            {
                //throw new Exception("请输入SN!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN ObjSN = (SN)SessionSN.Value;

            //if (SessionSKU == null)
            //{
            //    SessionSKU = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
            //    Station.StationSession.Add(SessionSKU);
            //}

            if (SessionAQLTYPE == null)
            {
                SessionAQLTYPE = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionAQLTYPE);
            }

            if (SessionLotQTY == null)
            {
                SessionLotQTY = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionLotQTY);
            }

            if (SessionSAMPLEQTY == null)
            {
                SessionSAMPLEQTY = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionSAMPLEQTY);
            }

            if (SessionREJECTQTY == null)
            {
                SessionREJECTQTY = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionREJECTQTY);
            }

            if (SessionPassQty == null)
            {
                SessionPassQty = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionPassQty);
            }

            if (SessionFailQty == null)
            {
                SessionFailQty = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionFailQty);
            }

            if (SessionLotNewFlag == null)
            {
                SessionLotNewFlag = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionLotNewFlag);
            }

            if (SessionLotNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[9].SESSION_TYPE + Paras[9].SESSION_KEY }));
            }

            try
            {
                if ((string)SessionLotNewFlag.Value == "1")
                {
                    if (Paras[1].VALUE.ToString() == "ALL")
                    {
                        RLotSku = TR.GetBySkuNo(Paras[1].VALUE.ToString(), ObjSN.NextStation, Station.SFCDB);
                        RAqltype = QR.GetByAqltype(RLotSku.AQL_TYPE, Station.SFCDB);
                        SessionAQLTYPE.Value = RAqltype.AQL_TYPE;
                        //Marked by LLF
                        //SessionLotQTY.Value = RAqltype.LOT_QTY;
                        //SessionSAMPLEQTY.Value = RAqltype.SAMPLE_QTY;
                        //SessionREJECTQTY.Value = RAqltype.REJECT_QTY;
                    }
                    else
                    {
                        RLotSku = TR.GetBySkuNo(ObjSN.SkuNo, ObjSN.NextStation, Station.SFCDB);
                        RAqltype = QR.GetByAqltype(RLotSku.AQL_TYPE, Station.SFCDB);
                        SessionAQLTYPE.Value = RAqltype.AQL_TYPE;
                        //Marked by LLF
                        //SessionLotQTY.Value = RAqltype.LOT_QTY;
                        //SessionSAMPLEQTY.Value = RAqltype.SAMPLE_QTY;
                        //SessionREJECTQTY.Value = RAqltype.REJECT_QTY;
                    }
                    SessionPassQty.Value = 0;
                    SessionFailQty.Value = 0;
                    SessionLotQTY.Value = 0;
                    SessionSAMPLEQTY.Value = 0;
                    SessionREJECTQTY.Value = 0;
                }
                else
                {
                    SessionAQLTYPE.Value = ((LotNo)SessionLotNo.Value).AQL_TYPE;
                    SessionLotQTY.Value = ((LotNo)SessionLotNo.Value).LOT_QTY;
                    SessionSAMPLEQTY.Value = ((LotNo)SessionLotNo.Value).SAMPLE_QTY;
                    SessionREJECTQTY.Value = ((LotNo)SessionLotNo.Value).REJECT_QTY;
                    SessionPassQty.Value = ((LotNo)SessionLotNo.Value).PASS_QTY;
                    SessionFailQty.Value = ((LotNo)SessionLotNo.Value).FAIL_QTY;
                }

                //Station.AddMessage("MES00000104", new string[] { "AQLTYPE", RAqltype.AQL_TYPE, "LotQTY", RAqltype.LOT_QTY.ToString(), "SAMPLEQTY", RAqltype.SAMPLE_QTY.ToString(), "REJECTQTY", RAqltype.REJECT_QTY.ToString() }, MESReturnView.Station.StationMessageState.Message);
                Station.AddMessage("MES00000104", new string[] { "AQLTYPE", SessionAQLTYPE.Value.ToString(), "LotQTY", SessionLotQTY.Value.ToString(), "SAMPLEQTY", SessionSAMPLEQTY.Value.ToString(), "REJECTQTY", SessionREJECTQTY.Value.ToString() }, MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }
        }

        public static void SampleLotDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string StrSN = "";
            Row_R_LOT_STATUS RLotStatus;
            T_R_LOT_STATUS TR = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            LotNo LOT = new LotNo();
            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession LotNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                StrSN = SNSession.Value.ToString();
            }

            if (LotNoSession == null)
            {
                LotNoSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(LotNoSession);
            }
            try
            {
                RLotStatus = TR.GetSampleLotBySN(StrSN, Station.SFCDB);
                if (RLotStatus != null)
                {
                    LotNoSession.Value = RLotStatus.GetDataObject();
                }
                else
                {
                    //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000159", new string[] { }));
                }
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }
        }

        /// <summary>
        /// 根據當前工站生產新的LOTNO
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetLotDataloaderNew(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession sessionLotNo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            MESStationSession sessionLotNewFlag = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            LotNo lot = new LotNo();
            
            string lotno = "";
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            if (sessionLotNewFlag == null)
            {
                sessionLotNewFlag = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionLotNewFlag);
            }

            try
            {
                if (sessionLotNo == null)
                {
                    T_R_LOT_STATUS tLotStatus = new T_R_LOT_STATUS(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    Row_R_LOT_STATUS rowLotStatus = tLotStatus.GetLotBySNNotCloesd(sessionSN.Value.ToString(), Station.SFCDB);
                    if (rowLotStatus != null)
                    {
                        sessionLotNewFlag.Value = "0";
                        lot.Init(rowLotStatus.LOT_NO, "", Station.SFCDB);
                        sessionLotNo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                        sessionLotNo.Value = lot;
                        Station.StationSession.Add(sessionLotNo);
                    }
                    else
                    {
                        //新建lot號                   
                        sessionLotNo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                        sessionLotNo.Value = lot.GetNewLotNo("HWD_FQCLOT", Station.SFCDB);
                        sessionLotNewFlag.Value = "1";
                        Station.StationSession.Add(sessionLotNo);
                    }
                }
                else
                {
                    if (sessionLotNo.Value is string && sessionLotNo.Value.ToString() != "")
                    {
                        lotno = sessionLotNo.Value.ToString();                       
                    }
                    else if (sessionLotNo.Value is LotNo)
                    {  
                        lotno = ((LotNo)sessionLotNo.Value).LOT_NO.ToString();
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }

                    try
                    {
                        lot.Init(lotno, "", Station.SFCDB);
                    }
                    catch
                    {
                        lot = null;
                    }

                    if (lot != null)
                    {
                        if (lot.CLOSED_FLAG.ToString() == "1")
                        {
                            sessionLotNo.Value = lot.GetNewLotNo("HWD_FQCLOT", Station.SFCDB);
                            sessionLotNewFlag.Value = "1";
                            Station.StationSession.Add(sessionLotNo);
                        }
                        else
                        {
                            sessionLotNewFlag.Value = "0";
                            lot.Init(((LotNo)lot).LOT_NO, "", Station.SFCDB);
                            sessionLotNo.Value = lot;
                        }
                    }                    
                }              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
