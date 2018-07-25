using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_ACTION_CODE : DataObjectTable
    {
        public T_C_ACTION_CODE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ACTION_CODE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ACTION_CODE);
            TableName = "C_ACTION_CODE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public C_ACTION_CODE GetByActionCode(string ActionCode, OleExec DB)
        {
            string strSql = $@"select * from c_action_code where action_code=:ActionCode";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":ActionCode", ActionCode) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_ACTION_CODE ret = (Row_C_ACTION_CODE)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public int AddNewActionCode(C_ACTION_CODE NewActionCode, OleExec DB)
        {
            Row_C_ACTION_CODE NewActionCodeRow = (Row_C_ACTION_CODE)NewRow();
            NewActionCodeRow.ID = NewActionCode.ID;
            NewActionCodeRow.ACTION_CODE = NewActionCode.ACTION_CODE;
            NewActionCodeRow.ENGLISH_DESCRIPTION = NewActionCode.ENGLISH_DESCRIPTION;
            NewActionCodeRow.CHINESE_DESCRIPTION = NewActionCode.CHINESE_DESCRIPTION;
            NewActionCodeRow.EDIT_EMP = NewActionCode.EDIT_EMP;
            NewActionCodeRow.EDIT_TIME = NewActionCode.EDIT_TIME;
            int result = DB.ExecuteNonQuery(NewActionCodeRow.GetInsertString(DBType), CommandType.Text);
            return result;
        }
        public int UpdateById(C_ACTION_CODE NewActionCode, OleExec DB)
        {
            Row_C_ACTION_CODE NewActionCodeRow = (Row_C_ACTION_CODE)NewRow();
            NewActionCodeRow.ID = NewActionCode.ID;
            NewActionCodeRow.ACTION_CODE = NewActionCode.ACTION_CODE;
            NewActionCodeRow.ENGLISH_DESCRIPTION = NewActionCode.ENGLISH_DESCRIPTION;
            NewActionCodeRow.CHINESE_DESCRIPTION = NewActionCode.CHINESE_DESCRIPTION;
            NewActionCodeRow.EDIT_EMP = NewActionCode.EDIT_EMP;
            NewActionCodeRow.EDIT_TIME = NewActionCode.EDIT_TIME;
            int result = DB.ExecuteNonQuery(NewActionCodeRow.GetUpdateString(DBType, NewActionCode.ID), CommandType.Text);
            return result;
        }
        public C_ACTION_CODE GetByid(string id, OleExec DB)
        {
            string strSql = $@"select * from c_action_code where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_C_ACTION_CODE ret = (Row_C_ACTION_CODE)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public List<C_ACTION_CODE> GetByFuzzySearch(string ParametValue, OleExec DB)
        {
            string strSql = $@"select * from c_action_code where upper(action_code) like'%{ParametValue}%' or upper(english_description) like'%{ParametValue}%' or upper(chinese_description) like'%{ParametValue}%'";
            List<C_ACTION_CODE> result = new List<C_ACTION_CODE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ACTION_CODE ret = (Row_C_ACTION_CODE)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public List<C_ACTION_CODE> GetAllActionCode(OleExec DB)
        {
            string strSql = $@"select * from c_action_code ";
            List<C_ACTION_CODE> result = new List<C_ACTION_CODE>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_ACTION_CODE ret = (Row_C_ACTION_CODE)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public int DeleteById(string Id, OleExec DB)
        {
            string strSql = $@"delete c_action_code where id=:Id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };         
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text,paramet);
            return result;
        }
    }
    public class Row_C_ACTION_CODE : DataObjectBase
    {
        public Row_C_ACTION_CODE(DataObjectInfo info) : base(info)
        {

        }
        public C_ACTION_CODE GetDataObject()
        {
            C_ACTION_CODE DataObject = new C_ACTION_CODE();
            DataObject.ID = this.ID;
            DataObject.ACTION_CODE = this.ACTION_CODE;
            DataObject.ENGLISH_DESCRIPTION = this.ENGLISH_DESCRIPTION;
            DataObject.CHINESE_DESCRIPTION = this.CHINESE_DESCRIPTION;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string ACTION_CODE
        {
            get
            {
                return (string)this["ACTION_CODE"];
            }
            set
            {
                this["ACTION_CODE"] = value;
            }
        }
        public string ENGLISH_DESCRIPTION
        {
            get
            {
                return (string)this["ENGLISH_DESCRIPTION"];
            }
            set
            {
                this["ENGLISH_DESCRIPTION"] = value;
            }
        }
        public string CHINESE_DESCRIPTION
        {
            get
            {
                return (string)this["CHINESE_DESCRIPTION"];
            }
            set
            {
                this["CHINESE_DESCRIPTION"] = value;
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
    }
    public class C_ACTION_CODE
    {
        public string ID;
        public string ACTION_CODE;
        public string ENGLISH_DESCRIPTION;
        public string CHINESE_DESCRIPTION;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}