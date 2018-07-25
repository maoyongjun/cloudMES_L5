using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using System.Web.Script.Serialization;
using MESDataObject.Module;
using Newtonsoft.Json;
using System.Reflection;

namespace MESStation.BaseClass
{
    public class MESStationBase
    {
        //public string Name;
        public string Line;
        public string BU;
        public LogicObject.User LoginUser;
        public string IP = "";



        public List<MESStationInput> Inputs = new List<MESStationInput>();
        //public List<C_Input> Inputs = new List<C_Input>();
        [JsonIgnore]
        [ScriptIgnore]
        public LogicObject.User User;

        [JsonIgnore]
        [ScriptIgnore]
        public Dictionary<string, OleExecPool> DBS;

        [JsonIgnore]
        [ScriptIgnore]
        public OleExec SFCDB;

        [JsonIgnore]
        [ScriptIgnore]
        public OleExec APDB;

        [JsonIgnore]
        [ScriptIgnore]
        public List<MESStationSession> StationSession = new List<MESStationSession>();

        [JsonIgnore]
        [ScriptIgnore]
        public List<MESStationSession> MainStationSession = null;

        [JsonIgnore]
        [ScriptIgnore]
        public List<MESStationSession> SubStationSession = null;

        [JsonIgnore]
        [ScriptIgnore]
        public DB_TYPE_ENUM DBType = DB_TYPE_ENUM.Oracle;

        [JsonIgnore]
        [ScriptIgnore]
        public List<R_Station_Output> StationOutputs = new List<R_Station_Output>();

        //public Dictionary<string, DisplayOutPut> DisplayOutput = new Dictionary<string, DisplayOutPut>();

        public List<DisplayOutPut> DisplayOutput = new List<DisplayOutPut>();

        public List<Label.LabelBase> LabelPrint = new List<Label.LabelBase>();

        MESDataObject.Module.R_Station _StationInfo;

        public string DisplayName
        {
            get
            { return _StationInfo.DISPLAY_STATION_NAME; }
        }

        public string StationName
        {
            get
            { return _StationInfo.STATION_NAME; }
            set //Add by LLF 2018-02-02,VI1&VI2,PRINT1&PRINT2,AOI1&AOI2 可以相互切換
            { _StationInfo.STATION_NAME = value; }
        }

        public string ID
        {
            get
            { return _StationInfo.ID; }
        }

        public MESStationBase FailStation = null;

        [JsonIgnore]
        [ScriptIgnore]
        public Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        [JsonIgnore]
        [ScriptIgnore]
        public Dictionary<string, c_station_action> CStationActionCache = new Dictionary<string, c_station_action>();

        [JsonIgnore]
        [ScriptIgnore]
        public Dictionary<string, List<R_Station_Action_Para>> ActionParaCache = new Dictionary<string, List<R_Station_Action_Para>>();

        [JsonIgnore]
        [ScriptIgnore]
        List<Row_R_Station_Output> ROutputs;

        [JsonIgnore]
        [ScriptIgnore]
        public MESStationInput NextInput;

        public List<MESReturnView.Station.StationMessage> StationMessages = new List<MESReturnView.Station.StationMessage>();

        public List<WaitKPReturn> ScanKP = new List<WaitKPReturn>();

