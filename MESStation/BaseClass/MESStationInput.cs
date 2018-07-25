using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MESDataObject;
using MESDataObject.Module;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using MESDBHelper;

namespace MESStation.BaseClass
{
    public class MESStationInput
    {
        public static  string[] RunActionSEQ = new string[] { "Default", "Customer", "Series", "Sku", "WorkerOrder", "Line" }; 
        [JsonIgnore]
        [ScriptIgnore]
        public MESStationBase Station;
        object _Value = "";
        
        [JsonIgnore]
        [ScriptIgnore]
        public string InputID
        {
            get { return BaseModel.ID; }
        }
        public string ID
        {
            get { return RecordModel.ID; }
        }
        public string Name
        {
            get { return BaseModel.INPUT_NAME; }
        }
        public string DisplayType
        {
            get { return BaseModel.DISPLAY_TYPE; }
        }
        //public string DisplayType;
        public object DataSourceAPI
        {
            get { return BaseModel.DATA_SOURCE_API; }
        }
        public string DataSourceAPIPara
        {
            get { return BaseModel.DATA_SOURCE_API_PARA; }
        }

        public string RefershType
        {
            get { return BaseModel.REFRESH_TYPE; }
        }

        public string DisplayName
        {
            get
            {
                return RecordModel.DISPLAY_NAME;
            }
        }

        public string RememberLastInput
        {
            get { return RecordModel.REMEMBER_LAST_INPUT; }
        }

        public double? SeqNo
        {
            get { return RecordModel.SEQ_NO; }
        }
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public double? ScanFlag
        {
            get { return RecordModel.SCAN_FLAG; }
        }

        bool _Visable = true;
        public bool Visable
        {
            set
            {
                _Visable = value;
            }
            get
            {
                return _Visable;
            }
        }
        bool _Enable = true;
        public bool Enable
        {
            set
            {
                _Enable = value;
            }
            get
            {
                return _Enable;
            }
        }

        List<object> _DataForUse = new List<object>();
        public List<object> DataForUse
        {
            get
            {
                return _DataForUse;
            }
            
        }



        //public string DataType;//SN,WO,SKUNO,PanelSN,NEW_SN,NEW_PanelSN,PO,PalletNO,CartonNO,TRAY_NO,TR_SN,OTHER 
        //[JsonIgnore]
        //[ScriptIgnore]
        //public List<R_Input_Action> InputDataLoaders = new List<R_Input_Action>();
        //[JsonIgnore]
        //[ScriptIgnore]
        //public Dictionary<string, List<R_Station_Action>> StationDataLoaders = new Dictionary<string, List<R_Station_Action>>();

        //[JsonIgnore]
        //[ScriptIgnore]
        //public Dictionary<string, List<R_Station_Action>> StationDataChecker = new Dictionary<string, List<R_Station_Action>>();

        //[JsonIgnore]
        //[ScriptIgnore]
        //public Dictionary<string, List<R_Station_Action>> StationActionRunner = new Dictionary<string, List<R_Station_Action>>();


        //[JsonIgnore]
        //[ScriptIgnore]
        public List<StationAction> InputActions = new List<StationAction>();//void function(station , MESStationInput )

        public List<StationAction> InputDataloaders
        {
            get
            {
                List < StationAction >  ret =InputActions.FindAll(t => t.ActionType == "DataLoader" && t.StationActionType == StationActionTypeEnum.Input);
                ret.Sort();
                return ret;
            }
        }

        public List<StationAction> Dataloaders
        {
            get
            {
                List<StationAction> ret = InputActions.FindAll(t => t.ActionType == "DataLoader" && t.StationActionType == StationActionTypeEnum.Station);
                ret.Sort();
                return ret;
            }
        }

        public List<StationAction> DataCheckers
        {
            get
            {
                List<StationAction> ret = InputActions.FindAll(t => t.ActionType == "DataChecker" && t.StationActionType == StationActionTypeEnum.Station);
                ret.Sort();
                return ret;
            }
        }

        public List<StationAction> ActionRunners
        {
            get
            {
                List<StationAction> ret = InputActions.FindAll(t => t.ActionType == "ActionRunner" && t.StationActionType == StationActionTypeEnum.Station);
                ret.Sort();
                return ret;
            }
        }



