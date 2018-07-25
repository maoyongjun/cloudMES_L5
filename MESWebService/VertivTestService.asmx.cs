using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MESDataObject;
using MESDBHelper;
using System.Data;
using MESDataObject.Module;

namespace MESWebService
{
    /// <summary>
    /// VertivTestService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class VertivTestService : System.Web.Services.WebService
    {
        public static MESDBHelper.OleExecPool VertivSfbDbPool = new OleExecPool("VERTIVTESTDB", true);
        public static MESDBHelper.OleExecPool VertivApDbPool = new OleExecPool("VERTIVTESTDB", true);
        public MESServiceRes resObj = new MESServiceRes();
        public VertivTestService()
        {

        }
        /// <summary>
        /// TE上传测试记录;
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="MODEL"></param>
        /// <param name="TESTTIME"></param>
        /// <param name="STATE"></param>
        /// <param name="STATION"></param>
        /// <param name="CELL"></param>
        /// <param name="OPERATOR"></param>
        /// <param name="ERROR_CODE"></param>
        /// <returns></returns>
        [WebMethod]
        public MESServiceRes TestDataUploadMES(string SN, string MODEL, string TESTTIME, string STATE, string STATION,
            string CELL, string OPERATOR, string ERROR_CODE)
        {
            //Sql注入;
            TestRecordData testRecord = new TestRecordData();

            #region DataCheck;

            try
            {
                testRecord = TestDataUploadMES_CheckInputData(SN, MODEL, TESTTIME, STATE, STATION, CELL, OPERATOR,
                    ERROR_CODE);
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int) StatusValue.fail;
                resObj.MessageCode = "MES00001";
                resObj.Message = "输入参数错误:" + e.ToString();
                return resObj;
            }

            #endregion

            OleExec DB = new OleExec("VERTIVTESTDB", true);
            T_C_TEMES_STATION_MAPPING cTeMesStationMappingControl = new T_C_TEMES_STATION_MAPPING(DB,
                DB_TYPE_ENUM.Oracle);
            T_R_TEST_RECORD rTestRecordControl = new T_R_TEST_RECORD(DB, DB_TYPE_ENUM.Oracle);
            T_R_TEST_DETAIL_VERTIV rTestDetailVertivControl = new T_R_TEST_DETAIL_VERTIV(DB, DB_TYPE_ENUM.Oracle);
            T_R_SN rSnControl = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);

