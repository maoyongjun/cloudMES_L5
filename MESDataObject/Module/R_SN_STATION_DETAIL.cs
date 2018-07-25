using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_R_SN_STATION_DETAIL : DataObjectTable
    {
        public T_R_SN_STATION_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_STATION_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_STATION_DETAIL);
            TableName = "R_SN_STATION_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
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

        public string AddDetailToRSnStationDetail(string SNDetailID,R_SN SN,string Line,string StationName,string DeviceName, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            T_R_SN_STATION_DETAIL Table_R_Sn_Station_Detail = new T_R_SN_STATION_DETAIL(DB, DBType);
            Row_R_SN_STATION_DETAIL row = null;

            if (SN != null && !string.IsNullOrEmpty(SN.ID))
            {
                row = (Row_R_SN_STATION_DETAIL)ConstructRow(SN);
                row.ID = SNDetailID;
                row.R_SN_ID = SN.ID;
                row.CLASS_NAME = GetWorkClass(DB);
                row.LINE = Line;
                row.STATION_NAME = StationName;
                row.DEVICE_NAME = DeviceName;
                row.EDIT_TIME = GetDBDateTime(DB);
                if (this.DBType == DB_TYPE_ENUM.Oracle)
                {
                    sql = row.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                    return result;

                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
            }
            return result;
        }

        /// <summary>
        /// BIP FAIL R_SN_STATION_DETAIL REPAIR_FAILED_FLAG=1
        /// </summary>
        /// <param name="SNDetailID"></param>
        /// <param name="SN"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="DB"></param>
        /// <param name="Fail_Flag"></param>
        /// <returns></returns>
        public string AddDetailToBipStationFailDetail(string SNDetailID, R_SN SN, string Line, string StationName, string DeviceName, OleExec DB,string Fail_Flag)
        {
            string result = string.Empty;
            string sql = string.Empty;
            T_R_SN_STATION_DETAIL Table_R_Sn_Station_Detail = new T_R_SN_STATION_DETAIL(DB, DBType);
            Row_R_SN_STATION_DETAIL row = null;

            if (SN != null && !string.IsNullOrEmpty(SN.ID))
            {
                row = (Row_R_SN_STATION_DETAIL)ConstructRow(SN);
                row.ID = SNDetailID;
                row.R_SN_ID = SN.ID;
                row.CLASS_NAME = GetWorkClass(DB);
                row.LINE = Line;
                row.REPAIR_FAILED_FLAG = Fail_Flag;
                row.STATION_NAME = StationName;
                row.DEVICE_NAME = DeviceName;
                //row.EDIT_TIME = GetDBDateTime(DB);
                if (this.DBType == DB_TYPE_ENUM.Oracle)
                {
                    sql = row.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                    return result;

                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
            }
            return result;
        }
        /// <summary>
        /// 獲取工單在某個工站過站數量
        /// </summary>
        /// <param name="strWo"></param>
        /// <param name="stationname"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int GetCountByWOAndStation(string strWo,string stationname, OleExec DB)
        {
            string strSql = $@"select count(distinct m.sn) from r_sn_station_detail m  where workorderno=:strWo and station_name=:stationname";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":strWo",OleDbType.VarChar,100 );
            paramet[0].Value = strWo;
            paramet[1] = new OleDbParameter(":stationname",OleDbType.VarChar,100);
            paramet[1].Value = stationname;
            int result =Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text, paramet));
            return result;
        }
        public int GetCountByWOAndStationNotContailFail(string strWo, string stationname, OleExec DB)
        {
            string strSql = $@"select count(distinct m.sn) from r_sn_station_detail m  where workorderno=:strWo and station_name=:stationname and REPAIR_FAILED_FLAG<>1";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":strWo", strWo);
            paramet[1] = new OleDbParameter(":stationname", stationname);
            int result = Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text, paramet));
            return result;
        }
        /// <summary>
        /// 分板工站替換序列號
        /// 黄杨盛 2018年4月14日10:00:27 增加edit_time,改为使用参数形式,注意:若MYSQL或SQLSERVER需要使用需要在执行前把TEXT中的:符号替换为@符号
        /// 黄杨盛 2018年4月28日09:39:29 取消edit_time,这个并没有使用id,而是直接用了sn
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void UpdateRSnStationDetailBySNID(string SN,string SN_ID,OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            //sql = $@"update r_sn_station_detail set sn='{SN}' where sn='{SN_ID}'";
            //DB.ExecSQL(sql);

            var editTime = GetDBDateTime(DB);
            sql = $@"update r_sn_station_detail set sn=:SN where sn=:ID";
            var parameters = new OleDbParameter[2]
            {
                new OleDbParameter("SN", SN) {DbType = DbType.String},
                new OleDbParameter("ID", SN_ID) {DbType = DbType.String}
            };
     
            DB.ExecuteNonQuery(sql, CommandType.Text, parameters);
        }

        public R_SN_STATION_DETAIL GetSNLastPassStationDetail(string SN, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select*from (select*from r_sn_station_detail where sn='{SN}' and repair_failed_flag='0' order by edit_time desc)a where rownum=1 ";
            Dt= DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }

        public int ReplaceSnStationDetail(string NewSn, string OldSn, OleExec DB,DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_SN_STATION_DETAIL R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
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
        /// 獲取某一時間段內掃入STOCKIN的機種及其數量,用於STOCKIN BACKFLUSH拋帳用
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public DataTable GetStockInQtyByTime(DateTime startTime,DateTime endTime,OleExec sfcdb)
        {
          string  sql = $@"select m.skuno,count(1) qty
                          from r_sn_station_detail m
                         where m.station_name = 'STOCKIN'
                           and m.edit_time >= to_date('{startTime.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy/mm/dd hh24:mi:ss')
                           and m.edit_time < to_date('{endTime.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy/mm/dd hh24:mi:ss') group by m.skuno";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if(dt.Rows.Count>0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }


        public List<R_SN_STATION_DETAIL> GetSNStationDetailByPanel(string PanelSn, OleExec DB)
        {
            List<R_SN_STATION_DETAIL> R_SN_STATION_DETAIL_LIST = new List<R_SN_STATION_DETAIL>();
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"SELECT * FROM R_SN_STATION_DETAIL WHERE SN IN (SELECT SN FROM R_PANEL_SN WHERE PANEL = '{PanelSn}') order by id ";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            foreach(DataRow r in Dt.Rows)
            {
                Rows.loadData(r);
                R_Sn_Station_Detail = Rows.GetDataObject();
                R_SN_STATION_DETAIL_LIST.Add(R_Sn_Station_Detail);
            }
            return R_SN_STATION_DETAIL_LIST;
        }
    }
    public class Row_R_SN_STATION_DETAIL : DataObjectBase
    {
        public Row_R_SN_STATION_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_STATION_DETAIL GetDataObject()
        {
            R_SN_STATION_DETAIL DataObject = new R_SN_STATION_DETAIL();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.PLANT = this.PLANT;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.LINE = this.LINE;
            DataObject.STARTED_FLAG = this.STARTED_FLAG;
            DataObject.START_TIME = this.START_TIME;
            DataObject.PACKED_FLAG = this.PACKED_FLAG;
            DataObject.PACKED_TIME = this.PACKED_TIME;
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
            DataObject.DEVICE_NAME = this.DEVICE_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.SCRAPED_FLAG = this.SCRAPED_FLAG;
            DataObject.SCRAPED_TIME = this.SCRAPED_TIME;
            DataObject.PRODUCT_STATUS = this.PRODUCT_STATUS;
            DataObject.REWORK_COUNT = this.REWORK_COUNT;
            DataObject.VALID_FLAG = this.VALID_FLAG;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
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

        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
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
        public DateTime? PACKED_TIME
        {
            get
            {
                return (DateTime?)this["PACKED_TIME"];
            }
            set
            {
                this["PACKED_TIME"] = value;
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

        public string DEVICE_NAME
        {
            get
            {
                return (string)this["DEVICE_NAME"];
            }
            set
            {
                this["DEVICE_NAME"] = value;
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
    public class R_SN_STATION_DETAIL
    {
        public string ID;
        public string R_SN_ID;
        public string SN;
        public string SKUNO;
        public string WORKORDERNO;
        public string PLANT;
        public string CLASS_NAME;
        public string ROUTE_ID;
        public string LINE;
        public string STARTED_FLAG;
        public DateTime? START_TIME;
        public string PACKED_FLAG;
        public DateTime? PACKED_TIME;
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
        public string DEVICE_NAME;
        public string STATION_NAME;
        public string SCRAPED_FLAG;
        public DateTime? SCRAPED_TIME;
        public string PRODUCT_STATUS;
        public double? REWORK_COUNT;
        public string VALID_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}