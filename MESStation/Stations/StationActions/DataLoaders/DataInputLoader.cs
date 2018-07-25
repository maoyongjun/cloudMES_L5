using MESDataObject.Module;
using MESStation.BaseClass;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class DataInputLoader
    {

        /// <summary>
        /// 判斷是否為整數，若為整數保存到指定位置，否則提示報錯
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">
        /// IntSavePoint：保存到指定位置
        /// </param>
        public static void IntegerDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }

            string IntData = Input.Value.ToString();

            long Num;
            if (IntData == "")
            {
                Station.AddMessage("MES00000006", new string[] { "InputData" }, MESReturnView.Station.StationMessageState.Fail);
                //Station.NextInput = Station.Inputs[2];
            }
            else if (long.TryParse(IntData.ToString(), out Num) && long.Parse(IntData) >= 0)
            {
                s.Value = IntData;
                Station.AddMessage("MES00000029", new string[] { "InputData", Num.ToString() }, MESReturnView.Station.StationMessageState.Pass);
                //Station.NextInput = Station.Inputs[3];
            }
            else
            {
                Station.AddMessage("MES00000020", new string[] { "InputData", "Number" }, MESReturnView.Station.StationMessageState.Fail);
                //Station.NextInput = Station.Inputs[2];
            }
        }
        /// <summary>
        /// 加載輸入的字符串到指定的 MESStationSession
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">      
        /// </param>
        public static void InputDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[i].SESSION_TYPE && t.SessionKey == Paras[i].SESSION_KEY);
                if (s == null)
                {
                    s = new MESStationSession() { MESDataType = Paras[i].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[i].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(s);
                }
                s.Value = Input.Value.ToString();
                s.InputValue = Input.Value.ToString();
                s.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { Paras[i].SESSION_TYPE, Input.Value.ToString() }, MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 加載一個字符串特定格式到指定位置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">
        /// StrSavePoint:字符串保存位置；
        /// Change：0轉小寫,1轉大寫,2保持不變；
        /// Trim：0不變,1 去前空,2去后空,3前後去空；
        /// </param>
        public static void StringDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            for (int i = 0; i < Paras.Count; i++)
            {

                if (Paras[i].SESSION_TYPE == "Change")
                {
                    if (Paras[i].VALUE == "0")
                    {
                        Input.Value = Input.Value.ToString().ToLower();
                    }
                    else if (Paras[i].VALUE == "1")
                    {
                        Input.Value = Input.Value.ToString().ToUpper();
                    }
                    //else if (Paras[i].VALUE != "0"&& Paras[i].VALUE != "1"&& Paras[i].VALUE != "2")
                    //{
                    //    Station.AddMessage("MES00000020", new string[] { "SESSION_TYPE为\"Change\"的", "0,1,2" }, MESReturnView.Station.StationMessageState.Fail);
                    //}
                }
                else if (Paras[i].SESSION_TYPE == "Trim")
                {
                    if (Paras[i].VALUE == "1")
                    {
                        Input.Value = Input.Value.ToString().TrimStart();
                    }
                    else if (Paras[i].VALUE == "2")
                    {
                        Input.Value = Input.Value.ToString().TrimEnd();
                    }
                    else if (Paras[i].VALUE == "3")
                    {
                        Input.Value = Input.Value.ToString().Trim();
                    }
                    //else if (Paras[i].VALUE != "0" && Paras[i].VALUE != "1" && Paras[i].VALUE != "2" && Paras[i].VALUE != "3")
                    //{
                    //    Station.AddMessage("MES00000020", new string[] { "SESSION_TYPE为\"Trim\"的", "0,1,2,3" }, MESReturnView.Station.StationMessageState.Fail);
                    //}
                }

            }
            for (int j = 0; j < Paras.Count; j++)
            {
                if (Paras[j].SESSION_TYPE != "Change" && Paras[j].SESSION_TYPE != "Trim")
                {
                    MESStationSession StrInput = Station.StationSession.Find(t => t.MESDataType == Paras[j].SESSION_TYPE && t.SessionKey == Paras[j].SESSION_KEY);
                    if (StrInput == null)
                    {
                        StrInput = new MESStationSession() { MESDataType = Paras[j].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[j].SESSION_KEY, ResetInput = Input };
                        Station.StationSession.Add(StrInput);
                    }
                    StrInput.InputValue = Input.Value.ToString();
                    StrInput.ResetInput = Input;
                    StrInput.Value = Input.Value.ToString();
                    //Station.NextInput = Station.Inputs[0];
                    break;
                }
            }
        }



        /// <summary>
        /// 從輸入加載ActionCode
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">輸入的ActionCode轉換為大寫</param>
        /// <param name="Paras">ActionCode</param>
        public static void ActionCodeDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string ActionCodeInput = "";//Station.StationSession[0].Value.ToString();
            MESStationSession strInput = Station.StationSession.Find(t => t.MESDataType == "StrSavePoint" && t.SessionKey == "1");
            if (strInput == null)
            {
                //Station.AddMessage("MES00000076", new string[] { "Sn", Sn }, MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                ActionCodeInput = strInput.Value.ToString();
            }

            string strSql = $@"SELECT * FROM C_ACTION_CODE WHERE ACTION_CODE = '{ActionCodeInput.Replace("'", "''")}'";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":ActionCode", ActionCodeInput) };
            DataTable res = Station.SFCDB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count <= 0)
            {
                Station.NextInput = Input;
                Station.AddMessage("MES00000007", new string[] { "ActionCode", ActionCodeInput }, MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                MESStationSession ActionCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (ActionCode == null)
                {
                    ActionCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(ActionCode);
                }
                ActionCode.Value = ActionCodeInput;
            }

        }
        /// <summary>
        /// 從輸入加載RootCause 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RootCauseDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string RootCauseInput = Input.Value.ToString();// = Station.StationSession[0].Value.ToString();
            //Modify by LLF 2018-02-03
            //MESStationSession strInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (strInput == null)
            //{
            //    //Station.AddMessage("MES00000076", new string[] { "Sn", Sn }, MESReturnView.Station.StationMessageState.Fail);
            //}
            //else
            //{
            //    RootCauseInput = strInput.Value.ToString();
            //}
            //string strSql = $@"SELECT * FROM C_ERROR_CODE WHERE ERROR_CODE = '{RootCauseInput.Replace("'", "''")}'";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":RootCause", RootCauseInput) };
            //DataTable res = Station.SFCDB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            MESStationSession ErrorCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (ErrorCode == null)
            {
                ErrorCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(ErrorCode);
            }

            T_C_ERROR_CODE Obj_C_ERROR_CODE = new T_C_ERROR_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_ERROR_CODE ObjErrorCode = Obj_C_ERROR_CODE.GetByErrorCode(RootCauseInput, Station.SFCDB);

            if (ObjErrorCode == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000142", new string[] { RootCauseInput }));
            }
            else
            {
                ErrorCode.Value = RootCauseInput;
                Station.Inputs[Station.Inputs.Count-1].Value = ObjErrorCode.ENGLISH_DESCRIPTION.ToString();
            }
            //Modify by LLF 2018-02-03
            //if (res.Rows.Count <= 0)
            //{
            //    Station.NextInput = Input;
            //    Station.AddMessage("MES00000007", new string[] { "RootCause", RootCauseInput }, MESReturnView.Station.StationMessageState.Fail);
            //}
            //else
            //{
            //    MESStationSession ErrorCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //    if (ErrorCode == null)
            //    {
            //        ErrorCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
            //        Station.StationSession.Add(ErrorCode);
            //    }
            //    ErrorCode.Value = RootCauseInput;
            //}

        }

        /// <summary>
        /// 從輸入加載維修大項 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RepairItemsDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession RepairItemsSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (RepairItemsSession == null)
            {
                RepairItemsSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(RepairItemsSession);
            }
            RepairItemsSession.Value = Input.Value.ToString();
            //  Input.DataForUse.Add(Input.Value.ToString());
        }

        /// <summary>
        /// 從輸入加載維修小項 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RepairItemsSonDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession RepairItemsSonSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (RepairItemsSonSession == null)
            {
                RepairItemsSonSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(RepairItemsSonSession);
            }
            RepairItemsSonSession.Value = Input.Value.ToString();

        }

        /// <summary>
        /// 從維修大項加載維修小項 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RepairItemsSonFromRepairItemsDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            T_C_REPAIR_ITEMS RepairItems = new T_C_REPAIR_ITEMS(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS RowItems;
            T_C_REPAIR_ITEMS_SON RepairItemsSon = new T_C_REPAIR_ITEMS_SON(Station.SFCDB, Station.DBType);
            List<string> RepairItemsSonList = new List<string>();
            List<string> RepairItemsList = new List<string>();
            T_C_REPAIR_ITEMS TC_REPAIR_ITEM = new T_C_REPAIR_ITEMS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string ITEM_NAME = Input.Value.ToString();

            MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "Son_Items");
            List<object> ret = I.DataForUse;
            ret.Clear();

            try
            {

                //RepairItemsList = TC_REPAIR_ITEM.GetRepairItemsList(ITEM_NAME, Station.SFCDB);
                //Input.DataForUse.Add(RepairItemsList);//初始化維修大項
                RowItems = RepairItems.GetIDByItemName(ITEM_NAME, Station.SFCDB);

                RepairItemsSonList = RepairItemsSon.GetRepairItemsSonList(RowItems.ID, Station.SFCDB);
                //ret.Add(RepairItemsSonList);
                //添加維修小項
                foreach (object item in RepairItemsSonList)
                {
                    ret.Add(item);
                }


                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }
        }

        /// <summary>
        /// 初始化工站時加載出默認的維修大項，維修小項LIST
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RepairItemsInitDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            T_C_REPAIR_ITEMS RepairItems = new T_C_REPAIR_ITEMS(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS RowItems;
            T_C_REPAIR_ITEMS_SON RepairItemsSon = new T_C_REPAIR_ITEMS_SON(Station.SFCDB, Station.DBType);
            List<string> RepairItemsSonList = new List<string>();
            List<string> RepairItemsList = new List<string>();
            T_C_REPAIR_ITEMS TC_REPAIR_ITEM = new T_C_REPAIR_ITEMS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string ITEM_NAME = Input.Value.ToString();

            MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "Son_Items");

            try
            {
                Input.DataForUse.Clear();
                //RepairItemsList = TC_REPAIR_ITEM.GetRepairItemsList(ITEM_NAME, Station.SFCDB);
                //Input.DataForUse.Add(RepairItemsList);//初始化維修大項
                RowItems = RepairItems.GetIDByItemName(ITEM_NAME, Station.SFCDB);

                RepairItemsSonList = RepairItemsSon.GetRepairItemsSonList(RowItems.ID, Station.SFCDB);
                Input.DataForUse.Add(RepairItemsSonList);   //初始化維修小項

                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }

        }

        /// <summary>
        /// 從輸入加載FailDesc
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void FailDescDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession FailDescSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (FailDescSession == null)
            {
                FailDescSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FailDescSession);
            }
            FailDescSession.Value = Input.Value.ToString();
        }

        /// <summary>
        /// 加載FailList
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFailCollectDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string StrSn = "";
            string StrLocation = "";
            string StrProcess = "";
            string StrFailCode = "";
            string StrFailDesc = "";
            Int16 FailCount = 0;
            List<Dictionary<string, string>> FailList = new List<Dictionary<string, string>>();
            Dictionary<string, string> FailInfo = null;
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            //獲取Fail SN
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //StrSn = SNSession.Value.ToString();
            StrSn = SNSession.InputValue.ToString();

            //獲取Fail Location
            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LocationSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrLocation = LocationSession.Value.ToString();

            //獲取Fail Process
            MESStationSession ProcessSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (ProcessSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrProcess = ProcessSession.Value.ToString();

            //獲取FailCode
            MESStationSession FailCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (FailCodeSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrFailCode = FailCodeSession.Value.ToString();

            //獲取Fail Description
            MESStationSession FailDescSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (FailDescSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrFailDesc = FailDescSession.Value.ToString();

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (FailListSession == null)
            {
                FailListSession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FailListSession);
            }
            else
            {
                FailList = (List<Dictionary<string, string>>)FailListSession.Value;
            }

            FailInfo = new Dictionary<string, string>();
            //FailList=new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> failure in FailList)
            {

            }

            //            FailList.Select((dic) => {
            //                dic["FailProcess"].Equals(StrProcess) &&
            //dic["FailLocation"].Equals(StrLocation) && dic["FailCode"].Equals(StrFailCode); });   

            //add by ZGJ 2018-03-15
            //檢查當前不良信息是否已經存在於已輸入中
            Dictionary<string,string> ExistFailInfo = FailList.Find((dic) => {
                return (dic["FailProcess"].Equals(StrProcess)
                    && dic["FailLocation"].Equals(StrLocation)
                    && dic["FailCode"].Equals(StrFailCode));
                    });

            if (ExistFailInfo==null)
            {
                FailInfo.Add("FailLocation", StrLocation);
                FailInfo.Add("FailProcess", StrProcess);
                FailInfo.Add("FailDesc", StrFailDesc);
                FailInfo.Add("FailCode", StrFailCode);
                FailList.Add(FailInfo);
                FailListSession.Value = FailList;
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000163", new string[] { StrProcess,StrLocation,StrFailCode });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        ///Add by LLF 2018-01-28 AOI1&AOI2,Print1&Print2,VI1&VI2  工站可以相互轉換
        public static void ChangeCurrentStationDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string StrStation = Paras[0].VALUE.ToString();
            string StrChangeStation = Paras[1].VALUE.ToString();

            MESStationSession NextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (NextStationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            List<string> ListNextStation = new List<string>();
            ListNextStation = (List<string>)NextStationSession.Value;

            if (Station.StationName == StrStation && Station.StationName != StrChangeStation && ListNextStation.Contains(StrChangeStation))
            {
                Station.StationName = StrChangeStation;
            }
            else if (Station.StationName == StrChangeStation && Station.StationName != StrStation && ListNextStation.Contains(StrStation))
            {
                Station.StationName = StrStation;
            }

            Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
        }

        public static void SNSampleFailInfoDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string StrSn = "";
            string StrLocation = "";
            string StrFailCode = "";
            string StrFailDesc = "";
            string[] FailInfo = new string[3];
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            //獲取Fail SN
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrSn = SNSession.Value.ToString();

            //獲取FailCode
            MESStationSession FailCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCodeSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrFailCode = FailCodeSession.Value.ToString();

            //獲取Fail Description
            MESStationSession FailDescSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (FailDescSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrFailDesc = FailDescSession.Value.ToString();

            //獲取Fail Location
            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (LocationSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrLocation = LocationSession.Value.ToString();

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (FailListSession == null)
            {
                FailListSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FailListSession);
            }

            FailInfo[0] = StrFailCode;
            FailInfo[1] = StrLocation;
            FailInfo[2] = StrFailDesc;
            FailListSession.Value = FailInfo;
        }

        /// <summary>
        ///從輸入框加載數據到下拉框
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputValueLoaderToSelectList(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string strPanel = "";
            List<string> PackList =new List<string>();
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                PackNoSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                PackNoSession.InputValue = Input.Value.ToString();
                Station.StationSession.Add(PackNoSession);
            }
            strPanel = PackNoSession.InputValue.ToString();

            MESStationSession PackListSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PackListSession == null)
            {
                PackListSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(PackListSession);
            }
            else
            {
                PackList = (List<string>)PackListSession.Value;
            }
            PackList.Add(strPanel);
            PackListSession.Value = PackList;
            MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
            s.DataForUse.Clear();
            foreach (var VARIABLE in PackList)
            {
                s.DataForUse.Add(VARIABLE);
            }
            Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE).Value = "";
        }
        
        public static void ObaSampleStationInit(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE);
            MESStationInput scanTypeInput = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
            MESStationInput failSnInput = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE);
            MESStationInput FailCodeInput = Station.Inputs.Find(t => t.DisplayName == Paras[3].SESSION_TYPE);
            MESStationInput LocationInput = Station.Inputs.Find(t => t.DisplayName == Paras[4].SESSION_TYPE);
            MESStationInput FailDescInput = Station.Inputs.Find(t => t.DisplayName == Paras[5].SESSION_TYPE);
            
            snInput.Visable = false;
            failSnInput.Visable = false;
            FailCodeInput.Visable = false;
            LocationInput.Visable = false;
            FailDescInput.Visable = false;
            scanTypeInput.Visable = false;
            scanTypeInput.DataForUse.Add("Pass");
            scanTypeInput.DataForUse.Add("Fail");
        }

        public static void LoadSampleLotByPackNo(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            string packNo = Input.Value.ToString();
            #region 用於界面上顯示的批次信息
            R_LOT_STATUS rLotStatus = new R_LOT_STATUS();
            List<R_LOT_PACK> rLotPackList = new List<R_LOT_PACK>();
            #endregion
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(Station.SFCDB,Station.DBType);
            T_R_LOT_PACK rRLotPack = new T_R_LOT_PACK(Station.SFCDB, Station.DBType);
            List<R_LOT_STATUS> rLotStatusList = tRLotStatus.getSampleLotByPackNo(packNo, Station.SFCDB);

            if (rLotStatusList.FindAll(t=>t.CLOSED_FLAG=="1").Count>1)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529105040", new string[] { packNo }));
            else if(rLotStatusList.FindAll(t => t.CLOSED_FLAG == "0").Count > 0)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529111019", new string[] { packNo, rLotStatusList.Find(t => t.CLOSED_FLAG == "0").LOT_NO }));
            else if(rLotStatusList.FindAll(t => t.CLOSED_FLAG != "2").Count == 0)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529111245", new string[] { packNo }));

            rLotStatus = rLotStatusList.Find(t => t.CLOSED_FLAG == "1");
            rLotPackList = rRLotPack.GetRLotPackByLotNo(Station.SFCDB, rLotStatus.LOT_NO);

            #region 加載界面信息
            MESStationSession lotNoSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
            MESStationSession skuNoSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
            MESStationSession aqlSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
            MESStationSession lotQtySession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
            MESStationSession sampleQtySession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
            MESStationSession rejectQtySession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
            MESStationSession sampleQtyWithAQLSession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
            MESStationSession passQtySession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
            MESStationSession failQtySession = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };

            Station.StationSession.Clear();
            Station.StationSession.Add(lotNoSession);
            Station.StationSession.Add(skuNoSession);
            Station.StationSession.Add(aqlSession);
            Station.StationSession.Add(lotQtySession);
            Station.StationSession.Add(sampleQtySession);
            Station.StationSession.Add(rejectQtySession);
            Station.StationSession.Add(sampleQtyWithAQLSession);
            Station.StationSession.Add(passQtySession);
            Station.StationSession.Add(failQtySession);

            lotNoSession.Value = rLotStatus.LOT_NO;
            skuNoSession.Value = rLotStatus.SKUNO;
            aqlSession.Value = rLotStatus.AQL_TYPE;
            lotQtySession.Value = rLotStatus.LOT_QTY;
            sampleQtySession.Value = rLotStatus.SAMPLE_QTY;
            rejectQtySession.Value = rLotStatus.REJECT_QTY;
            sampleQtyWithAQLSession.Value = rLotStatus.PASS_QTY + rLotStatus.FAIL_QTY;
            passQtySession.Value = rLotStatus.PASS_QTY;
            failQtySession.Value = rLotStatus.FAIL_QTY;

            MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[9].SESSION_TYPE);
            s.DataForUse.Clear();
            foreach (var VARIABLE in rLotPackList)
                s.DataForUse.Add(VARIABLE.PACKNO);

            MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == "SN");
            MESStationInput packInput = Station.Inputs.Find(t => t.DisplayName == "PACKNO");
            MESStationInput scanTypeInput = Station.Inputs.Find(t => t.DisplayName == "ScanType");
            packInput.Visable = false;
            snInput.Visable = true;
            scanTypeInput.Visable = true;
            #endregion

        }

    }
}
