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
    public class T_C_CONTROL : DataObjectTable
    {
        public T_C_CONTROL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CONTROL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CONTROL);
            TableName = "C_CONTROL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_CONTROL GetControlByName(string controlName,OleExec db)
        {
            string strSql = $@" select * from c_control where control_name='{controlName}'";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_CONTROL result = new C_CONTROL();
            if (table.Rows.Count > 0)
            {
                Row_C_CONTROL ret = (Row_C_CONTROL)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }
    }
    public class Row_C_CONTROL : DataObjectBase
    {
        public Row_C_CONTROL(DataObjectInfo info) : base(info)
        {

        }
        public C_CONTROL GetDataObject()
        {
            C_CONTROL DataObject = new C_CONTROL();
            DataObject.ID = this.ID;
            DataObject.CONTROL_NAME = this.CONTROL_NAME;
            DataObject.CONTROL_VALUE = this.CONTROL_VALUE;
            DataObject.CONTROL_TYPE = this.CONTROL_TYPE;
            DataObject.CONTROL_LEVEL = this.CONTROL_LEVEL;
            DataObject.CONTROL_DESC = this.CONTROL_DESC;
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
        public string CONTROL_NAME
        {
            get
            {
                return (string)this["CONTROL_NAME"];
            }
            set
            {
                this["CONTROL_NAME"] = value;
            }
        }
        public string CONTROL_VALUE
        {
            get
            {
                return (string)this["CONTROL_VALUE"];
            }
            set
            {
                this["CONTROL_VALUE"] = value;
            }
        }
        public string CONTROL_TYPE
        {
            get
            {
                return (string)this["CONTROL_TYPE"];
            }
            set
            {
                this["CONTROL_TYPE"] = value;
            }
        }
        public string CONTROL_LEVEL
        {
            get
            {
                return (string)this["CONTROL_LEVEL"];
            }
            set
            {
                this["CONTROL_LEVEL"] = value;
            }
        }
        public string CONTROL_DESC
        {
            get
            {
                return (string)this["CONTROL_DESC"];
            }
            set
            {
                this["CONTROL_DESC"] = value;
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
    public class C_CONTROL
    {
        public string ID;
        public string CONTROL_NAME;
        public string CONTROL_VALUE;
        public string CONTROL_TYPE;
        public string CONTROL_LEVEL;
        public string CONTROL_DESC;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}