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
    public class T_R_WO_BASE : DataObjectTable
    {
        public T_R_WO_BASE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_BASE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_BASE);
            TableName = "R_WO_BASE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public Row_R_WO_BASE GetWo(string _WO, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from r_wo_base where workorderno='{_WO.Replace("'", "''")}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "WorkOrder:" + _WO });
                    throw new MESReturnMessage(errMsg);
                }
                Row_R_WO_BASE R = (Row_R_WO_BASE)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        public R_WO_BASE GetWoByWoNo(string _WO, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from r_wo_base where workorderno='{_WO.Replace("'", "''")}'";
                DataTable result = DB.ExecuteDataTable(strsql, CommandType.Text);
                if (result.Rows.Count > 0)
                {
                    return CreateLanguageClass(result.Rows[0]);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public Row_R_WO_BASE LoadWorkorder(string _WO, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from r_wo_base where workorderno='{_WO.Replace("'", "''")}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    return null;
                }
                Row_R_WO_BASE R = (Row_R_WO_BASE)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 給工單的投入數增加指定值
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="count"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string AddCountToWo(string wo, double count, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            Row_R_WO_BASE row = null;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                //Modify by LLF 2018-04-10 for 同時幾台電腦在掃描時會存在數量更新不准的問題
                //row = GetWo(wo, DB);
                //row.INPUT_QTY += count;
                //if (row.INPUT_QTY > row.WORKORDER_QTY)
                //{
                //    row.INPUT_QTY = row.WORKORDER_QTY;
                //}
                //sql = row.GetUpdateString(this.DBType);
                //result = DB.ExecSQL(sql);
                string strSql = $@"update r_wo_base set input_qty=input_qty+{count} where workorderno='{wo}'";
                result = DB.ExecSQL(strSql);

                return result;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 更新完工數量以及判斷是否應該關結工單
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="count"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateFinishQty(string wo, double count, OleExec DB)
        {
            int result = 0;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO='{wo}' AND WORKORDER_QTY-FINISHED_QTY>={count}";
                dt = DB.ExecSelect(sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    sql = $@"UPDATE R_WO_BASE SET FINISHED_QTY=CASE WHEN (FINISHED_QTY IS NULL) THEN {count} ELSE FINISHED_QTY+{count} END,
                                EDIT_TIME='{GetDBDateTime(DB)}' WHERE WORKORDERNO='{wo}'";
                    result = DB.ExecSqlNoReturn(sql, null);
                    sql = $@"UPDATE R_WO_BASE SET CLOSED_FLAG='1',CLOSE_DATE='{GetDBDateTime(DB)}' WHERE WORKORDERNO='{wo}' 
                                AND FINISHED_QTY>=WORKORDER_QTY";
                    result = DB.ExecSqlNoReturn(sql, null);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
        /// <summary>
        /// 依據10進制或34進制獲取下一個流水碼
        /// </summary>
        /// <param name="CurrentSN"></param>
        /// <param name="DecimalType"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string Get_NextSN(string strCurrentSN, string strDecimalType)
        {
            string strNextSN = strCurrentSN;
            string strTempSN = "";
            string strTempCH = "";
            int intAdd = 1;
            int intIndex = 0;
            int intCount = 0;
            string strBaseDecimal = "0123456789";

            if (strDecimalType == "10H")
            {
                strBaseDecimal = "0123456789";
            }
            else if (strDecimalType == "34H")
            {
                strBaseDecimal = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";
            }
            else if (strDecimalType == "36H")
            {
                strBaseDecimal = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            else if (strDecimalType == "16H")
            {
                strBaseDecimal = "0123456789ABCDEF";
            }


            intIndex = strNextSN.Length;
            while (intAdd == 1)
            {
                intAdd = 0;
                strTempCH = strNextSN.Substring(intIndex - 1, 1);
                intCount = strBaseDecimal.IndexOf(strTempCH);
                if (intCount == strBaseDecimal.Length - 1)
                {
                    intAdd = 1;
                    strTempSN = "0" + strTempSN;
                }
                else
                {
                    strTempSN = strBaseDecimal.Substring(intCount + 1, 1) + strTempSN;
                }
                intIndex = intIndex - 1;
            }

            intIndex = strNextSN.Length - strTempSN.Length;
            strNextSN = strNextSN.Substring(0, intIndex) + strTempSN;
            return strNextSN;
        }

        public bool CheckDataExist(string wo_no,string skuno, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO='{wo_no}' AND SKUNO='{skuno}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        public bool CheckDataExist(string wo_no, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO='{wo_no}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }
        public List<R_WO_BASE> CheckFlag(string wo_no, OleExec DB)
        {
            List<R_WO_BASE> WOList = new List<R_WO_BASE>();
            string sql = "";
            DataTable dt = null;
            try
            {

                sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO='{wo_no}' and input_qty<>0 ";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    WOList.Add(CreateLanguageClass(dr));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return WOList;
        }
        public List<R_WO_BASE> ShowAllData(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_WO_BASE> LanguageList = new List<R_WO_BASE>();
            sql = $@"SELECT * FROM R_WO_BASE order by EDIT_TIME";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }
        public R_WO_BASE CreateLanguageClass(DataRow dr)
        {
            Row_R_WO_BASE row = (Row_R_WO_BASE)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        //use for load WO in add_WOSNRange function in WOSNRange.html
        public List<R_WO_BASE> GetAllWO(string r_wo, OleExec DB)
        {
            List<R_WO_BASE> WOList = new List<R_WO_BASE>();
            string sql = "";
            DataTable dt = null;
            try
            {
                if (string.IsNullOrEmpty(r_wo.Trim()))
                {
                    sql = "SELECT * FROM  R_WO_BASE where WORKORDERNO NOT IN (SELECT WORKORDERNO FROM R_WO_REGION)  and rownum<20";
                }
                else
                {
                    sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO like '%{r_wo.ToUpper()}%' and rownum<20 ORDER BY EDIT_TIME";
                }
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    WOList.Add(CreateLanguageClass(dr));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return WOList;
        }
        public List<R_WO_BASE> GetSkunoByWO(string r_wo, string r_skuno, OleExec DB)
        {
            List<R_WO_BASE> SkunoList = new List<R_WO_BASE>();
            string sql = "";
            DataTable dt = null;
            try
            {
                sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO='{r_wo.ToUpper()}'";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SkunoList.Add(CreateLanguageClass(dr));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return SkunoList;
        }
        public List<R_WO_BASE> GetQtyByWOSkuno(string r_wo, string r_skuno, int r_qty, OleExec DB)
        {
            List<R_WO_BASE> qty = new List<R_WO_BASE>();
            string sql = "";
            DataTable dt = null;
            try
            {
                sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO='{r_wo.ToUpper()}' and SKUNO='{r_skuno}' ";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    qty.Add(CreateLanguageClass(dr));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return qty;
        }

        public List<R_WO_BASE> CheckToUpload(string r_wo, string r_skuno, int r_qty, OleExec DB)
        {
            List<R_WO_BASE> qty = new List<R_WO_BASE>();
            string sql = "";
            DataTable dt = null;
            try
            {
                sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO='{r_wo.ToUpper()}' and SKUNO='{r_skuno.ToUpper()}'and QTY='{r_qty}' ";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    qty.Add(CreateLanguageClass(dr));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return qty;
        }
        public int UpdateFINISHEDQTYAddOne(string r_wo, OleExec DB)
        {
            string strSql = $@"update r_wo_base set finished_qty=case when (finished_qty is null) then 1 else (finished_qty+1) end where workorderno=:wono";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":wono", r_wo) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }

        /// <summary>
        /// 查詢工單前綴為00251且沒有關閉的工單信息
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public List<R_WO_BASE> MatchSpecialPrefixWO(OleExec sfcdb, string prefix)
        {
            List<R_WO_BASE> woes = new List<R_WO_BASE>();
            if (string.IsNullOrEmpty(prefix))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_WO_BASE row_wo = null;
            string sql = $@"select * from {this.TableName} where workorderno like '{prefix.Replace("'", "''")}%' and closed_flag='0' ";
            dt = sfcdb.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                row_wo = (Row_R_WO_BASE)this.NewRow();
                row_wo.loadData(dr);

                woes.Add(row_wo.GetDataObject());
            }
            return woes;
        }

        public int UpdateWoQty(string WO,int CutQty,string Emp_NO, OleExec sfcdb)
        {
            string strSql = $@"update r_wo_base set workorder_qty=workorder_qty-{CutQty},edit_emp='{Emp_NO}',edit_time=sysdate where workorderno='{WO}'";
            int result = sfcdb.ExecSqlNoReturn(strSql, null);
            return result;
        }

    }
    public class Row_R_WO_BASE : DataObjectBase
    {
        public Row_R_WO_BASE(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_BASE GetDataObject()
        {
            R_WO_BASE DataObject = new R_WO_BASE();
            DataObject.ID = this.ID;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.PLANT = this.PLANT;
            DataObject.RELEASE_DATE = this.RELEASE_DATE;
            DataObject.DOWNLOAD_DATE = this.DOWNLOAD_DATE;
            DataObject.PRODUCTION_TYPE = this.PRODUCTION_TYPE;
            DataObject.WO_TYPE = this.WO_TYPE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_VER = this.SKU_VER;
            DataObject.SKU_SERIES = this.SKU_SERIES;
            DataObject.SKU_NAME = this.SKU_NAME;
            DataObject.SKU_DESC = this.SKU_DESC;
            DataObject.CUST_PN = this.CUST_PN;
            DataObject.CUST_PN_VER = this.CUST_PN_VER;
            DataObject.CUSTOMER_NAME = this.CUSTOMER_NAME;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.START_STATION = this.START_STATION;
            DataObject.KP_LIST_ID = this.KP_LIST_ID;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.CLOSE_DATE = this.CLOSE_DATE;
            DataObject.WORKORDER_QTY = this.WORKORDER_QTY;
            DataObject.INPUT_QTY = this.INPUT_QTY;
            DataObject.FINISHED_QTY = this.FINISHED_QTY;
            DataObject.SCRAPED_QTY = this.SCRAPED_QTY;
            DataObject.STOCK_LOCATION = this.STOCK_LOCATION;
            DataObject.PO_NO = this.PO_NO;
            DataObject.CUST_ORDER_NO = this.CUST_ORDER_NO;
            DataObject.ROHS = this.ROHS;
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
        public DateTime? RELEASE_DATE
        {
            get
            {
                return (DateTime?)this["RELEASE_DATE"];
            }
            set
            {
                this["RELEASE_DATE"] = value;
            }
        }
        public DateTime? DOWNLOAD_DATE
        {
            get
            {
                return (DateTime?)this["DOWNLOAD_DATE"];
            }
            set
            {
                this["DOWNLOAD_DATE"] = value;
            }
        }
        public string PRODUCTION_TYPE
        {
            get
            {
                return (string)this["PRODUCTION_TYPE"];
            }
            set
            {
                this["PRODUCTION_TYPE"] = value;
            }
        }
        public string WO_TYPE
        {
            get
            {
                return (string)this["WO_TYPE"];
            }
            set
            {
                this["WO_TYPE"] = value;
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
        public string SKU_VER
        {
            get
            {
                return (string)this["SKU_VER"];
            }
            set
            {
                this["SKU_VER"] = value;
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
        public string SKU_DESC
        {
            get
            {
                return (string)this["SKU_DESC"];
            }
            set
            {
                this["SKU_DESC"] = value;
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
        public string CUST_PN_VER
        {
            get
            {
                return (string)this["CUST_PN_VER"];
            }
            set
            {
                this["CUST_PN_VER"] = value;
            }
        }
        public string CUSTOMER_NAME
        {
            get
            {
                return (string)this["CUSTOMER_NAME"];
            }
            set
            {
                this["CUSTOMER_NAME"] = value;
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
        public string START_STATION
        {
            get
            {
                return (string)this["START_STATION"];
            }
            set
            {
                this["START_STATION"] = value;
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
        public string CLOSED_FLAG
        {
            get
            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
            }
        }
        public DateTime? CLOSE_DATE
        {
            get
            {
                return (DateTime?)this["CLOSE_DATE"];
            }
            set
            {
                this["CLOSE_DATE"] = value;
            }
        }
        public double? WORKORDER_QTY
        {
            get
            {
                return (double?)this["WORKORDER_QTY"];
            }
            set
            {
                this["WORKORDER_QTY"] = value;
            }
        }
        public double? INPUT_QTY
        {
            get
            {
                return (double?)this["INPUT_QTY"];
            }
            set
            {
                this["INPUT_QTY"] = value;
            }
        }
        public double? FINISHED_QTY
        {
            get
            {
                return (double?)this["FINISHED_QTY"];
            }
            set
            {
                this["FINISHED_QTY"] = value;
            }
        }
        public double? SCRAPED_QTY
        {
            get
            {
                return (double?)this["SCRAPED_QTY"];
            }
            set
            {
                this["SCRAPED_QTY"] = value;
            }
        }
        public string STOCK_LOCATION
        {
            get
            {
                return (string)this["STOCK_LOCATION"];
            }
            set
            {
                this["STOCK_LOCATION"] = value;
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
        public string ROHS
        {
            get
            {
                return (string)this["ROHS"];
            }
            set
            {
                this["ROHS"] = value;
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
    public class R_WO_BASE
    {
        public string ID;
        public string WORKORDERNO;
        public string PLANT;
        public DateTime? RELEASE_DATE;
        public DateTime? DOWNLOAD_DATE;
        public string PRODUCTION_TYPE;
        public string WO_TYPE;
        public string SKUNO;
        public string SKU_VER;
        public string SKU_SERIES;
        public string SKU_NAME;
        public string SKU_DESC;
        public string CUST_PN;
        public string CUST_PN_VER;
        public string CUSTOMER_NAME;
        public string ROUTE_ID;
        public string START_STATION;
        public string KP_LIST_ID;
        public string CLOSED_FLAG;
        public DateTime? CLOSE_DATE;
        public double? WORKORDER_QTY;
        public double? INPUT_QTY;
        public double? FINISHED_QTY;
        public double? SCRAPED_QTY;
        public string STOCK_LOCATION;
        public string PO_NO;
        public string CUST_ORDER_NO;
        public string ROHS;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
    //RuRun
}