        /// <summary>
        /// 初始化方法
        /// </summary>
        public virtual void Init(string _DisplayName ,string _Line,string _BU ,Dictionary<string, OleExecPool> _DBS)
        {
            Inputs.Clear();
            DBS = _DBS;
            SFCDB = DBS["SFCDB"].Borrow();
            Line = _Line;
            BU = _BU;
            //加載Station Input
            try
            {
                T_R_Station T = new T_R_Station(SFCDB, DBType);
                Row_R_Station R = T.GetRowByDisplayName(_DisplayName, SFCDB);
                _StationInfo = R.GetDataObject();
            
                T_R_Station_Input T_I = new T_R_Station_Input(SFCDB, DBType);
                List<Row_R_Station_Input> R_Inputs = T_I.GetRowsByStationID(_StationInfo.ID, SFCDB);

                T_C_Input T_INPUT = new T_C_Input(SFCDB, DBType);
                for (int i = 0; i < R_Inputs.Count; i++)
                {
                    Row_C_Input R_C_I = (Row_C_Input)T_INPUT.GetObjByID(R_Inputs[i].INPUT_ID, SFCDB);
                    C_Input C_Input = R_C_I.GetDataObject();
                    MESStationInput Input = new MESStationInput();
                    Input.Station = this;
                    Input.Init(C_Input, R_Inputs[i].GetDataObject());
                    Inputs.Add(Input);
                }
                //加載Station OutPut
                T_R_Station_Output TRSO = new T_R_Station_Output(SFCDB, DBType);
                List<Row_R_Station_Output> R_Outputs = TRSO.GetStationOutputByStationID(_StationInfo.ID, SFCDB);

                ROutputs = R_Outputs;
                for (int i = 0; i < R_Outputs.Count; i++)
                {
                    R_Station_Output o = R_Outputs[i].GetDataObject();
                    DisplayOutPut o1 = new DisplayOutPut();
                    o1.DisplayType = o.DISPLAY_TYPE;
                    o1.Value = "";
                    o1.Name = o.NAME;
                    StationOutputs.Add(o);
                    DisplayOutput.Add( o1);
                }
                DBS["SFCDB"].Return(SFCDB);
            }
            catch(Exception ex)
            {
                DBS["SFCDB"].Return(SFCDB);
                throw ex;
            }

            this.MainStationSession = StationSession;

            if (_StationInfo.FAIL_STATION_ID != null && _StationInfo.FAIL_STATION_ID.ToString().Trim() != "")
            {
                if (this.FailStation != null)
                {
                    this.FailStation.StationMessages.Clear();
                    this.NextInput = null;
                }
                else
                {
                    this.FailStation = new MESStationBase();
                    
                }
                this.FailStation.MainStationSession = this.StationSession;
                this.FailStation.SubStationSession = this.FailStation.StationSession;
                this.SubStationSession = this.FailStation.StationSession;
                this.FailStation.LoginUser = LoginUser;
                this.FailStation.Init(_StationInfo.FAIL_STATION_ID, _Line,_BU,_DBS);
            }

        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        public virtual void Init(string _DisplayName, string _Line, string _BU,OleExec DB)
        {
            Inputs.Clear();
            SFCDB = DB;
            Line = _Line;
            BU = _BU;
            //加載Station Input
            try
            {
                T_R_Station T = new T_R_Station(SFCDB, DBType);
                Row_R_Station R = T.GetRowByDisplayName(_DisplayName, SFCDB);
                _StationInfo = R.GetDataObject();

                T_R_Station_Input T_I = new T_R_Station_Input(SFCDB, DBType);
                List<Row_R_Station_Input> R_Inputs = T_I.GetRowsByStationID(_StationInfo.ID, SFCDB);

                T_C_Input T_INPUT = new T_C_Input(SFCDB, DBType);
                for (int i = 0; i < R_Inputs.Count; i++)
                {
                    Row_C_Input R_C_I = (Row_C_Input)T_INPUT.GetObjByID(R_Inputs[i].INPUT_ID, SFCDB);
                    C_Input C_Input = R_C_I.GetDataObject();
                    MESStationInput Input = new MESStationInput();
                    Input.Station = this;
                    Input.Init(C_Input, R_Inputs[i].GetDataObject());
                    Inputs.Add(Input);
                }
                //加載Station OutPut
                T_R_Station_Output TRSO = new T_R_Station_Output(SFCDB, DBType);
                List<Row_R_Station_Output> R_Outputs = TRSO.GetStationOutputByStationID(_StationInfo.ID, SFCDB);

                ROutputs = R_Outputs;
                for (int i = 0; i < R_Outputs.Count; i++)
                {
                    R_Station_Output o = R_Outputs[i].GetDataObject();
                    DisplayOutPut o1 = new DisplayOutPut();
                    o1.DisplayType = o.DISPLAY_TYPE;
                    o1.Value = "";
                    o1.Name = o.NAME;
                    StationOutputs.Add(o);
                    DisplayOutput.Add(o1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            if (_StationInfo.FAIL_STATION_ID != null && _StationInfo.FAIL_STATION_ID.ToString().Trim() != "")
            {
                if (this.FailStation != null)
                {
                    this.FailStation.StationMessages.Clear();
                    this.NextInput = null;
                }
                else
                {
                    this.FailStation = new MESStationBase();
                }
                this.FailStation.LoginUser = LoginUser;
                this.FailStation.Init(_StationInfo.FAIL_STATION_ID, _Line, _BU, DB);
            }

        }

        public void AddMessage(string MessageCode, string[] para, MESReturnView.Station.StationMessageState state )
        {
            string message = MESReturnMessage.GetMESReturnMessage(MessageCode, para);
            MESReturnView.Station.StationMessage msg = new MESReturnView.Station.StationMessage();
            msg.Message = message;
            msg.State = state;
            StationMessages.Add(msg);
        }

        public void MakeOutput()
        {
            StationOutputs.Clear();
            DisplayOutput.Clear();
            for (int i = 0; i < ROutputs.Count; i++)
            {
                R_Station_Output o = ROutputs[i].GetDataObject();
                DisplayOutPut o1 = new DisplayOutPut();
                o1.DisplayType = o.DISPLAY_TYPE;
                o1.Value = "";
                o1.Name = o.NAME;
                if (o1.Name.ToUpper() == "USER")
                {
                    o1.Value = this.LoginUser.EMP_NO;
                }
                if (o1.Name.ToUpper() == "LINE")
                {
                    o1.Value = this.Line;
                }

                MESStationSession s = StationSession.Find(t => t.MESDataType == o.SESSION_TYPE && o.SESSION_KEY == t.SessionKey);
                

                if (s != null)
                {
                    if (o.DISPLAY_TYPE != "Table")
                    {
                        if (s.Value != null)
                        {
                            o1.Value = s.Value.ToString();
                        }
                        else
                        {
                            o1.Value = s.Value;
                        }
                    }
                    else
                    {
                        o1.Value = s.Value;
                    }
                }


                StationOutputs.Add(o);
                DisplayOutput.Add(o1);
            }
        }

        public object GetStationValue(string _MESDataType)
        {
            MESStationSession session = this.StationSession.Find(t => t.MESDataType == _MESDataType && t.SessionKey == "1");
            if (session != null)
            {
                return session.Value;
            }
            else
            {
                return null;
            }
        }

        public MESStationInput FindInputByName(string Name)
        {
            MESStationInput input = this.Inputs.Find(t => t.DisplayName.Equals(Name));
            return input;
        }
        public DateTime GetDBDateTime()
        {         
            string strSql = "select sysdate from dual";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = "select sysdate from dual";
            }
            else if (DBType == DB_TYPE_ENUM.SqlServer)
            {
                strSql = "select get_date() ";
            }
            else
            {
                throw new Exception(DBType.ToString() + " not Work");
            }
            DateTime DBTime = (DateTime)SFCDB.ExecSelectOneValue(strSql);        
            return DBTime;
        }

        public void Reset()
        {

        }

        public void Reset(List<MESStationInput> inputs)
        {
            for(int i = 0; i< inputs.Count;i++)
            {
                List<MESStationSession> RemoveInputValues = StationSession.FindAll(t => t.ResetInput == inputs[i]);
                for (int j = 0; j < RemoveInputValues.Count; j++)
                {
                    StationSession.Remove(RemoveInputValues[j]);
                }
                inputs[i].Value = null;
            }
        }
        public void AddKPScan(string SN,string WO,string Station )
        {
            WaitKPReturn W = new WaitKPReturn() { SN = SN, WO = WO, Station = Station };
            ScanKP.Add(W);
        }
    }
    public class MESInputDataLoader
    {
        public MESStationInput Input;
        public string SessionType;//WO
        public string SessionKey;//1
        public string DataSourceType;
        public string DataSourceSessionKey;

        public virtual void LoadData(MESStationBase station , MESStationInput Input)
        {
            string WO = Input.Value.ToString();
            //.....
            station.StationSession.Add(new MESStationSession { Value = WO, MESDataType = SessionType, SessionKey = SessionKey });
        }
    }


    public class MESStationSession
    {

        public MESStationInput ResetInput;
        public string MESDataType;
        public string SessionKey;
        public object Value;
        public string InputValue;
        
    }

    public class DisplayOutPut
    {
        public string Name;
        public object Value;
        public string DisplayType;
       
    }

    public class WaitKPReturn
    {
        public string SN;
        public string WO;
        public string Station;
    }

}
