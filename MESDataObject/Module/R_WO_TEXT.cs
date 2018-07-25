using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_TEXT : DataObjectTable
    {
        public T_R_WO_TEXT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_TEXT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_TEXT);
            TableName = "R_WO_TEXT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckWoTextByWo(string Workorderno, bool Download_Auto, string ColumnName, OleExec DB, DB_TYPE_ENUM DBType)
        {
            bool CheckFlag = false;
            string StrSql = "";
            string StrReturnMsg = "";
            int n = 0;

            if (Download_Auto)
            {
                StrSql = $@"select * from R_WO_TEXT where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    CheckFlag = true;
                }
            }
            else
            {
                StrSql = $@"select * from R_WO_TEXT where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    StrSql = $@"insert into H_WO_TEXT(ID,{ ColumnName }) ";
                    StrSql = StrSql + $@" select* from R_WO_TEXT where AUFNR = '{Workorderno}' ";
                    StrReturnMsg = DB.ExecSqlReturn(StrSql);

                    int.TryParse(StrReturnMsg, out n);
                    if (n > 1)
                    {
                        StrSql = $@" delete from R_WO_TEXT where AUFNR = '{Workorderno}' ";
                        StrReturnMsg = DB.ExecSQL(StrSql);

                        int.TryParse(StrReturnMsg, out n);
                        CheckFlag = false;
                    }

                }
            }

            return CheckFlag;
        }

        public bool CheckWoTextByWo(string Workorderno, bool Download_Auto,OleExec DB)
        {
            bool CheckFlag = false;
            string StrSql = "";
            string StrReturnMsg = "";
            int n = 0;

            if (Download_Auto)
            {
                StrSql = $@"select * from R_WO_TEXT where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    CheckFlag = true;
                }
            }
            else
            {
                StrSql = $@"select * from R_WO_TEXT where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    StrSql = $@"insert into H_WO_TEXT select* from R_WO_TEXT where AUFNR = '{Workorderno}' ";
                    StrReturnMsg = DB.ExecSqlReturn(StrSql);

                    int.TryParse(StrReturnMsg, out n);
                    if (n > 1)
                    {
                        StrSql = $@" delete from R_WO_TEXT where AUFNR = '{Workorderno}' ";
                        StrReturnMsg = DB.ExecSQL(StrSql);

                        int.TryParse(StrReturnMsg, out n);
                        CheckFlag = false;
                    }

                }
            }

            return CheckFlag;
        }
        public string EditWoText(string EditSql, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string ReturnMsg = DB.ExecSQL(EditSql);

            return ReturnMsg;
        }
    }
    public class Row_R_WO_TEXT : DataObjectBase
    {
        public Row_R_WO_TEXT(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_TEXT GetDataObject()
        {
            R_WO_TEXT DataObject = new R_WO_TEXT();
            DataObject.ID = this.ID;
            DataObject.AUFNR = this.AUFNR;
            DataObject.MATNR = this.MATNR;
            DataObject.ARBPL = this.ARBPL;
            DataObject.LTXA1 = this.LTXA1;
            DataObject.ISAVD = this.ISAVD;
            DataObject.VORNR = this.VORNR;
            DataObject.MGVRG = this.MGVRG;
            DataObject.LMNGA = this.LMNGA;
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
        public string AUFNR
        {
            get
            {
                return (string)this["AUFNR"];
            }
            set
            {
                this["AUFNR"] = value;
            }
        }
        public string MATNR
        {
            get
            {
                return (string)this["MATNR"];
            }
            set
            {
                this["MATNR"] = value;
            }
        }
        public string ARBPL
        {
            get
            {
                return (string)this["ARBPL"];
            }
            set
            {
                this["ARBPL"] = value;
            }
        }
        public string LTXA1
        {
            get
            {
                return (string)this["LTXA1"];
            }
            set
            {
                this["LTXA1"] = value;
            }
        }
        public string ISAVD
        {
            get
            {
                return (string)this["ISAVD"];
            }
            set
            {
                this["ISAVD"] = value;
            }
        }
        public string VORNR
        {
            get
            {
                return (string)this["VORNR"];
            }
            set
            {
                this["VORNR"] = value;
            }
        }
        public string MGVRG
        {
            get
            {
                return (string)this["MGVRG"];
            }
            set
            {
                this["MGVRG"] = value;
            }
        }
        public string LMNGA
        {
            get
            {
                return (string)this["LMNGA"];
            }
            set
            {
                this["LMNGA"] = value;
            }
        }
    }
    public class R_WO_TEXT
    {
        public string ID;
        public string AUFNR;
        public string MATNR;
        public string ARBPL;
        public string LTXA1;
        public string ISAVD;
        public string VORNR;
        public string MGVRG;
        public string LMNGA;
    }
}