        C_Input BaseModel;
        R_Station_Input RecordModel;
        public void Init(C_Input Cmodel, R_Station_Input Rmodel)
        {
            BaseModel = Cmodel;
            RecordModel = Rmodel;
            OleExec SFCDB = Station.SFCDB;
            try
            {
                //調用處理邏輯
                T_c_station_action TCSA = new T_c_station_action(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_Input_Action TRIA = new T_R_Input_Action(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_Station_Action TRSA = new T_R_Station_Action(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_Station_Input TRSI = new T_R_Station_Input(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_Station_Action_Para TRSAP = new T_R_Station_Action_Para(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_Input_Action> _InputActions = TRIA.GetActionByInputID(InputID, SFCDB);
                //_InputActions.Sort();
                _InputActions.OrderBy(r => r.SEQ_NO);
                for (int i = 0; i < _InputActions.Count; i++)
                {
                    StationAction Action = new StationAction(_InputActions[i], this);
                    InputActions.Add(Action);
                }
                
                List<R_Station_Action> _StationActions = TRSA.GetActionByInputID(RecordModel.ID, SFCDB);
                for (int i = 0; i < _StationActions.Count; i++)
                {
                    StationAction Action = new StationAction(_StationActions[i], this);
                    InputActions.Add(Action);
                }


               

            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        public void Run()
        {
            List<StationAction> actions = this.InputDataloaders;


            Station.SFCDB.BeginTrain();
            Station.APDB.BeginTrain();
            string StrValue = this.Value.ToString();
            string StrActionRunTime = "";
            try
            {
                for (int i = 0; i < RunActionSEQ.Length; i++)
                {
                    List<StationAction> A = InputActions;
                    actions = A.FindAll(t => t.ConfigType == RunActionSEQ[i]);
                    for (int j = 0; j < actions.Count; j++)
                    {
                        DateTime start = DateTime.Now;
                        if (CheckRun(A, j, actions[j].CActionID))
                        {
                            try
                            {
                                actions[j].Run(Station, this);
                            }
                            catch (Exception e1)
                            {
                                throw new Exception(actions[j].ActionName + ":" + e1.Message);
                            }
                        }
                        TimeSpan RunTime = DateTime.Now - start;
                        StrActionRunTime += actions[j].ActionName + " : " + RunTime.TotalSeconds.ToString() + "\r\n";
                    }
                    
                }
                Station.StationMessages.Add
                    (new MESReturnView.Station.StationMessage()
                    {
                        Message = StrActionRunTime,
                        State = MESReturnView.Station.StationMessageState.Debug
                    }
                    );
                List<MESReturnView.Station.StationMessage> remove = Station.StationMessages.FindAll(t => t.State == MESReturnView.Station.StationMessageState.Pass);
                for (int i = 0; i < remove.Count; i++)
                {
                    Station.StationMessages.Remove(remove[i]);
                }
                Station.StationMessages.Add(new MESReturnView.Station.StationMessage() { Message = (StrValue + " OK"), State = MESReturnView.Station.StationMessageState.Pass });
                Station.SFCDB.CommitTrain();
                Station.APDB.CommitTrain();
            }
            catch (Exception ee)
            {
                Station.SFCDB.RollbackTrain();
                Station.APDB.RollbackTrain();
                throw ee;
            }
        }

        bool CheckRun(List<StationAction> actions ,int j, string ActionID)
        {
            if (j == RunActionSEQ.Length - 1)
            {
                return true;
            }
            for (int i = j+1; i < RunActionSEQ.Length; i++)
            {
                StationAction A = actions.Find(t => t.AddFlag == 0 && t.CActionID == ActionID);
                if (A != null)
                {
                    return false;
                }
               
            }
            return true;
        }

    }

    public class StationAction : IComparer<StationAction>, IComparable<StationAction>
    {

        public string CActionID
        {
            get
            { return CAction.ID; }
        }
        public string ActionName
        {
            get
            { return CAction.ACTION_NAME; }
        }
        public string ActionType
        {
            get
            { return CAction.ACTION_TYPE; }
        }

        public string DLL
        {
            get
            { return CAction.DLL_NAME; }
        }

        public string ClassName
        {
            get
            { return CAction.CLASS_NAME; }
        }

        public string ConfigType
        {
            get
            {
                if (StationActionType == StationActionTypeEnum.Input)
                {
                    return RInputAction.CONFIG_TYPE;
                }
                else
                {
                    return RStationAction.CONFIG_TYPE;
                }
            }
        }

        public string ConfigValue
        {
            get
            {
                if (StationActionType == StationActionTypeEnum.Input)
                {
                    return RInputAction.CONFIG_VALUE;
                }
                else
                {
                    return RStationAction.CONFIG_VALUE;
                }
            }
        }
        public double? SEQ
        {
            get
            {
                if (StationActionType == StationActionTypeEnum.Input)
                {
                    return RInputAction.SEQ_NO;
                }
                else
                {
                    return RStationAction.SEQ_NO;
                }
            }
        }

        public double? AddFlag
        {
            get
            {
                if (StationActionType == StationActionTypeEnum.Input)
                {
                    return RInputAction.ADD_FLAG;
                }
                else
                {
                    return RStationAction.ADD_FLAG;
                }
            }
        }

        public string FunctionName
        {
            get
            { return CAction.FUNCTION_NAME; }
        }

        [JsonIgnore]
        [ScriptIgnore]
        public MESStationBase Station;
        [JsonIgnore]
        [ScriptIgnore]
        MESStationInput Input;

        R_Input_Action RInputAction;
        c_station_action CAction;
        R_Station_Action RStationAction;

        Assembly assenby;
        Type type;
        MethodInfo function;

        public StationActionTypeEnum StationActionType;


        public List<R_Station_Action_Para> Paras;



        public StationAction(R_Input_Action _InputAction, MESStationInput _Input)
        {
            Input = _Input;
            RInputAction = _InputAction;
            Station = _Input.Station;
            StationActionType = StationActionTypeEnum.Input;
            OleExec SFCDB = Input.Station.SFCDB;
            LoadT_c_station_action(_InputAction.C_STATION_ACTION_ID, SFCDB);
            LoadPara(SFCDB);

        }

        public StationAction(R_Station_Action _StationAction, MESStationInput _Input)
        {
            Input = _Input;
            RStationAction = _StationAction;
            StationActionType = StationActionTypeEnum.Station;
            OleExec SFCDB = Input.Station.SFCDB;
            LoadT_c_station_action(_StationAction.C_STATION_ACTION_ID, SFCDB);
            LoadPara(SFCDB);

        }

        void LoadT_c_station_action(string C_STATION_ACTION_ID, OleExec SFCDB)
        {
            T_c_station_action TCSA = new T_c_station_action(SFCDB, DB_TYPE_ENUM.Oracle);
            Row_c_station_action RCSA = (Row_c_station_action)TCSA.GetObjByID(C_STATION_ACTION_ID, SFCDB);
            CAction = RCSA.GetDataObject();
        }

        void LoadPara(OleExec SFCDB)
        {
            T_R_Station_Action_Para TRSAP = new T_R_Station_Action_Para(SFCDB, DB_TYPE_ENUM.Oracle);

            if (this.StationActionType == StationActionTypeEnum.Input)
            {
                Paras = TRSAP.GetActionParaByInputActionID(RInputAction.ID, SFCDB);
            }
            else
            {
                Paras = TRSAP.GetActionParaByStationActionID(RStationAction.ID, SFCDB);
            }
        }

        public virtual void Run(MESStationBase Station, MESStationInput Input)
        {
            
            
            if (function == null)
            {
                if (type == null)
                {
                    if (assenby == null)
                    {
                        try
                        {
                            assenby = Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\" + DLL);
                        }
                        catch {
                            //assenby = Assembly.LoadFile(System.Web.HttpContext.Current.Server.MapPath(DLL));
                            //assenby = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory+"\\bin\\"+ DLL);
                            assenby = Assembly.Load("MESStation");
                        }
                    }
                    type = assenby.GetType(ClassName.Trim().Replace("\r\n",""));
                    if (type == null)
                    {
                        throw new Exception(ClassName.Trim().Replace("\r\n", "") +"Can not be Create!");
                    }
                }
                function = type.GetMethod(FunctionName.Trim().Replace("\r\n",""));
            }
            try
            {
                //function.Invoke(null, new object[] { null, null, null });
                function.Invoke(null, new object[] { Station, Input, Paras });
            }
            catch (MESReturnMessage e1)
            {
                throw e1; 
            }
            catch (Exception ee)
            {
                if (ee.InnerException.GetType() == typeof(MESReturnMessage))
                {
                    throw ee.InnerException;
                }
                if (ee.InnerException != null)
                {
                    throw ee.InnerException;
                }
                throw ee;
            }


        }

        public int Compare(StationAction x, StationAction y)
        {
            if (x.SEQ > y.SEQ)
            {
                return 1;
            }
            else if (x.SEQ == y.SEQ)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public int CompareTo(StationAction other)
        {
            //if (SEQ_NO == other.SEQ_NO)
            if (SEQ > other.SEQ)
            {
                return 1;
            }
            else if (SEQ == other.SEQ)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
    public enum StationActionTypeEnum
    {
        Input,
        Station
    }
}
