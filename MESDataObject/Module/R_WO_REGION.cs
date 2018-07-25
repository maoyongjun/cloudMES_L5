using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using System.Text.RegularExpressions;

//WO_Skuno: get in MO
namespace MESDataObject.Module
{
    public class T_R_WO_REGION : DataObjectTable
    {
        public T_R_WO_REGION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_REGION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_REGION);
            TableName = "R_WO_REGION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public bool CheckDataExist(string wo_no, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO='{wo_no}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }
        public List<R_WO_REGION> ShowAllData(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            sql = $@"SELECT * FROM R_WO_REGION order by EDIT_TIME";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }
        public List<R_WO_REGION> GetWObyWONO(string WO, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO='{WO}'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }

        //对查询整个表的数据进行分页
        public List<R_WO_REGION> ShowAllDataAndShowPage(OleExec DB,string strWorkOrder,int CurrentPage, int PageSize, out int TotalData ) 
        {
            string strSql = string.Empty;
            bool isGetAll = true;
            DataTable dt = new DataTable();
            OleDbParameter[] paramet;
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            strSql = $@" select count(*) from r_wo_region a ";

            if (strWorkOrder.Length > 0)
            {
                strSql = strSql + $@"where upper(a.workorderno) like'%{strWorkOrder}%'";
                isGetAll = false;
            }
            TotalData = Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text));
            strSql = $@"select * from (select rownum rnumber,a.* from r_wo_region a ";
            if (isGetAll)
            {
                strSql = strSql + " order by edit_time desc)  where rnumber>((:CurrentPage-1)*:PageSize) and rnumber<=((:CurrentPage1-1)*:PageSize1+:PageSize2) order by edit_time desc";
                //oldb 的參數只能是按照順序對應，不能復用，
                paramet = new OleDbParameter[] {
                    new OleDbParameter(":CurrentPage", CurrentPage),
                    new OleDbParameter(":PageSize", PageSize),
                    new OleDbParameter(":CurrentPage1", CurrentPage),
                    new OleDbParameter(":PageSize1", PageSize),
                    new OleDbParameter(":PageSize2", PageSize)
                };
                dt = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            }
            else
            {
                strSql = strSql + $@" where  upper(a.workorderno) like'%{strWorkOrder}%' order by edit_time desc) where rnumber>((:CurrentPage-1)*:PageSize) and rnumber<=((:CurrentPage1-1)*:PageSize1+:PageSize2) order by edit_time desc";
                //oldb 的參數只能是按照順序對應，不能復用，
                paramet = new OleDbParameter[] {
                    new OleDbParameter(":CurrentPage", CurrentPage),
                    new OleDbParameter(":PageSize", PageSize),
                    new OleDbParameter(":CurrentPage1", CurrentPage),
                    new OleDbParameter(":PageSize1", PageSize),
                    new OleDbParameter(":PageSize2", PageSize)
                };
                dt = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            }

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Row_R_WO_REGION ret = (Row_R_WO_REGION)NewRow();
                    ret.loadData(dt.Rows[i]);
                    LanguageList.Add(ret.GetDataObject());
                }
                return LanguageList;
            }
            else
            {
                return null;
            }
        }

        public List<R_WO_REGION> CheckZone(string min, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            sql = $@"SELECT * FROM R_WO_REGION WHERE '{min}' BETWEEN MIN_SN AND MAX_SN";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }

        /// <summary>
        /// 查詢SN所在的工單區間
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_WO_REGION> ShowWORegionBySN(string sn, OleExec DB) {
            string sql = string.Empty;
            List<R_WO_REGION> WORegionList = new List<R_WO_REGION>();
            DataTable dt = new DataTable();
            sql = $@"Select * From R_WO_Region 
                     Where 1=1 and :strStartSN>=Min_SN and :strEndSN<=Max_SN and 
                     Length(:strStartSN)=Length(Min_SN)";
            OleDbParameter[] parameter = new OleDbParameter[3];          
            parameter[0] = new OleDbParameter(":strStartSN", sn);
            parameter[1] = new OleDbParameter(":strEndSN", sn);
            parameter[2] = new OleDbParameter(":strStartSN", sn);
            dt = DB.ExecuteDataTable(sql, CommandType.Text, parameter);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Row_R_WO_REGION ret = (Row_R_WO_REGION)NewRow();
                    ret.loadData(dt.Rows[i]);
                    WORegionList.Add(ret.GetDataObject());
                }                
            }
            return WORegionList;
        }

        /// <summary>
        /// 檢查SN是否在工單SN區間，存在返回TRUE，不存在返回FALSE
        /// 黄杨盛 2018年4月14日16:18:38 修正条码字数长度小于预配区间条码字数长度的时候会返回true的bug
        /// Eden 2018年4月27日16:49:38未配置工單區間返回FALSE
        /// </summary>
        /// <param name="strSN"></param>
        /// <param name="strWo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckSNInWoRange(string strSN,string strWo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO='{strWo}'";
            dt = DB.ExecSelect(sql, null).Tables[0];
            if (dt.Rows.Count > 0)
            {

                //sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO=:strwo AND :strsn BETWEEN MIN_SN AND MAX_SN AND LENGTH(:strsn)<=LENGTH(MAX_SN)";
                //OleDbParameter[] paramet = new OleDbParameter[3];
                //paramet[0] = new OleDbParameter(":strwo", strWo);
                //paramet[1] = new OleDbParameter(":strsn", strSN);
                //paramet[2] = new OleDbParameter(":strsn", strSN);
                //dt = DB.ExecuteDataTable(sql, CommandType.Text, paramet);
                sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO='{strWo}' AND '{strSN}' BETWEEN MIN_SN AND MAX_SN AND (LENGTH('{strSN}')=LENGTH(MAX_SN) and LENGTH('{strSN}')=LENGTH(MIN_SN) )";
                dt = DB.ExecuteDataTable(sql, CommandType.Text);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
          
        }
        /// <summary>
        /// 檢查StartSN&EndSN是否在工單區間內，存在返回TRUE，不存在返回FALSE
        /// 2018/1/23 Rain
        /// </summary>
        /// <param name="strStartSN"></param>
        /// <param name="strEndSN"></param>
        /// <param name="strWo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckLotSNInWoRange(string strStartSN, string strEndSN,string strWo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"Select * From R_WO_Region 
                     Where Workorderno=:strwo and :strStartSN>=Min_SN and :strEndSN<=Max_SN and 
                     Length(:strStartSN)=Length(Min_SN)";
            OleDbParameter[] parameter = new OleDbParameter[4];
            parameter[0] = new OleDbParameter(":strwo", strWo);
            parameter[1] = new OleDbParameter(":strStartSN", strStartSN);
            parameter[2] = new OleDbParameter(":strEndSN", strEndSN);
            parameter[3] = new OleDbParameter(":strStartSN", strStartSN);
            dt = DB.ExecuteDataTable(sql, CommandType.Text, parameter);
            if (dt.Rows.Count>0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 條碼后4位流水碼為34進制
        /// 取得條碼StartSN&EndSN區間內包含的序號數量
        /// 2018/1/24 Rain
        /// </summary>
        /// <param name="strStartSN"></param>
        /// <param name="strEndSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int GetQtyBy34HSNRange(string strStartSN, string strEndSN,  OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" SELECT 
      ((case   when (ascii(substr(:strEndSN,1,1)) between 48 and 57) then to_number(substr(:strEndSN,1,1)) 
              when (ascii(substr(:strEndSN,1,1)) between 65 and 72) then ascii(substr(:strEndSN,1,1))-55
              when (ascii(substr(:strEndSN,1,1)) between 74 and 78) then ascii(substr(:strEndSN,1,1))-56
              when (ascii(substr(:strEndSN,1,1)) between 80 and 90) then ascii(substr(:strEndSN,1,1))-57 else 0 end)*34*34*34+
      (case   when (ascii(substr(:strEndSN,2,1)) between 48 and 57) then to_number(substr(:strEndSN,2,1)) 
              when (ascii(substr(:strEndSN,2,1)) between 65 and 72) then ascii(substr(:strEndSN,2,1))-55
              when (ascii(substr(:strEndSN,2,1)) between 74 and 78) then ascii(substr(:strEndSN,2,1))-56
              when (ascii(substr(:strEndSN,2,1)) between 80 and 90) then ascii(substr(:strEndSN,2,1))-57 else 0 end)*34*34+
      (case   when (ascii(substr(:strEndSN,3,1)) between 48 and 57) then to_number(substr(:strEndSN,3,1)) 
              when (ascii(substr(:strEndSN,3,1)) between 65 and 72) then ascii(substr(:strEndSN,3,1))-55
              when (ascii(substr(:strEndSN,3,1)) between 74 and 78) then ascii(substr(:strEndSN,3,1))-56
              when (ascii(substr(:strEndSN,3,1)) between 80 and 90) then ascii(substr(:strEndSN,3,1))-57 else 0 end)*34+
      (case   when (ascii(substr(:strEndSN,4,1)) between 48 and 57) then to_number(substr(:strEndSN,4,1)) 
              when (ascii(substr(:strEndSN,4,1)) between 65 and 72) then ascii(substr(:strEndSN,4,1))-55
              when (ascii(substr(:strEndSN,4,1)) between 74 and 78) then ascii(substr(:strEndSN,4,1))-56
              when (ascii(substr(:strEndSN,4,1)) between 80 and 90) then ascii(substr(:strEndSN,4,1))-57 else 0 end)) -
      ((case   when (ascii(substr(:strStartSN,1,1)) between 48 and 57) then to_number(substr(:strStartSN,1,1)) 
              when (ascii(substr(:strStartSN,1,1)) between 65 and 72) then ascii(substr(:strStartSN,1,1))-55
              when (ascii(substr(:strStartSN,1,1)) between 74 and 78) then ascii(substr(:strStartSN,1,1))-56
              when (ascii(substr(:strStartSN,1,1)) between 80 and 90) then ascii(substr(:strStartSN,1,1))-57 else 0 end)*34*34*34+
      (case   when (ascii(substr(:strStartSN,2,1)) between 48 and 57) then to_number(substr(:strStartSN,2,1)) 
              when (ascii(substr(:strStartSN,2,1)) between 65 and 72) then ascii(substr(:strStartSN,2,1))-55
              when (ascii(substr(:strStartSN,2,1)) between 74 and 78) then ascii(substr(:strStartSN,2,1))-56
              when (ascii(substr(:strStartSN,2,1)) between 80 and 90) then ascii(substr(:strStartSN,2,1))-57 else 0 end)*34*34+
      (case   when (ascii(substr(:strStartSN,3,1)) between 48 and 57) then to_number(substr(:strStartSN,3,1)) 
              when (ascii(substr(:strStartSN,3,1)) between 65 and 72) then ascii(substr(:strStartSN,3,1))-55
              when (ascii(substr(:strStartSN,3,1)) between 74 and 78) then ascii(substr(:strStartSN,3,1))-56
              when (ascii(substr(:strStartSN,3,1)) between 80 and 90) then ascii(substr(:strStartSN,3,1))-57 else 0 end)*34+
      (case   when (ascii(substr(:strStartSN,4,1)) between 48 and 57) then to_number(substr(:strStartSN,4,1)) 
              when (ascii(substr(:strStartSN,4,1)) between 65 and 72) then ascii(substr(:strStartSN,4,1))-55
              when (ascii(substr(:strStartSN,4,1)) between 74 and 78) then ascii(substr(:strStartSN,4,1))-56
              when (ascii(substr(:strStartSN,4,1)) between 80 and 90) then ascii(substr(:strStartSN,4,1))-57 else 0 end)) +1  Qty          
      from dual";
            OleDbParameter[] parameter = new OleDbParameter[64];
            for (int i = 0; i <= 31; i++)
            {
                parameter[i] = new OleDbParameter(":strEndSN", strEndSN);
            }
            for (int i = 32; i <= 63; i++)
            {
                parameter[i] = new OleDbParameter(":strStartSN", strStartSN);
            }
            dt = DB.ExecuteDataTable(sql, CommandType.Text, parameter);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["Qty"].ToString());
            }
            else
            {
                return -1;
            }

        }

        public R_WO_REGION CreateLanguageClass(DataRow dr)
        {
            Row_R_WO_REGION row = (Row_R_WO_REGION)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        /// <summary>
        /// add by fgg 2018.05.17
        /// HWD PE 杜軍要求配置工單區間所輸入的字符必須是字母或數字
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="notMatch">out not match string</param>
        /// <returns>bool</returns>
        public bool InputIsStringOrNum(string input,out string notMatch)
        {
            string regexRule = "[A-Za-z0-9]";
            string outString = "";
            bool isStringOrNum = true;
            char[] inputArray = input.ToArray();
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (!Regex.IsMatch(inputArray[i].ToString(), regexRule))
                {                   
                    isStringOrNum = false;
                    outString = inputArray[i].ToString();
                    break;
                }
            }
            notMatch = outString;            
            return isStringOrNum;
        }

        /// <summary>
        ///  add by fgg 2018.05.17
        /// HWD PE 杜軍要求配置工單區間所輸入的SN必須符合機種設置的SN規則
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sku"></param>
        /// <returns></returns>
        public bool InputIsMatchSkuRule(string input,C_SKU sku)
        {
            bool isMatchSkuRule = true;
            string[] ruleArray;
            string[] snArray;
            char[] charConfigRule = sku.SN_RULE.ToCharArray();
            ruleArray = new string[charConfigRule.Length];

            if (sku.SN_RULE == "")
            {
                return true;
            }
            //長度不符
            if(input.Length!=sku.SN_RULE.Length)
            {
                return false;
            }

            for (int i = 0; i < charConfigRule.Length; i++)
            {
                ruleArray[i] = charConfigRule[i].ToString();
            }

            char[] charSn = input.ToCharArray();
            snArray = new string[charSn.Length];
            for (int j = 0; j < charSn.Length; j++)
            {
                snArray[j] = charSn[j].ToString();
            }

            for (int k = 0; k < ruleArray.Length; k++)
            {
                if (ruleArray[k] == "*")
                {
                    continue;
                }
                if (!ruleArray[k].Equals(snArray[k]))
                {
                    isMatchSkuRule = false;
                    break;
                }
            }            
            return isMatchSkuRule;
        }
    }
    public class Row_R_WO_REGION : DataObjectBase
    {
        public Row_R_WO_REGION(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_REGION GetDataObject()
        {
            R_WO_REGION DataObject = new R_WO_REGION();
            DataObject.ID = this.ID;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.QTY = this.QTY;
            DataObject.MIN_SN = this.MIN_SN;
            DataObject.MAX_SN = this.MAX_SN;
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
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string MIN_SN
        {
            get
            {
                return (string)this["MIN_SN"];
            }
            set
            {
                this["MIN_SN"] = value;
            }
        }
        public string MAX_SN
        {
            get
            {
                return (string)this["MAX_SN"];
            }
            set
            {
                this["MAX_SN"] = value;
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
    public class R_WO_REGION
    {
        public string ID;
        public string WORKORDERNO;
        public string SKUNO;
        public double? QTY;
        public string MIN_SN;
        public string MAX_SN;
        public string EDIT_EMP;
        public DateTime EDIT_TIME;
    }

    public class WoRangeMainPage
    {
        public List<R_WO_REGION> WoRangeData = new List<R_WO_REGION>();
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int CountPage { get; set; }
    }
}