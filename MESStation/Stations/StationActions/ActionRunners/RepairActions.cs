using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Module;
using MESStation.BaseClass;
using MESStation.LogicObject;
using MESStation.MESReturnView.Station;
using MESDBHelper;
using System.Data;
//using System.Transactions;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class RepairActions
    {
        //產品維修CheckIn Action
        public static void SNInRepairAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNLoadPoint == null)
            {
                SNLoadPoint = new MESStationSession() { MESDataType = "SN", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(SNLoadPoint);
            }

            string sendEmp = Station.Inputs.Find(s => s.DisplayName == "SendEMP").Value.ToString();
            string receiveEmp = Station.Inputs.Where(s => s.DisplayName == "ReceiveEMP").FirstOrDefault().Value.ToString();
            //string sendEmp = Station.Inputs[0].Value.ToString();
            //string receiveEmp = Station.Inputs[1].Value.ToString();
            string strSn = Input.Value.ToString();
            SN sn = new SN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_REPAIR_TRANSFER rTransfer = new T_R_REPAIR_TRANSFER(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_TRANSFER rowTransfer = (Row_R_REPAIR_TRANSFER)rTransfer.NewRow();
            T_R_REPAIR_MAIN rRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_MAIN> RepariMainList = rRepairMain.GetRepairMainBySN(Station.SFCDB, strSn);
            R_REPAIR_MAIN rMain = RepariMainList.Where(r => r.CLOSED_FLAG == "0").FirstOrDefault();  // Find(r => r.CLOSED_FLAG == "0");
            if (rMain != null)
            {
                rowTransfer.ID = rTransfer.GetNewID(Station.BU, Station.SFCDB);
                rowTransfer.REPAIR_MAIN_ID = rMain.ID;
                rowTransfer.IN_SEND_EMP = sendEmp;
                rowTransfer.IN_RECEIVE_EMP = receiveEmp;
                rowTransfer.IN_TIME = DateTime.Now;
                rowTransfer.SN = strSn;
                rowTransfer.LINE_NAME = Station.Line;
                rowTransfer.STATION_NAME = sn.CurrentStation;
                rowTransfer.WORKORDERNO = sn.WorkorderNo;
                rowTransfer.SKUNO = sn.SkuNo;
                rowTransfer.CLOSED_FLAG = "1";
                string strRet = (Station.SFCDB).ExecSQL(rowTransfer.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000037", new string[] { "INSET R_REPAIR_TRANSFER" }, MESReturnView.Station.StationMessageState.Pass);
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000066", new string[] { strSn, "CLOSED" }));
            }
        }

        //產品維修CheckOut Action
        public static void SNOutRepairAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNLoadPoint == null)
            {
                SNLoadPoint = new MESStationSession() { MESDataType = "SN", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(SNLoadPoint);
            }

            string checkOutSendEmp = Station.Inputs.Find(s => s.DisplayName == "SendEMP").Value.ToString();
            string checkOutReceiveEmp = Station.Inputs.Find(s => s.DisplayName == "ReceiveEMP").Value.ToString();
            string strSn = Input.Value.ToString();
            SN sn = new SN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_REPAIR_TRANSFER rTransfer = new T_R_REPAIR_TRANSFER(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_TRANSFER rowTransfer = (Row_R_REPAIR_TRANSFER)rTransfer.NewRow();

            List<R_REPAIR_TRANSFER> transferList = rTransfer.GetReSNbysn(strSn, Station.SFCDB);
            R_REPAIR_TRANSFER rRepairTransfer = transferList.Where(r => r.CLOSED_FLAG == "1").FirstOrDefault();//TRANSFER表 1 表示不良

            if (rRepairTransfer != null)
            {
                rowTransfer = (Row_R_REPAIR_TRANSFER)rTransfer.GetObjByID(rRepairTransfer.ID, Station.SFCDB);

                //T_R_REPAIR_MAIN rRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                //List<R_REPAIR_MAIN> RepariMainList = rRepairMain.GetRepairMainBySN(Station.SFCDB, strSn);
                //R_REPAIR_MAIN rMain = RepariMainList.Where(r => r.CLOSED_FLAG == "0").FirstOrDefault();

                rowTransfer.OUT_TIME = DateTime.Now;
                rowTransfer.OUT_SEND_EMP = checkOutSendEmp;
                rowTransfer.OUT_RECEIVE_EMP = checkOutReceiveEmp;

                string strRet = (Station.SFCDB).ExecSQL(rowTransfer.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000037", new string[] { "UPDATE R_REPAIR_TRANSFER" }, MESReturnView.Station.StationMessageState.Pass);
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000066", new string[] { strSn, "abnormal" }));
            }
        }

        public static void SNFailAction_Old(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string R_SN_STATION_DETAIL_ID = "";
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNLoadPoint == null)
            {
                SNLoadPoint = new MESStationSession() { MESDataType = "SN", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(SNLoadPoint);
            }

            //獲取頁面傳過來的數據
            string failCode = Station.Inputs.Find(s => s.DisplayName == "Fail_Code").Value.ToString();
            string failLocation = Station.Inputs.Find(s => s.DisplayName == "Fail_Location").Value.ToString();
            string failProcess = Station.Inputs.Find(s => s.DisplayName == "Fail_Process").Value.ToString();
            string failDescription = Station.Inputs.Find(s => s.DisplayName == "Description").Value.ToString();
            string strSn = Input.Value.ToString();

            OleExec oleDB = null;
            oleDB = Station.SFCDB;
            //oleDB = this.DBPools["SFCDB"].Borrow();
            oleDB.BeginTrain();  //以下執行 要么全成功，要么全失敗

            //更新R_SN REPAIR_FAILED_FLAG=’1’
            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_SN rrSn = (Row_R_SN)rSn.NewRow();
            R_SN r = rSn.GetDetailBySN(strSn, Station.SFCDB);
            rrSn = (Row_R_SN)rSn.GetObjByID(r.ID, Station.SFCDB);
            rrSn.REPAIR_FAILED_FLAG = "1";
            string strRet = (Station.SFCDB).ExecSQL(rrSn.GetUpdateString(DB_TYPE_ENUM.Oracle));
            if (!(Convert.ToInt32(strRet) > 0))
            {
                throw new Exception("update repair failed flag error!");
            }

            //新增一筆FAIL記錄到R_SN_STATION_DETAIL
            T_R_SN_STATION_DETAIL rSnStationDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_SN_STATION_DETAIL_ID = rSnStationDetail.GetNewID(Station.BU, Station.SFCDB);
            string detailResult = rSnStationDetail.AddDetailToRSnStationDetail(R_SN_STATION_DETAIL_ID,rrSn.GetDataObject(),Station.Line,Station.StationName,Station.StationName, Station.SFCDB);
            if (!(Convert.ToInt32(detailResult) > 0))
            {
                throw new Exception("Insert sn station detail error!");
            }

            //新增一筆到R_REPAIR_MAIN
            T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN)tRepairMain.NewRow();
            rRepairMain.ID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
            rRepairMain.SN = strSn;
            rRepairMain.WORKORDERNO = rrSn.WORKORDERNO;
            rRepairMain.SKUNO = rrSn.SKUNO;
            rRepairMain.FAIL_LINE = Station.Line;
            rRepairMain.FAIL_STATION = Station.StationName;
            rRepairMain.FAIL_EMP = Station.User.EMP_NO;
            rRepairMain.FAIL_TIME = Station.GetDBDateTime();
            rRepairMain.CLOSED_FLAG = "0";
            string insertResult = (Station.SFCDB).ExecSQL(rRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
            if (!(Convert.ToInt32(insertResult) > 0))
            {
                throw new Exception("Insert repair main error!");
            }

            //新增一筆到R_REPAIR_FAILCODE
            T_R_REPAIR_FAILCODE tRepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_REPAIR_FAILCODE rRepairFailCode = (Row_R_REPAIR_FAILCODE)tRepairFailCode.NewRow();
            rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
            rRepairFailCode.REPAIR_MAIN_ID = rRepairMain.ID;
            rRepairFailCode.SN = strSn;
            rRepairFailCode.FAIL_CODE = failCode;
            rRepairFailCode.FAIL_EMP = Station.User.EMP_NO;
            rRepairFailCode.FAIL_TIME = DateTime.Now;
            rRepairFailCode.FAIL_CATEGORY = "SYMPTON";
            rRepairFailCode.FAIL_LOCATION = failLocation;
            rRepairFailCode.FAIL_PROCESS = failProcess;
            rRepairFailCode.DESCRIPTION = failDescription;
            rRepairFailCode.REPAIR_FLAG = "0";
            string strResult = (Station.SFCDB).ExecSQL(rRepairFailCode.GetInsertString(DB_TYPE_ENUM.Oracle));
            if (!(Convert.ToInt32(strResult) > 0))
            {
                throw new Exception("Insert repair failcode error!");
            }

            oleDB.CommitTrain();

            Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
        }

        public static void SNFailAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            Int16 FailCount = 0;
            string StrSn = "";
            string R_SN_STATION_DETAIL_ID = "";
            List<Dictionary<string, string>> FailList = null;

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailCountSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCountSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (FailListSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[3].VALUE, SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
          //      Station.StationSession.Add(StatusSession);
                if (StatusSession.Value==null)
                {
                    StatusSession.Value = "FAIL";
                }
                Station.StationSession.Add(StatusSession);
            }

            FailCount = Convert.ToInt16(FailCountSession.Value.ToString());
            FailList = (List<Dictionary<string, string>>)FailListSession.Value;

            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            DateTime FailTime = rSn.GetDBDateTime(Station.SFCDB);
            if (FailList.Count >= FailCount && FailCount != 0) //允許掃描多個Fail
            {
                StrSn = SNSession.InputValue.ToString();
                string repairMainId = "";
                for (int i = 0; i < FailList.Count; i++)
                {
                    //獲取頁面傳過來的數據
                    string failCode = FailList[i]["FailCode"].ToString();
                    string failLocation = FailList[i]["FailLocation"].ToString();
                    string failProcess = FailList[i]["FailProcess"].ToString();
                    string failDescription = FailList[i]["FailDesc"].ToString();

                    OleExec oleDB = null;
                    oleDB = Station.SFCDB;
                    //oleDB.BeginTrain();  //以下執行 要么全成功，要么全失敗
                    T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN)tRepairMain.NewRow();

                    Row_R_SN rrSn = (Row_R_SN)rSn.NewRow();
                    if (i == 0)
                    {
                        //更新R_SN REPAIR_FAILED_FLAG=’1’
                        R_SN r = rSn.GetDetailBySN(StrSn, Station.SFCDB);
                        rrSn = (Row_R_SN)rSn.GetObjByID(r.ID, Station.SFCDB);
                        rrSn.REPAIR_FAILED_FLAG = "1";
                        rrSn.EDIT_EMP= Station.LoginUser.EMP_NO;
                        rrSn.EDIT_TIME= FailTime;
                        string strRet = (Station.SFCDB).ExecSQL(rrSn.GetUpdateString(DB_TYPE_ENUM.Oracle));
                        if (!(Convert.ToInt32(strRet) > 0))
                        {
                            throw new Exception("update repair failed flag error!");
                        }

                        //新增一筆FAIL記錄到R_SN_STATION_DETAIL
                        T_R_SN_STATION_DETAIL rSnStationDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                        R_SN_STATION_DETAIL_ID = rSnStationDetail.GetNewID(Station.BU, Station.SFCDB);
                        string detailResult = rSnStationDetail.AddDetailToRSnStationDetail(R_SN_STATION_DETAIL_ID,rrSn.GetDataObject(),Station.Line,Station.StationName,Station.StationName,Station.SFCDB);
                        if (!(Convert.ToInt32(detailResult) > 0))
                        {
                            throw new Exception("Insert sn station detail error!");
                        }

                        //新增一筆到R_REPAIR_MAIN 
                        repairMainId = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                        rRepairMain.ID = repairMainId;
                        rRepairMain.SN = StrSn;
                        rRepairMain.WORKORDERNO = rrSn.WORKORDERNO;
                        rRepairMain.SKUNO = rrSn.SKUNO;
                        rRepairMain.FAIL_LINE = Station.Line;
                        rRepairMain.FAIL_STATION = Station.StationName;
                        rRepairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
                        //rRepairMain.FAIL_TIME = Station.GetDBDateTime();//Mpdofy by LLF 2018-03-17
                        rRepairMain.FAIL_TIME = FailTime;
                        rRepairMain.CREATE_TIME = Station.GetDBDateTime();
                        rRepairMain.EDIT_EMP= Station.LoginUser.EMP_NO;
                        rRepairMain.EDIT_TIME= Station.GetDBDateTime();
                        rRepairMain.CLOSED_FLAG = "0";
                        string insertResult = (Station.SFCDB).ExecSQL(rRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
                        if (!(Convert.ToInt32(insertResult) > 0))
                        {
                            throw new Exception("Insert repair main error!");
                        }
                    }

                    ////新增一筆到R_REPAIR_MAIN
                    //T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    //Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN)tRepairMain.NewRow();
                    //rRepairMain.ID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                    //rRepairMain.SN = StrSn;
                    //rRepairMain.WORKORDERNO = rrSn.WORKORDERNO;
                    //rRepairMain.SKUNO = rrSn.SKUNO;
                    //rRepairMain.FAIL_LINE = Station.Line;
                    //rRepairMain.FAIL_STATION = Station.StationName;
                    //rRepairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
                    //rRepairMain.FAIL_TIME = FailTime.ToString();
                    //rRepairMain.CLOSED_FLAG = "0";
                    //string insertResult = (Station.SFCDB).ExecSQL(rRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
                    //if (!(Convert.ToInt32(insertResult) > 0))
                    //{
                    //    throw new Exception("Insert repair main error!");
                    //}

                    //新增一筆到R_REPAIR_FAILCODE
                    T_R_REPAIR_FAILCODE tRepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    Row_R_REPAIR_FAILCODE rRepairFailCode = (Row_R_REPAIR_FAILCODE)tRepairFailCode.NewRow();
                    rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
                    rRepairFailCode.REPAIR_MAIN_ID = repairMainId;
                    rRepairFailCode.SN = StrSn;
                    rRepairFailCode.FAIL_CODE = failCode;
                    rRepairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
                    rRepairFailCode.FAIL_TIME = FailTime;
                    rRepairFailCode.FAIL_CATEGORY = "SYMPTOM";
                    rRepairFailCode.FAIL_LOCATION = failLocation;
                    rRepairFailCode.FAIL_PROCESS = failProcess;
                    rRepairFailCode.DESCRIPTION = failDescription;
                    rRepairFailCode.REPAIR_FLAG = "0";
                    rRepairFailCode.CREATE_TIME = Station.GetDBDateTime();
                    rRepairFailCode.EDIT_EMP= Station.LoginUser.EMP_NO;
                    rRepairFailCode.EDIT_TIME= Station.GetDBDateTime();

                    string strResult = (Station.SFCDB).ExecSQL(rRepairFailCode.GetInsertString(DB_TYPE_ENUM.Oracle));
                    if (!(Convert.ToInt32(strResult) > 0))
                    {
                        throw new Exception("Insert repair failcode error!");
                    }

                    //oleDB.CommitTrain();
                }
                ((List<Dictionary<string, string>>)FailListSession.Value).Clear();
                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
        }


        public static void HWDBIPSNFailAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string StrSn = "";
            string APVirtualSn = "";
            string VirtualSn = "";
            AP_DLL APObj = new AP_DLL();
            OleExec APDB = null;
            R_PANEL_SN PANELObj = null;
            Int16 FailCount = 0;
            List<Dictionary<string, string>> FailList = null;
            string R_SN_STATION_DETAIL_ID = "";
            StringBuilder ReturnMessage = new StringBuilder();
            string RepairmainID = "";//add by LLF 2018-04-12
        
            if (Paras.Count < 5)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailCountSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCountSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (FailListSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession PanelVitualSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (PanelVitualSNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[4].VALUE, SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StatusSession);
                if (StatusSession.Value.ToString() == "")
                {
                    StatusSession.Value = "FAIL";
                }
            }

            MESStationSession ClearInputSession = null;
            if (Paras.Count >= 6)
            {
                ClearInputSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[5].SESSION_TYPE) && t.SessionKey.Equals(Paras[5].SESSION_KEY));
                if (ClearInputSession == null)
                {
                    ClearInputSession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY };
                    Station.StationSession.Add(ClearInputSession);
                }
            }

            FailCount = Convert.ToInt16(FailCountSession.Value.ToString());
            FailList = (List<Dictionary<string, string>>)FailListSession.Value;

            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            DateTime FailTime = rSn.GetDBDateTime(Station.SFCDB);
            StrSn = SNSession.Value.ToString();
            if (FailList.Count >= FailCount && FailCount != 0) //允許掃描多個Fail
            {
                #region 業務邏輯

                OleExec oleDB = null;
                oleDB = Station.SFCDB;
                //oleDB = this.DBPools["SFCDB"].Borrow();
                //oleDB.BeginTrain(); 
                //OleExecPool APDBPool = Station.DBS["APDB"];
                //APDB = APDBPool.Borrow();
                //APDB.BeginTrain();
                APDB = Station.APDB;
                try
                {
                    Row_R_SN rrSn = (Row_R_SN) rSn.NewRow();
                    for (int i = 0; i < FailList.Count; i++)
                    {
                        //獲取頁面傳過來的數據
                        string failCode = FailList[i]["FailCode"].ToString();
                        string failLocation = FailList[i]["FailLocation"].ToString();
                        string failProcess = FailList[i]["FailProcess"].ToString();
                        string failDescription = FailList[i]["FailDesc"].ToString();



                        //黃楊盛 2018年4月24日14:14:28 模擬自動做維修的動作,修正時間
                        //更新R_SN REPAIR_FAILED_FLAG=’1’
                        //modify by ZGJ 2018-03-22 BIP Fail 的產品自動清除待維修狀態，但是記錄不良

                        PANELObj = (R_PANEL_SN) PanelVitualSNSession.Value;
                        if (i == 0)
                        {
                            VirtualSn = PANELObj.SN.ToString();
                            R_SN r = rSn.GetDetailBySN(VirtualSn, Station.SFCDB);
                            rrSn = (Row_R_SN) rSn.GetObjByID(r.ID, Station.SFCDB);
                            //rrSn.REPAIR_FAILED_FLAG = "0";
                            rrSn.REPAIR_FAILED_FLAG = "1";
                            rrSn.SN = StrSn;
                            rrSn.EDIT_EMP = Station.LoginUser.EMP_NO; //add by LLF 2018-03-17
                            rrSn.EDIT_TIME = FailTime; //add by LLF 2018-03-17
                            string strRet = (Station.SFCDB).ExecSQL(rrSn.GetUpdateString(DB_TYPE_ENUM.Oracle));
                            if (!(Convert.ToInt32(strRet) > 0))
                            {
                                throw new MESReturnMessage("update repair failed flag error!");
                            }

                            // Update R_PANEL_SN
                            T_R_PANEL_SN RPanelSN = new T_R_PANEL_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            Row_R_PANEL_SN Row_Panel = (Row_R_PANEL_SN) RPanelSN.NewRow();
                            Row_Panel = (Row_R_PANEL_SN) RPanelSN.GetObjByID(PANELObj.ID, Station.SFCDB);
                            Row_Panel.SN = StrSn;
                            strRet = (Station.SFCDB).ExecSQL(Row_Panel.GetUpdateString(DB_TYPE_ENUM.Oracle));
                            if (!(Convert.ToInt32(strRet) > 0))
                            {
                                throw new MESReturnMessage("update r_panel_sn error!");
                            }

                            //Update AP 

                            //黄杨盛 2018年4月14日09:10:50 修正不能超过9连板的情况.同时加上不支持3位数连板的约束
                            //APVirtualSn = PANELObj.PANEL + "0" + PANELObj.SEQ_NO.ToString();
                            if (PANELObj.SEQ_NO > 99)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000226",
                                    new string[] {DB_TYPE_ENUM.Oracle.ToString()}));
                            }
                            APVirtualSn = PANELObj.PANEL + Convert.ToInt16(PANELObj.SEQ_NO).ToString("00");


                            string result = APObj.APUpdatePanlSN(APVirtualSn, StrSn, APDB);
                            //APDBPool.Return(APDB);
                            if (!result.Equals("OK"))
                            {
                                //2018年4月24日13:56:14 黃楊盛 
                                //throw new MESReturnMessage("already be binded to other serial number");
                                throw new MESReturnMessage(result);
                            }


                            //新增一筆FAIL記錄到R_SN_STATION_DETAIL
                            T_R_SN_STATION_DETAIL rSnStationDetail =
                                new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            //Add by LLF 2018-03-19
                            R_SN_STATION_DETAIL_ID = rSnStationDetail.GetNewID(Station.BU, Station.SFCDB);
                            //string detailResult = rSnStationDetail.AddDetailToRSnStationDetail(R_SN_STATION_DETAIL_ID,rrSn.GetDataObject(), Station.Line, Station.StationName, Station.StationName, Station.SFCDB);
                            string detailResult = rSnStationDetail.AddDetailToBipStationFailDetail(
                                R_SN_STATION_DETAIL_ID, rrSn.GetDataObject(), Station.Line, Station.StationName,
                                Station.StationName, Station.SFCDB, "1");
                            if (!(Convert.ToInt32(detailResult) > 0))
                            {
                                throw new MESReturnMessage("Insert sn station detail error!");
                            }

                            //update R_SN_STATION_DETAIL 
                            rSnStationDetail.UpdateRSnStationDetailBySNID(StrSn, PANELObj.SN, Station.SFCDB);

                            //新增一筆到R_REPAIR_MAIN
                            T_R_REPAIR_MAIN tRepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            Row_R_REPAIR_MAIN rRepairMain = (Row_R_REPAIR_MAIN) tRepairMain.NewRow();

                            RepairmainID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                            //rRepairMain.ID = tRepairMain.GetNewID(Station.BU, Station.SFCDB);
                            rRepairMain.ID = RepairmainID;
                            rRepairMain.SN = StrSn;
                            rRepairMain.WORKORDERNO = rrSn.WORKORDERNO;
                            rRepairMain.SKUNO = rrSn.SKUNO;
                            rRepairMain.FAIL_LINE = Station.Line;
                            rRepairMain.FAIL_STATION = Station.StationName;
                            rRepairMain.FAIL_EMP = Station.LoginUser.EMP_NO;
                            //rRepairMain.FAIL_TIME = Station.GetDBDateTime();//Modify by LLF 2018-03-17
                            rRepairMain.FAIL_TIME = FailTime; //Modify by LLF 2018-03-17
                            rRepairMain.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rRepairMain.EDIT_TIME = Station.GetDBDateTime();
                            rRepairMain.CLOSED_FLAG = "1";
                            string insertResult =
                                (Station.SFCDB).ExecSQL(rRepairMain.GetInsertString(DB_TYPE_ENUM.Oracle));
                            if (!(Convert.ToInt32(insertResult) > 0))
                            {
                                throw new Exception("Insert repair main error!");
                            }
                        }

                        //新增一筆到R_REPAIR_FAILCODE
                        T_R_REPAIR_FAILCODE tRepairFailCode =
                            new T_R_REPAIR_FAILCODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                        Row_R_REPAIR_FAILCODE rRepairFailCode = (Row_R_REPAIR_FAILCODE) tRepairFailCode.NewRow();
                        rRepairFailCode.ID = tRepairFailCode.GetNewID(Station.BU, Station.SFCDB);
                        //rRepairFailCode.REPAIR_MAIN_ID = rRepairMain.ID; //Modify by LLF 2018-04-11 多筆FAIL 取1一個RepairMainID
                        rRepairFailCode.REPAIR_MAIN_ID = RepairmainID;
                        rRepairFailCode.SN = StrSn;
                        rRepairFailCode.FAIL_CODE = failCode;
                        rRepairFailCode.FAIL_EMP = Station.LoginUser.EMP_NO;
                        rRepairFailCode.FAIL_TIME = FailTime;
                        rRepairFailCode.FAIL_CATEGORY = "SYMPTOM";
                        rRepairFailCode.FAIL_LOCATION = failLocation;
                        rRepairFailCode.FAIL_PROCESS = failProcess;
                        rRepairFailCode.DESCRIPTION = failDescription;
                        rRepairFailCode.REPAIR_FLAG = "1";
                        rRepairFailCode.EDIT_EMP = Station.LoginUser.EMP_NO;
                        rRepairFailCode.EDIT_TIME = Station.GetDBDateTime();

                        string strResult =
                            (Station.SFCDB).ExecSQL(rRepairFailCode.GetInsertString(DB_TYPE_ENUM.Oracle));
                        if (!(Convert.ToInt32(strResult) > 0))
                        {
                            throw new MESReturnMessage("Insert repair failcode error!");
                        }


                        //oleDB.CommitTrain();

                        ReturnMessage.Append(failDescription).Append("|");
                    }


                    //黃楊盛 2018年4月24日14:11:42 做一筆出來的記錄
                    R_SN snOut = rSn.GetDetailBySN(StrSn, Station.SFCDB);
                    rrSn = (Row_R_SN) rSn.GetObjByID(snOut.ID, Station.SFCDB);
                    rrSn.CURRENT_STATION = rrSn.NEXT_STATION;
                    rrSn.NEXT_STATION = rSn.GetNextStation(snOut.ROUTE_ID, snOut.NEXT_STATION, oleDB);
                    rrSn.REPAIR_FAILED_FLAG = "0";
                    rrSn.SN = StrSn;
                    rrSn.EDIT_EMP = Station.LoginUser.EMP_NO;
                    rrSn.EDIT_TIME = FailTime;
                    var count = (Station.SFCDB).ExecSQL(rrSn.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    if (!(Convert.ToInt32(count) > 0))
                    {
                        throw new MESReturnMessage("update rsn failed flag error!");
                    }

                    // 方國剛 2018.05.02 11:45:30
                    // 因拋賬計算過站數量時，不計算REPAIR_FAILED_FLAG=1的數量，故BIP Fail 的產品自動清除待維修狀態后再在過站記錄表記錄一筆REPAIR_FAILED_FLAG=0的記錄
                    T_R_SN_STATION_DETAIL rSnStationDetailRepaired = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    R_SN_STATION_DETAIL_ID = rSnStationDetailRepaired.GetNewID(Station.BU, Station.SFCDB);                    
                    string detailResultRepaired = rSnStationDetailRepaired.AddDetailToBipStationFailDetail(
                        R_SN_STATION_DETAIL_ID, rrSn.GetDataObject(), Station.Line, Station.StationName,
                        Station.StationName, Station.SFCDB, "0");
                    if (!(Convert.ToInt32(detailResultRepaired) > 0))
                    {
                        throw new MESReturnMessage("Insert sn station detail error!");
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        //APDB.RollbackTrain();
                        //oleDB.RollbackTrain();
                    }
                    catch ( Exception x)
                    {
                        ;
                    }
                    
                    throw new MESReturnMessage(e.Message + e.StackTrace);
                }
                finally
                {
                    //APDBPool.Return(APDB);
                }
              

             
                #endregion
                //add by zgj 2018-03-14
                //當記錄完當前 SN 不良後，清除保存在 session 中的不良信息
                //((List<Dictionary<string, string>>)FailListSession.Value).Clear();

                if (ClearInputSession != null)
                {
                    ClearInputSession.Value = "true";
                }

                ReturnMessage.Remove(ReturnMessage.Length - 1, 1);
                Station.NextInput = Station.FindInputByName("PanelSn");
                Station.AddMessage("MES00000158", new string[] { StrSn, ReturnMessage.ToString() }, MESReturnView.Station.StationMessageState.Pass); 
            }
            else
            {
                Station.NextInput = Station.FindInputByName("Location");
                Station.AddMessage("MES00000162", new string[] { StrSn, FailCount.ToString(),FailList.Count.ToString() }, MESReturnView.Station.StationMessageState.Message);
            }
        }
    }
}
