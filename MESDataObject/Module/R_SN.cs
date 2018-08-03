using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Reflection;
using System.Data.OleDb;


namespace MESDataObject.Module
{
    public class T_R_SN : DataObjectTable
    {
        public T_R_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN);
            TableName = "r_sn".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckSNExists(string StrSN, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"select * from r_sn where sn='{StrSN}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public R_SN LoadSN(string StrSN, OleExec DB)
        {
            R_SN R_Sn = null;
            Row_R_SN Row_R_Sn = (Row_R_SN)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = $@"select * from r_sn where sn='{StrSN}' and valid_flag='1'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                Row_R_Sn.loadData(Dt.Rows[0]);
                R_Sn=Row_R_Sn.GetDataObject();
            }

            return R_Sn;
        }

        public List<R_SN> GETSN(string _WO, OleExec DB)
        {

            string strSql = string.Empty;
            DataTable dt = new DataTable();
            List<R_SN> SNInfolsit = new List<R_SN>();
            try
            {
                if (DBType == DB_TYPE_ENUM.Oracle)
                {
                    strSql = $@" select * from r_sn where workorderno='{_WO.Replace("'", "''")}'";
                    dt = DB.ExecSelect(strSql).Tables[0];
                    if (dt.Rows.Count != 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            SNInfolsit.Add(new R_SN
                            {
                                ID = item["ID"].ToString(),
                                SN = item["SN"].ToString(),
                                SKUNO = item["SKUNO"].ToString(),
                                WORKORDERNO = item["WORKORDERNO"].ToString(),
                                PLANT = item["PLANT"].ToString(),
                                ROUTE_ID = item["ROUTE_ID"].ToString(),
                                STARTED_FLAG = item["STARTED_FLAG"].ToString(),
                                // START_TIME = Convert.ToDateTime(item["START_TIME"]),
                                START_TIME = item["START_TIME"].ToString()==""?null:((DateTime?)item["START_TIME"]),
                                PACKED_FLAG = item["PACKED_FLAG"].ToString(),
                                PACKDATE = item["PACKDATE"].ToString() == "" ? null : ((DateTime?)item["PACKDATE"]),
                                COMPLETED_FLAG = item["COMPLETED_FLAG"].ToString(),
                                COMPLETED_TIME = item["COMPLETED_TIME"].ToString() == "" ? null : ((DateTime?)item["COMPLETED_TIME"]),
                                SHIPPED_FLAG = item["SHIPPED_FLAG"].ToString(),
                                SHIPDATE = item["SHIPDATE"].ToString() == "" ? null : ((DateTime?)item["SHIPDATE"]),
                                REPAIR_FAILED_FLAG = item["REPAIR_FAILED_FLAG"].ToString(),
                                CURRENT_STATION = item["CURRENT_STATION"].ToString(),
                                NEXT_STATION = item["NEXT_STATION"].ToString(),
                                KP_LIST_ID = item["KP_LIST_ID"].ToString(),
                                PO_NO = item["PO_NO"].ToString(),
                                CUST_ORDER_NO = item["CUST_ORDER_NO"].ToString(),
                                CUST_PN = item["CUST_PN"].ToString(),
                                BOXSN = item["BOXSN"].ToString(),
                                SCRAPED_FLAG = item["SCRAPED_FLAG"].ToString(),
                                SCRAPED_TIME = item["SCRAPED_TIME"].ToString() == "" ? null : ((DateTime?)item["SCRAPED_TIME"]),
                                PRODUCT_STATUS = item["PRODUCT_STATUS"].ToString(),
                                REWORK_COUNT = Convert.ToDouble(item["REWORK_COUNT"].ToString()==""?"0": item["REWORK_COUNT"].ToString()),
                                VALID_FLAG = item["VALID_FLAG"].ToString(),
                                STOCK_STATUS=item["STOCK_STATUS"].ToString(),
                                STOCK_IN_TIME=item["STOCK_IN_TIME"].ToString()==""?null:((DateTime?)item["STOCK_IN_TIME"]),
                                EDIT_EMP = item["EDIT_EMP"].ToString(),
                                EDIT_TIME = item["EDIT_TIME"].ToString() == "" ? null : ((DateTime?)item["EDIT_TIME"]),

                            });
                        }
                    }
                    return SNInfolsit;
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
            }
            catch (Exception )
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            
        }

        #region add by champion

