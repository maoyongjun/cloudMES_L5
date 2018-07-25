using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_REPLACE_SN : DataObjectTable
    {
        public T_R_REPLACE_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPLACE_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPLACE_SN);
            TableName = "R_REPLACE_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int AddReplaceSNRecord(R_REPLACE_SN ReplaceSn,string Bu, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;
            Row_R_REPLACE_SN row = null;


            if(ReplaceSn != null)
            {
                row = (Row_R_REPLACE_SN)ConstructRow(ReplaceSn);
                if(row.ID==null)
                {
                    row.ID = GetNewID(Bu, DB);
                }
                strSql = row.GetInsertString(DBType);
                result = DB.ExecSqlNoReturn(strSql, null);
            }

            return result;
        }
    }
    public class Row_R_REPLACE_SN : DataObjectBase
    {
        public Row_R_REPLACE_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_REPLACE_SN GetDataObject()
        {
            R_REPLACE_SN DataObject = new R_REPLACE_SN();
            DataObject.ID = this.ID;
            DataObject.OLD_SN_ID = this.OLD_SN_ID;
            DataObject.OLD_SN = this.OLD_SN;
            DataObject.NEW_SN = this.NEW_SN;
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
        public string OLD_SN_ID
        {
            get
            {
                return (string)this["OLD_SN_ID"];
            }
            set
            {
                this["OLD_SN_ID"] = value;
            }
        }
        public string OLD_SN
        {
            get
            {
                return (string)this["OLD_SN"];
            }
            set
            {
                this["OLD_SN"] = value;
            }
        }
        public string NEW_SN
        {
            get
            {
                return (string)this["NEW_SN"];
            }
            set
            {
                this["NEW_SN"] = value;
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
    public class R_REPLACE_SN
    {
        public string ID;
        public string OLD_SN_ID;
        public string OLD_SN;
        public string NEW_SN;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}