            Row_R_TEST_RECORD rowRTestRecord = (Row_R_TEST_RECORD) rTestRecordControl.NewRow();
            Row_R_TEST_DETAIL_VERTIV rowRTestDetailVertiv = (Row_R_TEST_DETAIL_VERTIV) rTestDetailVertivControl.NewRow();
            try
            {
                C_TEMES_STATION_MAPPING cTeMesStationMapping = cTeMesStationMappingControl.GetTeMesStationMapping(DB,
                    STATION, "A");
                if(cTeMesStationMapping==null)
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES000017";
                    resObj.Message = $@"上傳失敗:未在MES系統找到Te-Station:{STATION}對應的工站,請聯繫IT配置!";
                    return resObj;
                } 
                R_SN rSn = rSnControl.LoadSN(SN, DB);
                //RTestRecord
                rowRTestRecord.ID = cTeMesStationMappingControl.GetNewID("VERTIV", DB);
                rowRTestRecord.R_SN_ID = rSn?.ID;
                rowRTestRecord.SN = testRecord.SN;
                rowRTestRecord.ENDTIME = DateTime.Parse(testRecord.TESTTIME);
                rowRTestRecord.STARTTIME = DateTime.Parse(testRecord.TESTTIME);
                rowRTestRecord.STATE = testRecord.STATE;
                rowRTestRecord.TEGROUP = "A";
                rowRTestRecord.TESTATION = testRecord.STATION;
                rowRTestRecord.MESSTATION = cTeMesStationMapping?.MES_STATION;
                rowRTestRecord.DETAILTABLE = "R_TEST_DETAIL_VERTIV";
                //RTestDetailVertiv
                rowRTestDetailVertiv.ID = rTestDetailVertivControl.GetNewID("VERTIV", DB);
                rowRTestDetailVertiv.R_TEST_RECORD_ID = rowRTestRecord.ID;
                rowRTestDetailVertiv.SN = testRecord.SN;
                rowRTestDetailVertiv.SKUNO = testRecord.MODEL;
                rowRTestDetailVertiv.CREATETIME = DateTime.Parse(testRecord.TESTTIME);
                rowRTestDetailVertiv.STATE = testRecord.STATE;
                rowRTestDetailVertiv.STATION = testRecord.STATION;
                rowRTestDetailVertiv.CELL = testRecord.CELL;
                rowRTestDetailVertiv.OPERATOR = testRecord.OPERATOR;
                rowRTestDetailVertiv.ERROR_CODE = testRecord.ERROR_CODE;
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int) StatusValue.fail;
                resObj.MessageCode = "MES00003";
                resObj.Message = "MESDB异常:" + e.ToString();
                return resObj;
            }
            try
            {
                DB.BeginTrain();
                DB.ExecSQL(rowRTestDetailVertiv.GetInsertString(DB_TYPE_ENUM.Oracle));
                DB.ExecSQL(rowRTestRecord.GetInsertString(DB_TYPE_ENUM.Oracle));
                DB.CommitTrain();
                resObj.Statusvalue = (int) StatusValue.success;
                resObj.MessageCode = "";
                resObj.Message = "Upload Success!";
            }
            catch (Exception e)
            {
                DB.RollbackTrain();
                resObj.Statusvalue = (int) StatusValue.fail;
                resObj.MessageCode = "MES00002";
                resObj.Message = "写入MESERR:" + e.ToString();
            }
            finally
            {
                DB.FreeMe();
            }
            return resObj;
        }

        [WebMethod]
        public MESServiceRes PassTestStation(string SN,string STATION)
        {                    
            CallWebService c = new CallWebService(VertivSfbDbPool, VertivApDbPool);
            resObj = new MESServiceRes {Message= "", Statusvalue = (int)StatusValue.success, MessageCode = ""};
            MESStation.BaseClass.MESStationReturn s = new MESStation.BaseClass.MESStationReturn();
            try
            {
                #region StationPara setting
                StationPara sp = new StationPara { Station = STATION, Line = "TestStation" };
                #endregion

                #region InitStation
                c.InitStation(s,sp);
                #endregion

                #region Setting Inputs Value
                MESStation.MESReturnView.Station.CallStationReturn ret = (MESStation.MESReturnView.Station.CallStationReturn)s.Data;
                ret.Station.Inputs[0].Value = SN;
                #endregion

                #region Doing Inputs Events
                c.StationInput(s, "PASS", "SN");
                #endregion

                #region setting run results
                foreach (var stationRes in ret.Station.StationMessages)
                {
                    if (stationRes.State == MESStation.MESReturnView.Station.StationMessageState.Fail)
                    {
                        resObj = new MESServiceRes { Message = stationRes.Message , Statusvalue = (int)StatusValue.fail, MessageCode = "MES00011" };
                        break;
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                resObj = new MESServiceRes { Message = s.Message, Statusvalue = (int)StatusValue.fail, MessageCode = "MES00012" };
            }

            return resObj;
        }

        /// <summary>
        /// 获取上传到MES的测试记录
        /// </summary>
        /// <param name="SN"></param>
        /// <returns>MESServiceRes</returns>
        [WebMethod]
        public MESServiceRes GetTestDataFromMES(string SN)
        {
            //DataCheck;
            //Sql注入;
            OleExec DB = new OleExec("VERTIVTESTDB", true);

            T_R_TEST_DETAIL_VERTIV rTestDetailVertivControl = new T_R_TEST_DETAIL_VERTIV(DB, DB_TYPE_ENUM.Oracle);
            DataTable dt = new DataTable();
            try
            {
                dt = rTestDetailVertivControl.GetDTRTestDetailVertivBySn(DB, SN);
                string resDtjson = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                if (dt.Rows.Count > 0)
                {
                    resObj.Statusvalue = (int)StatusValue.success;
                    resObj.MessageCode = "";
                    resObj.Message = resDtjson;
                }
                else
                {
                    resObj.Statusvalue = (int)StatusValue.fail;
                    resObj.MessageCode = "MES00005";
                    resObj.Message = "No Data!";
                }
            }
            catch (Exception e)
            {
                resObj.Statusvalue = (int)StatusValue.fail;
                resObj.MessageCode = "MES00004";
                resObj.Message = "查询错误:"+e.ToString();
            }
            return resObj;
        }

        TestRecordData TestDataUploadMES_CheckInputData(string SN, string MODEL, string TESTTIME, string STATE, string STATION, string CELL, string OPERATOR, string ERROR_CODE)
        {
            TestRecordData testRecord = new TestRecordData();
            testRecord.SN = SN;
            testRecord.MODEL = MODEL;
            testRecord.TESTTIME = TESTTIME;
            testRecord.STATE = STATE;
            testRecord.STATION = STATION;
            testRecord.CELL = CELL;
            testRecord.OPERATOR = OPERATOR;
            testRecord.ERROR_CODE = ERROR_CODE;
            return testRecord;
        }

        /// <summary>
        /// Service Return Obj
        /// </summary>
        public class MESServiceRes
        {
            public int Statusvalue;
            public string MessageCode;
            public string Message;
        }

        public enum StatusValue
        {
            success = 0,
            fail =1
        }

        public class TestRecordData
        {
            //string SN,string MODEL,string TESTTIME,string STATE,string STATION,string CELL,string OPERATOR,string ERROR_CODE
            private string sn;
            private string model;
            private string testtime;
            private string state;
            private string station;
            private string cell;
            private string operatordata;
            private string errorcode;
            //public TestRecordData(string _sn, string _model, string _testtime, string _state, string _station, string _cell, string _operatordata, string _errorcode)
            //{
            //    sn = _sn;
            //    model = _model;
            //    TESTTIME = _testtime;
            //    state = _state;
            //    station = _station;
            //    cell = _cell;
            //    operatordata = _operatordata;
            //    errorcode = _errorcode;
            //}
            public string SN
            {
                get
                {
                    return sn;
                } 
                set {
                    if (value.Length > 30)
                        throw new Exception("SN长度超过30");
                    sn = value;
                }
            }

            public string MODEL
            {
                get
                {
                    return model;
                }
                set
                {
                    if (value.Length > 20)
                        throw new Exception("MODEL长度超过20");
                    model = value;
                }
            }

            public string TESTTIME
            {
                get
                {
                    return testtime;
                }
                set
                {
                    try
                    {
                       DateTime.Parse(value);
                       testtime = value;
                    }
                    catch(Exception e)
                    {
                        throw new Exception("TESTTIME格式應為:yyyy-mm-dd hh24:mi:ss");
                    }
                }
            }
            public string STATE
            {
                get
                {
                    return state;
                }
                set
                {
                    if (value.Length > 5)
                        throw new Exception("STATE长度超过5");
                    switch (value.ToUpper().Trim())
                    {
                        case "P": state="PASS";break;
                        case "F": state = "FAIL"; break;
                        case "PASS": state = "PASS"; break;
                        case "FAIL": state = "FAIL"; break;
                        default: throw new Exception("STATE格式有误!");
                    }
                }
            }
            public string STATION
            {
                get
                {
                    return station;
                }
                set
                {
                    if (value.Length > 10)
                        throw new Exception("STATION长度超过10");
                    station = value;
                }
            }
            public string CELL
            {
                get
                {
                    return cell;
                }
                set
                {
                    if (value.Length > 5)
                        throw new Exception("CELL长度超过5");
                    cell = value;
                }
            }
            public string OPERATOR
            {
                get
                {
                    return operatordata;
                }
                set
                {
                    if (value.Length > 10)
                        throw new Exception("OPERATOR长度超过10");
                    operatordata = value;
                }
            }
            public string ERROR_CODE
            {
                get
                {
                    return errorcode;
                }
                set
                {
                    if (value.Length > 60)
                        throw new Exception("ERROR_CODE长度超过60");
                    errorcode = value;
                }
            }
          
        }

    }
}