        /// <summary>
        /// 獲取數據庫時間
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DateTime GetDBDateTime(OleExec DB)
        {
            string strSql = "select sysdate from dual";
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = "select sysdate from dual";
            }
            else if (this.DBType == DB_TYPE_ENUM.SqlServer)
            {
                strSql = "select get_date() ";
            }
            else
            {
                throw new Exception(this.DBType.ToString() + " not Work");
            }
            return (DateTime)DB.ExecSelectOneValue(strSql);

        }

        /// <summary>
        /// 批量插入 R_SN 中，針對SN投入和Panel 投入，如果Panel 投入的話，SN 實例的 SN 屬性是沒有值的，因此以 ID 代替
        /// 返回插入的 ID
        /// </summary>
        /// <param name="SNs"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<string> AddToRSn(List<R_SN> SNs,string Line,string StationName,string DeviceName, string Bu, OleExec DB)
        {
            string sql = string.Empty;
            string result = string.Empty;
            T_R_SN Table_R_SN = new T_R_SN(DB, DBType); //add by LLF 2018-03-19
            Row_R_SN row = null;
            List<string> SNIds = new List<string>();
            bool ModifyFlag = false;
            DateTime DateTime = GetDBDateTime(DB);
            string NextStation = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                NextStation = GetNextStation(SNs.ElementAt(0).ROUTE_ID, SNs.ElementAt(0).CURRENT_STATION, DB);
                foreach (R_SN SN in SNs)
                {
                    if (SN.ID != null && SN.SN.Equals(SN.ID))
                    {
                        ModifyFlag = true;
                    }
                    //Modify by LLF 2018-03-19,獲取ID,需根據表名生成
                    //SN.ID = GetNewID(Bu, DB); 
                    SN.ID = Table_R_SN.GetNewID(Bu, DB);
                    SN.START_TIME = DateTime;
                    SN.EDIT_TIME = DateTime;
                    SN.NEXT_STATION = NextStation;
                    if (string.IsNullOrEmpty(SN.SN) || ModifyFlag)
                    {
                        SN.SN = SN.ID;
                    }
                    row = (Row_R_SN)this.ConstructRow(SN);
                    sql = row.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                    RecordPassStationDetail(SN.SN, Line, StationName, DeviceName, Bu, DB);
                    SNIds.Add(SN.ID);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return SNIds;
        }

        /// <summary>
        /// 記錄良率
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="SerialNo"></param>
        /// <param name="Status"></param>
        /// <param name="Day"></param>
        /// <param name="Time"></param>
        /// <param name="Line"></param>
        /// <param name="Station"></param>
        /// <param name="EmpNo"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string RecordYieldRate(string WorkOrder, double LinkQty, string SerialNo, string Status,
                                        string Line, string Station, string EmpNo, string Bu, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            bool Reworked = false;
            bool Passed = false;
            T_R_YIELD_RATE_DETAIL YieldRateTable = null;
            Row_R_YIELD_RATE_DETAIL YieldRateRow = null;
            T_R_WO_BASE WoTable = null;
            Row_R_WO_BASE WoRow = null;
            DateTime DateTime = GetDBDateTime(DB);
            string Day = DateTime.ToString("yyyy-MM-dd");
            string Time = DateTime.ToString("HH");

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Reworked = WorkTimes(SerialNo, Station, DB) > 0 ? true: false;
                Passed = Status.ToUpper().Equals("PASS");
                YieldRateTable = new T_R_YIELD_RATE_DETAIL(DB, this.DBType);
                YieldRateRow = (Row_R_YIELD_RATE_DETAIL)YieldRateTable.NewRow();
                sql = $@"SELECT * FROM R_YIELD_RATE_DETAIL WHERE WORK_DATE='{Day}' AND WORK_TIME='{Time}' AND PRODUCTION_LINE='{Line}'
                            AND STATION_NAME='{Station}' AND WORKORDERNO='{WorkOrder}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    YieldRateRow.loadData(dt.Rows[0]);
                    if (Reworked)
                    {
                        YieldRateRow.TOTAL_REWORK_BUILD_QTY += LinkQty;
                        if (Passed)
                        {
                            YieldRateRow.TOTAL_REWORK_PASS_QTY += LinkQty;
                        }
                        else
                        {
                            YieldRateRow.TOTAL_REWORK_FAIL_QTY += LinkQty;
                        }
                    }
                    else
                    {
                        YieldRateRow.TOTAL_FRESH_BUILD_QTY += LinkQty;
                        if (Passed)
                        {
                            YieldRateRow.TOTAL_FRESH_PASS_QTY += LinkQty;
                        }
                        else
                        {
                            YieldRateRow.TOTAL_FRESH_FAIL_QTY += LinkQty;
                        }
                    }
                    YieldRateRow.EDIT_EMP = EmpNo;
                    YieldRateRow.EDIT_TIME = DateTime;
                    sql = YieldRateRow.GetUpdateString(this.DBType);
                }
                else
                {
                    WoTable = new T_R_WO_BASE(DB, this.DBType);
                    WoRow = WoTable.GetWo(WorkOrder, DB);
                    YieldRateRow.ID = YieldRateTable.GetNewID(Bu, DB);
                    YieldRateRow.WORK_DATE = Convert.ToDateTime(Day);
                    YieldRateRow.WORK_TIME = Time;
                    YieldRateRow.PRODUCTION_LINE = Line;
                    YieldRateRow.CLASS_NAME = GetWorkClass(DB);
                    YieldRateRow.STATION_NAME = Station;
                    YieldRateRow.WORKORDERNO = WorkOrder;
                    YieldRateRow.SKUNO = WoRow.SKUNO;
                    YieldRateRow.SKU_NAME = WoRow.SKU_NAME;
                    YieldRateRow.SKU_SERIES = WoRow.SKU_SERIES;
                    YieldRateRow.EDIT_EMP = EmpNo;
                    YieldRateRow.EDIT_TIME = DateTime;
                    YieldRateRow.TOTAL_FRESH_BUILD_QTY = YieldRateRow.TOTAL_FRESH_FAIL_QTY = YieldRateRow.TOTAL_FRESH_PASS_QTY = 0;
                    YieldRateRow.TOTAL_REWORK_BUILD_QTY = YieldRateRow.TOTAL_REWORK_FAIL_QTY = YieldRateRow.TOTAL_REWORK_PASS_QTY = 0;

                    if (Reworked)
                    {
                        YieldRateRow.TOTAL_REWORK_BUILD_QTY = LinkQty;
                        if (Passed)
                        {
                            YieldRateRow.TOTAL_REWORK_PASS_QTY = LinkQty;
                        }
                        else
                        {
                            YieldRateRow.TOTAL_REWORK_FAIL_QTY = LinkQty;
                        }
                    }
                    else
                    {
                        YieldRateRow.TOTAL_FRESH_BUILD_QTY = LinkQty;
                        if (Passed)
                        {
                            YieldRateRow.TOTAL_FRESH_PASS_QTY = LinkQty;
                        }
                        else
                        {
                            YieldRateRow.TOTAL_FRESH_FAIL_QTY = LinkQty;
                        }
                    }
                    sql = YieldRateRow.GetInsertString(this.DBType);

                }

                result = DB.ExecSQL(sql);
                return result;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        /// <summary>
        /// 記錄 UPH
        /// </summary>
        /// <param name="WorkOrder"></param>
        /// <param name="UnitType"></param>
        /// <param name="SerialNo"></param>
        /// <param name="Status"></param>
        /// <param name="Day"></param>
        /// <param name="Time"></param>
        /// <param name="Line"></param>
        /// <param name="Station"></param>
        /// <param name="EmpNo"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string RecordUPH(string WorkOrder, double LinkQty, string SerialNo, string Status,
                                        string Line, string Station, string EmpNo, string Bu, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            bool Reworked = false;
            bool Passed = false;
            T_R_UPH_DETAIL UPHTable = null;
            Row_R_UPH_DETAIL UPHRow = null;
            T_R_WO_BASE WoTable = null;
            Row_R_WO_BASE WoRow = null;
            DateTime DateTime = GetDBDateTime(DB);
            string Day = DateTime.ToString("yyyy-MM-dd");
            string Time = DateTime.ToString("HH");

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Reworked = WorkTimes(SerialNo, Station, DB) > 1 ? true:false;
                Passed = Status.ToUpper().Equals("PASS");
                UPHTable = new T_R_UPH_DETAIL(DB, this.DBType);
                UPHRow = (Row_R_UPH_DETAIL)UPHTable.NewRow();
                sql = $@"SELECT * FROM R_UPH_DETAIL WHERE WORK_DATE='{Day}' AND WORK_TIME='{Time}' AND PRODUCTION_LINE='{Line}'
                            AND STATION_NAME='{Station}' AND WORKORDERNO='{WorkOrder}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    UPHRow.loadData(dt.Rows[0]);
                    if (Reworked)
                    {
                        UPHRow.TOTAL_REWORK_BUILD_QTY += LinkQty;
                        if (Passed)
                        {
                            UPHRow.TOTAL_REWORK_PASS_QTY += LinkQty;
                        }
                        else
                        {
                            UPHRow.TOTAL_REWORK_FAIL_QTY += LinkQty;
                        }
                    }
                    else
                    {
                        UPHRow.TOTAL_FRESH_BUILD_QTY += LinkQty;
                        if (Passed)
                        {
                            UPHRow.TOTAL_FRESH_PASS_QTY += LinkQty;
                        }
                        else
                        {
                            UPHRow.TOTAL_FRESH_FAIL_QTY += LinkQty;
                        }
                    }
                    UPHRow.EDIT_EMP = EmpNo;
                    UPHRow.EDIT_TIME = DateTime;
                    sql = UPHRow.GetUpdateString(this.DBType);
                }
                else
                {
                    WoTable = new T_R_WO_BASE(DB, this.DBType);
                    WoRow = WoTable.GetWo(WorkOrder, DB);
                    UPHRow.ID = UPHTable.GetNewID(Bu, DB);
                    UPHRow.WORK_DATE = Convert.ToDateTime(Day);
                    UPHRow.WORK_TIME = Time;
                    UPHRow.PRODUCTION_LINE = Line;
                    UPHRow.CLASS_NAME = GetWorkClass(DB);
                    UPHRow.STATION_NAME = Station;
                    UPHRow.WORKORDERNO = WorkOrder;
                    UPHRow.SKUNO = WoRow.SKUNO;
                    UPHRow.SKU_NAME = WoRow.SKU_NAME;
                    UPHRow.SKU_SERIES = WoRow.SKU_SERIES;
                    UPHRow.EDIT_EMP = EmpNo;
                    UPHRow.EDIT_TIME = DateTime;
                    UPHRow.TOTAL_FRESH_BUILD_QTY = UPHRow.TOTAL_FRESH_FAIL_QTY = UPHRow.TOTAL_FRESH_PASS_QTY = 0;
                    UPHRow.TOTAL_REWORK_BUILD_QTY = UPHRow.TOTAL_REWORK_FAIL_QTY = UPHRow.TOTAL_REWORK_PASS_QTY = 0;

                    if (Reworked)
                    {
                        UPHRow.TOTAL_REWORK_BUILD_QTY = LinkQty;
                        if (Passed)
                        {
                            UPHRow.TOTAL_REWORK_PASS_QTY = LinkQty;
                        }
                        else
                        {
                            UPHRow.TOTAL_REWORK_FAIL_QTY = LinkQty;
                        }
                    }
                    else
                    {
                        UPHRow.TOTAL_FRESH_BUILD_QTY = LinkQty;
                        if (Passed)
                        {
                            UPHRow.TOTAL_FRESH_PASS_QTY = LinkQty;
                        }
                        else
                        {
                            UPHRow.TOTAL_FRESH_FAIL_QTY = LinkQty;
                        }
                    }
                    sql = UPHRow.GetInsertString(this.DBType);

                }

                result = DB.ExecSQL(sql);
                return result;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 獲取當前數據庫時間所屬的班別
        /// </summary>
        /// <param name="DateTime"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetWorkClass(OleExec DB)
        {
            string TimeFormat = "HH24:MI:SS";
            DataTable dt = new DataTable();
            string sql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM C_WORK_CLASS WHERE TO_DATE(TO_CHAR(SYSDATE,'{TimeFormat}'),'{TimeFormat}')
                            BETWEEN TO_DATE(START_TIME,'{TimeFormat}') AND TO_DATE(END_TIME,'{TimeFormat}')";

                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["NAME"] != null)
                    {
                        return dt.Rows[0]["NAME"].ToString();
                    }
                    else
                    {
                        throw new Exception("班別的名字不能為空");
                    }
                }
                else
                {
                    //如果上面的沒有結果，表示某一條數據的 END_TIME 是第二天的時間，那麼那一條的 START_TIME 肯定是所有數據中最大的
                    sql = "SELECT * FROM C_WORK_CLASS ORDER BY START_TIME DESC";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0]["NAME"].ToString();
                    }
                    else
                    {
                        throw new Exception("沒有配置班別");
                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        /// <summary>
        /// 判斷該 SN 過了多少次這個工站
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int WorkTimes(string SerialNo, string Station, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            int TotalReworkTimes = 0;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_SN_STATION_DETAIL WHERE SN='{SerialNo}' AND STATION_NAME='{Station}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    TotalReworkTimes = dt.Rows.Count;
                }
                return TotalReworkTimes;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 獲取下一站
        /// </summary>
        /// <param name="RouteId"></param>
        /// <param name="CurrentStation"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetNextStation(string RouteId, string CurrentStation, OleExec DB)
        {
            T_C_ROUTE_DETAIL RouteDetailTable = null;
            List<C_ROUTE_DETAIL> RouteDetails = null;
            Dictionary<string, object> Routes = null;
            string NextStation = string.Empty;
            int Counter = 0;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (CurrentStation.Equals("JOBFINISH"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000090",new string[] { CurrentStation}));
                }
                RouteDetailTable = new T_C_ROUTE_DETAIL(DB, this.DBType);
                RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(RouteId, DB);
                Routes = RouteDetailTable.GetNextStations(RouteId, CurrentStation, DB);
                Counter = ((List<string>)Routes["NextStations"]).Count + ((List<string>)Routes["DirectLinks"]).Count;
                if (Counter == 0)
                {
                    //黃楊盛 2018年4月24日14:54:17 沒有跳站的情況下更新為使用最後一個的station_type
                    //NextStation = "NA_SHIP";
                    NextStation = RouteDetails[RouteDetails.Count-1].STATION_TYPE;
                }
                else if (((List<string>)Routes["NextStations"]).Count > 0)
                {
                    NextStation = ((List<string>)Routes["NextStations"]).ElementAt(0).ToString();
                }
                else if (((List<string>)Routes["DirectLinks"]).Count > 0)
                {
                    NextStation = ((List<string>)Routes["DirectLinks"]).ElementAt(0).ToString();
                }
                //else if (Counter == 1 && ((List<string>)Routes["NextStations"]).Count > 0)
                //{
                //    NextStation = ((List<string>)Routes["NextStations"]).ElementAt(0).ToString();
                //}
                //else if (Counter == 1 && ((List<string>)Routes["DirectLinks"]).Count > 0)
                //{
                //    NextStation = ((List<string>)Routes["DirectLinks"]).ElementAt(0).ToString();
                //}
                //else
                //{
                //    NextStation = "NA";
                //}
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return NextStation;
        }

        /// <summary>
        /// 獲取流程中的最後一站
        /// </summary>
        /// <param name="RouteId"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetLastStation(string RouteId,OleExec DB)
        {
            string LastStation = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                //sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND STATION_TYPE='JOBFINISHED'";
                sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND instr(STATION_TYPE,'JOBFINISH')>0";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    LastStation = dt.Rows[0]["STATION_NAME"].ToString();
                }
                return LastStation;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 更新 SN 狀態，用於 SN 過站
        /// </summary>
        /// <param name="SnObject"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void ChangeSnStatus(ref R_SN SnObject, string PassOrFail,string EmpNo,OleExec DB)
        {
            bool ReworkFlag = false;
            bool RepairFlag = false;
            bool PackedFlag = false;
            bool FinishedFlag = false;
            bool ShippedFlag = false;
            int MaxReworkCount = 0; // 保存設定的最大重工次數
            int HadReworkedTimes = 0;
            string OriginalNextStation = string.Empty;
            string NextStation = string.Empty;
            T_R_WO_BASE WoBase = new T_R_WO_BASE(DB, this.DBType);

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (SnObject != null)
                {
                    OriginalNextStation = SnObject.NEXT_STATION.ToUpper();
                    //MaxReworkCount 還需要獲取維修的最大次數
                    RepairFlag = PassOrFail.ToUpper().Equals("PASS") ? false : MaxReworkCount - SnObject.REWORK_COUNT == 1;
                    HadReworkedTimes = WorkTimes(SnObject.SN, SnObject.NEXT_STATION, DB);
                    ReworkFlag = HadReworkedTimes > 0 ? true : false;
                    PackedFlag = OriginalNextStation.Contains("PACK")
                            || OriginalNextStation.Contains("CTN")
                            || OriginalNextStation.Contains("PALT") ? true : false;
                    FinishedFlag = OriginalNextStation.Equals(GetLastStation(SnObject.ROUTE_ID, DB)) ? true : false;
                    //ShippedFlag 還需要判斷

                    if (RepairFlag)
                    {
                        NextStation = "Repair_" + SnObject.NEXT_STATION;
                        SnObject.REPAIR_FAILED_FLAG = "1";
                        SnObject.NEXT_STATION = NextStation;
                    }
                    else
                    {
                        if (FinishedFlag)
                        {
                            SnObject.COMPLETED_FLAG = "1";
                            SnObject.COMPLETED_TIME = GetDBDateTime(DB);
                            SnObject.CURRENT_STATION = OriginalNextStation;
                            SnObject.NEXT_STATION = "JOBFINISH";
                            WoBase.UpdateFinishQty(SnObject.WORKORDERNO, 1, DB);
                        }
                        else
                        {
                            NextStation = GetNextStation(SnObject.ROUTE_ID, SnObject.NEXT_STATION.Replace("Repair_", ""), DB);
                            SnObject.CURRENT_STATION = OriginalNextStation;
                            SnObject.NEXT_STATION = NextStation;

                            if (PackedFlag)
                            {
                                SnObject.PACKED_FLAG = "1";
                                SnObject.PACKDATE = GetDBDateTime(DB);
                            }
                            if (ReworkFlag)
                            {
                                SnObject.REWORK_COUNT = HadReworkedTimes++;
                                SnObject.PRODUCT_STATUS = "REWORK";
                            }
                            //if (ShippedFlag)
                            //{
                            //    SnObject.SHIPPED_FLAG = "1";
                            //    SnObject.SHIPDATE = GetDBDateTime(DB);
                            //}
                        }
                    }
                    SnObject.EDIT_TIME = GetDBDateTime(DB);
                    SnObject.EDIT_EMP = EmpNo;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// SN 過站
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="DB"></param>
        public void PassStation(string SerialNo,string Line,string StationName,string DeviceName,string Bu,string PassOrFail,string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            Row_R_SN SnRow = (Row_R_SN)NewRow();
            T_R_SN_STATION_DETAIL SnStationDetailTable = new T_R_SN_STATION_DETAIL(DB, this.DBType);
            DataTable dt = new DataTable();
            List<string> SNs = new List<string>();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                SNs.Add(SerialNo);
                LotsPassStation(SNs,Line,StationName,DeviceName,Bu, PassOrFail, EmpNo, DB);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        /// <summary>
        /// 多個 SN 批量過站，需要處於相同狀態
        /// </summary>
        /// <param name="SNs"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void LotsPassStation(List<string> SNs, string Line,string StationName,string DeviceName, string Bu,string PassOrFail, string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            R_SN TemplateSNObject = null;
            Row_R_SN row = (Row_R_SN)NewRow();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (SNs.Count > 0)
                {
                    foreach (string SN in SNs)
                    {
                        sql = $@"SELECT * FROM R_SN WHERE SN='{SN}' AND VALID_FLAG='1'"; //表示當前有效的 SN
                        dt = DB.ExecSelect(sql).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            row.loadData(dt.Rows[0]);
                            TemplateSNObject = row.GetDataObject();
                            ChangeSnStatus(ref TemplateSNObject, PassOrFail, EmpNo, DB);
                            row.ConstructRow(TemplateSNObject);
                            sql = row.GetUpdateString(this.DBType);
                            DB.ExecSQL(sql);
                            RecordPassStationDetail(row.SN, Line, StationName, DeviceName, Bu, DB);
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048",new string[] { SN}));
                        }

                    }
                    
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 多個 SN 批量過站，需要處於相同狀態
        /// </summary>
        /// <param name="SNs"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void LotsPassStation(List<R_SN> SNs, string Line, string StationName, string DeviceName, string Bu, string PassOrFail, string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            R_SN TemplateSNObject = null;
            Row_R_SN row = (Row_R_SN)NewRow();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (SNs.Count > 0)
                {
                    foreach (R_SN snobj in SNs)
                    {
                        sql = $@"SELECT * FROM R_SN WHERE SN='{snobj.SN}' AND VALID_FLAG='1'"; //表示當前有效的 SN
                        dt = DB.ExecSelect(sql).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            row.loadData(dt.Rows[0]);
                            TemplateSNObject = row.GetDataObject();
                            ChangeSnStatus(ref TemplateSNObject, PassOrFail, EmpNo, DB);
                            row.ConstructRow(TemplateSNObject);
                            sql = row.GetUpdateString(this.DBType);
                            DB.ExecSQL(sql);
                            RecordPassStationDetail(row.SN, Line, StationName, DeviceName, Bu, DB);
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { snobj.SN }));
                        }
                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// Panel 過站
        /// </summary>
        /// <param name="PanelSN"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void PanelPassStation(string PanelSN,string Line,string StationName,string DeviceName,string Bu, string PassOrFail,string EmpNo, OleExec DB)
        {
            string sql = string.Empty;
            List<string> PanelSNs = new List<string>();
            DataTable dt = new DataTable();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT A.* FROM R_SN A JOIN R_PANEL_SN B ON A.ID=B.SN WHERE B.PANEL='{PanelSN}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        PanelSNs.Add(dr["SN"].ToString());
                    }
                    LotsPassStation(PanelSNs,Line,StationName,DeviceName,Bu, PassOrFail, EmpNo, DB);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000089",new string[] { PanelSN}));
                }

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 分板過站
        /// 先確定 Panel 中是否還有未替換的 SN
        /// 接著從 R_SN 中找當前第一筆並且替換掉 R_SN 中的 SN，然後調用 SN 過站
        /// </summary>
        /// <param name="PanelSN"></param>
        /// <param name="SerialNo"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="DB"></param>
        public void SplitsPassStation(string PanelSN,string Line,string StationName,string DeviceName,string Bu, string SerialNo, string PassOrFail, string EmpNo, OleExec DB, OleExec APDB,R_PANEL_SN PANELObj)
        {
            string sql = string.Empty;
            AP_DLL APObj = new AP_DLL();
            string APVirtualSn = "";
           
            DataTable dt = new DataTable();
            Row_R_SN SnRow = (Row_R_SN)NewRow();
            string PanelId = string.Empty;
            string GetSnID = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_PANEL_SN WHERE PANEL='{PanelSN}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    //從 Panel 中找到還沒有被分配實際 SN 的記錄
                  //  sql = $@"SELECT A.ID FROM R_PANEL_SN A,R_SN B WHERE A.PANEL='{PanelSN}' AND A.SN=B.SN AND B.SN=B.ID  ";
                    sql = $@"SELECT A.* FROM R_PANEL_SN A,R_SN B WHERE A.PANEL='{PanelSN}' AND A.SN=B.SN AND B.SN=B.ID ORDER BY A.SEQ_NO ";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        PanelId = dt.Rows[0]["ID"].ToString();
                        GetSnID = dt.Rows[0]["SN"].ToString();
                        //   sql = $@"SELECT * FROM R_SN WHERE SN IN (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelSN}') AND ID=SN";
                        sql = $@"SELECT * FROM R_SN WHERE ID ='{GetSnID}'";
                        dt = DB.ExecSelect(sql).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            try
                            {
                                SnRow.loadData(dt.Rows[0]);
                                SnRow.SN = SerialNo;
                                SnRow.EDIT_TIME = GetDBDateTime(DB);
                                SnRow.EDIT_EMP = EmpNo;
                                sql = SnRow.GetUpdateString(this.DBType);
                                DB.ExecSQL(sql);

                                // Update R_PANEL_SN,add by LLF 2018-02-06
                                T_R_PANEL_SN RPanelSN = new T_R_PANEL_SN(DB, this.DBType);
                                Row_R_PANEL_SN Row_Panel = (Row_R_PANEL_SN)RPanelSN.NewRow();
                                Row_Panel = (Row_R_PANEL_SN)RPanelSN.GetObjByID(PanelId, DB);
                                Row_Panel.SN = SerialNo;
                                Row_Panel.EDIT_EMP = EmpNo;
                                Row_Panel.EDIT_TIME = GetDBDateTime(DB);
                                DB.ExecSQL(Row_Panel.GetUpdateString(this.DBType));

                                //Update r_sn_kp add by fgg 2018.05.23
                                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(DB, this.DBType);
                                t_r_sn_kp.UpdateSNBySnId(SnRow.ID, SnRow.SN, EmpNo, DB);

                                //Update AP 
                                //OleExecPool APDBPool = Station.DBS["APDB"];

                                APVirtualSn = PANELObj.PANEL + "0" + PANELObj.SEQ_NO.ToString();
                                string result = APObj.APUpdatePanlSN(APVirtualSn, SerialNo, APDB);

                                if (!result.Equals("OK"))
                                {
                                    throw new MESReturnMessage("already be binded to other serial number");
                                }

                                //update R_SN_STATION_DETAIL 
                                T_R_SN_STATION_DETAIL RSnStationDetail = new T_R_SN_STATION_DETAIL(DB, this.DBType);
                                RSnStationDetail.UpdateRSnStationDetailBySNID(SerialNo, dt.Rows[0]["ID"].ToString(), DB);

                                PassStation(SerialNo, Line, StationName, DeviceName, Bu, PassOrFail, EmpNo, DB); //調用 SN 過站

                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000088", new string[] { PanelSN }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000149", new string[] { PanelSN }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000038",new string[] { PanelSN}));
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// 補 Allpart 資料
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="ProductionLine"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string AddAPRecords(string SerialNo,string Wo,string StationName,string ProductionLine,OleExec DB)
        {
            //Modify by LLF 2017-01-27 
            string result = string.Empty;
            //R_SN SnObject = null;
            //T_R_PANEL_SN table = null;
            //R_PANEL_SN PanelObject = null;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] parameters = new OleDbParameter[5];
                parameters[0] = new OleDbParameter("VAR_PANELNO", SerialNo);
                parameters[0].OleDbType = OleDbType.VarChar;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = new OleDbParameter("VAR_WORKORDERNO", Wo);
                parameters[1].OleDbType = OleDbType.VarChar;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = new OleDbParameter("VAR_PRODUCTIONLINE", ProductionLine);
                parameters[2].OleDbType = OleDbType.VarChar;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = new OleDbParameter("VAR_NEXTEVENT", StationName);
                parameters[3].OleDbType = OleDbType.VarChar;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = new OleDbParameter("VAR_MESSAGE", OleDbType.VarChar);
                parameters[4].Direction = ParameterDirection.Output;
                parameters[4].Size = 200;

                result = DB.ExecProcedureNoReturn("MES1.CMC_INSERTDATA_SP", parameters);

                return result;
                //SnObject = GetDetailBySN(SerialNo, DB);
                //if (SnObject != null)
                //{
                //table = new T_R_PANEL_SN(DB, this.DBType);
                //PanelObject = table.GetPanelBySn(SerialNo, DB);
                // if (PanelObject != null)
                //{
                //OleDbParameter[] parameters = new OleDbParameter[5];
                //parameters[0] = new OleDbParameter("VAR_PANELNO", PanelObject.PANEL);
                //parameters[0].OleDbType = OleDbType.VarChar;
                //parameters[0].Direction = ParameterDirection.Input;

                //parameters[1] = new OleDbParameter("VAR_WORKORDERNO", SnObject.WORKORDERNO);
                //parameters[1].OleDbType = OleDbType.VarChar;
                //parameters[1].Direction = ParameterDirection.Input;

                //parameters[2] = new OleDbParameter("VAR_PRODUCTIONLINE", ProductionLine);
                //parameters[2].OleDbType = OleDbType.VarChar;
                //parameters[2].Direction = ParameterDirection.Input;

                //parameters[3] = new OleDbParameter("VAR_NEXTEVENT", SnObject.NEXT_STATION);
                //parameters[3].OleDbType = OleDbType.VarChar;
                //parameters[3].Direction = ParameterDirection.Input;

                //parameters[4] = new OleDbParameter("VAR_MESSAGE", OleDbType.VarChar);
                //parameters[4].Direction = ParameterDirection.Output;

                //result = DB.ExecProcedureNoReturn("MES1.CMC_INSERTDATA_SP", parameters);

                //return result;
                //}
                //else
                //{
                //   throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000049", new string[] { SerialNo }));
                //}
            }
            //    else
            //    {
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SerialNo }));
            //    }
            //}
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        //Add by LLF 2018-01-26 begin
        public string GetAPTrCode(string WorkOrderNo, string TrSN, string Process,string EmpNo, string MacAddress, OleExec DB)
        {
            string result = string.Empty;
            Dictionary<string, object> GetTRCodeDic = null;
            string TRCode = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] ParasForGetTRCode = new OleDbParameter[7];

                ParasForGetTRCode[0] = new OleDbParameter("G_TRSN", TrSN);
                ParasForGetTRCode[0].Direction = ParameterDirection.Input;

                ParasForGetTRCode[1] = new OleDbParameter("G_WO", WorkOrderNo);
                ParasForGetTRCode[1].Direction = ParameterDirection.Input;

                ParasForGetTRCode[2] = new OleDbParameter("MAC_ADDRESS", MacAddress);
                ParasForGetTRCode[2].Direction = ParameterDirection.Input;

                ParasForGetTRCode[3] = new OleDbParameter("G_EMP_NO", EmpNo);
                ParasForGetTRCode[3].Direction = ParameterDirection.Input;

                ParasForGetTRCode[4] = new OleDbParameter("G_PROCESS", Process);
                ParasForGetTRCode[4].Direction = ParameterDirection.Input;

                ParasForGetTRCode[5] = new OleDbParameter("V_TRCODE", OleDbType.VarChar);
                ParasForGetTRCode[5].Direction = ParameterDirection.Output;
                ParasForGetTRCode[5].Size = 2000;

                ParasForGetTRCode[6] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForGetTRCode[6].Direction = ParameterDirection.Output;
                ParasForGetTRCode[6].Size = 2000;

                GetTRCodeDic = DB.ExecProcedureReturnDic("MES1.GET_TRCODE", ParasForGetTRCode);
                TRCode = GetTRCodeDic["V_TRCODE"].ToString();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return TRCode;
        }



        public string addStartSNRecords(string workorderno, string trsn, OleExec DB) {
            T_R_SN SnDetailTable = new T_R_SN(DB, this.DBType);
            Row_R_SN r_sn = (Row_R_SN)SnDetailTable.NewRow();
            string result = string.Empty;
            r_sn.ID = SnDetailTable.GetNewID("TJ", DB);
            r_sn.SN = trsn;
            r_sn.EDIT_TIME = DateTime.Now;
            r_sn.WORKORDERNO = workorderno;
            string sql = r_sn.GetInsertString(this.DBType);
            result = DB.ExecSQL(sql);

            return result;
        }


        //Add by LLF 2018-01-26 End

        /// <summary>
        /// 補 SMTLOADING 資料
        /// </summary>
        /// <param name="PanelSn"></param>
        /// <param name="TrSN"></param>
        /// <param name="WorkOrderNo"></param>
        /// <param name="Process"></param>
        /// <param name="EmpNo"></param>
        /// <param name="MacAddress"></param>
        /// <param name="LinkQty"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string AddSMTLoadingRecords(string WorkOrderNo, string PanelSn,string TrSN,string Process,double LinkQty, string EmpNo, string MacAddress,string TrCode, OleExec DB)
        {
            string result = string.Empty;
            Dictionary<string, object> GetTRCodeDic = null;
            Dictionary<string, object> InsertSnLinkDic = null;
            //string TRCode = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                //OleDbParameter[] ParasForGetTRCode = new OleDbParameter[7];
                OleDbParameter[] ParasForInsert = new OleDbParameter[11];

                //ParasForGetTRCode[0] = new OleDbParameter("G_TRSN", TrSN);
                //ParasForGetTRCode[0].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[1] = new OleDbParameter("G_WO", WorkOrderNo);
                //ParasForGetTRCode[1].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[2] = new OleDbParameter("MAC_ADDRESS", MacAddress);
                //ParasForGetTRCode[2].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[3] = new OleDbParameter("G_EMP_NO", EmpNo);
                //ParasForGetTRCode[3].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[4] = new OleDbParameter("G_PROCESS", Process);
                //ParasForGetTRCode[4].Direction = ParameterDirection.Input;

                //ParasForGetTRCode[5] = new OleDbParameter("V_TRCODE", OleDbType.VarChar);
                //ParasForGetTRCode[5].Direction = ParameterDirection.Output;
                //ParasForGetTRCode[5].Size = 2000;

                //ParasForGetTRCode[6] = new OleDbParameter("RES", OleDbType.VarChar);
                //ParasForGetTRCode[6].Direction = ParameterDirection.Output;
                //ParasForGetTRCode[6].Size = 2000;

                //GetTRCodeDic = DB.ExecProcedureReturnDic("MES1.GET_TRCODE", ParasForGetTRCode);
                //TRCode = GetTRCodeDic["V_TRCODE"].ToString();

                ParasForInsert[0] = new OleDbParameter("G_TRSN", TrSN);
                ParasForInsert[0].Direction = ParameterDirection.Input;

                ParasForInsert[1] = new OleDbParameter("G_WO", WorkOrderNo);
                ParasForInsert[1].Direction = ParameterDirection.Input;

                ParasForInsert[2] = new OleDbParameter("MAC_ADDRESS", MacAddress);
                ParasForInsert[2].Direction = ParameterDirection.Input;

                ParasForInsert[3] = new OleDbParameter("G_EMP_NO", EmpNo);
                ParasForInsert[3].Direction = ParameterDirection.Input;

                ParasForInsert[4] = new OleDbParameter("G_TRCODE", TrCode);
                ParasForInsert[4].Direction = ParameterDirection.Input;

                ParasForInsert[5] = new OleDbParameter("G_PANELNO", PanelSn);
                ParasForInsert[5].Direction = ParameterDirection.Input;

                ParasForInsert[6] = new OleDbParameter("G_LINK_QTY", LinkQty);
                ParasForInsert[6].OleDbType = OleDbType.Numeric;
                ParasForInsert[6].Direction = ParameterDirection.Input;

                ParasForInsert[7] = new OleDbParameter("G_PROCESS", Process);
                ParasForInsert[7].Direction = ParameterDirection.Input;

                ParasForInsert[8] = new OleDbParameter("G_FLAG", "T");
                ParasForInsert[8].Direction = ParameterDirection.Input;

                ParasForInsert[9] = new OleDbParameter("V_EXT_QTY", OleDbType.VarChar);
                ParasForInsert[9].Direction = ParameterDirection.Output;
                ParasForInsert[9].Size = 2000;

                ParasForInsert[10] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForInsert[10].Direction = ParameterDirection.Output;
                ParasForInsert[10].Size = 2000;

                InsertSnLinkDic = DB.ExecProcedureReturnDic("MES1.Z_INSERT_SN_LINK", ParasForInsert);
                result = InsertSnLinkDic["RES"].ToString();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
        
        /// <summary>
        /// 寫過站記錄
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="DB"></param>
        public string RecordPassStationDetail(string SerialNo, string Line, string StationName, string DeviceName, string Bu,OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_SN SnRow = (Row_R_SN)NewRow();
            T_R_SN_STATION_DETAIL SnDetailTable = new T_R_SN_STATION_DETAIL(DB, this.DBType);
            Row_R_SN_STATION_DETAIL SnDetailRow = (Row_R_SN_STATION_DETAIL)SnDetailTable.NewRow();
            string result = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_SN WHERE SN='{SerialNo}' AND VALID_FLAG=1";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        SnRow.loadData(dt.Rows[0]);
                        SnDetailRow.ConstructRow((R_SN)SnRow.GetDataObject());
                        SnDetailRow.ID = SnDetailTable.GetNewID(Bu, DB);
                        SnDetailRow.R_SN_ID = SnRow.ID;
                        SnDetailRow.LINE = Line;
                        SnDetailRow.REPAIR_FAILED_FLAG = "0";
                        SnDetailRow.CLASS_NAME = GetWorkClass(DB);
                        SnDetailRow.DEVICE_NAME = DeviceName;
                        SnDetailRow.STATION_NAME = StationName;
                        sql = SnDetailRow.GetInsertString(this.DBType);                        
                        result = DB.ExecSQL(sql);
                        if(Int32.Parse(result)==0)
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SerialNo }));                       
                    }
                    catch(Exception ex)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SerialNo }) + ex.Message);
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048",new string[] { SerialNo}));
                }
                
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 寫過站記錄
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="DB"></param>
        public string LinkPassStationDetail(R_SN SNObj,string WO,string Skuno,string RouteID, string Line, string StationName, string DeviceName,string EmpNO, string Bu, OleExec DB)
        {
            string sql = "";
            T_R_SN_STATION_DETAIL SnDetailTable = new T_R_SN_STATION_DETAIL(DB, this.DBType);
            Row_R_SN_STATION_DETAIL SnDetailRow = (Row_R_SN_STATION_DETAIL)SnDetailTable.NewRow();
            string result = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                SnDetailRow.ID = SnDetailTable.GetNewID(Bu, DB);
                SnDetailRow.R_SN_ID = SNObj.ID;
                SnDetailRow.SN = SNObj.SN;
                SnDetailRow.SHIPPED_FLAG = SNObj.SHIPPED_FLAG;
                SnDetailRow.SHIPDATE = SNObj.SHIPDATE;
                SnDetailRow.COMPLETED_FLAG = SNObj.COMPLETED_FLAG;
                SnDetailRow.COMPLETED_TIME = SNObj.COMPLETED_TIME;
                SnDetailRow.STARTED_FLAG = "1";
                SnDetailRow.START_TIME = SNObj.START_TIME;
                SnDetailRow.PLANT = SNObj.PLANT;
                SnDetailRow.WORKORDERNO = WO;
                SnDetailRow.SKUNO = Skuno;
                SnDetailRow.ROUTE_ID = RouteID;
                SnDetailRow.VALID_FLAG = "1";
                SnDetailRow.CURRENT_STATION = SNObj.CURRENT_STATION;
                SnDetailRow.NEXT_STATION = SNObj.NEXT_STATION;
                SnDetailRow.LINE = Line;
                SnDetailRow.REPAIR_FAILED_FLAG = "0";
                SnDetailRow.CLASS_NAME = GetWorkClass(DB);
                SnDetailRow.DEVICE_NAME = DeviceName;
                SnDetailRow.STATION_NAME = StationName;
                SnDetailRow.EDIT_EMP = EmpNO;
                SnDetailRow.EDIT_TIME = GetDBDateTime(DB);
                sql = SnDetailRow.GetInsertString(this.DBType);
                result = DB.ExecSQL(sql);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public R_SN GetDetailBySN(string sn, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            Row_R_SN row_rsn = null;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"select * from r_sn where sn='{sn.Replace("'", "''")}' and valid_flag='1'";//只取有效的sn
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    row_rsn = (Row_R_SN)this.NewRow();
                    row_rsn.loadData(dt.Rows[0]);
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { sn });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return row_rsn.GetDataObject();
        }

        ///
        public R_SN GetDetailByPanelSN(string sn, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            Row_R_SN row_rsn = null;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"select * from r_sn a,r_panel_sn b where b.panel='{sn.Replace("'", "''")}' and a.WORKORDERNO=b.WORKORDERNO and a.sn=b.sn and valid_flag='1'";//只取有效的sn
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count >= 1)
                {
                    row_rsn = (Row_R_SN)this.NewRow();
                    row_rsn.loadData(dt.Rows[0]);
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + sn });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return row_rsn.GetDataObject();
        }

        /// <summary>
        /// 判断SN是否已经使用，已经使用返回TRUE，没有使用返回FALSE
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool IsUsed(string sn, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            sql = $@"select * from r_sn where sn=:sn  and valid_flag='1'";//只取有效的sn
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":sn", sn);
            dt = db.ExecuteDataTable(sql, CommandType.Text, paramet);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判断StartSN&EndSN區間是否已经使用，已经使用返回TRUE，没有使用返回FALSE
        /// </summary>
        /// <param name="strStartSN"></param>
        /// <param name="strEndSN"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool SNRangeIsUsed(string strStartSN,string strEndSN, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            sql = $@"select * from r_sn where sn between :strStartSN and :strEndSN and valid_flag='1'";//只取有效的sn
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":strStartSN", strStartSN);
            paramet[1] = new OleDbParameter(":strEndSN", strEndSN);
            dt = db.ExecuteDataTable(sql, CommandType.Text, paramet);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //get list<r_sn> by wo .
        public List<R_SN> GetRSNbyWo(string wo, OleExec DB)
        {
            string strSql = $@"select * from r_sn where workorderno=:wo";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":wo", wo) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_SN> listSn = new List<R_SN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_SN ret = (Row_R_SN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            //else
            //{
            //    return null;
            //}
            return listSn;
        }


        /// <summary>
        /// 取到 SN VALID_FLAG=0 時間最晚的無效記錄
        /// </summary>
        /// <param name="strStartSN"></param>
        /// <param name="strEndSN"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<R_SN> GetINVaildSN(string sn,string MarkSnid, OleExec DB)
        {
          
            string strSql = $@" select* from r_sn a where a.sn=:sn  and a.valid_flag = '0' and a.edit_time in (select max(b.edit_time) from r_sn b where b.sn = a.sn and b.id<>:MarkSnid)";

            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":sn", sn);
            paramet[1] = new OleDbParameter(":MarkSnid", MarkSnid);           
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_SN> listSn = new List<R_SN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_SN ret = (Row_R_SN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            return listSn;
        }

        /// <summary>
        /// 传入一个panelSn 返回对应R_SN表里SN的集合
        /// </summary>
        /// <param name="PanelSn"></param>
        /// <param name="DB"></param>
        /// <returns>List<R_SN></returns>
        public List<R_SN> GetRSNbyPsn(string PanelSn, OleExec DB)
        {
            //Modify BY LLF 2018-01-27
            //string strSql = $@"SELECT * FROM R_SN WHERE ID IN (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelSn.Replace("'", "''")}')";
            //string strSql = $@"SELECT * FROM R_SN WHERE SN IN (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelSn.Replace("'", "''")}')";
            string strSql = $@"SELECT * FROM R_SN A WHERE EXISTS (SELECT * FROM R_PANEL_SN B WHERE A.SN=B.SN AND A.WORKORDERNO=B.WORKORDERNO AND PANEL='{PanelSn.Replace("'", "''")}')";//修復BUG－一個PANEL裡有SN已經掃LINK，其他沒過BIP的產品會導致ROUTE改變，增加WORKORDER作為條件2018-04-07 11:00 by LJD
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":PanelSn", PanelSn) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_SN> listSn = new List<R_SN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_SN ret = (Row_R_SN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }

        //Add by LLF 2018-01-27
        public List<R_SN> GetRSNbySN(string SN, OleExec DB)
        {
            string strSql = $@"SELECT * FROM R_SN WHERE SN ='{SN.Replace("'", "''")}'";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_SN> listSn = new List<R_SN>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_SN ret = (Row_R_SN)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }

        /// <summary>
        /// SN入MRB時，更新SN當前站為MRB，下一站為REWORK，completed_flag=1,completed_time=now
        /// </summary>
        /// <param name="snid"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int  SN_Mrb_Pass_action(string snid,string userno, OleExec DB)
        {
            string strSql = $@" update r_sn set current_station='MRB',next_station='REWORK',completed_flag='1',completed_time=sysdate,edit_emp=:userno,edit_time=sysdate where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":userno", userno),
                new OleDbParameter(":snid", snid) };
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);           
            return res;
        }
        /// <summary>
        /// SN入MRB時，更新SN當前站為MRB，下一站為REWORK，不更新completed_flag=1,completed_time=now
        /// </summary>
        /// <param name="snid"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int SN_Mrb_Pass_actionNotUpdateCompleted(string snid, string userno, OleExec DB)
        {
            string strSql = $@" update r_sn set current_station='MRB',next_station='REWORK',edit_emp=:userno,edit_time=sysdate where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":userno", userno),
                new OleDbParameter(":snid", snid) };
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return res;
        }
        public int updateValid_Flag(string snid,string validFlag,string userno, OleExec DB)
        {
            string strSql = $@" update r_sn set valid_flag=:validflag,edit_emp=:empno,edit_time=sysdate where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":validflag", validFlag),
                new OleDbParameter(":empno", userno),
                new OleDbParameter(":snid", snid)
            };
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return res;
        }
        public R_SN GetById(string snid, OleExec DB)
        {
            string strSql = $@" select * from r_sn  where id=:snid";
            OleDbParameter[] paramet = new OleDbParameter[] {              
                new OleDbParameter(":snid", snid)
            };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_R_SN ret = (Row_R_SN)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        public int AddNewSN(R_SN NewSn, OleExec DB)
        {
            Row_R_SN rowSN = (Row_R_SN)this.ConstructRow(NewSn);
            string strSql = rowSN.GetInsertString(this.DBType);           
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, null);
            return res;
        }

        public Row_R_SN getR_SNbySN(string SN, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_MENU where SN='{SN}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_R_SN ret = (Row_R_SN)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public List<R_SN> GetObaSnListByLotNo(string lotNo, OleExec DB)
        {
            List<R_SN> res = new List<R_SN>();
           string strSql = $@" SELECT E.* FROM R_LOT_PACK A,R_PACKING B,R_PACKING C,R_SN_PACKING D,R_SN E
                                WHERE A.LOTNO='{lotNo}' AND A.PACKNO=B.PACK_NO AND B.ID=C.PARENT_PACK_ID AND C.ID=D.PACK_ID AND D.SN_ID=E.ID  AND E.VALID_FLAG=1 ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_R_SN r = (Row_R_SN)NewRow();
                r.loadData(VARIABLE);
                res.Add(r.GetDataObject());
            }
            return res;
        }

        //Add by LLF 2018-02-19 begin
        public string GetAPPTHTrCode(string WorkOrderNo, string Station, OleExec DB)
        {
            string result = string.Empty;
            Dictionary<string, object> GetTRCodeDic = null;
            string TRCode = string.Empty;
            string Message = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                OleDbParameter[] ParasForGetPTHTRCode = new OleDbParameter[5];

                ParasForGetPTHTRCode[0] = new OleDbParameter("g_type", "");
                ParasForGetPTHTRCode[0].Direction = ParameterDirection.Input;

                ParasForGetPTHTRCode[1] = new OleDbParameter("g_wo", WorkOrderNo);
                ParasForGetPTHTRCode[1].Direction = ParameterDirection.Input;

                ParasForGetPTHTRCode[2] = new OleDbParameter("g_station", Station);
                ParasForGetPTHTRCode[2].Direction = ParameterDirection.Input;

                ParasForGetPTHTRCode[3] = new OleDbParameter("g_tr_code", OleDbType.VarChar);
                ParasForGetPTHTRCode[3].Direction = ParameterDirection.Output;
                ParasForGetPTHTRCode[3].Size = 2000;

                ParasForGetPTHTRCode[4] = new OleDbParameter("RES", OleDbType.VarChar);
                ParasForGetPTHTRCode[4].Direction = ParameterDirection.Output;
                ParasForGetPTHTRCode[4].Size = 2000;

                GetTRCodeDic = DB.ExecProcedureReturnDic("MES1.GET_EXISTS_TRCODE_PTH", ParasForGetPTHTRCode);
                Message = GetTRCodeDic["RES"].ToString();

                if (Message == "OK")
                {
                    TRCode = GetTRCodeDic["g_tr_code"].ToString();
                }
                else
                {
                    throw new MESReturnMessage(Message);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return TRCode;
        }
        //Add by LLF 2018-02-19 End

        //add by LLF 2018-02-22 begin
        public R_SN GetDetailByPanelAndSN(string sn, OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            Row_R_SN row_rsn = null;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"select * from r_sn where sn='{sn.Replace("'", "''")}' and valid_flag='1'";//只取有效的sn
                sql = sql + $@"union select a.* from r_sn a, r_panel_sn b where b.panel = '{sn.Replace("'", "''")}' and a.sn=b.sn and valid_flag='1'";//只取有效的sn
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    row_rsn = (Row_R_SN)this.NewRow();
                    row_rsn.loadData(dt.Rows[0]);
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + sn });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return row_rsn.GetDataObject();
        }
        ///add by LLF 2018-02-22 end
        
        
        public string UpdateSNKeyparStatus(string SnID,string Emp_NO,string Valid, OleExec db)
        {
            string result = "";
            string sql = "";
            DataTable Dt = new DataTable();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Row_R_SN Rows = (Row_R_SN)NewRow();
                sql = $@"select * from r_sn where ID='{SnID}' and valid_flag='1'";//只取有效的sn
                Dt = db.ExecSelect(sql).Tables[0];
                if (Dt.Rows.Count == 1)
                {
                    Rows = (Row_R_SN)this.NewRow();
                    Rows.loadData(Dt.Rows[0]);
                }
                Rows.ID = SnID;
                Rows.VALID_FLAG = Valid;
                Rows.SHIPPED_FLAG = "1";
                Rows.SHIPDATE = GetDBDateTime(db);
                Rows.EDIT_EMP = Emp_NO;
                Rows.EDIT_TIME = GetDBDateTime(db);
                result=db.ExecSQL(Rows.GetUpdateString(DBType, SnID));

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public string InsertLinkSN(string Sn,string WO,string Skuno,string RouteID,string Kp_List_ID,string Station,string NextStation, string Emp_NO,string Bu, OleExec db,string Plant)
        {
            string result = "";
            T_R_SN Table_R_SN = new T_R_SN(db,DBType);
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Row_R_SN RowsInsert = (Row_R_SN)NewRow();
                RowsInsert.ID = Table_R_SN.GetNewID(Bu, db);
                RowsInsert.SN = Sn;
                RowsInsert.PLANT = Plant;
                RowsInsert.STARTED_FLAG = "1";
                RowsInsert.START_TIME = GetDBDateTime(db);
                RowsInsert.WORKORDERNO = WO;
                RowsInsert.SKUNO = Skuno;
                RowsInsert.ROUTE_ID = RouteID;
                RowsInsert.SHIPPED_FLAG = "0";
                RowsInsert.COMPLETED_FLAG = "0";
                RowsInsert.STOCK_STATUS = "0";
                RowsInsert.VALID_FLAG = "1";
                RowsInsert.KP_LIST_ID = Kp_List_ID;
                RowsInsert.CURRENT_STATION = Station;
                RowsInsert.NEXT_STATION = NextStation;
                RowsInsert.EDIT_EMP = Emp_NO;
                RowsInsert.EDIT_TIME = GetDBDateTime(db);
                result=db.ExecSQL(RowsInsert.GetInsertString(DBType));
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public int ReplaceSn(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_SN R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public int UpdateSNRMAStaus(string SnID,string Skuno, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_SN R SET R.workorderno = 'RMA',R.CURRENT_STATION = 'RMA',SKUNO = '{Skuno}' WHERE R.ID='{SnID}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        /// <summary>
        /// Oba工站ByLotNo取批次里所有SN
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_SN> GetSnByLotNoWithOba(string lotNo,OleExec DB)
        {
            string strSql = $@"  select e.* from r_sn_packing a ,r_lot_pack b,r_packing c,r_packing d,r_sn e where a.pack_id=c.id and b.lotno='{lotNo}' and c.parent_pack_id=d.id  and d.pack_no=b.packno and a.sn_id=e.id ";
            DataSet ds = DB.ExecSelect(strSql);
            List<R_SN> res = new List<R_SN>();
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_R_SN r = (Row_R_SN)this.NewRow();
                r.loadData(VARIABLE);
                res.Add(r.GetDataObject());
            }
            return res;
        }

        public string InsertRMASN(string Sn, string WO, string Skuno, string RouteID, string Kp_List_ID, string Station, string NextStation, string Emp_NO, string Bu, OleExec db, string Plant,string ProductStatus)
        {
            string result = "";
            T_R_SN Table_R_SN = new T_R_SN(db, DBType);
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Row_R_SN RowsInsert = (Row_R_SN)NewRow();
                RowsInsert.ID = Table_R_SN.GetNewID(Bu, db);
                RowsInsert.SN = Sn;
                RowsInsert.PLANT = Plant;
                RowsInsert.STARTED_FLAG = "1";
                RowsInsert.START_TIME = GetDBDateTime(db);
                RowsInsert.WORKORDERNO = WO;
                RowsInsert.SKUNO = Skuno;
                RowsInsert.ROUTE_ID = RouteID;
                RowsInsert.SHIPPED_FLAG = "0";
                RowsInsert.COMPLETED_FLAG = "1";
                RowsInsert.COMPLETED_TIME = GetDBDateTime(db);
                RowsInsert.STOCK_STATUS = "0";
                RowsInsert.VALID_FLAG = "1";
                RowsInsert.PRODUCT_STATUS = ProductStatus;
                RowsInsert.KP_LIST_ID = Kp_List_ID;
                RowsInsert.CURRENT_STATION = Station;
                RowsInsert.NEXT_STATION = NextStation;
                RowsInsert.EDIT_EMP = Emp_NO;
                RowsInsert.EDIT_TIME = GetDBDateTime(db);
                result = db.ExecSQL(RowsInsert.GetInsertString(DBType));
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

    }
    public class Row_R_SN : DataObjectBase
    {
        public Row_R_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_SN GetDataObject()
        {
            R_SN DataObject = new R_SN();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.PLANT = this.PLANT;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.STARTED_FLAG = this.STARTED_FLAG;
            DataObject.START_TIME = this.START_TIME;
            DataObject.PACKED_FLAG = this.PACKED_FLAG;
            DataObject.PACKDATE = this.PACKDATE;
            DataObject.COMPLETED_FLAG = this.COMPLETED_FLAG;
            DataObject.COMPLETED_TIME = this.COMPLETED_TIME;
            DataObject.SHIPPED_FLAG = this.SHIPPED_FLAG;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.REPAIR_FAILED_FLAG = this.REPAIR_FAILED_FLAG;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.KP_LIST_ID = this.KP_LIST_ID;
            DataObject.PO_NO = this.PO_NO;
            DataObject.CUST_ORDER_NO = this.CUST_ORDER_NO;
            DataObject.CUST_PN = this.CUST_PN;
            DataObject.BOXSN = this.BOXSN;
            DataObject.SCRAPED_FLAG = this.SCRAPED_FLAG;
            DataObject.SCRAPED_TIME = this.SCRAPED_TIME;
            DataObject.PRODUCT_STATUS = this.PRODUCT_STATUS;
            DataObject.REWORK_COUNT = this.REWORK_COUNT;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.STOCK_STATUS = this.STOCK_STATUS;
            DataObject.STOCK_IN_TIME = this.STOCK_IN_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string ROUTE_ID
        {
            get
            {
                return (string)this["ROUTE_ID"];
            }
            set
            {
                this["ROUTE_ID"] = value;
            }
        }
        public string STARTED_FLAG
        {
            get
            {
                return (string)this["STARTED_FLAG"];
            }
            set
            {
                this["STARTED_FLAG"] = value;
            }
        }
        public DateTime? START_TIME
        {
            get
            {
                return (DateTime?)this["START_TIME"];
            }
            set
            {
                this["START_TIME"] = value;
            }
        }
        public string PACKED_FLAG
        {
            get
            {
                return (string)this["PACKED_FLAG"];
            }
            set
            {
                this["PACKED_FLAG"] = value;
            }
        }
        public DateTime? PACKDATE
        {
            get
            {
                return (DateTime?)this["PACKDATE"];
            }
            set
            {
                this["PACKDATE"] = value;
            }
        }
        public string COMPLETED_FLAG
        {
            get
            {
                return (string)this["COMPLETED_FLAG"];
            }
            set
            {
                this["COMPLETED_FLAG"] = value;
            }
        }
        public DateTime? COMPLETED_TIME
        {
            get
            {
                return (DateTime?)this["COMPLETED_TIME"];
            }
            set
            {
                this["COMPLETED_TIME"] = value;
            }
        }
        public string SHIPPED_FLAG
        {
            get
            {
                return (string)this["SHIPPED_FLAG"];
            }
            set
            {
                this["SHIPPED_FLAG"] = value;
            }
        }
        public DateTime? SHIPDATE
        {
            get
            {
                return (DateTime?)this["SHIPDATE"];
            }
            set
            {
                this["SHIPDATE"] = value;
            }
        }
        public string REPAIR_FAILED_FLAG
        {
            get
            {
                return (string)this["REPAIR_FAILED_FLAG"];
            }
            set
            {
                this["REPAIR_FAILED_FLAG"] = value;
            }
        }
        public string CURRENT_STATION
        {
            get
            {
                return (string)this["CURRENT_STATION"];
            }
            set
            {
                this["CURRENT_STATION"] = value;
            }
        }
        public string NEXT_STATION
        {
            get
            {
                return (string)this["NEXT_STATION"];
            }
            set
            {
                this["NEXT_STATION"] = value;
            }
        }
        public string KP_LIST_ID
        {
            get
            {
                return (string)this["KP_LIST_ID"];
            }
            set
            {
                this["KP_LIST_ID"] = value;
            }
        }
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string CUST_ORDER_NO
        {
            get
            {
                return (string)this["CUST_ORDER_NO"];
            }
            set
            {
                this["CUST_ORDER_NO"] = value;
            }
        }
        public string CUST_PN
        {
            get
            {
                return (string)this["CUST_PN"];
            }
            set
            {
                this["CUST_PN"] = value;
            }
        }
        public string BOXSN
        {
            get
            {
                return (string)this["BOXSN"];
            }
            set
            {
                this["BOXSN"] = value;
            }
        }
        public string SCRAPED_FLAG
        {
            get
            {
                return (string)this["SCRAPED_FLAG"];
            }
            set
            {
                this["SCRAPED_FLAG"] = value;
            }
        }
        public DateTime? SCRAPED_TIME
        {
            get
            {
                return (DateTime?)this["SCRAPED_TIME"];
            }
            set
            {
                this["SCRAPED_TIME"] = value;
            }
        }
        public string PRODUCT_STATUS
        {
            get
            {
                return (string)this["PRODUCT_STATUS"];
            }
            set
            {
                this["PRODUCT_STATUS"] = value;
            }
        }
        public double? REWORK_COUNT
        {
            get
            {
                return (double?)this["REWORK_COUNT"];
            }
            set
            {
                this["REWORK_COUNT"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string STOCK_STATUS
        {
            get
            {
                return (string)this["STOCK_STATUS"];
            }
            set
            {
                this["STOCK_STATUS"] = value;
            }
        }

        public DateTime? STOCK_IN_TIME
        {
            get
            {
                return (DateTime?)this["STOCK_IN_TIME"];
            }
            set
            {
                this["STOCK_IN_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_SN
    {
        public string ID;
        public string SN;
        public string SKUNO;
        public string WORKORDERNO;
        public string PLANT;
        public string ROUTE_ID;
        public string STARTED_FLAG;
        public DateTime? START_TIME;
        public string PACKED_FLAG;
        public DateTime? PACKDATE;
        public string COMPLETED_FLAG;
        public DateTime? COMPLETED_TIME;
        public string SHIPPED_FLAG;
        public DateTime? SHIPDATE;
        public string REPAIR_FAILED_FLAG;
        public string CURRENT_STATION;
        public string NEXT_STATION;
        public string KP_LIST_ID;
        public string PO_NO;
        public string CUST_ORDER_NO;
        public string CUST_PN;
        public string BOXSN;
        public string SCRAPED_FLAG;
        public DateTime? SCRAPED_TIME;
        public string PRODUCT_STATUS;
        public double? REWORK_COUNT;
        public string VALID_FLAG;
        public string STOCK_STATUS;
        public DateTime? STOCK_IN_TIME;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }

    //public class GetSNByWorkorderNo
    //{
    //    public string ID;
    //    public string SN;
    //    public string SKUNO;
    //    public string WORKORDERNO;
    //    public string PLANT;
    //    public string ROUTE_ID;
    //    public string STARTED_FLAG;
    //    public string START_TIME;
    //    public string PACKED_FLAG;
    //    public string PACKDATE;
    //    public string COMPLETED_FLAG;
    //    public string COMPLETED_TIME;
    //    public string SHIPPED_FLAG;
    //    public string SHIPDATE;
    //    public string REPAIR_FAILED_FLAG;
    //    public string CURRENT_STATION;
    //    public string NEXT_STATION;
    //    public string KP_LIST_ID;
    //    public string PO_NO;
    //    public string CUST_ORDER_NO;
    //    public string CUST_PN;
    //    public string BOXSN;
    //    public string SCRAPED_FLAG;
    //    public string SCRAPED_TIME;
    //    public string PRODUCT_STATUS;
    //    public string REWORK_COUNT;
    //    public string VALID_FLAG;
    //    public string EDIT_EMP;
    //    public DateTime? EDIT_TIME;
    //}


    public class T_R_YIELD_RATE_DETAIL : DataObjectTable
    {
        public T_R_YIELD_RATE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_YIELD_RATE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_YIELD_RATE_DETAIL);
            TableName = "R_YIELD_RATE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_YIELD_RATE_DETAIL : DataObjectBase
    {
        public Row_R_YIELD_RATE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_YIELD_RATE_DETAIL GetDataObject()
        {
            R_YIELD_RATE_DETAIL DataObject = new R_YIELD_RATE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.WORK_DATE = this.WORK_DATE;
            DataObject.WORK_TIME = this.WORK_TIME;
            DataObject.PRODUCTION_LINE = this.PRODUCTION_LINE;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_NAME = this.SKU_NAME;
            DataObject.SKU_SERIES = this.SKU_SERIES;
            DataObject.TOTAL_FRESH_BUILD_QTY = this.TOTAL_FRESH_BUILD_QTY;
            DataObject.TOTAL_FRESH_PASS_QTY = this.TOTAL_FRESH_PASS_QTY;
            DataObject.TOTAL_FRESH_FAIL_QTY = this.TOTAL_FRESH_FAIL_QTY;
            DataObject.TOTAL_REWORK_BUILD_QTY = this.TOTAL_REWORK_BUILD_QTY;
            DataObject.TOTAL_REWORK_PASS_QTY = this.TOTAL_REWORK_PASS_QTY;
            DataObject.TOTAL_REWORK_FAIL_QTY = this.TOTAL_REWORK_FAIL_QTY;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public DateTime WORK_DATE
        {
            get
            {
                return (DateTime)this["WORK_DATE"];
            }
            set
            {
                this["WORK_DATE"] = value;
            }
        }
        public string WORK_TIME
        {
            get
            {
                return (string)this["WORK_TIME"];
            }
            set
            {
                this["WORK_TIME"] = value;
            }
        }
        public string PRODUCTION_LINE
        {
            get
            {
                return (string)this["PRODUCTION_LINE"];
            }
            set
            {
                this["PRODUCTION_LINE"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string SKU_NAME
        {
            get
            {
                return (string)this["SKU_NAME"];
            }
            set
            {
                this["SKU_NAME"] = value;
            }
        }
        public string SKU_SERIES
        {
            get
            {
                return (string)this["SKU_SERIES"];
            }
            set
            {
                this["SKU_SERIES"] = value;
            }
        }
        public double? TOTAL_FRESH_BUILD_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_BUILD_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_BUILD_QTY"] = value;
            }
        }
        public double? TOTAL_FRESH_PASS_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_PASS_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_PASS_QTY"] = value;
            }
        }
        public double? TOTAL_FRESH_FAIL_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_FAIL_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_FAIL_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_BUILD_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_BUILD_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_BUILD_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_PASS_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_PASS_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_PASS_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_FAIL_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_FAIL_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_FAIL_QTY"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_YIELD_RATE_DETAIL
    {
        public string ID;
        public DateTime WORK_DATE;
        public string WORK_TIME;
        public string PRODUCTION_LINE;
        public string CLASS_NAME;
        public string STATION_NAME;
        public string WORKORDERNO;
        public string SKUNO;
        public string SKU_NAME;
        public string SKU_SERIES;
        public double? TOTAL_FRESH_BUILD_QTY;
        public double? TOTAL_FRESH_PASS_QTY;
        public double? TOTAL_FRESH_FAIL_QTY;
        public double? TOTAL_REWORK_BUILD_QTY;
        public double? TOTAL_REWORK_PASS_QTY;
        public double? TOTAL_REWORK_FAIL_QTY;
        public string EDIT_EMP;
        public DateTime EDIT_TIME;
    }

    public class T_R_UPH_DETAIL : DataObjectTable
    {
        public T_R_UPH_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_UPH_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_UPH_DETAIL);
            TableName = "R_UPH_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_UPH_DETAIL : DataObjectBase
    {
        public Row_R_UPH_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_UPH_DETAIL GetDataObject()
        {
            R_UPH_DETAIL DataObject = new R_UPH_DETAIL();
            DataObject.ID = this.ID;
            DataObject.WORK_DATE = this.WORK_DATE;
            DataObject.WORK_TIME = this.WORK_TIME;
            DataObject.PRODUCTION_LINE = this.PRODUCTION_LINE;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_NAME = this.SKU_NAME;
            DataObject.SKU_SERIES = this.SKU_SERIES;
            DataObject.TOTAL_FRESH_BUILD_QTY = this.TOTAL_FRESH_BUILD_QTY;
            DataObject.TOTAL_FRESH_PASS_QTY = this.TOTAL_FRESH_PASS_QTY;
            DataObject.TOTAL_FRESH_FAIL_QTY = this.TOTAL_FRESH_FAIL_QTY;
            DataObject.TOTAL_REWORK_BUILD_QTY = this.TOTAL_REWORK_BUILD_QTY;
            DataObject.TOTAL_REWORK_PASS_QTY = this.TOTAL_REWORK_PASS_QTY;
            DataObject.TOTAL_REWORK_FAIL_QTY = this.TOTAL_REWORK_FAIL_QTY;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public DateTime WORK_DATE
        {
            get
            {
                return (DateTime)this["WORK_DATE"];
            }
            set
            {
                this["WORK_DATE"] = value;
            }
        }
        public string WORK_TIME
        {
            get
            {
                return (string)this["WORK_TIME"];
            }
            set
            {
                this["WORK_TIME"] = value;
            }
        }
        public string PRODUCTION_LINE
        {
            get
            {
                return (string)this["PRODUCTION_LINE"];
            }
            set
            {
                this["PRODUCTION_LINE"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string SKU_NAME
        {
            get
            {
                return (string)this["SKU_NAME"];
            }
            set
            {
                this["SKU_NAME"] = value;
            }
        }
        public string SKU_SERIES
        {
            get
            {
                return (string)this["SKU_SERIES"];
            }
            set
            {
                this["SKU_SERIES"] = value;
            }
        }
        public double? TOTAL_FRESH_BUILD_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_BUILD_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_BUILD_QTY"] = value;
            }
        }
        public double? TOTAL_FRESH_PASS_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_PASS_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_PASS_QTY"] = value;
            }
        }
        public double? TOTAL_FRESH_FAIL_QTY
        {
            get
            {
                return (double?)this["TOTAL_FRESH_FAIL_QTY"];
            }
            set
            {
                this["TOTAL_FRESH_FAIL_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_BUILD_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_BUILD_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_BUILD_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_PASS_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_PASS_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_PASS_QTY"] = value;
            }
        }
        public double? TOTAL_REWORK_FAIL_QTY
        {
            get
            {
                return (double?)this["TOTAL_REWORK_FAIL_QTY"];
            }
            set
            {
                this["TOTAL_REWORK_FAIL_QTY"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_UPH_DETAIL
    {
        public string ID;
        public DateTime WORK_DATE;
        public string WORK_TIME;
        public string PRODUCTION_LINE;
        public string CLASS_NAME;
        public string STATION_NAME;
        public string WORKORDERNO;
        public string SKUNO;
        public string SKU_NAME;
        public string SKU_SERIES;
        public double? TOTAL_FRESH_BUILD_QTY;
        public double? TOTAL_FRESH_PASS_QTY;
        public double? TOTAL_FRESH_FAIL_QTY;
        public double? TOTAL_REWORK_BUILD_QTY;
        public double? TOTAL_REWORK_PASS_QTY;
        public double? TOTAL_REWORK_FAIL_QTY;
        public string EDIT_EMP;
        public DateTime EDIT_TIME;
    }

}
